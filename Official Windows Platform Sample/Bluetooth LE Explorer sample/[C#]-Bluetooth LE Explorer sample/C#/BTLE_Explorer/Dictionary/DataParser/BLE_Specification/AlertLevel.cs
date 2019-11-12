using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class AlertLevel
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_level.xml
        public enum AlertLevelEnum
        {
            NoAlert = 0,
            MildAlert = 1, 
            HighAlert = 2,
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            string categoryName;
            if (result < 3)
            {
                categoryName = ((AlertLevelEnum)result).ToString();
            }
            else
            {
                categoryName = "Reserved for future use";
            }
            return String.Format("{0} ({1})", result, categoryName);
        }
    }
}
