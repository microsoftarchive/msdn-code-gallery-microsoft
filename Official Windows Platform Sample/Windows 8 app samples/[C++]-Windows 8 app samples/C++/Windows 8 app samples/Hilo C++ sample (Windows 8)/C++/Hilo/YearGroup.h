// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IYearGroup.h"

namespace Hilo
{
    interface class IMonthBlock;
    class ExceptionPolicy;
    class Repository;

    // The YearGroup class provides the data for the image browser's zoomed-out view of the user's picture library.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class YearGroup sealed : public IYearGroup, public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    internal:
        YearGroup(Windows::Foundation::DateTime yearDate, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);

    public:
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property Platform::String^ Title
        { 
            virtual Platform::String^ get();
        }

        property int Year
        {
            virtual int get();
        }

        property Windows::Foundation::Collections::IObservableVector<IMonthBlock^>^ Items
        {
            virtual Windows::Foundation::Collections::IObservableVector<IMonthBlock^>^ get();
        }

    private:
        Platform::String^ m_name;
        Windows::Foundation::DateTime m_yearDate;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        std::shared_ptr<Repository> m_repository;
        int m_year;
        Platform::Collections::Vector<IMonthBlock^>^ m_months;
        Windows::Storage::Search::IStorageFolderQueryOperations^ m_folderQuery;

        void OnPropertyChanged(Platform::String^ propertyName);
    };
}
