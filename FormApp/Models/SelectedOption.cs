namespace FormApp.Models;

public class SelectedOption
{
    public int Id { set; get; }
    public int OptionId { set; get; }
    public int AnswerId { set; get; }

    public virtual Options Option { set; get; }
    public virtual Answer Answer { set; get; }
}