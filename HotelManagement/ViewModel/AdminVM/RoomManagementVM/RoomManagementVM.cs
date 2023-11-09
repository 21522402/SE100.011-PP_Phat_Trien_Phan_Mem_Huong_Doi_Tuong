using HotelManagement.DTOs;
using HotelManagement.Model;
using HotelManagement.Model.Services;
using HotelManagement.Utils;
using HotelManagement.View.Admin.RoomManagement;
using HotelManagement.View.Admin.RoomTypeManagement;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.ViewModel.AdminVM.RoomTypeManagementVM;
using MaterialDesignThemes.Wpf;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagement.ViewModel.AdminVM.RoomManagementVM
{
    public partial class RoomManagementVM : BaseVM
    {
        public Frame mainFrame { get; set; }
        public Card ButtonView { get; set; }
        private string _RoomId;
        public string RoomId
        {
            get { return _RoomId; }
            set { _RoomId = value; OnPropertyChanged(); }
        }
        private string _roomTypeID;
        public string RoomTypeID
        {
            get { return _roomTypeID; }
            set { _roomTypeID = value; OnPropertyChanged(); }
        }

        private int _roomNumber;
        public int RoomNumber
        {
            get { return _roomNumber; }
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        private string _roomNote;
        public string RoomNote
        {
            get { return _roomNote; }
            set { _roomNote = value; OnPropertyChanged(); }
        }

        private string _RoomStatus;
        public string RoomStatus
        {
            get { return _RoomStatus; }
            set { _RoomStatus = value; OnPropertyChanged(); }
        }

        private string _cbRoomType;
        public string CbRoomType
        {
            get { return _cbRoomType; }
            set { _cbRoomType = value; OnPropertyChanged(); }
        }


        private RoomDTO _selectedRoomItem;
        public RoomDTO SelectedRoomItem
        {
            get { return _selectedRoomItem; }
            set { _selectedRoomItem = value; OnPropertyChanged(); }
        }

        private bool isloaddingRoom;
        public bool IsLoaddingRoom
        {
            get { return isloaddingRoom; }
            set { isloaddingRoom = value; OnPropertyChanged(); }
        }

        private bool isSavingRoom;
        public bool IsSavingRoom
        {
            get { return isSavingRoom; }
            set { isSavingRoom = value; OnPropertyChanged(); }
        }

        private ObservableCollection<RoomDTO> _roomList;
        public ObservableCollection<RoomDTO> RoomList
        {
            get => _roomList;
            set
            {
                _roomList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _listRoomType;
        public ObservableCollection<string> ListRoomType
        {
            get => _listRoomType;
            set
            {
                _listRoomType = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadRoomTypeCM { get; set; }
        public ICommand LoadRoomCM { get; set; }
        public ICommand FirstLoadRoomCM { get; set; }
        public ICommand LoadViewCM { get; set; }
        public ICommand StoreButtonNameCM { get; set; }
        public ICommand CloseRoomCM { get; set; }
        public ICommand LoadDeleteRoomCM { get; set; }
        public ICommand LoadNoteRoomCM { get; set; }
        public ICommand SaveRoomCM { get; set; }
        public ICommand UpdateRoomCM { get; set; }

        public RoomManagementVM()
        {


            LoadViewCM = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                mainFrame = p;
            });
            StoreButtonNameCM = new RelayCommand<Card>((p) => { return true; }, (p) =>
            {
                ButtonView = p;
                p.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                p.SetValue(ElevationAssist.ElevationProperty, Elevation.Dp3);
            });

            LoadRoomTypeCM = new RelayCommand<Card>((p) => { return true; }, (p) =>
            {
                ChangeView(p);
                mainFrame.Content = new View.Admin.RoomTypeManagement.RoomTypeManagementPage();
            });

            LoadRoomCM = new RelayCommand<Card>((p) => { return true; }, (p) =>
            {
                ChangeView(p);
                mainFrame.Content = new RoomManagementPage();
            });

            FirstLoadRoomCM = new RelayCommand<System.Windows.Controls.Page>((p) => { return true; }, async (p) =>
            {
                RoomList = new ObservableCollection<RoomDTO>();
                try
                {
                    IsLoaddingRoom = true;
                    RoomList = new ObservableCollection<RoomDTO>(await Task.Run(() => RoomService.Ins.GetAllRoom()));
                    IsLoaddingRoom = false;
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            });

            LoadAddRoomCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                RenewWindowData();
                AddNewRoom addRoom = new AddNewRoom();
                try
                {
                    IsLoaddingRoom = true;
                    ListRoomType = new ObservableCollection<string>((await RoomTypeService.Ins.GetAllRoomType()).Select(x => x.RoomTypeName));
                    IsLoaddingRoom = false;
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
                addRoom.ShowDialog();
            });
            LoadEditRoomCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                View.Admin.RoomManagement.EditRoom w1 = new View.Admin.RoomManagement.EditRoom();
                LoadEditRoom(w1);
                w1.ShowDialog();
            });
            LoadNoteRoomCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                NoteRoom w1 = new NoteRoom();
                RoomNote = SelectedRoomItem.Note;
                w1.ShowDialog();
            });
            LoadDeleteRoomCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {

                string message = "Bạn có chắc muốn xoá phim này không? Dữ liệu không thể phục hồi sau khi xoá!";
                CustomMessageBoxResult kq = CustomMessageBox.ShowOkCancel(message, "Cảnh báo", "Xác nhận", "Hủy", CustomMessageBoxImage.Warning);

                if (kq == CustomMessageBoxResult.OK)
                {
                    IsLoaddingRoom = true;

                    (bool successDeleteRoom, string messageFromDelRoom) = await RoomService.Ins.DeleteRoom(SelectedRoomItem.RoomId);

                    IsLoaddingRoom = false;

                    if (successDeleteRoom)
                    {
                        LoadRoomListView(Operation.DELETE);
                        SelectedRoomItem = null;
                        CustomMessageBox.ShowOk(messageFromDelRoom, "Thông báo", "OK", CustomMessageBoxImage.Success);
                    }
                    else
                    {
                        CustomMessageBox.ShowOk(messageFromDelRoom, "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                }
            });
            UpdateRoomCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSavingRoom) return false; return true; }, async (p) =>
            {
                IsSavingRoom = true;
                await UpdateRoomFunc(p);
                IsSavingRoom = false;
            });
            SaveRoomCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSavingRoom) return false; return true; }, async (p) =>
            {
                IsSavingRoom = true;

                await SaveRoomFunc(p);

                IsSavingRoom = false;
            });

            CloseRoomCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                SelectedRoomItem = null;
                p.Close();
            });


           
        }


        public async void ReloadListView()
        {
            RoomList = new ObservableCollection<RoomDTO>();
            try
            {
                IsLoaddingRoom = true;
                RoomList = new ObservableCollection<RoomDTO>(await RoomService.Ins.GetAllRoom());
                IsLoaddingRoom = false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }
        public void LoadRoomListView(Operation oper = Operation.READ, RoomDTO r = null)
        {

            switch (oper)
            {
                case Operation.CREATE:
                    RoomList.Add(r);
                    break;
                case Operation.UPDATE:
                    var roomFound = RoomList.FirstOrDefault(x => x.RoomId == r.RoomId);
                    RoomList[RoomList.IndexOf(roomFound)] = r;
                    break;
                case Operation.DELETE:
                    for (int i = 0; i < RoomList.Count; i++)
                    {
                        if (RoomList[i].RoomTypeId == SelectedRoomItem?.RoomTypeId)
                        {
                            RoomList.Remove(RoomList[i]);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void RenewWindowData()
        {

            RoomId = null;
            RoomNumber = 0;
            RoomNote = null;
            RoomTypeID = null;
            RoomStatus = "Phòng trống";
            CbRoomType = null;
        }
        public bool IsValidData()
        {
            if (!string.IsNullOrEmpty(RoomNote) &&
                !string.IsNullOrEmpty(RoomStatus) &&
                CbRoomType != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeView(Card p)
        {
            ButtonView.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
            ButtonView.SetValue(ElevationAssist.ElevationProperty, Elevation.Dp3);
            ButtonView = p;
            p.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
            p.SetValue(ElevationAssist.ElevationProperty, Elevation.Dp3);
        }

    }
}
