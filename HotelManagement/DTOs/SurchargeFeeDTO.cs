using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class SurchargeFeeDTO
    {
        public string SurchargeId { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int CustomerOutIndex { get; set; }
        public double Rate { get; set; }
        public string RateStr { get; set; }
    }
}
