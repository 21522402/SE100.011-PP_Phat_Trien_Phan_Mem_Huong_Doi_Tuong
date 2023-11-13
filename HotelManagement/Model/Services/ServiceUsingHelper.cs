using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class ServiceUsingHelper
    {
        private static ServiceUsingHelper _ins;
        public static ServiceUsingHelper Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ServiceUsingHelper();
                }
                return _ins;
            }
            private set { _ins = value; }
        }
        public ServiceUsingHelper() { }

   
        public async Task<List<ProductUsingDTO>> GetListUsingService(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                   
                    var listUsingService = await context.ProductUsings.Where(x=> x.RentalContractId ==rentalContractId).Select(x=> new ProductUsingDTO 
                    {
                        RentalContractId= x.RentalContractId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.ProductName,
                        UnitPrice = x.Product.Price,
                        Quantity = x.Quantity,
                    }).ToListAsync();

      

                    var listUsingService2 = listUsingService
                                                            .GroupBy(x => x.ProductId)
                                                           .Select(t => new ProductUsingDTO
                                                           {
                                                               RentalContractId = t.First().RentalContractId,
                                                               ProductId = t.First().ProductId,
                                                               ProductName = t.First().ProductName,
                                                               UnitPrice = t.First().UnitPrice,
                                                               Quantity = t.Sum(g => g.Quantity)
                                                           }).ToList();

                   
                    return listUsingService2;
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool, string)> SaveUsingProduct(ObservableCollection<ProductDTO> orderList, RoomDTO selectedRoom)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    int length = orderList.Count();
                    for(int i = 0; i < length; i++)
                    {
                        ProductDTO s = orderList[i];
                        ProductUsing productUsing = await context.ProductUsings.FirstOrDefaultAsync(item => item.ProductId == s.ProductId && item.RentalContractId == selectedRoom.RentalContractId) ;
                        if (productUsing == null)
                        {
                            productUsing = new ProductUsing
                            {
                                RentalContractId = selectedRoom.RentalContractId,
                                ProductId = s.ProductId,
                                UnitPrice = s.Price,
                                Quantity = s.ImportQuantity,
                            };
                            context.ProductUsings.Add(productUsing);
                        }
                        else
                            productUsing.Quantity += s.ImportQuantity;

                        Product product = await context.Products.FirstOrDefaultAsync(item => item.ProductId == s.ProductId);
                        if (product == null)
                            return (false, "Sản phẩm không tồn tại trong kho lưu trữ");
                        else
                            product.QuantityOfStorage -= s.ImportQuantity;
                    }

                    await context.SaveChangesAsync();
                    return (true, "Đặt sản phẩm thành công!");
                }
            }
            catch(EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống!");
            }
        }

    }
}
