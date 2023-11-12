using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class RoomCustomerService
    {
        private static RoomCustomerService _ins;
        public static RoomCustomerService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RoomCustomerService();
                }
                return _ins;
            }
            private set => _ins = value;
        }
        private RoomCustomerService() { }

        public async Task<List<RentalContractDetailDTO>> GetCustomersOfRoom(string RentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    
                    var listCustomer = await context.RentalContractDetails.Where(x=> x.RentalContractId == RentalContractId).Select(x => new RentalContractDetailDTO
                    {
                        CustomerName = x.CustomerName,
                        CustomerId = x.CustomerId,
                        RentalContractId= x.RentalContractId,
                        RentalContractDetailId= x.RentalContractDetailId,   
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

        public async Task<int> GetPersonNumberOfRoom(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var customerList = await context.RentalContractDetails.Where(x => x.RentalContractId == rentalContractId).CountAsync();

                    return customerList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool, string, List<RentalContractDetailDTO>)> AddRoomCustomer(RentalContractDetailDTO roomCustomer)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var listCCCD = await context.RentalContractDetails.Where(x=> x.RentalContractId == roomCustomer.RentalContractId).Select(x=> x.CustomerId).ToListAsync(); 
                    if (listCCCD != null)
                    {
                        if (listCCCD.Contains(roomCustomer.CustomerId))
                        {
                            return (false, "Thêm thất bại! Mã CCCD bị trùng!", null);
                        }
                    }
                    RentalContractDetail rc = new RentalContractDetail
                    {
                        CustomerName = roomCustomer.CustomerName,
                        CustomerId = roomCustomer.CustomerId,
                        RentalContractId= roomCustomer.RentalContractId,
                    };
                    context.RentalContractDetails.Add(rc);
                    await context.SaveChangesAsync();
                    
                    
                    var listCustomer = await context.RentalContractDetails.Where(x=> x.RentalContractId == roomCustomer.RentalContractId).Select(x=> new RentalContractDetailDTO
                    {
                        CustomerName = x.CustomerName,
                        CustomerId = x.CustomerId,
                        RentalContractId = x.RentalContractId,
                        RentalContractDetailId = x.RentalContractDetailId,
                    }).ToListAsync();
                    for (int i = 0; i < listCustomer.Count; i++)
                    {
                        listCustomer[i].STT = i + 1;
                    }

                    return (true, "Thêm khách ở thành công!", listCustomer);
                }
            }
            catch(Exception ex)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }
        public async Task<(bool, string, List<RentalContractDetailDTO>)> UpdateRoomCustomer(RentalContractDetailDTO roomCustomer)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    
                    RentalContractDetail cus = await context.RentalContractDetails.Where(x => x.RentalContractDetailId == roomCustomer.RentalContractDetailId).FirstOrDefaultAsync();
                    var listCCCD = await context.RentalContractDetails.Where(x => x.RentalContractId == roomCustomer.RentalContractId && x.RentalContractDetailId != cus.RentalContractDetailId).Select(x => x.CustomerId).ToListAsync();
                    if (listCCCD != null)
                    {
                        if (listCCCD.Contains(roomCustomer.CustomerId))
                        {
                            return (false, "Cập nhật thất bại! Mã CCCD bị trùng!", null);
                        }
                    }
                    cus.CustomerName = roomCustomer.CustomerName;
                    cus.CustomerId = roomCustomer.CustomerId;
                 
                    await context.SaveChangesAsync();

                    var listCustomer = await context.RentalContractDetails.Where(x => x.RentalContractId == roomCustomer.RentalContractId).Select(x => new RentalContractDetailDTO
                    {
                       
                        CustomerName = x.CustomerName,
                        CustomerId = x.CustomerId,
                        RentalContractId = x.RentalContractId,
                        RentalContractDetailId = x.RentalContractDetailId,
                    }).ToListAsync();
                    for (int i = 0; i < listCustomer.Count; i++)
                    {
                        listCustomer[i].STT = i + 1;
                    }
                    return (true, "Cập nhật thông tin khách ở thành công!", listCustomer);
                }
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }
        public async Task<(bool, string, List<RentalContractDetailDTO>)> DeleteRoomCustomer(RentalContractDetailDTO roomCustomer)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    RentalContractDetail cus = await context.RentalContractDetails.Where(x => x.RentalContractDetailId == roomCustomer.RentalContractDetailId).FirstOrDefaultAsync();
                    context.RentalContractDetails.Remove(cus);
                    await context.SaveChangesAsync();

                    var listCustomer = await context.RentalContractDetails.Where(x => x.RentalContractId == roomCustomer.RentalContractId).Select(x => new RentalContractDetailDTO
                    {

                        CustomerName = x.CustomerName,
                        CustomerId = x.CustomerId,
                        RentalContractId = x.RentalContractId,
                        RentalContractDetailId = x.RentalContractDetailId,
                    }).ToListAsync();
                    for (int i = 0; i < listCustomer.Count; i++)
                    {
                        listCustomer[i].STT = i + 1;
                    }
                    return (true, "Cập nhật thông tin khách ở thành công!", listCustomer);
                }
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }

    }
}
