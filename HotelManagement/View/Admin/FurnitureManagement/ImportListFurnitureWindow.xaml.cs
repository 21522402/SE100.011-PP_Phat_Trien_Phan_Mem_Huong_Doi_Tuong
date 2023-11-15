using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagement.View.Admin.FurnitureManagement
{
    /// <summary>
    /// Interaction logic for ImportListFurnitureWindow.xaml
    /// </summary>
    public partial class ImportListFurnitureWindow : Window
    {
        public ImportListFurnitureWindow()
        {
            InitializeComponent();
        }
        private void FurnitureImportWD_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchBox_SearchTextChange(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewFurniture.ItemsSource);
            if (view != null)
            {
                view.Filter = Filter;
                CollectionViewSource.GetDefaultView(ListViewFurniture.ItemsSource).Refresh();
            }
        }
        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as FurnitureDTO).FurnitureName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((item as FurnitureDTO).FurnitureType.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void ItemFurniture_MouseMove(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Rectangle rec = (Rectangle)grid.FindName("Mask");
            rec.Opacity = 0.1;
        }

        private void ItemFurniture_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Rectangle rec = (Rectangle)grid.FindName("Mask");
            rec.Opacity = 0;
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "") tb.Text = "0";

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "0") tb.Text = "";
        }

        private void tb_Import_Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void tb_Import_Quantity_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "") tb.Text = "1";
        }
        private void outer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
