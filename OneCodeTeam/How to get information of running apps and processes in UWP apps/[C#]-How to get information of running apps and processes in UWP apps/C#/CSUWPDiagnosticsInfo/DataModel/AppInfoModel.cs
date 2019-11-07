using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace CSUWPDiagnosticsInfo.DataModel
{
    public class AppInfoModel : INotifyPropertyChanged
    {
        
        public string AppUserModelId { get; set; }
        public string PackageFamilyName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        private BitmapImage logoImage;
        public BitmapImage LogoImage { get { return logoImage; } set { logoImage = value; NotifyPropertyChanged("LogoImage"); } }

        public AppInfoModel(AppInfo appInfo)
        {
            AppUserModelId = appInfo.AppUserModelId;
            DisplayName = appInfo.DisplayInfo.DisplayName;
            Description = appInfo.DisplayInfo.Description;
            PackageFamilyName = appInfo.PackageFamilyName;
            RandomAccessStreamReference logoStream = appInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(64, 64));
            SetLogo(logoStream);
        }

        private async void SetLogo(RandomAccessStreamReference logoStream)
        {
            IRandomAccessStreamWithContentType logoContent = await logoStream.OpenReadAsync();
            BitmapImage bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(logoContent);
            LogoImage = bitmap;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
