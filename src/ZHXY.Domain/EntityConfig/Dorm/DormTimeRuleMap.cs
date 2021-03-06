﻿using System.Data.Entity.ModelConfiguration;

namespace ZHXY.Domain
{
    public class DormTimeRuleMap: EntityTypeConfiguration<DormTimeRule>
    {
        public DormTimeRuleMap()
        {
            ToTable("zhxy_dorm_rule");
            HasKey(p => p.DayOfWeek);

            Property(p => p.DayOfWeek).HasColumnName("day_of_week");
            Property(p => p.ClosedTime).HasColumnName("closed_time");
            Property(p => p.NotReturnLimitTime).HasColumnName("not_return_limit_time");
            Property(p => p.NotOutLimit).HasColumnName("not_out_limit");
        }
    }
}
