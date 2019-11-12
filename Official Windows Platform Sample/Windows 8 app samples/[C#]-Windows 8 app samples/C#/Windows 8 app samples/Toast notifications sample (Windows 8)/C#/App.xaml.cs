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
using SDKTemplateCS;

namespace ToastsSampleCS
{
    public partial class App
    {
        MainPage mainPage = null;
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
                //     Do an asynchronous restore
                await SuspensionManager.RestoreAsync();
            }
            if (mainPage == null)
            {
                var rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
                mainPage = rootFrame.Content as MainPage;
                mainPage.RootNamespace = this.GetType().Namespace;
            }
            if (args.Arguments != "")
            {
                (mainPage.ScenariosFrame.Content as ScenarioList).SelectedIndex = 4;
                (mainPage.InputFrame.Content as ScenarioInput5).LaunchedFromToast(args.Arguments);
            }
            
            Window.Current.Activate();
        }
    }
}
