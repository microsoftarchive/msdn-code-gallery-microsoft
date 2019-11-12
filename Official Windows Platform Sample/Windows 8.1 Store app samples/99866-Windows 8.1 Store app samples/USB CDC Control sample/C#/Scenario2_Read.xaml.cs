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
    /// Page containing working sample code to demonstrate how to read data from a CDC ACM device.
    /// </summary>
    internal sealed partial class CdcAcmRead : UsbCdcControl.SingleDevicePage
    {
        public CdcAcmRead()
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
                this.buttonReadBulkIn.IsEnabled = false;
                this.buttonWatchBulkIn.IsEnabled = false;
                this.buttonStopWatching.IsEnabled = false;
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

            if (pageState != null)
            {
                pageState.Add(textBoxBytesToRead.Name, textBoxBytesToRead.Text);
                pageState.Add(textBoxReadTimeout.Name, textBoxReadTimeout.Text);
            }
        }

        private void buttonReadBulkIn_Click(object sender, RoutedEventArgs e)
        {
            if (buttonReadBulkIn.Content.ToString() == "Read")
            {
                uint bytesToRead = uint.Parse(textBoxBytesToRead.Text);
                if (bytesToRead > 0)
                {
                    // UI status.
                    buttonReadBulkIn.Content = "Stop Read";
                    buttonWatchBulkIn.IsEnabled = false;
                    SDKTemplate.MainPage.Current.NotifyUser("Reading", SDKTemplate.NotifyType.StatusMessage);

                    var buffer = new Windows.Storage.Streams.Buffer(bytesToRead);
                    buffer.Length = bytesToRead;
                    int timeout = int.Parse(textBoxReadTimeout.Text);

                    var dispatcher = this.Dispatcher;
                    Action readAction = async () =>
                    {
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(async () =>
                        {
                            int count = 0;
                            try
                            {
                                count = await this.Read(buffer, timeout);
                            }
                            catch (System.OperationCanceledException)
                            {
                                // cancel.
                                SDKTemplate.MainPage.Current.NotifyUser("Canceled", SDKTemplate.NotifyType.ErrorMessage);
                                return;
                            }
                            catch (System.Exception exception)
                            {
                                if (exception.HResult == -2146233088)
                                {
                                    // Device removed.
                                    return;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                this.cancelTokenSrcOpRead = null;
                            }

                            this.buttonReadBulkIn.Content = "Read";
                            this.buttonWatchBulkIn.IsEnabled = true;

                            if (count < bytesToRead)
                            {
                                // This would be timeout.
                                SDKTemplate.MainPage.Current.NotifyUser("Timeout: read " + count.ToString() + " byte(s)", SDKTemplate.NotifyType.ErrorMessage);
                            }
                            else
                            {
                                SDKTemplate.MainPage.Current.NotifyUser("Completed", SDKTemplate.NotifyType.StatusMessage);
                            }

                            if (count > 0)
                            {
                                var isAscii = this.radioButtonAscii.IsChecked.Value == true;
                                var temp = this.textBoxReadBulkInLogger.Text;
                                temp += isAscii ? Util.AsciiBufferToAsciiString(buffer) : Util.BinaryBufferToBinaryString(buffer);
                                this.textBoxReadBulkInLogger.Text = temp;
                            }
                        }));
                    };
                    readAction.Invoke();
                }
            }
            else
            {
                this.buttonStopWatching_Click(this, null);
                this.buttonReadBulkIn.Content = "Read";
            }
        }

        private void buttonWatchBulkIn_Click(object sender, RoutedEventArgs e)
        {
            this.buttonReadBulkIn.IsEnabled = false;
            this.buttonWatchBulkIn.IsEnabled = false;
            this.buttonStopWatching.IsEnabled = true;
            SDKTemplate.MainPage.Current.NotifyUser("", SDKTemplate.NotifyType.StatusMessage);

            this.ReadByteOneByOne();
        }

        private void buttonStopWatching_Click(object sender, RoutedEventArgs e)
        {
            if (this.cancelTokenSrcOpRead != null)
            {
                this.cancelTokenSrcOpRead.Cancel();
                this.cancelTokenSrcOpRead = null;
            }
            this.buttonReadBulkIn.IsEnabled = true;
            this.buttonWatchBulkIn.IsEnabled = true;
            this.buttonStopWatching.IsEnabled = false;
        }

        private void textBoxLogger_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.GotoEndPosTextBox(sender as TextBox);
        }

        private void radioButtonDataFormat_Checked(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(this.radioButtonAscii))
            {
                var binary = this.textBoxReadBulkInLogger.Text;
                var separator = new String[1];
                separator[0] = " ";
                var byteArray = binary.Split(separator, StringSplitOptions.None);

                // Binary to Unicode.
                uint strlen = 0;
                var writer = new Windows.Storage.Streams.DataWriter();
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                foreach (var onebyte in byteArray)
                {
                    if (onebyte.Length < 2)
                    {
                        continue;
                    }
                    writer.WriteByte(byte.Parse(onebyte, System.Globalization.NumberStyles.HexNumber));
                    strlen++;
                }
                
                var reader = Windows.Storage.Streams.DataReader.FromBuffer(writer.DetachBuffer());
                this.textBoxReadBulkInLogger.Text = reader.ReadString(strlen);
            }
            else if (sender.Equals(this.radioButtonBinary))
            {
                var ascii = this.textBoxReadBulkInLogger.Text;

                // Unicode to ASCII.
                var chars = ascii.ToCharArray(0, ascii.Length);
                var writer = new Windows.Storage.Streams.DataWriter();
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                foreach (var onechar in chars)
                {
                    writer.WriteByte((byte)onechar);
                }

                var str = Util.BinaryBufferToBinaryString(writer.DetachBuffer());
                this.textBoxReadBulkInLogger.Text = str;
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
                if (this.SerialPortInfo != null && this.SerialPortInfo.DeviceId == info.Id)
                {
                    dialogShowAsync = (new Windows.UI.Popups.MessageDialog(info.Name + " has been removed.")).ShowAsync();
                    if (this.cancelTokenSrcOpRead != null)
                    {
                        this.buttonStopWatching_Click(this, null); // cancel read op if possible.
                    }
                    this.buttonReadBulkIn.IsEnabled = false;
                    this.buttonWatchBulkIn.IsEnabled = false;
                    this.buttonStopWatching.IsEnabled = false;
                    this.textBlockDeviceInUse.Text = "No device selected.";
                    this.textBlockDeviceInUse.Foreground = new SolidColorBrush(Windows.UI.Colors.OrangeRed);
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

        private void ReadByteOneByOne()
        {
            var dispatcher = this.Dispatcher;
            var buffer = new Windows.Storage.Streams.Buffer(1);
            buffer.Length = 1;

            System.Threading.Tasks.Task.Run(async () =>
            {
                int count = 0;
                try
                {
                    count = await this.Read(buffer, -1);
                }
                catch (System.OperationCanceledException)
                {
                    return; // StopWatching seems clicked.
                }
                finally
                {
                    this.cancelTokenSrcOpRead = null;
                }

                if (count > 0)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        var isAscii = this.radioButtonAscii.IsChecked.Value == true;
                        var temp = this.textBoxReadBulkInLogger.Text;
                        temp += isAscii ? Util.AsciiBufferToAsciiString(buffer) : Util.BinaryBufferToBinaryString(buffer);
                        this.textBoxReadBulkInLogger.Text = temp;
                    }));
                }

                this.ReadByteOneByOne();
            });
        }

        private Windows.Foundation.IAsyncOperation<int> Read(Windows.Storage.Streams.Buffer buffer, int timeout)
        {
            this.SerialPortInfo.Port.ReadTimeout = timeout;

            var source = this.cancelTokenSrcOpRead = new System.Threading.CancellationTokenSource();

            return this.SerialPortInfo.Port.Read(buffer, 0, buffer.Length, source.Token);
        }

        private System.Threading.CancellationTokenSource cancelTokenSrcOpRead = null;
    }
}