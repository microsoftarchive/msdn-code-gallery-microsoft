using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BTLE_Explorer.Dictionary.DataParser
{
    // A class which includes methods for parsing a buffer in a few generic ways.
    // (String, uint8, etc)
    public static class BasicParsers
    {
        #region --------------- Parse as some type of integer ---------------
        #region --------------- Parse as UInt8
        // Single
        public static string ParseUInt8(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            return Convert.ToString(result);
        }

        // Multiple
        public static string ParseUInt8Multi(IBuffer buffer)
        {
            byte[] bytes = ReadBufferToBytes(buffer);
            string result = "";
            foreach (byte b in bytes) {
                result += Convert.ToString(b); 
            }
            return result; 
        }
        #endregion // Parse as UInt8

        #region --------------- Parse as UInt16
        public static string ParseUInt16Multi(IBuffer buffer)
        {
            byte[] bytes = ReadBufferToBytes(buffer);
            string hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "");
        }
        #endregion

        #endregion // Parse as some type of integer

        #region --------------- Parse as string ---------------
        public static string ParseString(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            return reader.ReadString(buffer.Length);
        }
        #endregion // Parse as string

        #region --------------- Helper Functions ---------------

        // Gets a byte array from a buffer
        public static byte[] ReadBufferToBytes(IBuffer buffer)
        {
            uint dataLength = buffer.Length;
            byte[] data = new byte[dataLength];
            DataReader reader = DataReader.FromBuffer(buffer);
            reader.ReadBytes(data);
            return data;
        }

        // Given an enum that represents a bit field and a byte, prints out a 
        // string which includes the name of each enum value and whether or not
        // that bit is set.
        // This function assumes that the first set of values are contiguous. 
        public static string FlagsSetInByte(Type type, byte b)
        {
            string result = ""; 
            
            foreach (string categoryName in Enum.GetNames(type))
            {
                byte categoryValue = (byte)Enum.Parse(type, categoryName);
                bool present = ((b & categoryValue) > 0);
                result += String.Format("\n{0} [{1}]", categoryName, present.ToString());
            }
            return result; 
        }

        #endregion // Helper Functions
    }
}
