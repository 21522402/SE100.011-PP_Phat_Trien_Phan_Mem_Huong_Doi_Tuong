using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class ProductUsingDTO
    {
        public int ProductUsingId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string RentalContractId { get; set; }
       
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public double TotalMoney
        {
            get
            {
                double totalMoney = (double)UnitPrice * (int)Quantity;
                return totalMoney;
            }
        }
        public string TotalMoneyStr
        {
            get
            {
                return Helper.FormatVNMoney(TotalMoney);
            }
        }
        public string UnitPriceStr
        {
            get
            {
                return Helper.FormatVNMoney((double)UnitPrice);
            }
        }
    }
}
