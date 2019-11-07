//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;

namespace UsbCdcControl
{
    internal class Util
    {
        internal static int CompareTo(Windows.Storage.Streams.IBuffer buffer, Windows.Storage.Streams.IBuffer otherBuffer)
        {
            var array = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);
            var otherArray = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(otherBuffer);

            if (array.Length != otherArray.Length)
            {
                return array.Length < otherArray.Length ? -1 : 1;
            }

            for (int index = 0; index < array.Length; index++)
            {
                if (array[index] != otherArray[index])
                {
                    return array[index] < otherArray[index] ? -1 : 1;
                }
            }

            return 0;
        }

        internal static String AsciiBufferToAsciiString(Windows.Storage.Streams.IBuffer buffer)
        {
            var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
            var srcBytes = new Byte[buffer.Length];
            reader.ReadBytes(srcBytes);

            var dstChars = new Char[buffer.Length];
            var decoder = System.Text.Encoding.UTF8.GetDecoder();
            int bytesUsed, charsUsed;
            bool completed;
            decoder.Convert(srcBytes, 0, srcBytes.Length, dstChars, 0, dstChars.Length, true, out bytesUsed, out charsUsed, out completed);

            return new String(dstChars);
        }

        internal static String BinaryBufferToBinaryString(Windows.Storage.Streams.IBuffer buffer)
        {
            var str = "";
            var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
            for (uint i = 0; i < buffer.Length; i++)
            {
                str += reader.ReadByte().ToString("X2") + " ";
            }
            return str;
        }

        internal static void GotoEndPosTextBox(Windows.UI.Xaml.Controls.TextBox textBox)
        {
            var count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(textBox);
            for (int i = 0; i < count; i++)
            {
                var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(textBox, i);
                var morecount = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(child);
                for (int j = 0; j < morecount; j++)
                {
                    var grandchild = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(child, j);
                    var scroll = grandchild as Windows.UI.Xaml.Controls.ScrollViewer;
                    if (scroll != null)
                    {
                        scroll.ChangeView(null, scroll.ExtentHeight, null);
                        return;
                    }
                }
            }
        }

        internal static Windows.UI.Xaml.Controls.Control FindControl(Windows.UI.Xaml.DependencyObject root, String name)
        {
            var count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(root, i);
                var childControl = child as Windows.UI.Xaml.Controls.Control;
                if (childControl != null)
                {
                    if (childControl.Name == name)
                    {
                        return childControl;
                    }
                }
                Windows.UI.Xaml.Controls.Control found = FindControl(child, name);
                if (found != null && found.Name == name)
                {
                    return found;
                }
            }
            return null;
        }
    }

    internal sealed class UsbSerialPortInfo
    {
        public UsbSerialPortInfo(UsbCdcControl.UsbCdcControlAccess.UsbSerialPort port, String id, String name)
        {
            this.Port = port;
            this.DeviceId = id;
            this.Name = name;
        }

        public UsbCdcControl.UsbCdcControlAccess.UsbSerialPort Port
        {
            get;
            set;
        }

        public String DeviceId
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }
    };

    /// <summary>
    /// This holds UsbSerialPort instance(s) between scenarios.
    /// </summary>
    internal sealed class UsbDeviceList : System.Collections.Generic.List<UsbSerialPortInfo>
    {
        internal delegate Windows.Foundation.IAsyncAction DeviceRemovedHandler(object sender, UsbCdcControl.UsbDeviceInfo e);
        internal event EventHandler<UsbCdcControl.UsbDeviceInfo> DeviceAdded;
        internal event DeviceRemovedHandler DeviceRemoved;

        private static UsbDeviceList singleton = null;
        internal static UsbDeviceList Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new UsbDeviceList();
                }
                return singleton;
            }
        }

        /// <summary>
        /// Constructor.
        /// Queries all the supported devices
        /// </summary>
        private UsbDeviceList()
        {
            foreach (var supportedDevice in Constants.SupportedDevices)
            {
                var deviceListForSupportedDevice = new UsbCdcControl.DeviceList(Windows.Devices.Usb.UsbDevice.GetDeviceSelector(supportedDevice.Vid, supportedDevice.Pid));
                deviceListForSupportedDevice.DeviceAdded += this.OnDeviceAdded;
                deviceListForSupportedDevice.DeviceRemoved += this.OnDeviceRemoved;
            }
        }

        ~UsbDeviceList()
        {
            foreach (var deviceList in UsbCdcControl.DeviceList.Instances)
            {
                deviceList.StopWatcher();
                deviceList.DeviceAdded -= this.OnDeviceAdded;
                deviceList.DeviceRemoved -= this.OnDeviceRemoved;
            }
        }

        internal void StartWatcher()
        {
            foreach (var deviceList in UsbCdcControl.DeviceList.Instances)
            {
                if (!deviceList.WatcherStarted)
                {
                    deviceList.StartWatcher();
                }
            }
        }

        private void OnDeviceAdded(object sender, UsbCdcControl.UsbDeviceInfo info)
        {
            if (DeviceAdded != null)
            {
                DeviceAdded(sender, info);
            }
        }

        private void OnDeviceRemoved(object sender, UsbCdcControl.UsbDeviceInfo info)
        {
            if (DeviceRemoved != null)
            {
                DeviceRemoved(sender, info).Completed = new Windows.Foundation.AsyncActionCompletedHandler((action, status) =>
                {
                    this.DisposeDevice(info.Id);
                });
            }
            else
            {
                this.DisposeDevice(info.Id);
            }
        }

        /// <summary>
        /// Call Dispose() for a UsbDevice.
        /// </summary>
        internal void DisposeDevice(String deviceId)
        {
            var info = this.Find((i) => { return i.DeviceId == deviceId; });
            if (info != null)
            {
                info.Port.UsbDevice.Dispose();
                this.Remove(info);
            }
        }

        /// <summary>
        /// Call Dispose() for each UsbDevice, and call List.Clear().
        /// </summary>
        internal void DisposeAll()
        {
            foreach (var portInfo in this)
            {
                portInfo.Port.UsbDevice.Dispose();
            }
            this.Clear();
        }
    }

    /// <summary>
    /// Base page class, which uses just one device.
    /// </summary>
    internal class SingleDevicePage : SDKTemplate.Common.LayoutAwarePage
    {
        /// <summary>
        /// ComboBoxItem for device selection.
        /// </summary>
        internal sealed class UsbDeviceComboBoxItem : Windows.UI.Xaml.Controls.ComboBoxItem
        {
            public UsbDeviceComboBoxItem(String id, Object content)
            {
                this.id = id;
                this.Content = content;
            }

            public String Id
            {
                get
                {
                    return this.id;
                }
            }

            private String id;
        }

        protected UsbSerialPortInfo SerialPortInfo
        {
            get
            {
                if (UsbDeviceList.Singleton.Count > 0)
                {
                    return UsbDeviceList.Singleton[0];
                }
                return null;
            }
        }
    }
}