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
            throw new DbUpdateException("Error while creating topic", ex);
        }
    }

    public async Task<TopicEditInputModel> GetTopicForEdit(Guid topicId)
    {
        Topic? topic = await topicRepo
            .SingleOrDefaultAsync(s => s.Id == topicId, ignoreQueryFilters: true);

        if (topic == null)
        {
            throw new InvalidOperationException($"Topic does not exist id: {topicId}");
        }

        TopicEditInputModel model = new TopicEditInputModel()
        {
            Id = topicId,
            Name = topic.Name,
            Description = topic.Description,
        };

        return model;
    }
    public async Task EditTopicAsync(TopicEditInputModel model)
    {
        Topic? topic = await topicRepo
            .SingleOrDefaultAsync(s => s.Id == model.Id, asNoTracking: false, ignoreQueryFilters: true);

        if (topic == null)
        {
            throw new InvalidOperationException($"Topic does not exist id: {model.Id}");
        }

        topic.Name = model.Name;
        topic.Description = model.Description;

        try
        {
            await topicRepo.SaveChangesAsync(); 
        }
        catch (Exception ex)
        {
            throw new DbUpdateException($"Could not edit topic. Id: {model.Id}");
        }
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
