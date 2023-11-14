using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class RoomTypeService
    {
        public RoomTypeService() { }
        private static RoomTypeService _ins;
        public static RoomTypeService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new RoomTypeService();
                return _ins;
            }
            private set { _ins = value; }
        }
        public List<string> GetListRoomTypeName()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var list = context.RoomTypes.Select(x => x.RoomTypeName).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetRoomTypeID(string rtn)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    var item = await db.RoomTypes.Where(x => x.RoomTypeName == rtn).FirstOrDefaultAsync();
                    return item.RoomTypeId;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        // Take all RoomType 
        public async Task<List<RoomTypeDTO>> GetAllRoomType()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<RoomTypeDTO> RoomTypeDTOs = await (
                        from rt in db.RoomTypes
                        select new RoomTypeDTO
                        {
                            // DTO = db
                            RoomTypeId = rt.RoomTypeId,
                            RoomTypeName = rt.RoomTypeName,
                            RoomTypePrice = (double)rt.Price,
                            MaxNumberGuest = (int)rt.MaxNumberGuest,
                            NumberGuestForUnitPrice = (int)rt.NumberGuestForUnitPrice,
                        }
                    ).ToListAsync();
                    RoomTypeDTOs.Reverse();
                    return RoomTypeDTOs;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string CreateNextRoomTypeCode(string maxCode)
        {
            if (maxCode == "")
            {
                return "LP001";
            }
            int index = (int.Parse(maxCode.Substring(2)) + 1);
            string CodeID = index.ToString();
            while (CodeID.Length < 3) CodeID = "0" + CodeID;

            return "LP" + CodeID;
        }

        public async Task<(bool, string, RoomTypeDTO)> AddRoomType(RoomTypeDTO newRoomType)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RoomType rt = context.RoomTypes.Where((RoomType RoomType) => RoomType.RoomTypeName == newRoomType.RoomTypeName).FirstOrDefault();

                    if (rt != null)
                    {
                        return (false, $"Loại phòng {rt.RoomTypeName} đã tồn tại!", null);
                    }
                    else
                    {
                        var listid = await context.RoomTypes.Select(s => s.RoomTypeId).ToListAsync();
                        string maxId = "";

                        if (listid.Count > 0)
                            maxId = listid[listid.Count - 1];

                        string id = CreateNextRoomTypeCode(maxId);
                        RoomType roomtype = new RoomType
                        {
                            RoomTypeId = id,
                            RoomTypeName = newRoomType.RoomTypeName,
                            Price = newRoomType.RoomTypePrice,
                            MaxNumberGuest = newRoomType.MaxNumberGuest,
                            NumberGuestForUnitPrice = newRoomType.NumberGuestForUnitPrice,
                        };

                        if (newRoomType.ListSurcharges != null)
                        {
                            for (int i = 0; i < newRoomType.ListSurcharges.Count(); i++)
                            {
                                SurchargeRate sr = new SurchargeRate
                                {
                                    RoomTypeId = roomtype.RoomTypeId,
                                    CustomerOutIndex = newRoomType.ListSurcharges[i].CustomerOutIndex,
                                    Rate = newRoomType.ListSurcharges[i].Rate
                                };
                                context.SurchargeRates.Add(sr);
                            }
                        }
                        context.RoomTypes.Add(roomtype);
                        await context.SaveChangesAsync();
                        newRoomType.RoomTypeId = roomtype.RoomTypeId;
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
            return (true, "Thêm loại phòng thành công", newRoomType);
        }

        public async Task<(bool, string)> UpdateRoomType(RoomTypeDTO updatedRoomType)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RoomType roomType = context.RoomTypes.Find(updatedRoomType.RoomTypeId);

                    if (roomType is null)
                    {
                        return (false, "Loại phòng này không tồn tại!");
                    }
                    
                    bool IsExistRoomTypeName = context.RoomTypes.Any((RoomType rt) => rt.RoomTypeId != roomType.RoomTypeId && rt.RoomTypeName == updatedRoomType.RoomTypeName);
                    if (IsExistRoomTypeName)
                    {
                        return (false, "Tên loại phòng đã tồn tại!");
                    }
                    var list = await context.SurchargeRates.Where(x => x.RoomTypeId == updatedRoomType.RoomTypeId).ToListAsync();
                    context.SurchargeRates.RemoveRange(list);
                    await context.SaveChangesAsync();
                    
                    roomType.RoomTypeName = updatedRoomType.RoomTypeName;
                    roomType.Price = updatedRoomType.RoomTypePrice;
                    roomType.RoomTypeId = updatedRoomType.RoomTypeId;
                    roomType.MaxNumberGuest = updatedRoomType.MaxNumberGuest;
                    roomType.NumberGuestForUnitPrice = updatedRoomType.NumberGuestForUnitPrice;
                    
                    if (updatedRoomType.ListSurcharges != null)
                    {
                        for (int i = 0; i < updatedRoomType.ListSurcharges.Count; i++)
                        {
                            SurchargeRate sr = new SurchargeRate
                            {
                                RoomTypeId = updatedRoomType.RoomTypeId,
                                CustomerOutIndex = updatedRoomType.ListSurcharges[i].CustomerOutIndex,
                                Rate = updatedRoomType.ListSurcharges[i].Rate
                            };
                            context.SurchargeRates.Add(sr);
                        }
                    }
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

        public async Task<(bool, string)> DeleteRoomType(string Id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room room = await (from p in context.Rooms
                                       where p.RoomTypeId == Id
                                       select p).FirstOrDefaultAsync();
                    RoomType roomtype = await (from p in context.RoomTypes
                                               where p.RoomTypeId == Id
                                               select p).FirstOrDefaultAsync();
                    var listSurchargeRates = await context.SurchargeRates.Where(x => x.RoomTypeId == Id).ToListAsync();

                    if (room is null)
                    {
                        if (roomtype is null)
                        {
                            return (false, "Không tìm thấy loại phòng này !");
                        }
                        else
                        {
                            context.SurchargeRates.RemoveRange(listSurchargeRates);
                            await context.SaveChangesAsync();

                            context.RoomTypes.Remove(roomtype);
                            await context.SaveChangesAsync();                           
                        }

                    }
                    else
                    {
                        return (false, "Loại phòng này đã áp dụng trước đây và đang có khách đặt. Không thể xóa!");
                    }
                }
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống");
            }
            return (true, "Xóa loại phòng thành công");
        }
    }
}
