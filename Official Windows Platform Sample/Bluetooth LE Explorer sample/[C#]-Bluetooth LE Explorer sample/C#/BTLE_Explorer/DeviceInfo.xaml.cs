using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

using Windows.Foundation;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;

using BTLE_Explorer.Models; 
using BTLE_Explorer.ViewModels; 

namespace BTLE_Explorer
{
    /// <summary>
    /// Page representing a specific device, and all its services
    /// </summary>
    public sealed partial class DeviceInfo : Page
    {

        #region ------------------------------ Variables ------------------------------
        private ListBox _serviceListBox;
        public BEDeviceVM DeviceVM { get; private set; }
        public BEServiceListVM ServicesVM { get; private set; }
        #endregion // Variables

        public DeviceInfo()
        {
            this.InitializeComponent();   // default init for page object

            // Handle back button press
            this.Loaded += (sender, e) => { HardwareButtons.BackPressed += OnBackPressed; };
            // De-register back button when the page is no longer visible
            this.Unloaded += (sender, e) => { HardwareButtons.BackPressed -= OnBackPressed; };

            // Create initial instances of page objects
            DeviceVM = new BEDeviceVM();
            ServicesVM = new BEServiceListVM();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                DeviceVM.UnregisterVMFromModel();
                this.Frame.GoBack();
                e.Handled = true;

                // Unregister notifications off the UI thrad
                Utilities.RunFuncAsTask(GlobalSettings.SelectedDevice.UnregisterNotificationsAsync);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Reset list choice
            if (_serviceListBox != null)
            {
                _serviceListBox.SelectedIndex = -1;
            }

            // These must be initialized before the UI loads
            DeviceVM.Initialize(GlobalSettings.SelectedDevice);
            ServicesVM.Initialize(GlobalSettings.SelectedDevice.ServiceModels);

            // Complete remaining initialization without blocking this callback
            Utilities.RunFuncAsTask(GlobalSettings.SelectedDevice.RegisterNotificationsAsync);
        }

        // XAML elements inside the HubSection are template-based and cannot be directly access, hook the ListBox instance here
        private void ServiceListLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                _serviceListBox = (ListBox)sender;
            }
        }

        private void OnServiceSelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            foreach (var listBoxItem in listBox.SelectedItems)
            {
                BEServiceVM service = (BEServiceVM) listBoxItem;
                GlobalSettings.SelectedService = service.ServiceM;
            }
            this.Frame.Navigate(typeof(ServiceInfo));
        }

    }
}
