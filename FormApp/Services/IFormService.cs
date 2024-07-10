using FormApp.Data;
using FormApp.Models;

namespace FormApp.Services;

public interface IFormService
{
    Task<List<Form>> GetUserFormAsync(int userId);
    Task<Form> GetFormAsync(string url);
    Task CreateFormAsync(FormCreateData formCreateData, int userId);
    Task DeleteFormAsync(int formId, int userId);
    Task UpdateFormAsync(int formId, int userId, FormPatchData formData);
}