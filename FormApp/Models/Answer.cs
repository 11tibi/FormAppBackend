namespace FormApp.Models;

public class Answer
{
    public int Id { set; get; }
    public int ResponseId { set; get; }
    public int QuestionId { set; get; }
    
    public virtual Response Response { set; get; }
    public virtual Question Question { set; get; }
    public virtual ICollection<SelectedOption> SelectedOptions { set; get; }
}