using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

using Windows.ApplicationModel;
using Windows.Foundation;

using BTLE_Explorer.Models; 

namespace BTLE_Explorer.ViewModels
{
    /// <summary>
    /// A list containing Bluetooth Devices with wrappers to the XAML UI. 
    /// </summary>
    public class BEDeviceListVM : ObservableCollection<BEDeviceVM> 
    {
        public void Initialize(ICollection<BEDeviceModel> deviceModels) 
        {
            this.Clear();
            foreach (BEDeviceModel deviceM in deviceModels) 
            {
                BEDeviceVM deviceVM = new BEDeviceVM();
                deviceVM.Initialize(deviceM); 
                this.Add(deviceVM);
            }
        }

        /// <summary>
        /// Unregisters view-models in this from their models.
        /// </summary>
        public void Unregister()
        {
            foreach (var VMinstance in this)
            {
                VMinstance.UnregisterVMFromModel();
            }
        }
    }
}