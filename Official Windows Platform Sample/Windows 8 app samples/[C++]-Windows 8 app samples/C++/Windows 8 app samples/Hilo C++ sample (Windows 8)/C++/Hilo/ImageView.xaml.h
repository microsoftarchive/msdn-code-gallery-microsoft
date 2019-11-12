// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "FileSizeConverter.h" // Required by generated header
#include "ImageView.g.h"
    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app’s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.

namespace Hilo
{
    // The ImageView class implements the view image page.
    public ref class ImageView sealed
    {
    public:
        ImageView();
        virtual ~ImageView();

    private:
        void OnFilmStripLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OnImagePointerPressed(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ e);
        void OnImagePointerReleased(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ e);
        void OnImagePointerMoved(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ e);

        Windows::Foundation::EventRegistrationToken m_filmStripLoadedToken;
        bool m_pointerPressed;
        Windows::Foundation::Point m_pointer;
    };
}
