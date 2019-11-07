//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace VoiceSynthesis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private SpeechSynthesizer synthesizer;
        private bool disposed = false;

        public Scenario1()
        {
            this.InitializeComponent();
            this.synthesizer = new SpeechSynthesizer();
            this.ListboxVoiceChooser_Initialize();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Performs any finalization method needed
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// cleans up all local objects disposable objects
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.synthesizer.Dispose();
                }                
            }

            this.disposed = true;
        }

        /// <summary>
        /// This is invoked when the user clicks on the speak button
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        private async void BtnSpeak_Click(object sender, RoutedEventArgs e)
        {
            string text = this.tbData.Text;

            // disable this button, so that the user does not attempt to hit it again
            Button b = sender as Button;
            b.IsEnabled = false;
            this.media.CurrentStateChanged += this.media_CurrentStateChanged;

            // create the data stream
            SpeechSynthesisStream synthesisStream;
            try
            {
                //creating a stream from the text which can be played using media element. This new API converts text input into a stream.
                synthesisStream = await this.synthesizer.SynthesizeTextToStreamAsync(text);
            }
            catch (Exception)
            {
                synthesisStream = null;
                this.btnSpeak.IsEnabled = true;
            }
            // if the SSML stream is not in the correct format throw an error message to the user
            if (synthesisStream == null)
            {
                MessageDialog dialog = new MessageDialog("unable to synthesize text");
                await dialog.ShowAsync();
                return;
            }


            // start this audio stream playing
            this.media.AutoPlay = true;
            this.media.SetSource(synthesisStream, synthesisStream.ContentType);
            this.media.Play();
        }

        /// <summary>
        /// this is invoked when the media element state is changing
        /// </summary>
        /// <remarks>
        /// this simply is looking for the media element to go to a paused state, then it can enable the button
        /// </remarks>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused object parameter</param>
        void media_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.media.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Paused)
            {
                this.btnSpeak.IsEnabled = true;
                this.media.CurrentStateChanged -= this.media_CurrentStateChanged;
            }
        }

        /// <summary>
        /// invoked on form load, used to initialize the list of installed voices.
        /// </summary>
        private void ListboxVoiceChooser_Initialize()
        {
            // get all of the installed voices
            var voices = Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices;

            // get the currently selected voice
            VoiceInformation currentVoice = this.synthesizer.Voice;

            foreach (VoiceInformation voice in voices)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = voice.DisplayName;
                item.Tag = voice;
                item.Content = voice.DisplayName;
                this.listboxVoiceChooser.Items.Add(item);

                // check to see if this is the current voice, so that we can set it to be selected
                if (currentVoice.Id == voice.Id)
                {
                    item.IsSelected = true;
                    this.listboxVoiceChooser.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// invoked when the user makes a selection in the choose voices listbox
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        private void ListboxVoiceChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.listboxVoiceChooser.SelectedItem;
            VoiceInformation voice = (VoiceInformation)item.Tag;
            this.synthesizer.Voice = voice;
        }

        /// <summary>
        /// invoked when the user clicks the save to file button
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        private async void BtnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            string text = this.tbData.Text;


            // select the file to save this data to
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".wav";
            // this is the only type available
            savePicker.FileTypeChoices.Add("Audio file", new List<string>() { ".wav" });

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                this.btnSaveToFile.IsEnabled = false;

                // create the data stream
                SpeechSynthesisStream synthesisStream;
                try
                {
                    synthesisStream = await this.synthesizer.SynthesizeTextToStreamAsync(text);
                }
                catch (Exception)
                {
                    synthesisStream = null;
                    this.btnSaveToFile.IsEnabled = true;
                }

                if (synthesisStream == null)
                {
                    MessageDialog dialog = new MessageDialog("unable to synthesize text");
                    await dialog.ShowAsync();
                    return;
                }

                // open the output stream                    
                Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer(4096);
                IRandomAccessStream writeStream = (IRandomAccessStream)await file.OpenAsync(FileAccessMode.ReadWrite);
                IOutputStream outputStream = writeStream.GetOutputStreamAt(0);
                DataWriter dataWriter = new DataWriter(outputStream);

                // copy the stream data into the file                    
                while (synthesisStream.Position < synthesisStream.Size)
                {
                    await synthesisStream.ReadAsync(buffer, 4096, InputStreamOptions.None);
                    dataWriter.WriteBuffer(buffer);
                }

                // close the data file streams
                dataWriter.StoreAsync().AsTask().Wait();
                outputStream.FlushAsync().AsTask().Wait();

                this.btnSaveToFile.IsEnabled = true;
            }
        }
    }
}
