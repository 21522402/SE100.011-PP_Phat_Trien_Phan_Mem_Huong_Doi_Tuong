using HotelManagement.DTOs;
using HotelManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class RentalContractService
    {
        private static RentalContractService _ins;
        public static RentalContractService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RentalContractService();
                }
                return _ins;
            }
            private set => _ins = value;
        }
        private RentalContractService() { }
        public async Task<List<RentalContractDTO>> GetAllRentalContracts()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var rentalContractList = await (from r in context.RentalContracts
                                         
                                          select new RentalContractDTO
                                          {
                                             RentalContractId = r.RentalContractId,
                                             StartDate= r.StartDate,
                                             EndDate= r.EndDate,
                                             RoomId= r.RoomId,
                                             Validated=r.Validated,
                                          }
                                          ).ToListAsync();
                    
                    return rentalContractList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<RentalContractDTO>> GetRentalContractsNow()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var rentalContractList = await (from r in context.RentalContracts

                                                    select new RentalContractDTO
                                                    {
                                                        RentalContractId = r.RentalContractId,
                                                        StartDate = r.StartDate,
                                                        EndDate = r.EndDate,
                                                        RoomId = r.RoomId,
                                                        Validated = r.Validated,
                                                    }
                                          ).ToListAsync();
                    rentalContractList = rentalContractList.Where(x => x.EndDate  > DateTime.Today  && x.StartDate  <= DateTime.Today ).ToList();

                    return rentalContractList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<RentalContractDTO> GetRentalContractById(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var res = await context.RentalContracts.Select(x=> new RentalContractDTO
                    {
                        RentalContractId=x.RentalContractId,
                        RoomId=x.RoomId, 
                        RoomNumber = x.Room.RoomNumber,
                        EndDate = x.EndDate,
                        StartDate = x.StartDate, 
                        Validated= x.Validated,
                    }).FirstAsync(x => x.RentalContractId == rentalContractId);
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        //public async Task<List<RoomCustomerDTO>> GetCustomersOfRoom(string RentalContractId)
        //{
        //    try
        //    {
        //        using (var context = new HotelManagementEntities())
        //        {
        //            var listCustomerId = await context.RoomCustomers.Where(x=> x.RentalContractId == RentalContractId).Select(x=> x.CustomerId).ToListAsync();
        //            var listCustomer = await context.Customers.Where(x => listCustomerId.Contains(x.CustomerId)).Select(x => new RoomCustomerDTO
        //            {
        //                CustomerName = x.CustomerName,
        //                CustomerType = x.CustomerType,
        //                CCCD = x.CCCD,
        //                CustomerAddress= x.CustomerAddress,
        //            }).ToListAsync();
        //            for (int i=0; i<listCustomer.Count; i++)
        //            {
        //                listCustomer[i].STT = i + 1;
        //            }

        //            return listCustomer;

        //return listCustomer;
        //return new List<RoomCustomerDTO>();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<int> GetPersonNumber(string rentalContractId)
        //{
        //    try
        //    {
        //        using (var context = new HotelManagementEntities())
        //        {
        //            var customerList = await context.RoomCustomers.Where(x => x.RentalContractId == rentalContractId).Select(x => x.CustomerId).CountAsync();
        //            return customerList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
