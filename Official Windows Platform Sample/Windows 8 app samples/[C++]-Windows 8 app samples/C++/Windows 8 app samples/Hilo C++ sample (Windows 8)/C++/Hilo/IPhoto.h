// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IPhotoGroup;

    // The IPhoto class defines the signature of data used by XAML image controls.
    [Windows::Foundation::Metadata::WebHostHidden]
    public interface class IPhoto
    {
        property IPhotoGroup^ Group
        {
            IPhotoGroup^ get();
        }

        property Platform::String^ Name { Platform::String^ get(); }
        
        property Platform::String^ Path
        {
            Platform::String^ get();
        }

        property Platform::String^ FormattedPath
        {
            Platform::String^ get();
        }
        
        property Platform::String^ FileType
        {
            Platform::String^ get();
        }

        property Windows::Foundation::DateTime DateTaken
        {
            Windows::Foundation::DateTime get();
        }

        property Platform::String^ FormattedDateTaken
        {
            Platform::String^ get();
        }

        property Platform::String^ FormattedTimeTaken
        {
            Platform::String^ get();
        }

        property Platform::String^ Resolution
        {
            Platform::String^ get();
        }

        property uint64 FileSize
        {
            uint64 get();
        }

        property Platform::String^ DisplayType
        {
            Platform::String^ get();
        }

        property Windows::UI::Xaml::Media::Imaging::BitmapImage^ Thumbnail 
        { 
            Windows::UI::Xaml::Media::Imaging::BitmapImage^ get();
        }

        property Windows::UI::Xaml::Media::Imaging::BitmapImage^ Image
        { 
            Windows::UI::Xaml::Media::Imaging::BitmapImage^ get();
        }

        property bool IsInvalidThumbnail
        {
            bool get();
        }

        void ClearImageData();

        Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::IRandomAccessStreamWithContentType^>^ OpenReadAsync();
    };
}