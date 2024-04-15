using System.Security.Claims;
using FormApp.Data;
using FormApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormApp.Controllers;

[Authorize]
[ApiController]
[Route("Api/Form")]
public class FormController : ControllerBase
{
    private readonly ILogger<FormController> _logger;
    private readonly AppDbContext _context;

    public FormController(ILogger<FormController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserForms()
    {
        var user = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var forms = await _context.Forms.Where(f => f.UserId == user).ToListAsync();
        return Ok(forms);
    }

    [HttpGet("{url}")]
    public async Task<IActionResult> GetForm(string url)
    {
        var form = await _context.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstAsync(f => f.URL == url);

        return Ok(form);
    }

    [HttpPost]
    public async Task<IActionResult> PostForm([FromBody] FormCreateData formCreateData)
    {
        var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            var form = new Form
            {
                UserId = currentUserId,
                Title = formCreateData.Title,
                URL = Guid.NewGuid().ToString(),
                IsOpen = true
            };
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            foreach (var question in formCreateData.Questions)
            {
                if (question.Choices.Count == 0)
                {
                    return BadRequest();
                }

                var newQuestion = new Question
                {
                    FormId = form.Id,
                    QuestionTypeId = question.QuestionType,
                    QuestionText = question.QuestionText,
                    IsRequired = question.IsRequired,
                    SequenceNumber = question.SequenceNumber
                };
                _context.Questions.Add(newQuestion);
                await _context.SaveChangesAsync();
                foreach (var questionChoice in question.Choices)
                {
                    _context.Options.Add(new Options
                    {
                        QuestionId = newQuestion.Id,
                        OptionText = questionChoice.OptionText
                    });
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }

        return Created();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(int id)
    {
        var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var form = await _context.Forms.FirstAsync(f => f.Id == id && f.UserId == currentUserId);
        _context.Forms.Remove(form);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchForm(int id, [FromBody] FormPatchData formData)
    {
        var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var form = await _context.Forms.FirstAsync(f => f.Id == id);
        if (form.UserId != currentUserId)
        {
            return BadRequest();
        }

        form.Title = formData.Title;

        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            foreach (var question in formData.Questions)
            {
                if (question.Choices.Count == 0)
                {
                    return BadRequest();
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
                    _context.Questions.Add(newQuestion);
                    await _context.SaveChangesAsync();
                    foreach (var questionChoice in question.Choices)
                    {
                        _context.Options.Add(new Options
                        {
                            QuestionId = newQuestion.Id,
                            OptionText = questionChoice.OptionText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    var currentQuestion = await _context.Questions
                        .FirstAsync(q => q.Id == question.Id && q.FormId == form.Id);
                    currentQuestion.QuestionText = question.QuestionText;
                    currentQuestion.IsRequired = question.IsRequired;

                    foreach (var option in question.Choices)
                    {
                        var currentOption = await _context.Options
                            .FirstAsync(o => o.Id == option.Id && o.QuestionId == currentQuestion.Id);
                        currentOption.OptionText = option.OptionText;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }

        return Ok();
    }
}