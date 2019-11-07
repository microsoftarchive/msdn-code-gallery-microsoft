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
using HttpControlChannelTrigger;
using System.Diagnostics;
using DiagnosticsHelper;


namespace HttpControlChannelTrigger
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
            		try
            		{
                		SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();
                		await SuspensionManager.SaveAsync();
                		deferral.Complete();
            		}
            		catch (Exception ex)
            		{
                		Diag.DebugPrint("Exception occured while saving session on suspension of this app. exception: "+ ex.ToString());
            		}
		}

        	static bool initialized = false;

		async protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
            		if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            		{
                		//     Do an asynchronous restore
                		await SuspensionManager.RestoreAsync();

            		}
            		if (initialized == false)
            		{
                		var rootFrame = new Frame();
                		rootFrame.Navigate(typeof(MainPage));
                		Window.Current.Content = rootFrame;
                		MainPage p = rootFrame.Content as MainPage;
                		p.RootNamespace = this.GetType().Namespace;
                		initialized = true;
            		}
            		Window.Current.Activate();
		}
	}
}
