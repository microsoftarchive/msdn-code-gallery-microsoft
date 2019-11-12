// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AssociationLaunching;

namespace AssociationLaunching
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            this.Suspending += new SuspendingEventHandler(OnSuspending);
        }

        async protected void OnSuspending(object sender, SuspendingEventArgs args)
        {
            SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        async protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //  Do an asynchronous restore.
                await SuspensionManager.RestoreAsync();
            }

            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(rootPage));
            Window.Current.Content = rootFrame;
            rootPage p = rootFrame.Content as rootPage;
            p.RootNamespace = this.GetType().Namespace;
            p.FileEvent = null;
            p.ProtocolEvent = null;

            Window.Current.Activate();
        }

        // Handle file activations.
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(rootPage));
            Window.Current.Content = rootFrame;
            rootPage p = rootFrame.Content as rootPage;
            p.RootNamespace = this.GetType().Namespace;

            // Shuttle the event args to the scenario selector to display the proper scenario.
            p.FileEvent = args;
            p.ProtocolEvent = null;

            Window.Current.Activate();
        }

        // Handle protocol activations.
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;

                var rootFrame = new Frame();
                rootFrame.Navigate(typeof(rootPage));
                Window.Current.Content = rootFrame;
                rootPage p = rootFrame.Content as rootPage;
                p.RootNamespace = this.GetType().Namespace;

                // Shuttle the event args to the scenario selector to display the proper scenario.
                p.ProtocolEvent = protocolArgs;
                p.FileEvent = null;
            }

            Window.Current.Activate();
        }
    }
}