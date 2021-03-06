﻿using System;

namespace ZHXY.Domain
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : IEntity
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        //自定义数据权限机构字段
        public string DataDeps { get; set; }
        public int? Sort { get; set; }
    }
}