using System;
namespace ZHXY.Domain
{
    /// <summary>
    /// ¥����Ϣ
    /// </summary>   
    public class Building : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").ToUpper();
        public string BuildingNo { get; set; }
    }
}