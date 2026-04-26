using ProjectIntern.Web.ViewModels.Topic;

namespace ProjectIntern.Services.Core.Interfaces;

public interface ITopicService
{
    public Task CreateTopicAsync(TopicCreateInputModel model, Guid specialityId);
    public Task SoftDeleteTopicAsync(Guid topicId);
    public Task RestoreTopicAsync(Guid topicId);
    public Task<TopicAdminViewModel> GetSpecialityDetailsAsync(Guid id);
    public Task EditTopicAsync(TopicEditInputModel model);
}
