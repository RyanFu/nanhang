﻿using System;

namespace ZHXY.Application
{
    public class AddHolidayDto
    { 
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
