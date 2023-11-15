using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeFurnitureManagementVM
{
    public partial class RoomTypeFurnitureManagementVM
    {
        public ICommand FirstLoadDeleteWindowCM { get; set; }
        public ICommand CloseFurnitureRoomDeleteCM { get; set; }
    }
}
