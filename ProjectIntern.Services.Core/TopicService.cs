using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Services.Core.Repository.Interfaces;
using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Services.Core;

public class TopicService : ITopicService
{
    private readonly IGenericRepository<Topic> topicRepo;
    private readonly IGenericRepository<InternshipSpeciality> specialityService;

    public TopicService(IGenericRepository<Topic> topicRepo, IGenericRepository<InternshipSpeciality> specialityService)
    {
        this.topicRepo = topicRepo;
        this.specialityService = specialityService;
    }

    public async Task CreateTopicAsync(TopicCreateInputModel model, Guid specialityId)
    {
        InternshipSpeciality? speciality = await specialityService
            .GetQueryable()
            .FirstOrDefaultAsync(s => s.Id == specialityId);

        if (speciality == null)
        {
            throw new InvalidOperationException($"Speciality not found. Id: {specialityId}");
        }

        int nextOrder = await topicRepo.GetQueryable()
                .Where(t => t.InternshipSpecialityId == model.InternshipSpecialityId)
                .CountAsync();

        Topic topic = new Topic()
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Order = nextOrder,
            IsDeleted = false,
            InternshipSpecialityId = specialityId,
        };

        try
        {
            await topicRepo.AddAsync(topic);
            await topicRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error while creating topic", ex);
        }
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
