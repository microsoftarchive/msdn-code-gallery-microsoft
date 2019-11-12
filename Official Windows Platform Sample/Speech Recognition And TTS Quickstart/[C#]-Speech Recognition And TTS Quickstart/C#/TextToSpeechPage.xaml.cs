/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SpeechRecognitionAndTTSQuickstart
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class TextToSpeechPage : Page
	{
		private Windows.Media.SpeechSynthesis.SpeechSynthesizer speechSynthesizer;

		public TextToSpeechPage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			try
			{
				this.installedVoicesListBox.ItemsSource = from voiceInformation in Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices select voiceInformation;

				this.speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
			}
			catch (Exception exception)
			{
				var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
				messageDialog.ShowAsync().GetResults();
			}
		}

		private async void InstalledVoicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            try
            {
				if (e.AddedItems.Count > 0)
				{
					var voiceInformation = e.AddedItems[0] as Windows.Media.SpeechSynthesis.VoiceInformation;
					this.speechSynthesizer.Voice = voiceInformation;
					var stream = await this.speechSynthesizer.SynthesizeTextToStreamAsync(string.Format("This is {0}, {1}, {2}. {3}", voiceInformation.DisplayName, voiceInformation.Language, voiceInformation.Gender, speakTextBox.Text));
					feedbackMediaElement.SetSource(stream, stream.ContentType);
					feedbackMediaElement.Play();
				}
            }
			catch (Exception exception)
			{
				var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
				messageDialog.ShowAsync().GetResults();
			}
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
		}
	}
}
