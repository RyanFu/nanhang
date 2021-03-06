﻿using ZHXY.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using ZHXY.Common;
using System.Data.Entity;
using ZHXY.Web.Shared;

namespace ZHXY.Application
{
    /// <summary>
    /// 晚归报表服务
    /// </summary>
    public class LateReturnReportService: AppService
    {
        public LateReturnReportService(DbContext r) : base(r) { }
        public List<LateReturnReport> GetList(Pagination pagination,string startTime, string endTime, string classId)
        {
            pagination.Sord = "desc";
            pagination.Sidx = "CreatedTime";
            var expression = ExtLinq.True<LateReturnReport>();
            if (!string.IsNullOrEmpty(classId))
                expression = expression.And(p => p.Class.Equals(classId));
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = Convert.ToDateTime(startTime + " 00:00:00");
                expression = expression.And(p => p.CreatedTime >= start);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = Convert.ToDateTime(endTime + " 23:59:59");
                expression = expression.And(p => p.CreatedTime <= end);
            }
            return Read(expression).Paging(pagination).ToList();
        }

        //根据学生ID获取晚归记录
        public List<LateReturnReport> GetLateListByStuId(string studentId, string startTime, string endTime)
        {
            var expression = ExtLinq.True<LateReturnReport>();
            if (!string.IsNullOrEmpty(studentId))
                expression = expression.And(p => p.StudentId.Equals(studentId));
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = Convert.ToDateTime(startTime + " 00:00:00");
                expression = expression.And(p => p.CreatedTime >= start);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = Convert.ToDateTime(endTime + " 23:59:59");
                expression = expression.And(p => p.CreatedTime <= end);
            }
            return Read(expression).ToList();
        }

        public List<LateReturnReport> GetListByClass(string classId, string startTime, string endTime)
        {
            var expression = ExtLinq.True<LateReturnReport>();
            if (!string.IsNullOrEmpty(classId))
            {
                var OrgList = new List<string> { classId };
                this.GetChildOrg(classId, OrgList);
                expression = expression.And(p => OrgList.Contains(p.Class));
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = Convert.ToDateTime(startTime + " 00:00:00");
                expression = expression.And(p => p.CreatedTime >= start);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = Convert.ToDateTime(endTime + " 23:59:59");
                expression = expression.And(p => p.CreatedTime <= end);
            }
            return Read(expression).ToList();
        }

        public List<LateReturnReport> GetListByClassList(List<string> classIds, string startTime, string endTime)
        {
            var expression = ExtLinq.True<LateReturnReport>();
                expression = expression.And(p => classIds.Contains(p.Class));
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = Convert.ToDateTime(startTime + " 00:00:00");
                expression = expression.And(p => p.CreatedTime >= start);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = Convert.ToDateTime(endTime + " 23:59:59");
                expression = expression.And(p => p.CreatedTime <= end);
            }
            return  Read(expression).ToList();
        }
        public List<object> GetListByDivisList(List<Org> divisList,List<Org> classList, string startTime, string endTime)
        {
            var expression = ExtLinq.True<LateReturnReport>();
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = Convert.ToDateTime(startTime + " 00:00:00");
                expression = expression.And(p => p.CreatedTime >= start);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = Convert.ToDateTime(endTime + " 23:59:59");
                expression = expression.And(p => p.CreatedTime <= end);
            }
            var lateReturnlist = Read(expression).ToList();
            var resObjList = new List<object>();
            foreach (var item in divisList)
            {
                var classIds= classList.Where(p => p.ParentId.Equals(item.Id)).Select(p=>p.Id).ToList();
                var stuLateList= lateReturnlist.Where(p => classIds.Contains(p.Class));
                if (stuLateList.Count() < 1) continue;
                var group = stuLateList.GroupBy(p => p.Class);
                var classObjList = new List<object>(); ;
                foreach (var g in group)
                {

                    var classObj = new
                    {
                        classId = g.Key,
                        className = classList.FirstOrDefault(p => p.Id.Equals(g.Key)).Name,
                        count = g.Count()
                    };
                    classObjList.Add(classObj);
                }
                object divisObj = new
                {
                    divisId = item.Id,
                    divisName = item.Name,
                    list = classObjList
                };
                resObjList.Add(divisObj);
            }
            return resObjList;
        }
    }
}
