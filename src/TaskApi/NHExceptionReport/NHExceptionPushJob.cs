﻿using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ZHXY.Application;
using ZHXY.Common;
using ZHXY.Domain;
using ZHXY.Domain.Entity.Dorm.Report;

namespace TaskApi.NHExceptionReport
{
    public class NHExceptionPushJob : IJob
    {
        private ILog Logger { get; } = LogManager.GetLogger(typeof(NHExceptionPushJob));
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("开始推送异常消息：" + DateTime.Now);
            //new PushAppMessage().PushReportMessage("48038@nchu.edu.cn", "Test Message", "");
            //DateTime Time = Convert.ToDateTime("2019-06-05 08:00:00");
            var Time = DateTime.Now;
            var dbContext = new EFContext();
            //测试阶段：  只推给 罗尉平 老师
            var leaderList = dbContext.Set<OrgLeader>().Where(p => p.UserId.Equals("c769eb3d87d6f7468133840f055bc9e6")).ToList();
            var PushList = new List<ZhxyPush>();
            foreach (var leader in leaderList)
            {
                var Ids = new HashSet<string>(); //当前组织机构下属所有组织机构的ID集合
                var OrgId = leader.OrgId;
                var UserId = leader.UserId;
                var OrgIds = dbContext.Set<Org>().Where(p => p.ParentId.Equals(OrgId)).Select(p => p.Id).ToList();
                if (null == OrgIds || OrgIds.Count() == 0)
                {
                    continue;
                }
                foreach (var id in OrgIds)
                {
                    Ids.Add(id);
                    var SonOrgIds = dbContext.Set<Org>().Where(p => p.ParentId.Equals(id)).Select(p => p.Id).ToList();
                    if (null == SonOrgIds || SonOrgIds.Count() == 0)
                    {
                        continue;
                    }
                    foreach (var sid in SonOrgIds)
                    {
                        Ids.Add(sid);
                        var SonOfSonOrgIds = dbContext.Set<Org>().Where(p => p.ParentId.Equals(sid)).Select(p => p.Id).ToList();
                        if (null == SonOfSonOrgIds || SonOfSonOrgIds.Count() == 0)
                        {
                            continue;
                        }
                        foreach (var ssid in SonOfSonOrgIds)
                        {
                            Ids.Add(sid);
                            var SonOfSonOfSonIds = dbContext.Set<Org>().Where(p => p.ParentId.Equals(ssid)).Select(p => p.Id).ToList();
                            if (null == SonOfSonOfSonIds || SonOfSonOfSonIds.Count() == 0)
                            {
                                continue;
                            }
                            foreach (var sssid in SonOfSonOfSonIds)
                            {
                                Ids.Add(sssid);
                            }
                        }
                    }
                }
                Ids.Add(OrgId);
                if (null != Ids && Ids.Count() != 0)
                {
                    var DataList = new List<Dictionary<string, object>>();
                    var Yeatday = Time.Date.AddDays(-1).Date;
                    var userName = dbContext.Set<User>().Where(p => p.Id.Equals(leader.UserId)).Select(p => p.Account).FirstOrDefault();
                    var NoReturnCount = dbContext.Set<NoReturnReport>().Where(p => Yeatday.Equals(p.CreatedTime) && Ids.Contains(p.ClassId)).ToList().Count();
                    var LateReturnCount = dbContext.Set<LateReturnReport>().Where(p => Yeatday == p.CreatedTime && Ids.Contains(p.Class)).ToList().Count();
                    var NoOutCount = dbContext.Set<NoOutReport>().Where(p => Yeatday == p.CreatedTime && Ids.Contains(p.ClassId)).ToList().Count();

                    var NoReturnDic = new Dictionary<string, object>();
                    var LateReturnDic = new Dictionary<string, object>();
                    var NotOutDic = new Dictionary<string, object>();
                    var DataDic = new Dictionary<string, object>();
                    NoReturnDic.Add("未归人员数量：", NoReturnCount + "人");
                    NoReturnDic.Add("URI", ConfigurationManager.AppSettings["NoReturnReport"] + "?OrgId=" + leader.OrgId + "&ReportDate=" + Yeatday.Date.ToString("yyyy-MM-dd"));
                    LateReturnDic.Add("晚归人员数量：", LateReturnCount + "人");
                    LateReturnDic.Add("URI", ConfigurationManager.AppSettings["LaterReturnReport"] + "?OrgId=" + leader.OrgId + "&ReportDate=" + Yeatday.Date.ToString("yyyy-MM-dd"));
                    NotOutDic.Add("长时间未出人员数量：", NoOutCount + "人");
                    NotOutDic.Add("URI", ConfigurationManager.AppSettings["NotOutReport"] + "?OrgId=" + leader.OrgId + "&ReportDate=" + Yeatday.Date.ToString("yyyy-MM-dd"));
                    DataDic.Add("date", Yeatday.Date);
                    DataDic.Add("to", userName);
                    DataList.Add(NoReturnDic);
                    DataList.Add(LateReturnDic);
                    DataList.Add(NotOutDic);
                    DataList.Add(DataDic);
                    new PushAppMessage().PushReportMessage(userName, DataList.ToJson(), "");
                    PushList.Add(new ZhxyPush()
                    {
                        Account = userName,
                        Content = DataList.ToJson(),
                        CreateTime = DateTime.Now,
                        ReportDate = Yeatday.Date
                    });
                    Console.WriteLine("NHExceptionPush 推送成功：" + userName  + " : " + DataList.ToJson());
                }
            }
            dbContext.Set<ZhxyPush>().AddRange(PushList);
            dbContext.SaveChanges();
            Console.WriteLine("  ********* NHExceptionPush 全部推送成功 ");
            dbContext.Dispose();
        }
    }
}
