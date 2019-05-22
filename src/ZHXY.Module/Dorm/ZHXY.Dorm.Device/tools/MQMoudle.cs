﻿namespace ZHXY.Dorm.Device.tools
{
    public class MQMoudle
    {
        public string clientType { get; set; } = "WINPC"; // 电脑名称
        public string clientMac { get; set; } = "30:9c:23:79:40:08"; //电脑mac地址
        public string clientPushId { get; set; } = ""; //
        public string project { get; set; } = "PSDK"; //
        public string method { get; set; } = "BRM.Config.GetMqConfig"; //方法名
        public data data { get; set; } = new data(); //Json串

    }

    public class data
    {
        public string optional { get; set; } //url信息
    }

    public class SurveyMoudle
    {
        public string[] channelId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int sex { get; set; } //性别 1男 2女
        public string idCode { get; set; }
        public string photoBase64 { get; set; }
        public string initialTime { get; set; }
        public string expireTime { get; set; }
    }
}
