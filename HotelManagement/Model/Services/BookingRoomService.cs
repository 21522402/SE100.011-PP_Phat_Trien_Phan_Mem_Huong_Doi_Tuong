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
                    rentalContract.RentalContractId = rc.RentalContractId;
                    return (true, "Đặt phòng thành công!");
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

        public async Task<(bool, string)> DeleteRentalContractBooked(string Id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RentalContract rental = await (from p in context.RentalContracts
                                       where p.RentalContractId == Id
                                       select p).FirstOrDefaultAsync();
                    if (rental == null)
                    {
                        return (false, "Phiếu thuê phòng này không tồn tại!");
                    }

                    rental.Room.RoomStatus = ROOM_STATUS.READY;

                    context.RentalContracts.Remove(rental);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống");
            }
            return (true, "Xóa phiếu thuê phòng thành công");
        }

        public async Task<(bool, string)> DeleteRentalContractOutDate(string Id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RentalContract rental = await (from p in context.RentalContracts
                                                   where p.RentalContractId == Id
                                                   select p).FirstOrDefaultAsync();
                    if (rental == null)
                    {
                        return (false, "Phiếu thuê phòng này không tồn tại!");
                    }

                    context.RentalContracts.Remove(rental);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return (false, "Không thể xóa do ràng buộc cơ sở dữ liệu!");
            }
            return (true, "Xóa phiếu thuê phòng thành công");
        }
        public async Task<string> GetRoomStatusBy(string rtID)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    return (await context.RentalContracts.FindAsync(rtID)).Room.RoomStatus;
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
                return "PT001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "PT" + newIdString.Substring(newIdString.Length - 3, 3);
        }

    }
}
