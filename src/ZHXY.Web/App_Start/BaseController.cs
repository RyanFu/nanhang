﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using ZHXY.Application;
using ZHXY.Common;

namespace ZHXY.Web
{
    [LoginAuthentication]
    [ProcessMvcError]
    public abstract class BaseController : Controller
    {
        #region property

        protected ILog FileLog => LogHelper.GetLogger(GetType().ToString());

        #endregion property

        #region View

        [HttpGet]
        [HandlerAuthorize]
        public virtual async Task<ViewResult> Index() => await Task.Run(() => View());

        [HttpGet]
        public virtual async Task<ViewResult> Form() => await Task.Run(() => View());

        [HttpGet]
        public virtual async Task<ViewResult> Details() => await Task.Run(() => View());

        [HttpGet]
        public virtual async Task<ViewResult> Approve() => await Task.Run(() => View());

        [HttpGet]
        public virtual async Task<ViewResult> Return() => await Task.Run(() => View());

        [HttpGet]
        public virtual async Task<ViewResult> Import() => await Task.Run(() => View());

        #endregion View

        #region others
        protected DbParameter[] CreateParms(IDictionary<string, string> parms)
        {
            return parms.Select(kp => new SqlParameter("@" + kp.Key, kp.Value)).Cast<DbParameter>().ToArray();
        }
        #endregion others
    }
}