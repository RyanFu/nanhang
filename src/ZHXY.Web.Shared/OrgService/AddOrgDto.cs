﻿namespace ZHXY.Web.Shared
{
    public class AddOrgDto
    {
        public string ParentId { get; set; }
        public string EnCode { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CategoryId { get; set; }
        public int? SortCode { get; set; }

    }

}