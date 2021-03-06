﻿using System.Collections.Generic;
using System.Data.Entity;
using ZHXY.Domain;
using System;
using ZHXY.Web.Shared;

namespace ZHXY.Application
{
    /// <summary>
    /// 宿舍管理
    /// </summary>
    public class DormRoomAppService : AppService
    {
        public DormRoomAppService(DbContext r) : base(r)
        {
        }

        public List<DormRoom> Load(Pagination p)
        {
            return Read<DormRoom>().Paging(p).ToListAsync().Result;
        }

        public object GetById(string id) => throw new NotImplementedException();
    }
}

