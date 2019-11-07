//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

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
                return "File Revocation Manager"; 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

        static property Platform::String^ PickedFolderToken
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"PickedFolderToken");
            }
        }

        static property Platform::String^ SampleFilename
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"RevokeSample.txt");
            }
        }

        static property Platform::String^ TargetFilename
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"RevokeTarget.txt");
            }
        }

        static property Platform::String^ SampleFoldername
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"RevokeSample");
            }
        }

        static property Platform::String^ TargetFoldername
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"RevokeTarget");
            }
        }

        property Windows::Storage::StorageFolder^ PickedFolder
        {
            Windows::Storage::StorageFolder^ get()
            {
                return pickedFolder;
            }
            void set(Windows::Storage::StorageFolder^ value)
            {
                pickedFolder = value;
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

        property Windows::Storage::StorageFile^ TargetFile
        {
            Windows::Storage::StorageFile^ get()
            {
                return targetFile;
            }
            void set(Windows::Storage::StorageFile^ value)
            {
                targetFile = value;
            }
        }

        property Windows::Storage::StorageFolder^ SampleFolder
        {
            Windows::Storage::StorageFolder^ get()
            {
                return sampleFolder;
            }
            void set(Windows::Storage::StorageFolder^ value)
            {
                sampleFolder = value;
            }
        }

        property Windows::Storage::StorageFolder^ TargetFolder
        {
            Windows::Storage::StorageFolder^ get()
            {
                return targetFolder;
            }
            void set(Windows::Storage::StorageFolder^ value)
            {
                targetFolder = value;
            }
        }

        void NotifyUserFileNotExist();
        void HandleFileNotFoundException(Platform::COMException^ e);

    private:
        static Platform::Array<Scenario>^ scenariosInner;
        Windows::Storage::StorageFolder^ pickedFolder;
        Windows::Storage::StorageFile^ sampleFile;
        Windows::Storage::StorageFile^ targetFile;
        Windows::Storage::StorageFolder^ sampleFolder;
        Windows::Storage::StorageFolder^ targetFolder;
    };

    namespace FileRevocation
    {
        // Sample specific types should be in this namespace
    }
}
