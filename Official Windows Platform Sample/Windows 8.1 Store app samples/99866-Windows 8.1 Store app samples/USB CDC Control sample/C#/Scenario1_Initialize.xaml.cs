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
    /// Page containing working sample code to demonstrate how to initialize a CDC ACM device.
    /// </summary>
    internal sealed partial class CdcAcmInitialize : UsbCdcControl.SingleDevicePage
    {
        public CdcAcmInitialize()
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
                    var textBox = control as Windows.UI.Xaml.Controls.TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = pair.Value as String;
                        continue;
                    }
                    var comboBox = control as Windows.UI.Xaml.Controls.ComboBox;
                    if (comboBox != null)
                    {
                        if (pair.Key == this.comboBoxDevices.Name)
                        {
                            this.previousSelectedDeviceId = pair.Value as String;
                        }
                        else
                        for (int j = 0; j < comboBox.Items.Count; j ++)
                        {
                            var comboBoxItem = comboBox.Items[j] as Windows.UI.Xaml.Controls.ComboBoxItem;
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

            foreach (var deviceList in UsbCdcControl.DeviceList.Instances)
            {
                foreach (var info in deviceList.Devices)
                {
                    this.OnDeviceAdded(this, new UsbDeviceInfo(info));
                }
            }

            if (UsbCdcControl.UsbDeviceList.Singleton.Count > 0)
            {
                this.comboBoxDevices.IsEnabled = false;
                this.buttonDeviceSelect.IsEnabled = false;
                this.buttonDeviceDeselect.IsEnabled = true;
            }
            else
            {
                this.buttonInitialize.IsEnabled = false;
                this.buttonDeviceDeselect.IsEnabled = false;
            }
            UsbCdcControl.UsbDeviceList.Singleton.DeviceAdded += this.OnDeviceAdded;
            UsbCdcControl.UsbDeviceList.Singleton.DeviceRemoved += this.OnDeviceRemoved;
            UsbCdcControl.UsbDeviceList.Singleton.StartWatcher();
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

            if (pageState != null)
            {
                if (this.comboBoxDevices.SelectedValue != null)
                {
                    pageState.Add(this.comboBoxDevices.Name, (this.comboBoxDevices.SelectedValue as UsbDeviceComboBoxItem).Id);
                }
                pageState.Add(this.textBoxDTERate.Name, this.textBoxDTERate.Text);
                pageState.Add(this.comboBoxCharFormat.Name, (this.comboBoxCharFormat.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxParityType.Name, (this.comboBoxParityType.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxDataBits.Name, (this.comboBoxDataBits.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxRTS.Name, (this.comboBoxRTS.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
                pageState.Add(this.comboBoxDTR.Name, (this.comboBoxDTR.SelectedValue as Windows.UI.Xaml.Controls.ComboBoxItem).Content);
            }
        }

        private void buttonDeviceSelect_Click(object sender, RoutedEventArgs e)
        {
            // No device selected.
            if (this.comboBoxDevices.SelectedIndex == -1)
            {
                return;
            }

            var dispatcher = this.Dispatcher;
            var deviceId = (this.comboBoxDevices.SelectedItem as UsbDeviceComboBoxItem).Id;
            var deviceName = (this.comboBoxDevices.SelectedItem as UsbDeviceComboBoxItem).Content as String;
            UsbDevice.FromIdAsync(deviceId).Completed = new AsyncOperationCompletedHandler<UsbDevice>(async (op, status) =>
            {
                var usbDevice = op.GetResults();
                var serialport = await UsbCdcControlAccess.UsbSerialPort.CreateAsync(usbDevice);
                if (serialport == null)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        SDKTemplate.MainPage.Current.NotifyUser(deviceName + " is not compatible with CDC ACM.", SDKTemplate.NotifyType.ErrorMessage);
                    }));
                    if (usbDevice != null)
                    {
                        usbDevice.Dispose();
                    }
                    return;
                }
                UsbCdcControl.UsbDeviceList.Singleton.Add(new UsbCdcControl.UsbSerialPortInfo(serialport, deviceId, deviceName));

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    this.comboBoxDevices.IsEnabled = false;
                    this.buttonDeviceSelect.IsEnabled = false;
                    this.buttonInitialize.IsEnabled = true;
                    this.buttonDeviceDeselect.IsEnabled = true;
                    SDKTemplate.MainPage.Current.NotifyUser("", SDKTemplate.NotifyType.ErrorMessage);
                }));
            });
        }

        private void buttonDeviceDeselect_Click(object sender, RoutedEventArgs e)
        {
            string prevSelectedDeviceId = null;
            if (this.SerialPortInfo != null)
            {
                prevSelectedDeviceId = this.SerialPortInfo.DeviceId;
            }
            this.comboBoxDevices.IsEnabled = true;
            UsbCdcControl.UsbDeviceList.Singleton.DisposeAll();
            foreach (var deviceList in UsbCdcControl.DeviceList.Instances)
            {
                foreach (var info in deviceList.Devices)
                {
                    int foundIndex = -1;
                    for (int i = 0; i < this.comboBoxDevices.Items.Count; i++)
                    {
                        if ((this.comboBoxDevices.Items[i] as UsbDeviceComboBoxItem).Id == info.Id)
                        {
                            foundIndex = i;
                            break;
                        }
                    }
                    if (foundIndex == -1)
                    {
                        this.comboBoxDevices.Items.Insert(0, new UsbDeviceComboBoxItem(info.Id, info.Name));
                        foundIndex = 0;
                    }

                    if (String.IsNullOrEmpty(prevSelectedDeviceId) || prevSelectedDeviceId == info.Id)
                    {
                        this.comboBoxDevices.SelectedIndex = foundIndex;
                    }
                }
            }
            this.buttonDeviceSelect.IsEnabled = true;
            this.buttonInitialize.IsEnabled = false;
            this.buttonDeviceDeselect.IsEnabled = false;
        }

        private void buttonInitialize_Click(object sender, RoutedEventArgs e)
        {
            if (this.SerialPortInfo == null)
            {
                return;
            }

            var dispatcher = this.Dispatcher;
            var dteRate = uint.Parse(this.textBoxDTERate.Text);
            var parity = (Parity)this.comboBoxParityType.SelectedIndex;
            var dataBits = int.Parse((this.comboBoxDataBits.SelectedItem as ComboBoxItem).Content.ToString());
            var charFormat = (StopBits)this.comboBoxCharFormat.SelectedIndex;
            var dtr = this.comboBoxDTR.SelectedIndex != 0;
            var rts = this.comboBoxRTS.SelectedIndex != 0;
            var port = this.SerialPortInfo.Port;
            
            System.Threading.Tasks.Task.Run(async () =>
            {
                await port.Open(dteRate, parity, dataBits, charFormat);
                // DtrEnable
                await port.DtrEnable_set(dtr);
                // RtsEnable
                await port.RtsEnable_set(rts);

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    SDKTemplate.MainPage.Current.NotifyUser("Initialized.", SDKTemplate.NotifyType.StatusMessage);
                }));
            });
        }

        private void OnDeviceAdded(object sender, UsbDeviceInfo info)
        {
            var dispatcher = this.Dispatcher;
            if (dispatcher.HasThreadAccess)
            {
                this.AddDeviceToComboBox(info);
            }
            else
            ((Action)(async () =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    this.AddDeviceToComboBox(info);
                }));
            }
            )).Invoke();
        }

        private void AddDeviceToComboBox(UsbDeviceInfo info)
        {
            this.comboBoxDevices.Items.Insert(0, new UsbDeviceComboBoxItem(info.Id, info.Name));
            if (this.SerialPortInfo != null && this.SerialPortInfo.DeviceId == info.Id)
            {
                this.comboBoxDevices.SelectedIndex = 0;
            }
            else if (this.comboBoxDevices.SelectedIndex == -1)
            {
                if (this.previousSelectedDeviceId == info.Id || this.previousSelectedDeviceId == "")
                {
                    this.comboBoxDevices.SelectedIndex = 0;
                }
            }
        }

        private Windows.Foundation.IAsyncAction OnDeviceRemoved(object sender, UsbDeviceInfo info)
        {
            var dispatcher = this.Dispatcher;
            IAsyncOperation<Windows.UI.Popups.IUICommand> dialogShowAsync = null;
            var uiThreadTasks = dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                for (int i = 0; i < this.comboBoxDevices.Items.Count; i++)
                {
                    var item = this.comboBoxDevices.Items[i] as UsbDeviceComboBoxItem;
                    if (item.Id == info.Id)
                    {                     
                        bool isEnabled = this.comboBoxDevices.IsEnabled;
                        this.comboBoxDevices.IsEnabled = true;
                        this.comboBoxDevices.Items.RemoveAt(i);
                        if (this.SerialPortInfo != null && this.SerialPortInfo.DeviceId == info.Id)
                        {
                            dialogShowAsync = new Windows.UI.Popups.MessageDialog(info.Name + " has been removed.").ShowAsync();
                            this.buttonDeviceDeselect_Click(null, null);
                            if (this.comboBoxDevices.Items.Count > 0)
                            {
                                this.comboBoxDevices.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (this.comboBoxDevices.SelectedIndex == -1)
                            {
                                this.comboBoxDevices.SelectedIndex = 0;
                            }
                            this.comboBoxDevices.IsEnabled = isEnabled;
                        }
                        return;
                    }
                }
            }));
            return System.Threading.Tasks.Task.Run(async () =>
            {
                await uiThreadTasks;
                if (dialogShowAsync != null)
                {
                    await dialogShowAsync;
                }
            }
            ).AsAsyncAction();
        }

        private String previousSelectedDeviceId = "";
    }
}