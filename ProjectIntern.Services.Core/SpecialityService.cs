using ForumApp.GCommon;
using InternSolution.Data.Models;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Services.Core.Repository.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

namespace ProjectIntern.Services.Core;

public class SpecialityService : ISpecialityService
{
    private readonly IGenericRepository<InternshipSpeciality> specialityRepo;

    public SpecialityService(IGenericRepository<InternshipSpeciality> specialityRepo)
    {
        this.specialityRepo = specialityRepo;
    }

    public async Task<PaginatedResult<InternshipSpecialityViewModel>> GetAllSpecialities(int pageNumber, int pageSize, string? searchTerm, bool includeDeleted)
    {
        IQueryable<InternshipSpeciality> query = specialityRepo
            .GetQueryable(true, ignoreQueryFilters: includeDeleted);
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            string loweredSearchTerm = searchTerm.ToLower();

            query = query.Where(s => s.Name != null && s.Name.ToLower().Contains(loweredSearchTerm));
        }

        IQueryable<InternshipSpecialityViewModel> result = query
            .OrderBy(s => s.Name)
            .Select(s => new InternshipSpecialityViewModel
            {
                Id = s.InternshipSpecialityID,
                Name = s.Name,
                IsDeleted = s.IsDeleted
            });

        return await PaginatedResult<InternshipSpecialityViewModel>.CreateAsync(result, pageNumber, pageSize);
    }

    public Task<InternshipSpecialityDetailsViewModel> GetSpecialityDetails(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RestoreSpeciality(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteSpeciality(Guid id)
    {
        throw new NotImplementedException();
    }
}
