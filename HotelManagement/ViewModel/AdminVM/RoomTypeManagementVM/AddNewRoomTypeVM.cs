using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.View.Admin.RoomTypeManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeManagementVM
{
    public partial class RoomTypeManagementVM : BaseVM
    {
        public async Task SaveRoomTypeFunc(System.Windows.Window p, System.Windows.Window windowAdd)
        {
            RoomTypeDTO roomtype = new RoomTypeDTO
            {
                RoomTypeName = RoomTypeName.Trim(),
                RoomTypePrice = RoomTypePrice,
                MaxNumberGuest = MaxNumberGuest,
                NumberGuestForUnitPrice = NumberGuestForUnitPrice,
                ListSurcharges = ListSurchargeRate,
            };

            if (roomtype.ListSurcharges != null && roomtype.MaxNumberGuest > roomtype.NumberGuestForUnitPrice)
            {
                for (int i = 0; i < ListSurchargeRate.Count; i++)
                {
                    if (ListSurchargeRate[i].Rate <= 0 || ListSurchargeRate[i].Rate > 1)
                    {
                        CustomMessageBox.ShowOk("Tỷ lệ phụ thu phải lớn hơn 0 và nhỏ hơn hoặc bằng 1 !!!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                        return;
                    }
                }
            }

            (bool successAddRoomType, string messageFromAddRoomType, RoomTypeDTO newRoomType) = await RoomTypeService.Ins.AddRoomType(roomtype);

            if (successAddRoomType)
            {
                isSaving = false;
                CustomMessageBox.ShowOk(messageFromAddRoomType, "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                ReloadListView();
                p.Close();
                windowAdd.Close();
            }
            else
            {
                CustomMessageBox.ShowOk(messageFromAddRoomType, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                return;
            }

        }
    }
}