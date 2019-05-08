using System;
namespace ZHXY.Domain
{
    /// <summary>
    /// ¥����Ϣ
    /// </summary>   
    public class Building : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").ToUpper();
        public string Title { get; set; }
        public string Area { get; set; }
        public string BuildingNo { get; set; }
        public string FloorNum { get; set; }
        public string UnitNum { get; set; }
        public string Address { get; set; }
        public string BuildingType { get; set; }//��ѧ¥���칫¥��ѧ�����ᡢ��ְ�����ᡢʳ�á�����
        public string BuildingStatus { get; set; }//����ʹ�á������С���ͣ��
    }
}