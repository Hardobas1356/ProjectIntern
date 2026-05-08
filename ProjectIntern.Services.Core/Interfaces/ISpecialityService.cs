using ForumApp.GCommon;
using InternSolution.Data.Models;
using ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

namespace ProjectIntern.Services.Core.Interfaces;

public interface ISpecialityService
{
    public Task SoftDeleteSpecialityAsync(Guid id);
    public Task RestoreSpecialityAsync(Guid id); 
    public Task<InternshipSpecialityDetailsViewModel> GetSpecialityDetailsAsync(Guid id, bool includeDeletedTopics);
    public Task<PaginatedResult<InternshipSpecialityViewModel>> GetAllSpecialitiesAsync(int pageNumber, int pageSize, string? searchTerm, bool includeDeleted);
    public Task CreateSpecialityAsync(InternshipSpecialityCreateInputModel inputModel);
    public Task<InternshipSpecialityEditInputModel> GetSpecialityForEditAsync(Guid id);
    public Task EditSpecialityAsync(InternshipSpecialityEditInputModel inputModel);
    public Task ReorderTopicsAsync(Guid specialityId, List<Guid> orderedTopicIds);

}
