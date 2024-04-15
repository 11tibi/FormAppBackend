using FormApp.Models;

namespace FormApp.Data;

public class ResponseCreateData
{
    public int FormId { set; get; }
    public ICollection<AnswerData> Answers { set; get; }
}