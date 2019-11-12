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
}
