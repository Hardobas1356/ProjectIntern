using ProjectIntern.Services.Core.Interfaces;
using ProjectIntern.Web.ViewModels.Topic;

namespace ProjectIntern.Services.Core;

public class TopicService : ITopicService
{
    public Task CreateTopicAsync(TopicCreateInputModel model, Guid specialityId)
    {
        throw new NotImplementedException();
    }

    public Task EditTopicAsync(TopicEditInputModel model)
    {
        throw new NotImplementedException();
    }

    public Task<TopicAdminViewModel> GetSpecialityDetailsAsync(Guid id)
    {
        throw new NotImplementedException();
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
