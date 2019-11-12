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
    interface class IPhoto;
    class ExceptionPolicy;
    class Repository;

    // The HubPhotoGroup class provides data for the main hub page's grid control.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class HubPhotoGroup sealed : public IPhotoGroup, public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    internal:
        HubPhotoGroup(Platform::String^ title, Platform::String^ emptyTitle, std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);
        concurrency::task<void> QueryPhotosAsync();

    public:
        virtual ~HubPhotoGroup();

        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property Platform::String^ Title 
        { 
            virtual Platform::String^ get(); 
        }

        property Windows::Foundation::Collections::IObservableVector<IPhoto^>^ Items
        {
            virtual Windows::Foundation::Collections::IObservableVector<IPhoto^>^ get();
        }

    private:
        Platform::String^ m_title;
        Platform::String^ m_emptyTitle;
        std::shared_ptr<Repository> m_repository;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        Platform::Collections::Vector<IPhoto^>^ m_photos;
        bool m_retrievedPhotos;
        bool m_receivedChangeWhileRunning;
        bool m_runningQuery;
        bool m_hasFileUpdateTask;
        ULONGLONG m_lastFileChangeTime;

        void OnDataChanged();
        void ObserveFileChange();
        void OnPropertyChanged(Platform::String^ propertyName);
    };
}
