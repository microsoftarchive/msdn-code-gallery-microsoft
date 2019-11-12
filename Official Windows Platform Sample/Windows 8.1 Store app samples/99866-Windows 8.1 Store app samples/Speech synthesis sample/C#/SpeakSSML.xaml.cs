//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;
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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private SpeechSynthesizer synthesizer;
        private bool disposed = false;

        public Scenario2()
        {
            this.InitializeComponent();
            this.synthesizer = new SpeechSynthesizer();
            this.ListboxVoiceChooser_Initialize();

            this.UpdateSSMLText();
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
        /// this is invoked when the user clicks on the speak button for this scenario
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused argument parameter</param>
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
                synthesisStream = await this.synthesizer.SynthesizeSsmlToStreamAsync(text);

            }
            catch (Exception)
            {
                synthesisStream = null;
                this.btnSpeak.IsEnabled = true;
            }

            // if the SSML stream is not in the correct format throw an error message to the user
            if (synthesisStream == null)
            {
                MessageDialog dialog = new MessageDialog("Unable to synthesize text");
                await dialog.ShowAsync();
                return;
            }

            // start this audio stream playing
            this.media.AutoPlay = true;
            this.media.SetSource(synthesisStream, synthesisStream.ContentType);
            this.media.Play();
        }

        /// <summary>
        /// this is invoked when the media changes state.
        /// in this case, we are looking for the media element to clear, so that we re-enable the button
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        void media_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.media.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Paused)
            {
                this.btnSpeak.IsEnabled = true;
                this.media.CurrentStateChanged -= this.media_CurrentStateChanged;
            }
        }

        /// <summary>
        /// This is invoked when the form is initialized, and is used to initialize the list box
        ///  this simply enumerates all of the installed voices
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
        /// this is invoked when the user has selected a voice from the list box
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        private void ListboxVoiceChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.listboxVoiceChooser.SelectedItem;
            VoiceInformation voice = (VoiceInformation)item.Tag;
            this.synthesizer.Voice = voice;
            this.UpdateSSMLText();
        }

        /// <summary>
        /// Update the text in the SSML field for the language to match the chosen language
        /// </summary>
        /// <remarks>
        /// SSML language takes priority over the chosen language used by the synthesier.
        /// so when changing voices, we need to update the SSML language as well.
        /// </remarks>
        private void UpdateSSMLText()
        {
            try
            {
                string text = this.tbData.Text;
                string language = this.synthesizer.Voice.Language;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);

                var LangAttribute = doc.DocumentElement.GetAttributeNode("xml:lang");
                LangAttribute.InnerText = language;

                this.tbData.Text = doc.GetXml();
            }
            catch
            {
                // this can fail if the user is in the process of editing the XML
                // in this case, we simply don't update the SSML language, but don't throw a failure
            }
        }

        /// <summary>
        /// this is used when the user selects save to file
        /// </summary>
        /// <remarks>
        /// to implement this, we will need to create and instance of the save file picker
        /// then write the output stream into a file
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    synthesisStream = await this.synthesizer.SynthesizeSsmlToStreamAsync(text);
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
