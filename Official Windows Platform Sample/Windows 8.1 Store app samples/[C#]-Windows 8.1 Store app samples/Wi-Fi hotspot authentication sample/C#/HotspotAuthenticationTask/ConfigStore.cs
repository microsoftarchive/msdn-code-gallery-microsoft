//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Networking.NetworkOperators;

// The namespace for the background tasks.
namespace HotspotAuthenticationTask
{
    // A helper class for providing the application configuration.
    public sealed class ConfigStore
    {
        // For the sake of simplicity of the sample, the following authentication parameters are hard coded:
        public static string AuthenticationHost
        {
            get { return "login.contoso.com"; }
        }

        public static string UserName
        {
            get { return "MyUserName"; }
        }

        public static string Password
        {
            get { return "MyPassword"; }
        }

        public static string ExtraParameters
        {
            get { return ""; }
        }

        public static bool MarkAsManualConnect
        {
            get { return false; }
        }

        public static bool UseNativeWISPr
        {
            get
            {
                object value;
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("usenativewispr", out value) &&
                    value is bool)
                    return (bool)value;
                return true;  // default value
            }
            set { ApplicationData.Current.LocalSettings.Values["usenativewispr"] = value; }
        }

        // This flag is set by the foreground app to toogle authentication to be done by the
        // background task handler.
        public static bool AuthenticateThroughBackgroundTask
        {
            get {
                object value;
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("background", out value) &&
                    value is bool)
                    return (bool)value;
                return true;  // default value
            }

            set { ApplicationData.Current.LocalSettings.Values["background"] = value; }
        }

        // This item is set by the background task handler to pass an authentication event
        // token to the foreground app.
        public static string AuthenticationToken
        {
            get
            {
                object value;
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("token", out value) &&
                    value is string)
                    return value as string;
                return "";
            }

            set { ApplicationData.Current.LocalSettings.Values["token"] = value; }
        }
    }
}
