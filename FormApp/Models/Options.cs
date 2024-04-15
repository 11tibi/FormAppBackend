namespace FormApp.Models;

public class Options
{
    public int Id { set; get; }
    public int QuestionId { set; get; }
    public string OptionText { set; get; }
    
    ///
    public virtual Question Question { set; get; } ///
    public virtual ICollection<SelectedOption> SelectedOptions { set; get; }
}