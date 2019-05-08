﻿using System.Data.Entity.ModelConfiguration;

namespace ZHXY.Domain
{
    public class RoleAuthorizeMap : EntityTypeConfiguration<RoleAuthorize>
    {
        public RoleAuthorizeMap()
        {
            ToTable("Sys_RoleAuthorize");
            HasKey(t => t.F_Id);
        }
    }
}