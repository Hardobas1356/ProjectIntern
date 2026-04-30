using ProjectIntern.Web.ViewModels.Admin.Topic;

namespace ProjectIntern.Services.Core.Interfaces;

public interface ITopicService
{
    public Task CreateTopicAsync(TopicCreateInputModel model, Guid specialityId);
    public Task SoftDeleteTopicAsync(Guid topicId);
    public Task RestoreTopicAsync(Guid topicId);
    public Task<IEnumerable<TopicAdminViewModel>> GetAllTopicsForSpecialityAsync(Guid specialityId, bool includeDeleted);
    public Task EditTopicAsync(TopicEditInputModel model);
}
