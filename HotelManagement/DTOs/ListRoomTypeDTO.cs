using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class ListRoomTypeDTO
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public IList<RoomDTO> Rooms { get; set; }
        public double Price { get; set; }
        public string PriceStr
        {
            get
            {
                return Helper.FormatVNMoney(Price);
            }
        }
    }
}
