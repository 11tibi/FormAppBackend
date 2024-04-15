namespace FormApp.Data;

public class QuestionsPatchData
{
    public int Id { get; set; }
    public List<OptionsData> Choices { set; get; }
    public int QuestionType { set; get; }
    public string QuestionText { set; get; }
    public bool IsRequired { set; get; }
    public uint SequenceNumber { set; get; } 
    public string Op { set; get; }
}