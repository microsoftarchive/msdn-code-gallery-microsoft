//
// SpbAccelerometer device node
//
Device(SPBA)
{
    Name(_ADR, 0)
    Name(_HID, "SpbAccelerometer")
    Name(_CID, "SpbAccelerometer")
    Name(_UID, 1)
    Method(_CRS, 0x0, NotSerialized)
    {
        Name(RBUF, ResourceTemplate()
        {
            //
            // Sample I2C and GPIO resources. Modify to match your
            // platform's underlying controllers and connections.
            // \_SB.I2C and \_SB.GPIO are paths to predefined I2C 
            // and GPIO controller instances. 
            //
            // Note: as written SpbAccelerometer requires a GPIO resource.
            //
            I2CSerialBus(0x1D, ControllerInitiated, 400000, AddressingMode7Bit, "\\_SB.I2C", , )
            GpioInt(Level, ActiveHigh, Exclusive, PullDown, 0, "\\_SB.GPIO") {1}
        })
        Return(RBUF)
    }
    Method(_DSM, 0x4, NotSerialized)
    {
        //
        // Device specific method used to query
        // configuration data. See ACPI 5.0 specification
        // for further details.
        //
        If(LEqual(Arg0, Buffer(0x10)
        {
            //
            // UUID: {7681541E-8827-4239-8D9D-36BE7FE12542}
            //
            0x1e, 0x54, 0x81, 0x76, 0x27, 0x88, 0x39, 0x42, 0x8d, 0x9d, 0x36, 0xbe, 0x7f, 0xe1, 0x25, 0x42
        }))
        {
            If(LEqual(Arg2, Zero))
            {
                Return(Buffer(One)
                {
                    0x03
                })
            }
            If(LEqual(Arg2, One))
            {
                //
                // Sample configuration data. This format is not
                // defined and should be modified to fit the needs
                // of the driver and part.
                //
                Return(Buffer(0x4)
                {
                    0x00, 0x01, 0x02, 0x03
                })
            }
        }
        Else
        {
            Return(Buffer(One)
            {
                0x00
            })
        }
    }
}
