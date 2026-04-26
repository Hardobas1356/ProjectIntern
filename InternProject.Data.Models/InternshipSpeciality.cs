namespace InternSolution.Data.Models
{
    public class InternshipSpeciality
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<ApplicationUser> Interns { get; set; }
            = new HashSet<ApplicationUser>();
        public virtual ICollection<Topic> Topics { get; set; }
            = new HashSet<Topic>();
    }
}
