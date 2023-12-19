using HotelManagement.DTOs;
using HotelManagement.View.Admin;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.Utils;
using HotelManagement.View.Admin.ProductManagement;
using HotelManagement.Model.Services;
using System.Security.Cryptography.X509Certificates;
using System;
using HotelManagement.Utilities;

namespace HotelManagement.ViewModel.AdminVM.ProductManagementVM
{
    public partial class ProductManagementVM : BaseVM
    {
        private bool isLoadding;
        public bool IsLoadding
        {
            get { return isLoadding; }
            set { isLoadding = value; OnPropertyChanged(); }
        }private bool isLoaddingIP;
        public bool IsLoaddingIP
        {
            get { return isLoaddingIP; }
            set { isLoaddingIP = value; OnPropertyChanged(); }
        }
        private string _staffname;
        public string StaffName
        {
            get { return _staffname; }
            set { _staffname = value; OnPropertyChanged(); }
        }

        public DateTime CreateDate { get; set; }
        public string CreateDateString { get; set; }

        private ProductDTO selectedProduct;
        public ProductDTO SelectedProduct
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged(); }
        }

        private ProductDTO productCache;
        public ProductDTO ProductCache
        {
            get { return productCache; }
            set { productCache = value; OnPropertyChanged(); }
        }


        private ObservableCollection<ProductDTO> productList;
        public ObservableCollection<ProductDTO> ProductList
        {
            get { return productList; }
            set { productList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductDTO> allProducts;
        public ObservableCollection<ProductDTO> AllProducts
        {
            get { return allProducts; }
            set { allProducts = value; OnPropertyChanged(); }
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
        public ICommand FirstLoadCM { get; set; }
        public ICommand SelectionFilterChangeCM { get; set; }
        public ICommand LoadChooseImageWindowCM { get; set; }

        //Sửa 
        public ICommand OpenEditWindowCM { get; set; }
        public ICommand CloseEditWindowCM { get; set; }
        public ICommand SaveEditProductCM { get; set; }

        //Thêm
        public ICommand OpenAddFoodCM { get; set; }


        public ICommand AddProductCM { get; set; }
        public ICommand CloseAddWindowCM { get; set; }

        //Xóa
        public ICommand DeleteProductCM { get; set; }

        //Nhập
        public ICommand OpenImportFoodCM { get; set; }
        public ICommand ImportProductCM { get; set; }
        public ICommand CloseImportWindowCM { get; set; }

        // Nhập danh sách

        public ICommand OpenImportListProductCM { get; set; }
        public ICommand FirstLoadImportListProductWindowCM { get; set; }
        public ICommand CloseImportListWindowCM { get; set; }
        public ICommand ChooseProductToListCM { get; set; }
        public ICommand DecreaseQuantityOrderItem { get; set; }
        public ICommand IncreaseQuantityOrderItem { get; set; }
        public ICommand OpenReviewImportListProductCM { get; set; }
        public ICommand DeleteItemInBillStackCM { get; set; }
        public ICommand ClosePreviewImportListCM { get; set; }
        public ICommand ImportListProductCM { get; set; }
        public ICommand ImportPriceChangeCM { get; set; }
        public ICommand ImportQuantityChangeCM { get; set; }

        public ProductManagementVM()
        {
            AdminWindow tk = System.Windows.Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();
            if (AdminVM.CurrentStaff != null)
                StaffName = AdminVM.CurrentStaff.StaffName;
            else
                StaffName = "CurrentAdmin";
            filterSource = new List<string>();
            filterSource.Add("Đồ ăn");
            filterSource.Add("Nước uống");

            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                IsLoadding = true;
                (bool isSuccess, string messageReturn, List<ProductDTO> listProduct) = await Task.Run(() => ProductService.Ins.GetAllProduct());
                IsLoadding = false;

                if (isSuccess)
                {
                    AllProducts = new ObservableCollection<ProductDTO>(listProduct);
                    ProductList = new ObservableCollection<ProductDTO>(listProduct);
                }
                else
                {
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            });

            ImportPriceChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CalculateTotalPrice();
            });

            ImportQuantityChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CalculateTotalPrice();
            });

            SelectionFilterChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedItemFilter != null)
                {
                    if (SelectedItemFilter.Tag.ToString() == "All")
                        ProductList = new ObservableCollection<ProductDTO>(AllProducts);
                    else
                        ProductList = new ObservableCollection<ProductDTO>(ProductService.Ins.GetAllProductByType(SelectedItemFilter.Content.ToString(), AllProducts));
                }
            });

            OpenEditWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct == null)
                    return;
                ProductCache = new ProductDTO(SelectedProduct);
                SalePrice = ProductCache.ProductPrice.ToString();
                EditProductWindow editProductWindow = new EditProductWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                editProductWindow.ShowDialog();
            });

            LoadChooseImageWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.Title = "Select an image";
                openfile.Filter = "Image File (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg; *.png";
                if (openfile.ShowDialog() == true)
                {
                    ProductCache.SetAvatar(openfile.FileName);
                    return;
                }
            });

            SaveEditProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoadding = true;
                await SaveEditProduct(ProductCache, p, tk);
                IsLoadding = false;
            });

            CloseEditWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                ProductCache = null;
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                SalePrice = null;
            });

            DeleteProductCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                if (SelectedProduct == null)
                    return;
                ProductCache = SelectedProduct;

                if (CustomMessageBox.ShowOkCancel("Bạn có chắc chắn muốn xóa tiện nghi này không?", "Cảnh báo", "Có", "Hủy", CustomMessageBoxImage.Warning)
                    == CustomMessageBoxResult.OK)
                {
                    (bool isSuccess, string messageReturn) = await Task.Run(() => ProductService.Ins.DeleteProduct(ProductCache));

                    if (isSuccess)
                    {
                        CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", CustomMessageBoxImage.Success);
                        LoadProductListView(Operation.DELETE, ProductCache);
                        CalculateTotalPrice();
                    }
                    else
                        CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", CustomMessageBoxImage.Error);
                }

            });

            OpenAddFoodCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ProductCache = new ProductDTO();
                AddProductWindow addProductWD = new AddProductWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                addProductWD.ShowDialog();
            });

            AddProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoadding = true;
                await AddProduct(ProductCache, p, tk);
                IsLoadding = false;
            });

            CloseAddWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                ProductCache = null;
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                p.Close();
                SalePrice = null;
            });

            OpenImportFoodCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct == null)
                    return;

                ProductCache = new ProductDTO(SelectedProduct);
                ImportProductWindow importProductWindow = new ImportProductWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;
                importProductWindow.ShowDialog();
            });

            ImportProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoadding = true;
                await ImportProduct(ProductCache, p, tk);
                IsLoadding = false;
            });

            CloseImportWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
                ProductCache = null;
                ImportPrice = null;
                ImportQuantity = null;
            });


            OpenImportListProductCM = new RelayCommand<object>((p) => { return true; }, (p) => {
                ImportListProductWindow importListProductWindow = new ImportListProductWindow();
                tk.MaskOverSideBar.Visibility = Visibility.Visible;

                TotalImportPrice = 0;
                InitQuantity();
                OrderProductList = new ObservableCollection<ProductDTO>();

                importListProductWindow.ShowDialog();
            });

            FirstLoadImportListProductWindowCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
               
            });

            CloseImportListWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                tk.MaskOverSideBar.Visibility = Visibility.Collapsed;
            });

            ChooseProductToListCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct == null) return;
                
                SelectedProduct.ImportQuantity++;
                SelectedProduct.RemainQuantity--;
                if(!OrderProductList.Contains(SelectedProduct))
                    OrderProductList.Add(SelectedProduct);
                TotalImportPrice += SelectedProduct.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            DecreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ProductDTO pd = SelectedProduct;
                if(pd.ImportQuantity == 1)
                {
                    if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm khỏi danh sách", "Thông báo", "Oke", "Hủy", CustomMessageBoxImage.Warning) == CustomMessageBoxResult.Cancel) return;
                    pd.ImportQuantity = 0;
                    pd.ImportPrice = 0;

                    OrderProductList.Remove(pd);
                }
                else
                {
                    pd.ImportQuantity--;
                }

                TotalImportPrice -= pd.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            IncreaseQuantityOrderItem = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectedProduct.ImportQuantity++;
                TotalImportPrice += SelectedProduct.ImportPrice;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            DeleteItemInBillStackCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct == null) return;
                ProductDTO pd = SelectedProduct;
                if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa sản phẩm khỏi danh sách", "Thông báo", "Oke", "Hủy", CustomMessageBoxImage.Warning) == CustomMessageBoxResult.Cancel) return;
                OrderProductList.Remove(pd);
                TotalImportPrice -= (pd.ImportPrice * pd.ImportQuantity);
                pd.ImportQuantity = 0;
                pd.ImportPrice = 0;
                TotalImportPriceStr = Helper.FormatVNMoney(TotalImportPrice);
            });

            OpenReviewImportListProductCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CreateDate = DateTime.Now;
                CreateDateString = DateTimeToString(CreateDate);
                ImportListProductWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListProductWindow>().FirstOrDefault();
                ipWD.Mask.Visibility = Visibility.Visible;
                CalculateTotalPrice();
                PreviewImportListProduct previewImportListProduct = new PreviewImportListProduct();
                previewImportListProduct.ShowDialog();
            });

            ClosePreviewImportListCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
                ImportListProductWindow ipWD = System.Windows.Application.Current.Windows.OfType<ImportListProductWindow>().FirstOrDefault();
                ipWD.Mask.Visibility = Visibility.Collapsed;
            });
            ImportListProductCM = new RelayCommand<Window>((p) => { return true; },async (p) =>
            {
                IsLoaddingIP = true;
                await ImportListProduct(p, tk);
                IsLoaddingIP = false;

            });
        }
        public string DateTimeToString(DateTime dt)
        {
            string date;
            date = dt.Day + "/" + dt.Month + "/" + dt.Year + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
            return date;
        }
        public void LoadProductListView(Operation operation, ProductDTO Product = null)
        {
            switch (operation)
            {
                case Operation.UPDATE:
                    ProductDTO ProductDTO = ProductList.FirstOrDefault(item => item.ProductId == Product.ProductId);
                    if (ProductDTO == null)
                        return;
                    ProductList[ProductList.IndexOf(ProductDTO)] = Product;
                    AllProducts[AllProducts.IndexOf(ProductDTO)] = Product;
                    break;
                case Operation.CREATE:
                    ProductDTO ProductCreate = new ProductDTO(Product);
                    if (ProductCreate == null)
                        return;
                    ProductList.Add(ProductCreate);
                    AllProducts.Add(ProductCreate);
                    break;
                case Operation.DELETE:
                    ProductDTO ProductDelete = ProductList.FirstOrDefault(item => item.ProductId == Product.ProductId);
                    if (ProductDelete == null)
                        return;
                    ProductList.Remove(ProductDelete);
                    AllProducts.Remove(ProductDelete);
                    break;
                case Operation.UPDATE_PROD_QUANTITY:
                    ProductDTO ProductUpdateQuantity = ProductList.FirstOrDefault(item => item.ProductId == Product.ProductId);
                    if (ProductUpdateQuantity == null)
                        return;
                    ProductList[ProductList.IndexOf(ProductUpdateQuantity)] = Product;
                    AllProducts[AllProducts.IndexOf(ProductUpdateQuantity)] = Product;
                    break;
            }
        }
    }
}