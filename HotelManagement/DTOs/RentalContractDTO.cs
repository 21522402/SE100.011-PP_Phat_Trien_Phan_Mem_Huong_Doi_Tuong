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
        public Nullable<System.DateTime> StartDate { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string RoomId { get; set; }
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
        public int PersonNumber { get; set; }
        public string PersonNumberStr
        {
            get { return PersonNumber.ToString(); }
        }
        public IList<CustomerDTO> CustomersOfRoom { get; set; }
        public string StartDateStr
        {
            get { return ((DateTime)StartDate).ToString("dd/MM/yyyy"); }
        }

        public int DayNumber
        {
            get
            {
                if (StartDate == null)
                {
                    return 0;
                }
                else
                {
                    if (!(bool)Validated) return 0;
                }
                TimeSpan t = (TimeSpan)(EndDate - StartDate);
                int res = (int)t.TotalDays;
                return res;
            }
        }

        public IList<RentalContractDetailDTO> ListRentalContractDetails { get; set; }

    }
}
