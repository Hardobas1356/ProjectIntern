using Microsoft.AspNetCore.Identity;

namespace InternSolution.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = null!;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? InternshipStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? InternshipEndDate { get; set; } = DateTime.UtcNow;
        public string? University { get; set; } = null!;
        public int LastAssignmentOrder { get; set; } = 0;    
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<WorkDayAssignment>? WorkDayAssignments { get; set; }
            = new HashSet<WorkDayAssignment>();
        public Guid? InternshipSpecialityId { get; set; }
        public InternshipSpeciality? InternshipSpeciality { get; set; }

    }
}
