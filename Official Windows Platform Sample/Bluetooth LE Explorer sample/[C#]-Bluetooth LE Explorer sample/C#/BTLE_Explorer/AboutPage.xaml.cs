using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.System;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Phone.UI.Input;


using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;

using BTLE_Explorer.ViewModels;

namespace BTLE_Explorer
{
    /// <summary>
    /// Page with details on all active toasts, and options and functions that one could
    /// use to customize usage of this app
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        #region ------------------------------ Variables ------------------------------
        private ListBox _toastListBox;
        public BEAboutVM AboutVM { get; private set; }
        public BECharacteristicListVM ToastListVM { get; private set; }
        #endregion // Variables

        public AboutPage()
        {
            this.InitializeComponent();   // default init for page object

            // Handle back button press
            this.Loaded += (sender, e) => { HardwareButtons.BackPressed += OnBackPressed; };
            // De-register back button when the page is no longer visible
            this.Unloaded += (sender, e) => { HardwareButtons.BackPressed -= OnBackPressed; };
            AboutVM = new BEAboutVM();
            ToastListVM = new BECharacteristicListVM();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AboutVM.Initialize();
            ToastListVM.Initialize(GlobalSettings.CharacteristicsWithActiveToast);
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                e.Handled = true;
            }
        }

        private async void unregisterToasts_Click(object sender, RoutedEventArgs e)
        {
            await GlobalSettings.UnregisterAllToastsAsync();
            ToastListVM.Initialize(GlobalSettings.CharacteristicsWithActiveToast);
        }

        private async void clearCustomNames_Click(object sender, RoutedEventArgs e)
        {
            await AboutVM.ClearDictionariesAsync();
        }

        private void useCachedMode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            AboutVM.UseCachedMode(box.IsChecked == null ? false : (bool)box.IsChecked);
        }

        private void ToastListLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                _toastListBox = (ListBox)sender;
            }
        }

        private async void OnToastSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            foreach (var listBoxItem in listBox.SelectedItems)
            {
                BECharacteristicVM characteristicVM = (BECharacteristicVM)listBoxItem;
                await characteristicVM.CharacteristicM.TaskUnregisterForToastAsync();
                ToastListVM.Initialize(GlobalSettings.CharacteristicsWithActiveToast);
            }
        }
    }
}
