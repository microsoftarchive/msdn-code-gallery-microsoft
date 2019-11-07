//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Phone Version ***

#include "pch.h"
#include "MoveLookControllerDX.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Windows::Phone::UI::Input;

//----------------------------------------------------------------------

MoveLookController^ MoveLookControllerDX::Create(_In_ CoreWindow^ window)
{
    auto p = ref new MoveLookControllerDX(window);
    return static_cast<MoveLookController^>(p);
}

//----------------------------------------------------------------------

MoveLookControllerDX::MoveLookControllerDX(_In_ CoreWindow^ window)
{
    // Even though all current realizations of MoveLookController install the
    // PointerPressed, PointerMoved, PointerReleased and PointerExited event
    // handlers, it was decided to put all event handler registrations together
    // in the constructor.

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerDX::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerDX::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerDX::OnPointerReleased);

    window->PointerExited +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerDX::OnPointerExited);

    HardwareButtons::BackPressed +=
        ref new EventHandler<BackPressedEventArgs^>(this, &MoveLookControllerDX::OnHardwareBackButtonPressed);
}

//----------------------------------------------------------------------

void MoveLookControllerDX::OnHardwareBackButtonPressed(
    _In_ Platform::Object^ sender,
    BackPressedEventArgs ^args
    )
{
    if (m_state == MoveLookControllerState::Active)
    {
        // The game is currently in active play mode, so hitting the hardware back button
        // will cause the game to pause.
        m_pausePressed = true;
        args->Handled = true;
    }
    else
    {
        // The game is not currently in active play mode, so take the default behavior
        // for the hardware back button.
        args->Handled = false;
    }
}

//----------------------------------------------------------------------
