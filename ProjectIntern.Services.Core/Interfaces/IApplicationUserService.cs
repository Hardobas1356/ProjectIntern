using ForumApp.GCommon;
using ProjectIntern.Web.ViewModels.Admin.ApplicationUser;
using ProjectIntern.Web.ViewModels.ApplicationUser;

namespace ProjectIntern.Services.Core.Interfaces;

public interface IApplicationUserService
{
    public Task<PaginatedResult<UserAdminViewModel>> GetAllUsersAdminAsync(
        int pageNumber, int pageSize, string? searchTerm, bool showDeleted);
    public Task<UserEditInputModel> GetUserForEditAsync(Guid id);
    public Task SoftDeleteUserAsync(Guid id);
    public Task RestoreUserAsync(Guid id);
    public Task EditUserAsync(UserEditInputModel model);
    public Task MakeAdminAsync(Guid id);
    public Task RemoveAdminAsync(Guid id, Guid actingUserId);
}
