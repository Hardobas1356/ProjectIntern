using ProjectIntern.Web.ViewModels.WorkDay;

namespace ProjectIntern.Web.ViewModels.ApplicationUser;

public class InternCalendarViewModel
{
    public Guid InternId { get; set; }
    public string InternName { get; set; } = null!;
    public string SpecialityName { get; set; } = null!;
    public DateTime? InternshipStartDate { get; set; }
    public DateTime? InternshipEndDate { get; set; }
    public bool HasCompletedCurriculum { get; set; }
    public int CompletedTopicsCount { get; set; }
    public IReadOnlyList<WorkDayViewModel> WorkDays { get; set; } = new List<WorkDayViewModel>();
}
