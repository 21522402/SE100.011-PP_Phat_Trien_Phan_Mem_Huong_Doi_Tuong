using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.View.Admin;
using HotelManagement.View.Admin.ServiceManagement;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.View.Admin.ServiceManagement.OtherServices;
using HotelManagement.Utilities;
using HotelManagement.Utils;

namespace HotelManagement.ViewModel.AdminVM.ServiceManagementVM
{
    public partial class ServiceManagementVM : BaseVM
    {
        //private bool isLoadding;
        //public bool IsLoadding
        //{
        //    get { return isLoadding; }
        //    set { isLoadding = value; OnPropertyChanged(); }
        //}

        //private ProductDTO selectedProduct;
        //public ProductDTO SelectedProduct
        //{
        //    get { return selectedProduct; }
        //    set { selectedProduct = value; OnPropertyChanged(); }
        //}

        //private ProductDTO serviceCache;
        //public ProductDTO ServiceCache
        //{
        //    get { return serviceCache; }
        //    set { serviceCache = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<ProductDTO> cleaningService;
        //public ObservableCollection<ProductDTO> CleaningService
        //{
        //    get { return cleaningService; }
        //    set { cleaningService = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<ProductDTO> productList;
        //public ObservableCollection<ProductDTO> ProductList
        //{
        //    get { return productList; }
        //    set { productList = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<ProductDTO> allProducts;
        //public ObservableCollection<ProductDTO> AllProducts
        //{
        //    get { return allProducts; }
        //    set { allProducts = value; OnPropertyChanged(); }
        //}

        //private ComboBoxItem _selectedItemFilter;
        //public ComboBoxItem SelectedItemFilter
        //{
        //    get => _selectedItemFilter;
        //    set
        //    {
        //        _selectedItemFilter = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public ICommand FirstLoadCM { get; set; }
        //public ICommand SelectionFilterChangeCM { get; set; }
        //public ICommand LoadChooseImageWindowCM { get; set; }

        ////Sửa 
        //public ICommand OpenEditWindowCM { get; set; }
        //public ICommand CloseEditWindowCM { get; set; }
        //public ICommand SaveEditProductCM { get; set; }

        ////Thêm
        //public ICommand OpenAddFoodCM { get; set; }


        //public ICommand AddProductCM { get; set; }
        //public ICommand CloseAddWindowCM { get; set; }

        ////Xóa
        //public ICommand DeleteProductCM { get; set; }

        ////Nhập
        //public ICommand OpenImportFoodCM { get; set; }
        //public ICommand ImportProductCM { get; set; }
        //public ICommand CloseImportWindowCM { get; set; }

        //// Dịch vụ khác

        //public ICommand OpenOtherServiceCM { get; set; }
        //public ICommand CloseOtherServiceWindowCM { get; set; }
        //public ICommand OpenEditCleanServiceWindowCM { get; set; }
        //public ICommand SaveEditOtherServiceCM { get; set; }

        //public ServiceManagementVM()
        //{
        //    AdminWindow tk = System.Windows.Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();
        //    filterSource = new List<string>();
        //    filterSource.Add("Đồ ăn");
        //    filterSource.Add("Nước uống");

        //    FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
        //    {
        //        IsLoadding = true;
        //        (bool isSuccess, string messageReturn, List<ProductDTO> listProduct, List<ProductDTO> cleanService) = await Task.Run(() => ServiceHelper.Ins.GetAllService());
        //        IsLoadding = false;

        //        if(isSuccess)
        //        {
        //            if(cleanService.Count() != 0)
        //            {
        //                CleaningService = new ObservableCollection<ProductDTO>(cleanService);
        //                for (int i = 0; i < cleanService.Count(); i++)
        //                    listProduct.Remove(cleanService[i]);
        //            }    

        //            AllProducts = new ObservableCollection<ProductDTO>(listProduct);
        //            ProductList = new ObservableCollection<ProductDTO>(listProduct);
        //        }   
        //        else
        //        {
        //            CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
        //        }    
        //    });

        //    SelectionFilterChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        if (SelectedItemFilter != null)
        //        {
        //            if (SelectedItemFilter.Tag.ToString() == "All")
        //                ProductList = new ObservableCollection<ProductDTO>(AllProducts);
        //            else
        //                ProductList = new ObservableCollection<ProductDTO>(ServiceHelper.Ins.GetAllServiceByType(SelectedItemFilter.Content.ToString(), AllProducts));
        //        }
        //    });

        //    OpenEditWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        if (SelectedProduct == null)
        //            return;
        //        ServiceCache = new ProductDTO(SelectedProduct);
        //        SalePrice = ServiceCache.ServicePrice.ToString();
        //        EditProductWindow editProductWindow = new EditProductWindow();
        //        tk.MaskOverSideBar.Visibility = Visibility.Visible;
        //        editProductWindow.ShowDialog();
        //    });

        //    LoadChooseImageWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        OpenFileDialog openfile = new OpenFileDialog();
        //        openfile.Title = "Select an image";
        //        openfile.Filter = "Image File (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg; *.png";
        //        if (openfile.ShowDialog() == true)
        //        {
        //            ServiceCache.SetAvatar(openfile.FileName);
        //            return;
        //        }
        //    });

        //    SaveEditProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
        //    {
        //        IsLoadding = true;
        //        await SaveEditProduct(ServiceCache, p, tk);
        //        IsLoadding = false;
        //    });

        //    CloseEditWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
        //    {
        //        ServiceCache = null;
        //        p.Close();
        //        tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
        //        SalePrice = null;
        //    });

        //    DeleteProductCM = new RelayCommand<object>((p) => { return true; },async (p) =>
        //    {
        //        if (SelectedProduct == null)
        //            return;
        //        ServiceCache = SelectedProduct;

        //        if (CustomMessageBox.ShowOkCancel("Bạn có chắc chắn muốn xóa tiện nghi này không?", "Cảnh báo", "Có", "Hủy", CustomMessageBoxImage.Warning)
        //            == CustomMessageBoxResult.OK)
        //        {
        //            (bool isSuccess, string messageReturn) = await Task.Run(() => ServiceHelper.Ins.DeleteService(ServiceCache));

        //            if (isSuccess)
        //            {
        //                CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", CustomMessageBoxImage.Success);
        //                LoadProductListView(Operation.DELETE, ServiceCache);
        //            }
        //            else
        //                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", CustomMessageBoxImage.Error);
        //        }    

        //    });

        //    OpenAddFoodCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        ServiceCache = new ProductDTO();
        //        AddProductWindow addProductWD = new AddProductWindow();
        //        tk.MaskOverSideBar.Visibility = Visibility.Visible;
        //        addProductWD.ShowDialog();
        //    });

        //    AddProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
        //    {
        //        IsLoadding = true;
        //        await AddProduct(ServiceCache, p, tk);
        //        IsLoadding = false;
        //    });

        //    CloseAddWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
        //    {
        //        ServiceCache = null;
        //        tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
        //        p.Close();
        //        SalePrice = null;
        //    });

        //    OpenImportFoodCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        if (SelectedProduct == null)
        //            return;

        //        ServiceCache = new ProductDTO(SelectedProduct);
        //        ImportProductWindow importProductWindow = new ImportProductWindow();
        //        tk.MaskOverSideBar.Visibility = Visibility.Visible;
        //        importProductWindow.ShowDialog();
        //    });

        //    ImportProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
        //    {
        //        IsLoadding = true;
        //        await ImportService(ServiceCache, p, tk);
        //        IsLoadding = false;
        //    });

        //    CloseImportWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
        //    {
        //        p.Close();
        //        tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
        //        ServiceCache = null;
        //        ImportPrice = null;
        //        ImportQuantity = null;
        //    });

        //    OpenOtherServiceCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        foreach(var item in CleaningService)
        //            item.FormatStringUnitAndPrice();
        //        OtherServicesWindow otherServicesWindow = new OtherServicesWindow();
        //        tk.MaskOverSideBar.Visibility = Visibility.Visible;
        //        otherServicesWindow.ShowDialog();
        //    });


        //    CloseOtherServiceWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
        //    {
        //        p.Close();
        //        tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
        //    });

        //    OpenEditCleanServiceWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
        //    {
        //        if (SelectedProduct == null)
        //            return;

        //        ServiceCache = new ProductDTO(SelectedProduct);
        //        SalePriceService = ServiceCache.ServicePrice.ToString();
        //        EditCleanServiceWindow editCleanServiceWindow = new EditCleanServiceWindow();
        //        editCleanServiceWindow.ShowDialog();
        //    });

        //    SaveEditOtherServiceCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
        //    {
        //        IsLoadding = true;
        //        await SaveEditCleanService(ServiceCache, p);
        //        IsLoadding = false;
        //    });
        //}

        //public void LoadProductListView(Operation operation, ProductDTO service = null)
        //{
        //    switch (operation)
        //    {
        //        case Operation.UPDATE:
        //            ProductDTO serviceDTO = ProductList.FirstOrDefault(item => item.ServiceId == service.ServiceId);
        //            if (serviceDTO == null)
        //                return;
        //            ProductList[ProductList.IndexOf(serviceDTO)] = service;
        //            AllProducts[AllProducts.IndexOf(serviceDTO)] = service;
        //            break;
        //        case Operation.CREATE:
        //            ProductDTO serviceCreate = new ProductDTO(service);
        //            if (serviceCreate == null)
        //                return;
        //            ProductList.Add(serviceCreate);
        //            AllProducts.Add(serviceCreate);
        //            break;
        //        case Operation.DELETE:
        //            ProductDTO serviceDelete = ProductList.FirstOrDefault(item => item.ServiceId == service.ServiceId);
        //            if (serviceDelete == null)
        //                return;
        //            ProductList.Remove(serviceDelete);
        //            AllProducts.Remove(serviceDelete);
        //            break;
        //        case Operation.UPDATE_PROD_QUANTITY:
        //            ProductDTO serviceUpdateQuantity = ProductList.FirstOrDefault(item => item.ServiceId == service.ServiceId);
        //            if (serviceUpdateQuantity == null)
        //                return;
        //            ProductList[ProductList.IndexOf(serviceUpdateQuantity)] = service;
        //            AllProducts[AllProducts.IndexOf(serviceUpdateQuantity)] = service;
        //            break;
        //        case Operation.UPDATECLEAN:
        //            ProductDTO cleanServiceUpdateQuantity = CleaningService.FirstOrDefault(item => item.ServiceId == service.ServiceId);
        //            if (cleanServiceUpdateQuantity == null)
        //                return;
        //            CleaningService[CleaningService.IndexOf(cleanServiceUpdateQuantity)] = service;
        //            break;
        //    }
        //}
    }
}