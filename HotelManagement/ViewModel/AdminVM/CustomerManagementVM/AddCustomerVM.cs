using HotelManagement.View.CustomMessageBoxWindow;
using System;
using System.Threading.Tasks;
using HotelManagement.Utilities;
using Window = System.Windows.Window;
using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Utils;
using System.Linq;

namespace HotelManagement.ViewModel.AdminVM.CustomerManagementVM
{
    public partial class CustomerManagementVM : BaseVM
    {
     

        private (bool isvalid, string error)  IsValidData()
        {
            if(String.IsNullOrEmpty(FullName)|| String.IsNullOrEmpty(Phonenumber)  || String.IsNullOrEmpty(Address)||String.IsNullOrEmpty(Cccd)|| Gender is null || CustomerType is null|| Birthday is null)
            {
                return (false, "Vui lòng nhập đủ thông tin khách hàng!");
            }
            (bool isv, string err)= IsValidAge((DateTime)Birthday);
            if (!isv) return (false, err);
            if (!Helper.IsPhoneNumber(Phonenumber)) return (false, "Số điện thoại không hợp lệ!");
            return (true, null);
        }
        private (bool isvalid, string err)  IsValidAge( DateTime birthday)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - birthday.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (birthday.DayOfYear > today.DayOfYear) age--;

            if (age < 16) return (false,  "Khách hàng chưa đủ 16 tuổi!" );
            return (true, null);
        }
    }
}
