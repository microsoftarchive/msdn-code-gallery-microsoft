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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace VoiceCommandsQuickstart
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private Frame rootFrame;
		private TransitionCollection transitions;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			this.Suspending += this.OnSuspending;
			HardwareButtons.BackPressed += HardwareButtons_BackPressed;
		}

		/// <summary>
		/// Handles back button press. If app is at the root page of app, don't go back and the
		/// system will suspend the app.
		/// </summary>
		/// <param name="sender">The source of the BackPressed event.</param>
		/// <param name="e">Details for the BackPressed event.</param>
		private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
		{
			if (this.rootFrame == null)
			{
				return;
			}

			if (this.rootFrame.CanGoBack)
			{
				this.rootFrame.GoBack();
				e.Handled = true;
			}
		}

		/// <summary>
		/// Both the OnLaunched and OnActivated event handlers need to make sure the root frame has been created, so the common 
		/// code to do that is factored into this method and called from both.
		/// </summary>
		private async void EnsureRootFrame(ApplicationExecutionState previousExecutionState)
		{
			this.rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (this.rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				this.rootFrame = new Frame();

				//Associate the frame with a SuspensionManager key                                
				SuspensionManager.RegisterFrame(this.rootFrame, "AppFrame");

				this.rootFrame.CacheSize = 1;

				if (previousExecutionState == ApplicationExecutionState.Terminated)
				{
					// Load state from previously suspended application
					try
					{
						await SuspensionManager.RestoreAsync();
					}
					catch (SuspensionManagerException)
					{
						//Something went wrong restoring state.
						//Assume there is no state and continue
					}
				}

				// Place the frame in the current Window
				Window.Current.Content = this.rootFrame;
			}

			// Ensure the current window is active
			Window.Current.Activate();
		}

		/// <summary>
		/// Invoked when the application is activated.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnActivated(IActivatedEventArgs e)
		{
			// Was the app activated by a voice command?
			if (e.Kind != Windows.ApplicationModel.Activation.ActivationKind.VoiceCommand)
			{
				return;
			}

			var commandArgs = e as Windows.ApplicationModel.Activation.VoiceCommandActivatedEventArgs;
			Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

			// The commandMode is either "voice" or "text", and it indicates how the voice command was entered by the user.
			// We should respect "text" mode by providing feedback in a silent form.
			string commandMode = this.SemanticInterpretation("commandMode", speechRecognitionResult);

			// If so, get the name of the voice command, the actual text spoken, and the value of Command/Navigate@Target.
			string voiceCommandName = speechRecognitionResult.RulePath[0];
			string textSpoken = speechRecognitionResult.Text;
			string navigationTarget = this.SemanticInterpretation("NavigationTarget", speechRecognitionResult);

			Type navigateToPageType = typeof(MainPage);
			string navigationParameterString = string.Empty;

			switch (voiceCommandName)
			{
				case "showASection":
				case "goToASection":
					string newspaperSection = this.SemanticInterpretation("newspaperSection", speechRecognitionResult);
					navigateToPageType = typeof(ShowASectionPage);
					navigationParameterString = string.Format("{0}|{1}", commandMode, newspaperSection);
					break;

				case "message":
				case "text":
					string contact = this.SemanticInterpretation("contact", speechRecognitionResult);
					string msgText = this.SemanticInterpretation("msgText", speechRecognitionResult);
					navigateToPageType = typeof(MessagePage);
					navigationParameterString = string.Format("{0}|{1}|{2}", commandMode, contact, msgText);
					break;

				case "playAMovie":
					string movieSearch = this.SemanticInterpretation("movieSearch", speechRecognitionResult);
					navigateToPageType = typeof(PlayAMoviePage);
					navigationParameterString = string.Format("{0}|{1}", commandMode, movieSearch);
					break;

				default:
					// There is no match for the voice command name.
					break;
			}

			this.EnsureRootFrame(e.PreviousExecutionState);
			if (!this.rootFrame.Navigate(navigateToPageType, navigationParameterString))
			{
				throw new Exception("Failed to create voice command page");
			}
		}

		private string SemanticInterpretation(string key, Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult)
		{
			if (speechRecognitionResult.SemanticInterpretation.Properties.ContainsKey(key))
			{
				return speechRecognitionResult.SemanticInterpretation.Properties[key][0];
			}
			else
			{
				return "unknown";
			}
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user. Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			this.EnsureRootFrame(e.PreviousExecutionState);

			if (this.rootFrame.Content == null)
			{
				// Removes the turnstile navigation for startup.
				if (this.rootFrame.ContentTransitions != null)
				{
					this.transitions = new TransitionCollection();
					foreach (var c in this.rootFrame.ContentTransitions)
					{
						this.transitions.Add(c);
					}
				}

				this.rootFrame.ContentTransitions = null;
				this.rootFrame.Navigated += this.RootFrame_FirstNavigated;

				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				if (!this.rootFrame.Navigate(typeof(MainPage), e.Arguments))
				{
					throw new Exception("Failed to create initial page");
				}
			}

			// The app must install its command sets at least once. Doing this in OnLaunched
			// causes it to happen as infrequently as possible.
			var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///QuickstartCommands.xml"));
			await Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(storageFile);

		}

		/// <summary>
		/// Restores the content transitions after the app has launched.
		/// </summary>
		/// <param name="sender">The object where the handler is attached.</param>
		/// <param name="e">Details about the navigation event.</param>
		private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
		{
			var rootFrame = sender as Frame;
			rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
			rootFrame.Navigated -= this.RootFrame_FirstNavigated;
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private async void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();

			// Save application state (and stop any background activity if applicable)
			await SuspensionManager.SaveAsync();

			deferral.Complete();
		}
	}
}