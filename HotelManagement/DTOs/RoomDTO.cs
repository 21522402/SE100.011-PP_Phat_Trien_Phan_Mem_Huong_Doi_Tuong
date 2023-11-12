using HotelManagement.Utilities;
using HotelManagement.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HotelManagement.DTOs
{
    public class RoomDTO
    {
        public string RoomId { get; set; }
        public Nullable<int> RoomNumber { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string Note { get; set; }
        public string RoomStatus { get; set; }
        public double Price { get; set; }
        public double RentalPrice { get; set; }
        public int CountPerson { get; set; }
        public string RentalContractId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

        public string DaysLeft
        {
            get
            {
                if (StartDate == null)
                {
                    return "";
                }
                TimeSpan leftTime = ((TimeSpan)(EndDate + TimeSpan.FromHours(12) - (DateTime.Today + DateTime.Now.TimeOfDay)));
                int days = (int)leftTime.TotalDays;
                if (days < 1)
                {
                    int hours = (int)leftTime.TotalHours;
                    if (hours < 1)
                    {
                        int minutes = (int)leftTime.TotalMinutes;
                        return $"Còn {minutes} phút";
                    }
                    return $"Còn {hours} giờ";
                }
                return $"Còn {days} ngày";
            }
        }
        public string colorLeftDayBar
        {
            get
            {
                if (StartDate == null) { return null; }
                TimeSpan leftTime = ((TimeSpan)(EndDate + TimeSpan.FromHours(12) - (DateTime.Today + DateTime.Now.TimeOfDay)));
                int hours = (int)leftTime.TotalHours;
                if (hours < 12)
                {
                    return "#F68A73";
                }
                return  "#59D66D";

            }
        }
        public string RoomBackground
        {
            get
            {
                string res = "";
                if (RoomStatus == ROOM_STATUS.READY)
                {
                    res = "#59D66D";
                }
                else {
                    res = "#72B6DC";
                    TimeSpan leftTime = ((TimeSpan)(EndDate + TimeSpan.FromHours(12) - (DateTime.Today + DateTime.Now.TimeOfDay)));
                    if (leftTime.TotalMinutes < 1)
                    {
                        res = "#F68A73";
                    }
                }
                
                return res;

            }
        }
        public string RoomIcon
        {
            get
            {
                if (RoomStatus == ROOM_STATUS.READY)
                {
                    return "Check";
                }
                else return "Account";

            }
        }
        public double PercentLeft
        {
            get
            {   if (StartDate == null) { return 0; }
                TimeSpan difference = (TimeSpan)(EndDate - StartDate);
                double totalTime = (double)difference.TotalMinutes;
                double leftTime = ((TimeSpan)(EndDate + TimeSpan.FromHours(12) - (DateTime.Today + DateTime.Now.TimeOfDay))).TotalMinutes;
                return (1 - leftTime / totalTime)*180;
            }
        }
        public string RoomShowLeftBar
        {
            get
            {
                if (RoomStatus == ROOM_STATUS.READY)
                {
                    return "Collapsed";
                }
                else return "Visible";


            }
        }
        public string RoomPriceStr
        {
            get { return Helper.FormatVNMoney(Price); }
        }
        public string RoomName
        {
            get { return "P" + RoomNumber.ToString(); }
        }

        public int DayNumber
        {
            get
            {
                if ( StartDate == null)
                {
                    return 0;
                
                }
                TimeSpan t = (TimeSpan)(EndDate - StartDate);
                int res = (int)t.TotalDays;
                return res;
            }
        }
    }
}
