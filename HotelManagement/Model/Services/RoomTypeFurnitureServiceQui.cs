using HotelManagement.DTOs;
using HotelManagement.ViewModel.AdminVM;
using HotelManagement.ViewModel.StaffVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class RoomTypeFurnitureServiceQui
    {
        private RoomTypeFurnitureServiceQui() { }
        private static RoomTypeFurnitureServiceQui _ins;
        public static RoomTypeFurnitureServiceQui Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new RoomTypeFurnitureServiceQui();
                return _ins;
            }
            private set { _ins = value; }
        }






        public async Task<List<RoomFurnituresDetailDTO>> GetRoomFurnituresDetail(string roomTypeId)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    var listRoomFurniture = await (from a in db.RoomTypeFurnitures
                                                   join b in db.Furnitures
                                                   on a.FurnitureId equals b.FurnitureId
                                                   where a.RoomTypeId == roomTypeId
                                                   select new RoomFurnituresDetailDTO
                                                   {
                                                       FurnitureId = a.FurnitureId,
                                                       FurnitureName = b.FurnitureName,
                                                       Quantity = a.Quantity,
                                                       FurnitureAvatarData = b.FurnitureAvatar
                                                   }
                                                   ).ToListAsync();
                    int i = 0;
                    foreach (var item in listRoomFurniture)
                    {
                        item.STT = ++i;
                        if (item.FurnitureAvatarData!= null)
                        {
                            item.SetAvatar();
                        }
                    }
                    return listRoomFurniture;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
   
}
