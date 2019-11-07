using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CSReceiveUri
{
    sealed partial class App : Application
    {
        #region look this
        /// <summary>
        /// For example a uri had been requested,and the scheme is test-launchmainpage.
        /// In OS, the test-launchmainpage will be regist as a protocol, and the protocal have a default handle app,
        /// OS will launch the default app to handle the request.
        /// When the handle app is launching, the OnActivated will be trigger
        /// </summary>
        /// <param name="args"></param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame == null)
                {
                    rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                    rootFrame.NavigationFailed += OnNavigationFailed;
                }

                //because this is in (args.Kind == ActivationKind.Protocol) block, so the type of args must is ProtocolActivatedEventArgs
                //convert to type ProtocolActivatedEventArgs, and we can visit Uri property in type ProtocolActivatedEventArgs
                var protocolEventArgs = args as ProtocolActivatedEventArgs;
                //Switch to a view by Scheme
                switch (protocolEventArgs.Uri.Scheme)
                {
                    //under case is the protocol scheme in the Package.appxmanifest
                    //Navigate to target page with Uri as parameter
                    case "test-launchmainpage":
                        rootFrame.Navigate(typeof(MainPage), protocolEventArgs.Uri);
                        break;
                    case "test-launchpage1":
                        rootFrame.Navigate(typeof(Page1), protocolEventArgs.Uri);
                        break;
                    default:
                        rootFrame.Navigate(typeof(MainPage), protocolEventArgs.Uri);
                        break;
                }

                //start show UI
                Window.Current.Activate();
            }
        }
        #endregion

        #region Ignore this
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        #endregion
    }
}
