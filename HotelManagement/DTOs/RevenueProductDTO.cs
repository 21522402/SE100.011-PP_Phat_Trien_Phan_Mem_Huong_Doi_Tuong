using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RevenueProductDTO
    {
        public string RentalContractId { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
