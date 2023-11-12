using HotelManagement.DTOs;
using HotelManagement.Utilities;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

    }
}
