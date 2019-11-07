// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once
#include "Common\LayoutAwarePage.h"
#include "PageType.h"
#include "ViewModelBase.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267274 for info about the Hilo app.

    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app�s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.

    // The HiloPage class contains common logic used by all pages in the app.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class HiloPage : Common::LayoutAwarePage
    {
    internal:
        HiloPage();

    public:
        // Hilo Page
        static property Windows::UI::Xaml::DependencyProperty^ HiloDataContextProperty
        {
            Windows::UI::Xaml::DependencyProperty^ get();
        }

    protected:
        // Hilo Page
        virtual void NavigateBack();
        virtual void NavigateHome();
        virtual void NavigateToPage(PageType page, Platform::Object^ parameter);        
        virtual void LoadState(Platform::Object^ navigationParameter,
            Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
        virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    internal:
        static property bool IsSuspending;
        static void OnHiloDataContextPropertyChanged(Windows::UI::Xaml::DependencyObject^ element, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);

    private:     
        // Hilo Page
        Windows::Foundation::EventRegistrationToken m_navigateBackEventToken;
        Windows::Foundation::EventRegistrationToken m_navigateHomeEventToken;
        Windows::Foundation::EventRegistrationToken m_navigateToPageEventToken;
        bool m_hasHandlers;

        void AttachNavigationHandlers(Hilo::ViewModelBase^ viewModel);
        void DetachNavigationHandlers(Hilo::ViewModelBase^ viewModel);
    };
}
