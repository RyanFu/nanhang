﻿using ZHXY.Domain.Entity;
using System.Data.Entity.ModelConfiguration;

namespace ZHXY.Mapping
{
    public class AttendanceStuCountMap : EntityTypeConfiguration<AttendanceStuCount>
    {
        public AttendanceStuCountMap()
        {
            ToTable("School_Attendance_Stu_Count");
            HasKey(t => t.F_Id);
            HasOptional(t => t.School_Students_Entity)
                .WithMany()
                .HasForeignKey(t => t.F_Student)
                .WillCascadeOnDelete(false);
        }
    }
}