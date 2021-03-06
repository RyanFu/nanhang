﻿using ZHXY.Common;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ZHXY.Domain;
using System;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Text;
using ZHXY.Web.Shared;

namespace ZHXY.Application
{
    /// <summary>
    /// 访客管理
    /// </summary>
    public class VisitorService :  AppService
    {
        public VisitorService(DbContext r) : base(r)
        {
        }

        public object GetList(Pagination pagination, string F_Building, int Time_Type, string startTime, string endTime)
        {
            var StartTime = Convert.ToDateTime(startTime);
            var EndTime = Convert.ToDateTime(endTime);
            return Read<VisitorApply>(p => p.VisitStartTime > StartTime && p.VisitStartTime < EndTime)
                .Paging(pagination).Join(Read<Student>(), p => p.ApplicantId, s => s.Id, (apply, stu) => new
            {
                apply.Id,
                stu.Name,
                apply.VisitorName,
                apply.VisitorIDCard,
                apply.VisitReason,
                apply.VisitorGender,
                apply.ApplicantId,
                apply.DormId,
                apply.Status
            }).Join(Read<DormRoom>(), p => p.DormId, s => s.Id, (temp, dorm) => new {
                F_Id = temp.Id,
                F_Visit_Object = temp.Name,
                F_Visitor_Name = temp.VisitorName,
                F_Visitor_Card = temp.VisitorIDCard,
                F_Visit_Reason = temp.VisitReason,
                F_Sex = temp.VisitorGender,
                temp.ApplicantId,
                F_Classroom_ID = dorm.Title,
                F_EnabledMark = temp.Status
            }).ToList();
        }

        public object GetBuilding(string KeyWords)
        {           
            var query = R.Set<Building>();
            if(null != KeyWords && KeyWords.Length > 0)
            {
                query.Where(p => p.BuildingNo.Contains(KeyWords));
            }
            return query.Select(p => new
            {
                id = p.Id,
                text = p.BuildingNo
            }).ToList();
        }

        public object SearchStudents(string KeyWords)
        {
            var query = Read<DormVisitLimit>().Join(Read<Student>(),a=>a.StudentId,b=>b.Id,(a, b)=>new{a,b});
            if (KeyWords != null && KeyWords.Length != 0)
            {
                query = IsNumeric(KeyWords) ? query.Where(p => p.b.StudentNumber.Equals(KeyWords)) : query.Where(p => p.b.Name.Contains(KeyWords));
            }
            var result = query.Select(p => new { id = p.b.Id, text = p.b.Name, limit = p.a.UsableLimit, ImgUri = p.b.FacePic }).OrderBy(p => p.id).Take(20).ToList();

            return result;
        }

        public object GetForm(string keyValue) => throw new NotImplementedException();

        public object SupervisorByStudent(string StudentId)
        {
            return null;
        }

        /// <summary>
        /// 宿管查询所审批访客，学生查询所提交访客
        /// </summary>
        public dynamic GetVisitorApprovalList(VisitorApprovalListDto input)
        {
            IQueryable<VisitorApply> query = null;
            var visitorListViews = new List<VisitorListView>();
            //判断登陆用户是学生，还是宿管   若是学生获取所有提交的申请，若是老师则查看所有审批的申请
            var dutyId = Read<User>(p => p.Id.Equals(input.CurrentUserId)).Select(p => p.DutyId).FirstOrDefaultAsync().Result;
            if (dutyId.Equals("teacherDuty") || dutyId.Equals("suguanDuty"))
            {
                //获取当前用户所审批的审批单据
                var visitIds = Read<VisitorApprove>(p => p.ApproverId.Contains(input.CurrentUserId)).Select(p => p.VisitId).ToListAsync().Result;
                //根据审批单据获取访客详细信息
                query = Read<VisitorApply>(p => visitIds.Contains(p.Id));
            }
            else if (dutyId.Equals("studentDuty"))
            {
                query = Read<VisitorApply>(p => p.ApplicantId.Equals(input.CurrentUserId));
            }
            else
            {
                return visitorListViews;
            }
            query = string.IsNullOrEmpty(input.ApprovalStatus) ? query : query.Where(p => p.Status.Equals(input.ApprovalStatus));
            query = string.IsNullOrEmpty(input.Keyword) ? query : query.Where(p => p.Student.Name.Contains(input.Keyword));            
            //query = query.Paging(input);
            visitorListViews = query.OrderByDescending(p => p.CreatedTime).PagingNoSort(input).Select(p => new VisitorListView
            {
                Id = p.Id,
                ApplicantName = p.Student.Name,
                VisitorGender = p.VisitorGender,
                VisitorIDCard = p.VisitorIDCard,
                VisitorName = p.VisitorName,
                VisitReason = p.VisitReason,
                VisitType = p.VisitType,
                Relationship = p.Relationship,
                VisitStartTime = p.VisitStartTime,
                VisitEndTime = p.VisitEndTime,                
                ApprovalStatus = p.Status,
                CreatedTime = p.CreatedTime
            }).ToListAsync().Result;
            return visitorListViews;
        }

        /// <summary>
        /// 学生、宿管获取访客审批详情
        /// </summary>
        public VisitorListView GetVisitorApprovalDetail(string visitId, string currentUserId)
        {
            //获取审批结果和意见（随机取一条即可）
            var approveInfo = Read<VisitorApprove>(p => p.VisitId.Equals(visitId)).FirstOrDefaultAsync().Result;


            var visitor = Get<VisitorApply>(visitId);
            var view = new VisitorListView
            {
                Id = visitor.Id,
                ApplicantName = visitor.Student.Name,
                VisitorGender = visitor.VisitorGender,
                VisitorIDCard = visitor.VisitorIDCard,
                VisitorName = visitor.VisitorName,
                VisitReason = visitor.VisitReason,
                VisitType = visitor.VisitType,
                Relationship = visitor.Relationship,
                VisitStartTime = visitor.VisitStartTime,
                VisitEndTime = visitor.VisitEndTime,                
                ApprovalStatus = visitor.Status,
                Result = approveInfo.ApproveResult,
                Opinion = approveInfo.Opinion,
                CreatedTime = visitor.CreatedTime,
                ImgUrl = visitor.ImgUri
            };
            return view;
        }

        /// <summary>
        /// 访客审批
        /// </summary>
        public string Approval(VisitorApprovalDto input, string img)
        {
            var visitor = Get<VisitorApply>(input.VisitId);
            visitor.ImgUri = img;
            var visitorApprover = Query<VisitorApprove>(p => p.VisitId.Equals(visitor.Id)).FirstOrDefault();
            var DHMessage = "";
            visitorApprover.ApproveResult = input.IsAgreed ? "1" : "-1";
            visitorApprover.Opinion = input.Opinion;
            if(visitorApprover.ApproveResult=="1")
            {
                DHMessage =  PushVisitor("",Convert.ToInt32( visitor.VisitorGender), visitor.ImgUri, visitor.VisitorName, visitor.VisitorIDCard, visitor.BuildingId, visitor.VisitEndTime);
                if(DHMessage != null)
                {
                    var jo = DHMessage.Parse2JObject();
                    if (jo.Value<bool>("success"))
                    {
                        visitor.DhId = jo.Value<JObject>("data").Value<string>("personId");
                        var VisitLimit = Query<DormVisitLimit>(p => p.StudentId.Equals(visitor.ApplicantId)).FirstOrDefault();
                        VisitLimit.UsableLimit--;
                    }
                }
            }
            visitor.ApprovedTime = DateTime.Now;
            visitor.Status = "1";
            SaveChanges();
            return DHMessage;
        }
        
        // 推送访客至闸机
        private string PushVisitor(string studentNum,int sex, string img,string name,string idCode, string buildingId,DateTime endTime)
        {
            // 闸机Id列表
            var zjids = Read<Relevance>(p => p.SecondKey == buildingId && p.Name == Relation.GateBuilding).Select(p => p.FirstKey).ToList();
            var channelIds= Read<Gate>(t=>zjids.Contains(t.Id)).Select(p=>p.DeviceNumber).ToArray();
            var survey = new SurveyMoudle();
            survey.channelId = channelIds.Select(p => p + "$7$0$0").ToArray();
            survey.code = studentNum;
            survey.name = name;
            survey.sex = sex;
            survey.idCode = idCode;
            survey.photoBase64 = GetImageBase64Str.ImageBase64Str(img);
            survey.initialTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            survey.expireTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            return DHAccount.TempSurvey(survey);
        }

        public object VisivorByStudent(Pagination pag, string userId, int status)
        {
            var query = Read<VisitorApply>(p => p.ApplicantId.Equals(userId) && p.Status.Equals(status));
            return query.Paging(pag).ToListAsync().Result;
        }


        /// <summary>
        /// 获取访问详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public object GetDetail(string id) => Get<VisitorApply>(id);

        public void CheckVisitor(string UserId, int CheckType, string VisitLogId)
        {
            var updateSql = "update visit set f_memo = 1 from [dbo].[Dorm_VisitLog] visit where visit.F_Id = '"+ VisitLogId + "'";
            R.Database.ExecuteSqlCommand(updateSql);
        }

        public object SearchStudentLimit(string StudentId)
        {
            return R.Set<DormVisitLimit>().Where(p => p.StudentId == StudentId).Select(p => new { id = StudentId, text = p.UsableLimit}).FirstOrDefault();
        }

        /// <summary>
        /// 审批
        /// </summary>
        public void Approval(string id,bool pass)
        {
            var v = Get<VisitorApply>(id);
            v.Status = pass ? "1" : "-1";
            v.ApprovedTime = DateTime.Now;
            SaveChanges();
        }
        /// <summary>
        /// 批量审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pass"></param>
        public void ApprovalList(string[] ids, int pass, string img)
        {
            var ApplyIds = Query<VisitorApprove>(p => ids.Contains(p.Id)).Select(p => p.VisitId).ToList();
            Query<VisitorApply>(p => ApplyIds.Contains(p.Id)).ToList().ForEach(s =>  {
                s.Status = pass.ToString();
                s.ApprovedTime = DateTime.Now;
            });
            //审批通过之后，把当前学生的访问额度 -1
            if(pass == 1)
            {
                var listLimit = Query<VisitorApprove>(p => ids.Contains(p.Id)).Join(Query<VisitorApply>(), p => p.VisitId, s => s.Id, (app, visit) => new VisitorApply
                {
                    VisitorGender = visit.VisitorGender,
                    ImgUri = visit.ImgUri,
                    VisitorName = visit.VisitorName,
                    VisitorIDCard = visit.VisitorIDCard,
                    BuildingId = visit.BuildingId,
                    VisitEndTime = visit.VisitEndTime,
                    VisitStartTime = visit.VisitStartTime,
                }).ToList();
                var ListStuId = Query<VisitorApprove>(p => ids.Contains(p.Id)).Join(Query<VisitorApply>(), p => p.VisitId, s => s.Id, (app, visit) => app.ApproverId).ToList();
                Query<DormVisitLimit>().Where(p => ListStuId.Contains(p.StudentId)).ToList().ForEach(p => 
                {
                    p.UsableLimit -= 1;
                });
                foreach(var visitor in listLimit)
                {
                    visitor.ImgUri = img;
                    PushVisitor("", Convert.ToInt32(visitor.VisitorGender), visitor.ImgUri, visitor.VisitorName, visitor.VisitorIDCard, visitor.BuildingId, visitor.VisitEndTime);
                }
            }
            var UserId = Operator.GetCurrent().Id;
            Query<VisitorApprove>(p => ids.Contains(p.Id)).ToList().ForEach(s => 
            {
                s.ApproverId = UserId;
                s.ApproveResult = pass.ToString();
                s.Opinion = pass == 1 ? "通过" : "不通过";
            });
            SaveChanges();
        }

        /// <summary>
        /// 提交申请
        /// </summary>
        /// <param name="input"></param>
        public string Submit(VisitorApplyDto input, string img)
        {
            input.ImgUri = img;
            var currentUserId = Operator.GetCurrent().Id;
            var MSG = "";
            var LimitCount = Query<DormVisitLimit>(p => p.StudentId.Equals(currentUserId)).FirstOrDefault();
            if(LimitCount.UsableLimit == 0)
            {
                return "当前学生被访额度为0";
            }
            // var visit = input.MapTo<VisitApply>();//映射到数据库中对应的表
            var visit = new VisitorApply
            {
                ApplicantId = currentUserId,
                VisitorGender = input.VisitorGender,
                VisitorName = input.VisitorName,
                VisitorIDCard = input.VisitorIDCard,
                VisitReason = input.VisitReason,
                VisitType = input.VisitType,
                VisitStartTime = input.VisitStartTime,
                VisitEndTime = input.VisitEndTime,
                Relationship = input.Relationship,
                Status = "0",
                ImgUri = input.ImgUri
            };
            //获取学生所在的dormid
            var dormid = Read<DormStudent>(p => p.StudentId.Equals(currentUserId)).Select(p => p.DormId).FirstOrDefault();
            //获取dorm对应的楼栋Id
            var buildingId = Read<DormRoom>(p => p.Id.Equals(dormid)).Select(p => p.BuildingId).FirstOrDefault();
            //获取楼栋对应的宿管
            var Approvers = Read<Relevance>(p => p.Name.Equals("Building_User") && p.FirstKey.Equals(buildingId)).Select(p => p.SecondKey).ToListAsync().Result;
            if (null == Approvers) throw new Exception("请先绑定宿管!");
            if (visit.VisitType.Equals("0"))
            {
                var stu = Read<Student>(p => p.Id.Equals(input.VisitorId)).FirstOrDefault();
                visit.Status = "1";
                visit.VisitorIDCard = stu.CredNumber;
                visit.VisitorGender = stu.Gender;
                AddAndSave(new VisitorApprove
                {
                    ApproverId = Approvers.ToJson(),
                    VisitId = visit.Id,
                    ApproveLevel = 1,
                    ApproveResult = "1"
                });
                //人员在大华的ID
                var code = Read<Student>(p => p.Id.Equals(input.VisitorId)).Select(p => p.StudentNumber).FirstOrDefault();
                var dhUserstr = DHAccount.SELECT_DH_PERSON(new PersonMoudle() { code = code });
                var ResultList = (List<object>)dhUserstr.ToString().Parse2JObject()["data"]["list"].ToObject(typeof(List<object>));
                visit.DhId = ResultList.First().ToString().Parse2JObject().Value<int>("id").ToString();
                MSG = PushVisitorSchool(buildingId, visit.VisitEndTime, Convert.ToInt32(visit.DhId));
                //添加校内访客成功： 访问额度 -1
                LimitCount.UsableLimit--;
            }
            else
            {
                AddAndSave(new VisitorApprove
                {
                    ApproverId = Approvers.ToJson(),
                    VisitId = visit.Id,
                    ApproveLevel = 1,
                    ApproveResult = "0"
                });
            }
            visit.DormId = dormid;
            visit.BuildingId = buildingId;
            visit.ImgUri = input.ImgUri;
            AddAndSave(visit);
            SaveChanges();
            return MSG;
        }

        /// <summary>
        /// 用于同学互访
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string PushVisitorSchool(string buildingId, DateTime endTime, int DHPersonId)
        {
            // 闸机Id列表
            var zjids = Read<Relevance>(p => p.SecondKey == buildingId && p.Name == Relation.GateBuilding).Select(p => p.FirstKey).ToList();
            var channelIds = Read<Gate>(t => zjids.Contains(t.Id)).Select(p => p.DeviceNumber).ToArray();
            var survey = new SurveyMoudle();
            survey.channelId = channelIds.Select(p => p + "$7$0$0").ToArray();
            survey.initialTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            survey.expireTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            survey.personId = DHPersonId;
            return DHAccount.Survey(survey);
        }

        public object NotCheckApply(Pagination pagination)
        {
            var UserId = Operator.GetCurrent().Id;
            return 
                Read<VisitorApprove>(p => p.ApproveResult.Equals("0") && p.ApproverId.Contains(UserId)).Paging(pagination)
                .Join(Read<VisitorApply>(p => p.Status.Equals("0")), p => p.VisitId, s=>s.Id, (approve, apply) => new {
                    approve.Id,
                    apply.ApplicantId,
                    apply.VisitorName,
                    apply.VisitorIDCard,
                    apply.VisitReason,
                    apply.VisitorGender,
                    apply.DormId
                }).Join(Read<Student>(), p => p.ApplicantId, s => s.Id, (apply, stu) => new
                 {
                     apply.Id,
                     stu.Name,
                     apply.VisitorName,
                     apply.VisitorIDCard,
                     apply.VisitReason,
                     apply.VisitorGender,
                     apply.ApplicantId,
                     apply.DormId
                 }).Join(Read<DormRoom>(), p => p.DormId, s => s.Id, (temp, dorm) => new {
                     F_Id = temp.Id,
                     F_Visit_Object = temp.Name,
                     F_Visitor_Name = temp.VisitorName,
                     F_Visitor_Card = temp.VisitorIDCard,
                     F_Visit_Reason = temp.VisitReason,
                     F_Sex = temp.VisitorGender,
                     temp.ApplicantId,
                     F_Classroom_ID = dorm.Title
                 }).ToList();
        }


        ////////////////////////////////                 宿管相关                 //////////////////////////////////////
        public object GetDorm()
        {
            var SuperId = Operator.GetCurrent().Id;
            var BuildingIds = Read<Relevance>(p => p.SecondKey.Equals(SuperId) && p.Name.Equals("Building_User")).Select(p => p.FirstKey).ToList();
            return Read<Building>(p => BuildingIds.Contains(p.Id)).Select(p => new {
                id = p.Id,
                buildingNo = p.BuildingNo
            }).ToList();
        }


        public string StatisticsByBuild(string BuildingId)
        {
            var Total = Read<DormRoom>(p => p.BuildingId.Equals(BuildingId)).Join(Read<DormStudent>(), s => s.Id, p => p.DormId, (dormRoom, dormStu) => new
            {
                dormStu.StudentId
            }).Join(Read<Student>(), p => p.StudentId, s => s.Id, (temp, stu) => new {
                stu.InOut
            }).ToList();
        
            var OutCount = Total.Count(p => string.IsNullOrEmpty(p.InOut) || p.InOut.Equals("1"));
            var InCount = Total.Count(p => !string.IsNullOrEmpty(p.InOut) && p.InOut.Equals("0"));
            var ResultDic = new Dictionary<string, int>();
            ResultDic.Add("TotalCount", Total.Count());
            ResultDic.Add("OutCount", OutCount);
            ResultDic.Add("InCount", InCount);
            return ResultDic.ToJson();
        }



        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            var ascii = new ASCIIEncoding();
            var bytestr = ascii.GetBytes(str);

            foreach (var c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

