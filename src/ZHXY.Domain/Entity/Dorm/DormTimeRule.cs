﻿namespace ZHXY.Domain
{
    /// <summary>
    /// 宿舍考勤规则
    /// </summary>
    public class DormTimeRule:IEntity
    {
        /// <summary>
        /// 星期几
        /// </summary>
        public string DayOfWeek { get; set; }
        /// <summary>
        /// 关门时间(超出该时间视为晚归)
        /// </summary>
        public string ClosedTime { get; set; }
        /// <summary>
        /// 未归限制时间(超过该时间限制即视为未归)
        /// </summary>
        public string NotReturnLimitTime { get; set; }

        /// <summary>
        /// 未出时间限制(超过该时间无出宿舍记录即视为长时间未出)
        /// </summary>
        public decimal? NotOutLimit { get; set; }
    }
}
