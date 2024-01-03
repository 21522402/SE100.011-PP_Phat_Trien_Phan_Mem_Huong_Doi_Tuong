using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RevenueRoomTypeDTO
    {
        public string RentalContractId { get; set; }
        public Nullable<double> RentalPrice { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public string RoomId { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomType { get; set; }

        public int DayNumber
        {
            get
            { 
                TimeSpan t = (TimeSpan)(EndDate - StartDate);
                int res = (int)t.TotalDays;
                return res;
            }
        }
    }
}
