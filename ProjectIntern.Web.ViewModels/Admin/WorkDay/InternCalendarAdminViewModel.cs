namespace ProjectIntern.Web.ViewModels.Admin.WorkDay;

public class InternCalendarAdminViewModel
{
    public Guid InternId { get; set; }
    public string InternName { get; set; } = null!;
    public string SpecialityName { get; set; } = null!;
    public DateTime? InternshipStartDate { get; set; }
    public DateTime? InternshipEndDate { get; set; }
    public bool HasCompletedCurriculum { get; set; }
    public IEnumerable<WorkDayViewModel> WorkDays { get; set; } = new HashSet<WorkDayViewModel>();
}
