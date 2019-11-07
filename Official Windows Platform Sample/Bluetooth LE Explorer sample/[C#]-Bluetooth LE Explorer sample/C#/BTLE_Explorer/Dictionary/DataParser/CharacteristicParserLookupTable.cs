using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

using TIUuids = BTLE_Explorer.Extras.TI_BLESensorTagGattUuids.TISensorTagCharacteristicUUIDs;
using TIParsers = BTLE_Explorer.Extras.TI_BLESensorTagCharacteristicParsers;

namespace BTLE_Explorer.Dictionary.DataParser
{
    public class CharacteristicParserLookupTable : Dictionary<Guid, Func<IBuffer, string>>
    {
        // Contains the default string parsers for the characteristics. 
        public CharacteristicParserLookupTable()
        {
            /* WinRT implemented BTLE specification characteristics */
            this.Add(GattCharacteristicUuids.AlertCategoryId, BLE_Specification.AlertCategoryId.ParseBuffer);
            this.Add(GattCharacteristicUuids.AlertCategoryIdBitMask, BLE_Specification.AlertCategoryIdBitMask.ParseBuffer);
            this.Add(GattCharacteristicUuids.AlertLevel, BLE_Specification.AlertLevel.ParseBuffer);
            this.Add(GattCharacteristicUuids.AlertNotificationControlPoint, BLE_Specification.AlertNotificationControlPoint.ParseBuffer);
            this.Add(GattCharacteristicUuids.AlertStatus, BLE_Specification.AlertStatus.ParseBuffer);
            this.Add(GattCharacteristicUuids.BatteryLevel, BasicParsers.ParseUInt8);
            this.Add(GattCharacteristicUuids.BloodPressureFeature, BLE_Specification.BloodPressureFeature.ParseBuffer);
            this.Add(GattCharacteristicUuids.BodySensorLocation, BLE_Specification.BodySensorLocation.ParseBuffer);
            this.Add(GattCharacteristicUuids.BootKeyboardInputReport, BasicParsers.ParseUInt8Multi);
            this.Add(GattCharacteristicUuids.BootKeyboardOutputReport, BasicParsers.ParseUInt8Multi);
            this.Add(GattCharacteristicUuids.BootMouseInputReport, BasicParsers.ParseUInt8Multi);
            this.Add(GattCharacteristicUuids.CscFeature, BLE_Specification.CscFeature.ParseBuffer);
            this.Add(GattCharacteristicUuids.DayOfWeek, BLE_Specification.DayOfWeek.ParseBuffer);
            this.Add(GattCharacteristicUuids.GapDeviceName, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.HardwareRevisionString, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.HeartRateMeasurement, BLE_Specification.HeartRateMeasurement.ParseBuffer);
            this.Add(GattCharacteristicUuids.ManufacturerNameString, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.ModelNumberString, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.SerialNumberString, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.SoftwareRevisionString, BasicParsers.ParseString);
            this.Add(GattCharacteristicUuids.FirmwareRevisionString, BasicParsers.ParseString);

            
            /* TI Characteristics */
            this.Add(TIUuids.IRTemperature_Data, TIParsers.Parse_Temperature_Data);
            this.Add(TIUuids.IRTemperature_Config, TIParsers.Parse_Temperature_Configuration);
            this.Add(TIUuids.IRTemperature_Period, TIParsers.Parse_Temperature_Period);

            this.Add(TIUuids.Accelerometer_Data, TIParsers.Parse_Accelerometer_Data);
            this.Add(TIUuids.Accelerometer_Config, TIParsers.Parse_Accelerometer_Configuration);
            this.Add(TIUuids.Accelerometer_Period, TIParsers.Parse_Accelerometer_Period);

            this.Add(TIUuids.Humidity_Data, TIParsers.Parse_Humidity_Data);
            this.Add(TIUuids.Humidity_Config, TIParsers.Parse_Humidity_Configuration);
            this.Add(TIUuids.Humidity_Period, TIParsers.Parse_Humidity_Period);

            this.Add(TIUuids.Magnetometer_Data, TIParsers.Parse_Magnetometer_Data);
            this.Add(TIUuids.Magnetometer_Config, TIParsers.Parse_Magnetometer_Configuration);
            this.Add(TIUuids.Magnetometer_Period, TIParsers.Parse_Magnetometer_Period);

            this.Add(TIUuids.Barometer_Data, TIParsers.Parse_Barometer_Data);
            this.Add(TIUuids.Barometer_Config, TIParsers.Parse_Barometer_Configuration);
            this.Add(TIUuids.Barometer_Calibration, TIParsers.Parse_Barometer_Calibration);
            this.Add(TIUuids.Barometer_Period, TIParsers.Parse_Barometer_Period);

            this.Add(TIUuids.Gyroscope_Data, TIParsers.Parse_Gyroscope_Data);
            this.Add(TIUuids.Gyroscope_Config, TIParsers.Parse_Gyroscope_Configuration);
            this.Add(TIUuids.Gyroscope_Period, TIParsers.Parse_Gyroscope_Period);
            

            /*
             * FUTURE
             * 
             * Add parsers for the rest of the Bluetooth-SIG defined characteristics
             * 
             */
        }
    }
}
