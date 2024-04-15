namespace FormApp.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string GoogleSubject { get; set; }
    
    public virtual ICollection<Form> Forms { get; set; }
}