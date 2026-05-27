using Microsoft.AspNetCore.Identity;

namespace ProjectIntern.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = null!;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? InternshipStartDate { get; set; } = null;
    public DateTime? InternshipEndDate { get; set; } = null;
    public string? University { get; set; } = null!;
    public int CompletedTopicsCount { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;


    public Guid? LastAssignedTopicId { get; set; }
    public Topic? LastAssignedTopic { get; set; }
    public Guid? InternshipSpecialityId { get; set; }
    public InternshipSpeciality? InternshipSpeciality { get; set; }
    public virtual ICollection<WorkDayAssignment>? WorkDayAssignments { get; set; }
    = new HashSet<WorkDayAssignment>();
}
