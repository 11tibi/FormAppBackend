using System.Security.Claims;
using FormApp.Data;
using FormApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormApp.Controllers;

[Authorize]
[ApiController]
[Route("Api/Response")]
public class ResponseController : ControllerBase
{
    private readonly ILogger<ResponseController> _logger;
    private readonly AppDbContext _context;

    public ResponseController(ILogger<ResponseController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Returns all the responses associated to a specific form 
    /// </summary>
    /// <param name="url">The form URL</param>
    /// <returns></returns>
    [HttpGet("Form/{url}")]
    public async Task<IActionResult> GetResponsesUser(string url)
    {
        var responses = await _context.Responses
            .Include(r => r.User)
            .Where(r => r.Form.URL == url)
            .ToListAsync();
        return Ok(responses);
    }

    /// <summary>
    /// Return a response
    /// </summary>
    /// <param name="id">The response id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetResponse(int id)
    {
        var response = await _context.Responses
            .Include(r => r.Form)
            .Include(r => r.Answers)
            .ThenInclude(r => r.SelectedOptions)
            .FirstAsync(r => r.Id == id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResponse([FromBody] ResponseCreateData responseData)
    {
        var form = await _context.Forms.FirstAsync(f => f.Id == responseData.FormId);
        if (!form.IsOpen)
        {
            return BadRequest();
        }

        var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            var response = new Response
            {
                UserId = currentUserId,
                FormId = responseData.FormId,
                SubmittedAd = DateTime.UtcNow
            };
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
            foreach (var answers in responseData.Answers)
            {
                var newAnswer = new Answer
                {
                    ResponseId = response.Id,
                    QuestionId = answers.QuestionId
                };

                _context.Answers.Add(newAnswer);
                await _context.SaveChangesAsync();


                foreach (var option in answers.SelectedOptions)
                {
                    _logger.LogWarning(option.ToString());
                    var opt = await _context.Options
                        .Include(o => o.Question)
                        .ThenInclude(q => q.Form)
                        .Where(o => o.Id == option && o.Question.Form.Id == responseData.FormId)
                        .FirstAsync();
                    var newSelectedOption = new SelectedOption
                    {
                        OptionId = option,
                        AnswerId = newAnswer.Id
                    };
                    _context.SelectedOptions.Add(newSelectedOption);
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }

        return Created();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(int id)
    {
        var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var response = await _context.Responses
            .Include(r => r.Form)
            .FirstAsync(r => r.Id == id && r.Form.UserId == currentUserId);
        _context.Remove(response);
        await _context.SaveChangesAsync();
        return Ok();
    }
}