using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser.BLE_Specification
{
    public static class DayOfWeek
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.day_of_week.xml
        public enum Status
        {
            Unknown = 0,
            Monday = 1,
            Tuseday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7,
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            string categoryName;
            if (result < Enum.GetNames(typeof(Status)).Length)
            {
                categoryName = ((Status)result).ToString();
            }
            else
            {
                categoryName = "Reserved for future use";
            }
            return String.Format("{0} ({1})", result, categoryName);
        }
    }
}
