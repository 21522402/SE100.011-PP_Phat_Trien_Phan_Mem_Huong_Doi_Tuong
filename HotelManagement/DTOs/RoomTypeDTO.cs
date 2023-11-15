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
        public double RoomTypePrice { get; set; }
        public bool IsDeleted { get; set; }
        public int MaxNumberGuest { get; set; }
        public int NumberGuestForUnitPrice { get; set; }
        public string RoomTypePriceStr
        {
            get { return Helper.FormatVNMoney(RoomTypePrice); }
        }
        public IList<SurchargeFeeDTO> ListSurcharges { get; set; }

        public IList<RoomDTO> Rooms { get; set; }

        public double Revenue { get; set; }
        public double Ratio { get; set; }
        public int STT { get; set; }
    }
}
