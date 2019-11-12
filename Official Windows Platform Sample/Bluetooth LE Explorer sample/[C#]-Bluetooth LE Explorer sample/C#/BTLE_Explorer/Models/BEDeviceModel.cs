using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;

using BTLE_Explorer.Dictionary;
using BTLE_Explorer.ViewModels;


namespace BTLE_Explorer.Models
{
    /// <summary>
    /// A model class to handle data manipulations. Manipulations to this class will push
    /// changes to the corresponding view model instances, which is bound to the UI. 
    ///
    /// This model is a wrapper around the BluetoothLEDevice class.
    /// </summary>
    public class BEDeviceModel : BEGattModelBase<BluetoothLEDevice>
    {
        #region ------------------------------ Properties ------------------------------
        private DeviceInformation _deviceInfo;
        public List<BEServiceModel> ServiceModels { get; private set; }
        private BluetoothLEDevice _device { get; set; }
        
        public String Name
        {
            get
            {
                return _device.Name.Trim();
            }
        }  // sanitized to remove spaces

        public UInt64 BluetoothAddress
        {
            get
            {
                return _device.BluetoothAddress;
            }
        }

        public String DeviceId
        {
            get
            {
                return _device.DeviceId;
            }
        }
        
        public bool Connected;
        #endregion // Properties

        #region ------------------------------ Constructor/Initialize ------------------------------
        public BEDeviceModel()
        {
            ServiceModels = new List<BEServiceModel>();
            this._viewModelInstances = new List<BEGattVMBase<BluetoothLEDevice>>();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceInfo"></param>
        public void Initialize(BluetoothLEDevice device, DeviceInformation deviceInfo)
        {
            // Check for valid input
            if (device == null)
            {
                throw new ArgumentNullException("In BEDeviceVM, BluetoothLEDevice cannot be null.");
            }
            if (deviceInfo == null)
            {
                throw new ArgumentNullException("In BEDeviceVM, DeviceInformation cannot be null.");
            }

            // Initialize variables
            _device = device;
            _deviceInfo = deviceInfo;
            if (_device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                Connected = true;
            }

            foreach (GattDeviceService service in _device.GattServices)
            {
                BEServiceModel serviceM = new BEServiceModel();
                serviceM.Initialize(service, this);
                ServiceModels.Add(serviceM); 
            }

            // Register event handlers
            _device.ConnectionStatusChanged += OnConnectionStatusChanged;
            _device.NameChanged += OnNameChanged;
            _device.GattServicesChanged += OnGattervicesChanged; 

            // Register for notifications from the device, on a separate thread
            //
            // NOTE:
            // This has the effect of telling the OS that we're interested in
            // these devices, and for it to automatically connect to them when
            // they are advertising.
            Utilities.RunFuncAsTask(RegisterNotificationsAsync);
        }
        #endregion

        #region ---------------------------- Event Handlers ----------------------------
        /// <summary>
        /// NameChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void OnNameChanged(BluetoothLEDevice sender, Object obj)
        {
            SignalChanged("Name");
        }

        /// <summary>
        /// GattServicesChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void OnGattervicesChanged(BluetoothLEDevice sender, Object obj)
        {
            Utilities.MakeAlertBox("Services on '" + Name + "' has changed! Please navigate back to the main page and refresh devices if you would like to update the device.");

            // Slightly hacky way of making sure that 1) nothing breaks if services/characteristics of this device are currently
            // being viewed while 2) ensuring that everything gets refreshed properly upon pressing the button on the main page. 
            if (GlobalSettings.PairedDevices.Contains(this))
            {
                GlobalSettings.PairedDevices.Remove(this);
            }
        }

        /// <summary>
        /// ConnectionStatusChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void OnConnectionStatusChanged(BluetoothLEDevice sender, Object obj)
        {
            bool value = false; 
            if (_device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                value = true;
            }
            if (value != Connected)
            {
                // Change internal boolean and signal UI
                Connected = value;
                SignalChanged("ConnectString");
                SignalChanged("ConnectColor");
            }
        }
        #endregion // event handlers

        #region ---------------------------- Registering Notifications ----------------------------
        /// <summary>
        /// Registers notifications for all characteristics in all services in this device
        /// </summary>
        private bool _notificationsRegistered;
        public async Task RegisterNotificationsAsync()
        {
            // Don't need to register notifications multiple times. 
            if (_notificationsRegistered)
            {
                return; 
            }

            foreach (var serviceM in ServiceModels)
            {
                await serviceM.RegisterNotificationsAsync();
            }

            // Notifications now registered.
            _notificationsRegistered = true; 
        }

        /// <summary>
        /// Unregisters notifications for all characteristics in all services in this devices
        /// </summary>
        /// <returns></returns>
        public async Task UnregisterNotificationsAsync()
        {
            try 
            {
                foreach (var serviceM in ServiceModels)
                {
                    await serviceM.UnregisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                // There's a chance the unregister will fail, as the device has been removed.
                Utilities.OnExceptionWithMessage(ex, "This failure may be expected as we're trying to unregister a device upon removal.");
            }

            _notificationsRegistered = false; 
        }
        #endregion // registering notifications
    }
}
