//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using MediaExtensions;
using System;
using System.Collections.Generic;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Media Extensions";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Local decoder", ClassType = typeof(CustomDecoder) },
            new Scenario() { Title = "Local scheme handler", ClassType = typeof(SchemeHandler) },
            new Scenario() { Title = "Video Stabilization Effect", ClassType = typeof(VideoStabilization) },
            new Scenario() { Title = "Custom Video Effect", ClassType = typeof(VideoEffect) }
        };

        private MediaExtensionManager _extensionManager = new MediaExtensionManager();

        public MediaExtensionManager ExtensionManager
        {
            get { return _extensionManager; }
        }

        /// <summary>
        /// Common video failed error handler.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void VideoOnError(Object obj, ExceptionRoutedEventArgs args)
        {
            NotifyUser("Cannot open video file - error: " + args.ErrorMessage, NotifyType.ErrorMessage);
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
