using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

using BTLE_Explorer.Dictionary.DataParser; 

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class HeartRateMeasurement
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.heart_rate_measurement.xml
        public enum HeartRateMeasurementFlags : byte
        {
            TwoByteFormatFlag = (1 << 0),
            SkinContactSupportedFlag = (1 << 1),
            SkinContactDetectedFlag = (1 << 2),
            EnergyExpendedStatusFlag = (1 << 3),
            RRIntervalFlag = (1 << 4)
        };

        public static UInt16 HeartRate { get; private set; }
        public static UInt16 EnergyExpended { get; private set; }
        public static UInt16 RRInterval { get; private set; }
        public static byte Flags { get; private set; }

        public static string ParseBuffer(IBuffer input)
        {
            HeartRate = 0;
            RRInterval = 0;
            EnergyExpended = 0;
            Flags = 0;

            DataReader reader = DataReader.FromBuffer(input);
            Flags = reader.ReadByte();

            // Get heart rate measurement
            if ((Flags & (byte)HeartRateMeasurementFlags.TwoByteFormatFlag) != 0)
            {
                HeartRate = reader.ReadUInt16();
            }
            else
            {
                HeartRate = reader.ReadByte();
            }

            // Get Energy Expended Status, if present
            if (HasEnergyExpendedField)
            {
                EnergyExpended = reader.ReadUInt16();
            }

            // Get RRInterval, if present
            if (HasRRIntervalField)
            {
                RRInterval = reader.ReadUInt16();
            }

            string result = "";
            result += string.Format("\nHeart Rate [{0}]\nSkinContactSupported [{1}]",
                HeartRate,
                SensorContactSupported.ToString());

            if (SensorContactSupported)
            {
                result += string.Format("\nSkinContactDetected [{0}]",
                    SensorContactDetected.ToString());
            }
            if (HasEnergyExpendedField)
            {
                result += string.Format("\nEnergy Expended [{0}]", EnergyExpended);
            }
            if (HasRRIntervalField)
            {
                result += string.Format("\nRRInterval [{0}]", RRInterval);
            }
            return result;
        }

        public static bool SensorContactDetected
        {
            get
            {
                return ((Flags & (byte)HeartRateMeasurementFlags.SkinContactSupportedFlag) != 0) &&
                       ((Flags & (byte)HeartRateMeasurementFlags.SkinContactDetectedFlag) != 0);
            }
        }

        public static bool SensorContactSupported
        {
            get
            {
                return SensorContactDetected &
                    ((Flags & (byte)HeartRateMeasurementFlags.SkinContactSupportedFlag) != 0);
            }
        }

        public static bool HasRRIntervalField
        {
            get
            {
                return ((Flags & (byte)HeartRateMeasurementFlags.RRIntervalFlag) != 0);
            }
        }
        
        public static bool HasEnergyExpendedField
        {
            get
            {
                return ((Flags & (byte)HeartRateMeasurementFlags.EnergyExpendedStatusFlag) != 0);
            }
        }
    }
}
