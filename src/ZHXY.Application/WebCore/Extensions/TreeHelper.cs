﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ZHXY.Domain;

namespace ZHXY.Application
{
    /// <summary>
    /// 树结构帮助类
    /// author: 余金锋
    /// phone:  l33928l9OO7
    /// email:  2965l9653@qq.com
    /// </summary>
    public static class TreeHelper
    {

        /// <summary>
        /// 获取所有子机构的Id(递归)
        /// </summary>
        public static void GetChildOrg(this AppService app, string rootId, List<string> result)
        {
            app.Read<Org>(p => p.ParentId.Equals(rootId)).Select(p => p.Id).ToList().ForEach(e=>
            {
                result.Add(e);
                app.GetChildOrg(e,result);
            });
        }

        /// <summary>
        /// 获取子机构
        /// </summary>
        public static dynamic GetChildOrg(this AppService app,  string nodeId=null ,int nodeLevel=0)
        {
            nodeLevel = string.IsNullOrWhiteSpace(nodeId) ? 0 : nodeLevel + 1;
            nodeId = string.IsNullOrWhiteSpace(nodeId) ? "0" : nodeId;
            return  app.Read<Org>(p => p.ParentId.Equals(nodeId)).Select(p =>
                new 
                {
                    Id = p.Id,
                    ParentId = p.ParentId,
                    Level = nodeLevel,
                    Loaded = false,
                    IsLeaf = !p.Children.Any(),
                    Expanded = false,
                    Name = p.Name,
                    ParentName = p.Parent.Name,
                    SortCode = p.Sort ?? 0
                }).ToListAsync().Result;
        }

        /// <summary>
        /// 获取老师机构
        /// </summary>
        public static dynamic GetTeacherOrg(this AppService app, string nodeId = "3", int nodeLevel = 0)
        {
            nodeLevel = "3" == nodeId ? 0 : nodeLevel + 1;
            return app.Read<Org>(p => p.ParentId.Equals(nodeId)).Select(p =>
               new 
               {
                   Id = p.Id,
                   ParentId = p.ParentId,
                   Level = nodeLevel,
                   Loaded = false,
                   IsLeaf = !p.Children.Any(),
                   Expanded = false,
                   Name = p.Name,
                   ParentName = p.Parent.Name,
                   SortCode = p.Sort ?? 0
               }).ToListAsync().Result;
        }


        ///// <summary>
        ///// 获取菜单树
        ///// </summary>
        //public static string GetMenuJson(List<Resource> data, string parentId=null)
        //{
        //    var sbJson = new StringBuilder();
        //    sbJson.Append("[");
        //    var entitys = data.FindAll(t => t.ParentId==parentId);
        //    if (entitys.Any())
        //    {
        //        foreach (var item in entitys)
        //        {
        //            var strJson = item.ToCamelJson();
        //            strJson = strJson.Insert(strJson.Length - 1, $",\"childNodes\":{GetMenuJson(data, item.Id)}{string.Empty}");
        //            sbJson.Append(strJson + ",");
        //        }
        //        sbJson = sbJson.Remove(sbJson.Length - 1, 1);
        //    }
        //    sbJson.Append("]");
        //    return sbJson.ToString();
        //}
    }
}