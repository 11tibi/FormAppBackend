using System.Security.Claims;
using FormApp.Data;
using FormApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormApp.Controllers;

[Authorize]
[ApiController]
[Route("Api/Response")]
public class ResponseController(IResponseService responseService) : ControllerBase
{
    /// <summary>
    /// Returns all the responses associated to a specific form 
    /// </summary>
    /// <param name="url">The form URL</param>
    /// <returns>A list with responses</returns>
    [HttpGet("Form/{url}")]
    public async Task<IActionResult> GetResponsesUser(string url)
    {
        return Ok(await responseService.GetAllFormResponsesAsync(url));
    }

    /// <summary>
    /// Return an existing response
    /// </summary>
    /// <param name="responseId">The response id</param>
    /// <returns>A response</returns>
    [HttpGet("{responseId}")]
    public async Task<IActionResult> GetResponse(int responseId)
    {
        return Ok(await responseService.GetResponseAsync(responseId));
    }

    /// <summary>
    /// Creates a response
    /// </summary>
    /// <param name="responseCreateData">The response form data</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateResponse([FromBody] ResponseCreateData responseCreateData)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new InvalidOperationException());
        await responseService.CreateResponseAsync(responseCreateData, userId);
        return Created();
    }

    /// <summary>
    /// Deletes an existing response
    /// </summary>
    /// <param name="responseId">The id of the Response</param>
    /// <returns></returns>
    [HttpDelete("{responseId}")]
    public async Task<IActionResult> DeleteResponse(int responseId)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new InvalidOperationException());
        await responseService.DeleteResponseAsync(responseId, userId);
        return Ok();
    }
}