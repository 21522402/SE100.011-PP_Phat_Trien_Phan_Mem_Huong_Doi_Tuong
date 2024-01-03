using HotelManagement.DTOs;
using HotelManagement.ViewModel.AdminVM;
using HotelManagement.ViewModel.StaffVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using HotelManagement.Utils;
using System.Diagnostics;

namespace HotelManagement.Model.Services
{
    internal class FurnitureRoomTypeService
    {

        private FurnitureRoomTypeService() { }
        private static FurnitureRoomTypeService _ins;
        public static FurnitureRoomTypeService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new FurnitureRoomTypeService();
                return _ins;
            }
            private set { _ins = value; }
        }

        public async Task<(bool, string, List<FurnitureRoomTypeDTO>)> GetAllFurnituresRoomType()
        {
            try
            {
                List<FurnitureRoomTypeDTO> FurnitureRoomTypeDTOs = new List<FurnitureRoomTypeDTO>();

                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    FurnitureRoomTypeDTOs = await (
                        from roomType in db.RoomTypes

                        select new FurnitureRoomTypeDTO
                        {
                            RoomTypeId = roomType.RoomTypeId,
                            RoomTypeName = roomType.RoomTypeName,
                            Price = (double)roomType.Price,
                            MaxNumberGuest = (int)roomType.MaxNumberGuest,
                            NumberGuestForUnitPrice = (int)roomType.NumberGuestForUnitPrice
                        }
                    ).ToListAsync();

                    return (true, "Thành công", FurnitureRoomTypeDTOs);
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống", null);
            }

        }
        public async Task<(bool, string, List<FurnitureDTO>)> GetAllFurnituresIn(FurnitureRoomTypeDTO roomTypeSelected)
        {
            try
            {
                List<FurnitureDTO> FurnitureRoomTypeDTOs = new List<FurnitureDTO>();

                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    FurnitureRoomTypeDTOs = await (
                        from p in db.RoomTypeFurnitures
                        where p.RoomTypeId == roomTypeSelected.RoomTypeId
                        select new FurnitureDTO
                        {
                            FurnitureID = p.FurnitureId,
                            FurnitureName = p.Furniture.FurnitureName,
                            FurnitureAvatarData = p.Furniture.FurnitureAvatar,
                            FurnitureType = p.Furniture.FurnitureType,
                            InUseQuantity = (int)p.Quantity,
                            DeleteInRoomQuantity = (int)p.Quantity,
                        }
                    ).ToListAsync();

                    FurnitureRoomTypeDTOs.ForEach(item => item.SetAvatar());

                    return (true, "Thành công", FurnitureRoomTypeDTOs);
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }
        public async Task<(bool, string, List<FurnitureDTO>)> GetAllFurniture()
        {
            try
            {
                List<FurnitureDTO> listFurniture = new List<FurnitureDTO>();
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    listFurniture = await (
                        from p in db.Furnitures
                        let Sum =
                        (
                            from rfd in db.RoomTypeFurnitures
                            where p.FurnitureId == rfd.FurnitureId
                            select rfd.Quantity
                        ).Sum()
                        select new FurnitureDTO
                        {
                            FurnitureID = p.FurnitureId,
                            FurnitureAvatarData = p.FurnitureAvatar,
                            FurnitureName = p.FurnitureName,
                            FurnitureType = p.FurnitureType,
                            Quantity = (int)p.QuantityOfStorage,
                            TotalUseQuantity = (int)(Sum == null ? 0 : Sum),
                        }
                    ).ToListAsync();
                }
                listFurniture.ForEach(item => { item.SetRemaining(); item.SetAvatar(); });

                return (true, "", listFurniture);
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }

        public async Task<(bool, string, List<FurnitureDTO>)> ImportListFurnitureToRoom(ObservableCollection<FurnitureDTO> orderList, FurnitureRoomTypeDTO roomTypeFurnitureSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    int Length = orderList.Count;
                    List<FurnitureDTO> listFurniture = new List<FurnitureDTO>();
                    int maxIdRoomTypeFurnitureId = getMaxRoomTypeFurnitureId(db.RoomTypeFurnitures.ToList());
                    for (int i = 0; i < Length; i++)
                    {
                        FurnitureDTO temp = new FurnitureDTO(orderList[i]);
                        RoomTypeFurniture furnitureInRoomType = await db.RoomTypeFurnitures.FirstOrDefaultAsync(item => item.RoomTypeId == roomTypeFurnitureSelected.RoomTypeId && temp.FurnitureID == item.FurnitureId);
                        if (furnitureInRoomType != null)
                            furnitureInRoomType.Quantity += temp.QuantityImportRoom;
                        else
                        {
                            string nextRoomTypeFurnitureId = getID(++maxIdRoomTypeFurnitureId);
                            furnitureInRoomType = new RoomTypeFurniture
                            {
                                RoomTypeFurnitureId = nextRoomTypeFurnitureId,
                                RoomTypeId = roomTypeFurnitureSelected.RoomTypeId,
                                FurnitureId = temp.FurnitureID,
                                Quantity = temp.QuantityImportRoom,
                            };
                            db.RoomTypeFurnitures.Add(furnitureInRoomType);
                        }
                        listFurniture.Add(temp);
                        orderList[i].QuantityImportRoom = 0;
                    }
                    await db.SaveChangesAsync();
                    return (true, "Nhập tiện nghi thành công", listFurniture);
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }

        public async Task<(bool, string)> DeleteFurnitureRoom(string roomTypeFurnitureSelectedID, FurnitureDTO selectedFurniture)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    RoomTypeFurniture roomTypeFurnitures = await db.RoomTypeFurnitures.FirstOrDefaultAsync(item => item.RoomTypeId == roomTypeFurnitureSelectedID && item.FurnitureId == selectedFurniture.FurnitureID);
                    if (roomTypeFurnitures == null)
                        return (false, "Không tìm thấy thông tin trong cơ sở dữ liệu");

                    db.RoomTypeFurnitures.Remove(roomTypeFurnitures);
                    await db.SaveChangesAsync();
                    return (true, "Xóa tiện nghi thành công");
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống");
            }
        }
        public async Task<(bool, string)> DeleteListFurnitureRoom(string roomFurnitureSelectedID, ObservableCollection<FurnitureDTO> DeleteList)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    int lengh = DeleteList.Count();
                    for (int i = 0; i < lengh; i++)
                    {
                        FurnitureDTO temp = DeleteList[i];
                        RoomTypeFurniture roomFurnituresDetail = await db.RoomTypeFurnitures.FirstOrDefaultAsync(item => item.RoomTypeId == roomFurnitureSelectedID && item.FurnitureId == temp.FurnitureID);
                        if (roomFurnituresDetail == null)
                            return (false, "Không tìm thấy thông tin trong cơ sở dữ liệu");

                        if (temp.DeleteInRoomQuantity == temp.InUseQuantity)
                            db.RoomTypeFurnitures.Remove(roomFurnituresDetail);
                        else
                            roomFurnituresDetail.Quantity -= temp.DeleteInRoomQuantity;
                    }

                    await db.SaveChangesAsync();
                    return (true, "Xóa tiện nghi thành công");
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống");
            }
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
                        if (item.FurnitureAvatarData != null)
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
        public async Task<List<RoomFurnituresDetailDTO>> GetRoomTypeFurnituresDetail(string roomTypeId)
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
                                                       RoomId = roomTypeId,
                                                       FurnitureId = a.FurnitureId,
                                                       FurnitureName = b.FurnitureName,
                                                       FurnitureType = b.FurnitureType,
                                                       Quantity = a.Quantity,
                                                       FurnitureAvatarData = b.FurnitureAvatar
                                                   }
                                                   ).ToListAsync();
                    foreach (var item in listRoomFurniture)
                    {
                        item.SetAvatar();
                    }
                    return listRoomFurniture;
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int getMaxRoomTypeFurnitureId(List<RoomTypeFurniture> listRoomTypeFurniture)
        {
            int length = listRoomTypeFurniture.Count();
            if (length == 0) 
                return 0;
            return Int32.Parse(listRoomTypeFurniture[length - 1].RoomTypeFurnitureId);
        }
        public string getID(int id)
        {
            if (id < 10)
                return "000" + id;
            if (id < 100)
                return "00" + id;
            if (id < 1000)
                return "0" + id;

            return id.ToString();
        }
    }
}
