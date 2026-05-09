using ProjectIntern.Web.ViewModels.Admin.ApplicationUser;
using ProjectIntern.Web.ViewModels.ApplicationUser;

namespace ProjectIntern.Services.Core.Interfaces;

public interface IWorkDayService
{
    public Task<InternCalendarAdminViewModel> GetWorkDaysForInternAsAdminAsync(Guid internId);
    public Task<InternCalendarViewModel> GetWorkDaysForInternAsync(Guid internId);
    public Task CreateWorkDayAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId);
    public Task DeleteWorkDaysAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId);
}
