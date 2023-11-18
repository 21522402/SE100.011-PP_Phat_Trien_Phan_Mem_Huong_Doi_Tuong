using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeFurnitureManagementVM
{
    public partial class RoomTypeFurnitureManagementVM
    {
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


        private ObservableCollection<FurnitureDTO> orderFurnitureList;
        public ObservableCollection<FurnitureDTO> OrderFurnitureList
        {
            get { return orderFurnitureList; }
            set { orderFurnitureList = value; OnPropertyChanged(); }
        }

        private FurnitureDTO selectedFurniture;
        public FurnitureDTO SelectedFurniture
        {
            get { return selectedFurniture; }
            set { selectedFurniture = value; OnPropertyChanged(); }
        }
        public List<string> AllFurnitureType { get; set; }
        private ObservableCollection<string> currentListFurnitureType { get; set; }
        public ObservableCollection<string> CurrentListFurnitureType
        {
            get { return currentListFurnitureType; }
            set { currentListFurnitureType = value; OnPropertyChanged(); }
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
        public ICommand FirstLoadImportWindowCM { get; set; }
        public ICommand SelectionFilterChangeCM { get; set; }
        public ICommand ChoosedFurnitureToListCM { get; set; }
        public ICommand ImportListFurnitureToRoomCM { get; set; }

        public async Task LoadAllFurniture()
        {
            IsLoading = true;
            (bool isSuccess, string messageReturn, List<FurnitureDTO> listFurniture) = await Task.Run(() => FurnitureRoomTypeService.Ins.GetAllFurniture());
            IsLoading = false;

            if (isSuccess)
            {
                AllFurniture = new ObservableCollection<FurnitureDTO>(listFurniture);
                FurnitureList = new ObservableCollection<FurnitureDTO>(AllFurniture);
            }
            else
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
        }
        public void LoadFurnitureToList(FurnitureDTO furnitureSelected)
        {
            if (furnitureSelected.RemainingQuantity == 0)
            {
                CustomMessageBox.ShowOk("Số lượng tiện nghi trong kho đã hết!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            FurnitureDTO furnitureInOrder = OrderFurnitureList.FirstOrDefault(item => item.FurnitureID == furnitureSelected.FurnitureID);
            if (furnitureInOrder == null)
            {
                OrderFurnitureList.Add(furnitureSelected);
                furnitureSelected.IncreaseImport(1);
            }
            else
                furnitureInOrder.IncreaseImport(1);
        }

        public async Task ImportListFurnitureToRoom(Window p)
        {
            if (OrderFurnitureList.Count() == 0)
            {
                CustomMessageBox.ShowOk("Vui lòng chọn tiện nghi vào danh sách nhập!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                return;
            }
            (bool isSuccess, string messageReturn, List<FurnitureDTO> listFurniture) = await Task.Run(() => FurnitureRoomTypeService.Ins.ImportListFurnitureToRoom(OrderFurnitureList, FurnituresRoomTypeCache));

            if (isSuccess)
            {
                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                OrderFurnitureList.Clear();
                p.Close();
            }
            else
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
        }
        public void LoadListViewFurnitureInRoom(List<FurnitureDTO> listFurnitureNeedUpdate)
        {
            int length = listFurnitureNeedUpdate.Count();
            for (int i = 0; i < length; i++)
            {
                FurnitureDTO temp = new FurnitureDTO(listFurnitureNeedUpdate[i]);
                FurnitureDTO furnitureUpdateQuantity = FurnituresRoomTypeCache.ListFurnitureRoomType.FirstOrDefault(item => item.FurnitureID == listFurnitureNeedUpdate[i].FurnitureID);
                if (furnitureUpdateQuantity != null)
                {
                    temp.SetInUseQuantity(temp.QuantityImportRoom + furnitureUpdateQuantity.InUseQuantity);
                    FurnituresRoomTypeCache.ListFurnitureRoomType[FurnituresRoomTypeCache.ListFurnitureRoomType.IndexOf(furnitureUpdateQuantity)] = temp;
                }
                else
                {
                    temp.SetInUseQuantity(temp.QuantityImportRoom);
                    FurnituresRoomTypeCache.ListFurnitureRoomType.Add(temp);
                }

            }
        }
    }
}