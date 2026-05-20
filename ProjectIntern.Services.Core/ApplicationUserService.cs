using ForumApp.GCommon;
using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.ApplicationUser;
using Microsoft.EntityFrameworkCore;

using static ForumApp.GCommon.GlobalConstants;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectIntern.Services.Core.Repository.Interfaces;

namespace ProjectIntern.Services.Core;

public class ApplicationUserService : IApplicationUserService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IGenericRepository<InternshipSpeciality> internSpecialityRepo;

    public ApplicationUserService(UserManager<ApplicationUser> userManager,
        IGenericRepository<InternshipSpeciality> internSpecialityRepo)
    {
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.internSpecialityRepo = internSpecialityRepo ?? throw new ArgumentNullException(nameof(internSpecialityRepo));
    }

    public async Task EditUserAsync(UserEditInputModel model)
    {
        ApplicationUser? user = await userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {model.Id} not found.");
        }

        if (user.InternshipSpecialityId != model.InternshipSpecialityId)
        {
            user.InternshipSpecialityId = model.InternshipSpecialityId;
            user.LastAssignedTopicId = null;
            user.HasCompletedCurriculum = false;
            user.CompletedTopicsCount = 0;
        }

        user.Name = model.Name;
        user.University = model.University;
        user.InternshipSpecialityId = model.InternshipSpecialityId;
        user.InternshipStartDate = model.InternshipStartDate.HasValue
                ? DateTime.SpecifyKind(model.InternshipStartDate.Value, DateTimeKind.Utc)
                : null;
        user.HasCompletedCurriculum = model.HasCompletedCurriculum;

        user.InternshipEndDate = model.InternshipEndDate.HasValue
            ? DateTime.SpecifyKind(model.InternshipEndDate.Value, DateTimeKind.Utc)
            : null;

        IdentityResult result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update user: {errors}");
        }
    }

    public async Task<PaginatedResult<UserAdminViewModel>> GetAllUsersAdminAsync(int pageNumber, int pageSize, string? searchTerm, bool showDeleted = false)
    {
        IQueryable<ApplicationUser> query = userManager.Users
            .Include(u => u.InternshipSpeciality)
            .OrderBy(u => u.Name)
            .AsNoTracking()
            .IgnoreQueryFilters();

        if (!showDeleted)
        {
            query = query.Where(u => !u.IsDeleted);
        }

        if (!String.IsNullOrWhiteSpace(searchTerm))
        {
            string loweredSearchTerm = searchTerm.ToLower();

            query = query.Where(u =>
                        (u.Name != null && u.Name.ToLower().Contains(loweredSearchTerm)) ||
                        (u.NormalizedUserName != null && u.NormalizedUserName.Contains(loweredSearchTerm.ToUpper())) ||
                        (u.NormalizedEmail != null && u.NormalizedEmail.Contains(loweredSearchTerm.ToUpper()))
                    );
        }

        IQueryable<UserAdminViewModel> users = query
            .Select(u => new UserAdminViewModel
            {
                Id = u.Id,
                Name = u.Name == null
                              ? DeletedUser.DELETED_DISPLAYNAME : u.Name,

                CreationDate = u.CreationDate.ToString(APPLICATION_DATE_TIME_FORMAT),
                InternshipStartDate = u.InternshipStartDate.HasValue
                    ? u.InternshipStartDate.Value.ToString(APPLICATION_DATE_TIME_FORMAT)
                    : "N/A",
                InternshipEndDate = u.InternshipEndDate.HasValue
                    ? u.InternshipEndDate.Value.ToString(APPLICATION_DATE_TIME_FORMAT)
                    : "N/A",

                University = u.University ?? "N/A",
                InternshipSpeciality = u.InternshipSpeciality != null
                    ? u.InternshipSpeciality.Name
                    : "No Speciality (Admin/Unassigned)",
                IsDeleted = u.IsDeleted,
                CompletedTopicsCount = u.CompletedTopicsCount,
                HasCompletedCurriculum = u.HasCompletedCurriculum,
            });

        return await PaginatedResult<UserAdminViewModel>.CreateAsync(users, pageNumber, pageSize);
    }

    public async Task<UserEditInputModel> GetUserForEditAsync(Guid id)
    {
        ApplicationUser? user = await userManager.Users
                .Include(u => u.InternshipSpeciality)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        IEnumerable<InternshipSpeciality> specialities = await internSpecialityRepo.GetAllAsync(asNoTracking: true);

        UserEditInputModel model = new UserEditInputModel
        {
            Id = user.Id,
            Name = user.Name,
            University = user.University,
            InternshipSpecialityId = user.InternshipSpecialityId,
            InternshipSpecialityName = user.InternshipSpeciality?.Name,
            InternshipStartDate = user.InternshipStartDate,
            InternshipEndDate = user.InternshipEndDate,
            HasCompletedCurriculum = user.HasCompletedCurriculum,
            IsAdmin = userManager.IsInRoleAsync(user, "Admin").Result,
            IsDeleted = user.IsDeleted,
            Specialities = specialities.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = s.Id == user.InternshipSpecialityId
            }).ToList()
        };

        return model;
    }

    public async Task HardDeleteUserAsync(Guid id)
    {
        ApplicationUser? user = await userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        IdentityResult result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to delete user: {errors}");
        }
    }

    public async Task MakeAdminAsync(Guid id)
    {
        ApplicationUser? user = await userManager
            .FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        if (user != null)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    public async Task RemoveAdminAsync(Guid id, Guid actingUserId)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        if (id == actingUserId)
            throw new InvalidOperationException("You cannot remove your own admin role.");

        if (user != null)
        {
            await userManager.RemoveFromRoleAsync(user, "Admin");
        }
    }

    public async Task RestoreUserAsync(Guid id)
    {
        ApplicationUser? user = await userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user != null && user.IsDeleted)
        {
            user.IsDeleted = false;
            await userManager.UpdateAsync(user);
        }
    }


    public async Task SoftDeleteUserAsync(Guid id)
    {
        ApplicationUser? user = await userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user != null && !user.IsDeleted)
        {
            user.IsDeleted = true;
            await userManager.UpdateAsync(user);
        }
    }
}
