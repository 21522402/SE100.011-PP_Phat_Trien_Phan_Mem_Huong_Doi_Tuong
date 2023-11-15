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

namespace HotelManagement.View.Admin.ProductManagement
{
    /// <summary>
    /// Interaction logic for ProductManagementPage.xaml
    /// </summary>
    public partial class ProductManagementPage : Page
    {
        public ProductManagementPage()
        {
            InitializeComponent();
        }
        private void Search_SearchTextChange(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProducts.ItemsSource);
            if (view != null)
            {
                view.Filter = Filter;
                CollectionViewSource.GetDefaultView(ListViewProducts.ItemsSource).Refresh();
            }
        }
        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as ProductDTO).ProductName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)||
                     ((item as ProductDTO).ProductType.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Grid a = sender as Grid;
            Border b = (Border)a.FindName("Mask");
            Button ib = (Button)a.FindName("ImportButton");
            ib.Visibility = Visibility.Visible;
            b.Visibility = Visibility.Visible;
            b.Opacity = 0.25;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid a = sender as Grid;
            Border b = (Border)a.FindName("Mask");
            Button ib = (Button)a.FindName("ImportButton");
            ib.Visibility = Visibility.Collapsed;
            b.Visibility = Visibility.Collapsed;
            b.Opacity = 0;
        }
    }
}
