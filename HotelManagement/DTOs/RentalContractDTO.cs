using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RentalContractDTO: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public string RentalContractId { get; set; }
        public string RoomId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int RoomNumber { get; set; }
        public float RentalPrice { get; set; }
        public bool Validated { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr
        {
            get { return ((DateTime)CreateDate).ToString("dd/MM/yyyy"); }
        }
        private DateTime _EndDate;
        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                this._EndDate = value;
                this.NotifyPropertyChanged("EndDate");
            }
        }
        public string EndDateStr
        {
            get { return ((DateTime)EndDate).ToString("dd/MM/yyyy"); }
            
        }
       
        public IList<RentalContractDetailDTO> ListRentalContractDetails { get; set; }

    }
}
