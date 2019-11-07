//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Documents;

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceReadWrite
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DeviceReadWrite()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private int writeCounter = 0;
        private int readCounter = 0;

        private async void ReadBlock_Click(object sender, RoutedEventArgs e)
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            var button = (Button)sender;
            button.IsEnabled = false;

            var dataReader = new DataReader(fx2Device.InputStream);

            // load the data reader from the stream.  For purposes of this 
            // sample, assume all messages read are < 64 bytes

            int counter = readCounter++;

            LogMessage("Read {0} begin", counter);
            await dataReader.LoadAsync(64);
            
            // Get the message string out of the buffer
            var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);

            LogMessage("Read {0} end: {1}", counter, message);

            button.IsEnabled = true;
        }

        private async void WriteBlock_Click(object sender, RoutedEventArgs e)
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            var button = (Button)sender;
            button.IsEnabled = false;

            // Generate a string to write to the device
            var counter = writeCounter++;
            
            // Use a data writer to convert the string into a buffer
            var dataWriter = new DataWriter();
            var msg = "This is message " + counter;
            dataWriter.WriteString(msg);

            LogMessage("Write {0} begin: {1}", counter, msg);
            var bytesRead = await fx2Device.OutputStream.WriteAsync(dataWriter.DetachBuffer());
            LogMessage("Write {0} end: {1} bytes written", counter, bytesRead);

            button.IsEnabled = true;
        }

        private void LogMessage(string format, params object[] args)
        {
            var span = new Span();
            var run = new Run();
            run.Text = string.Format(format, args);
            
            span.Inlines.Add(run);
            span.Inlines.Add(new LineBreak());

            OutputText.Inlines.Insert(0, span);
        }
    }
}
