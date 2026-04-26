using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InternProject.Data.Configurations
{
    public class WorkDayAssignmentConfiguration : IEntityTypeConfiguration<WorkDayAssignment>
    {
        public void Configure(EntityTypeBuilder<WorkDayAssignment> builder)
        {
            builder.HasKey(wa => wa.Id);

            builder.Property(wa => wa.Date)
                .IsRequired();

            builder.HasQueryFilter(wa => !wa.IsDeleted);

            builder.HasIndex(wa => wa.Date)
                .HasDatabaseName("IX_WorkDayAssignment_Date");

            builder.HasIndex(wa => new { wa.InternId, wa.TopicId })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = FALSE");
        }
    }
}
