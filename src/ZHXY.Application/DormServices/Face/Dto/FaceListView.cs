﻿using System;

namespace ZHXY.Application
{
    /// <summary>
    /// 头像审批列表Dto
    /// </summary>
    public class FaceListView
    {
        /// <summary>
        /// 请假Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplierName { get; set; }

        /// <summary>
        /// 审批前头像
        /// </summary>
        public string SubmitImg { get; set; }

        /// <summary>
        /// 提交后头像
        /// </summary>
        public string ApproveImg { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime CreatorTime { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime ApproveTime { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public string ApprovalStatus { get; set; }

       

      
    }

    public class FaceView : FaceListView
    {
        public bool IsFinal { get; set; }
    }

}