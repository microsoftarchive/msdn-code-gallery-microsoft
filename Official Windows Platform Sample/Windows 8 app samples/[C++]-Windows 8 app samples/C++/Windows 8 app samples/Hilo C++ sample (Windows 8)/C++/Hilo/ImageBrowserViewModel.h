// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ViewModelBase.h"

namespace Hilo
{
    interface class IPhoto;
    interface class IPhotoGroup;
    interface class IYearGroup;
    class concurrency::cancellation_token_source;
    class ExceptionPolicy;
    class PhotoCache;
    class Repository;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    // The ImageBrowserViewModel class contains the presentation logic for the image browser page (ImageBrowserView.xaml).
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ImageBrowserViewModel sealed : public ViewModelBase
    {
    internal:
        ImageBrowserViewModel(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        IPhoto^ GetPhotoForYearAndMonth(unsigned int year, unsigned int month);

    public:
        virtual ~ImageBrowserViewModel();

        property Windows::Foundation::Collections::IObservableVector<IPhotoGroup^>^ MonthGroups
        {
            Windows::Foundation::Collections::IObservableVector<IPhotoGroup^>^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<IYearGroup^>^ YearGroups
        {
            Windows::Foundation::Collections::IObservableVector<IYearGroup^>^ get();
        }

        property bool InProgress
        {
            bool get();
        }

        property Windows::UI::Xaml::Input::ICommand^ GroupCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CropImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ RotateImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CartoonizeImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        // These are really IPhoto^ but XAML cannot convert any underlying
        // real object (e.g. Photo) back to IPhoto which results in strange
        // side effects (such as the AppBar not working).
        property Platform::Object^ SelectedItem
        {
            Platform::Object^ get();
            void set(Platform::Object^ value);
        }

    private:
        enum class Mode {
            Default,        /* (0, 0, 0): no pending data changes, not updating, not visible */
            Active,         /* (0, 0, 1): no pending data changes, not updating, visible */
            Pending,        /* (1, 0, 0): pending data changes, not updating, not visible */
            Running,        /* (0, 1, 1): no pending data changes, updating, visible  */    
            NotAllowed      /* error state */
        };

        Platform::Collections::Vector<IPhotoGroup^>^ m_monthGroups;
        Platform::Collections::Vector<IYearGroup^>^ m_yearGroups;
        Windows::UI::Xaml::Input::ICommand^ m_groupCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cropImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_rotateImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cartoonizeImageCommand;

        bool m_runningMonthQuery;
        bool m_runningYearQuery;
        int m_currentQueryId;
        Mode m_currentMode;
        ULONGLONG m_lastFileChangeTime;
        bool m_hasFileUpdateTask;

        concurrency::cancellation_token_source m_cancellationTokenSource;
        std::shared_ptr<PhotoCache> m_photoCache;
        IPhoto^ m_photo;
        std::shared_ptr<Repository> m_repository;

        void NavigateToGroup(Platform::Object^ parameter);
        void CropImage(Platform::Object^ parameter);
        void RotateImage(Platform::Object^ parameter);
        void CartoonizeImage(Platform::Object^ parameter);
        bool CanProcessImage(Object^ parameter);
        void OnDataChanged();
        void ObserveFileChange();
        void CancelMonthAndYearQueries();
        void StartMonthAndYearQueries();
        void StartMonthQuery(int queryId, concurrency::cancellation_token token);
        void StartYearQuery(int queryId, concurrency::cancellation_token token);
        void FinishMonthAndYearQueries();
    };
}