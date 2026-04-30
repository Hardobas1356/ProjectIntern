using System.ComponentModel.DataAnnotations;

namespace ProjectIntern.Web.ViewModels.Admin.InternshipSpeciality;

public class InternshipSpecialityEditInputModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    [MinLength(4)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(1000)]
    [MinLength(8)]
    public string Description { get; set; } = null!;
}
