using HotelManagement.Components.Search;
using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.View.Admin;
using HotelManagement.View.Admin.RoomTypeFurnitureManagement;
using HotelManagement.View.CustomMessageBoxWindow;
using IronXL.Formatting;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeFurnitureManagementVM
{
    public partial class RoomTypeFurnitureManagementVM : BaseVM
    {
        private bool isLoading { get; set; }
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FurnitureRoomTypeDTO> furnituresRoomTypeList;
        public ObservableCollection<FurnitureRoomTypeDTO> FurnituresRoomTypeList
        {
            get { return furnituresRoomTypeList; }
            set { furnituresRoomTypeList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FurnitureRoomTypeDTO> allFurnituresRoomType;
        public ObservableCollection<FurnitureRoomTypeDTO> AllFurnituresRoomType
        {
            get { return allFurnituresRoomType; }
            set { allFurnituresRoomType = value; OnPropertyChanged(); }
        }

        private FurnitureRoomTypeDTO selectedFurnituresRoomType { get; set; }
        public FurnitureRoomTypeDTO SelectedFurnituresRoomType
        {
            get { return selectedFurnituresRoomType; }
            set { selectedFurnituresRoomType = value; OnPropertyChanged(); }
        }

        private FurnitureRoomTypeDTO furnituresRoomTypeCache { get; set; }
        public FurnitureRoomTypeDTO FurnituresRoomTypeCache
        {
            get { return furnituresRoomTypeCache; }
            set { furnituresRoomTypeCache = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ComboBoxItem> listRoomType { get; set; }
        public ObservableCollection<ComboBoxItem> ListRoomType
        {
            get { return listRoomType; }
            set { listRoomType = value; OnPropertyChanged(); }
        }

        private ComboBoxItem selectedFilterTypeRoom { get; set; }
        public ComboBoxItem SelectedFilterTypeRoom
        {
            get { return selectedFilterTypeRoom; }
            set { selectedFilterTypeRoom = value; OnPropertyChanged(); }
        }
        private ComboBoxItem selectedFilterStatusRoom { get; set; }
        public ComboBoxItem SelectedFilterStatusRoom
        {
            get { return selectedFilterStatusRoom; }
            set { selectedFilterStatusRoom = value; OnPropertyChanged(); }
        }
        public ICommand FirstLoadCM { get; set; }
        public ICommand OpenFurnituresInRoomWindow { get; set; }
        public ICommand CloseFurnitureRoomInfoCM { get; set; }
        public ICommand IncreaseQuantityOrderItem { get; set; }
        public ICommand DecreaseQuantityOrderItem { get; set; }
        public ICommand DeleteItemInBillStackCM { get; set; }
        public ICommand DeleteItemInRoomFurnitureInfoCM { get; set; }
        public ICommand OpenDeleteFurnitureInRoomTypeCM { get; set; }
        public ICommand OpenImportFurnitureRoomTypeCM { get; set; }

        public RoomTypeFurnitureManagementVM()
        {
            AdminWindow tk = System.Windows.Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {

                IsLoading = true;
                (bool isSuccess, string messageReturn, List<FurnitureRoomTypeDTO> listFurnituresRoomReturn) = await Task.Run(() => FurnitureRoomTypeService.Ins.GetAllFurnituresRoomType());

                IsLoading = false;
                if (isSuccess)
                {
                    FurnituresRoomTypeList = new ObservableCollection<FurnitureRoomTypeDTO>(listFurnituresRoomReturn);
                    AllFurnituresRoomType = new ObservableCollection<FurnitureRoomTypeDTO>(listFurnituresRoomReturn);
                }
                else
                {
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            });
            OpenFurnituresInRoomWindow = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurnituresRoomType == null)
                    return;

                FurnituresRoomTypeCache = new FurnitureRoomTypeDTO(SelectedFurnituresRoomType);

                IsLoading = true;

                RoomTypeFurnitureInfoWindow roomTypeFurnitureInfoWindow = new RoomTypeFurnitureInfoWindow();

                tk.MaskOverSideBar.Visibility = Visibility.Visible;

                roomTypeFurnitureInfoWindow.ShowDialog();

                IsLoading = false;
            });

            OpenDeleteFurnitureInRoomTypeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurnituresRoomType == null)
                    return;

                FurnituresRoomTypeCache = new FurnitureRoomTypeDTO(SelectedFurnituresRoomType);

                IsLoading = true;

                RoomTypeFurnitureDeleteWindow roomFurnitureInfoWD = new RoomTypeFurnitureDeleteWindow();

                tk.MaskOverSideBar.Visibility = Visibility.Visible;

                roomFurnitureInfoWD.ShowDialog();

                IsLoading = false;
            });

            FirstLoadInfoWindowCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                if (FurnituresRoomTypeCache == null)
                    return;
                ListFurnitureNeedDelete = new ObservableCollection<FurnitureDTO>();

                IsLoading = true;

                await LoadFurniture();

                IsLoading = false;

            });

            CloseFurnitureRoomInfoCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                FurnituresRoomTypeCache = null;
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
            });

            CloseFurnitureRoomDeleteCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                FurnituresRoomTypeCache = null;
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
            });

            OpenImportFurnitureRoomTypeCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                if (SelectedFurnituresRoomType == null)
                    return;

                FurnituresRoomTypeCache = new FurnitureRoomTypeDTO(SelectedFurnituresRoomType);

                IsLoading = true;

                OrderFurnitureList = new ObservableCollection<FurnitureDTO>();

                RoomTypeFurnitureImportWindow roomFurnitureImportWindow = new RoomTypeFurnitureImportWindow();

                roomFurnitureImportWindow.Show();

                IsLoading = false;
            });

            FirstLoadImportWindowCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                IsLoading = true;

                await LoadAllFurniture();

                IsLoading = false;
            });

            ChoosedFurnitureToListCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;

                FurnitureCache = SelectedFurniture;

                IsLoading = true;

                LoadFurnitureToList(FurnitureCache);

                IsLoading = false;
            });

            DecreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = SelectedFurniture;
                if (furnitureCache.QuantityImportRoom <= 1)
                    OrderFurnitureList.Remove(furnitureCache);
                furnitureCache.DecreaseImport(1);
            });

            IncreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = SelectedFurniture;
                if (furnitureCache.RemainingQuantity == 0)
                {
                    CustomMessageBox.ShowOk("Số lượng tiện nghi trong kho đã hết!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }
                furnitureCache.IncreaseImport(1);
            });

            DeleteItemInBillStackCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                furnitureCache = SelectedFurniture;
                if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa tiện nghi này không?", "Cảnh báo", "Có", "Không", CustomMessageBoxImage.Warning)
                    == CustomMessageBoxResult.OK)
                {
                    OrderFurnitureList.Remove(furnitureCache);
                    furnitureCache.DecreaseImport(furnitureCache.QuantityImportRoom);
                }
            });

            ImportListFurnitureToRoomCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoading = true;

                await ImportListFurnitureToRoom(p);

                IsLoading = false;
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

            DeleteItemInRoomFurnitureInfoCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                if (SelectedFurniture == null)
                    return;

                FurnitureCache = SelectedFurniture;

                if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa " + FurnitureCache.FurnitureName + " ra khỏi loại phòng " + FurnituresRoomTypeCache.RoomTypeName + " không?", "Cảnh báo", "Có", "Không", CustomMessageBoxImage.Warning)
                    == CustomMessageBoxResult.OK)
                {
                    (bool isSuccess, string messageReturn) = await Task.Run(() => FurnitureRoomTypeService.Ins.DeleteFurnitureRoom(FurnituresRoomTypeCache.RoomTypeId, FurnitureCache));
                    if (isSuccess)
                    {
                        CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", CustomMessageBoxImage.Success);
                        FurnituresRoomTypeCache.ListFurnitureRoomType.Remove(furnitureCache);
                        FurnituresRoomTypeCache.SetQuantityAndStringTypeFurniture();

                    }
                    else
                    {
                        CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                }
            });

            ChooseItemToListNeedDelete = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                FurnitureCache = SelectedFurniture;
                FurnitureCache.IsSelectedDelete = true;
                ListFurnitureNeedDelete.Add(FurnitureCache);
            });

            RemoveItemToListNeedDelete = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedFurniture == null)
                    return;
                FurnitureCache = SelectedFurniture;
                FurnitureCache.IsSelectedDelete = false;
                ListFurnitureNeedDelete.Remove(FurnitureCache);
            });
            bool isChooseAll = false;
            ChooseAllFurnitureToDeleteCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (isChooseAll)
                {
                    ListFurnitureNeedDelete = new ObservableCollection<FurnitureDTO>(FurnituresRoomTypeCache.ListFurnitureRoomType);
                    foreach (var item in ListFurnitureNeedDelete)
                        item.IsSelectedDelete = true;
                    isChooseAll = false;
                }
                else
                {
                    ListFurnitureNeedDelete.Clear();
                    foreach (var item in ListFurnitureNeedDelete)
                        item.IsSelectedDelete = false;
                    isChooseAll = true;
                }
            });

            DeleteListFurnitureCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoading = true;

                Search textSearchFinal = (Search)p.FindName("SearchBox");
                string text = textSearchFinal.Text;

                FilterListFurnitureInRoomByKey(text);

                await DeleteListFurniture(p, tk);

                IsLoading = false;
            });
            CloseDeleteControlCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ListFurnitureNeedDelete.Clear();
            });
            SelectionFilterInfoChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedItemFilter != null)
                {
                    if (SelectedItemFilter.Tag.ToString() == "All")
                        FurnituresRoomTypeCache.ListFurnitureRoomType = new ObservableCollection<FurnitureDTO>(AllFurniture);
                    else
                        FurnituresRoomTypeCache.ListFurnitureRoomType = new ObservableCollection<FurnitureDTO>(FurnitureService.Ins.GetAllFurnitureByType(SelectedItemFilter.Content.ToString(), AllFurniture));
                }
            });
        }
        public void SetRoomTypeToCombobox(List<RoomTypeDTO> roomType)
        {
            ComboBoxItem cbiall = new ComboBoxItem();
            cbiall.Tag = "Tất cả";
            cbiall.Content = "Tất cả loại phòng";
            ListRoomType.Add(cbiall);
            for (int i = 0; i < roomType.Count(); i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Tag = cbi.Content = roomType[i].RoomTypeName;
                ListRoomType.Add(cbi);
            }
        }

        public void FilterListFurnitureInRoomByKey(string key)
        {
            ListFurnitureNeedDelete = new ObservableCollection<FurnitureDTO>(ListFurnitureNeedDelete.Where(item => item.FurnitureName.ToLower().Contains(key.ToLower())));
        }
    }
}