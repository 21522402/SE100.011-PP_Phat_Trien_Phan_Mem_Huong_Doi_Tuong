using HotelManagement.DTOs;
using HotelManagement.Model;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.View.Staff.RoomCatalogManagement;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomInfo;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomOrder;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomPayment;
using HotelManagement.ViewModel.AdminVM;
using HotelManagement.ViewModel.StaffVM;
using IronXL.Formatting;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HotelManagement.ViewModel.StaffVM.RoomCatalogManagementVM
{
    
    public partial class RoomCatalogManagementVM : BaseVM
    {
        

        Page MainPage;
        RoomWindow RoomWindow;
       
       

       
        private List<RoomTypeDTO> _RoomTypes;
        public List<RoomTypeDTO> RoomTypes 
        { 
            get { return _RoomTypes; }
            set { _RoomTypes = value; OnPropertyChanged();}
        }
        private List<RoomDTO> _ListRooms;
        public List<RoomDTO> ListRooms
        {
            get { return _ListRooms; }
            set { _ListRooms = value; OnPropertyChanged(); }
        }
       

        private RoomDTO _SelectedRoom;
        public RoomDTO SelectedRoom
        {
            get { return _SelectedRoom; }
            set { _SelectedRoom = value; OnPropertyChanged(); }
        }

        private ListRoomTypeDTO _SelectedListRoom;
        public ListRoomTypeDTO SelectedListRoom
        {
            get { return _SelectedListRoom; }
            set { _SelectedListRoom = value; OnPropertyChanged(); }
        }
        private bool TimeChange = false;
        private bool Refresh = false;
        private bool IsLoad = false;
        private bool IsPageLoad = false;
        private bool IsViewChange = false;
        public StaffDTO CurrentStaff;
        DispatcherTimer timer = new DispatcherTimer();


        // Mới
        private ObservableCollection<string> _ListRoomType;
        public ObservableCollection<string> ListRoomType
        {
            get { return _ListRoomType; }
            set { _ListRoomType = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ListRoomTypeDTO> _ListListRoomType;
        public ObservableCollection<ListRoomTypeDTO> ListListRoomType
        {
            get { return _ListListRoomType; }
            set { _ListListRoomType = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ListRoomTypeDTO> _ListListRoomTypeMini;
        public ObservableCollection<ListRoomTypeDTO> ListListRoomTypeMini
        {
            get { return _ListListRoomTypeMini; }
            set { _ListListRoomTypeMini = value; OnPropertyChanged(); }
        }
        private RadioButton _radioButtonRoomStatus;
        public RadioButton RadioButtonRoomStatus
        {
            get { return _radioButtonRoomStatus; }
            set { _radioButtonRoomStatus = value; OnPropertyChanged(); }
        }
        private RadioButton _radioButtonRoomType;
        public RadioButton RadioButtonRoomType
        {
            get { return _radioButtonRoomType; }
            set { _radioButtonRoomType = value; OnPropertyChanged(); }
        }



        public ICommand FirstLoadCM { get; set; }


        public ICommand ChangeRoomType1CM { get; set; }
        public ICommand ChangeRoomType2CM { get; set; }


        public ICommand LoadRoomInfoCM { get; set; }
        public ICommand ChangeViewCM { get; set; }
        public ICommand HandlePreviewMouseDownRoom { get; set; }
        public ICommand OpenRoomWindowCM { get; set; }
        public ICommand CheckInRoomCM { get; set; }
        public ICommand UpdateRoomInfoCM { get; set; }
        public ICommand FirstLoadRoomWindowCM { get; set; }
        public ICommand CloseRoomWindowCM { get; set; }
        public ICommand RefreshCM { get; set; }
        public ICommand LoadRoomRentalContractInfoCM { get; set; }
        public ICommand LoadRoomCustomerInfoCM { get; set; }
        public ICommand LoadAddCustomerWindowCM { get; set; }
        public ICommand LoadRoomFurnitureInfoCM { get; set; }
        public ICommand SaveCustomerCM { get; set; }
        public ICommand LoadEditCustomerWindowCM { get; set; }
        public ICommand SaveEditCustomerCM { get; set; }
        public ICommand DeleteCustomerCM { get; set; }
        public ICommand FirstLoadRoomFurnitureInfoCM { get; set; }
        public ICommand LoadRoomOrderCleaningCM { get; set; }
        public ICommand LoadRoomOrderLaundryCM { get; set; }
        public ICommand ConfirmCleaningServiceCM { get; set; }
        public ICommand ConfirmLaundryServiceCM { get; set; }

        // Đặt sản phẩm
        public ICommand FirstLoadOrderProductPage { get; set; }
        public ICommand SelectionFilterChangeCM { get; set; }
        public ICommand SelectedProductToBillCM { get; set; }
        public ICommand DecreaseQuantityOrderItemCM { get; set; }
        public ICommand IncreaseQuantityOrderItemCM { get; set; }
        public ICommand DeleteItemInBillStackCM { get; set; }
        public ICommand CloseOrderProductWindowCM { get; set; }
        public ICommand AddOrderProductCM { get; set; }
        public ICommand LoadRoomOrderProductsCM { get; set; }

        // Thanh toán
        public ICommand PaymentCM { get; set; }
        public ICommand StoreListPaymentRoomCM { get; set; }
        public ICommand UnStoreListPaymentRoomCM { get; set; }
        public ICommand LoadRoomGroupPaymentCM { get; set; }
        public ICommand LoadRoomBillCM { get; set; }
        public ICommand FirstLoadRoomBillCM { get; set; }

        public ICommand SaveBillCM { get; set; }

        public RoomCatalogManagementVM()
        {
            Color color = new Color();
            FormatStringDate();
            if (StaffVM.CurrentStaff != null) CurrentStaff = StaffVM.CurrentStaff;
            if (AdminVM.AdminVM.CurrentStaff != null) CurrentStaff = AdminVM.AdminVM.CurrentStaff;
            StaffName = CurrentStaff.StaffName;

            FirstLoadCM = new RelayCommand<Page>((p) => { return true; }, async (p) =>
            {

                await PageSetting(p);
                timer.Interval = TimeSpan.FromMinutes(1);
                timer.Tick +=  timer_Tick;
                //RefreshCM.Execute(p);
            });
            RefreshCM = new RelayCommand<RoomCatalogManagementPage>((p) => { return true; }, async (p) =>
            {
                
            


            });
            ChangeRoomType1CM = new RelayCommand<RadioButton>((p) => { return true; }, async (p) =>
            {
                RadioButtonRoomType = p;
                ListListRoomType = new ObservableCollection<ListRoomTypeDTO>(ListListRoomTypeMini);
                ChangeView();

            });
            ChangeRoomType2CM = new RelayCommand<RadioButton>((p) => { return true; }, async (p) =>
            {

                RadioButtonRoomStatus = p;
                ListListRoomType = new ObservableCollection<ListRoomTypeDTO>(ListListRoomTypeMini);
                ChangeView();

            });



            HandlePreviewMouseDownRoom = new RelayCommand<Grid>((p) => { return true; }, async (p) =>
            {

                
                Label labelRoomName = (Label)p.FindName("labelRoomName");
                string number = labelRoomName.Content.ToString().Substring(1);
                int roomNumber = 0;

                if (Int32.TryParse(number, out roomNumber))
                {
                    SelectedRoom = await RoomService.Ins.GetSelectedRoom(roomNumber);
                    OpenRoomWindowCM.Execute(p);
                }
        
              

               
            });
            OpenRoomWindowCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {

               
                
                if (SelectedRoom == null)
                {
                    return;
                }
                try
                {
                    RoomWindow wd = new RoomWindow();

                    ListService = new ObservableCollection<ProductUsingDTO>(await ServiceUsingHelper.Ins.GetListUsingService(SelectedRoom.RentalContractId));
                    if (SelectedRoom.RoomStatus == ROOM_STATUS.READY)
                    {
                        wd.btnAddService.Visibility = Visibility.Collapsed;
                        wd.btnPayment.Visibility = Visibility.Collapsed;
                    }

                    else
                    {
                        wd.btnAddService.Visibility = Visibility.Visible;
                        wd.btnPayment.Visibility = Visibility.Visible;
                    }
                    ListCustomer = new ObservableCollection<RentalContractDetailDTO>(await BookingRoomService.Ins.GetCustomersOfRoom(SelectedRoom.RentalContractId));
                    RoomWindow = (RoomWindow)wd;
                    wd.ShowDialog();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.ShowOk("Lỗi hệ thống!", "Lỗi", "Ok", CustomMessageBoxImage.Error);
                }

            });

            //CheckInRoomCM = new RelayCommand<RoomWindow>((p) => { return true; }, async (p) =>
            //{

            //    await ChangeRoomStatusFunc(p);
            //});
          
            CloseRoomWindowCM = new RelayCommand<RoomWindow>((p) => { return true; }, async (p) =>
            {
                p.Close();


            });

            LoadRoomRentalContractInfoCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                ListCustomer = new ObservableCollection<RentalContractDetailDTO>(await BookingRoomService.Ins.GetCustomersOfRoom(SelectedRoom.RentalContractId));
                RoomRentalContractInfo wd = new RoomRentalContractInfo();
                wd.ShowDialog();
            });
            //LoadRoomCustomerInfoCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            //{
            //    ListCustomer = new ObservableCollection<RentalContractDetailDTO>(await RoomCustomerService.Ins.GetCustomersOfRoom(SelectedRoom.RentalContractId));
            //    RoomCustomerInfo wd = new RoomCustomerInfo();

            //    wd.ShowDialog();
            //});

            //LoadAddCustomerWindowCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            //{
            //    if (ListCustomer.Count == ROOM_INFO.PERSON_NUMBER)
            //    {
            //        CustomMessageBox.ShowOk("Lượng khách trong phòng đã đạt tối đa!", "Thông báo", "Ok", CustomMessageBoxImage.Error);
            //        return;
            //    }


            //    AddCusWindow wd = new AddCusWindow();
            //    wd.tbName.Text= string.Empty;
            //    wd.tbAddress.Text= string.Empty;
            //    wd.tbCCCD.Text= string.Empty;
            //    wd.ShowDialog();
            //});
            //SaveCustomerCM = new RelayCommand<AddCusWindow>((p) => { return true; }, async (p) =>
            //{
            //     await SaveCustomerFunc(p);
            //});
            //LoadEditCustomerWindowCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            //{
            //    EditCusWindow wd = new EditCusWindow();
            //    CustomerName = SelectedCustomer.CustomerName;
            //    CCCD= SelectedCustomer.CustomerId;
            //    wd.ShowDialog();
            //});
            //SaveEditCustomerCM = new RelayCommand<EditCusWindow>((p) => { return true; }, async (p) =>
            //{
            //    await SaveEditCustomerFunc(p);
            //});
            //DeleteCustomerCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            //{
            //    await DeleteCustomerFunc();
            //});
            LoadRoomFurnitureInfoCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                ListRoomFurniture = new ObservableCollection<RoomFurnituresDetailDTO>(await FurnitureRoomTypeService.Ins.GetRoomFurnituresDetail(SelectedRoom.RoomTypeId));
                ListRoomFurnitureTemp = new ObservableCollection<RoomFurnituresDetailDTO>(await FurnitureRoomTypeService.Ins.GetRoomFurnituresDetail(SelectedRoom.RoomId));


                RoomFurnitureInfo wd = new RoomFurnitureInfo();
                wd.ShowDialog();
            });





            LoadRoomOrderProductsCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                RoomOrderProducts wd = new RoomOrderProducts();
                wd.ShowDialog();

            });
            FirstLoadOrderProductPage = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                IsLoad = true;

                await LoadAllProduct();

                IsLoad = false;
            });
            SelectionFilterChangeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedItemFilter != null)
                {
                    if (SelectedItemFilter.Tag.ToString() == "All")
                        ProductList = new ObservableCollection<ProductDTO>(AllProducts);
                    else
                        ProductList = new ObservableCollection<ProductDTO>(ProductService.Ins.GetAllServiceByType(SelectedItemFilter.Content.ToString(), AllProducts));
                }
            });

            SelectedProductToBillCM = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct != null)
                {
                    ServiceCache = SelectedProduct;
                    LoadProductToBill();
                }
            });

            DecreaseQuantityOrderItemCM = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct != null)
                {
                    ServiceCache = SelectedProduct;
                    DecreaseProductInBill();
                }
            });

            IncreaseQuantityOrderItemCM = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct != null)
                {
                    ServiceCache = SelectedProduct;
                    IncreaseProductInBill();
                }
            });

            DeleteItemInBillStackCM = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedProduct != null)
                {
                    ServiceCache = SelectedProduct;
                    DeleteProductInBill();
                }
            });

            CloseOrderProductWindowCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                OrderList = null;
                p.Close();
            });

            AddOrderProductCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                IsLoad = true;

                await AddOrderProduct(p);

                IsLoad = false;
            });

            // Thanh toán
            PaymentCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                await Payment();
            });
           
        
            FirstLoadRoomBillCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                await LoadRoomBillFunc();
            });
            //LoadRoomBillCM = new RelayCommand<StackPanel>((p) => { return true; }, async (p) =>
            //{
            //    RoomBill wd = new RoomBill();
            //    BillPayment = SelectedRoomBill;


            //    TotalMoneyPayment = 0;
            //    wd.ShowDialog();
            //});
            SaveBillCM = new RelayCommand<RoomBill>((p) => { return true; }, async (p) =>
            {
                await SaveBillFunc(p);
            });
        }


     
      
        public void FormatStringDate()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }
        private async Task PageSetting(Page p)
        {
            await ReloadRoom();
            ListRoomType.Add("Tất cả các phòng");
            ChangeView();
            timer.Start();
        }
        private async Task ReloadRoom()
        {
    
            ListListRoomType = new ObservableCollection<ListRoomTypeDTO>(await RoomService.Ins.GetListListRoomType());
            ListListRoomTypeMini = new ObservableCollection<ListRoomTypeDTO>(ListListRoomType);
            ListRoomType = new ObservableCollection<string>(await RoomTypeService.Ins.GetListRoomTypName());
            
        }



        private void ChangeView()
        {
            (string roomType, string roomStatus) = GetContentRadioButton(RadioButtonRoomType, RadioButtonRoomStatus);

            if (roomType != "Tất cả các phòng")
            {
                ListListRoomType = new ObservableCollection<ListRoomTypeDTO>(ListListRoomType.Where(x => x.RoomTypeName == roomType).ToList());
            }
            if (roomStatus != "Tất cả các phòng")
            {
                ListListRoomType = new ObservableCollection<ListRoomTypeDTO>(ListListRoomType.Select(x => new ListRoomTypeDTO
                {
                    RoomTypeId = x.RoomTypeId,
                    RoomTypeName = x.RoomTypeName,
                    Price = x.Price,
                    Rooms = x.Rooms.Where(t => t.RoomStatus == roomStatus).ToList(),
                }).ToList());
            }

        }
        private Tuple<string, string> GetContentRadioButton(RadioButton radioButton1, RadioButton radioButton2)
        {
            string res1, res2;
            if (radioButton1 == null) res1 = "Tất cả các phòng";
            else
            {
                TextBlock t = (TextBlock)radioButton1.FindName("tbRoomType");
                res1 = t.Text.ToString();
            }
            if (radioButton2 == null) res2 = "Tất cả các phòng";
            else
            {
                switch (radioButton2.Name.ToString())
                {
                    case "rdbRoomStatusReady":
                        res2 = ROOM_STATUS.READY;
                        break;
                    case "rdbRoomStatusRenting":
                        res2 = ROOM_STATUS.RENTING;
                        break;
                    default:
                        res2 = "Tất cả các phòng";
                        break;
                }
            }



            var res = Tuple.Create(res1, res2);
            return res;
        }

        async void timer_Tick(object sender, EventArgs e)
        {
             await ReloadRoom();
        }


            public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
    


}
