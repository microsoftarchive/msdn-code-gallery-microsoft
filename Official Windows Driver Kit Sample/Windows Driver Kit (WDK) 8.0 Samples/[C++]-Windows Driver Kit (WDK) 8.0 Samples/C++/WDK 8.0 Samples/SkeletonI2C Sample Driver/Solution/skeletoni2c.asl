//
// Test controller device node. 
//
// For a peripheral driver to access this controller 
// via SPB it must specify the ACPI device path within 
// the I2CSerialBus (or SPISerialBus) macro. Depending 
// on the scope this looks something like \_SB.I2C. See
// spbtesttool.asl for an example.
//
Device(I2C)
{
    Name(_ADR, 0)
    Name(_HID, "skeletoni2c")
    Name(_CID, "skeletoni2c")
    Name(_UID, 1)
    Method(_STA, 0x0, NotSerialized)
    {
        Return(0xf)
    }
}