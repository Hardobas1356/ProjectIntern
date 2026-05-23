using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectIntern.Web.ViewModels.Admin.ApplicationUser;

public class UserCreateAdminInputModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [MaxLength(150)]
    public string? University { get; set; }

    [Display(Name = "Speciality")]
    public Guid? InternshipSpecialityId { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime? InternshipStartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? InternshipEndDate { get; set; }

    public IEnumerable<SelectListItem> Specialities { get; set; } = new List<SelectListItem>();
}
