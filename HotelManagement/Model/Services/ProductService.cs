using HotelManagement.DTOs;
using HotelManagement.ViewModel.AdminVM;
using HotelManagement.ViewModel.StaffVM;
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
    public class ProductService
    {
        private ProductService() { }
        private static ProductService _ins;
        public static ProductService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new ProductService();
                return _ins;
            }
            private set { _ins = value; }
        }

      

        
  

        public async Task<(bool, string, List<ProductDTO>)> GetAllProduct()
        {
            try
            {
                List<ProductDTO> listProduct = new List<ProductDTO>();
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    listProduct = await (
                        from p in db.Products
                        select new ProductDTO
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            ProductType= p.ProductType,
                            ProductAvatarData = p.ProductAvatar,
                            Quantity = (int)p.QuantityOfStorage,
                            Price = (double)p.Price,
                        }
                    ).ToListAsync();
                    listProduct.ForEach(item => item.SetAvatar());
                }

                return (true, "Thành công", listProduct);
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }
        public List<ProductDTO> GetAllServiceByType(string typeSelection, ObservableCollection<ProductDTO> allService)
        {
            try
            {
                return allService.Where(item => item.ProductType == typeSelection).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string CreateServiceID(string maxId)
        {
            if (maxId is null)
            {
                return "DV001";
            }
            string numStr = (int.Parse(maxId.Substring(2)) + 1).ToString();
            while (numStr.Length < 3) numStr = "0" + numStr;
            return "DV" + numStr;
        }
        public string CreateGoodReceiptID(string maxId)
        {
            if (maxId is null)
            {
                return "GC001";
            }
            string numStr = (int.Parse(maxId.Substring(2)) + 1).ToString();
            while (numStr.Length < 3) numStr = "0" + numStr;
            return "GC" + numStr;
        }
   


    }
   
}
