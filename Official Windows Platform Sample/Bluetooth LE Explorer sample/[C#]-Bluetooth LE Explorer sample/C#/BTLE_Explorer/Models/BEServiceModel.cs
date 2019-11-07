using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

using BTLE_Explorer.Dictionary;
using BTLE_Explorer.ViewModels;

namespace BTLE_Explorer.Models
{
    /// <summary>
    /// A model class to handle data manipulations. Manipulations to this class will push
    /// changes to the corresponding view model instances, which is bound to the UI.
    /// 
    /// This model is a wrapper around the GattDeviceService class.
    /// </summary>
    public class BEServiceModel : BEGattModelBase<GattDeviceService>
    {
        #region ---------------------------- Properties ----------------------------
        private GattDeviceService _service { get; set; }
        public BEDeviceModel DeviceM { get; private set; }
        public List<BECharacteristicModel> CharacteristicModels { get; private set; }
        #region name
        
        private string _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;

            }
        }
        #endregion
        
        public Guid Uuid { get; private set; }
        public bool Mandatory { get; private set; }
        public bool Default { get; private set; }
        public bool Toastable { get; private set; }
        public bool Writable { get; private set; }
        #endregion

        #region ---------------------------- Constructor/Initialize ----------------------------
        public BEServiceModel()
        {
            this.Name = ServiceDictionaryEntry.SERVICE_MISSING_STRING;
            CharacteristicModels = new List<BECharacteristicModel>();
            this._viewModelInstances = new List<BEGattVMBase<GattDeviceService>>(); 
        }
        
        public override string ToString()
        {
            return this.Name;
        }
        
        /// <summary>
        /// Initialization of the serivce model
        /// </summary>
        /// <param name="service"></param>
        /// <param name="deviceM"></param>
        public void Initialize(GattDeviceService service, BEDeviceModel deviceM)
        {
            // Check for valid input
            if (service == null)
            {
                throw new ArgumentNullException("In BEServiceModel, GattDeviceService cannot be null");
            }
            if (deviceM == null)
            {
                throw new ArgumentNullException("In BEServiceModel, BEDeviceModel cannot be null");
            }
            
            // Initialize basics
            _service = service;
            this.Uuid = _service.Uuid;
            DeviceM = deviceM; 
            GetDictionaryAndUpdateProperties();
            DetermineProperties();

            // Initialize characteristics
            InitializeCharacteristics();
        }

        /// <summary>
        /// Initialize the list of all characteristics in this service.
        /// </summary>
        public void InitializeCharacteristics()
        {
            // Don't need to initialize multiple times. 
            if (CharacteristicModels.Count > 0)
            {
                return;
            }

            // Get characteristics. 
            try
            {
                IReadOnlyList<GattCharacteristic> characteristics = _service.GetAllCharacteristics();
                foreach (GattCharacteristic characteristic in characteristics)
                {
                    BECharacteristicModel characteristicM = new BECharacteristicModel();
                    characteristicM.Initialize(this, characteristic);
                    CharacteristicModels.Add(characteristicM);
                }
            }
            catch (Exception ex)
            {
                // GetAllCharacteristics can fail with E_ACCESS_DENIED if another app is holding a
                // reference to the BTLE service.  It can be an active background task, or in the
                // backstack.
                Utilities.OnExceptionWithMessage(ex, "This exception may be encountered if a another app holds a reference to the BTLE service.");
            }
        }

        /// <summary>
        /// Read all characteristics.
        /// </summary>
        /// <returns></returns>
        public async Task ReadCharacteristicsAsync()
        {
            foreach (var model in CharacteristicModels)
            {
                await model.ReadValueAsync();
            }
        }

        /// <summary>
        /// Check if this service has any members with toastable values.
        /// </summary>
        private void DetermineProperties()
        {
            try
            {
                IReadOnlyList<GattCharacteristic> characteristics = _service.GetAllCharacteristics();
                foreach (GattCharacteristic characteristic in characteristics)
                {
                    Toastable |= ((characteristic.CharacteristicProperties & GattCharacteristicProperties.Notify) != 0);
                    Writable |= ((characteristic.CharacteristicProperties & GattCharacteristicProperties.WriteWithoutResponse) != 0);
                    Writable |= ((characteristic.CharacteristicProperties & GattCharacteristicProperties.Write) != 0);
                }
            }
            catch (Exception ex)
            {
                // GetAllCharacteristics can fail with E_ACCESS_DENIED if another app is holding a
                // reference to the BTLE service.  It can be an active background task, or in the
                // backstack.
                Utilities.OnExceptionWithMessage(ex, "This exception may be encountered if a another app holds a reference to the BTLE service.");
            }
        }
        #endregion

        #region ---------------------------- Dictionary ----------------------------
        private ServiceDictionaryEntry _dictionaryEntry;

        /// <summary>
        /// Looks up the dictionary entry corresponding to this characteristic, to determine the
        /// type of parsers needed, for example.
        /// </summary>
         private void GetDictionaryAndUpdateProperties()
        {
            GetDictionaryEntry();
            UpdatePropertiesFromDictionaryEntry();
        }

        /// <summary>
        /// Gets dictionary entry if it exists. Otherwise, creates a new entry and adds it to the Unknown dictionary, then gets.
        /// </summary>
        private void GetDictionaryEntry()
        {
            if (GlobalSettings.ServiceDictionaryConstant.ContainsKey(Uuid))
            {
                _dictionaryEntry = GlobalSettings.ServiceDictionaryConstant[Uuid];
            }
            else if (GlobalSettings.ServiceDictionaryUnknown.ContainsKey(Uuid))
            {
                _dictionaryEntry = GlobalSettings.ServiceDictionaryUnknown[Uuid];
            }
            else
            {
                _dictionaryEntry = new ServiceDictionaryEntry();
                _dictionaryEntry.Initialize(Uuid); 
                GlobalSettings.ServiceDictionaryUnknown.Add(Uuid, _dictionaryEntry);
            }
        }

        /// <summary>
        /// Updates properties on this class based on dictionary entry
        /// </summary>
        private void UpdatePropertiesFromDictionaryEntry()
        {
            this.Name = _dictionaryEntry.Name;
            this.Default = _dictionaryEntry.IsDefault; 
        }
        #endregion

        #region ---------------------------- Changing properties ----------------------------
        /// <summary>
        /// Updates the name of the service
        /// </summary>
        /// <param name="name"></param>
        public void UpdateName(string name)
        {
            Name = name;
            SignalChanged("Name");
            _dictionaryEntry.ChangeFriendlyName(name); 
        }
        
        public bool DictionaryModelChanged
        {
            get
            {
                return _dictionaryEntry.HasChanged();
            }
        }
        #endregion

        #region ---------------------------- Registering Notifications ----------------------------
        /// <summary>
        /// Iterates through all characteristics in this service and registers for notifications
        /// from them
        /// </summary>
        /// <returns></returns>
        public async Task RegisterNotificationsAsync()
        {
            foreach (var characteristicM in CharacteristicModels)
            {
                await characteristicM.RegisterNotificationAsync(); 
            }
        }

        /// <summary>
        /// Unregisters for notifications from all characteristics in the service
        /// </summary>
        /// <returns></returns>
        public async Task UnregisterNotificationsAsync()
        {
            foreach (var characteristicM in CharacteristicModels)
            {
                await characteristicM.UnregisterNotificationAsync();
            }
        }
        #endregion // registering notifications
    }
}
