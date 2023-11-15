using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Utils;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomOrder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.ViewModel.StaffVM.RoomCatalogManagementVM
{
    public partial class RoomCatalogManagementVM : BaseVM
    {
        private ProductDTO _CleaningServices;
        public ProductDTO CleaningService
        {
            get { return _CleaningServices; }
            set { _CleaningServices = value; OnPropertyChanged(); }
        }
        private ProductDTO _LaundryService;
        public ProductDTO LaundryService
        {
            get { return _LaundryService; }
            set { _LaundryService = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ProductUsingDTO> _ListService;
        public ObservableCollection<ProductUsingDTO> ListService
        {
            get { return _ListService; }
            set { _ListService = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductDTO> _ProducList;
        public ObservableCollection<ProductDTO> ProductList
        {
            get { return _ProducList; }
            set { _ProducList = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ProductDTO> _AllProducts;
        public ObservableCollection<ProductDTO> AllProducts
        {
            get { return _AllProducts; }
            set { _AllProducts = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ProductDTO> _orderList;
        public ObservableCollection<ProductDTO> OrderList
        {
            get => _orderList;
            set
            {
                _orderList = value;
                OnPropertyChanged();
            }
        }
        private double _sumOrder;
        public double SumOrder
        {
            get { return _sumOrder; }
            set { _sumOrder = value; OnPropertyChanged(); }
        }
        private ProductDTO selectedProduct;
        public ProductDTO SelectedProduct
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged(); }
        }

        private ProductDTO serviceCache;
        public ProductDTO ServiceCache
        {
            get { return serviceCache; }
            set { serviceCache = value; OnPropertyChanged(); }
        }

        private ComboBoxItem _selectedItemFilter;
        public ComboBoxItem SelectedItemFilter
        {
            get => _selectedItemFilter;
            set
            {
                _selectedItemFilter = value;
                OnPropertyChanged();
            }
        }
      
        public async Task LoadAllProduct()
        {
            (bool isSuccess, string messageReturn, List<ProductDTO> listProduct) = await Task.Run(() => ProductService.Ins.GetAllProduct());

            if (isSuccess)
            {
                AllProducts = new ObservableCollection<ProductDTO>(listProduct);
                ProductList = new ObservableCollection<ProductDTO>(listProduct);
                OrderList = new ObservableCollection<ProductDTO>();
                SumOrder = 0;
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }
        //public void LoadProductListView(Operation operation, ServiceDTO service = null)
        //{
        //    if (OrderList.Count == 0)
        //    {
        //        CustomMessageBox.ShowOk("Vui lòng chọn sản phẩm!", "Thông báo", "Ok", CustomMessageBoxImage.Warning);
        //        return;
        //    }
        //    (bool isSucceed, string message) = await ServiceUsingHelper.Ins.SaveUsingProduct(OrderList, SelectedRoom);
        //    if (isSucceed)
        //    {
        //        CustomMessageBox.ShowOk(message, "Thông báo", "Ok", CustomMessageBoxImage.Success);
        //        SumOrder = 0;
        //        OrderList = null;
        //        ListService = new ObservableCollection<ServiceUsingDTO>(await ServiceUsingHelper.Ins.GetListUsingService(SelectedRoom.RentalContractId));
        //        p.Close();
        //    }
        //    else
        //    {
        //        CustomMessageBox.ShowOk(message, "Thông báo", "Ok", CustomMessageBoxImage.Error);
        //    }
        //}
        public void LoadProductToBill()
        {
            if (ServiceCache.Quantity > 0)
            {
                try
                {
                    ServiceCache.Quantity -= 1;
                    if (!OrderList.Contains(ServiceCache))
                    {
                        ServiceCache.ImportQuantity = 1;
                        OrderList.Add(ServiceCache);
                        SumOrder += ServiceCache.ProductPrice;
                    }
                    else
                    {
                        ServiceCache.ImportQuantity += 1;
                        SumOrder += ServiceCache.ProductPrice;
                    }
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk("Bạn đã chọn hết số lượng sản phẩm này!", "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
            }
        }
        public void DecreaseProductInBill()
        {
            if (ServiceCache.ImportQuantity > 1)
            {
                try
                {
                    ServiceCache.ImportQuantity -= 1;
                    SumOrder -= ServiceCache.ProductPrice;
                    ServiceCache.Quantity += 1;
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
            }
            else
            {
                if(CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm?", "Cảnh báo", "Xóa", "Không", CustomMessageBoxImage.Warning)
                == CustomMessageBoxResult.OK)
                {
                    OrderList.Remove(ServiceCache);
                    SumOrder -= ServiceCache.ProductPrice;
                    ServiceCache.Quantity += 1;
                }
            }
        }
        public void IncreaseProductInBill()
        {
            if (ServiceCache.Quantity > 0)
            {
                try
                {
                    ServiceCache.ImportQuantity += 1;
                    SumOrder += ServiceCache.ProductPrice;
                    ServiceCache.Quantity -= 1;
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk("Bạn đã chọn hết số lượng sản phẩm này!", "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
            }
        }
        public void DeleteProductInBill()
        {
            try
            {
                if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm?", "Cảnh báo", "Xóa", "Không", CustomMessageBoxImage.Warning)
               == CustomMessageBoxResult.OK)
                {
                    ServiceCache.Quantity += ServiceCache.ImportQuantity;
                    SumOrder -= (ServiceCache.ProductPrice * ServiceCache.ImportQuantity);
                    ServiceCache.ImportQuantity = 0;
                    OrderList.Remove(ServiceCache);
                }
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
            }
        }
        public async Task AddOrderProduct(Window p)
        {
            if (OrderList.Count == 0)
            {
                CustomMessageBox.ShowOk("Vui lòng chọn sản phẩm!", "Thông báo", "Ok", CustomMessageBoxImage.Warning);
                return;
            }
            (bool isSucceed, string message) = await ServiceUsingHelper.Ins.SaveUsingProduct(OrderList, SelectedRoom);
            if (isSucceed)
            {
                CustomMessageBox.ShowOk(message, "Thông báo", "Ok", CustomMessageBoxImage.Success);
                SumOrder = 0;
                OrderList = null;
                ListService = new ObservableCollection<ProductUsingDTO>(await ServiceUsingHelper.Ins.GetListUsingService(SelectedRoom.RentalContractId));
                p.Close();
            }
            else
            {
                CustomMessageBox.ShowOk(message, "Thông báo", "Ok", CustomMessageBoxImage.Error);
            }
        }
    }
}
