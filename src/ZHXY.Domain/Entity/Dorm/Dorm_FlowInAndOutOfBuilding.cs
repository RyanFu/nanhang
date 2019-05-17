using System;

namespace ZHXY.Domain
{
    /// <summary>
    /// ����������ˮ
    /// </summary>
    public partial class LDJCLS:IEntity
    {
        /// <summary>
        /// ��ˮId
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// ¥����
        /// </summary>
        public string BuildingNo { get; set; }

        /// <summary>
        /// �û�Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public bool? Direction { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? OccurrenceTime { get; set; }

        /// <summary>
        /// �Ƿ��쳣
        /// </summary>
        public bool? IsAbnormal { get; set; }
    }
}
