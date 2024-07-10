using System.Security.Claims;
using FormApp.Data;
using FormApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormApp.Controllers;

[Authorize]
[ApiController]
[Route("Api/Form")]
public class FormController(IFormService formService) : ControllerBase
{
    /// <summary>
    /// Returns all responses associated with the currently logged in user
    /// </summary>
    /// <returns>A list of forms</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserForms()
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                               throw new InvalidOperationException());
        return Ok(await formService.GetUserFormAsync(userId));
    }

    /// <summary>
    /// Returns an existing form
    /// </summary>
    /// <param name="url">The form url</param>
    /// <returns>A form</returns>
    [HttpGet("{url}")]
    public async Task<IActionResult> GetForm(string url)
    {
        return Ok(await formService.GetFormAsync(url));
    }

    /// <summary>
    /// Created a from
    /// </summary>
    /// <param name="formCreateData">The form form data</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostForm([FromBody] FormCreateData formCreateData)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new InvalidOperationException());
        await formService.CreateFormAsync(formCreateData, userId);
        return Created();
    }

    /// <summary>
    /// Deletes an existing form
    /// </summary>
    /// <param name="formId">The id of the form</param>
    /// <returns></returns>
    [HttpDelete("{formId}")]
    public async Task<IActionResult> DeleteForm(int formId)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new InvalidOperationException());
        await formService.DeleteFormAsync(formId, userId);
        return Ok();
    }

    /// <summary>
    /// Updates an existing form
    /// </summary>
    /// <param name="formId">The id of the form</param>
    /// <param name="formData">The form data to be updated</param>
    /// <returns></returns>
    [HttpPatch("{formId}")]
    public async Task<IActionResult> PatchForm(int formId, [FromBody] FormPatchData formData)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new InvalidOperationException());
        await formService.UpdateFormAsync(formId, userId, formData);
        return Ok();
    }
}