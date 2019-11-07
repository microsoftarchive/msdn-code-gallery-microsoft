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
    /// Page containing working sample code to demonstrate how to write data to a CDC ACM device.
    /// </summary>
    internal sealed partial class CdcAcmWrite : UsbCdcControl.SingleDevicePage
    {
        private static readonly DependencyProperty RawBinaryDataProperty =
            DependencyProperty.Register(
            "RawBinaryData",
            typeof(Windows.Storage.Streams.IBuffer),
            typeof(TextBox), null
            );

        public CdcAcmWrite()
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

            if (this.SerialPortInfo != null)
            {
                this.textBlockDeviceInUse.Text = this.SerialPortInfo.Name;
            }
            else
            {
                this.buttonWriteBulkOut.IsEnabled = false;
                this.buttonSendBreak.IsEnabled = false;
                this.buttonWriteBinary1.IsEnabled = false;
                this.buttonWriteBinary2.IsEnabled = false;
                this.textBlockDeviceInUse.Text = "No device selected.";
                this.textBlockDeviceInUse.Foreground = new SolidColorBrush(Windows.UI.Colors.OrangeRed);
            }

            UsbCdcControl.UsbDeviceList.Singleton.DeviceAdded += this.OnDeviceAdded;
            UsbCdcControl.UsbDeviceList.Singleton.DeviceRemoved += this.OnDeviceRemoved;
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
        }

        private void buttonLoadBinaryData_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".bin");
            var dispatcher = this.Dispatcher;
            ((Action)(async () =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
                {
                    var file = await picker.PickSingleFileAsync();
                    if (file == null)
                    {
                        return;
                    }
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    var reader = new Windows.Storage.Streams.DataReader(stream.GetInputStreamAt(0));
                    var count = await reader.LoadAsync((uint)stream.Size);
                    var buffer = reader.ReadBuffer(count);
                    var str = Util.BinaryBufferToBinaryString(buffer);

                    var index = (sender as Windows.UI.Xaml.Controls.Control).Name.Replace("buttonLoadBinaryData", "");
                    TextBox textBox = Util.FindControl(Input, "textBoxBinaryData" + index) as TextBox;
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        textBox.Text = str;
                        textBox.SetValue(RawBinaryDataProperty, buffer);
                    }));
                }));
            }
            )).Invoke();
        }

        private void buttonWriteBinary_Click(object sender, RoutedEventArgs e)
        {
            var index = (sender as Windows.UI.Xaml.Controls.Control).Name.Replace("buttonWriteBinary", "");
            TextBox textBox = Util.FindControl(Input, "textBoxBinaryData" + index) as TextBox;
            var value = textBox.GetValue(RawBinaryDataProperty) as Windows.Storage.Streams.IBuffer;
            if (value != null)
            {
                var buffer = value;
                var dispatcher = this.Dispatcher;
                ((Action)(async () =>
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
                    {
                        await this.SerialPortInfo.Port.Write(buffer, 0, buffer.Length);
                        var temp = this.textBoxWriteLog.Text;
                        temp += "Write completed: \"" + Util.BinaryBufferToBinaryString(buffer) + "\" (" + buffer.Length.ToString() + " bytes)\n";
                        this.textBoxWriteLog.Text = temp;
                    }));
                }
                )).Invoke();
            }
        }

        private void buttonWriteBulkOut_Click(object sender, RoutedEventArgs e)
        {
            if (this.SerialPortInfo != null)
            {
                var dataToWrite = this.textBoxDataToWrite.Text;

                var dispatcher = this.Dispatcher;
                ((Action)(async () =>
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
                    {
                        // Unicode to ASCII.
                        var encoder = System.Text.Encoding.UTF8.GetEncoder();
                        var utf8bytes = new byte[dataToWrite.Length];
                        int bytesUsed, charsUsed;
                        bool completed;
                        encoder.Convert(dataToWrite.ToCharArray(), 0, dataToWrite.Length, utf8bytes, 0, utf8bytes.Length, true, out bytesUsed, out charsUsed, out completed);

                        var writer = new Windows.Storage.Streams.DataWriter();
                        writer.WriteBytes(utf8bytes);
                        var isChecked = checkBoxSendNullTerminateCharToBulkOut.IsChecked;
                        if (isChecked.HasValue && isChecked.Value == true)
                        {
                            writer.WriteByte(0x00); // NUL
                        }
                        var buffer = writer.DetachBuffer();
                        await this.SerialPortInfo.Port.Write(buffer, 0, buffer.Length);

                        var temp = this.textBoxWriteLog.Text;
                        temp += "Write completed: \"" + dataToWrite + "\" (" + buffer.Length.ToString() + " bytes)\n";
                        this.textBoxWriteLog.Text = temp;

                        this.textBoxDataToWrite.Text = "";
                    }));
                }
                )).Invoke();
            }
        }

        private void textBoxLogger_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.GotoEndPosTextBox(sender as TextBox);
        }

        /// <summary>
        /// Generates a RS-232C break signal during a time length.
        /// </summary>
        private void buttonSendBreak_Click(object sender, RoutedEventArgs e)
        {
            if (this.SerialPortInfo != null)
            {
                var durationOfBreak = uint.Parse(this.textBoxDurationOfBreak.Text);
                if (durationOfBreak > ushort.MaxValue)
                {
                    durationOfBreak = ushort.MaxValue; // The argument 'value' is ushort.
                }

                ((Action)(async () =>
                {
                    await this.SerialPortInfo.Port.SetControlRequest(RequestCode.SendBreak, (ushort)durationOfBreak, null);
                }
                )).Invoke();
            } 
        }

        private void OnDeviceAdded(object sender, UsbDeviceInfo info)
        {

        }

        private Windows.Foundation.IAsyncAction OnDeviceRemoved(object sender, UsbDeviceInfo info)
        {
            var dispatcher = this.Dispatcher;
            IAsyncOperation<Windows.UI.Popups.IUICommand> dialogShowAsync = null;
            var uiThreadTasks = dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                if (this.SerialPortInfo != null)
                {
                    if (this.SerialPortInfo.DeviceId == info.Id)
                    {
                        dialogShowAsync = (new Windows.UI.Popups.MessageDialog(info.Name + " has been removed.")).ShowAsync();
                        this.buttonWriteBulkOut.IsEnabled = false;
                        this.buttonSendBreak.IsEnabled = false;
                        this.buttonWriteBinary1.IsEnabled = false;
                        this.buttonWriteBinary2.IsEnabled = false;
                        this.textBlockDeviceInUse.Text = "No device selected.";
                        this.textBlockDeviceInUse.Foreground = new SolidColorBrush(Windows.UI.Colors.OrangeRed);
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
    }
}