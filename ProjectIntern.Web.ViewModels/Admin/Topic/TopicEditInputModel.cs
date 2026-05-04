using System.ComponentModel.DataAnnotations;

namespace ProjectIntern.Web.ViewModels.Admin.Topic;

public class TopicEditInputModel
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(4)]
    [MaxLength(150)]
    public string Name { get; set; } = null!;
    [Required]
    [MinLength(8)]
    [MaxLength(2000)]
    public string Description { get; set; } = null!;
}
