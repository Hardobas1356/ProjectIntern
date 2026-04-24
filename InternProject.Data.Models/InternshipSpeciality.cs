namespace InternSolution.Data.Models
{
    public class InternshipSpeciality
    {
        public Guid InternshipSpecialityID { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Intern> Interns { get; set; }
            = new HashSet<Intern>();
        public virtual ICollection<Topic> Topics { get; set; }
            = new HashSet<Topic>();
    }
}
