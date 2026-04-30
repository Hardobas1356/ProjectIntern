using ForumApp.GCommon;
using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Services.Core.Repository.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;
using ProjectIntern.Web.ViewModels.Admin.Topic;

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
                Id = s.Id,
                Name = s.Name,
                IsDeleted = s.IsDeleted
            });

        return await PaginatedResult<InternshipSpecialityViewModel>.CreateAsync(result, pageNumber, pageSize);
    }

    public async Task<InternshipSpecialityDetailsViewModel> GetSpecialityDetailsAsync(Guid id)
    {
        InternshipSpeciality? speciality = await specialityRepo.GetQueryable(asNoTracking: true, ignoreQueryFilters: true)
            .Include(s => s.Topics)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            throw new ArgumentException("Speciality not found");

        return new InternshipSpecialityDetailsViewModel
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description,
            IsDeleted = speciality.IsDeleted,
            Topics = speciality.Topics
                .OrderBy(t => t.Order)
                .Select(t => new TopicAdminViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Order = t.Order,
                    IsDeleted = t.IsDeleted
                })
        };
    }

    public async Task RestoreSpecialityAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("SpecialityId is null", nameof(id));
        }

        InternshipSpeciality? speciality = await specialityRepo
            .GetByIdAsync(id,
                          asNoTracking: false,
                          ignoreQueryFilters: true);

        if (speciality == null)
        {
            throw new ArgumentException("Speciality not found with Id", nameof(id));
        }

        try
        {
            speciality.IsDeleted = false;
            await specialityRepo.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Failed to restore speciality", e);
        }
    }

    public async Task SoftDeleteSpecialityAsync(Guid id)
    {
        InternshipSpeciality? speciality = await specialityRepo
            .GetByIdAsync(id, asNoTracking: false);

        if(speciality == null)
            throw new ArgumentException("Speciality not found", nameof(id));

        try
        {
            speciality.IsDeleted = true;
            await specialityRepo.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Failed to soft delete speciality", e);
        }
    }
}
