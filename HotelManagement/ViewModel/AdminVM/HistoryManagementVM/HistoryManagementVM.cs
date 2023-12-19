using System.Windows.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using HotelManagement.DTOs;
using System.Windows.Controls;
using HotelManagement.View.Admin.HistoryManagement;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.Model.Services;
using System.Windows.Forms;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Windows;
using HotelManagement.Utilities;
using HotelManagement.Components.Search;

namespace HotelManagement.ViewModel.AdminVM.HistoryManagementVM
{
    public class HistoryManagementVM : BaseVM
    {
        private int SelectedView = 0;
        private ObservableCollection<ImportReceiptDTO> importList;
        public ObservableCollection<ImportReceiptDTO> ImportList
        {
            get { return importList; }
            set { importList = value; OnPropertyChanged(); }
        }
        private ObservableCollection<BillDTO> billExportList;
        public ObservableCollection<BillDTO> BillExportList
        {
            get { return billExportList; }
            set { billExportList = value; OnPropertyChanged(); }
        }
        private ComboBoxItem _selectedFilterImportItem;
        public ComboBoxItem SelectedFilterImportItem
        {
            get { return _selectedFilterImportItem; }
            set { _selectedFilterImportItem = value; OnPropertyChanged(); }
        }
        private int _selectedMonthImportItem;
        public int SelectedMonthImportItem
        {
            get { return _selectedMonthImportItem; }
            set { _selectedMonthImportItem = value; OnPropertyChanged(); }
        }
        private int _selectedMonthExportItem;
        public int SelectedMonthExportItem
        {
            get { return _selectedMonthExportItem; }
            set { _selectedMonthExportItem = value; OnPropertyChanged(); }
        }
        private int _filterImportList;
        public int FilterImportList
        {
            get { return _filterImportList; }
            set { _filterImportList = value; OnPropertyChanged(); }
        }
        private ComboBoxItem _filterExportList;
        public ComboBoxItem FilterExportList
        {
            get { return _filterExportList; }
            set { _filterExportList = value; OnPropertyChanged(); }
        }
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(); }
        }
        private DateTime GetCurrentDate;

        private BillDTO _selectedItem;
        public BillDTO SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }
        private BillDTO _billDetail;
        public BillDTO BillDetail
        {
            get { return _billDetail; }
            set { _billDetail = value; OnPropertyChanged(); }
        }
        private ImportReceiptDTO _selectedReceiptItem;
        public ImportReceiptDTO SelectedReceiptItem
        {
            get { return _selectedReceiptItem; }
            set { _selectedReceiptItem = value; OnPropertyChanged(); }
        }
        private ImportReceiptDTO _receiptDetail;
        public ImportReceiptDTO ReceiptDetail
        {
            get { return _receiptDetail; }
            set { _receiptDetail = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ProductUsingDTO> _ListServicePayment;
        public ObservableCollection<ProductUsingDTO> ListServicePayment
        {
            get { return _ListServicePayment; }
            set { _ListServicePayment = value; OnPropertyChanged(); }
        }
        public ICommand LoadImportPageCM { get; set; }
        public ICommand LoadExportPageCM { get; set; }
        public ICommand CheckImportItemFilterCM { get; set; }
        public ICommand SelectedImportMonthCM { get; set; }
        public ICommand ExportFileCM { get; set; }
        public ICommand FilterListImportCM { get; set; }
        public ICommand CheckItemFilterCM { get; set; }
        public ICommand SelectedMonthCM { get; set; }
        public ICommand SelectedDateExportListCM { get; set; }
        public ICommand LoadInforBillCM { get; set; }
        public ICommand LoadImportReceiptCM { get; set; }
        public ICommand FirstLoadRoomBillCM { get; set; }
        public ICommand FirstLoadReceiptCM { get; set; }
        public HistoryManagementVM()
        {
            GetCurrentDate = DateTime.Today;
            SelectedDate = GetCurrentDate;
            SelectedMonthExportItem = DateTime.Now.Month - 1;
            SelectedMonthImportItem = DateTime.Now.Month - 1;
            LoadImportPageCM = new RelayCommand<Frame>((p) => { return true; }, async (p) =>
            {
                SelectedView = 0;
                await GetImportListSource();
                ImportManagementPage page = new ImportManagementPage();
                p.Content = page;
            });
            LoadExportPageCM = new RelayCommand<Frame>((p) => { return true; }, async (p) =>
            {
                SelectedView = 1;
                BillExportList = new ObservableCollection<BillDTO>();
                await GetExportListSource("date");
                ExportManagementPage page = new ExportManagementPage();
                p.Content = page;
            });
            CheckImportItemFilterCM = new RelayCommand<System.Windows.Controls.ComboBox>((p) => { return true; }, async (p) =>
            {
                switch (SelectedFilterImportItem.Tag.ToString())
                {
                    case "Toàn bộ":
                        {
                            await GetImportListSource("");
                            return;
                        }
                    case "Theo tháng":
                        {
                            await GetImportListSource("month");
                            return;
                        }
                }
            });
            SelectedImportMonthCM = new RelayCommand<System.Windows.Controls.ComboBox>((p) => { return true; }, async (p) =>
            {
                await GetListImportByMonth();
            });
            ExportFileCM = new RelayCommand<Search>((p) => { return true; }, (p) =>
            {
                ExportToFileFunc(p.Text);
            });
            SelectedDateExportListCM = new RelayCommand<DatePicker>(p => true, async p =>
            {
                await GetExportListSource("date");
            });
            FilterListImportCM = new RelayCommand<ComboBox>(p => true, async p =>
            {
                switch (SelectedFilterImportItem.Tag.ToString())
                {
                    case "Toàn bộ":
                        {
                            ObservableCollection<ImportReceiptDTO> iplist = new ObservableCollection<ImportReceiptDTO>();
                            ObservableCollection<ImportReceiptDTO> importserviceList = new ObservableCollection<ImportReceiptDTO>(await ProductService.Ins.GetListImportReceipt());
                            ObservableCollection<ImportReceiptDTO> importFunitureList = new ObservableCollection<ImportReceiptDTO>(await FurnitureService.Ins.GetListImportFunitureReceipt());
                            foreach (var item in importserviceList)
                            {
                                iplist.Add(item);
                            }
                            foreach (var item in importFunitureList)
                            {
                                iplist.Add(item);
                            }
                            if (FilterImportList == 0)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist);
                            }
                            if (FilterImportList == 1)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist.Where(ip => ip.typeImport == 0));
                            }
                            if (FilterImportList == 2)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist.Where(ip => ip.typeImport == 1));
                            }
                            return;
                        }
                    case "Theo tháng":
                        {
                            ObservableCollection<ImportReceiptDTO> iplist = new ObservableCollection<ImportReceiptDTO>();

                            ObservableCollection<ImportReceiptDTO> importserviceList = new ObservableCollection<ImportReceiptDTO>(await ProductService.Ins.GetListImportReceipt(SelectedMonthImportItem + 1));
                            ObservableCollection<ImportReceiptDTO> importFunitureList = new ObservableCollection<ImportReceiptDTO>(await FurnitureService.Ins.GetListImportFunitureReceipt(SelectedMonthImportItem + 1));
                            foreach (var item in importserviceList)
                            {
                                iplist.Add(item);
                            }
                            foreach (var item in importFunitureList)
                            {
                                iplist.Add(item);
                            }
                            if (FilterImportList == 0)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist);
                            }
                            if (FilterImportList == 1)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist.Where(ip => ip.typeImport == 0));
                            }
                            if (FilterImportList == 2)
                            {
                                ImportList = new ObservableCollection<ImportReceiptDTO>(iplist.Where(ip => ip.typeImport == 1));
                            }
                            return;
                        }
                }
            });
            CheckItemFilterCM = new RelayCommand<System.Windows.Controls.ComboBox>((p) => { return true; }, async (p) =>
            {
                switch (FilterExportList.Tag.ToString())
                {
                    case "Toàn bộ":
                        {
                            await GetExportListSource("");
                            return;
                        }
                    case "Theo ngày":
                        {
                            await GetExportListSource("date");
                            return;
                        }
                    case "Theo tháng":
                        {
                            await GetExportListSource("month");
                            return;
                        }
                }
            });
            SelectedMonthCM = new RelayCommand<System.Windows.Controls.ComboBox>((p) => { return true; }, async (p) =>
            {
                await CheckMonthFilter();
            });
            LoadInforBillCM = new RelayCommand<object>(p => true, async p =>
            {
                if (SelectedItem != null)
                {
                    try
                    {
                        BillDetail = await Task.Run(() => BillService.Ins.GetBillDetails(SelectedItem.BillId));
                    }
                    catch (System.Data.Entity.Core.EntityException e)
                    {
                        CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                    catch (Exception e)
                    {
                        CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                    BillDetail b = new BillDetail();
                    b.ShowDialog();
                }

            });
            LoadImportReceiptCM = new RelayCommand<object>(p => true, async p =>
            {
                if (SelectedReceiptItem != null)
                {
                    try
                    {
                        if(SelectedReceiptItem.typeImport == 0)
                        {
                            ReceiptDetail = await Task.Run(() => ProductService.Ins.GetImportReceiptDetail(SelectedReceiptItem.ReceiptId));
                        }
                        if (SelectedReceiptItem.typeImport == 1)
                        {
                            ReceiptDetail = await Task.Run(() => FurnitureService.Ins.GetImportReceiptDetail(SelectedReceiptItem.ReceiptId));
                        }
                       
                    }
                    catch (System.Data.Entity.Core.EntityException e)
                    {
                        CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                    catch (Exception e)
                    {
                        CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                    ReceiptDetail b = new ReceiptDetail();
                    b.ShowDialog();
                }

            });
            FirstLoadRoomBillCM = new RelayCommand<Window>(p => true, async p =>
            {
                ListServicePayment = new ObservableCollection<ProductUsingDTO>(BillDetail.ListListProductPayment);
                ListServicePayment.Insert(0, new ProductUsingDTO
                {
                    ProductName = BillDetail.RoomName,
                    UnitPrice = BillDetail.RentalPrice,
                    Quantity = BillDetail.DayNumber,
                });
            });
            FirstLoadReceiptCM = new RelayCommand<Window>(p => true, async p =>
            {
               
            });
        }

        private async Task GetListImportByMonth()
        {
            ImportList = new ObservableCollection<ImportReceiptDTO>();
            try
            {
                ObservableCollection<ImportReceiptDTO> importserviceList = new ObservableCollection<ImportReceiptDTO>(await ProductService.Ins.GetListImportReceipt(SelectedMonthImportItem + 1));
                ObservableCollection<ImportReceiptDTO> importFunitureList = new ObservableCollection<ImportReceiptDTO>(await FurnitureService.Ins.GetListImportFunitureReceipt(SelectedMonthImportItem + 1));
                foreach (var item in importserviceList)
                {
                    ImportList.Add(item);
                }
                foreach (var item in importFunitureList)
                {
                    ImportList.Add(item);
                }
                return;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                throw;
            }
        }

        private async Task GetExportListSource(string v)
        {
            BillExportList = new ObservableCollection<BillDTO>();
            switch (v)
            {
                case "date":
                    {
                        try
                        {
                            BillExportList = new ObservableCollection<BillDTO>(await BillService.Ins.GetAllBillByDate(SelectedDate));
                            return;
                        }
                        catch (System.Data.Entity.Core.EntityException e)
                        {
                            CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }

                    }
                case "":
                    {
                        try
                        {
                            BillExportList = new ObservableCollection<BillDTO>(await BillService.Ins.GetAllBill());
                            return;
                        }
                        catch (System.Data.Entity.Core.EntityException e)
                        {
                            CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }

                    }
                case "month":
                    {
                        await CheckMonthFilter();
                        return;
                    }
            }
        }

        private async Task CheckMonthFilter()
        {
            try
            {
                BillExportList = new ObservableCollection<BillDTO>(await BillService.Ins.GetAllBillByMonth(SelectedMonthExportItem + 1));
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                throw;
            }

        }


        private async Task GetImportListSource(string v = "")
        {
            ImportList = new ObservableCollection<ImportReceiptDTO>();
            switch (v)
            {
                case "":
                    {
                        try
                        {
                            ObservableCollection<ImportReceiptDTO> importserviceList = new ObservableCollection<ImportReceiptDTO>(await ProductService.Ins.GetListImportReceipt());
                            ObservableCollection<ImportReceiptDTO> importFunitureList = new ObservableCollection<ImportReceiptDTO>(await FurnitureService.Ins.GetListImportFunitureReceipt());
                            foreach (var item in importserviceList)
                            {
                                ImportList.Add(item);
                            }
                            foreach (var item in importFunitureList)
                            {
                                ImportList.Add(item);
                            }
                            return;
                        }
                        catch (System.Data.Entity.Core.EntityException e)
                        {
                            CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                            throw;
                        }

                    }
                case "month":
                    {
                        await GetListImportByMonth();
                        return;
                    }
            }
        }
        public void ExportToFileFunc(string search)
        {
            switch (SelectedView)
            {
                case 0:
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", ValidateNames = true })
                        {
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                                app.Visible = false;
                                Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];

                                ws.Cells[1, 1] = "Mã đơn";
                                ws.Cells[1, 2] = "Loại phiếu nhập";
                                ws.Cells[1, 3] = "Ngày nhập";
                                ws.Cells[1, 4] = "Tổng tiền nhập";
                                ws.Cells[1, 5] = "Số lượng";
                                ws.Cells[1, 6] = "Nhân viên nhập";

                                int i2 = 2;

                                ObservableCollection<ImportReceiptDTO> listImportSearch = new ObservableCollection<ImportReceiptDTO>(ImportList.Where(item => item.typeImprtStr.ToLower().Contains(search.ToLower())
                                                                                                                                    || item.ReceiptIdStr.ToLower().Contains(search.ToLower())
                                                                                                                                    || item.StaffName.ToLower().Contains(search.ToLower())));

                                foreach (var item in listImportSearch)
                                {
                                    ws.Cells[i2, 1] = item.ReceiptIdStr;
                                    ws.Cells[i2, 2] = item.typeImprtStr;
                                    ws.Cells[i2, 3] = item.CreateAt;
                                    ws.Cells[i2, 4] = item.TotalPriceStr;
                                    ws.Cells[i2, 5] = item.TotalQuality;
                                    ws.Cells[i2, 6] = item.StaffName;

                                    i2++;
                                }
                                ws.SaveAs(sfd.FileName);
                                wb.Close();
                                app.Quit();

                                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

                                CustomMessageBox.ShowOk("Xuất file thành công", "Thông báo", "OK", CustomMessageBoxImage.Success);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", ValidateNames = true })
                        {
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                                app.Visible = false;
                                Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];


                                ws.Cells[1, 1] = "Mã hóa đơn";
                                ws.Cells[1, 2] = "Mã phiếu thuê";
                                ws.Cells[1, 3] = "Tên phòng";
                                ws.Cells[1, 4] = "Loại phòng";
                                ws.Cells[1, 5] = "Tổng tiền";
                                ws.Cells[1, 6] = "Phí thuê phòng";
                                ws.Cells[1, 7] = "Phí sản phẩm";
                                ws.Cells[1, 8] = "Nhân viên xuất";
                                ws.Cells[1, 9] = "Ngày xuất";
                                int i2 = 2;

                                ObservableCollection<BillDTO> listBillSearch = new ObservableCollection<BillDTO>(BillExportList.Where(item => item.BillId.ToLower().Contains(search.ToLower())
                                                                                                    || item.RentalContractId.ToLower().Contains(search.ToLower())
                                                                                                    || item.StaffName.ToLower().Contains(search.ToLower())));
                                foreach (var item in listBillSearch)
                                {

                                    ws.Cells[i2, 1] = item.BillId;
                                    ws.Cells[i2, 2] = item.RentalContractId;
                                    ws.Cells[i2, 3] = item.RoomName;
                                    ws.Cells[i2, 4] = item.RoomTypeName;
                                    ws.Cells[i2, 5] = item.TotalPriceTempStr;
                                    ws.Cells[i2, 6] = item.RentalPriceStr;
                                    ws.Cells[i2, 7] = item.ProductPriceTempStr;
                                    ws.Cells[i2, 8] = item.StaffName;
                                    ws.Cells[i2, 9] = item.CreateDate;


                                    i2++;
                                }
                                ws.SaveAs(sfd.FileName);
                                wb.Close();
                                app.Quit();

                                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                                CustomMessageBox.ShowOk("Xuất file thành công", "Thông báo", "OK", CustomMessageBoxImage.Success);

                            }
                        }
                        break;
                    }
            }
        }
    }
}