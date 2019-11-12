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
using Tiles;
using System.Xml.Linq;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Tiles C#";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Send tile notification with text", ClassType = typeof(SendTextTile) },
            new Scenario() { Title = "Send tile notification with local images", ClassType = typeof(SendLocalImageTile) },
            new Scenario() { Title = "Send tile notification with web images", ClassType = typeof(SendWebImageTile) },
            new Scenario() { Title = "Send badge notification", ClassType = typeof(SendBadge) },
            new Scenario() { Title = "Preview all tile notification templates", ClassType = typeof(PreviewAllTemplates) },
            new Scenario() { Title = "Enable notification queue and tags", ClassType = typeof(EnableNotificationQueue) },
            new Scenario() { Title = "Use notification expiration", ClassType = typeof(NotificationExpiration) },
            new Scenario() { Title = "Image protocols and baseUri", ClassType = typeof(ImageProtocols) },
            new Scenario() { Title = "Globalization, localization, scale, and accessibility", ClassType = typeof(Globalization) }
        };

        internal static string PrettyPrint(string inputString)
        {
            XDocument doc = XDocument.Parse(inputString);
            return doc.ToString();
        }

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped, 
            // it must attempt to unsnap first.
            return ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
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