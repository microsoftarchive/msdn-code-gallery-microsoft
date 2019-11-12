//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME {
            Platform::String^ get() {  return ref new Platform::String(L"Simple Communication"); }
        }

        static property Platform::Array<Scenario>^ scenarios {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

        static void EnsureExtensionRegistration()
        {
            mediaExtensionManager = ref new Windows::Media::MediaExtensionManager();
            mediaExtensionManager->RegisterSchemeHandler("Microsoft.Samples.SimpleCommunication.StspSchemeHandler", "stsp:");
        }

    private:
        static Platform::Array<Scenario>^ scenariosInner;
        static Windows::Media::MediaExtensionManager^ mediaExtensionManager;
    };


}
