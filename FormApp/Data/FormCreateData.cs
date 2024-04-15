using FormApp.Models;

namespace FormApp.Data;

public class FormCreateData
{
    public string Title { set; get; }
    public ICollection<QuestionsData> Questions { set; get; }
}
