namespace ProjectIntern.Web.ViewModels.Admin.ApplicationUser;

public class UserAdminViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CreationDate { get; set; } = null!;
    public string InternshipStartDate { get; set; } = null!;
    public string InternshipEndDate { get; set; } = null!;
    public string University { get; set; } = null!;
    public string InternshipSpeciality { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public int CompletedTopicsCount { get; set; } = 0;
    public bool HasCompletedCurriculum { get; set; } = false;
}
