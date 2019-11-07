using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;

using BTLE_Explorer.Models;

namespace BTLE_Explorer.ViewModels
{
    /// <summary>
    /// Glue between the Device View and Model
    /// </summary>
    public class BEDeviceVM : BEGattVMBase<BluetoothLEDevice>
    {
        #region Properties
        // Funnels the model's properties to the XAML UI.
        public BEDeviceModel DeviceM { get; private set; }
        
        public string Name
        {
            get
            {
                return DeviceM.Name.Trim();
            }
        }

        public UInt64 BluetoothAddress
        {
            get
            {
                return DeviceM.BluetoothAddress;
            }
        }

        public String DeviceId
        {
            get
            {
                return DeviceM.DeviceId;
            }
        }
        #region Connectivity 
        
        public string ConnectString
        {
            get
            {
                if (DeviceM.Connected)
                {
                    return "connected";
                }
                else
                {
                    return "disconnected";
                }
            }
        }
        
        public Brush ConnectColor
        {
            get
            {
                if (DeviceM.Connected)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
        }
        #endregion
        #endregion

        public void Initialize(BEDeviceModel deviceM)
        {
            Model = deviceM; 
            DeviceM = deviceM;
            DeviceM.Register(this);
        }
    }

}