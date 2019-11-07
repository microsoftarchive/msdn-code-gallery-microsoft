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
	public sealed partial class SRGSGrammarFileConstraintPage : Page
	{
		private Windows.Media.SpeechRecognition.SpeechRecognizer speechRecognizer;

		public SRGSGrammarFileConstraintPage()
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
			this.InitializeSpeechRecognizer();
		}

		private async void InitializeSpeechRecognizer()
		{
			// Create an instance of SpeechRecognizer.
			this.speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

			// Add a grammar file constraint to the recognizer.
			var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Colors.grxml"));
			var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(storageFile, "colors");

			this.speechRecognizer.UIOptions.ExampleText = @"Ex. ""blue background"", ""green text""";
			this.speechRecognizer.Constraints.Add(grammarFileConstraint);

			// Compile the constraint.
			await this.speechRecognizer.CompileConstraintsAsync();
		}

		private async void RecognizeWithSRGSGrammarFileConstraintOnce_Click(object sender, RoutedEventArgs e)
		{
			this.heardYouSayTextBlock.Visibility = this.resultTextBlock.Visibility = Visibility.Collapsed;

			// Start recognition.
			try
			{
				Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await this.speechRecognizer.RecognizeWithUIAsync();
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

			this.InitializeSpeechRecognizer();
		}

		private async void RecognizeWithSRGSGrammarFileConstraintContinuously_Click(object sender, RoutedEventArgs e)
		{
			this.recognizeOnceButton.IsEnabled = false;
			this.recognizeContinuouslyButton.Visibility = Visibility.Collapsed;
			this.stopRecognizingTextBlock.Visibility = this.listeningTextBlock.Visibility = Visibility.Visible;
			this.heardYouSayTextBlock.Visibility = this.resultTextBlock.Visibility = Visibility.Collapsed;

			// Start recognition.
			while (true)
			{
				try
				{
					Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await this.speechRecognizer.RecognizeAsync();
					// If successful, display the recognition result.
					if (speechRecognitionResult.Status == Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
					{
						this.resultTextBlock.Visibility = Visibility.Visible;
						this.resultTextBlock.Text = speechRecognitionResult.Text;
						if (speechRecognitionResult.Text == "stop recognizing")
						{
							break;
						}
					}
				}
				catch (Exception exception)
				{
					if ((uint)exception.HResult == App.HResultPrivacyStatementDeclined)
					{
						var messageDialog = new Windows.UI.Popups.MessageDialog("and accept the privacy statement", "Tap \"with UI\" ");
						messageDialog.ShowAsync().GetResults();
					}
					else
					{
						var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
						messageDialog.ShowAsync().GetResults();
					}
					break;
				}
			}

			this.recognizeOnceButton.IsEnabled = true;
			this.recognizeContinuouslyButton.Visibility = Visibility.Visible;
			this.stopRecognizingTextBlock.Visibility = this.listeningTextBlock.Visibility = Visibility.Collapsed;
			this.heardYouSayTextBlock.Visibility = this.resultTextBlock.Visibility = Visibility.Collapsed;
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
		}
	}
}
