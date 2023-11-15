using HotelManagement.DTOs;
using HotelManagement.Utils;
using HotelManagement.ViewModel.StaffVM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class BillService
    {
        public BillService() { }
        private static BillService _ins;
        public static BillService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BillService();
                }
                return _ins;
            }
            private set { _ins = value; }
        }


        public async Task<BillDTO> GetBillByRentalContract(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var rentalContract = await context.RentalContracts.FindAsync(rentalContractId);
                    var billDTO = new BillDTO
                    {
                        RentalContractId = rentalContract.RentalContractId,
                        RoomId = rentalContract.RoomId,
                        RoomNumber = (int)rentalContract.Room.RoomNumber,
                        RoomTypeName = rentalContract.Room.RoomType.RoomTypeName,
                        StartDate = rentalContract.StartDate,
                        PersonNumber = rentalContract.RentalContractDetails.Count,
                        EndDate = rentalContract.EndDate,
                        RentalPrice = rentalContract.RentalPrice,
                        ListListProductPayment = rentalContract.ProductUsings.Select(t => new ProductUsingDTO
                        {
                            RentalContractId = t.RentalContractId,
                            ProductId = t.ProductId,
                            ProductName = t.Product.ProductName,
                            UnitPrice = t.Product.Price,
                            Quantity = t.Quantity,
                        }).ToList()

                    };

                    var listService = billDTO.ListListProductPayment
                                                            .GroupBy(x => x.ProductId)
                                                            .Select(t => new ProductUsingDTO
                                                            {
                                                                RentalContractId = t.First().RentalContractId,
                                                                ProductId = t.First().ProductId,
                                                                ProductName = t.First().ProductName,
                                                                UnitPrice = t.First().UnitPrice,
                                                                Quantity = t.Sum(g => g.Quantity)
                                                            }).ToList();

                    billDTO.ListListProductPayment = listService;
                    return billDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(bool, string)> SaveBill(BillDTO bill)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var maxBillId = await context.Bills.MaxAsync(x => x.BillId);
                    Bill newBill = new Bill
                    {
                        BillId = CreateNextBillId(maxBillId),
                        RentalContractId = bill.RentalContractId,
                        StaffId = bill.StaffId,
                        TotalPrice = bill.TotalPrice,
                        CreateDate = bill.CreateDate,
                    };
                    context.Bills.Add(newBill);
                    await context.SaveChangesAsync();
                    return (true, "Thanh toán thành công!");
                }
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống");
            }
        }
        private string CreateNextBillId(string maxBillId)
        {
            if (maxBillId is null) return "HD001";
            int num = int.Parse(maxBillId.Substring(2));
            string nextNumString = (num + 1).ToString();
            while (nextNumString.Length < 3) nextNumString = "0" + nextNumString;
            return "HD" + nextNumString;

        }
        public async Task<List<BillDTO>> GetAllBill()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var billList = (from b in context.Bills
                                    orderby b.CreateDate descending
                                    select new BillDTO
                                    {
                                        BillId = b.BillId,
                                        RentalContractId = b.RentalContractId,
                                        StaffId = b.StaffId,
                                        StaffName = b.Staff.StaffName,
                                        RoomId = b.RentalContract.RoomId,
                                        RoomNumber = (int)b.RentalContract.Room.RoomNumber,
                                        RoomTypeName = b.RentalContract.Room.RoomType.RoomTypeName,
                                        PersonNumber = b.RentalContract.RentalContractDetails.Count,
                                        RentalPrice=b.RentalContract.RentalPrice,
                                        TotalPrice = b.TotalPrice,
                                        StartDate = b.RentalContract.StartDate,
                                        EndDate = b.RentalContract.EndDate,
                                        CreateDate = b.CreateDate,
                                        ListListProductPayment = b.RentalContract.ProductUsings.Select(t => new ProductUsingDTO
                                        {
                                            ProductUsingId=t.ProductUsingId,
                                            RentalContractId = t.RentalContractId,
                                            ProductId = t.ProductId,
                                            ProductName = t.Product.ProductName,
                                            UnitPrice = t.Product.Price,
                                            Quantity = t.Quantity,
                                        }).ToList()
                                    }).ToListAsync();
                    return await billList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BillDTO>> GetAllBillByDate(DateTime date)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var billList = (from b in context.Bills
                                    where DbFunctions.TruncateTime(b.CreateDate) == date.Date
                                    orderby b.CreateDate descending
                                    select new BillDTO
                                    {
                                        BillId = b.BillId,
                                        RentalContractId = b.RentalContractId,
                                        StaffId = b.StaffId,
                                        StaffName = b.Staff.StaffName,
                                        RoomId = b.RentalContract.RoomId,
                                        RoomNumber = (int)b.RentalContract.Room.RoomNumber,
                                        RoomTypeName = b.RentalContract.Room.RoomType.RoomTypeName,
                                        PersonNumber = b.RentalContract.RentalContractDetails.Count,
                                        RentalPrice = b.RentalContract.RentalPrice,
                                        TotalPrice = b.TotalPrice,
                                        StartDate = b.RentalContract.StartDate,
                                        EndDate = b.RentalContract.EndDate,
                                        CreateDate = b.CreateDate,
                                        ListListProductPayment = b.RentalContract.ProductUsings.Select(t => new ProductUsingDTO
                                        {
                                            ProductUsingId = t.ProductUsingId,
                                            RentalContractId = t.RentalContractId,
                                            ProductId = t.ProductId,
                                            ProductName = t.Product.ProductName,
                                            UnitPrice = t.Product.Price,
                                            Quantity = t.Quantity,
                                        }).ToList()
                                    }).ToListAsync();
                    return await billList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<BillDTO>> GetAllBillByMonth(int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var billList = (from b in context.Bills
                                    where ((DateTime)b.CreateDate).Year == DateTime.Now.Year && ((DateTime)b.CreateDate).Month == month
                                    orderby b.CreateDate descending
                                    select new BillDTO
                                    {
                                        BillId = b.BillId,
                                        RentalContractId = b.RentalContractId,
                                        StaffId = b.StaffId,
                                        StaffName = b.Staff.StaffName,
                                        RoomId = b.RentalContract.RoomId,
                                        RoomNumber = (int)b.RentalContract.Room.RoomNumber,
                                        RoomTypeName = b.RentalContract.Room.RoomType.RoomTypeName,
                                        PersonNumber = b.RentalContract.RentalContractDetails.Count,
                                        RentalPrice = b.RentalContract.RentalPrice,
                                        TotalPrice = b.TotalPrice,
                                        StartDate = b.RentalContract.StartDate,
                                        EndDate = b.RentalContract.EndDate,
                                        CreateDate = b.CreateDate,
                                        ListListProductPayment = b.RentalContract.ProductUsings.Select(t => new ProductUsingDTO
                                        {
                                            ProductUsingId = t.ProductUsingId,
                                            RentalContractId = t.RentalContractId,
                                            ProductId = t.ProductId,
                                            ProductName = t.Product.ProductName,
                                            UnitPrice = t.Product.Price,
                                            Quantity = t.Quantity,
                                        }).ToList()
                                    }).ToListAsync();
                    return await billList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<BillDTO> GetBillDetails(string id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var b = await context.Bills.FindAsync(id);

                    BillDTO billdetail = new BillDTO
                    {
                        BillId = b.BillId,
                        RentalContractId = b.RentalContractId,
                        StaffId = b.StaffId,
                        StaffName = b.Staff.StaffName,
                        RoomId = b.RentalContract.RoomId,
                        RoomNumber = (int)b.RentalContract.Room.RoomNumber,
                        RoomTypeName = b.RentalContract.Room.RoomType.RoomTypeName,
                        PersonNumber = b.RentalContract.RentalContractDetails.Count,
                        RentalPrice = b.RentalContract.RentalPrice,
                        TotalPrice = b.TotalPrice,
                        StartDate = b.RentalContract.StartDate,
                        EndDate = b.RentalContract.EndDate,
                        CreateDate = b.CreateDate,
                        ListListProductPayment = b.RentalContract.ProductUsings.Select(t => new ProductUsingDTO
                        {
                            ProductUsingId = t.ProductUsingId,
                            RentalContractId = t.RentalContractId,
                            ProductId = t.ProductId,
                            ProductName = t.Product.ProductName,
                            UnitPrice = t.Product.Price,
                            Quantity = t.Quantity,
                        }).ToList()
                    };
                    return billdetail;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
