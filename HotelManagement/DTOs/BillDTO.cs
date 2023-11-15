using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class BillDTO
    {
        public string BillId { get; set; }
        public string RentalContractId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomName
        {
            get
            {
                return "Phòng " + RoomNumber;
            }
        }
        public int PersonNumber { get; set; }
        public Nullable<double> RentalPrice { get; set; }
        public string RentalPriceStr
        {
            get
            {
                return Helper.FormatVNMoney((double)RentalPrice);
            }

        }
        public Nullable<double> TotalPrice { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int DayNumber
        {
            get
            {
                if (EndDate == null || StartDate == null)
                {
                    return 0;
                }

                TimeSpan t = (TimeSpan)(EndDate - StartDate);
                int res = (int)t.TotalDays;
                return res;
            }
        }
        public IList<ProductUsingDTO> ListListProductPayment { get; set; }


        public double ProductPriceTemp
        {
            get
            {
                double t = 0;
                foreach (var item in ListListProductPayment)
                {
                    t += item.TotalMoney;
                }
                return t;
            }

        }
        public string ProductPriceTempStr
        {
            get
            {
                return Helper.FormatVNMoney(ProductPriceTemp);
            }

        }

        public double TotalPriceTemp
        {
            get
            {
                return ProductPriceTemp + DayNumber * (double)RentalPrice;
            }
        }
        public string TotalPriceTempStr
        {
            get
            {
                return Helper.FormatVNMoney(TotalPriceTemp);
            }

        }

    }
}