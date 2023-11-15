using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using HotelManagement.View.Admin.RoomTypeManagement;
using Microsoft.Office.Interop.Excel;
using HotelManagement.Model;
using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Utils;
using System.Data.Entity.Core.Metadata.Edm;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeManagementVM
{
    public partial class RoomTypeManagementVM:BaseVM
    {

        public void LoadEditRoomType()
        {
            RoomTypeID = SelectedItem.RoomTypeId;
            RoomTypeName = SelectedItem.RoomTypeName;
            RoomTypePrice = SelectedItem.RoomTypePrice;
            MaxNumberGuest = SelectedItem.MaxNumberGuest;
            NumberGuestForUnitPrice = SelectedItem.NumberGuestForUnitPrice;
        }
        public async Task UpdateRoomTypeFunc(System.Windows.Window p, System.Windows.Window windowEdit)
        {

            if (RoomTypeID != null && IsValidData2())
            {
                RoomTypeDTO roomType = new RoomTypeDTO
                {
                    RoomTypeId = RoomTypeID,
                    RoomTypeName = RoomTypeName,
                    RoomTypePrice = RoomTypePrice,
                    MaxNumberGuest = MaxNumberGuest,
                    NumberGuestForUnitPrice = NumberGuestForUnitPrice,  
                    ListSurcharges = ListSurchargeRate,
                };
                if (roomType.ListSurcharges != null)
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

        //        (bool successUpdateRoomType, string messageFromUpdateRoomType) = await RoomTypeService.Ins.UpdateRoomType(roomType);

                if (successUpdateRoomType)
                {
                    isSaving = false;
                    CustomMessageBox.ShowOk(messageFromUpdateRoomType, "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    LoadRoomTypeListView(Operation.UPDATE, roomType);
                    p.Close();
                    windowEdit.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(messageFromUpdateRoomType, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk("Vui lòng nhập đủ thông tin!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
            }
        }
    }
}
