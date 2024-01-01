using BitMiracle.LibTiff.Classic;
using HotelManagement.DTOs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelManagement.View.Admin.RoomTypeFurnitureManagement
{
    /// <summary>
    /// Interaction logic for RoomTypeFurnitureManagementPage.xaml
    /// </summary>
    public partial class RoomTypeFurnitureManagementPage : Page
    {
        public RoomTypeFurnitureManagementPage()
        {
            InitializeComponent();
        }

        private void SearchBox_SearchTextChange(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_ListView.ItemsSource);
            if (view != null)
            {
                view.Filter = Filter;
                CollectionViewSource.GetDefaultView(_ListView.ItemsSource).Refresh();
            }
        }
        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as FurnitureRoomTypeDTO).RoomTypeName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as FurnitureRoomTypeDTO).RoomTypeId.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as FurnitureRoomTypeDTO).Price.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
