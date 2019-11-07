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
                return ref new Platform::String(L"Removable Storage");
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

        property Windows::Foundation::Collections::IVectorView<Windows::Storage::IStorageItem^>^ FileActivationFiles
        {
            Windows::Foundation::Collections::IVectorView<Windows::Storage::IStorageItem^>^ get()
            {
                return fileActivationFiles;
            }

            void set(Windows::Foundation::Collections::IVectorView<Windows::Storage::IStorageItem^>^ value)
            {
                fileActivationFiles = value;
            }
        }

        property Windows::Storage::StorageFolder^ AutoplayFileSystemDeviceFolder
        {
            Windows::Storage::StorageFolder^ get()
            {
                return autoplayFileSystemDeviceFolder;
            }

            void set(Windows::Storage::StorageFolder^ value)
            {
                autoplayFileSystemDeviceFolder = value;
            }
        }

        property Platform::String^ AutoplayNonFileSystemDeviceId
        {
            Platform::String^ get()
            {
                return autoplayNonFileSystemDeviceId;
            }

            void set(Platform::String^ value)
            {
                autoplayNonFileSystemDeviceId = value;
            }
        }

        // Selects and loads the Autoplay scenario
        void LoadAutoplayScenario()
        {
            LoadScenario((safe_cast<Windows::UI::Xaml::Controls::ListBoxItem^>(Scenarios->SelectedItem))->Name);
            Scenarios->SelectedIndex = MainPage::autoplayScenarioIndex;
        }

    private:
        static Platform::Array<Scenario>^ scenariosInner;
        static int autoplayScenarioIndex;

        // Contains the list of Windows.Storage.StorageItem's provided when this application is activated to handle
        // the supported file types specified in the manifest (here, these will be image files).
        Windows::Foundation::Collections::IVectorView<Windows::Storage::IStorageItem^>^ fileActivationFiles;

        // Contains the storage folder (representing a file-system removable storage) when this application is
        // activated by Content Autoplay.
        Windows::Storage::StorageFolder^ autoplayFileSystemDeviceFolder;

        // Contains the device identifier (representing a non-file system removable storage) provided when this application
        // is activated by Device Autoplay.
        Platform::String^ autoplayNonFileSystemDeviceId;
    };

    partial ref class App
    {
    public:
        virtual void OnFileActivated(Windows::ApplicationModel::Activation::FileActivatedEventArgs^ args) override;
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ args) override;
    };
}
