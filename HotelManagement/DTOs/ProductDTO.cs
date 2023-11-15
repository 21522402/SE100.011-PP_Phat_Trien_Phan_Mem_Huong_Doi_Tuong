using HotelManagement.Model;
using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HotelManagement.DTOs
{
    public class ProductDTO : INotifyPropertyChanged
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

        private ImageSource productAvatar;
        public ImageSource ProductAvatar
        {
            get { return productAvatar; }
            set { SetField(ref productAvatar, value, "ProductAvatar"); }
        }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public double ProductPrice { get; set; }
        public byte[] ProductAvatarData { get; set; }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set { SetField(ref quantity, value, "Quantity"); }
        }
        private double importPrice;
        public double ImportPrice
        {
            get { return importPrice; }
            set { SetField(ref importPrice, value, "ImportPrice"); }
        }

        private int importQuantity;
        public int ImportQuantity
        {
            get { return importQuantity; }
            set { SetField(ref importQuantity, value, "ImportQuantity"); }
        }

        private int remainQuantity;
        public int RemainQuantity
        {
            get { return remainQuantity; }
            set { SetField(ref remainQuantity, value, "RemainQuantity"); }
        }

        public string TotalImportPriceStr { get; set; }

        public string PriceStr { get; set; }
        public string Unit { get; set; }

        public ProductDTO()
        {
        }
        public ProductDTO(string productId, string productName, string productType, double productPrice, int quantity, byte[] productAvatarData, ImageSource productAvatar)
        {
            ProductId = productId;
            ProductName = productName;
            ProductType = productType;
            ProductPrice = productPrice;
            Quantity = quantity;
            ProductAvatarData = productAvatarData;
            ProductAvatar = productAvatar;
            PriceStr = Helper.FormatVNMoney((double)ProductPrice);
        }
        public ProductDTO(ProductDTO s)
        {

            ProductId = s.ProductId;
            ProductName = s.ProductName;
            ProductType = s.ProductType;
            ProductPrice = s.ProductPrice;
            Quantity = s.Quantity;
            ProductAvatarData = s.ProductAvatarData;
            ProductAvatar = s.ProductAvatar;
            PriceStr = Helper.FormatVNMoney((double)ProductPrice);
        }
        public void SetTotalImportPrice()
        {
            TotalImportPriceStr = Helper.FormatVNMoney(ImportQuantity * ImportPrice);
        }
        public void FormatStringUnitAndPrice()
        {
            PriceStr = Helper.FormatVNMoney((double)ProductPrice);
        }

        
        public void SetAvatar()
        {
            if (ProductAvatarData != null)
                ProductAvatar = LoadAvatarImage(ProductAvatarData);
        }
        public void SetAvatar(string filePath)
        {
            BitmapImage _image = new BitmapImage();
            _image.BeginInit();
            _image.CacheOption = BitmapCacheOption.None;
            _image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            _image.CacheOption = BitmapCacheOption.OnLoad;
            _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _image.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            _image.EndInit();

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] photo_aray = new byte[fs.Length];
            fs.Read(photo_aray, 0, photo_aray.Length);
            ProductAvatarData = photo_aray;
            ProductAvatar = _image;
        }
        public BitmapImage LoadAvatarImage(byte[] data)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                Image img = Image.FromStream(stream);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
