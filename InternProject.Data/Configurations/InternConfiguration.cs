using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternProject.Data.Configurations
{
    public class InternConfiguration : IEntityTypeConfiguration<Intern>
    {
        public void Configure(EntityTypeBuilder<Intern> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.University)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(i => i.CreationDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.LastAssignmentOrder)
                .HasDefaultValue(0);

            builder.HasQueryFilter(i => !i.IsDeleted);

            builder.HasOne(i => i.InternshipSpeciality)
                .WithMany(s => s.Interns)
                .HasForeignKey(i => i.InternshipSpecialityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.WorkDayAssignments)
                .WithOne(wa => wa.Intern)
                .HasForeignKey(wa => wa.InternId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

