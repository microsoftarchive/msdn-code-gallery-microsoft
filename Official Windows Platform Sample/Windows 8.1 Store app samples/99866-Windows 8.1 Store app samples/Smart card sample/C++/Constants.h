//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#define ADMIN_KEY_LENGTH_IN_BYTES 24

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    internal:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {
                return "Smart Card"; 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            {
                return scenariosInner; 
            }
        }

         property Windows::Storage::Streams::IBuffer^ AdminKey
        {
            Windows::Storage::Streams::IBuffer^  get()
            {
                return adminkey;
            }

            void set(Windows::Storage::Streams::IBuffer^  value)
            {
                adminkey = value;
            }
        }

        property Platform::String^ SmartCardReaderDeviceId
        {
            Platform::String^  get()
            {
                return deviceId;
            }

            void set( Platform::String^  value)
            {
                deviceId = value;
            }
        }

        Concurrency::task<Windows::Devices::SmartCards::SmartCard^>
        GetSmartCard();

    private:
        static Platform::Array<Scenario>^ scenariosInner;
        Windows::Storage::Streams::IBuffer^ adminkey;
        Platform::String^ deviceId;
    };

    namespace SmartCardSample
    {
        // Sample specific types should be in this namespace
    }
}
