using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class AlertStatus
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_status.xml
        public enum Status
        {
            RingerStateActive = 1 << 0,
            VibrateStateActive = 1 << 1, 
            DisplayAlertStatus = 1 << 2,
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            return BasicParsers.FlagsSetInByte(typeof(Status), result);
        }
    }
}
