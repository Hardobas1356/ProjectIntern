using ProjectIntern.Web.ViewModels.Admin.WorkDay;

namespace ProjectIntern.Services.Core.Interfaces;

public interface IWorkDayService
{
    public Task<InternCalendarAdminViewModel> GetWorkDaysForInternAsync(Guid internId);
    public Task CreateWorkDayAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId);
    public Task DeleteWorkDaysAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId);
}
