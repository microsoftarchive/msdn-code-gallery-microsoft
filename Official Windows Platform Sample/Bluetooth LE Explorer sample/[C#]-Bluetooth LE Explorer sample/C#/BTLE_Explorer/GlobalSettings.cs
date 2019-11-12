using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth; 
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;

using BTLE_Explorer.Dictionary;
using BTLE_Explorer.Models; 
using BTLE_Explorer.ViewModels;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace BTLE_Explorer
{
    /// <summary>
    /// Global settings for the app.
    /// 
    /// Acts as a singleton for the entire app, containing the list of all devices,
    /// storing details of the currently displayed pages, as well as maintaining our
    /// list of custom service and characteristic names.
    /// 
    /// Several settings and the currently registered gatt triggers are also tracked
    /// here.
    /// </summary>
    public static class GlobalSettings
    {
        #region ----------------------------- Variables --------------------------
        // For navigation through pages
        public static BEDeviceModel SelectedDevice;
        public static BEServiceModel SelectedService;
        public static BECharacteristicModel SelectedCharacteristic; 

        // Dictionaries for keeping track of objects
        public static ServiceDictionary ServiceDictionaryConstant;
        public static ServiceDictionary ServiceDictionaryUnknown;
        public static CharacteristicDictionary CharacteristicDictionaryConstant; 
        public static CharacteristicDictionary CharacteristicDictionaryUnknown;
        public static Dictionary.DataParser.CharacteristicParserLookupTable ParserLookupTable;
        public static bool DictionariesCleared { get; private set; }

        // List of active devices
        public static List<BEDeviceModel> PairedDevices;

        // Toast and background related objects
        public static bool BackgroundAccessRequested;
        public static List<BECharacteristicModel> CharacteristicsWithActiveToast;

        // List of settings
        private const string USE_CACHED_MODE = "UseCachedMode";
        private static bool _useCachedMode = false;
        
        public static bool UseCachedMode
        {
            get
            {
                return _useCachedMode;
            } 
            set
            {
                SetUseCachedMode(value);
            }
        }
        #endregion // variables

        /// <summary>
        /// Initializes the GlobalSettings class.  Also reads the stored cache mode setting.
        /// </summary>
        public static void Initialize()
        {
            // Create objects for the objects that we need. 
            PairedDevices = new List<BEDeviceModel>();

            ParserLookupTable = new Dictionary.DataParser.CharacteristicParserLookupTable();
            ServiceDictionaryUnknown = new ServiceDictionary();
            ServiceDictionaryConstant = new ServiceDictionary();
            CharacteristicDictionaryUnknown = new CharacteristicDictionary();
            CharacteristicDictionaryConstant = new CharacteristicDictionary();
            ServiceDictionaryConstant.InitAsConstant();
            CharacteristicDictionaryConstant.InitAsConstant();
            DictionariesCleared = false;
            CharacteristicsWithActiveToast = new List<BECharacteristicModel>();

            _useCachedMode = ApplicationData.Current.LocalSettings.Values.ContainsKey(USE_CACHED_MODE);
        }

        /// <summary>
        /// Async initialization function that loads our characteristic dictionaries
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeDictionariesAsync()
        {
            await ServiceDictionaryUnknown.LoadDictionaryAsync();
            await CharacteristicDictionaryUnknown.LoadDictionaryAsync();
        }

        #region ----------------------------- Populate Device List -------------------------
        /// <summary> 
        /// Populates the device list and initializes all the various models.
        /// 
        /// Waiting for the async calls to complete takes a while, so we want to call this
        /// function somewhat sparingly.
        /// </summary>
        /// <returns></returns>
        public static async Task PopulateDeviceListAsync()
        {
            // Remove all devices and start from scratch
            PairedDevices.Clear();

            // Asynchronously get all paired/connected bluetooth devices. 
            var infoCollection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());

            // Re-add devices
            foreach (DeviceInformation info in infoCollection)
            {
                // Make sure we don't initialize duplicates
                if (PairedDevices.FindIndex(device => device.DeviceId == info.Id) >= 0)
                {
                    continue;
                }
                BluetoothLEDevice WRTDevice = await BluetoothLEDevice.FromIdAsync(info.Id);
                BEDeviceModel deviceM = new BEDeviceModel();
                deviceM.Initialize(WRTDevice, info);
                PairedDevices.Add(deviceM);
            }

            /*
             * FUTURE
             * 
             * Consider reading one characteristic from each device uncached to trigger a connection to the
             * device, in case the device does not have notifiable characteristics.
             * 
             * Also consider registering for DeviceConnectionChangeTrigger, in case a device does not have
             * notifiable characteristics.  But that may be overkill - what's the likelihood that a device
             * won't have notifiable characteristics?
             *
             */
        }
        #endregion // Populate device list

        #region ----------------------------- Notifications -------------------------
        /// <summary>
        /// Unregisters all notifications by all characteristics on all paired devices.
        /// Saves battery on the devices.
        /// </summary>
        /// <returns></returns>
        public static async Task UnregisterAllNotificationsAsync()
        {
            try
            {
                foreach (var deviceModel in PairedDevices)
                {
                    await deviceModel.UnregisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                Utilities.OnExceptionWithMessage(ex, "Hit an exception unregistering all notifications.");
            }
        }

        /// <summary>
        /// Registers for all notifications by all characteristics on all paired devices.
        /// Allows us to receive characteristic value updates, and acts as a secondary
        /// mechanism to inform the OS to auto-connect to the paired devices if they begin
        /// advertising.
        /// </summary>
        /// <returns></returns>
        public static async Task RegisterAllNotificationsAsync()
        {
            try
            {
                foreach (var deviceModel in PairedDevices)
                {
                    await deviceModel.RegisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                Utilities.OnExceptionWithMessage(ex, "Hit an exception Registering for all notifications.");
            }
        }
        #endregion // Notifications

        #region ----------------------------- Toasts and Background -------------------------
        /// <summary>
        /// Request background access so that we can register toasts.
        /// Do this once to begin with so that we don't have to do this everywhere.
        /// </summary>
        /// <returns></returns>
        public static async Task RequestBackgroundAccessAsync()
        {
            if (BackgroundAccessRequested)
            {
                return;
            }
            
            // Request access to run toast in background
            var result = await BackgroundExecutionManager.RequestAccessAsync();

            // Check if got access
            if (result == BackgroundAccessStatus.Denied)
            {
                BackgroundAccessRequested = false; 
            }
            else
            {
                BackgroundAccessRequested = true;
            }
        }

        /// <summary>
        /// An unregister all function, for those times when the toasts are getting annoying. 
        /// </summary>
        /// <returns></returns>
        public static async Task UnregisterAllToastsAsync()
        {
            // Unregister all toasts from current session
            foreach (BECharacteristicModel cm in CharacteristicsWithActiveToast)
            {
                await cm.TaskUnregisterInsideListAsync();
            }

            // Unregister all toasts from the past. 
            foreach (string key in ApplicationData.Current.LocalSettings.Containers.Keys)
            {
                if (key.StartsWith(BECharacteristicModel.TOAST_STRING_PREFIX))
                {
                    ApplicationData.Current.LocalSettings.Values.Remove(key); 
                }
            }
            ToastClear(); 
        }

        /// <summary>
        /// Simple helper to add a characteristic to be tracked as an active toast
        /// with a GattCharacteristicNotificationTrigger
        /// </summary>
        /// <param name="cm">Model representing the characteristic</param>
        public static void AddToast(BECharacteristicModel cm)
        {
            foreach (var model in CharacteristicsWithActiveToast)
            {
                if (cm.ToastEquals(model))
                {
                    return;
                }
            }
            CharacteristicsWithActiveToast.Add(cm);
        }
        
        /// <summary>
        /// Removes a characteristic from our list of characteristics with active toasts
        /// </summary>
        /// <param name="cm">Model representing the characteristic</param>
        public static void RemoveToast(BECharacteristicModel cm)
        {
            CharacteristicsWithActiveToast.Remove(cm);
        }
        
        /// <summary>
        /// Clears out the list of characteristics with active toasts
        /// </summary>
        private static void ToastClear()
        {
            CharacteristicsWithActiveToast.Clear();
        }
        #endregion // Toasts

        #region ----------------------------- Dictionary cleanup -------------------------
        /// <summary>
        /// Clear out all custom names in the dictionaries.
        /// </summary>
        /// <returns></returns>
        public static async Task ClearDictionariesAsync()
        {
            await ServiceDictionaryUnknown.ClearDictionaryAsync();
            await CharacteristicDictionaryUnknown.ClearDictionaryAsync();
            DictionariesCleared = true;
        }
        #endregion // Dictionary cleanup

        #region ----------------------------- Other settings -------------------------
        /// <summary>
        /// Sets and persists whether to use cached mode or not
        /// </summary>
        /// <param name="value"></param>
        private static void SetUseCachedMode(bool value)
        {
            if (value)
            {
                ApplicationData.Current.LocalSettings.Values[USE_CACHED_MODE] = "1";
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Remove(USE_CACHED_MODE);
            }
            _useCachedMode = value;
        }
        #endregion // Other settings
    }
}
