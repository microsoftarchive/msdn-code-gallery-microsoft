// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IResizable.h"
#include "IPhoto.h"

namespace Hilo 
{
    interface class IPhotoGroup;
    class ExceptionPolicy;

    // The Photo class provides data used by XAML image controls.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Photo sealed : public IResizable, public IPhoto, public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    internal:
        Photo(Windows::Storage::BulkAccess::FileInformation^ file, IPhotoGroup^ photoGroup, std::shared_ptr<ExceptionPolicy> exceptionPolicy);

    public:
        virtual ~Photo();

        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property IPhotoGroup^ Group
        {
            virtual IPhotoGroup^ get();
        }

        property Platform::String^ Name 
        { 
            virtual Platform::String^ get(); 
        }

        property Platform::String^ Path
        {
            virtual Platform::String^ get();
        }

        property Platform::String^ FormattedPath
        {
            virtual Platform::String^ get();
        }

        property Platform::String^ FileType
        {
            virtual Platform::String^ get();
        }

        property Windows::Foundation::DateTime DateTaken
        {
            virtual Windows::Foundation::DateTime get();
        }

        property Platform::String^ FormattedDateTaken
        {
            virtual Platform::String^ get();
        }

        property Platform::String^ FormattedTimeTaken
        {
            virtual Platform::String^ get();
        }

        property Platform::String^ Resolution
        {
            virtual Platform::String^ get();
        }

        property uint64 FileSize
        {
            virtual uint64 get();
        }

        property Platform::String^ DisplayType
        {
            virtual Platform::String^ get();
        }

        property Windows::UI::Xaml::Media::Imaging::BitmapImage^ Thumbnail 
        { 
            virtual Windows::UI::Xaml::Media::Imaging::BitmapImage^ get();
        }

        property Windows::UI::Xaml::Media::Imaging::BitmapImage^ Image
        { 
            virtual Windows::UI::Xaml::Media::Imaging::BitmapImage^ get();
        }

        property bool IsInvalidThumbnail
        {
            virtual bool get();
        }

        property int ColumnSpan 
        {
            virtual int get();
            virtual void set(int value);
        }

        property int RowSpan
        {
            virtual int get();
            virtual void set(int value);
        }

        virtual Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::IRandomAccessStreamWithContentType^>^ OpenReadAsync();
        virtual void ClearImageData();

    internal:
        concurrency::task<void> QueryPhotoImageAsync();

    private:
        Windows::Storage::BulkAccess::FileInformation^ m_fileInfo;
        Platform::WeakReference m_weakPhotoGroup;
        Windows::UI::Xaml::Media::Imaging::BitmapImage^ m_image;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        Windows::Foundation::EventRegistrationToken m_thumbnailUpdatedEventToken;
        Windows::Foundation::EventRegistrationToken m_imageFailedEventToken;
        int m_columnSpan;
        int m_rowSpan;
        bool m_queryPhotoImageAsyncIsRunning;
        bool m_isInvalidThumbnail;

        void OnThumbnailUpdated(Windows::Storage::BulkAccess::IStorageItemInformation^ sender, Platform::Object^ e);
        void OnImageFailedToOpen(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e);
        void OnPropertyChanged(Platform::String^ propertyName);
    };
}