using Google.Apis.Util;
using HotelManagement.DTOs;
using HotelManagement.Utils;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagement.Model.Services
{
    public class RoomService
    {
        public RoomService() { }
        private static RoomService _ins;
        public static RoomService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RoomService();
                }
                return _ins;
            }
            private set { _ins = value; }
        }
        public async Task<List<RoomDTO>> GetAllRoom()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<RoomDTO> RoomDTOs = await (
                        from r in db.Rooms
                        join temp in db.RoomTypes
                        on r.RoomTypeId equals temp.RoomTypeId into gj
                        from d in gj.DefaultIfEmpty()
                        select new RoomDTO
                        {
                            // DTO = db
                            RoomId = r.RoomId,
                            RoomNumber = (int)r.RoomNumber,
                            RoomTypeName = d.RoomTypeName,
                            RoomTypeId = d.RoomTypeId,
                            Note = r.Note,
                            RoomStatus = r.RoomStatus,
                        }
                    ).ToListAsync();
                    RoomDTOs.Reverse();
                    return RoomDTOs;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string CreateNextRoomCode(string maxCode)
        {
            if (maxCode == "")
            {
                return "PH001";
            }
            int index = (int.Parse(maxCode.Substring(2)) + 1);
            string CodeID = index.ToString();
            while (CodeID.Length < 3) CodeID = "0" + CodeID;

            return "PH" + CodeID;
        }
        public async Task<(bool, string, RoomDTO)> AddRoom(RoomDTO newRoom)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room r = context.Rooms.Where((Room Room) => Room.RoomNumber == newRoom.RoomNumber).FirstOrDefault();

                    if (r != null)
                    {
                                 
                            return (false, $"Phòng {r.RoomNumber} đã tồn tại!", null);

                    }
                    else
                    {
                        var listid = await context.Rooms.Select(s => s.RoomId).ToListAsync();
                        string maxId = "";

                        if (listid.Count > 0)
                            maxId = listid[listid.Count - 1];
                        string id = CreateNextRoomCode(maxId);
                        Room room = new Room
                        {
                            RoomId = id,
                            RoomNumber = newRoom.RoomNumber,
                            RoomTypeId = newRoom.RoomTypeId,
                            Note = newRoom.Note,
                            RoomStatus = newRoom.RoomStatus,
   
                        };
                        context.Rooms.Add(room);
                        await context.SaveChangesAsync();
                        newRoom.RoomId = room.RoomId;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return (false, "DbEntityValidationException", null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, $"Error Server {e}", null);
            }
            return (true, "Thêm phòng thành công", newRoom);
        }

        public async Task<(bool, string)> DeleteRoom(string Id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room room = await (from p in context.Rooms
                                       where p.RoomId == Id 
                                       select p).FirstOrDefaultAsync();
                    if (room == null)
                    {
                        return (false, "Loại phòng này không tồn tại!");
                    }
                    context.Rooms.Remove(room);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return (false, "Phòng này đã áp dụng trước đây và đã có khách đặt. Không thể xóa!");
            }
            return (true, "Xóa phòng thành công");
        }

        public async Task<(bool, string)> UpdateRoom(RoomDTO updatedRoom)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room room = context.Rooms.Find(updatedRoom.RoomId);

                    if (room is null)
                    {
                        return (false, "Phòng này không tồn tại!");
                    }

                    // ở dưới phải đợi thêm r fix, code coment ở dưới tức là khi
                    // phòng đã có người đặt hoặc đang thuê thì không thể chỉnh sửa

                    //bool IsExistRoomNumber = context.Rooms.Any((Room r) => r.RoomId != room.RoomTypeId && r.RoomNumber == updatedRoom.RoomNumber);
                    //if (IsExistRoomNumber)
                    //{
                    //    return (false, "Phòng đang được sử dụng không thể chỉnh sửa!");
                    //}

                    room.RoomId = updatedRoom.RoomId;
                    room.RoomNumber = updatedRoom.RoomNumber;
                    room.RoomStatus = updatedRoom.RoomStatus;
                    room.Note = updatedRoom.Note;
                    room.RoomTypeId = updatedRoom.RoomTypeId;

                    await context.SaveChangesAsync();
                    return (true, "Cập nhật thành công");
                }
            }
            catch (DbEntityValidationException)
            {
                return (false, "DbEntityValidationException");
            }
            catch (DbUpdateException e)
            {
                return (false, $"DbUpdateException: {e.Message}");
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống");
            }

        }

        public async Task<List<RoomDTO>> GetRooms()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var roomList = await (from r in context.Rooms
                                          join t in context.RoomTypes
                                          on r.RoomTypeId equals t.RoomTypeId
                                          select new RoomDTO
                                          {
                                              RoomId = r.RoomId,
                                              RoomNumber = r.RoomNumber,
                                              RoomTypeId = r.RoomTypeId,
                                              Note = r.Note,
                                              RoomStatus = r.RoomStatus,
                                              Price = (double)t.Price,
                                          }
                                          ).ToListAsync();
                    return roomList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
     
     
        public async Task<List<RoomTypeDTO>> GetRoomTypes()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var roomTypeList = await (from r in context.RoomTypes
                                              select new RoomTypeDTO
                                              {
                                                  RoomTypeId = r.RoomTypeId,
                                                  RoomTypeName = r.RoomTypeName.Trim(),
                                                  RoomTypePrice = (double)r.Price,
                                              }
                                          ).ToListAsync();
                    return roomTypeList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool, string)> ChangeRoomStatus(string roomId, string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room room = await context.Rooms.FindAsync(roomId);
                    RentalContract rentalContract = await context.RentalContracts.FindAsync(rentalContractId);
                    string mess = "";
                   if (room.RoomStatus == ROOM_STATUS.RENTING)
                    {
                        room.RoomStatus = ROOM_STATUS.READY;
                        rentalContract.Validated = false;
                        mess = "Thanh toán thành công!";

                    }
                    await context.SaveChangesAsync();
                    return (true, mess);
                    
                }
            }
            catch(Exception ex)
            {
                return (false, "Lỗi hệ thống!");
            }
        }
        public async Task<(bool, string)> UpdateRoomInfo(string roomId, string roomCleaningStatus)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room room = await context.Rooms.FindAsync(roomId);
                    await context.SaveChangesAsync();
                    return (true, "Cập nhật thành công!");

                }
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống!");
            }
        }


        public async Task<List<ListRoomTypeDTO>> GetListListRoomType()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    // Lấy danh sách các phiếu thuê có phòng đang thuê gần nhất
                 

                    var list = await context.RoomTypes.Select(x => new ListRoomTypeDTO
                    {
                        RoomTypeName = x.RoomTypeName,
                        RoomTypeId = x.RoomTypeId,
                        Price = (double)x.Price,
                        Rooms = x.Rooms.Where(t => t.RoomStatus != ROOM_STATUS.UNABLE).Select(t => new RoomDTO
                        {
                            RoomId = t.RoomId,
                            RoomTypeId = x.RoomTypeId,
                            RoomTypeName = x.RoomTypeName,
                            RoomStatus = t.RoomStatus,
                            RoomNumber = t.RoomNumber,
                            Note = t.Note,
                        }).ToList(),
                    }).ToListAsync();
                    foreach (var item in list)
                    {
                        foreach (var room in item.Rooms)
                        {
                            if (room.RoomStatus== ROOM_STATUS.RENTING) {
                                RentalContract rentalContract = await context.RentalContracts.FirstOrDefaultAsync(x => x.RoomId == room.RoomId && x.Validated == true);
                                room.RentalContractId = rentalContract.RentalContractId;
                                room.StartDate = rentalContract.StartDate;
                                room.EndDate = rentalContract.EndDate;
                                room.CountPerson = rentalContract.RentalContractDetails.Count;
                                room.RentalPrice = (double)rentalContract.RentalPrice;
                            }
                        }
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

        
