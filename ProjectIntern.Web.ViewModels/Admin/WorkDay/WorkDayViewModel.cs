namespace ProjectIntern.Web.ViewModels.Admin.WorkDay;

public class WorkDayViewModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string TopicName { get; set; } = null!;
    public string TopicDescription { get; set; } = null!;
}
