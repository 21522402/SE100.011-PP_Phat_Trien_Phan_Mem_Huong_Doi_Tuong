using HotelManagement.DTOs;
using HotelManagement.Model;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.Admin;
using HotelManagement.View.Admin.ProductManagement;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace HotelManagement.ViewModel.AdminVM.ProductManagementVM
{
    public partial class ProductManagementVM
    {
        public string ImportPrice { get; set; }
        public string ImportQuantity { get; set; }
        

        private ObservableCollection<ProductDTO> orderProductList;
        public ObservableCollection<ProductDTO> OrderProductList
        {
            get { return orderProductList; }
            set { orderProductList = value; OnPropertyChanged(); }
        }

        private double totalImportPrice;
        public double TotalImportPrice
        {
            get { return totalImportPrice; }
            set { totalImportPrice = value; OnPropertyChanged(); }
        }

        public string totalImportPriceStr { get; set; }
        public string TotalImportPriceStr
        {
            get { return totalImportPriceStr; }
            set { totalImportPriceStr = value; OnPropertyChanged(); }
        }
        public async Task ImportProduct(ProductDTO ProductSelected, Window wd, AdminWindow mainWD)
        {
            try
            {
                if (string.IsNullOrEmpty(ImportQuantity))
                {
                    CustomMessageBox.ShowOk("Vui lòng nhập số lượng", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(ImportPrice))
                {
                    CustomMessageBox.ShowOk("Vui lòng nhập giá nhập", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }

                

                int quantity;
                double price;
                bool isIntQuantity = Int32.TryParse(ImportQuantity, out quantity);
                bool isDoublePrice = double.TryParse(ImportPrice, out price);

                if (!isIntQuantity || quantity <= 0)
                {
                    CustomMessageBox.ShowOk("Số lượng là một số nguyên dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }
                if (!isDoublePrice || price <= 0)
                {
                    CustomMessageBox.ShowOk("Giá nhập phải là số dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }

                ProductCache.ImportPrice = ProductSelected.ImportPrice = price;
                ProductCache.ImportQuantity = ProductSelected.ImportQuantity = quantity;

                (bool isSuccess, string messageReturn) = await Task.Run(() => ProductService.Ins.ImportProduct(ProductSelected, ProductCache.ImportPrice, ProductCache.ImportQuantity));
                if (isSuccess)
                {
                    CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    ProductCache.Quantity = ProductCache.Quantity + ProductCache.ImportQuantity;
                    LoadProductListView(Operation.UPDATE_PROD_QUANTITY, new ProductDTO(ProductCache));
                    wd.Close();
                    mainWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            }
            catch (EntityException e)
            {
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }

        public void CalculateTotalPrice()
        {
            double total = 0;
            foreach (var item in OrderProductList)
            {
                total += (item.ImportQuantity * item.ImportPrice);
            }
            TotalImportPrice = total;
            TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
        }

        public void InitQuantity()
        {
            
            foreach (var item in ProductList)
            {
                item.ImportQuantity = 0;
                item.ImportPrice = 0;
            }
        }

        public async Task ImportListProduct(Window wd, AdminWindow mainWD)
        {
            (bool isSuccess, string messageReturn, List<ProductDTO> listReturned) = await Task.Run(() => ProductService.Ins.ImportListProduct(OrderProductList));
            if (isSuccess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                for (int i = 0; i < listReturned.Count; i++)
                    LoadProductListView(Operation.UPDATE_PROD_QUANTITY, listReturned[i]);
                ImportListProductWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListProductWindow>().FirstOrDefault();
                OrderProductList.Clear();
                TotalImportPrice = 0;
                TotalImportPriceStr = "";
                wd.Close();
                ipWD.Close();
                mainWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            
        }
    }
}
