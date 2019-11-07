//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using SDKTemplate;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MediaStreamSource
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
#if WINDOWS_PHONE_APP
    public sealed partial class Scenario1_LoadMP3FileForAudioStream : Page, IFileOpenPickerContinuable
#else
    public sealed partial class Scenario1_LoadMP3FileForAudioStream : Page
#endif
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private StorageFile inputMP3File;
        private Windows.Media.Core.MediaStreamSource MSS = null;
        private IRandomAccessStream mssStream;
        private IInputStream inputStream;
        private UInt64 byteOffset;
        private TimeSpan timeOffset;
        private TimeSpan songDuration;

        // MP3 Framesize and length for Layer II and Layer III

        private const UInt32 sampleSize = 1152; 
        private TimeSpan sampleDuration = new TimeSpan(0, 0, 0, 0, 70);

        public Scenario1_LoadMP3FileForAudioStream()
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

        async private void InitializeMediaStreamSource()
        {
            // initialize Parsing Variables
            byteOffset = 0;
            timeOffset = new TimeSpan(0);

            // get the encoding properties of the input MP3 file

            List<string> encodingPropertiesToRetrieve = new List<string>();

            encodingPropertiesToRetrieve.Add("System.Audio.SampleRate");
            encodingPropertiesToRetrieve.Add("System.Audio.ChannelCount");
            encodingPropertiesToRetrieve.Add("System.Audio.EncodingBitrate");

            IDictionary<string, object> encodingProperties = await inputMP3File.Properties.RetrievePropertiesAsync(encodingPropertiesToRetrieve);

            uint sampleRate = (uint)encodingProperties["System.Audio.SampleRate"];
            uint channelCount = (uint)encodingProperties["System.Audio.ChannelCount"];
            uint bitRate = (uint)encodingProperties["System.Audio.EncodingBitrate"];

            // get the common music properties of the input MP3 file

            MusicProperties mp3FileProperties = await inputMP3File.Properties.GetMusicPropertiesAsync();
            songDuration = mp3FileProperties.Duration;

            // creating the AudioEncodingProperties for the MP3 file

            AudioEncodingProperties audioProps = AudioEncodingProperties.CreateMp3(sampleRate, channelCount, bitRate);

            // creating the AudioStreamDescriptor for the MP3 file
 
            AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(audioProps);

            // creating the MediaStreamSource for the MP3 file

            MSS = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
            MSS.CanSeek = true;
            MSS.MusicProperties.Title = mp3FileProperties.Title;
            MSS.Duration = songDuration;

            // hooking up the MediaStreamSource event handlers
            MSS.Starting += MSS_Starting;
            MSS.SampleRequested += MSS_SampleRequested;
            MSS.Closed += MSS_Closed;

            mediaPlayer.SetMediaStreamSource(MSS);
            rootPage.NotifyUser("MediaStreamSource created and set as source", NotifyType.StatusMessage);
        }

        void MSS_Closed(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceClosedEventArgs args)
        {

            // close the MediaStreamSource and remove the MediaStreamSource event handlers

            if (mssStream != null)
            {
                mssStream.Dispose();
                mssStream = null;
            }

            sender.SampleRequested -= MSS_SampleRequested;
            sender.Starting -= MSS_Starting;
            sender.Closed -= MSS_Closed;

            if (sender == MSS) 
            { 
                MSS = null; 
            }
        }

        async void MSS_SampleRequested(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            MediaStreamSourceSampleRequest request = args.Request;

            // check if the sample requested byte offset is within the file size

            if (byteOffset + sampleSize <= mssStream.Size)
            {
                MediaStreamSourceSampleRequestDeferral deferal = request.GetDeferral();
                inputStream = mssStream.GetInputStreamAt(byteOffset);

                // create the MediaStreamSample and assign to the request object. 
                // You could also create the MediaStreamSample using createFromBuffer(...)

                MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, sampleSize, timeOffset);
                sample.Duration = sampleDuration;
                sample.KeyFrame = true;

                // increment the time and byte offset

                byteOffset += sampleSize;
                timeOffset = timeOffset.Add(sampleDuration);
                request.Sample = sample;
                deferal.Complete();
            }
        }

        async void MSS_Starting(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            MediaStreamSourceStartingRequest request = args.Request;
            if ((request.StartPosition != null) && request.StartPosition.Value <= MSS.Duration)
            {
                UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
                timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
                byteOffset = sampleOffset * sampleSize;
            }

            // create the RandomAccessStream for the input file for the first time 

            if (mssStream == null)
            {
                MediaStreamSourceStartingRequestDeferral deferal = request.GetDeferral();
                try
                {
                    mssStream = await inputMP3File.OpenAsync(FileAccessMode.Read);
                    request.SetActualStartPosition(timeOffset);
                    deferal.Complete();
                }
                catch (Exception)
                {
                    MSS.NotifyError(MediaStreamSourceErrorStatus.FailedToOpenFile);
                    deferal.Complete();
                }
            }
            else
            {
                request.SetActualStartPosition(timeOffset);
            }
        }

#if WINDOWS_APP
        async private void PickMP3_Click(object sender, RoutedEventArgs e)
#else
        private void PickMP3_Click(object sender, RoutedEventArgs e)
#endif
        {
            Button b = sender as Button;
            if (b != null)
            {
                FileOpenPicker filePicker = new FileOpenPicker();
                filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                filePicker.FileTypeFilter.Add(".mp3");
                filePicker.ViewMode = PickerViewMode.List;

#if WINDOWS_APP
                StorageFile localMP3 = await filePicker.PickSingleFileAsync();

                if (localMP3 != null)
                {
                    inputMP3File = localMP3;

                    // Initialize MediaStreamSource with the selected MP3 file
                    InitializeMediaStreamSource();

                    mediaPlayer.Play();

                    rootPage.NotifyUser("Playing MP3 using MediaStreamSource", NotifyType.StatusMessage);
                }
                else
                    rootPage.NotifyUser("Invalid file selected", NotifyType.ErrorMessage);
            }
        }
#else
                filePicker.ContinuationData.Add("Operation", "OpenMP3File");
                filePicker.PickSingleFileAndContinue();
            }
        }

        public void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.ContinuationData.ContainsKey("Operation") && args.ContinuationData["Operation"] as string == "OpenMP3File")
            {
                StorageFile localMP3 = null;
                if (args.Files.Count > 0)
                {
                    localMP3 = args.Files[0];
                }
                if (localMP3 != null)
                {
                    inputMP3File = localMP3;

                    // Initialize MediaStreamSource with the selected MP3 file
                    InitializeMediaStreamSource();

                    mediaPlayer.Play();

                    rootPage.NotifyUser("Playing MP3 using MediaStreamSource", NotifyType.StatusMessage);
                }
                else
                    rootPage.NotifyUser("No file selected or invalid file selected", NotifyType.ErrorMessage);
            }
        }

#endif

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Playing the selected MP3 file", NotifyType.StatusMessage);
            mediaPlayer.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Pausing the MP3 playback", NotifyType.StatusMessage);
            mediaPlayer.Pause();
        }
    }
}
