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
        static property Platform::String^ FEATURE_NAME
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"File access C++ sample");
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

        static property Platform::String^ Filename
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"sample.dat");
            }
        }

        property Windows::Storage::StorageFile^ SampleFile
        {
            Windows::Storage::StorageFile^ get()
            {
                return sampleFile;
            }
            void set(Windows::Storage::StorageFile^ value)
            {
                sampleFile = value;
            }
        }

        property Platform::String^ MruToken
        {
            Platform::String^ get()
            {
                return mruToken;
            }
            void set(Platform::String^ value)
            {
                mruToken = value;
            }
        }

        property Platform::String^ FalToken
        {
            Platform::String^ get()
            {
                return falToken;
            }
            void set(Platform::String^ value)
            {
                falToken = value;
            }
        }

    internal:
        void ResetScenarioOutput(Windows::UI::Xaml::Controls::TextBlock^ output);
        void NotifyUserFileNotExist();
        void HandleFileNotFoundException(Platform::COMException^ e);

    private:
        void Initialize();
        void ValidateFile(Platform::String^ scenarioName);

        static Platform::Array<Scenario>^ scenariosInner;
        Windows::Storage::StorageFile^ sampleFile;
        Platform::String^ mruToken;
        Platform::String^ falToken;
    };
}
