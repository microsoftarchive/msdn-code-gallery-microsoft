// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once
#include "Common\BindableBase.h"
#include "PageType.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    class ExceptionPolicy;

    public delegate void NavigateEventHandler();
    public delegate void PageNavigateEventHandler(PageType page, Platform::Object^ parameter);

    // The ViewModelBase class contains the common presentation logic used by all view models in Hilo.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ViewModelBase : public Common::BindableBase
    {
    internal:
        ViewModelBase(std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        event NavigateEventHandler^ NavigateBack;
        event NavigateEventHandler^ NavigateHome;
        event PageNavigateEventHandler^ NavigateToPage;

        virtual void LoadState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap);
        virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap);
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);

    public:
        property bool IsAppBarEnabled { bool get(); void set(bool value); }
        property bool IsAppBarOpen { bool get(); void set(bool value); }
        property bool IsAppBarSticky { bool get(); void set(bool value); }

    protected private:
        virtual void GoBack();
        virtual void GoHome();
        virtual void GoToPage(PageType page, Platform::Object^ parameter);

        bool m_isAppBarEnabled;
        bool m_isAppBarOpen;
        bool m_isAppBarSticky;

        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
    };
}

