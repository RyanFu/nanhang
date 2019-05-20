﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHXY.Application;
using ZHXY.Common;
using ZHXY.Domain;

namespace ZHXY.Web.Dorm.Controllers
{
    /// <summary>
    /// 节假日设置
    /// </summary>
    public class HolidayController : ZhxyController
    {
        private HolidayAppService App { get; }
        public HolidayController(HolidayAppService app) => App = app;

        [HttpGet]
        public ActionResult Get(string id)
        {
           return Resultaat.Success(App.GetById(id));
        }
        public ActionResult Update(UpdateHolidayDto input)
        {
            App.Update(input);
            return Resultaat.Success();
        }

        [HttpGet]
        public ActionResult Load(Pagination pag,string keyword)
        {
            var rows = App.Load(pag,  keyword);
            return Resultaat.PagingRst(rows, pag.Records, pag.Total);
        }

        public ActionResult GetList()
        {
            return Resultaat.Success(App.Read<Holiday>().ToList().ToCamelJson());
        }

        [HttpPost]
        public ActionResult Add(AddHolidayDto dto)
        {
            App.Add(dto);
            return Resultaat.Success();
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                App.Delete(id.Split(','));
            }
            return Resultaat.Success();
        }

    }
}