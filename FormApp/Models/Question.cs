namespace FormApp.Models;

public class Question
{
    public int Id { set; get; }
    public int FormId { set; get; }
    public int QuestionTypeId { set; get; }
    public string QuestionText { set; get; }
    public bool IsRequired { set; get; }
    public uint SequenceNumber { set; get; } 
    
    public virtual QuestionType QuestionType { set; get; }
    public virtual Form Form { set; get; } ////
    public virtual ICollection<Options> Options { set; get; }
}