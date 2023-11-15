using CinemaManagementProject.Utilities;
using HotelManagement.Components.Search;
using HotelManagement.DTOs;
using HotelManagement.Model;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.Admin;
using HotelManagement.View.Admin.FurnitureManagement;
using HotelManagement.View.Admin.FurnitureManagement;
using HotelManagement.View.CustomMessageBoxWindow;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HotelManagement.ViewModel.AdminVM.FurnitureManagementVM
{
    public partial class FurnitureManagementVM : BaseVM
    {
        private bool isChanged;
        public bool IsChanged { get { return isChanged; } set { isChanged = value; OnPropertyChanged(); } }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }
        private bool isLoaddingIP;
        public bool IsLoaddingIP
        {
            get { return isLoaddingIP; }
            set { isLoaddingIP = value; OnPropertyChanged(); }
        }
        private string _staffname;
        public string StaffName
        {
            get { return _staffname; }
            set { _staffname = value; OnPropertyChanged(); }
        }


        private ObservableCollection<FurnitureDTO> furnitureList;
        public ObservableCollection<FurnitureDTO> FurnitureList
        {
            get { return furnitureList; }
            set { furnitureList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FurnitureDTO> allFurniture;
        public ObservableCollection<FurnitureDTO> AllFurniture
        {
            get { return allFurniture; }
            set { allFurniture = value; OnPropertyChanged(); }
        }

        private FurnitureDTO selectedFurniture;
        public FurnitureDTO SelectedFurniture
        {
            get { return selectedFurniture; }
            set { selectedFurniture = value; OnPropertyChanged(); }
        }

        private ComboBoxItem _selectedItemFilter;
        public ComboBoxItem SelectedItemFilter
        {
            get => _selectedItemFilter;
            set
            {
                _selectedItemFilter = value;
                OnPropertyChanged();
            }
        }

        public ICommand FirstLoadCM { get; set; }
        public ICommand CloseCM { get; set; }
        public ICommand OpenImagesWindowCM { get; set; }

        //Lọc tiện nghi
        public ICommand SelectionFilterChangeCM { get; set; }

        //Sửa tiện nghi
        public ICommand OpenEditFurnitureCM { get; set; }
        public ICommand SaveEditFurnitureCM { get; set; }
        public ICommand CloseEditFurnitureCM { get; set; }

        //Thêm tiện nghi
        public ICommand OpenAddFurnitureCM { get; set; }
        public ICommand AddFurnitureCM { get; set; }
        public ICommand CloseAddFurnitureCM { get; set; }

        //Xóa tiện nghi
        public ICommand DeleteFurnitureCM { get; set; }

        //Chi tiết tiện nghi
        public ICommand OpenInfoFurnitureCM { get; set; }
        public ICommand CloseInfoFunitureCM { get; set; }

        //Nhập tiện nghi
        public ICommand OpenImportFurnitureCM { get; set; }
        public ICommand ImportFurnitureCM { get; set; }
        public ICommand CloseImportFurnitureCM { get; set; }

        //Nhập danh sách
        // Nhập danh sách

        public ICommand OpenImportListFurnitureCM { get; set; }
        public ICommand FirstLoadImportListFurnitureWindowCM { get; set; }
        public ICommand CloseImportListWindowCM { get; set; }
        public ICommand ChooseFurnitureToListCM { get; set; }
        public ICommand DecreaseQuantityOrderItem { get; set; }
        public ICommand IncreaseQuantityOrderItem { get; set; }
        public ICommand OpenReviewImportListFurnitureCM { get; set; }
        public ICommand DeleteItemInBillStackCM { get; set; }
        public ICommand ClosePreviewImportListCM { get; set; }
        public ICommand ImportListFurnitureCM { get; set; }
        public ICommand ImportPriceChangeCM { get; set; }
        public ICommand ImportQuantityChangeCM { get; set; }

        public FurnitureManagementVM()
        {

            AdminWindow tk = System.Windows.Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();
            if (AdminVM.CurrentStaff != null)
                StaffName = AdminVM.CurrentStaff.StaffName;
            else
                StaffName = "CurrentAdmin";
            AllFurnitureType = new List<string>();
            AllFurnitureType.Add("Nội thất");
            AllFurnitureType.Add("Tiện ích");
            FirstLoadCM = new RelayCommand<System.Windows.Controls.Page>((p) => { return true; }, async (p) =>
            {
                IsChanged = false;
                IsLoading = true;
                (bool isSuccess, string messageReturn, List<FurnitureDTO> listFurniture) = await Task.Run(() => FurnitureService.Ins.GetAllFurniture());
                IsLoading = false;

                if (isSuccess)
                {
                    AllFurniture = new ObservableCollection<FurnitureDTO>(listFurniture);
                    FurnitureList = new ObservableCollection<FurnitureDTO>(AllFurniture);
                }
                else
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi" , "OK");
            });

            OpenEditFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if(SelectedFurniture != null)
                {
                    furnitureCache = new FurnitureDTO(SelectedFurniture);
                    furnitureCache.DisplayQuantity = furnitureCache.Quantity.ToString();
                    FurnitureEditWindow furnitureEditWD = new FurnitureEditWindow();
                    tk.MaskOverSideBar.Visibility = Visibility.Visible;
                    furnitureEditWD.ShowDialog();
                }
            });

            CloseEditFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                furnitureCache = null;
            });

            SaveEditFurnitureCM = new RelayCommand<System.Windows.Window>((p) => { return true; },async (p) =>
            {
                if(furnitureCache != null)
                {
                    if(furnitureCache.IsEmptyFurniture())
                    {
                        if(CustomMessageBox.ShowOk("Vui lòng nhập đầy đủ thông tin", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning) == CustomMessageBoxResult.OK)
                            return;
                    }
                    (bool isSuccess, string messageReturn) = await Task.Run(() => FurnitureService.Ins.SaveEditFurniture(furnitureCache));
                    if(isSuccess)
                    {
                        CustomMessageBox.ShowOkCancel(messageReturn, "Thành công", "Ok", "Hủy", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                        LoadFurnitureListView(Operation.UPDATE, furnitureCache);
                    }
                    else
                    {
                        CustomMessageBox.ShowOk(messageReturn, "Lỗi", "Ok", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                    }
                    p.Close();  
                    tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                    furnitureCache = null;
                }
            });

            SelectionFilterChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedItemFilter != null)
                {
                    if (SelectedItemFilter.Tag.ToString() == "All")
                        FurnitureList = new ObservableCollection<FurnitureDTO>(AllFurniture);
                    else
                        FurnitureList = new ObservableCollection<FurnitureDTO>(FurnitureService.Ins.GetAllFurnitureByType(SelectedItemFilter.Content.ToString(), AllFurniture));
                }
            });
            OpenImagesWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.Title = "Select an image";
                openfile.Filter = "Image File (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg; *.png";
                if (openfile.ShowDialog() == true)
                {
                    furnitureCache.SetAvatar(openfile.FileName);
                    return;
                }
            });

            OpenAddFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                furnitureCache = new FurnitureDTO();
                FurnitureAddWindow furnitureAddWD = new FurnitureAddWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                furnitureAddWD.ShowDialog();
            });
            AddFurnitureCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, async (p) => {
                if (furnitureCache != null)
                {
                    furnitureCache.DisplayQuantity = "0";
                    if (furnitureCache.IsEmptyFurniture())
                    {
                        if (CustomMessageBox.ShowOk("Vui lòng nhập đầy đủ thông tin", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning) == CustomMessageBoxResult.OK)
                            return;
                    }

                    (bool isSuccess, string messageReturn, string id) = await Task.Run(() => FurnitureService.Ins.AddFurniture(furnitureCache));
                    furnitureCache.FurnitureID = id;
                    if (isSuccess)
                    {
                        CustomMessageBox.ShowOkCancel(messageReturn, "Thành công", "Ok", "Hủy", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                        LoadFurnitureListView(Operation.CREATE, furnitureCache);
                    }
                    else
                    {
                        CustomMessageBox.ShowOkCancel(messageReturn, "Lỗi", "Ok", "Hủy", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                    }
                    p.Close();
                    tk.MaskOverSideBar.Visibility = Visibility.Collapsed;

                    furnitureCache.DisplayQuantity = null;
                }
            });
            CloseAddFurnitureCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                furnitureCache = null;
            });

            DeleteFurnitureCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = SelectedFurniture;
                if(CustomMessageBox.ShowOkCancel("Bạn có chắc chắn muốn xóa tiện nghi này không?","Cảnh báo", "Có", "Hủy", CustomMessageBoxImage.Warning)
                    == CustomMessageBoxResult.Cancel) 
                    return;

                (bool isSuccess, string messageReturn) = await Task.Run(() =>  FurnitureService.Ins.DeleteFurniture(furnitureCache));

                if (isSuccess)
                {
                    CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", CustomMessageBoxImage.Success);
                    LoadFurnitureListView(Operation.DELETE, furnitureCache);
                }
                else
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", CustomMessageBoxImage.Error);
            });

            OpenInfoFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = SelectedFurniture;
                FurnitureInfoWindow furnitureInfoWD = new FurnitureInfoWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                furnitureInfoWD.ShowDialog();
            });

            CloseInfoFunitureCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                furnitureCache = null;
            });

            OpenImportFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = new FurnitureDTO(SelectedFurniture);
                FurnitureImportWindow furnitureImportWD = new FurnitureImportWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                furnitureImportWD.ShowDialog();
            });

            ImportFurnitureCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoaddingIP = true;
                await ImportFurniture(furnitureCache, p, tk);
                IsLoaddingIP = false;
            });

            CloseImportFurnitureCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility= Visibility.Collapsed;
            });

            OpenImportListFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) => {
                ImportListFurnitureWindow importListFurnitureWindow = new ImportListFurnitureWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;

                TotalImportPrice = 0;
                InitQuantity();
                OrderFurnitureList = new ObservableCollection<FurnitureDTO>();

                importListFurnitureWindow.ShowDialog();
            });

            FirstLoadImportListFurnitureWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {

            });

            CloseImportListWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
            });

            ChooseFurnitureToListCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null) return;

                SelectedFurniture.ImportQuantity++;
                SelectedFurniture.RemainQuantity--;
                if (!OrderFurnitureList.Contains(SelectedFurniture))
                    OrderFurnitureList.Add(SelectedFurniture);
                TotalImportPrice += SelectedFurniture.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            DecreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                FurnitureDTO pd = SelectedFurniture;
                if (pd.ImportQuantity == 1)
                {
                    if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm khỏi danh sách", "Thông báo", "Oke", "Hủy", CustomMessageBoxImage.Warning) == CustomMessageBoxResult.Cancel) return;
                    pd.ImportQuantity = 0;

                    OrderFurnitureList.Remove(pd);
                }
                else
                {
                    pd.ImportQuantity--;
                }

                TotalImportPrice -= pd.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            IncreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectedFurniture.ImportQuantity++;
                TotalImportPrice += SelectedFurniture.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            DeleteItemInBillStackCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null) return;
                FurnitureDTO pd = SelectedFurniture;
                if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm khỏi danh sách", "Thông báo", "Oke", "Hủy", CustomMessageBoxImage.Warning) == CustomMessageBoxResult.Cancel) return;

                OrderFurnitureList.Remove(pd);
                TotalImportPrice -= (pd.ImportPrice * pd.ImportQuantity);
                pd.ImportQuantity = 0;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            OpenReviewImportListFurnitureCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CreateDate = DateTime.Now;
                CreateDateString = DateTimeToString(CreateDate);
                ImportListFurnitureWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListFurnitureWindow>().FirstOrDefault();
                ipWD.Mask.Visibility = Visibility.Visible;
                CalculateTotalPrice();
                PreviewImportListFurniture previewImportListFurniture = new PreviewImportListFurniture();
                previewImportListFurniture.ShowDialog();
            });

            ClosePreviewImportListCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                TotalImportPrice = 0;
                TotalImportPriceStr = "";
                p.Close();
                ImportListFurnitureWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListFurnitureWindow>().FirstOrDefault();
                ipWD.Mask.Visibility = Visibility.Collapsed;
            });
            ImportListFurnitureCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoaddingIP = true;
                await ImportListFurniture(p, tk);
                IsLoaddingIP = false;

            });

            ImportPriceChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CalculateTotalPrice();
            });

            ImportQuantityChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CalculateTotalPrice();
            });

            CloseCM = new RelayCommand<System.Windows.Controls.Page>((p) => { return true; }, (p) =>
            {
                IsChanged = false;
            });
        }

        public void LoadFurnitureListView(Operation operation, FurnitureDTO furniture = null)
        {
            switch(operation)
            {
                case Operation.UPDATE:
                    FurnitureDTO furnitureDTO = FurnitureList.FirstOrDefault(item => item.FurnitureID == furniture.FurnitureID);
                    if (furnitureDTO == null)
                        return;
                    FurnitureList[FurnitureList.IndexOf(furnitureDTO)] = furniture;
                    AllFurniture[AllFurniture.IndexOf(furnitureDTO)] = furniture;
                    break;
                case Operation.CREATE:
                    FurnitureDTO furnitureCreate = new FurnitureDTO(furniture);
                    if (furnitureCreate == null)
                        return;
                    FurnitureList.Add(furnitureCreate);
                    AllFurniture.Add(furnitureCreate);
                    break;
                case Operation.DELETE:
                    FurnitureDTO furnitureDelete = FurnitureList.FirstOrDefault(item => item.FurnitureID == furniture.FurnitureID);
                    if (furnitureDelete == null)
                        return;
                    FurnitureList.Remove(furnitureDelete);
                    AllFurniture.Remove(furnitureDelete);
                    break;
                case Operation.UPDATE_PROD_QUANTITY:
                    FurnitureDTO furnitureUpdateQuantity = FurnitureList.FirstOrDefault(item => item.FurnitureID == furniture.FurnitureID);
                    FurnitureDTO temp = new FurnitureDTO(furniture);
                    FurnitureList[FurnitureList.IndexOf(furnitureUpdateQuantity)] = temp;
                    AllFurniture[AllFurniture.IndexOf(furnitureUpdateQuantity)] = temp;
                    break;
            }    
        }
    }
}
