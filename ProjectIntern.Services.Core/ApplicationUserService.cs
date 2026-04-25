using ForumApp.GCommon;
using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.ApplicationUser;
using ProjectIntern.Web.ViewModels.ApplicationUser;
using Microsoft.EntityFrameworkCore;

using static ForumApp.GCommon.GlobalConstants;

namespace ProjectIntern.Services.Core;

public class ApplicationUserService : IApplicationUserService
{
    private readonly UserManager<ApplicationUser> userManager;

    public ApplicationUserService(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public Task EditUserAsync(UserEditInputModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedResult<UserAdminViewModel>> GetAllUsersAdminAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        IQueryable<ApplicationUser> query = userManager.Users
            .Include(u => u.InternshipSpeciality)
            .AsNoTracking()
            .IgnoreQueryFilters();

        if (!String.IsNullOrWhiteSpace(searchTerm))
        {
            string loweredSearchTerm = searchTerm.ToLower();

            query = query
                .Where(u => !String.IsNullOrWhiteSpace(u.Name)
                    && (u.NormalizedUserName!.Contains(loweredSearchTerm)
                        || u.NormalizedEmail!.Contains(loweredSearchTerm))
                        || u.Name!.ToLower().Contains(loweredSearchTerm));
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
                LastAssignmentOrder = u.LastAssignmentOrder,
                IsDeleted = u.IsDeleted,
            });

        return await PaginatedResult<UserAdminViewModel>.CreateAsync(users, pageNumber, pageSize);
    }

    public Task<UserEditInputModel> GetUserForEditAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task MakeAdminAsync(Guid id)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    public Task RemoveAdminAsync(Guid id, Guid actingUserId)
    {
        throw new NotImplementedException();
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
        ApplicationUser? user = await userManager.FindByIdAsync(id.ToString());
        if (user != null && !user.IsDeleted)
        {
            user.IsDeleted = true;
            await userManager.UpdateAsync(user);
        }
    }
}
