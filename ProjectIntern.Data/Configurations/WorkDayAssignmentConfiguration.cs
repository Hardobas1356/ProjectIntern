using ProjectIntern.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectIntern.Data.Configurations;

public class WorkDayAssignmentConfiguration : IEntityTypeConfiguration<WorkDayAssignment>
{
    public void Configure(EntityTypeBuilder<WorkDayAssignment> builder)
    {
        builder.HasKey(wa => wa.Id);

        builder.Property(wa => wa.Date)
            .IsRequired();

        builder.HasQueryFilter(wa => !wa.IsDeleted);

        builder.HasOne(wa => wa.Topic)
            .WithMany(t => t.WorkDayAssignments)
            .HasForeignKey(wa => wa.TopicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wa => wa.CreatedByUser)
            .WithMany()
            .HasForeignKey(wa => wa.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wa => wa.Intern)
            .WithMany(u => u.WorkDayAssignments)
            .HasForeignKey(wa => wa.InternId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(wa => new { wa.InternId, wa.Date })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE")
            .HasDatabaseName("IX_WorkDayAssignment_Intern_Date")
            .IncludeProperties(wa => wa.TopicId);

        builder.HasIndex(wa => wa.TopicId)
            .HasDatabaseName("IX_WorkDayAssignment_TopicId");

        builder.HasIndex(wa => wa.Date)
            .HasDatabaseName("IX_WorkDayAssignment_Date")
            .HasMethod("brin");

        builder.HasIndex(wa => wa.CreatedByUserId)
            .HasDatabaseName("IX_WorkDayAssignment_CreatedByUserId");
    }
}
