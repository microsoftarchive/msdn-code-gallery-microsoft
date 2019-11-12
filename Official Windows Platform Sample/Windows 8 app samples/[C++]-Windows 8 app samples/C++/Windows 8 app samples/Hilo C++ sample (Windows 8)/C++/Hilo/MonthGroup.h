// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IPhotoGroup.h"

namespace Hilo
{
    interface class IPhotoGroup;
    class ExceptionPolicy;
    class PhotoCache;
    class Repository;

    // The MonthGroup class provides data to the image browser's grid control.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MonthGroup sealed : public IPhotoGroup, public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    internal:
        MonthGroup(std::shared_ptr<PhotoCache> photoCache, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);     
        concurrency::task<Windows::Foundation::DateTime> QueryPhotosAsync();

    public:
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property Platform::String^ Title 
        { 
            virtual Platform::String^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<IPhoto^>^ Items
        {
            virtual Windows::Foundation::Collections::IObservableVector<IPhoto^>^ get();
        }

        property bool HasPhotos
        {
            bool get();
        }

        property bool IsRunning
        {
            bool get();
        }

    private:
        Platform::String^ m_title;
        Windows::Foundation::DateTime m_dateTimeForTitle;
        std::weak_ptr<PhotoCache> m_weakPhotoCache;
        Windows::Storage::Search::IStorageFolderQueryOperations^ m_folderQuery;
        std::shared_ptr<Repository> m_repository;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        Platform::Collections::Vector<IPhoto^>^ m_photos;
        size_t m_count;
        bool m_hasCount;
        bool m_runningQuery;

        void OnPropertyChanged(Platform::String^ propertyName);
    };
}
