using ForumApp.GCommon;
using InternSolution.Data.Models;
using ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

namespace ProjectIntern.Services.Core.Interfaces;

public interface ISpecialityService
{
    public Task SoftDeleteSpeciality(Guid id);
    public Task RestoreSpeciality(Guid id); 
    public Task<InternshipSpecialityDetailsViewModel> GetSpecialityDetails(Guid id);
    public Task<PaginatedResult<InternshipSpecialityViewModel>> GetAllSpecialities(int pageNumber, int pageSize, string? searchTerm, bool includeDeleted);

}
