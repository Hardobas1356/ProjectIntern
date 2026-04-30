namespace ProjectIntern.Web.ViewModels.Admin.Topic;

public class TopicAdminViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid SpecialityId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int Order { get; set; }
}
