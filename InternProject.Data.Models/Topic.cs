namespace InternSolution.Data.Models
{
    public class Topic
    {
        public Guid TopicID { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Order { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Guid InternshipSpecialityId { get; set; }
        public InternshipSpeciality InternshipSpeciality { get; set; } = null!;
        public virtual ICollection<WorkDayAssignment> WorkDayAssignments { get; set; }
            = new HashSet<WorkDayAssignment>();
    }
}
