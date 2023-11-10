using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.DTOs
{
    public class RentalContractDTO
    {
        public string RentalContractId { get; set; }
        public string RoomId { get; set; }
        public int RoomNumber { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int STT_RentalContract { get; set; }
        public string CreateDateStr
        {
            get { return ((DateTime)CreateDate).ToString("dd/MM/yyyy"); }
        }
        public IList<RentalContractDetailDTO> RentalContracts { get; set; }

    }
}
