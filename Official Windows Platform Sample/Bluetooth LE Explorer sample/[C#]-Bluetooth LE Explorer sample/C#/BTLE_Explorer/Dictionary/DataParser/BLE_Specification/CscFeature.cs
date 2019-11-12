using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class CscFeature
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.csc_feature.xml
        public enum Status
        {
            WheelRevolutionDataSupported = 1 << 0,
            CrankRevolutionDataSupported = 1 << 1,
            MultipleSensorLocationsSupported = 1 << 2,
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            return BasicParsers.FlagsSetInByte(typeof(Status), result);
        }
    }
}
