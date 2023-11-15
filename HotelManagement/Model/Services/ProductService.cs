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
                    var TiLeGiuaGiaBanVaGiaNhap = await db.Parameters.FirstOrDefaultAsync((item) => item.ParameterKey == "TiLeGiuaGiaBanVaGiaNhap");
                    listProduct = await (
                        from p in db.Products
                        select new ProductDTO
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            ProductType= p.ProductType,
                            ProductAvatarData = p.ProductAvatar,
                            ProductType = p.ProductType,
                            Quantity = (int)p.QuantityOfStorage,
                            ProductPrice = (double)p.Price,
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
                return (false, ex + "", null);
            }
        }

        public async Task<(bool, string)> SaveEditProduct(ProductDTO productSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {

                    Product CheckProductName = await db.Products.FirstOrDefaultAsync(item => item.ProductName.Equals(productSelected.ProductName) && item.ProductId != productSelected.ProductId);
                    if (CheckProductName != null)
                        return (false, "Đã có sản phẩm trong cơ sở  dữ liệu");

                    Product product = await db.Products.FirstOrDefaultAsync(item => item.ProductId == productSelected.ProductId);
                    if (product == null)
                        return (false, "Không tìm thấy sản phẩm trong cơ sở dữ liệu");

                    product.ProductAvatar = productSelected.ProductAvatarData;
                    product.ProductName = productSelected.ProductName;
                    product.ProductType = productSelected.ProductType;
                    product.Price = productSelected.ProductPrice;

                    await db.SaveChangesAsync();

                    return (true, "Cập nhật sản phẩm thành công");
                }
            }
            catch (EntityException ex)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }

        public async Task<(bool, string)> AddProduct(ProductDTO productSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Product product = await db.Products.FirstOrDefaultAsync(item => item.ProductName == productSelected.ProductName && item.ProductType == productSelected.ProductType);

                    if (product != null)
                        return (false, "Đã có sản phẩm trong cơ sở dữ liệu");
                    string maxId = await db.Products.MaxAsync(x => x.ProductId);
                    string newId = CreateProductID(maxId);
                    Product newProduct = new Product
                    {
                        ProductId = newId,
                        ProductName = productSelected.ProductName,
                        ProductType = productSelected.ProductType,
                        Price = productSelected.ProductPrice,
                        QuantityOfStorage = 0,
                        ProductAvatar = productSelected.ProductAvatarData,
                    };

                    db.Products.Add(newProduct);
                    await db.SaveChangesAsync();

                    productSelected.ProductId = newId;
                    return (true, "Thêm sản phẩm thành công");
                }
            }
            catch (EntityException ex)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }

        public async Task<(bool, string)> DeleteProduct(ProductDTO ProductSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Product Product = await db.Products.FirstOrDefaultAsync(item => item.ProductId == ProductSelected.ProductId);

                    if (Product == null)
                        return (false, "Không tìm thấy sản phẩm trong cơ sở dữ liệu");

                    ProductReceiptDetail prd = await db.ProductReceiptDetails.FirstOrDefaultAsync(item => item.ProductId == ProductSelected.ProductId);

                    if (prd != null)
                        return (false, "Không thể xóa sản phẩm do bị ràng buộc tham chiếu của phiếu nhập sản phẩm");

                    db.Products.Remove(Product);

                    await db.SaveChangesAsync();
                    return (true, "Xóa sản phẩm thành công");
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }

        public async Task<(bool, string)> ImportProduct(ProductDTO ProductSelected, double importPrice, int importQuantity)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Product Product = await db.Products.FirstOrDefaultAsync(item => item.ProductId == ProductSelected.ProductId);
                    if (Product == null)
                        return (false, "Không tìm thấy tiện nghi trong cơ sở dữ liệu");

                    string nextProductReceiptId = getProductReceiptId(db.ProductReceipts.ToList());
                    string nextProductReceiptDetailId = getID(getMaxProductReceiptId(db.ProductReceiptDetails.ToList()) + 1);

                    string id = "";

                    if (AdminVM.CurrentStaff != null)
                    {
                        id = AdminVM.CurrentStaff.StaffId;
                    }
                    else
                    {
                        id = StaffVM.CurrentStaff.StaffId;
                    }
                    ProductReceipt ProductReceipt = new ProductReceipt
                    {
                        ProductReceiptId = nextProductReceiptId,
                        StaffId = id,
                        Price = importPrice * importQuantity,
                        CreateAt = DateTime.Now,
                    };

                    ProductReceiptDetail ProductReceiptDetail = new ProductReceiptDetail
                    {
                        ProductReceiptDetailId = nextProductReceiptDetailId,
                        ProductReceiptId = nextProductReceiptId,
                        ProductId = ProductSelected.ProductId,
                        ImportPrice = importPrice,
                        Quantity = importQuantity,
                    };



                    db.ProductReceipts.Add(ProductReceipt);
                    db.ProductReceiptDetails.Add(ProductReceiptDetail);
                    Product.QuantityOfStorage = (int)Product.QuantityOfStorage + importQuantity;
                    await db.SaveChangesAsync();
                    return (true, "Nhập sản phẩm thành công");
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu");
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống");
            }
        }

        public async Task<(bool, string, List<ProductDTO>)> ImportListProduct(ObservableCollection<ProductDTO> orderList)
        {
            try
            {
                List<ProductDTO> listProduct = new List<ProductDTO>(orderList);
                using (HotelManagementEntities db = new HotelManagementEntities())
                {

                    string id = "";

                    if (AdminVM.CurrentStaff != null)
                    {
                        id = AdminVM.CurrentStaff.StaffId;
                    }
                    else
                    {
                        id = StaffVM.CurrentStaff.StaffId;
                    }

                    int Length = orderList.Count;
                    string nextProductReceiptId = getProductReceiptId(db.ProductReceipts.ToList());

                    ProductReceipt ProductReceipt = new ProductReceipt
                    {
                        ProductReceiptId = nextProductReceiptId,
                        StaffId = id,
                        CreateAt = DateTime.Now,
                        Price = listProduct.Sum((item) => item.ImportPrice * item.ImportQuantity)
                    };

                    db.ProductReceipts.Add(ProductReceipt);
                    int maxIdReceiptDetail = getMaxProductReceiptId(db.ProductReceiptDetails.ToList());
                    for (int i = 0; i < Length; i++)
                    {
                        ProductDTO temp = orderList[i];
                        Product Product = await db.Products.FirstOrDefaultAsync(item => item.ProductId == temp.ProductId);
                        if (Product == null)
                            return (false, "Không tìm thấy sản phẩm " + Product.ProductName + " trong cơ sở dữ liệu", null);


                        string nextProductReceiptDetailId = getID(++maxIdReceiptDetail);
                        ProductReceiptDetail ProductReceiptDetail = new ProductReceiptDetail
                        {
                            ProductReceiptDetailId = nextProductReceiptDetailId,
                            ProductReceiptId = nextProductReceiptId,
                            ImportPrice = temp.ImportPrice,
                            Quantity = temp.ImportQuantity,
                        };

                        db.ProductReceiptDetails.Add(ProductReceiptDetail);
                        listProduct[i].Quantity = (int)Product.QuantityOfStorage + temp.ImportQuantity;
                        Product.QuantityOfStorage = (int)Product.QuantityOfStorage + temp.ImportQuantity;
                    }
                    await db.SaveChangesAsync();
                    return (true, "Nhập sản phẩm thành công", listProduct);
                }
            }
            catch (EntityException e)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống", null);
            }
        }

        public List<ProductDTO> GetAllProductByType(string typeSelection, ObservableCollection<ProductDTO> allProduct)
        {
            try
            {
                return allProduct.Where(item => item.ProductType == typeSelection).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
     
        public string CreateProductID(string maxId)
        {
            if (maxId is null)
            {
                return "SP001";
            }
            string numStr = (int.Parse(maxId.Substring(2)) + 1).ToString();
            while (numStr.Length < 3) numStr = "0" + numStr;
            return "SP" + numStr;
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
        public async Task<List<ImportProductDTO>> GetListImportProduct()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<ImportProductDTO> ImportProduct = await (
                                                            from g in db.ProductReceiptDetails
                                                            join s in db.Products
                                                            on g.ProductId equals s.ProductId into gs
                                                            from s in gs.DefaultIfEmpty()
                                                            join st in db.Staffs
                                                            on g.ProductReceipt.StaffId equals st.StaffId into gst
                                                            from st in gst.DefaultIfEmpty()
                                                            orderby g.ProductReceipt.CreateAt descending
                                                            select new ImportProductDTO
                                                            {
                                                                ImportId = g.ProductReceiptId,
                                                                ProductName = s.ProductName,
                                                                ProductImportQuantity = (int)g.Quantity,
                                                                ProductImportPrice = (float)g.ImportPrice,
                                                                StaffName = st.StaffName,
                                                                CreatedDate = (DateTime)g.ProductReceipt.CreateAt,
                                                                typeimport = 0

                                                            }
                                                            ).ToListAsync();
                    return ImportProduct;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<ImportProductDTO>> GetListImportProduct(int month)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<ImportProductDTO> ImportProduct = await (
                                                            from g in db.ProductReceiptDetails
                                                            join s in db.Products
                                                            on g.ProductId equals s.ProductId into gs
                                                            from s in gs.DefaultIfEmpty()
                                                            join st in db.Staffs
                                                            on g.ProductReceipt.StaffId equals st.StaffId into gst
                                                            from st in gst.DefaultIfEmpty()
                                                            where ((DateTime)g.ProductReceipt.CreateAt).Year == DateTime.Today.Year && ((DateTime)g.ProductReceipt.CreateAt).Month == month
                                                            orderby g.ProductReceipt.CreateAt descending
                                                            select new ImportProductDTO
                                                            {
                                                                ImportId = g.ProductReceiptId,
                                                                ProductName = s.ProductName,
                                                                ProductImportQuantity = (int)g.Quantity,
                                                                ProductImportPrice = (float)g.ImportPrice,
                                                                StaffName = st.StaffName,
                                                                CreatedDate = (DateTime)g.ProductReceipt.CreateAt,
                                                                typeimport = 0
                                                            }
                                                            ).ToListAsync();
                    return ImportProduct;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int getMaxProductReceiptId(List<ProductReceiptDetail> listProductReceiptDetail)
        {
           return listProductReceiptDetail.Count();
        }
        public string getProductReceiptId(List<ProductReceipt> listProductReceipt)
        {
            int ProductReceiptID = listProductReceipt.Count();
            string mainID;
            if (ProductReceiptID == 0)
                mainID = "0001";
            else
            {
                ProductReceiptID = int.Parse(listProductReceipt.Max(item => item.ProductReceiptId));
                mainID = getID(++ProductReceiptID);
            }

            return mainID;
        }

        public string getID(int id)
        {
            if (id < 10)
                return "000" + id;
            if (id < 100)
                return "00" + id;
            if (id < 1000)
                return "0" + id;

            return id.ToString();
        }
    }

}
