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

       
      

        public async Task<(bool,string)> SaveBooking(RentalContractDTO rentalContract)
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
                        RoomId= rentalContract.RoomId,
                        StaffId = rentalContract.StaffId,
                        StartDate = rentalContract.StartDate,
                        EndDate = rentalContract.StartDate,
                        Validated = rentalContract.Validated,
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
        //public async Task<(bool, string, string)> SaveCustomer(CustomerDTO customer)
        //{
        //    try
        //    {
        //        using (var context = new HotelManagementEntities())
        //        {
        //            var c = await context.Customers.FirstOrDefaultAsync(x => x.CCCD == customer.CCCD);
        //            var maxId = await context.Customers.MaxAsync(s => s.CustomerId);
        //            if (c != null) return (true, null,c.CustomerId);
        //            else
        //            {
        //                string newCusId = CreateNextCustomerId(maxId);
        //                Customer newCus = new Customer
        //                {
        //                    CustomerName = customer.CustomerName,
        //                    CCCD = customer.CCCD,
        //                    CustomerAddress = customer.CustomerAddress,
        //                    PhoneNumber = customer.PhoneNumber,
        //                    Email = customer.Email,
        //                    CustomerId = newCusId,
        //                    CustomerType = customer.CustomerType,
        //                    DateOfBirth = customer.DateOfBirth,
        //                    Gender = customer.Gender,
        //                    IsDeleted = customer.IsDeleted,
        //                };
        //                context.Customers.Add(newCus);
        //                await context.SaveChangesAsync();
        //                string cusId = (await context.Customers.FirstOrDefaultAsync(x => x.CCCD == customer.CCCD)).CustomerId;
        //                return (true, "", cusId);
        //            }
        //        }
        //    }
        //    catch (System.Data.Entity.Core.EntityException)
        //    {
        //        return (false, "Mất kết nối cơ sở dữ liệu",null);
        //    }
        //    catch (Exception)
        //    {
        //        return (false, "Lỗi hệ thống",null);
        //    }
        //}
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
     
        private string CreateNextCustomerId(string maxId)
        {
            //KHxxx
            if (maxId is null)
            {
                return "KH001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "KH" + newIdString.Substring(newIdString.Length - 3, 3);
        }
        private string CreateNextRentalContractId(string maxId)
        {
            //KHxxx
            if (maxId is null)
            {
                throw ex;
            }

        }
    }
}
