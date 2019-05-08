﻿/*******************************************************************************
 * Author: mario
 * Description: School_ExamTitle  Controller类
********************************************************************************/

using NFine.Application;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.Entity.SchoolManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Web.Mvc;

namespace NFine.Web.Areas.SchoolManage.Controllers
{
    //School_ExamTitle
    public class School_ExamTitleController : ControllerBase
    {
        private School_ExamTitle_App app = new School_ExamTitle_App();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword, string F_Enabled, string F_CreatorTime_Start, string F_CreatorTime_Stop)
        {
            var data = new
            {
                rows = app.GetList(pagination, keyword, F_Enabled, F_CreatorTime_Start, F_CreatorTime_Stop),
                total = pagination.Total,
                page = pagination.Page,
                records = pagination.Records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = app.GetForm(keyValue);
            //将用户id替换成姓名
            // var creator = new object();
            // var modifier = new object();
            // Dictionary<string, object>  dic = CacheFactory.Cache().GetCache<Dictionary<string, object>>(Cons.USERS);
            //if (data.F_CreatorUserId != null && dic.TryGetValue(data.F_CreatorUserId,out creator)) {
            //     data.F_CreatorUserId = creator.GetType().GetProperty("fullname").GetValue(creator, null).ToString();
            // }
            // if (data.F_LastModifyUserId != null &&　dic.TryGetValue(data.F_LastModifyUserId, out modifier))
            // {
            //     data.F_LastModifyUserId = modifier.GetType().GetProperty("fullname").GetValue(modifier, null).ToString();
            // }
            return Content(data.ToJson());
        }

        /// <summary>
        /// 考试主题下拉
        /// </summary>
        /// <returns>  </returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectList()
        {
            var data = app.GetList();
            List<object> list = new List<object>();
            foreach (ExamTitle item in data)
            {
                list.Add(new { id = item.F_Id, text = item.F_Title });
            }
            return Content(list.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ExamTitle entity, string keyValue)
        {
            app.SubmitForm(entity, keyValue);
            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            app.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        //导出excel
        [HttpGet]
        [HandlerAuthorize]
        public FileResult export(string keyword)
        {
            //参数 字段名->string[]{"F_Id",value}
            IDictionary<string, string> parms = new Dictionary<string, string>();
            //过滤条件
            if (!Ext.IsEmpty(keyword))
                parms.Add("F_RealName", keyword);

            DbParameter[] dbParameter = CreateParms(parms);

            string exportSql = CreateExportSql("School_ExamTitle", parms);
            //string exportSql = "";
            //Console.WriteLine("exportSql==>" + exportSql);
            DataTable dt = app.getDataTable(new BaseApp().dataScopeFilter(exportSql), dbParameter);
            ///////////////////写流
            MemoryStream ms = new NPOIExcel().ToExcelStream(dt, "School_ExamTitle列表");
            ms.Seek(0, SeekOrigin.Begin);
            string filename = "School_ExamTitle列表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            return File(ms, "application/ms-excel", filename);
        }

        //导入excel
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult import(string filePath)
        {
            //////////////////定义规则：字段名，表头名称，字典
            //字段名->string[]{表头,字典}，若是一般字段 字典为空字符串
            IDictionary<string, string[]> rules = new Dictionary<string, string[]>();
            //rules.Add("F_Id", new string[] { "编号", "" });
            //rules.Add("F_RealName", new string[] { "姓名", "" });
            //rules.Add("F_Gender", new string[] { "性别", "104" });
            //rules.Add("F_OrganizeId", new string[] { "公司", "F_OrganizeId" });
            //rules.Add("F_DepartmentId", new string[] { "部门", "F_DepartmentId" });
            //rules.Add("F_AreaId", new string[] { "地区", "F_AreaId" });
            //rules.Add("F_RoleId", new string[] { "角色", "F_RoleId" });
            //rules.Add("F_DutyId", new string[] { "岗位", "F_DutyId" });
            //rules.Add("F_CreatorTime", new string[] { "创建时间", "" });
            //rules.Add("F_HeadIcon", new string[] { "头像", "" });

            //所有字段代码
            //rules.Add("F_Id", new string[] { "考试主题ID", "" });
            //rules.Add("F_Title", new string[] { "考试主题", "" });
            //rules.Add("F_ExamDTM", new string[] { "考试时间", "" });
            //rules.Add("F_Divis_ID", new string[] { "学部ID", "" });
            //rules.Add("F_Grade_ID", new string[] { "年级ID", "" });
            //rules.Add("F_Description", new string[] { "备注", "" });
            //rules.Add("F_Enabled", new string[] { "是否显示成绩", "" });
            //rules.Add("F_SortCode", new string[] { "序号", "" });
            //rules.Add("F_DepartmentId", new string[] { "部门ID", "" });
            //rules.Add("F_DeleteMark", new string[] { "删除标记", "" });
            //rules.Add("F_EnabledMark", new string[] { "启用标记", "" });
            //rules.Add("F_CreatorTime", new string[] { "创建时间", "" });
            //rules.Add("F_CreatorUserId", new string[] { "创建人", "" });
            //rules.Add("F_LastModifyTime", new string[] { "修改时间", "" });
            //rules.Add("F_LastModifyUserId", new string[] { "修改人", "" });
            //rules.Add("F_DeleteTime", new string[] { "删除时间", "" });
            //rules.Add("F_DeleteUserId", new string[] { "删除人", "" });

            //////////////////处理数据(机构 岗位 等字典替换，过滤不要的字段，修改表头)
            List<ExamTitle> list = ExcelToList<ExamTitle>(Server.MapPath(filePath), rules);

            ///////////////////入库
            if (list == null)
                return Error("导入失败");
            app.import(list);
            return Success("导入成功。");
        }
    }
}