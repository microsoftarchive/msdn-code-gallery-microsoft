//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
namespace Geofencing4SqSampleTasks
{
    sealed class Constants
    {
        // Date the FourSquare API was verified (e.g. date of validation for this app, in YYYYMMDD format).
        public const string ApiVerifiedDate = "20130423";
        public const string ClientId = "YOUR_FOURSQUARE_CLIENT_ID"; // Set your Foursquare client Id here

        public const string DeferredCheckinsFilename = "_deferredCheckins.xml";
        public const string GeofenceIdsToVenuesStateFilename = "_geofenceIdsToVenues.xml";
        public const string BackgroundEventsLogFilename = "_backgroundEvents.log";

        public const string OAuthTokenKey = "oauth";
        public const string AutoCheckinEnabledKey = "Foursquare.AutoCheckinEnabled";
    }
}
