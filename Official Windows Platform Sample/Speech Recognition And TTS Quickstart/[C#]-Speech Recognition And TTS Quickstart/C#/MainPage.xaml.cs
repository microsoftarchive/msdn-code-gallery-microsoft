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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SpeechRecognitionAndTTSQuickstart
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;
		}


		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Prepare page for display here.
		}

		private void PredefinedDictationGrammar_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(PredefinedDictationGrammarPage));
		}

		private void PredefinedWebSearchGrammar_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(PredefinedWebSearchGrammarPage));
		}

		private void ListConstraint_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(ListConstraintPage));
		}

		private void SRGSGrammarFileConstraint_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(SRGSGrammarFileConstraintPage));
		}

		private void TextToSpeech_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(TextToSpeechPage));
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
		}
	}
}