using InternSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InternProject.Data.Configurations
{
    public class InternshipSpecialityConfiguration : IEntityTypeConfiguration<InternshipSpeciality>
    {
        public void Configure(EntityTypeBuilder<InternshipSpeciality> builder)
        {
            builder.HasKey(s => s.InternshipSpecialityID);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

            builder.HasIndex(s => s.Name)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0"); // Only enforce uniqueness on non-deleted items

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
