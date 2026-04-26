namespace InternSolution.Data.Models
{
    public class WorkDayAssignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Guid InternId { get; set; }
        public ApplicationUser Intern { get; set; } = null!;
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
    }
}
