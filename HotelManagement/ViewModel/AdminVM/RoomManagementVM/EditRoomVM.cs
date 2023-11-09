using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Model;
using HotelManagement.View.Admin.RoomManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using HotelManagement.Utils;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.AdminVM.RoomManagementVM
{
    public partial class RoomManagementVM : BaseVM
    {

        public ICommand LoadEditRoomCM { get; set; }

        public async void LoadEditRoom(EditRoom w1)
        {
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
            RoomId = SelectedRoomItem.RoomId;
            RoomNumber = (int)SelectedRoomItem.RoomNumber;
            RoomNote = SelectedRoomItem.Note;
            RoomStatus = SelectedRoomItem.RoomStatus;
            CbRoomType = SelectedRoomItem.RoomTypeName;
        }

        public async Task UpdateRoomFunc(System.Windows.Window p)
        {
            string rtn = CbRoomType;
            string rti = await RoomTypeService.Ins.GetRoomTypeID(rtn);

            if (RoomId != null && IsValidData())
            {
                RoomDTO room = new RoomDTO
                {
                    RoomId = RoomId,
                    Note = RoomNote,
                    RoomNumber = RoomNumber,
                    RoomTypeId = rti,
                    RoomTypeName = CbRoomType,
                    RoomStatus = RoomStatus,
                };

                (bool successUpdateRoom, string messageFromUpdateRoom) = await RoomService.Ins.UpdateRoom(room);

                if (successUpdateRoom)
                {
                    isSavingRoom = false;
                    CustomMessageBox.ShowOk(messageFromUpdateRoom, "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    LoadRoomListView(Operation.UPDATE, room);
                    p.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(messageFromUpdateRoom, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk("Vui lòng nhập đủ thông tin!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
            }
        }
    }
}
