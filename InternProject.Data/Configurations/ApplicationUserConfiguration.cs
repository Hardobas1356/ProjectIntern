using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternProject.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.University)
            .HasMaxLength(150);

        builder.Property(i => i.CreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");


        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasOne(i => i.InternshipSpeciality)
            .WithMany(s => s.Interns)
            .HasForeignKey(i => i.InternshipSpecialityId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.LastAssignedTopic)
            .WithMany(t => t.ApplicationUsers)
            .HasForeignKey(u => u.LastAssignedTopicId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasIndex(u => u.LastAssignedTopicId)
            .HasDatabaseName("IX_ApplicationUser_LastAssignedTopicId");
    }

}

