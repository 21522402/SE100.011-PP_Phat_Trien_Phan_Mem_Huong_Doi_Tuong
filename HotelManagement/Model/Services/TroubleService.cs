using HotelManagement.DTOs;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.ViewModel.StaffVM;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HotelManagement.Model.Services
{
    public class TroubleService
    {
        private static TroubleService _ins;
        public static TroubleService Ins
        {
            get { return _ins ?? (_ins = new TroubleService()); }
            private set { _ins = value; }
        }
        private TroubleService(){}
        public async Task<List<TroubleDTO>> GetAllTrouble()
        {
            List<TroubleDTO> Troublelist;
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    Troublelist = (from s in context.Troubles
                                   select new TroubleDTO {
                                       TroubleId = s.TroubleId,
                                       Title = s.Title,
                                       Avatar = s.Avatar,
                                       Description = s.Description,
                                       Price = (double)s.Price,
                                       StartDate = s.StartDate,
                                       FixedDate = s.FixedDate,
                                       StaffName = s.Staff.StaffName,
                                       FinishDate = s.FinishDate,
                                       Status = s.Status,
                                       StaffId=s.StaffId,
                                       Level = s.Level,
                                   }) .ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Troublelist.Reverse();
            return Troublelist;
        }
        public async Task<(bool, string, TroubleDTO)> AddTrouble(TroubleDTO s, string id  )
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {                    
                    var maxId = await context.Troubles.MaxAsync(p => p.TroubleId);

                    Trouble newtrouble = new Trouble { 
                        TroubleId = CreateNextTroubleId(maxId),
                        Title = s.Title,
                        Avatar = s.Avatar,
                        Description = s.Description,
                        Price = 0,
                        StartDate = s.StartDate,
                        Status = STATUS.WAITING,
                        StaffId = s.StaffId,
                        Level = s.Level ?? LEVEL.NORMAL,
                    };
                    
                    s.TroubleId = newtrouble.TroubleId;
                    context.Troubles.Add(newtrouble);
                    await context.SaveChangesAsync();
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                return (false, "Mất kết nối cơ sở dữ liệu", null);
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống", null);
            }
            return (true, "Báo cáo sự cố mới thành công", s);

        }
      
      
        public async Task<string> GetStaffNameById(string id)
        {
            string staffname;
            using ( var context = new HotelManagementEntities())
            {
                var staff = await context.Staffs.FindAsync(id);
                staffname = staff.StaffName;
            }
            return staffname;
        }
     
  
        public async Task<List<string>> GetCurrentListRentalContractId()
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                   var list = await context.RentalContracts.Where(x=> x.Room.RoomStatus == ROOM_STATUS.RENTING && x.Validated==true).Select(x=> x.RentalContractId).ToListAsync();
                    return list;
                }
            }
           
            catch (Exception)
            {
                return (null);
            }

        }
        private string CreateNextTroubleId(string maxId)
        {
            if (maxId is null)
            {
                return "SC001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "SC" + newIdString.Substring(newIdString.Length - 3, 3);
        }
    }
}
