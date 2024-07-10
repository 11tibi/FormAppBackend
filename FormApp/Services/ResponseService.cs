using FormApp.Data;
using FormApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FormApp.Services;

public class ResponseService(AppDbContext context) : IResponseService
{
    public async Task<List<Response>> GetAllFormResponsesAsync(string url)
    {
        return await context.Responses
            .Include(r => r.User)
            .Where(r => r.Form.URL == url)
            .ToListAsync();
    }

    public async Task<Response> GetResponseAsync(int responseId)
    {
        return await context.Responses
            .Include(r => r.Form)
            .Include(r => r.Answers)
            .ThenInclude(r => r.SelectedOptions)
            .FirstAsync(r => r.Id == responseId);
    }

    public async Task CreateResponseAsync(ResponseCreateData responseCreateData, int userId)
    {
        var form = await context.Forms.FirstAsync(f => f.Id == responseCreateData.FormId);
        if (!form.IsOpen)
        {
            throw new ArgumentException("Form is not open.");
        }
        
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            var response = new Response
            {
                UserId = userId,
                FormId = responseCreateData.FormId,
                SubmittedAd = DateTime.UtcNow
            };
            context.Responses.Add(response);
            await context.SaveChangesAsync();
            foreach (var answers in responseCreateData.Answers)
            {
                var newAnswer = new Answer
                {
                    ResponseId = response.Id,
                    QuestionId = answers.QuestionId
                };

                context.Answers.Add(newAnswer);
                await context.SaveChangesAsync();


                foreach (var option in answers.SelectedOptions)
                {
                    var opt = await context.Options
                        .Include(o => o.Question)
                        .ThenInclude(q => q.Form)
                        .Where(o => o.Id == option && o.Question.Form.Id == responseCreateData.FormId)
                        .FirstAsync();
                    var newSelectedOption = new SelectedOption
                    {
                        OptionId = option,
                        AnswerId = newAnswer.Id
                    };
                    context.SelectedOptions.Add(newSelectedOption);
                    await context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }
    }

    public async Task DeleteResponseAsync(int formId, int userId)
    {
        var response = await context.Responses
            .Include(r => r.Form)
            .FirstAsync(r => r.Id == formId && r.Form.UserId == userId);
        context.Remove(response);
        await context.SaveChangesAsync();
    }
}