using HotelManagement.DTOs;
using HotelManagement.Model;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.BookingRoomManagement;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.ViewModel.AdminVM;
using IronXL.Formatting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModel.BookingRoomManagementVM
{
    public partial class BookingRoomManagementVM : BaseVM
    {
        private ObservableCollection<RoomDTO> _ListReadyRoom;
        public ObservableCollection<RoomDTO> ListReadyRoom
        {
            get => _ListReadyRoom;
            set
            {
                _ListReadyRoom = value;
                OnPropertyChanged();
            }
        }

        private RoomDTO _SelectedRoom;
        public RoomDTO SelectedRoom
        {
            get => _SelectedRoom;
            set
            {
                _SelectedRoom = value;
                OnPropertyChanged();
            }
        }
        private float _Price;
        public float Price
        {
            get => _Price;
            set
            {
                _Price = value;
                OnPropertyChanged();
            }
        }
        private bool _IsEditRental;
        public bool IsEditRental
        {
            get => _IsEditRental;
            set
            {
                _IsEditRental = value;
                OnPropertyChanged();
            }
        }
        private string _PriceInfo;
        public string PriceInfo
        {
            get => _PriceInfo;
            set
            {
                _PriceInfo = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<RentalContractDTO> _RentalContractList;
        public ObservableCollection<RentalContractDTO> RentalContractList
        {
            get => _RentalContractList;
            set
            {
                _RentalContractList = value;
                OnPropertyChanged();
            }
        }

        private RentalContractDTO _SelectedItem;
        public RentalContractDTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }
        private string _PriceStr;
        public string PriceStr
        {
            get => _PriceStr;
            set
            {
                _PriceStr = value;
                OnPropertyChanged();
            }
        }
        private string _RoomId;
        public string RoomId
        {
            get { return _RoomId; }
            set { _RoomId = value; OnPropertyChanged(); }
        }

        private int _roomNumber;
        public int RoomNumber
        {
            get { return _roomNumber; }
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        private DateTime _CreateDate;
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; OnPropertyChanged(); }
        }
        private string _CreateDateStr;
        public string CreateDateStr
        {
            get { return _CreateDateStr; }
            set { _CreateDateStr = value; OnPropertyChanged(); }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; OnPropertyChanged(); }
        }
        private string _EndDateStr;
        public string EndDateStr
        {
            get { return _EndDateStr; }
            set { _EndDateStr = value; OnPropertyChanged(); }
        }
        private string _RentalContractDetailId;
        public string RentalContractDetailId
        {
            get { return _RentalContractDetailId; }
            set { _RentalContractDetailId = value; OnPropertyChanged(); }
        }
        private string _RentalContractId;
        public string RentalContractId
        {
            get { return _RentalContractId; }
            set { _RentalContractId = value; OnPropertyChanged(); }
        }
        private ObservableCollection<RentalContractDetailDTO> _ListCustomer;
        public ObservableCollection<RentalContractDetailDTO> ListCustomer
        {
            get => _ListCustomer;
            set
            {
                _ListCustomer = value;
                OnPropertyChanged();
            }
        }

        private RentalContractDetailDTO _SelectedCustomer;
        public RentalContractDetailDTO SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                _SelectedCustomer = value;
                OnPropertyChanged();
            }
        }
        private bool IsSave = false;
       
        private string _CustomerName;
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; OnPropertyChanged(); }
        }
        
        private string _CCCD;
        public string CCCD
        {
            get { return _CCCD; }
            set { _CCCD = value; OnPropertyChanged(); }
        }
        public string cccd_Last = "";

        private bool isSaving;
        public bool IsSaving
        {
            get { return isSaving; }
            set { isSaving = value; OnPropertyChanged(); }
        }

        private DateTime _RentalDay;
        public DateTime RentalDay
        {
            get { return _RentalDay; }
            set { _RentalDay = value; OnPropertyChanged(); }
        }

        private List<string> _ListFilterYear;
        public List<string> ListFilterYear
        {
            get { return _ListFilterYear; }
            set { _ListFilterYear = value; OnPropertyChanged(); }
        }
        private string _SelectedYear;
        public string SelectedYear
        {
            get { return _SelectedYear; }
            set { _SelectedYear = value; OnPropertyChanged(); }
        }
        private List<string> _ListFilterMonth;
        public List<string> ListFilterMonth
        {
            get { return _ListFilterMonth; }
            set { _ListFilterMonth = value; OnPropertyChanged(); }
        }
        private string _SelectedMonth;
        public string SelectedMonth
        {
            get { return _SelectedMonth; }
            set { _SelectedMonth = value; OnPropertyChanged(); }
        }
        public ICommand CloseCM { get; set; }
        public ICommand CloseCM2 { get; set; }
        public ICommand FirstLoadCM { get; set; }
        public ICommand LoadBookingCM { get; set; }
        public ICommand LoadFormInfoCustomerCM { get; set; }
        public ICommand LoadAddCustomerCM { get; set; }
        public ICommand ConfirmCustomerCM { get; set; }
        public ICommand LoadEditCustomerCM { get; set; }
        public ICommand LoadDeleteCustomerCM { get; set; }
        public ICommand ConfirmEditCustomerCM { get; set; }
        public ICommand LoadRentalContractInfoCM { get; set; }
        public ICommand ConfirmSaveRentalContract { get; set; }
        public ICommand LoadDeleteRentalContractCM { get; set; }
        public ICommand UpdateListCustomerCM { get; set; }
        public ICommand ChangeTimeCM { get; set; }
        public ICommand UpdateRentalPriceCM { get; set; }
        public ICommand SelectedTimeChangedCM { get; set; }
        

        public BookingRoomManagementVM()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            ListFilterYear = new List<string>(BookingRoomService.Ins.GetListFilterYear());
            SelectedYear = ListFilterYear[0];
            ListFilterMonth = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                ListFilterMonth.Add("Tháng " + i.ToString());
            }
            ListFilterMonth.Insert(0, "Tất cả");
            SelectedMonth = "Tất cả";

            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                SelectedRoom = null;
                PriceStr = "";
                await LoadReadyRoom();
                RentalContractList = new ObservableCollection<RentalContractDTO>(await BookingRoomService.Ins.GetRentalContractList());
            });
            ChangeTimeCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                await ChangeView();
            });
            LoadBookingCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ListCustomer = new ObservableCollection<RentalContractDetailDTO>();
                Booking booking = new Booking();
                CreateDate = DateTime.Now;
                CreateDateStr = CreateDate.ToString("dd/MM/yyyy");
                EndDate = CreateDate.AddDays(1);
                EndDateStr = EndDate.ToString("dd/MM/yyyy");
                booking.ShowDialog();
            });
            SelectedTimeChangedCM = new RelayCommand<object>((p) => { return true; },  (p) =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.EndDate <= SelectedItem.CreateDate)
                    {
                        CustomMessageBox.ShowOk("Ngày kết thúc phải lớn hơn ngày bắt đầu thuê!", "Thông Báo", "OK", CustomMessageBoxImage.Warning);
                        SelectedItem.EndDate = EndDate;
                        return;
                    }
                }
                if (EndDate<=CreateDate)
                {
                    CustomMessageBox.ShowOk("Ngày kết thúc phải lớn hơn ngày bắt đầu thuê!", "Thông Báo", "OK", CustomMessageBoxImage.Warning);
                    EndDate = CreateDate.AddDays(1);
                    EndDateStr = EndDate.ToString("dd/MM/yyyy");                  
                    return;
                }
            });
            LoadFormInfoCustomerCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedRoom == null)
                {
                    CustomMessageBox.ShowOk("Vui lòng chọn phòng trước!", "Thông Báo", "OK", CustomMessageBoxImage.Warning);
                    return;
                }
                else
                {
                    int maxPer = BookingRoomService.Ins.GetMaxNumOfPer(SelectedRoom.RoomId);
                    if (ListCustomer.Count == maxPer)
                    {
                        CustomMessageBox.ShowOk($"Mỗi phòng chỉ được ở tối đa {maxPer} khách!", "Thông Báo", "OK", CustomMessageBoxImage.Warning);
                        return;
                    }

                    RenewWindowDataCusTomer();
                    EnterInfoCustomer w = new EnterInfoCustomer();
                    w.ShowDialog();
                }
            });
            LoadAddCustomerCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                int maxPer = BookingRoomService.Ins.GetMaxNumOfPer(SelectedItem.RoomId);
                if (ListCustomer.Count == maxPer)
                {
                    CustomMessageBox.ShowOk($"Mỗi phòng chỉ được ở tối đa {maxPer} khách!", "Thông Báo", "OK", CustomMessageBoxImage.Warning);
                    return;
                }
                RenewWindowDataCusTomer();
                EnterInfoCustomer w = new EnterInfoCustomer();
                w.ShowDialog();
            });
            ConfirmCustomerCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSaving) return false; return true; }, async (p) =>
            {
                IsSaving = true;
                await SaveCustomerFunc(p);
                IsSaving = false;
            });
            ConfirmEditCustomerCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, async (p) =>
            {
                IsSave = true;
                await EditCustomerFunc(p);
            });
            LoadDeleteCustomerCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, async (p) =>
            {
                string message = "Bạn có chắc muốn xoá khách hàng này không?";
                CustomMessageBoxResult kq = CustomMessageBox.ShowOkCancel(message, "Cảnh báo", "Xác nhận", "Hủy", CustomMessageBoxImage.Warning);

                if (kq == CustomMessageBoxResult.OK)
                {
                    ListCustomer.Remove(SelectedCustomer);
                    await UpdateRentalPrice();
                }
            });
            LoadEditCustomerCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                cccd_Last = SelectedCustomer.CCCD.ToString();
                editEnterInfoCustomer w = new editEnterInfoCustomer();
                CustomerName = SelectedCustomer.CustomerName;
                CCCD = SelectedCustomer.CCCD;
                w.ShowDialog();
            });
            LoadDeleteRentalContractCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                string status = await BookingRoomService.Ins.GetCurrentRoomStatus(SelectedItem.RentalContractId);
                if (status == ROOM_STATUS.RENTING)
                {
                    CustomMessageBox.ShowOk("Phiếu thuê cho phòng đang thuê, không thể xóa!", "Thông báo", "Xác nhận", CustomMessageBoxImage.Warning);
                    return;
                }
                string message = "Bạn có chắc muốn xoá phiếu thuê này không? Dữ liệu không thể phục hồi sau khi xoá!";
                CustomMessageBoxResult kq = CustomMessageBox.ShowOkCancel(message, "Cảnh báo", "Xác nhận", "Hủy", CustomMessageBoxImage.Warning);

                if (kq == CustomMessageBoxResult.OK)
                {
                    (bool successDelete, string messageFromDelete) = await BookingRoomService.Ins.DeleteRentalContract(SelectedItem.RentalContractId);

                    if (successDelete)
                    {
                        RentalContractList = new ObservableCollection<RentalContractDTO>(await BookingRoomService.Ins.GetRentalContractList());
                        SelectedItem = null;
                        CustomMessageBox.ShowOk(messageFromDelete, "Thông báo", "OK", CustomMessageBoxImage.Success);
                    }
                    else
                    {
                        CustomMessageBox.ShowOk(messageFromDelete, "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                }
            });
            LoadRentalContractInfoCM = new RelayCommand<RentalContractInfo>((p) => { return true; }, async (p) =>
            {
                RentalDay = DateTime.Today;
                RentalContractInfo w = new RentalContractInfo();
                await LoadInforRentalContract(w);
                IsEditRental = true;
                EndDate = SelectedItem.EndDate;
                w.ShowDialog();
            });
            ConfirmSaveRentalContract = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                await SaveRentalContract(p);
            });

            CloseCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });
            CloseCM2 = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                IsEditRental = false;
                p.Close();
            });
            UpdateRentalPriceCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                await UpdateRentalPrice();

            });
            UpdateListCustomerCM = new RelayCommand<Window>((p) => { return true; }, async (p) =>
            {
                if (ListCustomer.Count == 0)
                {
                    CustomMessageBox.ShowOk("Danh sách khách hàng không được trống!", "Thông báo", "OK", CustomMessageBoxImage.Warning);
                    return;
                }
                (bool isSucsses, string message) = await BookingRoomService.Ins.UpdateListCustomer(SelectedItem, new List<RentalContractDetailDTO>(ListCustomer));
                if (isSucsses)
                {
                    CustomMessageBox.ShowOk(message, "Thông báo", "Ok", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                    RentalContractList = new ObservableCollection<RentalContractDTO>(await BookingRoomService.Ins.GetRentalContractList());
                    p.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(message, "Lỗi", "OK", CustomMessageBoxImage.Error);
                }

            });
        }
        public async Task LoadInforRentalContract(RentalContractInfo w)
        {
            w.roomNumberInfo.Text = SelectedItem.RoomNumber.ToString();
            w.createDateInfo.Text = ((DateTime)SelectedItem.CreateDate).ToString("dd/MM/yyyy");
            float RentalContractPrice = await BookingRoomService.Ins.GetRentalContractPrice(SelectedItem.RentalContractId);
            PriceInfo = Helper.FormatVNMoney2(RentalContractPrice);
            ListCustomer = new ObservableCollection<RentalContractDetailDTO>(SelectedItem.ListRentalContractDetails);
        }
        private async Task ChangeView()
        {
            RentalContractList = new ObservableCollection<RentalContractDTO>(await BookingRoomService.Ins.GetRentalContractListFilter(SelectedYear, SelectedMonth));
        }
        public void RenewWindowDataCusTomer()
        {
            CustomerName = null;
            CCCD = null;
        }
        public bool IsValidDataCustomer()
        {
            return (!string.IsNullOrEmpty(CustomerName) &&
                !string.IsNullOrEmpty(CCCD));
        }
        public async Task UpdateRentalPrice()
        {
            if (IsEditRental)
            {

                Price = await BookingRoomService.Ins.GetPriceBooking(SelectedItem.RoomId, new List<RentalContractDetailDTO>(ListCustomer));
                PriceStr = Helper.FormatVNMoney2(Price);
                PriceInfo = PriceStr;
            }
            else
            {
                if (SelectedRoom == null) return;
                Price = await BookingRoomService.Ins.GetPriceBooking(SelectedRoom.RoomId, new List<RentalContractDetailDTO>(ListCustomer));
                PriceStr = Helper.FormatVNMoney2(Price);
                PriceInfo = PriceStr;
            }

        }
    }
}
