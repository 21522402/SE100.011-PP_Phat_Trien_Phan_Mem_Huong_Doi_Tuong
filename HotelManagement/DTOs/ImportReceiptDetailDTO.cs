using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class ImportReceiptDetailDTO : INotifyPropertyChanged
    {
        public string ImportReceiptDetailId { get; set; }
        public string ImportReceiptId { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        
        public Nullable<double> ImportPrice { get; set; }
        public Nullable<int> Quantity { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public string ImportPriceStr
        {
            get
            {
                return Helper.FormatVNMoney((double)ImportPrice);
            }
        }
        public string totalPriceStr
        {
            get
            {
                return Helper.FormatVNMoney((double)((double)ImportPrice*Quantity));
            }
        }
    }
}
