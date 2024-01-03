using HotelManagement.DTOs;
using HotelManagement.Utils;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class BookingRoomService
    {
        private static BookingRoomService _ins;
        public static BookingRoomService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BookingRoomService();
                }
                return _ins;
            }
            private set { _ins = value; }
        }

        public BookingRoomService() { }
        public List<string> GetListFilterYear()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var listYear = context.RentalContracts.Select(x => x.StartDate.Value.Year).ToList();
                    if (listYear == null) listYear = new List<int>();
                    if (!listYear.Contains(DateTime.Now.Year))
                    {
                        listYear.Add(DateTime.Now.Year);
                    }
                    var listYearStr = listYear.Select(x => "Năm " + x.ToString()).ToList();
                    listYearStr.Reverse();
                    List<string> list = new List<string>();
                    foreach (var item in listYearStr)
                    {
                        if (!list.Contains(item)) list.Add(item);
                    }
                    list.Insert(0, "Tất cả");
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<float> GetPriceBooking(string roomId, List<RentalContractDetailDTO> ListCustomer)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Room r = await context.Rooms.FindAsync(roomId);
                    var listSurcharge = await context.SurchargeRates.Where(x => x.RoomTypeId == r.RoomTypeId).ToListAsync();
                    int numPerForUnitPrice = (int)r.RoomType.NumberGuestForUnitPrice;
                    float RoomTypePrice = (float)r.RoomType.Price;

                    int numPer = ListCustomer.Count;

                    float PricePerDay = RoomTypePrice;

                    if (numPer > numPerForUnitPrice)
                    {
                        for (int i = 1; i <= numPer - numPerForUnitPrice; i++)
                        {
                            PricePerDay += RoomTypePrice * (float)listSurcharge[i - 1].Rate;
                        }
                    }
                    return PricePerDay;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<RentalContractDTO>> GetRentalContractListFilter(string yearstr, string monthstr)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    List<RentalContractDTO> RentalContractDTOs = await (
                        from r in context.RentalContracts
                        join room in context.Rooms
                        on r.RoomId equals room.RoomId
                        select new RentalContractDTO
                        {
                            RentalContractId = r.RentalContractId,
                            CreateDate = (DateTime)r.StartDate,
                            RoomId = r.RoomId,
                            RoomNumber = (int)room.RoomNumber,
                            StaffId = r.StaffId,
                            StaffName = r.Staff.StaffName,
                            EndDate = (DateTime)r.EndDate,
                            RentalPrice = (float)r.RentalPrice,
                            Validated = (bool)r.Validated,
                            ListRentalContractDetails = r.RentalContractDetails.Select(x => new RentalContractDetailDTO
                            {
                                RentalContractId = x.RentalContractId,
                                CustomerName = x.CustomerName,
                                CCCD = x.CustomerId,
                            }).ToList()
                        }
                    ).ToListAsync();
                    RentalContractDTOs.Reverse();

                    if (yearstr != "Tất cả")
                    {
                        int year = int.Parse(yearstr.Substring(4));
                        RentalContractDTOs = new List<RentalContractDTO>(RentalContractDTOs.Where(x => x.CreateDate.Year == year).ToList());
                    }
                    if (monthstr != "Tất cả")
                    {
                        int month = int.Parse(monthstr.Substring(6));
                        RentalContractDTOs = new List<RentalContractDTO>(RentalContractDTOs.Where(x => x.CreateDate.Month == month).ToList());
                    }


                    return RentalContractDTOs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<(bool, string)> DeleteRentalContract(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RentalContract rentalContract = await context.RentalContracts.FindAsync(rentalContractId);
                    context.RentalContractDetails.RemoveRange(rentalContract.RentalContractDetails);
                    await context.SaveChangesAsync();
                    context.RentalContracts.Remove(rentalContract);
                    await context.SaveChangesAsync();
                    return (true, "Xóa phiếu thuê thành công");
                }
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống!");
            }

        }

        public async Task<(bool, string)> SaveRental(RentalContractDTO rentalContract, List<RentalContractDetailDTO> list)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var maxId = await context.RentalContracts.MaxAsync(s => s.RentalContractId);
                    string rentalId = CreateNextRentalContractId(maxId);
                    RentalContract rc = new RentalContract
                    {
                        RentalContractId = rentalId,
                        RoomId = rentalContract.RoomId,
                        StaffId = rentalContract.StaffId,
                        StartDate = DateTime.Now,
                        EndDate = rentalContract.EndDate,
                        RentalPrice = rentalContract.RentalPrice,
                        Validated = true,
                    };

                    context.RentalContracts.Add(rc);
                    await context.SaveChangesAsync();

                    foreach (var item in list)
                    {

                        RentalContractDetail rentalContractDetail = new RentalContractDetail
                        {
                            RentalContractId = rentalId,
                            CustomerName = item.CustomerName,
                            CustomerId = item.CCCD,
                        };
                        context.RentalContractDetails.Add(rentalContractDetail);
                        await context.SaveChangesAsync();
                    }
                    return (true, "Xác nhận đặt phòng thành công!");
                }
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }
        public async Task<(bool, string)> UpdateListCustomer(RentalContractDTO rentalContract, List<RentalContractDetailDTO> list)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    string rentalId = rentalContract.RentalContractId;
                    var listDetail = context.RentalContracts.FindAsync(rentalId).Result.RentalContractDetails.ToList();
                    context.RentalContractDetails.RemoveRange(listDetail);
                    await context.SaveChangesAsync();
                    foreach (var item in list)
                    {
                        RentalContractDetail rentalContractDetail = new RentalContractDetail
                        {
                            RentalContractId = rentalId,
                            CustomerName = item.CustomerName,
                            CustomerId = item.CCCD,
                        };
                        context.RentalContractDetails.Add(rentalContractDetail);
                        await context.SaveChangesAsync();
                    }
                    return (true, "Cập nhật phiếu thuê thành công!");
                }
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }
        private string CreateNextRentalContractId(string maxId)
        {
            if (maxId is null)
            {
                return "PT001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "PT" + newIdString.Substring(newIdString.Length - 3, 3);
        }
        private string CreateNextRentalContractDetailId(string maxId)
        {
            if (maxId is null)
            {
                return "CP001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "CP" + newIdString.Substring(newIdString.Length - 3, 3);
        }
        public async Task<List<RoomDTO>> GetListReadyRoom()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var list = await context.Rooms.Where(x => x.RoomStatus == ROOM_STATUS.READY).Select(x => new RoomDTO
                    {
                        RoomId = x.RoomId,
                        RoomNumber = (int)x.RoomNumber,
                        RoomTypeName = x.RoomType.RoomTypeName,
                        RoomTypeId = x.RoomTypeId,
                        Price = (float)x.RoomType.Price,
                        MaxNumberGuest = (int)x.RoomType.MaxNumberGuest,
                        NumberGuestForUnitPrice = (int)x.RoomType.NumberGuestForUnitPrice,
                    }).ToListAsync();

                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<float> GetRentalContractPrice(string rentId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var listSurcharge = await context.SurchargeRates.ToListAsync();


                    RentalContract rentalContract = await context.RentalContracts.FindAsync(rentId);

                    int numPerForUnitPrice = (int)context.RoomTypes.FirstOrDefault(x => x.RoomTypeId == rentalContract.Room.RoomTypeId).NumberGuestForUnitPrice;

                    int numPer = rentalContract.RentalContractDetails.Count;
                    float RoomTypePrice = (float)rentalContract.Room.RoomType.Price;
                    float PricePerDay = RoomTypePrice;
                    if (numPer > numPerForUnitPrice)
                    {
                        for (int i = 0; i < numPer - numPerForUnitPrice; i++)
                        {
                            PricePerDay += RoomTypePrice * (float)listSurcharge[i].Rate;
                        }
                    }
                    return PricePerDay;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<string> GetCurrentRoomStatus(string rentId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    string res = "";
                    RentalContract currentRental = await context.RentalContracts.FindAsync(rentId);
                    Room room = currentRental.Room;
                    res = room.RoomStatus.ToString();

                    if (room.RoomStatus.ToString() == ROOM_STATUS.RENTING)
                    {
                        var listRentalId = await context.RentalContracts.Where(x => x.RoomId == room.RoomId).Select(x => x.RentalContractId).ToListAsync();
                        listRentalId.Reverse();
                        if (rentId == listRentalId[0])
                        {
                            res = ROOM_STATUS.RENTING.ToString();
                        }
                        else
                        {
                            res = ROOM_STATUS.READY;
                        }

                    }
                    return res;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<RentalContractDetailDTO>> GetCustomersOfRoom(string RentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var listCustomer = await context.RentalContractDetails.Where(x => x.RentalContractId == RentalContractId).Select(x => new RentalContractDetailDTO
                    {
                        CustomerName = x.CustomerName,
                        CCCD = x.CustomerId,
                        RentalContractId = x.RentalContractId,
                        RentalContractDetailId = x.RentalContractDetailId,
                    }).ToListAsync();
                    for (int i = 0; i < listCustomer.Count; i++)
                    {
                        listCustomer[i].STT = i + 1;
                    }

                    return listCustomer;


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<RentalContractDTO>> GetRentalContractList()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    var RentalContractDTOs = await db.RentalContracts.OrderByDescending(x => x.StartDate).Select(r => new RentalContractDTO
                    {
                        RentalContractId = r.RentalContractId,
                        CreateDate = (DateTime)r.StartDate,
                        RoomId = r.RoomId,
                        RoomNumber = (int)r.Room.RoomNumber,
                        StaffId = r.StaffId,
                        StaffName = r.Staff.StaffName,
                        EndDate = (DateTime)r.EndDate,
                        RentalPrice = (float)r.RentalPrice,
                        Validated = (bool)r.Validated,
                        ListRentalContractDetails = r.RentalContractDetails.Select(x => new RentalContractDetailDTO
                        {
                            RentalContractId = x.RentalContractId,
                            CustomerName = x.CustomerName,
                            CCCD = x.CustomerId,
                            isVisibleBtn = (bool)r.Validated ? "Visible" : "Hidden",
                        }).ToList()
                    }).ToListAsync();

        
                    return RentalContractDTOs;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int GetMaxNumOfPer(string roomId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    string roomtypeId = context.Rooms.FirstOrDefault(x => x.RoomId == roomId).RoomTypeId;
                    int res = (int)context.RoomTypes.FirstOrDefault(x => x.RoomTypeId == roomtypeId).MaxNumberGuest;
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}