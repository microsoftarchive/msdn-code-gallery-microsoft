//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UsbCdcControl
{
    /// <summary>
    /// ComboBoxItem for device selection.
    /// </summary>
    internal sealed class UsbDeviceComboBoxItem : ComboBoxItem
    {
        public UsbDeviceComboBoxItem(String id, Object content)
        {
            this.id = id;
            this.Content = content;
        }

        public String DeviceId
        {
            get
            {
                return this.id;
            }
        }

        public String DeviceName
        {
            get
            {
                return this.Content as String;
            }
        }

        private readonly String id;
    }

    /// <summary>
    /// Page containing working sample code to demonstrate how to evaluate loopback CDC devices.
    /// </summary>
    public sealed partial class CdcAcmLoopback : SDKTemplate.Common.LayoutAwarePage
    {
        public CdcAcmLoopback()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState != null)
            {
                var enumerator = pageState.GetEnumerator();
                do
                {
                    var pair = enumerator.Current;
                    var control = Util.FindControl(this, pair.Key);
                    var textBox = control as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = pair.Value as String;
                        continue;
                    }
                    var comboBox = control as ComboBox;
                    if (comboBox != null)
                    {
                        if (pair.Key == this.comboBoxDevices1.Name)
                        {
                            this.previousSelectedDeviceId1 = pair.Value as String;
                        }
                        else if (pair.Key == this.comboBoxDevices2.Name)
                        {
                            this.previousSelectedDeviceId2 = pair.Value as String;
                        }
                        else
                        for (int j = 0; j < comboBox.Items.Count; j ++)
                        {
                            var comboBoxItem = comboBox.Items[j] as ComboBoxItem;
                            if (comboBoxItem.Content as String == pair.Value as String)
                            {
                                comboBox.SelectedIndex = j;
                                break;
                            }
                        }
                        continue;
                    }
                }
                while (enumerator.MoveNext());
            }

            // Dispose all devices which are used in Scenario1 to 3.
            UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();

            foreach (var deviceList in UsbCdcControl.DeviceList.Instances)
            {
                foreach (var info in deviceList.Devices)
                {
                    this.OnDeviceAdded(this, new UsbDeviceInfo(info));
                }
            }
            UsbCdcControl.UsbDeviceList.Singleton.DeviceAdded += this.OnDeviceAdded;
            UsbCdcControl.UsbDeviceList.Singleton.DeviceRemoved += this.OnDeviceRemoved;

            this.buttonLoopbackTest.IsEnabled = false;
            this.buttonStopLoopback.IsEnabled = false;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            UsbCdcControl.UsbDeviceList.Singleton.DeviceAdded -= this.OnDeviceAdded;
            UsbCdcControl.UsbDeviceList.Singleton.DeviceRemoved -= this.OnDeviceRemoved;

            // Cancel, if a read op is processing.
            this.buttonStopLoopback_Click(this, null);

            // Dispose all devices (prior to going to Scenario1 to 3).
            UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();

            if (pageState != null)
            {
                if (this.comboBoxDevices1.SelectedValue != null)
                {
                    pageState.Add(this.comboBoxDevices1.Name, (this.comboBoxDevices1.SelectedValue as UsbDeviceComboBoxItem).DeviceId);
                }
                if (this.comboBoxDevices2.SelectedValue != null)
                {
                    pageState.Add(this.comboBoxDevices2.Name, (this.comboBoxDevices2.SelectedValue as UsbDeviceComboBoxItem).DeviceId);
                }
                pageState.Add(this.textBoxDTERate.Name, this.textBoxDTERate.Text);
                pageState.Add(this.comboBoxCharFormat.Name, (this.comboBoxCharFormat.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxParityType.Name, (this.comboBoxParityType.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxDataBits.Name, (this.comboBoxDataBits.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxRTS.Name, (this.comboBoxRTS.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxDTR.Name, (this.comboBoxDTR.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
            }
        }

        private void buttonInitialize_Click(object sender, RoutedEventArgs e)
        {
            if (this.comboBoxDevices1.SelectedIndex == -1 || this.comboBoxDevices2.SelectedIndex == -1)
            {
                return;
            }

            var dispatcher = this.Dispatcher;
            var deviceId1 = (this.comboBoxDevices1.SelectedItem as UsbDeviceComboBoxItem).DeviceId;
            var deviceId2 = (this.comboBoxDevices2.SelectedItem as UsbDeviceComboBoxItem).DeviceId;
            var deviceName1 = (this.comboBoxDevices1.SelectedItem as UsbDeviceComboBoxItem).DeviceName;
            var deviceName2 = (this.comboBoxDevices2.SelectedItem as UsbDeviceComboBoxItem).DeviceName;
            var dteRate = uint.Parse(this.textBoxDTERate.Text);
            var parity = (Parity)this.comboBoxParityType.SelectedIndex;
            var dataBits = int.Parse((this.comboBoxDataBits.SelectedItem as ComboBoxItem).Content.ToString());
            var charFormat = (StopBits)this.comboBoxCharFormat.SelectedIndex;
            var dtr = this.comboBoxDTR.SelectedIndex != 0;
            var rts = this.comboBoxRTS.SelectedIndex != 0;

            var createSerialPortTasks = new List<System.Threading.Tasks.Task>();

            UsbDeviceInfo[] deviceInfos = { new UsbDeviceInfo(deviceId1, deviceName1), new UsbDeviceInfo(deviceId2, deviceName2) };
            foreach (var deviceInfo in deviceInfos)
            {
                var fromIdAsyncOp = UsbDevice.FromIdAsync(deviceInfo.Id);
                createSerialPortTasks.Add(System.Threading.Tasks.TaskExtensions.Unwrap(System.Threading.Tasks.Task.Run(async () =>
                {
                    var usbDevice = await fromIdAsyncOp;
                    var port = await UsbCdcControlAccess.UsbSerialPort.CreateAsync(usbDevice);
                    if (port == null)
                    {
                        if (usbDevice != null)
                        {
                            usbDevice.Dispose();
                        }

                        // Return a dummy task.
                        return System.Threading.Tasks.Task.Delay(0);
                    }

                    var addToListTask = dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        UsbCdcControl.UsbDeviceList.Singleton.Add(new UsbCdcControl.UsbSerialPortInfo(port, deviceInfo.Id, deviceInfo.Name));
                    }));

                    await port.Open(dteRate, parity, dataBits, charFormat);

                    // DtrEnable
                    await port.DtrEnable_set(dtr);

                    // RtsEnable
                    await port.RtsEnable_set(rts);

                    return addToListTask.AsTask();
                })));
            }

            var uiSyncContext = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
            System.Threading.Tasks.Task.WhenAll(createSerialPortTasks).ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    // Close all devices because some devices failed to be opened.
                    UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();

                    // Throw the exception.
                    System.Threading.SynchronizationContext.Current.Post((state) =>
                    {
                        throw state as Exception;
                    }
                    , task.Exception.GetBaseException());
                }
                else if (this.SerialPortInfo1 != null && this.SerialPortInfo2 != null)
                {
                    this.buttonLoopbackTest.IsEnabled = true;
                    this.buttonInitialize.IsEnabled = false;
                    SDKTemplate.MainPage.Current.NotifyUser("Initialized.", SDKTemplate.NotifyType.StatusMessage);
                }
                else
                {
                    String deviceNumber;

                    if (this.SerialPortInfo1 == null)
                    {
                        if (this.SerialPortInfo2 == null)
                        {
                            deviceNumber = "Both devices";
                        }
                        else
                        {
                            deviceNumber = "Device 1";
                        }
                    }
                    else
                    {
                        deviceNumber = "Device 2";
                    }

                    SDKTemplate.MainPage.Current.NotifyUser(deviceNumber + " failed to be initialized.", SDKTemplate.NotifyType.ErrorMessage);

                    // Close all devices because some devices failed to be opened.
                    UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();
                }
            }
            , uiSyncContext);
        }

        private void buttonLoopbackTest_Click(object sender, RoutedEventArgs e)
        {
            // Unicode to ASCII.
            String textToSend = this.textBoxForLoopback.Text;                    
            var encoder = System.Text.Encoding.UTF8.GetEncoder();
            var utf8bytes = new byte[textToSend.Length];
            int bytesUsed, charsUsed;
            bool completed;
            encoder.Convert(textToSend.ToCharArray(), 0, textToSend.Length, utf8bytes, 0, utf8bytes.Length, true, out bytesUsed, out charsUsed, out completed);

            var writer = new Windows.Storage.Streams.DataWriter();
            writer.WriteBytes(utf8bytes);
            writer.WriteByte(0x00); // NUL
            var buffer = writer.DetachBuffer();

            this.buttonLoopbackTest.IsEnabled = false;
            this.buttonStopLoopback.IsEnabled = true;
            SDKTemplate.MainPage.Current.NotifyUser("", SDKTemplate.NotifyType.StatusMessage);

            var dispatcher = this.Dispatcher;
            ((Action)(async () =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
                {
                    // serialport1 to serialport2

                    var readBuffer = new Windows.Storage.Streams.Buffer(buffer.Length);
                    readBuffer.Length = buffer.Length;

                    var writeTask = this.SerialPortInfo1.Port.Write(buffer, 0, buffer.Length).AsTask();
                    var readTask = this.Read(this.SerialPortInfo2.Port, readBuffer, Constants.InfiniteTimeout).AsTask();

                    try
                    {
                        await System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[] {writeTask, readTask});
                        readBuffer.Length = (uint)readTask.Result;
                    }
                    catch (System.OperationCanceledException)
                    {
                        // canceled.
                        SDKTemplate.MainPage.Current.NotifyUser("Canceled", SDKTemplate.NotifyType.ErrorMessage);
                        this.buttonLoopbackTest.IsEnabled = true;
                        this.buttonStopLoopback.IsEnabled = false;
                        return;
                    }
                    finally
                    {
                        this.cancelTokenSrcOpRead = null;
                        writeTask.AsAsyncAction().Cancel(); // just in case.
                        readTask.AsAsyncAction().Cancel(); // just in case.
                    }

                    var isSame = Util.CompareTo(buffer, readBuffer) == 0;
                    String statusMessage = "";
                    if (isSame)
                    {
                        statusMessage += "CDC device 2 received \"" + textToSend + "\" from CDC device 1. ";
                    }
                    else
                    {
                        statusMessage += "Loopback failed: CDC device 1 to CDC device 2. ";
                    }

                    // serialport2 to serialport1

                    readBuffer.Length = buffer.Length;

                    writeTask = this.SerialPortInfo2.Port.Write(buffer, 0, buffer.Length).AsTask();
                    readTask = this.Read(this.SerialPortInfo1.Port, readBuffer, Constants.InfiniteTimeout).AsTask();

                    try
                    {
                        await System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[] { writeTask, readTask });
                        readBuffer.Length = (uint)readTask.Result;
                    }
                    catch (System.OperationCanceledException)
                    {
                        // canceled.
                        SDKTemplate.MainPage.Current.NotifyUser("Canceled", SDKTemplate.NotifyType.ErrorMessage);
                        this.buttonLoopbackTest.IsEnabled = true;
                        this.buttonStopLoopback.IsEnabled = false;
                        return;
                    }
                    finally
                    {
                        this.cancelTokenSrcOpRead = null;
                        writeTask.AsAsyncAction().Cancel(); // just in case.
                        readTask.AsAsyncAction().Cancel(); // just in case.
                    }

                    isSame = Util.CompareTo(buffer, readBuffer) == 0;
                    if (isSame)
                    {
                        statusMessage += "CDC device 1 received \"" + textToSend + "\" from CDC device 2. ";
                    }
                    else
                    {
                        statusMessage += "Loopback failed: CDC device 2 to CDC device 1. ";
                    }

                    this.buttonLoopbackTest.IsEnabled = true;
                    this.buttonStopLoopback.IsEnabled = false;
                    SDKTemplate.MainPage.Current.NotifyUser(statusMessage, SDKTemplate.NotifyType.StatusMessage);
                }));
            }
            )).Invoke();
        }

        private void buttonStopLoopback_Click(object sender, RoutedEventArgs e)
        {
            if (this.cancelTokenSrcOpRead != null)
            {
                this.buttonStopLoopback.IsEnabled = false;
                this.cancelTokenSrcOpRead.Cancel();
                this.cancelTokenSrcOpRead.Dispose();
                this.cancelTokenSrcOpRead = null;
            }
        }

        private void comboBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Restore UI state.
            this.buttonStopLoopback_Click(this, null); // cancel read op if possible.
            this.buttonLoopbackTest.IsEnabled = false;
            this.buttonStopLoopback.IsEnabled = false;
            this.buttonInitialize.IsEnabled = true;

            // Dispose all devices.
            UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();

            var comboBoxDevices = sender as ComboBox;
            if (comboBoxDevices.SelectedItem == null)
            {
                return;
            }

            ComboBox[] comboBoxs = { this.comboBoxDevices1, this.comboBoxDevices2 };
            foreach (var comboBox in comboBoxs)
            {
                if (comboBox.SelectedItem == null)
                {
                    return;
                }
            }

            var item1 = this.comboBoxDevices1.SelectedItem as UsbDeviceComboBoxItem;
            var item2 = this.comboBoxDevices2.SelectedItem as UsbDeviceComboBoxItem;

            if (item1.DeviceId == item2.DeviceId)
            {
                // Both comboBoxes are selecting a same device.
                var currIndex = this.comboBoxDevices2.SelectedIndex;
                for (int index = 0; index < this.comboBoxDevices2.Items.Count; index++)
                {
                    if (index != currIndex)
                    {
                        this.comboBoxDevices2.SelectedIndex = index;
                        break;
                    }
                }
                if (currIndex == this.comboBoxDevices2.SelectedIndex)
                {
                    this.comboBoxDevices2.SelectedIndex = -1;
                    return;
                }
            }
        }

        private void OnDeviceAdded(object sender, UsbDeviceInfo info)
        {
            var dispatcher = this.Dispatcher;
            ((Action)(async () =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    this.comboBoxDevices1.Items.Add(new UsbDeviceComboBoxItem(info.Id, info.Name));
                    this.comboBoxDevices2.Items.Add(new UsbDeviceComboBoxItem(info.Id, info.Name));
                    if (this.comboBoxDevices1.SelectedIndex == -1)
                    {
                        if (this.previousSelectedDeviceId1 == info.Id || this.previousSelectedDeviceId1 == "")
                        {
                            this.comboBoxDevices1.SelectedIndex = 0;
                        }
                    }
                    if (this.comboBoxDevices2.SelectedIndex == -1)
                    {
                        if (this.previousSelectedDeviceId2 == info.Id || this.previousSelectedDeviceId2 == "")
                        {
                            this.comboBoxDevices2.SelectedIndex = 0;
                        }
                    }
                }));
            }
            )).Invoke();
        }

        private Windows.Foundation.IAsyncAction OnDeviceRemoved(object sender, UsbDeviceInfo info)
        {
            var dispatcher = this.Dispatcher;
            return dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                var showMessageDialog = false;
                ComboBox[] comboBoxs = { this.comboBoxDevices1, this.comboBoxDevices2 };
                foreach (var comboBoxDevices in comboBoxs)
                {
                    for (int i = 0; i < comboBoxDevices.Items.Count; i++)
                    {
                        var item = comboBoxDevices.Items[i] as UsbDeviceComboBoxItem;
                        if (item.DeviceId == info.Id)
                        {
                            if (this.buttonInitialize.IsEnabled == false && comboBoxDevices.SelectedIndex == i)
                            {
                                showMessageDialog = true;
                                if (this.cancelTokenSrcOpRead != null)
                                {
                                    this.buttonStopLoopback_Click(this, null); // cancel read op if possible.
                                }
                            }
                            comboBoxDevices.Items.RemoveAt(i);
                            if (comboBoxDevices.SelectedIndex == -1 && comboBoxDevices.Items.Count > 0)
                            {
                                comboBoxDevices.SelectedIndex = 0;
                            }
                            break;
                        }
                    }
                }

                if (showMessageDialog)
                {
                    await (new Windows.UI.Popups.MessageDialog(info.Name + " has been removed.")).ShowAsync();
                }
            }));
        }

        private Windows.Foundation.IAsyncOperation<int> Read(UsbCdcControlAccess.UsbSerialPort port, Windows.Storage.Streams.Buffer buffer, int timeout)
        {
            port.ReadTimeout = timeout;

            if (this.cancelTokenSrcOpRead != null)
            {
                this.cancelTokenSrcOpRead.Dispose();
            }
            var source = this.cancelTokenSrcOpRead = new System.Threading.CancellationTokenSource();

            return port.Read(buffer, 0, buffer.Length, source.Token);
        }

        private UsbCdcControl.UsbSerialPortInfo SerialPortInfo1
        {
            get
            {
                if (UsbCdcControl.UsbDeviceList.Singleton.Count > 0 && this.comboBoxDevices1.SelectedIndex != -1)
                {
                    for (int i = 0; i < UsbCdcControl.UsbDeviceList.Singleton.Count; i++)
                    {
                        if (UsbCdcControl.UsbDeviceList.Singleton[i].DeviceId == (this.comboBoxDevices1.SelectedItem as UsbDeviceComboBoxItem).DeviceId)
                        {
                            return UsbCdcControl.UsbDeviceList.Singleton[i];
                        }
                    }
                }
                return null;
            }
        }

        private UsbCdcControl.UsbSerialPortInfo SerialPortInfo2
        {
            get
            {
                if (UsbCdcControl.UsbDeviceList.Singleton.Count > 0 && this.comboBoxDevices2.SelectedIndex != -1)
                {
                    for (int i = 0; i < UsbCdcControl.UsbDeviceList.Singleton.Count; i++)
                    {
                        if (UsbCdcControl.UsbDeviceList.Singleton[i].DeviceId == (this.comboBoxDevices2.SelectedItem as UsbDeviceComboBoxItem).DeviceId)
                        {
                            return UsbCdcControl.UsbDeviceList.Singleton[i];
                        }
                    }
                }
                return null;
            }
        }

        private String previousSelectedDeviceId1 = "";
        private String previousSelectedDeviceId2 = "";
        private System.Threading.CancellationTokenSource cancelTokenSrcOpRead = null;
    }
}