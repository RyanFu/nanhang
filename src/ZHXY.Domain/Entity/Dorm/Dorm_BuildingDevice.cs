using System;

namespace ZHXY.Domain
{
    /// <summary>
    /// �����豸
    /// </summary>
    public partial class Device: IEntity
    {
        public string Id { get; set; }

        /// <summary>
        /// �豸��
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// �豸����
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? CreatorTime { get; set; }
    }
}
