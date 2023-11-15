using HotelManagement.DTOs;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.View.Admin;
using HotelManagement.Utils;
using static HotelManagement.Utilities.Helper;
using IronXL.Formatting;
using HotelManagement.Model.Services;

namespace HotelManagement.ViewModel.AdminVM.ProductManagementVM
{
    public partial class ProductManagementVM
    {
        public List<string> filterSource { get; set; }
        public string SalePrice { get; set; }
        public string SalePriceProduct { get; set; }
        public async Task SaveEditProduct(ProductDTO ProductDTO, Window wd, AdminWindow adWD)
        {
            if (string.IsNullOrEmpty(ProductDTO.ProductName))
            {
                CustomMessageBox.ShowOk("Vui lòng nhập tên sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(ProductDTO.ProductType))
            {
                CustomMessageBox.ShowOk("Vui lòng chọn loại sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SalePrice))
            {
                CustomMessageBox.ShowOk("Vui lòng nhập giá bán sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            double salePrice;
            bool isIntSalePrice = double.TryParse(SalePrice, out salePrice);
            if (!isIntSalePrice || salePrice <= 0)
            {
                CustomMessageBox.ShowOk("Vui lòng nhập một số dương cho giá sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            ProductDTO.ProductPrice = salePrice;
            (bool isSucess, string messageReturn) = await Task.Run(() => ProductService.Ins.SaveEditProduct(ProductDTO));

            if (isSucess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                LoadProductListView(Operation.UPDATE, ProductDTO);
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            wd.Close();
            adWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
            SalePrice = null;
        }
        public async Task SaveEditCleanProduct(ProductDTO ProductDTO, Window wd)
        {
            double price;
            bool IsNumber = double.TryParse(SalePriceProduct, out price);

            if (!IsNumber || price <= 0)
            {
                CustomMessageBox.ShowOk("Vui lòng nhập một số dương cho giá dịch vụ", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }

            ProductCache.ProductPrice = ProductDTO.ProductPrice = price;

            (bool isSucess, string messageReturn) = await Task.Run(() => ProductService.Ins.SaveEditProduct(ProductDTO));

            if (isSucess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                ProductCache.FormatStringUnitAndPrice();
                LoadProductListView(Operation.UPDATECLEAN, ProductDTO);
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            wd.Close();
            SalePriceProduct = null;
        }
        public async Task AddProduct(ProductDTO productCache, Window wd, AdminWindow adWD)
        {
            if (string.IsNullOrEmpty(productCache.ProductName))
            {
                CustomMessageBox.ShowOk("Vui lòng nhập tên sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(productCache.ProductType))
            {
                CustomMessageBox.ShowOk("Vui lòng chọn loại sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SalePrice))
            {
                CustomMessageBox.ShowOk("Vui lòng nhập giá bán sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            double salePrice;
            bool isIntSalePrice = double.TryParse(SalePrice, out salePrice);
            if (!isIntSalePrice || salePrice <= 0)
            {
                CustomMessageBox.ShowOk("Vui lòng nhập một số dương cho giá sản phẩm", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            productCache.ProductPrice = salePrice;
            (bool isSucess, string messageReturn) = await Task.Run(() => ProductService.Ins.AddProduct(productCache));

            if (isSucess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                LoadProductListView(Operation.CREATE, productCache);
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            ProductCache = null;
            wd.Close();
            adWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
            SalePrice = null;
        }
    }
}
