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

    public async Task CreateSpecialityAsync(InternshipSpecialityCreateInputModel inputModel)
    {
        if (inputModel == null)
            throw new ArgumentNullException(nameof(inputModel));

        if (string.IsNullOrWhiteSpace(inputModel.Name))
            throw new ArgumentException("Speciality name is required", nameof(inputModel.Name));

        if (string.IsNullOrWhiteSpace(inputModel.Description))
            throw new ArgumentException("Speciality description is required", nameof(inputModel.Description));

        InternshipSpeciality speciality = new InternshipSpeciality()
        {
            Name = inputModel.Name.Trim(),
            Description = inputModel.Description.Trim(),
            IsDeleted = false,
        };

        try
        {
            await specialityRepo.AddAsync(speciality);
            await specialityRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Could not save the speciality to the database.", ex);
        }
    }

    public async Task<PaginatedResult<InternshipSpecialityViewModel>> GetAllSpecialitiesAsync(int pageNumber, int pageSize, string? searchTerm, bool includeDeleted)
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

    public Task<InternshipSpecialityDetailsViewModel> GetSpecialityDetailsAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RestoreSpecialityAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteSpecialityAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
