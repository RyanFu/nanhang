﻿using System.Data.Entity.ModelConfiguration;
using ZHXY.Domain.Entity;

namespace ZHXY.Mapping
{
    public class StuHolidayLimitMap : EntityTypeConfiguration<StuHolidayLimit>
    {
        public StuHolidayLimitMap()
        {
            ToTable("School_StudentHolidayLimit");

            HasKey(p => new { p.SemesterId, p.StudentId });

            Property(p => p.SemesterId).HasColumnName("F_SemesterId");
            Property(p => p.StudentId).HasColumnName("F_StudentId");
            Property(p => p.UsedDays).HasColumnName("F_UsedDays");

        }
    }
}