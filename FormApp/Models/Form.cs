namespace FormApp.Models;

public class Form
{
    public int Id { set; get; }
    public int UserId { set; get; }
    public string Title { set; get; }
    public string URL { set; get; }
    public bool IsOpen { set; get; }
    
    public virtual User User { get; set; }
    public virtual ICollection<Question> Questions { get; set; }
}