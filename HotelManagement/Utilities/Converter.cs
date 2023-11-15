using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HotelManagement.Utilities
{
    public class Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert the int to a string:
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert the string back to an int here
            string vl = value.ToString();
            if (vl.Length > 3)
            {
                CustomMessageBox.ShowOk("Vui lòng không nhập quá 1000 sản phẩm cùng lúc", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return 1000;
            }

            if (value.ToString() == "") return 0;
            return int.Parse(value.ToString());
        }
    }
    public class Converter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert the int to a string:
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert the string back to an int here
            try
            {
                if (value.ToString() == "") return 0;
                return double.Parse(value.ToString());
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowOk("Giá trị nhập của bạn vượt quá lớn", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return double.Parse(value.ToString().Substring(0, value.ToString().Length - 2));
            }
        }
    }
}
