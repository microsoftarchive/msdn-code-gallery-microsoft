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

    class Errors
    {
    public:
        static Platform::String^ NoExifThumbnail;
        static Platform::String^ NoThumbnail;
        static Platform::String^ NoAlbumArt;
        static Platform::String^ NoIcon;
        static Platform::String^ NoImages;
        static Platform::String^ FileGroupEmpty;
        static Platform::String^ FileGroupLocation;
        static Platform::String^ Cancel;
    };

    class FileExtensions
    {
    public:
        static Platform::Array<Platform::String^>^ Document;
        static Platform::Array<Platform::String^>^ Image;
        static Platform::Array<Platform::String^>^ Music;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"File thumbnails C++ sample");
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

    internal:
        bool EnsureUnsnapped();
        void ResetOutput(Windows::UI::Xaml::Controls::Image^ image, Windows::UI::Xaml::Controls::TextBlock^ output, Windows::UI::Xaml::Controls::TextBlock^ outputDetails);
        static void DisplayResult(Windows::UI::Xaml::Controls::Image^ image, Windows::UI::Xaml::Controls::TextBlock^ textBlock,
                                  Platform::String^ thumbnailModeName, size_t size,
                                  Windows::Storage::IStorageItem^ item, Windows::Storage::FileProperties::StorageItemThumbnail^ thumbnail,
                                  bool isGroup);

    private:
        static Platform::Array<Scenario>^ scenariosInner;

        Windows::UI::Xaml::Media::Imaging::BitmapImage^ _placeholder;
        Windows::UI::Xaml::Media::Imaging::BitmapImage^ GetPlaceHolderImage();
    };
}
