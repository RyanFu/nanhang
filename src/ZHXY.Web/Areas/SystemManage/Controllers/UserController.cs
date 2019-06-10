﻿using System.Web.Mvc;
using ZHXY.Application;
using ZHXY.Common;
using ZHXY.Domain;

namespace ZHXY.Web.SystemManage.Controllers
{
    /// <summary>
    /// 用户管理
    /// [OK]
    /// </summary>
    public class UserController : ZhxyController
    {
        private UserService App { get; }

        public UserController(UserService app) => App = app;

        #region view

        [HttpGet]
        public ActionResult Info() => View();

        #endregion view

        [HttpGet]

        public ActionResult GetGridJson(Pagination pagination, string keyword, string F_DepartmentId, string F_DutyId, string F_CreatorTime_Start, string F_CreatorTime_Stop, string F_Contains)
        {
            var data = new
            {
                rows = App.GetList(pagination, keyword, F_DepartmentId, F_DutyId, F_CreatorTime_Start, F_CreatorTime_Stop),
                total = pagination.Total,
                page = pagination.Page,
                records = pagination.Records
            };
            return Content(data.ToJson());
        }

        [HttpGet]

        public ActionResult GetList(Pagination pagination, string keyword, string F_DepartmentId, string F_DutyId, string F_CreatorTime_Start, string F_CreatorTime_Stop, string F_Contains)
        {
            var data = new
            {
                rows = App.GetList(pagination, keyword, F_DepartmentId, F_DutyId, F_CreatorTime_Start, F_CreatorTime_Stop),
                total = pagination.Total,
                page = pagination.Page,
                records = pagination.Records
            };
            return Content(data.ToJson());
        }

        [HttpGet]

        public ActionResult GetFormJsonByOrg(string F_DutyId)
        {
            var data = App.GetByOrg(F_DutyId);
            return Content(data.ToJson());
        }

        public ActionResult GetListByOrg(string orgId)
        {
            var data = App.GetByOrg(orgId);
            return Content(data.ToJson());
        }

        [HttpGet]

        public ActionResult GetFormJson(string keyValue)
        {
            var data = App.GetById(keyValue);
            return Content(data.ToJson());
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(User userEntity,string F_RoleId, string keyValue)
        {
            App.Submit(userEntity, F_RoleId, keyValue);
            return Result.Success();
        }

      
        [HttpPost]
        [HandlerAuthorize]

        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            App.Delete(keyValue.Split('|'));
            return Result.Success();
        }

        [HttpGet]
        public ActionResult RevisePassword() => View();

        [HttpPost]

        public ActionResult SubmitRevisePassword(string userPassword, string keyValue)
        {
            App.RevisePassword(userPassword, keyValue);
            return Result.Success();
            
        }

       
        public JsonResult GetUserPassword(string userid, string password)
        {
            var IsOk = App.VerifyPwd(userid, password);
            return Json(IsOk);
        }
    }
}