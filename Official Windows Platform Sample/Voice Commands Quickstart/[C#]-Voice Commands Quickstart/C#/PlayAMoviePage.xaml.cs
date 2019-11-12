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
using VoiceCommandsQuickstart.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoiceCommandsQuickstart
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PlayAMoviePage : Page
	{
		public PlayAMoviePage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.New)
			{
				SuspensionManager.SessionState["PlayAMoviePage"] = e.Parameter.ToString();
			}

			string[] parameters = SuspensionManager.SessionState["PlayAMoviePage"].ToString().Split('|');

			if (parameters.Length > 1)
			{
				movieSearchTextBlock.Text = parameters[1];
				string feedback = string.Format("VoiceCommandsQuickstart has launched and is now playing the movie \"{0}\".", movieSearchTextBlock.Text);
				feedbackTextBlock.Text = feedback;

				// Only give audible feedback if the commandMode key in the SpeechRecognitionResult's SemanticInterpretation
				// collection is not "text". If the app was launched by typing, rather than voice, then we should give silent
				// feedback as a courtesy.
				if (parameters[0] != "text" && e.NavigationMode == NavigationMode.New)
				{
					var speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
					var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(feedback);
					feedbackMediaElement.SetSource(stream, stream.ContentType);
					feedbackMediaElement.Play();
				}
			}
		}

		private void HowToUse_Click(object sender, RoutedEventArgs e)
		{
			(Window.Current.Content as Frame).Navigate(typeof(MainPage));
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
		}
	}
}
