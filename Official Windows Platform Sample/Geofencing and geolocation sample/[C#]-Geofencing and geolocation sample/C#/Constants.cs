//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
namespace Geofencing4SqSample
{
    sealed class Constants
    {
        // Date the FourSquare API was verified (e.g. date of validation for this app, in YYYYMMDD format).
        public const string ApiVerifiedDate = "20130423";
        public const string ClientId = "YOUR_FOURSQUARE_CLIENT_ID"; // Set your Foursquare client Id here
 
        // Important: the App Sid is generated from your Package Display Name set in Package.appxmanifest.
        // This must match the redirect URL to your app settings on FourSquare developer site.
        public const string AppSid = "YOUR_APP_SID"; // Set your app SID here

        public const string DeferredCheckinsFilename = "_deferredCheckins.xml";
        public const string GeofenceIdsToVenuesStateFilename = "_geofenceIdsToVenues.xml";
        public const string BackgroundEventsLogFilename = "_backgroundEvents.log";

        public const string OAuthTokenKey = "oauth";

        public const double DefaultMapZoomLevel = 15;
        public const double VenuesQueryDistanceThresholdMeters = 100;
    }
}
