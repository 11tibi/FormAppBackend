namespace FormApp.Data;

public class QuestionsData
{
    public int Id { get; set; }
    public List<OptionsData> Choices { set; get; }
    public int QuestionType { set; get; }
    public string QuestionText { set; get; }
    public bool IsRequired { set; get; }
    public uint SequenceNumber { set; get; } 
}