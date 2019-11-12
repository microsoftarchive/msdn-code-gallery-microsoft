namespace KeepTheKeysCommon
{
    // Values for GATT Alert Level characteristic.
    // Reference:  https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_level.xml
    public enum AlertLevel : byte
    {
        None = 0,
        Mild = 1,
        High = 2,
    }
}
