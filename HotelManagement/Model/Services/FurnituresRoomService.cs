using HotelManagement.DTOs;
using HotelManagement.Utils;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomInfo;
using IronXL.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace HotelManagement.Model.Services
{
    public class FurnituresRoomService
    {
        private FurnituresRoomService() { }
        private static FurnituresRoomService _ins;
        public static FurnituresRoomService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new FurnituresRoomService();
                return _ins;
            }
            private set { _ins = value; }
        }

     
    
     
        
      

       
       
        
    }
}
