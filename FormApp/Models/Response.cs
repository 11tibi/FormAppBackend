namespace FormApp.Models;

public class Response
{
    public int Id { set; get; }
    public int UserId { set; get; }
    public int FormId { set; get; }
    public DateTime SubmittedAd { get; set; }
    
    public virtual User User { set; get; }
    public virtual Form Form { set; get; }
    public virtual ICollection<Answer> Answers { set; get; }
}