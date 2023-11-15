using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeManagementVM
{
    public partial class RoomTypeManagementVM : BaseVM
    {
        public async Task SaveRoomTypeFunc(System.Windows.Window p)
        {
            if (IsValidData())
            {
                RoomTypeDTO roomtype = new RoomTypeDTO
                {   // check ở đây
                    RoomTypeName = RoomTypeName.Trim(),
                    RoomTypePrice = RoomTypePrice,
                    MaxNumberGuest = MaxNumberGuest,
                    NumberGuestForUnitPrice = NumberGuestForUnitPrice,
                };

                (bool successAddRoomType, string messageFromAddRoomType, RoomTypeDTO newRoomType) = await RoomTypeService.Ins.AddRoomType(roomtype);

                if (successAddRoomType)
                {
                    isSaving = false;
                    CustomMessageBox.ShowOk(messageFromAddRoomType, "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    ReloadListView();
                    p.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(messageFromAddRoomType, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk("Vui lòng nhập đủ thông tin!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
            }
        }
    }
}
