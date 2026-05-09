namespace ProjectIntern.Web.ViewModels.WorkDay;

public class WorkDayViewModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsRevealed { get; set; }
    public string? TopicName { get; set; }
    public string? TopicDescription { get; set; }
}
