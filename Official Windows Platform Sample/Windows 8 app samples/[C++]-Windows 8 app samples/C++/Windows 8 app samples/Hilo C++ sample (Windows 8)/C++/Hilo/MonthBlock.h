// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IMonthBlock.h"

namespace Hilo
{
    interface class IYearGroup;
    interface class IResourceLoader;
    class ExceptionPolicy;
    class Repository;

    // The MonthBlock class provides per-month data used by the image browser's 
    // zoomed-out view of the user's picture library. XAML controls bind
    // to objects of this type.
    [Windows::Foundation::Metadata::WebHostHidden]
    [Windows::UI::Xaml::Data::Bindable]
    public ref class MonthBlock sealed : public IMonthBlock, public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    internal:
        MonthBlock(IYearGroup^ yearGroup, int month, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);
        concurrency::task<void> QueryPhotoCount();

    public:
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property Platform::String^ Name 
        { 
            virtual Platform::String^ get(); 
        }

        property bool HasPhotos
        { 
            virtual bool get();
        }

        property unsigned int Month
        {
            virtual unsigned int get();
        }

        property IYearGroup^ Group
        {
            virtual IYearGroup^ get();
        }

    private:
        Platform::WeakReference m_weakYearGroup;
        int m_month;
        Platform::String^ m_name;
        Windows::Storage::Search::IStorageFolderQueryOperations^ m_folderQuery;
        std::shared_ptr<Repository> m_repository;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        unsigned int m_count;
        bool m_runOperation;
        bool m_runningOperation;

        Platform::String^ BuildDateQuery();
        void OnPropertyChanged(Platform::String^ propertyName);
    };
}
