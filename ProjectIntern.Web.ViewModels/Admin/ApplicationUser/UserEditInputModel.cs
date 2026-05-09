using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ProjectIntern.Web.ViewModels.Admin.ApplicationUser;

public class UserEditInputModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(150)]
    public string? University { get; set; }

    [Display(Name = "Speciality")]
    public Guid? InternshipSpecialityId { get; set; }
    public string? InternshipSpecialityName { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime? InternshipStartDate { get; set; }
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? InternshipEndDate { get; set; }
    [Display(Name = "Has Completed Curriculum")]
    public bool HasCompletedCurriculum { get; set; } = false;
    public IEnumerable<SelectListItem> Specialities { get; set; } = new HashSet<SelectListItem>();
}
