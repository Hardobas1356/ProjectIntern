namespace ProjectIntern.Web.ViewModels.Admin.Requests;

public class ReorderRequest
{
    public Guid SpecialityId { get; set; }
    public List<Guid>? TopicIds { get; set; }
}
