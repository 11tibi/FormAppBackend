using FormApp.Data;
using FormApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FormApp.Services;

public class FormService(AppDbContext context) : IFormService
{
    public async Task<List<Form>> GetUserFormAsync(int userId)
    {
        return await context.Forms.Where(f => f.UserId == userId).ToListAsync();
    }

    public async Task<Form> GetFormAsync(string url)
    {
        return await context.Forms
            .Include(f => f.Questions.OrderBy(q => q.Id))
            .ThenInclude(q => q.Options)
            .FirstAsync(f => f.URL == url);
    }

    public async Task CreateFormAsync(FormCreateData formCreateData, int userId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var form = new Form
        {
            UserId = userId,
            Title = formCreateData.Title,
            URL = Guid.NewGuid().ToString(),
            IsOpen = true
        };
        context.Forms.Add(form);
        await context.SaveChangesAsync();
        foreach (var question in formCreateData.Questions)
        {
            if (question.Choices.Count == 0)
            {
                throw new ArgumentException("Question must have at least one argument");
            }

            var newQuestion = new Question
            {
                FormId = form.Id,
                QuestionTypeId = question.QuestionType,
                QuestionText = question.QuestionText,
                IsRequired = question.IsRequired,
                SequenceNumber = question.SequenceNumber
            };
            context.Questions.Add(newQuestion);
            await context.SaveChangesAsync();
            foreach (var questionChoice in question.Choices)
            {
                context.Options.Add(new Options
                {
                    QuestionId = newQuestion.Id,
                    OptionText = questionChoice.OptionText
                });
                await context.SaveChangesAsync();
            }
        }

        await transaction.CommitAsync();
    }

    public async Task DeleteFormAsync(int formId, int userId)
    {
        var form = await context.Forms.FirstAsync(f => f.Id == formId && f.UserId == userId);
        context.Forms.Remove(form);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFormAsync(int formId, int userId, FormPatchData formData)
    {
        var form = await context.Forms.FirstAsync(f => f.Id == formId);
        if (form.UserId != userId)
        {
            throw new ArgumentException("Form not found or user is not authorized.");
        }

        form.Title = formData.Title;

        await using var transaction = await context.Database.BeginTransactionAsync();
        foreach (var question in formData.Questions)
        {
            if (question.Choices.Count == 0)
            {
                throw new ArgumentException("Question must have at least one choice.");
            }

            if (question.Op == "create")
            {
                var newQuestion = new Question
                {
                    FormId = form.Id,
                    QuestionTypeId = question.QuestionType,
                    QuestionText = question.QuestionText,
                    IsRequired = question.IsRequired,
                    SequenceNumber = question.SequenceNumber
                };
                context.Questions.Add(newQuestion);
                await context.SaveChangesAsync();
                foreach (var questionChoice in question.Choices)
                {
                    context.Options.Add(new Options
                    {
                        QuestionId = newQuestion.Id,
                        OptionText = questionChoice.OptionText
                    });
                }

                await context.SaveChangesAsync();
            }
            else
            {
                var currentQuestion = await context.Questions
                    .FirstAsync(q => q.Id == question.Id && q.FormId == form.Id);
                currentQuestion.QuestionText = question.QuestionText;
                currentQuestion.IsRequired = question.IsRequired;

                foreach (var option in question.Choices)
                {
                    var currentOption = await context.Options
                        .FirstAsync(o => o.Id == option.Id && o.QuestionId == currentQuestion.Id);
                    currentOption.OptionText = option.OptionText;
                }

                await context.SaveChangesAsync();
            }
        }

        await transaction.CommitAsync();
    }
}