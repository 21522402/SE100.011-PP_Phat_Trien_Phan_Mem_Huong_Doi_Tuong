using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class ImportReceiptDTO
    {
        public string ReceiptId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public Nullable<double> TotalPrice { get; set; }
        public Nullable<int> TotalQuality { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public int typeImport { get; set; }// 0: product, 1: funiture
        public IList<ImportReceiptDetailDTO> Details { get; set; }

        public string typeImprtStr
        {
            get {
                if (typeImport==0) return "Phiếu Nhập Sản phẩm";
                else return "Phiếu Nhập Tiện nghi";
            }
        }
        public string ReceiptIdStr
        {
            get
            {
                return "PN" + ReceiptId;
            }
        }
        public string TotalPriceStr
        {
            get
            {
                return Helper.FormatVNMoney((double)TotalPrice);
            }
        }
    }
}
