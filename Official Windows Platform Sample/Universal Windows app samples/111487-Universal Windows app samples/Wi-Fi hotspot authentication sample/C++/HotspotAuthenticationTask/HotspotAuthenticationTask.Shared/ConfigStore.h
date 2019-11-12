//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
//
// ConfigStore.h
// Utility for retrieving configuration parameters
//

#pragma once

// The namespace for the background tasks.
namespace HotspotAuthenticationTask
{
    // A helper class for providing the application configuration.
    public ref class ConfigStore sealed
    {
    public:
        // For the sake of simplicity of the sample, the following authentication parameters are hard coded:
        static property Platform::String^ AuthenticationHost
        {
            Platform::String^ get();
        }

        static property Platform::String^ UserName
        {
            Platform::String^ get();
        }

        static property Platform::String^ Password
        {
            Platform::String^ get();
        }

        static property Platform::String^ ExtraParameters
        {
            Platform::String^ get();
        }

        static property bool MarkAsManualConnect
        {
            bool get();
        }

        // This flag is set by the foreground app to toogle authentication to be done by the
        // background task handler.
        static property bool AuthenticateThroughBackgroundTask
        {
            bool get();
            void set(bool value);
        }

        // This item is set by the background task handler to pass an authentication event
        // token to the foreground app.
        static property Platform::String^ AuthenticationToken
        {
            Platform::String^ get();
            void set(Platform::String^ value);
        }
    };
}
