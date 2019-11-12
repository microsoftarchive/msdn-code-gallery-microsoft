using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input; 
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input; 
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;

using BTLE_Explorer.Models;
using BTLE_Explorer.ViewModels;

namespace BTLE_Explorer
{
    /// <summary>
    /// Lists all characteristics and their values in a service.
    /// </summary>
    public sealed partial class ServiceInfo : Page
    {
        #region ------------------- Variables -------------------
        private ListBox _characteristicListBox;
        public BEServiceVM ServiceVM { get; private set; }
        public BECharacteristicListVM Characteristics { get; private set; }
        public BEDeviceVM DeviceVM { get; private set; }
        #endregion //endregion

        public ServiceInfo()
        {
            this.InitializeComponent();   // default init for page object

            // Handle back button press
            this.Loaded += (sender, e) => { HardwareButtons.BackPressed += OnBackPressed; };
            // De-register back button when the page is no longer visible
            this.Unloaded += (sender, e) => { HardwareButtons.BackPressed -= OnBackPressed; };
            ServiceVM = new BEServiceVM();
            Characteristics = new BECharacteristicListVM();
            DeviceVM = new BEDeviceVM();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                Characteristics.Unregister();
                this.Frame.GoBack();
                e.Handled = true;

                if (ServiceVM.ServiceM.DictionaryModelChanged)
                {
                    // This is not critical and doesn't have to block execution
                    // Make sure this is done at the end of this function, as it returns
                    // once the first blocking call is encountered
                    Utilities.RunFuncAsTask(GlobalSettings.ServiceDictionaryUnknown.SaveDictionaryAsync);
                }
            }
        }

        // Set up entry into page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Reset list choice
            if (_characteristicListBox != null)
            {
                _characteristicListBox.SelectedIndex = -1;
            }

            // These must be initialized before the UI loads
            DeviceVM.Initialize(GlobalSettings.SelectedService.DeviceM);
            ServiceVM.Initialize(GlobalSettings.SelectedService);
            Characteristics.Initialize(ServiceVM.ServiceM.CharacteristicModels);

            // Read all characteristic values
            Utilities.RunFuncAsTask(GlobalSettings.SelectedService.ReadCharacteristicsAsync);
        }


        /// <summary>
        /// XAML elements inside the HubSection are template-based and cannot be directly access, hook the ListBox instance here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCharacteristicListLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                _characteristicListBox = (ListBox)sender;
            }
        }

        // Navigate to Characteristic's page, once one has been chosen. 
        private void OnCharacteristicSelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox) sender;

            // Looks like we didn't actually select anything.
            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            // Get the characteristic that we've picked. 
            foreach (var listBoxItem in listBox.SelectedItems)
            {
                BECharacteristicVM characteristic = (BECharacteristicVM)listBoxItem;
                GlobalSettings.SelectedCharacteristic = characteristic.CharacteristicM;
            }

            // Go to the characteristic page. 
            this.Frame.Navigate(typeof(CharacteristicInfo));

            // Since we're navigating away from a page that may change a service, we should save
            // its dictionary.
            if (ServiceVM.ServiceM.DictionaryModelChanged)
            {
                // This is not critical and doesn't have to block execution
                // Make sure this is done at the end of this function, as it returns
                // once the first blocking call is encountered
                Utilities.RunFuncAsTask(GlobalSettings.ServiceDictionaryUnknown.SaveDictionaryAsync);
            }
        }

        #region -----------------------  Change Service Name -----------------------
        private void OnServiceNameChanged(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                TextBox t = (TextBox)sender;
                ServiceVM.ServiceM.UpdateName(t.Text);

                // Close the keyboard. (Slightly hacky, but works.)
                t.IsEnabled = false;
                t.IsEnabled = true;
            }
        }
        private void OnServiceNameLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            ServiceVM.ServiceM.UpdateName(t.Text);
        }
        #endregion
    }
}
