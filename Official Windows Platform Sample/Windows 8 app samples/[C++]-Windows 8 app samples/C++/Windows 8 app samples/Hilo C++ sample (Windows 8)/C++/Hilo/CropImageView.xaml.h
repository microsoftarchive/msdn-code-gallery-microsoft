// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "Common\BooleanToVisibilityConverter.h" // Required by generated header
#include "CropImageView.g.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app’s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.
    
    ref class CropImageViewModel;

    // The CropImageView class implements the app's crop page.
    public ref class CropImageView sealed
    {
    public:
        CropImageView();

    private:
        CropImageViewModel^ m_cropImageViewModel;
        bool m_sizeChangedAttached;

        void OnCropRectangleTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
        void OnThumbDragDelta(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::DragDeltaEventArgs^ e);
        void OnSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
    };
}





