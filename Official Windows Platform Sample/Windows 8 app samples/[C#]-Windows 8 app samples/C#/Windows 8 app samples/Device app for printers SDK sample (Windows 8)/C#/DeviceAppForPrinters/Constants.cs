//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

using Windows.Devices.Printers.Extensions;
using Microsoft.Samples.Printers.Extensions;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Device App For Printers C#";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Ink Level", ClassType = typeof(DeviceAppForPrinters.InkLevel) },
            new Scenario() { Title = "Advanced Settings", ClassType = typeof(DeviceAppForPrinters.Preferences) },
        };

        public PrintTaskConfiguration Config;
        public Object Context;

        public void LoadAdvancedPrintSettingsContext(PrintTaskSettingsActivatedEventArgs args)
        {
            Config = args.Configuration;
            Context = Config.PrinterExtensionContext;
            LoadScenario(typeof(DeviceAppForPrinters.Preferences));
        }
    }

    partial class App : Application
    {
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.PrintTaskSettings)
            {
                Frame rootFrame = new Frame();
                if (null == Window.Current.Content)
                {
                    rootFrame.Navigate(typeof(MainPage));
                    Window.Current.Content = rootFrame;
                }
                Window.Current.Activate();

                MainPage mainPage = (MainPage)rootFrame.Content;

                // Load advanced printer preferences scenario
                mainPage.LoadAdvancedPrintSettingsContext((PrintTaskSettingsActivatedEventArgs)args);
            }
        }
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
