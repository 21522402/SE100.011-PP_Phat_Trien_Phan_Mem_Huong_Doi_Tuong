using HotelManagement.Model;
using HotelManagement.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HotelManagement.DTOs
{
    public class FurnitureRoomTypeDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public double Price { get; set; }
        public int MaxNumberGuest { get; set; }
        public int NumberGuestForUnitPrice { get; set; }



        public FurnitureRoomTypeDTO() { }
        public FurnitureRoomTypeDTO(FurnitureRoomTypeDTO furnituresRoomDTO)
        {
            RoomTypeId = furnituresRoomDTO.RoomTypeId;
            RoomTypeName = furnituresRoomDTO.RoomTypeName;
            Price = furnituresRoomDTO.Price;
            MaxNumberGuest = furnituresRoomDTO.MaxNumberGuest;
            NumberGuestForUnitPrice = furnituresRoomDTO.NumberGuestForUnitPrice;
        }

        private Brush backgroundRoomBrush;
        public Brush BackgroundRoomBrush
        {
            get { return backgroundRoomBrush; }
            set { SetField(ref backgroundRoomBrush, value, "BackgroundRoomBrush"); }
        }

        private ObservableCollection<FurnitureDTO> listFurnitureRoomType;
        public ObservableCollection<FurnitureDTO> ListFurnitureRoomType
        {
            get { return listFurnitureRoomType; }

            set { SetField(ref listFurnitureRoomType, value, "ListFurnitureRoomType"); }
        }

        private int allFurnitureQuantity;
        public int AllFurnitureQuantity
        {
            get { return allFurnitureQuantity; }

            set { SetField(ref allFurnitureQuantity, value, "AllFurnitureQuantity"); }
        }
        private string allFurnitureString;
        public string AllFurnitureString
        {
            get { return allFurnitureString; }

            set { SetField(ref allFurnitureString, value, "AllFurnitureString"); }
        }


        private ObservableCollection<bool> roomCusList;
        public ObservableCollection<bool> RoomCusList
        {
            get { return roomCusList; }
            set { SetField(ref roomCusList, value, "RoomCusList"); }
        }

        private bool isEmptyRoom;
        public bool IsEmptyRoom
        {
            get { return isEmptyRoom; }
            set { SetField(ref isEmptyRoom, value, "IsEmptyRoom"); }
        }

        public void SetQuantityAndStringTypeFurniture()
        {
            List<string> furnitureType = ListFurnitureRoomType.Select(item => item.FurnitureType).Distinct().ToList();
            int length = furnitureType.Count();
            AllFurnitureQuantity = 0;
            AllFurnitureString = "";
            for (int i = 0; i < length; i++)
            {
                AllFurnitureQuantity += 1;
                if (i == 0)
                    AllFurnitureString += furnitureType[i];
                else
                    AllFurnitureString += (", " + furnitureType[i]);
            }
        }
        public void DeleteListFurniture(ObservableCollection<FurnitureDTO> listDelete)
        {
            foreach (FurnitureDTO item in listDelete)
            {
                if (item.DeleteInRoomQuantity == item.InUseQuantity)
                    ListFurnitureRoomType.Remove(item);
                else
                {
                    item.InUseQuantity -= item.DeleteInRoomQuantity;
                    item.DeleteInRoomQuantity = item.InUseQuantity;
                }
            }
        }
    }
}
