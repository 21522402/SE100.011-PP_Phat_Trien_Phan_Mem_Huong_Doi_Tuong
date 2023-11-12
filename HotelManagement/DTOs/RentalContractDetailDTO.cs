using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RentalContractDetailDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public string RentalContractId { get; set; }
        public int RentalContractDetailId { get; set; }
  
        private string _CustomerName;
        public string CustomerName
        {
            get { return _CustomerName; }
            set
            {
                this._CustomerName = value;
                this.NotifyPropertyChanged("CustomerName");
            }
        }
        private int _STT;
        public int STT
        {
            get { return _STT; }
            set
            {
                this._STT = value;
                this.NotifyPropertyChanged("STT");
            }
        }

        private string _CustomerId;
        public string CustomerId
        {
            get { return _CustomerId; }
            set
            {
                this._CustomerId = value;
                this.NotifyPropertyChanged("CCCD");
            }
        }
       
    }
}
