using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Services.Core.Repository.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Services.Core;

public class TopicService : ITopicService
{
    private readonly IGenericRepository<Topic> topicRepo;

    public TopicService(IGenericRepository<Topic> topicRepo)
    {
        this.topicRepo = topicRepo;
    }

    public Task CreateTopicAsync(TopicCreateInputModel model, Guid specialityId)
    {
        throw new NotImplementedException();
    }

    public Task EditTopicAsync(TopicEditInputModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TopicAdminViewModel>> GetAllTopicsForSpecialityAsync(Guid specialityId, bool includeDeleted)
    {
        IQueryable<Topic> query = topicRepo
            .GetQueryable(asNoTracking: true, ignoreQueryFilters: includeDeleted);

        query = query.Where(t => t.Id == specialityId);

        TopicAdminViewModel[] result = await query
            .OrderBy(t => t.Order)
            .Select(t => new TopicAdminViewModel
            {
                Id = t.Id,
                Name = t.Name,
                SpecialityId = t.InternshipSpecialityId,
                IsDeleted = t.IsDeleted,
                Order = t.Order,
            })
            .ToArrayAsync();

        return result;
    }

    public Task RestoreTopicAsync(Guid topicId)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteTopicAsync(Guid topicId)
    {
        throw new NotImplementedException();
    }
}
