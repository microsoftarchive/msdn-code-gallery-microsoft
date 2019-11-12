using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Custom;

namespace CustomDeviceAccess
{
    static class Fx2Driver
    {
        // The device type value expected by the driver in IO control 
        // codes.  The driver defines this as 0x65500 but the value gets
        // truncated to a ushort.  Do the same here.
        public const ushort DeviceType = unchecked((ushort)0x65500);
        public const ushort FunctionBase = 0x800;

        public static
        IOControlCode GetSevenSegmentDisplay = new IOControlCode(DeviceType,
                                                                 FunctionBase + 7,
                                                                 IOControlAccessMode.Read,
                                                                 IOControlBufferingMethod.Buffered);
        
        public static
        IOControlCode SetSevenSegmentDisplay = new IOControlCode(DeviceType,
                                                                 FunctionBase + 8,
                                                                 IOControlAccessMode.Write,
                                                                 IOControlBufferingMethod.Buffered);

        public static
        IOControlCode ReadSwitches = new IOControlCode(DeviceType,
                                                       FunctionBase + 6,
                                                       IOControlAccessMode.Read,
                                                       IOControlBufferingMethod.Buffered);

        public static
        IOControlCode GetInterruptMessage = new IOControlCode(DeviceType,
                                                              FunctionBase + 9,
                                                              IOControlAccessMode.Read,
                                                              IOControlBufferingMethod.DirectOutput);

        static readonly byte[] SevenSegmentValues = {
                                            0xD7, // 0
                                            0x06, // 1
                                            0xB3, // 2
                                            0xA7, // 3
                                            0x66, // 4
                                            0xE5, // 5
                                            0xF4, // 6
                                            0x07, // 7
                                            0xF7, // 8
                                            0x67, // 9
                                          };

        public static
        byte DigitToSevenSegment(byte value)
        {
            return SevenSegmentValues[value];
        }

        public static
        byte SevenSegmentToDigit(byte value)
        {
            for (byte i = 0; i < SevenSegmentValues.Length; i += 1)
            {
                if (SevenSegmentValues[i] == value)
                {
                    return i;
                }
            }
            return 0xff;
        }

        public static readonly Guid DeviceInterfaceGuid = new Guid("573E8C73-0CB4-4471-A1BF-FAB26C31D384");
    }
}
