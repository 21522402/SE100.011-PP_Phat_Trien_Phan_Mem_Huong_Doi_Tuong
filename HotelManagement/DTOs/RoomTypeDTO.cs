using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RoomTypeDTO
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> MaxNumberGuest { get; set; }
        public Nullable<int> NumberGuestForUnitPrice { get; set; }
    }
}
