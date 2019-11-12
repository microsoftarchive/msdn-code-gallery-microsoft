//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#if BUILD_AZURE_CLIENT_WITHOUT_WIZARD
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using Windows.Networking.PushNotifications;
using Windows.System.Profile;
using Windows.Security.Cryptography;
#endif
using Windows.UI.Xaml.Controls;


namespace Tiles
{
    public sealed partial class UsePushNotifications : Page
    {
        public UsePushNotifications()
        {
            this.InitializeComponent();
        }

        // The Windows Azure Mobile Service wizard will automatically add the necessary references to
        // your app, and set up your service in the cloud. The wizard will also provide a script
        // in the cloud that automatically sends an update back to the client whenever a push
        // notifications channel is uploaded. The wizard will also add code to your "OnLaunched" handler to 
        // automatically request a push notification channel and upload it to your service. Run the wizard, 
        // then look at App.xaml.cs to see the changes.

        // You can also set up the Mobile Service without using the wizard using the following steps:
        //
        // 1. Sign up for a free Azure trial (http://www.windowsazure.com/en-us/pricing/free-trial/) 
        // if you do not have an Azure subscription already. This will enable you to host 10 mobile services for free.
        // 2. Install Windows Azure command line tools - http://go.microsoft.com/fwlink/?LinkID=275464&clcid=0x409
        // 3. Open command line to download the credentials required to talk to Windows Azure. This is a one-time setup for 
        // running all subsequent commands to manage Windows Azure Mobile Service:
        //     a. Download Windows Azure management credentials.
        //         >> azure account download
        //         This will open up a web page to login to manage.windowsazure.com site. Once you log in, it will generate and 
        //         prompt you to download a publish settings  file for your Azure subscription. Save this file on your machine.
        //     b. Import the publishsettings file from this location. This will wire up your command line client to manage all 
        //        your Windows Azure services from the command line.
        //         >> azure account import [SavedLocation]
        // 4. Create a Windows Azure Mobile Service:
        //     >> azure mobile create [AzureMobileServiceName] [sqlAdminUsername] [sqlAdminPassword]
        // 5. Create a table in the Windows Azure Mobile Service database to store the channels:
        //     >> azure mobile table create [AzureMobileServiceName] channels
        // 6. Get the ApplicationUrl and ApplicationKey for your Mobile service and configure this in your Windows Store app 
        // in the App.xaml.cs:
        //     >> azure mobile show [AzureMobileServiceName]
        // 7. Register this Windows Store app in the Windows Store by creating a new app here:
        // http://go.microsoft.com/fwlink/p/?LinkID=266582&clcid=0x409
        // 8. Associate your app with this registered Windows Store app by right-clicking the project and then go to 
        // Store -> Associate your app with the Windows Store -> Login and follow the instructions
        // 9. Get the client secret and package SID for the registered app from Live Connect Developer center 
        // (http://go.microsoft.com/fwlink/p/?LinkId=262039&clcid=0x409 -> Edit Settings -> API Settings)
        // and run the following from command line to configure your mobile service 
        // to send push notifications:
        //     >> azure mobile config set [AzureMobileServiceName] microsoftAccountClientSecret [ClientSecret]
        //     >> azure mobile config set [AzureMobileServiceName] microsoftAccountPackageSID [PackageSID]
        // 10. Add code to the client to request and upload a channel. Running the wizard will produce such code,
        // which you can copy into your app. Be sure to add a reference to the Windows Azure Mobile Services client libraries
        // (Project -> Add reference -> Windows -> Extensions)
        // 11. Add code to the server to receive channels uploaded by the client. Again, running the wizard will produce an
        // example script, which you should copy. You can upload such a script using the following command:
        //     >> azure mobile script upload [AzureMobileServiceName] [PathToScript]
    }
}
