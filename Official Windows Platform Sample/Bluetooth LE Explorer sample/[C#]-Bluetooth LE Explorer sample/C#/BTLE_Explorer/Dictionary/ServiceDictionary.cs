using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Popups;

namespace BTLE_Explorer.Dictionary
{
    public class ServiceDictionary : DictionaryBase<ServiceDictionaryEntry>
    {
        private const string SERVICE_DICTIONARY_WRITTEN_FILE_NAME = "ServiceDictionary";

        private void AddAndCreateNewEntry(Guid uuid, String name, bool isDefault = false)
        {
            // Error handle duplicates
            if (this.ContainsKey(uuid))
            {
                Utilities.OnException(new ArgumentException("In ServiceDictionary.AddAndCreateNewEntry: Attempted to add Uuid which exists."));
                return; 
            }

            // Add in new value. 
            ServiceDictionaryEntry entry = new ServiceDictionaryEntry();
            entry.Initialize(uuid, name, isDefault);
            this.Add(uuid, entry);
        }

        // Initialize the dictionary with values for which names will not be changing. 
        public void InitAsConstant()
        {
            // Automatically add all UIUDs defined in GattServiceUuids to the system. (Uses System.Reflection)

            // MSDN defined services of the Bluetooth specification
            var properties = typeof(GattServiceUuids).GetRuntimeProperties();
            foreach (PropertyInfo prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }

            // TI Sensor tag services
            properties = typeof(BTLE_Explorer.Extras.TI_BLESensorTagGattUuids.TISensorTagServiceUUIDs).GetRuntimeProperties();
            foreach (PropertyInfo prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }
        }

        public override async Task LoadDictionaryAsync()
        {
            await ReadFileAndDeserializeIfExistsAsync(SERVICE_DICTIONARY_WRITTEN_FILE_NAME); 
        }

        public override async Task SaveDictionaryAsync()
        {
            await SerializeAndWriteFileAsync(SERVICE_DICTIONARY_WRITTEN_FILE_NAME); 
        }

        public override void AddLoadedEntry(ServiceDictionaryEntry input)
        {
            this.Add(input.Uuid, input);
        }
    }
}
