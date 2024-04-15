namespace FormApp.Data;

public class AnswerData
{
    public int QuestionId { set; get; }
    public ICollection<int> SelectedOptions { set; get; }
}