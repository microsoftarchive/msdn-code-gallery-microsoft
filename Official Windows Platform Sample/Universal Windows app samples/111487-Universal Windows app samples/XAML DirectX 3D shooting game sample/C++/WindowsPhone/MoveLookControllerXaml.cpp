//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Phone Version ***

#include "pch.h"
#include "MoveLookControllerXaml.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Windows::Phone::UI::Input;

//----------------------------------------------------------------------

MoveLookController^ MoveLookControllerXaml::Create(_In_ CoreWindow^ window,
    _In_ CoreDispatcher^ /* dispatcher */
    )
{
    auto p = ref new MoveLookControllerXaml(window);
    return static_cast<MoveLookController^>(p);
}

//----------------------------------------------------------------------

MoveLookControllerXaml::MoveLookControllerXaml(_In_ CoreWindow^ window)
{
    // Even though all current realizations of MoveLookController install the
    // PointerPressed, PointerMoved, PointerReleased and PointerExited event
    // handlers, it was decided to put all event handler registrations together
    // in the constructor.

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerReleased);

    window->PointerExited +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerExited);

    HardwareButtons::BackPressed +=
        ref new EventHandler<BackPressedEventArgs^>(this, &MoveLookControllerXaml::OnHardwareBackButtonPressed);
}

//----------------------------------------------------------------------

void MoveLookControllerXaml::OnHardwareBackButtonPressed(
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
