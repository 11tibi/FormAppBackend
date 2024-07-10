using FormApp.Data;
using FormApp.Models;

namespace FormApp.Services;

public interface IResponseService
{
    Task<List<Response>> GetAllFormResponsesAsync(string url);
    Task<Response> GetResponseAsync(int responseId);
    Task CreateResponseAsync(ResponseCreateData responseCreateData, int userId);
    Task DeleteResponseAsync(int formId, int userId);
}