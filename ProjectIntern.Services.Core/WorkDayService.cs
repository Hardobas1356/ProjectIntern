using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Services.Core.Repository.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.WorkDay;

namespace ProjectIntern.Services.Core;

public class WorkDayService : IWorkDayService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IGenericRepository<WorkDayAssignment> workDayAssignmentRepository;

    public WorkDayService(UserManager<ApplicationUser> userManager, IGenericRepository<WorkDayAssignment> workDayAssignmentRepository)
    {
        this.userManager = userManager;
        this.workDayAssignmentRepository = workDayAssignmentRepository;
    }

    public async Task CreateWorkDayAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId)
    {
        ApplicationUser applicationUser = await userManager
            .Users
            .Where(u => u.Id == internId)
            .Include(u => u.InternshipSpeciality)
                .ThenInclude(s => s!.Topics.Where(t => !t.IsDeleted))
            .Include(u => u.LastAssignedTopic)
            .FirstOrDefaultAsync()
            ?? throw new ArgumentException($"Intern with ID {internId} not found.");

        if (applicationUser.InternshipSpecialityId == null)
            throw new InvalidOperationException("Intern has no speciality assigned.");

        if (applicationUser.HasCompletedCurriculum)
            throw new InvalidOperationException("Intern has completed the curriculum. No new days can be added.");

        List<DateTime> sortedDates = dates
            .Select(d => d.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (applicationUser.InternshipStartDate.HasValue)
        {
            DateTime start = applicationUser.InternshipStartDate.Value.Date;
            if (sortedDates.Any(d => d < start))
                throw new InvalidOperationException("One or more dates are before the intern's start date.");
        }

        if (applicationUser.InternshipEndDate.HasValue)
        {
            DateTime end = applicationUser.InternshipEndDate.Value.Date;
            if (sortedDates.Any(d => d > end))
                throw new InvalidOperationException("One or more dates are after the intern's end date.");
        }

        List<WorkDayAssignment> existingWorkDays = await workDayAssignmentRepository
            .GetQueryable(asNoTracking: false, ignoreQueryFilters: true)
            .Where(w => w.InternId == internId
                        && sortedDates.Contains(w.Date))
            .ToListAsync();

        List<Topic> topics = applicationUser.InternshipSpeciality!.Topics
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Order)
            .ToList();

        if (!topics.Any())
            throw new InvalidOperationException("No active topics in this speciality.");

        List<DateTime> alreadyActiveDates = existingWorkDays
            .Where(w => !w.IsDeleted)
            .Select(w => w.Date)
            .ToList();

        if (alreadyActiveDates.Any())
            throw new InvalidOperationException(
                $"The following dates already have work days assigned: {string.Join(", ", alreadyActiveDates.Select(d => d.ToString("yyyy-MM-dd")))}");

        int lastOrder = applicationUser.LastAssignedTopic?.Order ?? -1;
        int newDatesCount = sortedDates
            .Count(d => !existingWorkDays.Any(w => w.Date == d));
        int remainingTopics = topics.Count(t => t.Order > lastOrder);

        if (newDatesCount > remainingTopics)
            throw new InvalidOperationException(
                $"Not enough topics remaining. {remainingTopics} topics left but {newDatesCount} new days requested.");

        foreach (DateTime date in sortedDates)
        {
            WorkDayAssignment? existing = existingWorkDays
                .FirstOrDefault(w => w.Date == date);

            if (existing != null)
            {
                // keep original topic to preserve progress, but "undelete" if it was previously deleted
                existing.IsDeleted = false;
                existing.CreatedByUserId = adminId;
                existing.CreatedAt = DateTime.UtcNow;
                continue;
            }

            Topic? nextTopic = topics.FirstOrDefault(t => t.Order > lastOrder);

            WorkDayAssignment newWorkDay = new WorkDayAssignment
            {
                Id = Guid.NewGuid(),
                Date = date,
                InternId = internId,
                TopicId = nextTopic.Id,
                CreatedByUserId = adminId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
            };

            await workDayAssignmentRepository.AddAsync(newWorkDay);

            lastOrder = nextTopic.Order;
            applicationUser.LastAssignedTopicId = nextTopic.Id;

            if (topics.Last().Id == nextTopic.Id)
            {
                applicationUser.HasCompletedCurriculum = true;
            }
        }

        await userManager.UpdateAsync(applicationUser);

        try
        {
            await workDayAssignmentRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DbUpdateException("An error occurred while saving work days.", ex);
        }
    }

    public async Task DeleteWorkDaysAsync(Guid internId, IEnumerable<DateTime> dates, Guid adminId)
    {
        ApplicationUser? applicationUser = await CheckUserExistsWithSpecialityIncluded(internId);

        List<DateTime> normalizedDates = dates
            .Select(d => d.Date)
            .Distinct()
            .ToList();

        var workDayAssignmentsToDelete = await workDayAssignmentRepository
            .GetQueryable(asNoTracking: false)
            .Where(w => w.InternId == internId && normalizedDates.Contains(w.Date))
            .ToListAsync();

        foreach (var workDayAssignment in workDayAssignmentsToDelete)
        {
            workDayAssignment.IsDeleted = true;
        }

        try
        {
            await workDayAssignmentRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DbUpdateException("An error occurred while deleting work days.", ex);
        }
    }

    public async Task<InternCalendarAdminViewModel> GetWorkDaysForInternAsync(Guid internId)
    {
        ApplicationUser? applicationUser = await CheckUserExistsWithSpecialityIncluded(internId);

        IOrderedQueryable<WorkDayAssignment> query = workDayAssignmentRepository
            .GetQueryable()
            .Where(w => w.InternId == internId)
            .Include(w => w.Topic)
            .OrderBy(w => w.Date);

        InternCalendarAdminViewModel result = new InternCalendarAdminViewModel
        {
            InternId = internId,
            InternName = applicationUser.Name,
            SpecialityName = applicationUser.InternshipSpeciality?.Name ?? "N/A",
            InternshipStartDate = applicationUser.InternshipStartDate,
            InternshipEndDate = applicationUser.InternshipEndDate,
            HasCompletedCurriculum = applicationUser.HasCompletedCurriculum,
            WorkDays = await query.Select(w => new WorkDayViewModel
            {
                Id = w.Id,
                Date = w.Date,
                TopicName = w.Topic.Name,
                TopicDescription = w.Topic.Description
            }).ToListAsync()
        };

        return result;
    }

    private async Task<ApplicationUser> CheckUserExistsWithSpecialityIncluded(Guid internId)
    {
        var applicationUser = await userManager
             .Users
             .IgnoreQueryFilters()
             .Where(u => u.Id == internId)
             .Include(u => u.InternshipSpeciality)
             .FirstOrDefaultAsync();

        if (applicationUser == null)
        {
            throw new ArgumentException($"Intern with ID {internId} not found.");
        }

        return applicationUser;
    }
}
