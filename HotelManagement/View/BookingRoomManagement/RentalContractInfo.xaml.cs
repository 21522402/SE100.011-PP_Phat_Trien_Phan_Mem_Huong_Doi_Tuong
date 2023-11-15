using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagement.View.BookingRoomManagement
{
    /// <summary>
    /// Interaction logic for RentalContractInfo.xaml
    /// </summary>
    public partial class RentalContractInfo : Window
    {
        public RentalContractInfo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog= new PrintDialog();
                btnPrint.Visibility= Visibility.Hidden;
                btnExit.Visibility= Visibility.Hidden;
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(rentalView, "RentalContract");
                }
            }
            finally 
            {
                this.IsEnabled = true;
                CustomMessageBox.ShowOk("In thành công!", "Thông báo", "Xác nhận", CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                this.Close();
            }
        }
    }
}
