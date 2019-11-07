using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BTLE_Explorer.Extras
{
    /// <summary>
    /// Texas Instrument SensorTag Gatt attribute Uuids
    /// 
    /// ATTRIBUTION
    /// "TI SensorTag User Guide" by Texas Instruments, used under CC BY-SA 3.0 US / Desaturated from original
    /// http://processors.wiki.ti.com/index.php/SensorTag_User_Guide
    /// </summary>
    public static class TI_BLESensorTagGattUuids
    {
        private const string TI_BASE_UUID_START = "F000";
        private const string TI_BASE_UUID_END = "-0451-4000-B000-000000000000";

        // Makes a TI UUID by replacing the appropriate string
        private static Guid MakeTIUuid(string uuid)
        {
            return new Guid(TI_BASE_UUID_START + uuid + TI_BASE_UUID_END);    
        }

        // Characteristic UUIDs for TI Sensor Tag. (See http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Gatt_Server)
        public static class TISensorTagCharacteristicUUIDs
        {
            public static Guid IRTemperature_Data { get { return MakeTIUuid("AA01"); } }
            public static Guid IRTemperature_Config { get { return MakeTIUuid("AA02"); } }
            public static Guid IRTemperature_Period { get { return MakeTIUuid("AA03"); } }
            public static Guid Accelerometer_Data { get { return MakeTIUuid("AA11"); } }
            public static Guid Accelerometer_Config { get { return MakeTIUuid("AA12"); } }
            public static Guid Accelerometer_Period { get { return MakeTIUuid("AA13"); } }
            public static Guid Humidity_Data { get { return MakeTIUuid("AA21"); } }
            public static Guid Humidity_Config { get { return MakeTIUuid("AA22"); } }
            public static Guid Humidity_Period { get { return MakeTIUuid("AA23"); } }
            public static Guid Magnetometer_Data { get { return MakeTIUuid("AA31"); } }
            public static Guid Magnetometer_Config { get { return MakeTIUuid("AA32"); } }
            public static Guid Magnetometer_Period { get { return MakeTIUuid("AA33"); } }
            public static Guid Barometer_Data { get { return MakeTIUuid("AA41"); } }
            public static Guid Barometer_Config { get { return MakeTIUuid("AA42"); } }
            public static Guid Barometer_Calibration { get { return MakeTIUuid("AA43"); } }
            public static Guid Barometer_Period { get { return MakeTIUuid("AA44"); } }
            public static Guid Gyroscope_Data { get { return MakeTIUuid("AA51"); } }
            public static Guid Gyroscope_Config { get { return MakeTIUuid("AA52"); } }
            public static Guid Gyroscope_Period { get { return MakeTIUuid("AA53"); } }
            public static Guid Test_Data { get { return MakeTIUuid("AA61"); } }
            public static Guid Test_Config { get { return MakeTIUuid("AA62"); } }
            public static Guid ConnectionParameters { get { return MakeTIUuid("CCC1"); } }
            public static Guid OadImage_Identify { get { return MakeTIUuid("FFc1"); } }
            public static Guid OadImage_Block { get { return MakeTIUuid("FFC2"); } }
        }

        public static class TISensorTagServiceUUIDs
        {
            public static Guid IRTemperature { get { return MakeTIUuid("AA00"); } }
            public static Guid Accelerometer { get { return MakeTIUuid("AA10"); } }
            public static Guid Humidity { get { return MakeTIUuid("AA20"); } }
            public static Guid Magnetometer { get { return MakeTIUuid("AA30"); } }
            public static Guid Barometer { get { return MakeTIUuid("AA40"); } }
            public static Guid Gyroscope { get { return MakeTIUuid("AA50"); } }
            public static Guid SensorTagSelfTest { get { return MakeTIUuid("AA60"); } }  
            public static Guid ConnectionControl { get { return MakeTIUuid("CCC0"); } }
            public static Guid OAD { get { return MakeTIUuid("FFc0"); } }
        }
    }
}
