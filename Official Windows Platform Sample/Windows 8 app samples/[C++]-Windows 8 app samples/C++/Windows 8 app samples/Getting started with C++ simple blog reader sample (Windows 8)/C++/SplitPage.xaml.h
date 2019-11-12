// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
//
// SplitPage.xaml.h
// Declaration of the SplitPage class
//

#pragma once

#include "Common\LayoutAwarePage.h" // Required by generated header
#include "SplitPage.g.h"

namespace SimpleBlogReader
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for the
    /// currently selected item.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]			
    public ref class SplitPage sealed
    {
    public:
        SplitPage();

    protected:
        virtual void LoadState(Platform::Object^ navigationParameter,
        Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
        virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
        virtual void GoBack(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) override;
        virtual Platform::String^ DetermineVisualState(Windows::UI::ViewManagement::ApplicationViewState viewState) override;
        void ViewDetail_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

    private:
        void ItemListView_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
        bool UsingLogicalPageNavigation();
        bool UsingLogicalPageNavigation(Windows::UI::ViewManagement::ApplicationViewState viewState);
        
    };

    
}
