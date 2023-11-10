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
        public async Task<double> GetPriceBooking(string roomId, List<RentalContractDetailDTO> ListCustomer)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    int numPerForUnitPrice = (int)context.Parameters.FirstOrDefault(x => x.ParameterKey == "SoKhachKhongTinhPhuPhi").ParameterValue;
                    var listSurcharge = await context.SurchargeRates.ToListAsync();

                    Room r = await context.Rooms.FindAsync(roomId);
                    double RoomTypePrice = (double)r.RoomType.Price;
                    int numPer = ListCustomer.Count;
                    double PricePerDay = RoomTypePrice;
                    if (numPer > numPerForUnitPrice)
                    {
                        for (int i = numPerForUnitPrice + 1; i <= numPer; i++)
                        {
                            PricePerDay += RoomTypePrice * (double)listSurcharge[i - (numPerForUnitPrice + 1)].Rate;
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

                    int index = 1;
                    List<RentalContractDTO> RentalContractDTOs = await (
                        from r in context.RentalContracts
                        join room in context.Rooms
                        on r.RoomId equals room.RoomId
                        select new RentalContractDTO
                        {
                            RentalContractId = r.RentalContractId,
                            CreateDate = r.StartDate,
                            RoomId = r.RoomId,
                            RoomNumber = (int)room.RoomNumber,
                            RentalContracts = r.RentalContractDetails.Select(x => new RentalContractDetailDTO
                            {
                                RentalContractId = x.RentalContractId,
                                CustomerName = x.CustomerName,
                                CCCD = x.CustomerId,
                            }).ToList()
                        }
                    ).ToListAsync();
                    RentalContractDTOs.Reverse();
                    foreach (var r in RentalContractDTOs)
                    {
                        r.STT_RentalContract = index++;
                        int index2 = 1;
                        foreach (var item in r.RentalContracts)
                        {
                            item.STT = index2++;
                        }
                    }
                    if (yearstr != "Tất cả")
                    {
                        int year = int.Parse(yearstr.Substring(4));
                        RentalContractDTOs = new List<RentalContractDTO>(RentalContractDTOs.Where(x => x.CreateDate.Value.Year == year).ToList());
                    }
                    if (monthstr != "Tất cả")
                    {
                        int month = int.Parse(monthstr.Substring(6));
                        RentalContractDTOs = new List<RentalContractDTO>(RentalContractDTOs.Where(x => x.CreateDate.Value.Month == month).ToList());
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
                        StartDate = DateTime.Now,
                    };

                    context.RentalContracts.Add(rc);
                    await context.SaveChangesAsync();

                    foreach (var item in list)
                    {
                        var maxCPId = await context.RentalContractDetails.MaxAsync(s => s.RentalContractDetailId);
                        string CPId = CreateNextRentalContractId(maxCPId.ToString());
                        RentalContractDetail rentalContractDetail = new RentalContractDetail
                        {
                            RentalContractDetailId = Int32.Parse(CPId),
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

                        var maxCPId = await context.RentalContractDetails.MaxAsync(s => s.RentalContractDetailId);
                        string CPId = CreateNextRentalContractId(maxCPId.ToString());
                        RentalContractDetail rentalContractDetail = new RentalContractDetail
                        {
                            RentalContractDetailId = Int32.Parse(CPId),
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
            //KHxxx
            if (maxId is null)
            {
                return "PT001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "PT" + newIdString.Substring(newIdString.Length - 3, 3);
        }
        private string CreateNextRentalContractDetailId(string maxId)
        {
            //KHxxx
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
                        Price = (double)x.RoomType.Price,
                    }).ToListAsync();

                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<double> GetRentalContractPrice(string rentId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    int numPerForUnitPrice = (int)context.Parameters.FirstOrDefault(x => x.ParameterKey == "SoKhachKhongTinhPhuPhi").ParameterValue;
                    var listSurcharge = await context.SurchargeRates.ToListAsync();


                    RentalContract rentalContract = await context.RentalContracts.FindAsync(rentId);
                    int numPer = rentalContract.RentalContractDetails.Count;
                    double RoomTypePrice = (double)rentalContract.Room.RoomType.Price;
                    double PricePerDay = RoomTypePrice;
                    if (numPer > numPerForUnitPrice)
                    {
                        for (int i = 0; i < numPer - numPerForUnitPrice; i++)
                        {
                            PricePerDay += RoomTypePrice * (double)listSurcharge[i].Rate;
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
        public async Task<List<RentalContractDTO>> GetRentalContractList()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    int index = 1;
                    var RentalContractDTOs = await db.RentalContracts.OrderByDescending(x => x.StartDate).Select(r => new RentalContractDTO
                    {
                        RentalContractId = r.RentalContractId,
                        CreateDate = r.StartDate,
                        RoomId = r.RoomId,
                        RoomNumber = (int)r.Room.RoomNumber,
                        RentalContracts = r.RentalContractDetails.Select(x => new RentalContractDetailDTO
                        {
                            RentalContractId = x.RentalContractId,
                            CustomerName = x.CustomerName,
                            CCCD = x.CustomerId,
                        }).ToList()

                    }).ToListAsync();


                    foreach (var r in RentalContractDTOs)
                    {
                        r.STT_RentalContract = index++;
                        int index2 = 1;
                        foreach (var item in r.RentalContracts)
                        {
                            item.STT = index2++;
                        }
                    }


                    return RentalContractDTOs;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int GetMaxNumOfPer()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    int res = (int)context.Parameters.FirstOrDefault(x => x.ParameterKey == "SoKhachToiDa").ParameterValue;
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
