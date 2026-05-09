namespace ProjectIntern.Web.ViewModels.Admin.Requests;

public class WorkDayBatchRequestModel
{
    public Guid InternId { get; set; }
    public List<string> Dates { get; set; } = new();
}
