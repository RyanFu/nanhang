﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ZHXY.Application;
using ZHXY.Domain;
using ZHXY.Common;

namespace ZHXY.Web.SystemManage.Controllers
{
    /// <summary>
    /// 机构管理
    /// </summary>
    public class OrganizeController : ZhxyWebControllerBase
    {
        private OrgAppService App { get; }

        public OrganizeController(OrgAppService app) => App = app;

        [HttpGet]
        public ActionResult GetSelectJson(string F_OrgId)
        {
            var list = new List<object>();
            var data = App.GetListByOrgId(F_OrgId);
            foreach (var item in data)
            {
                list.Add(new { id = item.Id, text = item.Name });
            }
            return Content(list.ToJson());
        }

        [HttpGet]
        public ActionResult GetFullNameById(string F_Id)
        {
            var data = App.GetListById(F_Id);
            return Content(data.ToJson());
        }

        [HttpGet]
        public ActionResult GetTreeSelectJson()
        {
            var data = App.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (var item in data)
            {
                var treeModel = new TreeSelectModel
                {
                    id = item.Id,
                    text = item.Name,
                    parentId = item.ParentId,
                    data = item
                };
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }

        /// <summary>
        /// 根据类别出机构下拉（非树状结构）
        /// </summary>
        /// <param name="parentId"> 父机构ID </param>
        /// <param name="keyword">  机构类别ID </param>
        /// <returns>  </returns>
        [HttpGet]
        public ActionResult GetSelectJsonByCategoryId(string keyword, string parentId)
        {
            var data = App.GetList();
            if (!string.IsNullOrEmpty(keyword))
                data = data.Where(t => t.CategoryId == keyword).ToList();
            if (!parentId.IsEmpty())
                data = data.Where(t => t.ParentId == parentId).ToList();
            var list = new List<object>();
            foreach (var item in data)
            {
                list.Add(new { id = item.Id, text = item.Name });
            }
            return Content(list.ToJson());
        }

        [HttpGet]
        public ActionResult GetTreeJson(string keyword)
        {
            var data = App.GetList();
            var data_deeps = "";
            var role = new SysRoleAppService().Get(keyword);
            if (!role.IsEmpty())
            {
                data_deeps = role.F_Data_Deps;
            }
            else
            {
                var user = new SysUserAppService().Get(keyword);
                if (!user.IsEmpty())
                    data_deeps = user.F_Data_Deps;
            }

            var treeList = new List<TreeViewModel>();
            foreach (var item in data)
            {
                var tree = new TreeViewModel();
                var hasChildren = data.Count(t => t.ParentId == item.Id) != 0;
                tree.id = item.Id;
                tree.text = item.Name;
                tree.value = item.EnCode;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = false;
                tree.hasChildren = hasChildren;
                tree.showcheck = true;
                //tree.img = "";
                if (!data_deeps.IsEmpty() && (data_deeps.IndexOf(item.Id, StringComparison.Ordinal) != -1))
                    tree.checkstate = 1;
                else
                    tree.checkstate = 0;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }

        [HttpGet]
        public ActionResult GetTreeGridJson(string keyword, string divisId, string gradeId, string classId, string schoolId)
        {
            var data = App.GetList();
            if (!string.IsNullOrEmpty(keyword))
                data = data.TreeWhere(t => t.Name.Contains(keyword));
            var treeList = new List<TreeGridModel>();
            foreach (var item in data)
            {
                var treeModel = new TreeGridModel();
                var hasChildren = data.Count(t => t.ParentId == item.Id) != 0;
                treeModel.id = item.Id;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.ParentId;
                treeModel.expanded = false;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }

        /// <summary>
        /// 获取学校
        /// </summary>
        /// <param name="keyword">  </param>
        /// <returns>  </returns>
        [HttpGet]
        public ActionResult GetSchoolOrGradeTreeGridJson(string keyword)
        {
            var data = App.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.CategoryId.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (var item in data)
            {
                var treeModel = new TreeGridModel();
                var hasChildren = data.Count(t => t.ParentId == item.Id) != 0;
                treeModel.id = item.Id;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.ParentId;
                treeModel.expanded = false;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }


        [HttpGet]
        public JsonResult GetDivisGradeClass(string keyValue)
        {
            var DivsGradeClass = "";
            var Dataclass = App.GetById(keyValue);
            if (Dataclass == null) return Json(DivsGradeClass, JsonRequestBehavior.AllowGet);
            var Gradedata = Dataclass.Parent;
            if (Gradedata == null) return Json(DivsGradeClass, JsonRequestBehavior.AllowGet);
            var Divis = Gradedata.Parent;
            if (Divis != null)
            {
                DivsGradeClass = Divis.F_FullName + Gradedata.F_FullName + Dataclass.F_FullName;
            }
            return Json(DivsGradeClass, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Get(string id)
        {
            var data = App.GetById(id);
            return Resultaat.Success(data);
        }

        [HttpPost]
        public ActionResult Update(UpdateOrgDto dto)
        {
            App.Update(dto);
            return Resultaat.Success();
        }

        public ActionResult Add(AddOrgDto dto)
        {
            App.Add(dto);
            return Resultaat.Success();
        }

        [HttpPost]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            App.Delete(id);
            CacheFactory.Cache().RemoveCache(SYS_CONSTS.ORGANIZE);
            CacheFactory.Cache().WriteCache(SysCacheAppService.GetOrganizeList(), SYS_CONSTS.ORGANIZE);
            return Resultaat.Success();
        }

        /// <summary>
        /// 取到学部对应
        /// </summary>
        [HttpGet]
        public ActionResult GetOrgDics()
        {
            // 获取所有学部
            var orgList = App.GetList().Where(t => t.CategoryId == "Division").ToList();
            var orgs = new Dictionary<string, Dictionary<string, string[]>>();
            //入学年段
            var tmp = new Dictionary<string, string[]>();
            ////就读方式
            //Dictionary <string, string[]> F_SchoolType = new Dictionary<string, string[]>();
            ////来源类型
            //Dictionary<string, string[]> F_ComeFrom_Type = new Dictionary<string, string[]>();
            foreach (var org in orgList)
            {
                switch (org.EnCode)
                {
                    //精品小学
                    case "01":
                    case "04":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "一年级", "二年级", "三年级", "四年级", "五年级", "六年级" });
                        tmp.Add("F_SchoolType", new string[] { "住校", "走读", "陪读" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "校内直升", "无学籍" });
                        orgs.Add(org.Id, tmp);
                        break;

                    case "02":
                    case "05":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "初一", "初二", "初三" });
                        tmp.Add("F_SchoolType", new string[] { "住校", "走读", "陪读" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "校内直升" });
                        orgs.Add(org.Id, tmp);
                        break;

                    case "03":
                    case "06":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "高一", "高二", "高三" });
                        tmp.Add("F_SchoolType", new string[] { "住校", "走读", "陪读" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "校内直升" });
                        orgs.Add(org.Id, tmp);
                        break;

                    case "07":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "小学", "初中", "高中" });
                        tmp.Add("F_SchoolType", new string[] { "住校", "走读", "陪读" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "校内直升" });
                        orgs.Add(org.Id, tmp);
                        break;

                    case "08":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "小一", "小二", "小三", "小四", "小五", "小六" });
                        tmp.Add("F_SchoolType", new string[] { "住校" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "校内直升" });
                        orgs.Add(org.Id, tmp);
                        break;

                    case "11":
                    case "12":
                    case "13":
                    case "14":
                    case "15":
                    case "16":
                        tmp = new Dictionary<string, string[]>();
                        tmp.Add("F_InYear", new string[] { "小小班", "小班", "中班", "大班" });
                        tmp.Add("F_SchoolType", new string[] { "日托", "全日托", "周托", "全托" });
                        tmp.Add("F_ComeFrom_Type", new string[] { "校外转入", "无学籍" });
                        orgs.Add(org.Id, tmp);
                        break;
                }
            }
            return Content(orgs.ToJson());
        }

        /// <summary>
        /// 获取老师机构
        /// author:yujf
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetTeacherOrg(string nodeId = "3", int n_level = 0) => await Task.Run(() =>
             {
                 var result = App.GetTeacherOrg(nodeId, n_level);
                 return Resultaat.Success(result);
             });

        /// <summary>
        /// 获取老师机构
        /// author:yujf
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetStudentOrg(string nodeId = "2", int n_level = 0) => await Task.Run(() =>
        {
            var result = App.GetTeacherOrg(nodeId, n_level);
            return Resultaat.Success(result);
        });


    }
}