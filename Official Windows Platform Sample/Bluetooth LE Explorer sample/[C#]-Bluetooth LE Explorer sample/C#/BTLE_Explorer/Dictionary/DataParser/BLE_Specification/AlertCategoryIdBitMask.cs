using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class AlertCategoryIdBitMask
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_category_id_bit_mask.xml
        public enum CategoryIdBitMask0
        {
            SimpleAlert = 1 << 0,
            Email = 1 << 1, 
            News = 1 << 2,
            Call = 1 << 3,
            MissedCall = 1 << 4,
            SMS_MMS = 1 << 5,
            VoiceMail = 1 << 6,
            Schedule = 1 << 7
        }

        public enum CategoryIdBitMask1
        {
            HighPrioritizedAlert = 1,
            InstantMessage = 1 << 1
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            string result = ""; 

            DataReader reader = DataReader.FromBuffer(buffer);
            byte current = reader.ReadByte();
            result += BasicParsers.FlagsSetInByte(typeof(CategoryIdBitMask0), current);
            if (reader.UnconsumedBufferLength > 0)
            {
                current = reader.ReadByte();
                result += BasicParsers.FlagsSetInByte(typeof(CategoryIdBitMask1), current);
            }
            return result; 
        }
    }
}
