using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class WarehouseDto
    {
        public ulong Id { get; set; }
        public long? FactoryId { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public int? WarehouseType { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
    }

    public class WarehouseLocationDto
    {
        public ulong Id { get; set; }
        public long? WarehouseZoneId { get; set; }
        public string? WarehouseLocationCode { get; set; }
        public string? WarehouseLocationName { get; set; }
        public decimal? MaxCapacity { get; set; }
        public bool? IsPickingLocation { get; set; }
        public bool? IsActive { get; set; }
    }
}
