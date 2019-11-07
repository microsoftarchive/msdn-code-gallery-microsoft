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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SpeechRecognitionAndTTSQuickstart
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PredefinedDictationGrammarPage : Page
	{
		public PredefinedDictationGrammarPage()
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
		}

		private async void RecognizeWithDictationGrammar_Click(object sender, RoutedEventArgs e)
		{
			// Create an instance of SpeechRecognizer.
			var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

			// Compile the dictation grammar that is loaded by default.
			await speechRecognizer.CompileConstraintsAsync();

			this.heardYouSayTextBlock.Visibility = this.resultTextBlock.Visibility = Visibility.Collapsed;

			// Start recognition.
			try
			{
				Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();
				// If successful, display the recognition result.
				if (speechRecognitionResult.Status == Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
				{
					this.heardYouSayTextBlock.Visibility = this.resultTextBlock.Visibility = Visibility.Visible;
					this.resultTextBlock.Text = speechRecognitionResult.Text;
				}
			}
			catch (Exception exception)
			{
				if ((uint)exception.HResult == App.HResultPrivacyStatementDeclined)
				{
					this.resultTextBlock.Visibility = Visibility.Visible;
					this.resultTextBlock.Text = "The privacy statement was declined.";
				}
				else
				{
					var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
					messageDialog.ShowAsync().GetResults();
				}
			}
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
		}
	}
}
