// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "BooleanToBrushConverter.h" // Required by generated header
#include "Common\BooleanNegationConverter.h" // Required by generated header
#include "ImageBrowserView.g.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app’s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.

    // The ImageBrowserView class implements the browser page.
    public ref class ImageBrowserView sealed
    {
    public:
        ImageBrowserView();

    private:
        void OnPhotoItemClicked(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
        void OnViewChangeCompleted(Platform::Object^ sender, Windows::UI::Xaml::Controls::SemanticZoomViewChangedEventArgs^ e);
        void OnZoomedOutGridPointerEntered(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ e);
    };
}
