using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

public class InternshipSpecialityDetailsViewModel 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public IEnumerable<TopicAdminViewModel> Topics { get; set; } = new HashSet<TopicAdminViewModel>();
}
