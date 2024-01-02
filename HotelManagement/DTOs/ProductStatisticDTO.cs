using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class ProductStatisticDTO
    {
        public string ProductName { get; set; }
        public int STT { get; set; }
        public double Revenue { get; set; }
        public string RevenueStr
        {
            get { return Helper.FormatVNMoney(Revenue); }
        }
        public double Ratio { get; set; }
    }
}