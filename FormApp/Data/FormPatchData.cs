namespace FormApp.Data;

public class FormPatchData
{
    
    public string Title { set; get; }
    public ICollection<QuestionsPatchData> Questions { set; get; }
}