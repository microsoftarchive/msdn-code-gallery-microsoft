using System;

//  Make it obvious which namespace provided each referenced type:
using BackgroundTaskDeferral = Windows.ApplicationModel.Background.BackgroundTaskDeferral;
using BluetoothConnectionStatus = Windows.Devices.Bluetooth.BluetoothConnectionStatus;
using BluetoothLEDevice = Windows.Devices.Bluetooth.BluetoothLEDevice;
using DeviceConnectionChangeTriggerDetails = Windows.Devices.Enumeration.DeviceConnectionChangeTriggerDetails;
using DeviceInformation = Windows.Devices.Enumeration.DeviceInformation;
using IBackgroundTask = Windows.ApplicationModel.Background.IBackgroundTask;
using KeyFob = KeepTheKeysCommon.KeyFob;
using ToastNotificationManager = Windows.UI.Notifications.ToastNotificationManager;
using ToastNotifier = Windows.UI.Notifications.ToastNotifier;
using ToastNotification = Windows.UI.Notifications.ToastNotification;
using ToastTemplateType = Windows.UI.Notifications.ToastTemplateType;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace KeepTheKeysBackground
{
    public sealed class KeyFobTask : IBackgroundTask
    {
        public async void Run(Windows.ApplicationModel.Background.IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                DeviceConnectionChangeTriggerDetails details = (DeviceConnectionChangeTriggerDetails)taskInstance.TriggerDetails;
                BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(details.DeviceId);
                KeyFob device = new KeyFob(bleDevice);

                if (bleDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    if (device.AlertOnDevice && device.HasLinkLossService)
                    {
                        await device.SetAlertLevelCharacteristic();
                    }
                }
                else
                {
                    if (device.AlertOnPhone)
                    {
                        XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                        xml.SelectSingleNode("/toast/visual/binding/text").InnerText = string.Format("Device {0} is out of range.", device.Name);
                        ToastNotification toast = new ToastNotification(xml);
                        ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();
                        notifier.Show(toast);
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
