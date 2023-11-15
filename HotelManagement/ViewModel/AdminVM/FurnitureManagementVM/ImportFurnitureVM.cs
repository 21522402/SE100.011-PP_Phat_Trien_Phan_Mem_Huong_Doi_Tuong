using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.Admin;
using HotelManagement.View.Admin.FurnitureManagement;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static HotelManagement.Utilities.Helper;

namespace HotelManagement.ViewModel.AdminVM.FurnitureManagementVM
{
    public partial class FurnitureManagementVM : BaseVM
    {
        //public string ImportQuantity { get; set; }
        //public string ImportPrice { get; set; }
        //public DateTime CreateDate { get; set; }
        //public string CreateDateString { get; set; }

        private ObservableCollection<FurnitureDTO> orderFurnitureList;
        public ObservableCollection<FurnitureDTO> OrderFurnitureList
        {
            get { return orderFurnitureList; }
            set { orderFurnitureList = value; OnPropertyChanged(); }
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
        public async Task ImportFurniture(FurnitureDTO furnitureSelected, Window wd, AdminWindow mainWD)
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

                furnitureCache.ImportPrice = furnitureSelected.ImportPrice = price;
                furnitureCache.ImportQuantity = furnitureSelected.ImportQuantity = quantity;

                (bool isSuccess, string messageReturn) = await Task.Run(() => FurnitureService.Ins.ImportFurniture(furnitureSelected, furnitureCache.ImportPrice, furnitureCache.ImportQuantity));
                if(isSuccess)
                {
                    CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    furnitureCache.Quantity = furnitureCache.Quantity + int.Parse(ImportQuantity);
                    LoadFurnitureListView(Operation.UPDATE_PROD_QUANTITY, furnitureCache);
                    wd.Close();
                
                    mainWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
                }    
                else
                {
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
                ImportQuantity = null;
                ImportPrice = null;
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
            foreach (var item in OrderFurnitureList)
            {
                total += (item.ImportQuantity * item.ImportPrice);
            }
            TotalImportPrice = total;
            TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
        }

        public void InitQuantity()
        {

            foreach (var item in FurnitureList)
            {
                item.ImportQuantity = 0;
                item.ImportPrice = 0;
            }
        }

        public async Task ImportListFurniture(Window wd, AdminWindow mainWD)
        {
            (bool isSuccess, string messageReturn, List<FurnitureDTO> listReturned) = await Task.Run(() => FurnitureService.Ins.ImportListFurniture(OrderFurnitureList));
            if (isSuccess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                for (int i = 0; i < listReturned.Count; i++)
                    LoadFurnitureListView(Operation.UPDATE_PROD_QUANTITY, listReturned[i]);
                OrderFurnitureList.Clear();
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            ImportListFurnitureWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListFurnitureWindow>().FirstOrDefault();
            wd.Close();
            ipWD.Close();
            mainWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
        }
        public string DateTimeToString(DateTime dt)
        {
            string date;
            date = dt.Day + "/" + dt.Month + "/" + dt.Year + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
            return date;
        }
    }
}
