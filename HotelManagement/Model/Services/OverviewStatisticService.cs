using HotelManagement.DTOs;
using HotelManagement.Utilities;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace HotelManagement.Model.Services
{
    public partial class OverviewStatisticService
    {
        private OverviewStatisticService() { }
        private static OverviewStatisticService _ins;
        public static OverviewStatisticService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new OverviewStatisticService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        public async Task<int> GetBillQuantity(int year, int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    return await context.Bills.Where(b => b.CreateDate.Value.Year == year && b.CreateDate.Value.Month == month).CountAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(List<double>, double ServiceRevenue, double RentalRevenue, string RentalRateStr)> GetRevenueByYear(int year)
        {
            List<double> MonthlyRevenueList = new List<double>(new double[12]);

            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var billList = context.Bills
                    .Where(b => b.CreateDate.Value.Year == year);

                    (double ServiceRevenue, double RentalRevenue) = await GetFullRevenue(context, year);

                    var MonthlyRevenue = billList
                             .GroupBy(b => b.CreateDate.Value.Month)
                             .Select(gr => new
                             {
                                 Month = gr.Key,
                                 Income = gr.Sum(b => (double?)b.TotalPrice) ?? 0
                             }).ToList();

                    foreach (var re in MonthlyRevenue)
                    {
                        MonthlyRevenueList[re.Month - 1] = (float)re.Income;
                    }

                    (double lastServiceRevenue, double lastRentalRevenue) = await GetFullRevenue(context, year - 1);
                    double lastRevenueTotal = lastServiceRevenue + lastRentalRevenue;
                    string RevenueRateStr;
                    if (lastRevenueTotal == 0)
                    {
                        RevenueRateStr = "-2";
                        if (ServiceRevenue + RentalRevenue == 0) RevenueRateStr = "-3";
                    }
                    else
                    {
                        RevenueRateStr = Helper.ConvertDoubleToPercentageStr((double)((ServiceRevenue + RentalRevenue) / lastRevenueTotal) - 1);
                    }

                    return (MonthlyRevenueList, (double)ServiceRevenue, (double)RentalRevenue, RevenueRateStr);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<(double, double)> GetFullRevenue(HotelManagementEntities context, int year, int month = 0)
        {
            try
            {
                if (month != 0)
                {
                    year = DateTime.Now.Year;
                    double ProductRevenue = await context.Revenues.Where(r => r.Year == year && r.Month == month).SumAsync(r => r.RevenueProducts.Sum(rp => rp.Revenue)) ?? 0;
                    double TotalPrice = await context.Bills.Where(x => x.CreateDate.Value.Year == year && x.CreateDate.Value.Month == month).SumAsync(x => x.TotalPrice) ?? 0;
                    double RentalRevenue = TotalPrice - ProductRevenue;
                    return (ProductRevenue, RentalRevenue);
                }
                else
                {
                    double ProductRevenue = await context.Revenues.Where(r => r.Year == year).SumAsync(r => r.RevenueProducts.Sum(rp => rp.Revenue)) ?? 0;
                    double TotalPrice = await context.Bills.Where(x => x.CreateDate.Value.Year == year).SumAsync(x => x.TotalPrice) ?? 0;
                    double RentalRevenue = TotalPrice - ProductRevenue;
                    return (ProductRevenue, RentalRevenue);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(List<double> MonthlyExpense,
            double ProductRevenue,
            double RepairCost,
            double FurnitureExpense,
            string ExpenseRate
            )> GetExpenseByYear(int year)
        {
            List<double> MonthlyExpense = new List<double>(new double[12]);
            double ServiceExpenseTotal = 0;
            double RepairCostTotal = 0;
            double FurnitureExpenseTotal = 0;

            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var MonthlyServiceExpense = await context.ProductReceipts
                     .Where(b => b.CreateAt.Value.Year == year)
                     .GroupBy(b => b.CreateAt.Value.Month)
                     .Select(gr => new
                     {
                         Month = gr.Key,
                         Outcome = gr.Sum(pr => pr.Price) ?? 0
                     }).ToListAsync();

                    var MonthlyFurnitureExpense = await context.FurnitureReceipts
                        .Where(x => x.CreateAt.Value.Year == year)
                        .GroupBy(x => x.CreateAt.Value.Month)
                         .Select(gr => new
                         {
                             Month = gr.Key,
                             Outcome = gr.Sum(b => (double?)b.Price) ?? 0
                         }).ToListAsync();

                    var MonthlyRepairCost = await context.Troubles
                         .Where(t => t.FinishDate != null && t.FinishDate.Value.Year == year)
                         .GroupBy(t => t.FinishDate.Value.Month)
                         .Select(gr =>
                         new
                         {
                             Month = gr.Key,
                             Outcome = (gr.Sum(t => t.Price))
                         }).ToListAsync();



                    //Accumulate
                    foreach (var ex in MonthlyServiceExpense)
                    {
                        MonthlyExpense[ex.Month - 1] += (double?)ex.Outcome ?? 0;
                        ServiceExpenseTotal += (double?)ex.Outcome ?? 0;
                    }

                    foreach (var ex in MonthlyRepairCost)
                    {
                        MonthlyExpense[ex.Month - 1] += (double)ex.Outcome;
                        RepairCostTotal += (double?)ex.Outcome ?? 0;
                    }
                    foreach (var ex in MonthlyFurnitureExpense)
                    {
                        MonthlyExpense[ex.Month - 1] += (double?)ex.Outcome ?? 0;
                        FurnitureExpenseTotal += (double?)ex.Outcome ?? 0;
                    }

                    double lastExpenseTotal = await GetFullExpenseLastTime(context, year - 1);

                    string ExpenseRateStr;
                    //check mẫu  = 0
                    if (lastExpenseTotal == 0)
                    {
                        ExpenseRateStr = "-2";
                        if (ServiceExpenseTotal + RepairCostTotal + FurnitureExpenseTotal == 0) ExpenseRateStr = "-3";
                    }

                    else
                    {
                        ExpenseRateStr = Helper.ConvertDoubleToPercentageStr(((double)(ServiceExpenseTotal + RepairCostTotal + FurnitureExpenseTotal / lastExpenseTotal) - 1));
                    }


                    return (MonthlyExpense, (double)ServiceExpenseTotal, (double)RepairCostTotal, (double)FurnitureExpenseTotal, ExpenseRateStr);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<double> GetFullExpenseLastTime(HotelManagementEntities context, int year, int month = 0)
        {
            try
            {
                if (month == 0)
                {
                    //Service Receipt
                    var LastYearServiceExpense = await context.ProductReceipts
                             .Where(pr => pr.CreateAt != null && pr.CreateAt.Value.Year == year).ToListAsync();
                    double lastYearServiceExpense = 0;
                    foreach (var item in LastYearServiceExpense)
                    {
                        if (item != null) lastYearServiceExpense += (double)item.Price;
                    }

                    //Repair Cost
                    var LastYearRepairCost = await context.Troubles
                             .Where(tr => tr.FinishDate != null && tr.FinishDate.Value.Year == year).ToListAsync();
                    double lastYearRepairCost = 0;
                    foreach (var item in LastYearRepairCost)
                    {
                        if (item != null) lastYearRepairCost += (double)item.Price;
                    }

                    var LastYearFurinitureExpenses = await context.FurnitureReceipts
                        .Where(fn => fn.CreateAt != null && fn.CreateAt.Value.Year == year).ToListAsync();
                    double lastYearFurinitureExpenses = 0;
                    foreach (var item in LastYearFurinitureExpenses)
                    {
                        if (item != null) lastYearFurinitureExpenses += (double)item.Price;
                    }

                    return (lastYearServiceExpense + lastYearRepairCost + lastYearFurinitureExpenses);
                }
                else
                {
                    //Service Receipt
                    var LastMonthServiceExpense = await context.ProductReceipts
                             .Where(pr => pr.CreateAt != null && pr.CreateAt.Value.Year == year && pr.CreateAt.Value.Month == month).ToListAsync();
                    double lastMonthServiceExpense = 0;
                    foreach (var item in LastMonthServiceExpense)
                    {
                        if (item != null) lastMonthServiceExpense += (double)item.Price;
                    }

                    //Repair Cost
                    var LastMonthRepairCost = await context.Troubles
                             .Where(tr => tr.FinishDate != null && tr.FinishDate.Value.Year == year && tr.FinishDate.Value.Month == month).ToListAsync();

                    double lastMonthRepairCost = 0;
                    foreach (var item in LastMonthRepairCost)
                    {
                        if (item != null)
                        {
                            if (item.Price != null) lastMonthRepairCost += (double)item.Price;
                        }

                    }
                    var LastMonthFurinitureExpenses = await context.FurnitureReceipts
                       .Where(fn => fn.CreateAt != null && fn.CreateAt.Value.Year == year && fn.CreateAt.Value.Month == month).ToListAsync();
                    double lastMonthFurinitureExpenses = 0;
                    foreach (var item in LastMonthFurinitureExpenses)
                    {
                        if (item != null) lastMonthFurinitureExpenses += (double)item.Price;
                    }
                    return (lastMonthServiceExpense + lastMonthRepairCost + lastMonthFurinitureExpenses);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(List<double>,
            double ServiceRevenue, double RentalRevenue,
            string RevenueRate)> GetRevenueByMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            List<double> DailyReveList = new List<double>(new double[days]);

            try
            {

                using (var context = new HotelManagementEntities())
                {
                    var billList = context.Bills
                     .Where(b => b.CreateDate.Value.Year == year && b.CreateDate.Value.Month == month);

                    (double ServiceRevenue, double RentalRevenue) = await GetFullRevenue(context, year, month);

                    var dailyRevenue = await billList
                                 .GroupBy(b => b.CreateDate.Value.Day)
                                 .Select(gr => new
                                 {
                                     Day = gr.Key,
                                     Income = gr.Sum(b => b.TotalPrice),
                                     DiscountPrice = 0,
                                 }).ToListAsync();

                    foreach (var re in dailyRevenue)
                    {
                        DailyReveList[re.Day - 1] = (double)re.Income;
                    }

                    if (month == 1)
                    {
                        year--;
                        month = 13;
                    }
                    (double lastServiceReve, double lastRentalReve) = await GetFullRevenue(context, year, month - 1);
                    double lastRevenueTotal = lastServiceReve + lastRentalReve;
                    string RevenueRateStr;
                    if (lastRevenueTotal == 0)
                    {
                        RevenueRateStr = "-2";
                        if (ServiceRevenue + RentalRevenue == 0) RevenueRateStr = "-3";
                    }
                    else
                    {
                        RevenueRateStr = Helper.ConvertDoubleToPercentageStr((double)((ServiceRevenue + RentalRevenue) / lastRevenueTotal) - 1);
                    }

                    return (DailyReveList, (double)ServiceRevenue, (double)RentalRevenue, RevenueRateStr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(List<double> DailyExpense,
            double ServiceExpense, double RepairCost,
            double FurnitureExpense, string RepairRateStr)> GetExpenseByMonth(int year, int month)
        {

            int days = DateTime.DaysInMonth(year, month);
            List<double> DailyExpense = new List<double>(new double[days]);
            double ServiceExpenseTotal = 0;
            double RepairCostTotal = 0;
            double FurnitureExpenseTotal = 0;

            try
            {
                using (var context = new HotelManagementEntities())
                {
                    //Product Receipt
                    var MonthlyServiceExpense = await context.ProductReceipts
                         .Where(b => b.CreateAt.Value.Year == year && b.CreateAt.Value.Month == month)
                         .GroupBy(b => b.CreateAt.Value.Day)
                         .Select(gr => new
                         {
                             Day = gr.Key,
                             Outcome = gr.Sum(b => (double?)b.Price) ?? 0
                         }).ToListAsync();

                    //Repair Cost
                    var MonthlyRepairCost = await context.Troubles
                         .Where(t => t.FinishDate != null && t.FinishDate.Value.Year == year && t.FinishDate.Value.Month == month)
                         .GroupBy(t => t.FinishDate.Value.Day)
                         .Select(gr => new
                         {
                             Day = gr.Key,
                             Outcome = gr.Sum(t => (double?)t.Price)
                         }).ToListAsync();

                    // Furniture cost
                    var MonthlyFurnitureExpense = await context.FurnitureReceipts
                        .Where(b => b.CreateAt.Value.Year == year && b.CreateAt.Value.Month == month)
                        .GroupBy(b => b.CreateAt.Value.Day)
                        .Select(gr => new
                        {
                            Day = gr.Key,
                            Outcome = gr.Sum(b => (double?)b.Price) ?? 0
                        }).ToListAsync();

                    //context.
                    //Accumulate
                    foreach (var ex in MonthlyServiceExpense)
                    {
                        DailyExpense[ex.Day - 1] += (double?)ex.Outcome ?? 0;
                        ServiceExpenseTotal += ex.Outcome;
                    }

                    foreach (var ex in MonthlyRepairCost)
                    {
                        DailyExpense[ex.Day - 1] += (double)ex.Outcome;
                        RepairCostTotal += (double?)ex.Outcome ?? 0;
                    }

                    foreach (var ex in MonthlyFurnitureExpense)
                    {
                        DailyExpense[ex.Day - 1] += (double?)ex.Outcome ?? 0;
                        FurnitureExpenseTotal += (double?)ex.Outcome ?? 0;
                    }
                    if (month == 1)
                    {
                        year--;
                        month = 13;
                    }


                    double lastProductExpenseTotal = await GetFullExpenseLastTime(context, year, month - 1);
                    string ExpenseRateStr;
                    //check mẫu  = 0
                    if (lastProductExpenseTotal == 0)
                    {
                        ExpenseRateStr = "-2";
                        if (ServiceExpenseTotal + RepairCostTotal + FurnitureExpenseTotal == 0) ExpenseRateStr = "-3";
                    }
                    else
                    {
                        ExpenseRateStr = Helper.ConvertDoubleToPercentageStr(((double)(ServiceExpenseTotal + RepairCostTotal + FurnitureExpenseTotal / lastProductExpenseTotal) - 1));
                    }

                    return (DailyExpense, ServiceExpenseTotal, RepairCostTotal, FurnitureExpenseTotal, ExpenseRateStr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<RoomTypeDTO>> GetListRoomTypeRevenue(int year, int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var revId = (await context.Revenues.FirstOrDefaultAsync(x => x.Year == year && x.Month == month))?.RevenueId ?? -1;

                    var list2 = await context.RevenueRoomTypes.Where(r => r.RevenueId == revId)
                        .Select(rt => new RoomTypeDTO
                        {
                            RoomTypeName = rt.RoomType.RoomTypeName,
                            Revenue = (double)rt.Revenue
                        }).ToListAsync();
                    for (int i = 0; i < list2.Count(); i++)
                    {
                        list2[i].STT = i + 1;
                    }
                    return list2;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<ProductStatisticDTO>> GetListServiceTypeRevenue(int year, int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var revId = (await context.Revenues.FirstOrDefaultAsync(x => x.Year == year && x.Month == month))?.RevenueId ?? -1;

                    var list2 = await context.RevenueProducts.Where(r => r.RevenueId == revId)
                        .Select(rt => new ProductStatisticDTO
                        {
                            ProductName = rt.Product.ProductName,
                            Revenue = (double)rt.Revenue
                        }).ToListAsync();
                    for (int i = 0; i < list2.Count(); i++)
                    {
                        list2[i].STT = i + 1;
                    }
                    return list2;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<SeriesCollection> GetDataRoomTypePieChart(int year, int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var listRoomType = await GetListRoomTypeRevenue(year, month);
                    SeriesCollection listRoomChart = new SeriesCollection();
                    foreach (var item in listRoomType)
                    {
                        PieSeries p = new PieSeries
                        {
                            Values = new ChartValues<double> { item.Revenue },
                            Title = item.RoomTypeName,
                        };
                        listRoomChart.Add(p);
                    }
                    return listRoomChart;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<SeriesCollection> GetDataServiceTypePieChart(int year, int month)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var listServiceType = await GetListServiceTypeRevenue(year, month);
                    SeriesCollection listRoomChart = new SeriesCollection();
                    foreach (var item in listServiceType)
                    {
                        PieSeries p = new PieSeries
                        {
                            Values = new ChartValues<double> { item.Revenue },
                            Title = item.ProductName,
                        };
                        listRoomChart.Add(p);
                    }
                    return listRoomChart;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<string> GetListFilterYear()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var listYear = context.Bills.Select(x => x.CreateDate.Value.Year).ToList();
                    if (listYear == null) listYear = new List<int>();
                    if (!listYear.Contains(DateTime.Now.Year))
                    {
                        listYear.Add(DateTime.Now.Year);
                    }
                    var listYearStr = listYear.Select(x => "Năm " + x.ToString()).ToList();
                    listYearStr.Reverse();
                    List<string> years = new List<string>();
                    foreach (var year in listYearStr)
                    {
                        if (!years.Contains(year)) years.Add(year);
                    }
                    return years;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetListRoomTypeName()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var list = context.RoomTypes.Select(x => x.RoomTypeName).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetListProductName()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var list = context.Products.Select(x => x.ProductName).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int CreateNextRvId(int maxRvId)
        {
            return maxRvId + 1;
        }
        public async Task<String> StatisticalRevenue()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    List<String> roomTypeNames = this.GetListRoomTypeName();
                    List<string> productNames = this.GetListProductName();

                    List<RevenueRoomTypeDTO> revenueRoomTypes = await (
                                                                from rt in context.RentalContracts
                                                                where rt.Validated == false
                                                                select new RevenueRoomTypeDTO
                                                                {
                                                                    RentalContractId = rt.RentalContractId,
                                                                    RoomTypeId = rt.Room.RoomType.RoomTypeId,
                                                                    RentalPrice = rt.RentalPrice,
                                                                    EndDate = rt.EndDate,
                                                                    StartDate = rt.StartDate,
                                                                    RoomId = rt.RoomId,
                                                                    RoomType = rt.Room.RoomType.RoomTypeName
                                                                }).ToListAsync();
                    
                    List<RevenueProductDTO> revenueProducts = await (from pu in context.ProductUsings
                                                                     where pu.RentalContract.Validated == false
                                                                     select new RevenueProductDTO
                                                                     {
                                                                         RentalContractId = pu.RentalContractId,
                                                                         UnitPrice= pu.UnitPrice,
                                                                         CreateDate = pu.RentalContract.EndDate,
                                                                         ProductId = pu.ProductId,
                                                                         ProductName = pu.Product.ProductName,
                                                                         Quantity = (int)pu.Quantity

                                                                     }).ToListAsync();

                    var uniqueMonthsAndYears1 = revenueRoomTypes
                                                        .Where(e => e.EndDate.HasValue)
                                                        .GroupBy(e => new { e.RoomTypeId, e.EndDate.Value.Month, e.EndDate.Value.Year })
                                                        .Select(group => new
                                                        {
                                                            RoomTypeId = group.Key.RoomTypeId,
                                                            Month = group.Key.Month,
                                                            Year = group.Key.Year,
                                                            TotalMoney = group.Sum(e => e.DayNumber * e.RentalPrice)
                                                        })
                                                        .ToList();

                    var uniqueMonthsAndYears2 = revenueProducts
                                                    .Where(e => e.CreateDate.HasValue)
                                                    .GroupBy(e => new { e.ProductId, e.CreateDate.Value.Month, e.CreateDate.Value.Year })
                                                    .Select(group => new
                                                    {
                                                        ProductId = group.Key.ProductId,
                                                        Month = group.Key.Month,
                                                        Year = group.Key.Year,
                                                        TotalMoney = group.Sum(e => e.Quantity * e.UnitPrice)
                                                    }).ToList();
                    foreach (var item in uniqueMonthsAndYears1)
                    {
                        var findRevenue = await context.Revenues.FirstOrDefaultAsync(i =>
                                                        i.Month == item.Month
                                                        && i.Year == item.Year);
                        if (findRevenue == null)
                        {
                            Revenue revenue = new Revenue
                            {
                                Month = item.Month,
                                Year =item.Year,
                            };
                            context.Revenues.Add(revenue);
                            context.SaveChanges();
                            
                            RevenueRoomType revenueRoomType = new RevenueRoomType
                            {
                                RevenueId = revenue.RevenueId,
                                RoomTypeId = item.RoomTypeId,
                                Revenue = item.TotalMoney
                            };
                            context.RevenueRoomTypes.Add(revenueRoomType);

                            context.SaveChanges();
                        }
                        else
                        {
                            var findRvRoomType = await context.RevenueRoomTypes.FirstOrDefaultAsync(i => i.RevenueId == findRevenue.RevenueId && i.RoomTypeId == item.RoomTypeId);
                            if(findRvRoomType == null)
                            {
                                RevenueRoomType revenueRoomType = new RevenueRoomType
                                {
                                    RevenueId = findRevenue.RevenueId,
                                    RoomTypeId = item.RoomTypeId,
                                    Revenue = item.TotalMoney
                                };
                                context.RevenueRoomTypes.Add(revenueRoomType);

                                context.SaveChanges();
                            }
                            else
                            {
                                findRvRoomType.Revenue = item.TotalMoney;
                                context.SaveChanges();
                            }
                        }
                    }

                    foreach (var item in uniqueMonthsAndYears2)
                    {
                        var findRevenue = await context.Revenues.FirstOrDefaultAsync(i =>
                                                        i.Month == item.Month
                                                        && i.Year == item.Year);
                        if (findRevenue == null)
                        {
                            Revenue revenue = new Revenue
                            {
                                Month = item.Month,
                                Year = item.Year,
                            };
                            context.Revenues.Add(revenue);
                            context.SaveChanges();

                            RevenueProduct revenueProduct = new RevenueProduct
                            {
                                RevenueId = revenue.RevenueId,
                                ProductId = item.ProductId,
                                Revenue = item.TotalMoney
                            };
                            context.RevenueProducts.Add(revenueProduct);

                            context.SaveChanges();
                        }
                        else
                        {
                            var findRvProduct = await context.RevenueProducts.FirstOrDefaultAsync(i => i.RevenueId == findRevenue.RevenueId && i.ProductId == item.ProductId);
                            if (findRvProduct == null)
                            {
                                RevenueProduct revenueProduct = new RevenueProduct
                                {
                                    RevenueId = findRevenue.RevenueId,
                                    ProductId = item.ProductId,
                                    Revenue = item.TotalMoney
                                };
                                context.RevenueProducts.Add(revenueProduct);

                                context.SaveChanges();
                            }
                            else
                            {
                                findRvProduct.Revenue = item.TotalMoney;
                                context.SaveChanges();
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                    return "ok";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        

    }
}