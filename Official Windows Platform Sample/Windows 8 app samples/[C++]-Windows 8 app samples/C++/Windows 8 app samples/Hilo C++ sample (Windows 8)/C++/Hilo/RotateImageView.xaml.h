// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "RotateImageView.g.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app’s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.

    // The RotateImageView class implements the rotate page.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class RotateImageView sealed
    {
    public:
        RotateImageView();

    protected:
        virtual void OnManipulationCompleted(Windows::UI::Xaml::Input::ManipulationCompletedRoutedEventArgs^ e) override;
        virtual void OnManipulationDelta(Windows::UI::Xaml::Input::ManipulationDeltaRoutedEventArgs^ e) override;


    private:
        RotateImageViewModel^ m_viewModel;
        void PhotoSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
    };
}
