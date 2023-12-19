using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;
using System.Runtime.Remoting.Contexts;
using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;
using System.Data;
using HotelManagement.View.Admin;
using HotelManagement.View.Staff;
using HotelManagement.ViewModel.AdminVM;
using HotelManagement.ViewModel.StaffVM;

namespace HotelManagement.Model.Services
{
    public class FurnitureService
    {
        private FurnitureService() { }
        private static FurnitureService _ins;
        public static FurnitureService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new FurnitureService();
                return _ins;
            }
            private set { _ins = value; }
        }

        public async Task<(bool, string, List<FurnitureDTO>)> GetAllFurniture()
        {
            try
            {
                List<FurnitureDTO> listFurniture = new List<FurnitureDTO>();
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    listFurniture = await (
                        from p in db.Furnitures
                        select new FurnitureDTO
                        {
                            FurnitureID = p.FurnitureId,
                            FurnitureAvatarData = p.FurnitureAvatar,
                            FurnitureType = p.FurnitureType,
                            FurnitureName = p.FurnitureName,
                            Quantity = (int)p.QuantityOfStorage,
                        }
                    ).ToListAsync();
                }
                for (int i = 0; i < listFurniture.Count; i++)
                    listFurniture[i].SetAvatar();
                return (true, "", listFurniture);
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

        public List<FurnitureDTO> GetAllFurnitureByType(string typeSelection, ObservableCollection<FurnitureDTO> allFurniture)
        {
            try
            {
                return allFurniture.Where(item => item.FurnitureType == typeSelection).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool, string)> SaveEditFurniture(FurnitureDTO furnitureSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Furniture CheckFurnitureName = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureName.Equals(furnitureSelected.FurnitureName) && item.FurnitureId != furnitureSelected.FurnitureID);
                    if (CheckFurnitureName != null)
                        return (false, "Đã có tiện nghi trong cơ sở  dữ liệu");

                    Furniture furniture = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureId == furnitureSelected.FurnitureID);
                    if (furniture == null)
                    {
                        return (false, "Không tìm thấy tiện nghi");
                    }
                    furniture.FurnitureName = furnitureSelected.FurnitureName;
                    furniture.FurnitureType = furnitureSelected.FurnitureType;
                    furniture.QuantityOfStorage = furnitureSelected.Quantity;
                    furniture.FurnitureAvatar = furnitureSelected.FurnitureAvatarData;

                    await db.SaveChangesAsync();
                    return (true, "Cập nhật thành công");
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
        public async Task<(bool, string, string)> AddFurniture(FurnitureDTO furnitureSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Furniture furniture = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureName == furnitureSelected.FurnitureName);

                    if (furniture != null)
                    {
                        return (false, "Đã có tiện nghi trong cơ sở dữ liệu", "-1");
                    }
                    int ID = db.Furnitures.ToList().Count();
                    string mainID;
                    if (ID == 0)
                        mainID = "0001";
                    else
                    {
                        ID = int.Parse(db.Furnitures.ToList().Max(item => item.FurnitureId));
                        mainID = getID(++ID);
                    }
                    Furniture fnt = new Furniture
                    {
                        FurnitureId = mainID,
                        FurnitureName = furnitureSelected.FurnitureName,
                        FurnitureType = furnitureSelected.FurnitureType,
                        QuantityOfStorage = 0,
                        FurnitureAvatar = furnitureSelected.FurnitureAvatarData,
                    };
                    db.Furnitures.Add(fnt);
                    await db.SaveChangesAsync();
                    return (true, "Thêm tiện nghi thành công", mainID);
                }
            }
            catch (EntityException ex)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", "-1");
            }
            catch (Exception e)
            {
                return (false, "Lỗi hệ thống", "-1");
            }
        }

        public async Task<(bool, string)> DeleteFurniture(FurnitureDTO furnitureSelected)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Furniture Furniture = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureId == furnitureSelected.FurnitureID);

                    if (Furniture == null)
                        return (false, "Không tìm thấy tiện nghi trong cơ sở dữ liệu");

                    FurnitureReceiptDetail prd = await db.FurnitureReceiptDetails.FirstOrDefaultAsync(item => item.FurnitureId == furnitureSelected.FurnitureID);

                    if (prd != null)
                        return (false, "Không thể xóa sản phẩm do bị ràng buộc tham chiếu của phiếu nhập tiện nghi");

                    db.Furnitures.Remove(Furniture);

                    await db.SaveChangesAsync();
                    return (true, "Xóa tiện nghi thành công");
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

        public async Task<(bool, string)> ImportFurniture(FurnitureDTO FurnitureSelected, double importPrice, int importQuantity)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    Furniture Furniture = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureId == FurnitureSelected.FurnitureID);
                    if (Furniture == null)
                        return (false, "Không tìm thấy tiện nghi trong cơ sở dữ liệu");

                    string nextFurnitureReceiptId = getFurnitureReceiptId(db.FurnitureReceipts.ToList());
                    string nextFurnitureReceiptDetailId = getID(getMaxFurnitureReceiptId(db.FurnitureReceiptDetails.ToList()) + 1);

                    string id = "";

                    if (AdminVM.CurrentStaff != null)
                    {
                        id = AdminVM.CurrentStaff.StaffId;
                    }
                    else
                    {
                        id = StaffVM.CurrentStaff.StaffId;
                    }
                    FurnitureReceipt FurnitureReceipt = new FurnitureReceipt
                    {
                        FurnitureReceiptId = nextFurnitureReceiptId,
                        StaffId = id,
                        Price = importPrice * importQuantity,
                        CreateAt = DateTime.Now,
                    };

                    FurnitureReceiptDetail FurnitureReceiptDetail = new FurnitureReceiptDetail
                    {
                        FurnitureReceiptDetailId = nextFurnitureReceiptDetailId,
                        FurnitureReceiptId = nextFurnitureReceiptId,
                        FurnitureId = FurnitureSelected.FurnitureID,
                        ImportPrice = importPrice,
                        Quantity = importQuantity,
                    };

                    db.FurnitureReceipts.Add(FurnitureReceipt);
                    db.FurnitureReceiptDetails.Add(FurnitureReceiptDetail);
                    Furniture.QuantityOfStorage = (int)Furniture.QuantityOfStorage + importQuantity;
                    await db.SaveChangesAsync();
                    return (true, "Nhập tiện nghi thành công");
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

        public async Task<(bool, string, List<FurnitureDTO>)> ImportListFurniture(ObservableCollection<FurnitureDTO> orderList)
        {
            try
            {
                List<FurnitureDTO> listFurniture = new List<FurnitureDTO>(orderList);
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
                    string nextFurnitureReceiptId = getFurnitureReceiptId(db.FurnitureReceipts.ToList());

                    FurnitureReceipt FurnitureReceipt = new FurnitureReceipt
                    {
                        FurnitureReceiptId = nextFurnitureReceiptId,
                        StaffId = id,
                        CreateAt = DateTime.Now,
                        Price = listFurniture.Sum((item) => item.ImportPrice)
                    };

                    db.FurnitureReceipts.Add(FurnitureReceipt);
                    int maxIdReceiptDetail = getMaxFurnitureReceiptId(db.FurnitureReceiptDetails.ToList());
                    for (int i = 0; i < Length; i++)
                    {
                        FurnitureDTO temp = orderList[i];
                        Furniture Furniture = await db.Furnitures.FirstOrDefaultAsync(item => item.FurnitureId == temp.FurnitureID);
                        if (Furniture == null)
                            return (false, "Không tìm thấy tiện nghi " + Furniture.FurnitureName + " trong cơ sở dữ liệu", null);


                        string nextFurnitureReceiptDetailId = getID(++maxIdReceiptDetail);
                        FurnitureReceiptDetail FurnitureReceiptDetail = new FurnitureReceiptDetail
                        {
                            FurnitureReceiptDetailId = nextFurnitureReceiptDetailId,
                            FurnitureReceiptId = nextFurnitureReceiptId,
                            ImportPrice = temp.ImportPrice,
                            Quantity = temp.ImportQuantity,
                        };

                        db.FurnitureReceiptDetails.Add(FurnitureReceiptDetail);
                        listFurniture[i].Quantity = (int)Furniture.QuantityOfStorage + temp.ImportQuantity;
                        Furniture.QuantityOfStorage = (int)Furniture.QuantityOfStorage + temp.ImportQuantity;
                    }
                    await db.SaveChangesAsync();
                    return (true, "Nhập sản phẩm thành công", listFurniture);
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
        public ImageSource LoadImage(string ImagePath)
        {
            BitmapImage _image = new BitmapImage();
            _image.BeginInit();
            _image.CacheOption = BitmapCacheOption.None;
            _image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            _image.CacheOption = BitmapCacheOption.OnLoad;
            _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _image.UriSource = new Uri(ImagePath, UriKind.RelativeOrAbsolute);
            _image.EndInit();
            return _image;
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
        public BitmapImage LoadAvatarImage(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            return bitmapImage;
        }
        public async Task<List<ImportReceiptDTO>> GetListImportFunitureReceipt()
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<ImportReceiptDTO> ImportFuniture = await (
                                                          from r in db.FurnitureReceipts
                                                          orderby r.CreateAt descending
                                                          select new ImportReceiptDTO
                                                          {
                                                              ReceiptId = r.FurnitureReceiptId,
                                                              StaffId = r.StaffId,
                                                              StaffName = r.Staff.StaffName,
                                                              TotalPrice = r.Price,
                                                              TotalQuality = r.FurnitureReceiptDetails.Sum(dt => dt.Quantity),
                                                              Details = r.FurnitureReceiptDetails.Select(dt => new ImportReceiptDetailDTO
                                                              {
                                                                  ImportReceiptDetailId = dt.FurnitureReceiptDetailId,
                                                                  ImportReceiptId = dt.FurnitureReceiptId,
                                                                  ProductId = dt.FurnitureId,
                                                                  ProductName = dt.Furniture.FurnitureName,
                                                                  ImportPrice = dt.ImportPrice,
                                                                  Quantity = dt.Quantity
                                                              }).ToList(),
                                                              CreateAt = (DateTime)r.CreateAt,
                                                              typeImport = 1
                                                          }).ToListAsync();
                    return ImportFuniture;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<ImportReceiptDTO>> GetListImportFunitureReceipt(int month)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    List<ImportReceiptDTO> ImportFuniture = await (
                                                           from r in db.FurnitureReceipts
                                                           where ((DateTime)r.CreateAt).Year == DateTime.Today.Year && ((DateTime)r.CreateAt).Month == month
                                                           orderby r.CreateAt descending
                                                           select new ImportReceiptDTO
                                                           {
                                                               ReceiptId = r.FurnitureReceiptId,
                                                               StaffId = r.StaffId,
                                                               StaffName = r.Staff.StaffName,
                                                               TotalPrice = r.Price,
                                                               TotalQuality = r.FurnitureReceiptDetails.Sum(dt => dt.Quantity),
                                                               Details = r.FurnitureReceiptDetails.Select(dt => new ImportReceiptDetailDTO
                                                               {
                                                                   ImportReceiptDetailId = dt.FurnitureReceiptDetailId,
                                                                   ImportReceiptId = dt.FurnitureReceiptId,
                                                                   ProductId = dt.FurnitureId,
                                                                   ProductName = dt.Furniture.FurnitureName,
                                                                   ImportPrice = dt.ImportPrice,
                                                                   Quantity = dt.Quantity
                                                               }).ToList(),
                                                               CreateAt = (DateTime)r.CreateAt,
                                                               typeImport = 1
                                                           }).ToListAsync();
                    return ImportFuniture;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<ImportReceiptDTO> GetImportReceiptDetail(string id)
        {
            try
            {
                using (HotelManagementEntities db = new HotelManagementEntities())
                {
                    ImportReceiptDTO ImportFuniture = await (
                                                          from r in db.FurnitureReceipts
                                                          where r.FurnitureReceiptId == id
                                                          select new ImportReceiptDTO
                                                          {
                                                              ReceiptId = r.FurnitureReceiptId,
                                                              StaffId = r.StaffId,
                                                              StaffName = r.Staff.StaffName,
                                                              TotalPrice = r.Price,
                                                              TotalQuality = r.FurnitureReceiptDetails.Sum(dt => dt.Quantity),
                                                              Details = r.FurnitureReceiptDetails.Select(dt => new ImportReceiptDetailDTO
                                                              {
                                                                  ImportReceiptDetailId = dt.FurnitureReceiptDetailId,
                                                                  ImportReceiptId = dt.FurnitureReceiptId,
                                                                  ProductId = dt.FurnitureId,
                                                                  ProductName = dt.Furniture.FurnitureName,
                                                                  ImportPrice = dt.ImportPrice,
                                                                  Quantity = dt.Quantity
                                                              }).ToList(),
                                                              CreateAt = (DateTime)r.CreateAt,
                                                              typeImport = 1
                                                          }).FirstAsync();
                    return ImportFuniture;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int getMaxFurnitureReceiptId(List<FurnitureReceiptDetail> listFurnitureReceiptDetail)
        {
            return listFurnitureReceiptDetail.Count();
        }
        public string getFurnitureReceiptId(List<FurnitureReceipt> listFurnitureReceipt)
        {
            int FurnitureReceiptID = listFurnitureReceipt.Count();
            string mainID;
            if (FurnitureReceiptID == 0)
                mainID = "0001";
            else
            {
                FurnitureReceiptID = int.Parse(listFurnitureReceipt.Max(item => item.FurnitureReceiptId));
                mainID = getID(++FurnitureReceiptID);
            }

            return mainID;
        }
    }
}