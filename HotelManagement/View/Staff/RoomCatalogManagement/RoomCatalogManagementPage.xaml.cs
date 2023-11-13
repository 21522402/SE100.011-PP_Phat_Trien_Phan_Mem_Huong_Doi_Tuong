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

namespace HotelManagement.View.Staff.RoomCatalogManagement
{
    /// <summary>
    /// Interaction logic for RoomCatalogManagementPage.xaml
    /// </summary>
    public partial class RoomCatalogManagementPage : Page
    {
        List<ListBox> listRoomList;
        ListBox listRoom;
        public RoomCatalogManagementPage()
        {
            InitializeComponent();
            listRoomList = new List<ListBox>();
            listRoom = new ListBox();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void SearchBox_SearchTextChange(object sender, EventArgs e)
        {
            for (int i = 0; i < listRoomList.Count; i++)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listRoomList[i].ItemsSource);
                if (view != null)
                {
                    view.Filter = Filter;
                    CollectionViewSource.GetDefaultView(listRoomList[i].ItemsSource).Refresh();
                }
            }
        }
        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as RoomDTO).RoomName.ToString().IndexOf(SearchBox.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as RoomDTO).RoomTypeName.ToString().IndexOf(SearchBox.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
        }

       

        private void listRoom_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox a = sender as ListBox;
            if (a != null)
                listRoomList.Add(a);
        }

        private void listListRoomType_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox a = sender as ListBox;
        }
       

        //private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ListBoxItem lbi = sender as ListBoxItem;
        //    listRoom1.SelectedItem = lbi.DataContext;
        //}

        //private void ListBoxItem2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ListBoxItem lbi = sender as ListBoxItem;
        //    listRoom2.SelectedItem = lbi.DataContext;
        //}

        //private void ListBoxItem3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ListBoxItem lbi = sender as ListBoxItem;
        //    listRoom3.SelectedItem = lbi.DataContext;
        //}
    }
}
