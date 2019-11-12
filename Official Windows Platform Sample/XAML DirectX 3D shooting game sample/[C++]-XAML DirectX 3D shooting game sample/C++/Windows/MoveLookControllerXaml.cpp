//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Version ***

#include "pch.h"
#include "MoveLookControllerXaml.h"

using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace DirectX;
using namespace Windows::Devices::Input;
using namespace Windows::System;

//----------------------------------------------------------------------

MoveLookController^ MoveLookControllerXaml::Create(_In_ CoreWindow^ window,
    _In_ CoreDispatcher^ dispatcher
    )
{
    auto p = ref new MoveLookControllerXaml(window, dispatcher);
    return static_cast<MoveLookController^>(p);
}

//----------------------------------------------------------------------

MoveLookControllerXaml::MoveLookControllerXaml(
    _In_ CoreWindow^ window,
    _In_ CoreDispatcher ^dispatcher
    ) :
    m_isControllerConnected(false)
{
    // Even though all current realizations of MoveLookController install the
    // PointerPressed, PointerMoved, PointerReleased and PointerExited event
    // handlers, it was decided to put all event handler registrations together
    // in the constructor.

    // The windows version of the MoveLookController installs event handlers
    // for keyboard input and for relative mouse movement.  There are two
    // parts required to enable relative mouse movement, the event handler
    // and disabling the cursor pointer.  The game is running on a separate thread
    // from the Xaml UI thread, and this separate thread does NOT have access to
    // the CoreWindow.  The dispatcher can be used to marshal execution back to
    // the Xaml UI thread which does have a CoreWindow.  The Dispatcher is cached
    // to enable execution of code on the UI thread to turn on and off the cursor glyph.

    m_dispatcher = dispatcher;

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerReleased);

    window->PointerExited +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookControllerXaml::OnPointerExited);

    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &MoveLookControllerXaml::OnKeyDown);

    window->KeyUp +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &MoveLookControllerXaml::OnKeyUp);

    // There is a separate handler for mouse only relative mouse movement events.
    MouseDevice::GetForCurrentView()->MouseMoved +=
        ref new TypedEventHandler<MouseDevice^, MouseEventArgs^>(this, &MoveLookControllerXaml::OnMouseMoved);
}

//----------------------------------------------------------------------

void MoveLookControllerXaml::ResetState()
{
    MoveLookController::ResetState();
    m_xinputStartButtonInUse = false;
    m_xinputTriggerInUse = false;
}

//----------------------------------------------------------------------

void MoveLookControllerXaml::ShowCursor()
{
    // This function may be called from a different thread.
    // All XAML updates need to occur on the UI thread so dispatch to ensure this is true.
    // NOTE: PointerCursor is not implemented on WindowsPhone.
    m_dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {
            // Turn on mouse cursor.
            // This disables relative mouse movement events.
            CoreWindow::GetForCurrentThread()->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);
        })
        );
}

//----------------------------------------------------------------------

void MoveLookControllerXaml::HideCursor()
{
    // This function may be called from a different thread.
    // All XAML updates need to occur on the UI thread so dispatch to ensure this is true.
    // NOTE: PointerCursor is not implemented on WindowsPhone.
    m_dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {
            // Turn off the cursor to enable relative mouse.
            CoreWindow::GetForCurrentThread()->PointerCursor = nullptr;
        })
        );
}

//----------------------------------------------------------------------

void MoveLookControllerXaml::UpdatePollingDevices()
{
    if (!m_isControllerConnected)
    {
        // Check for controller connection by trying to get the capabilties.
        DWORD capsResult = XInputGetCapabilities(0, XINPUT_FLAG_GAMEPAD, &m_xinputCaps);
        if (capsResult != ERROR_SUCCESS)
        {
            return;
        }
        // Device is connected.
        m_isControllerConnected = true;
        m_xinputStartButtonInUse = false;
        m_xinputTriggerInUse = false;
    }

    DWORD stateResult = XInputGetState(0, &m_xinputState);
    if (stateResult != ERROR_SUCCESS)
    {
        // Device is no longer connected.
        m_isControllerConnected = false;
    }

    switch (m_state)
    {
    case MoveLookControllerState::WaitForInput:
        if (m_xinputState.Gamepad.wButtons & XINPUT_GAMEPAD_START)
        {
            m_xinputStartButtonInUse = true;
        }
        else if (m_xinputStartButtonInUse)
        {
            // Trigger once only on button release.
            m_xinputStartButtonInUse = false;
            m_buttonPressed = true;
        }
        break;

    case MoveLookControllerState::Active:
        if (m_xinputState.Gamepad.wButtons & XINPUT_GAMEPAD_START)
        {
            m_xinputStartButtonInUse = true;
        }
        else if (m_xinputStartButtonInUse)
        {
            // Trigger once only on button release.
            m_xinputStartButtonInUse = false;
            m_pausePressed = true;
        }
        // Use the Left Thumb joystick on the Xbox controller to control
        // the eye point position control.
        // The controller input goes from -32767 to 32767.  We will normalize
        // this from -1 to 1 and keep a dead spot in the middle to avoid drift.

        if (m_xinputState.Gamepad.sThumbLX > XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE ||
            m_xinputState.Gamepad.sThumbLX < -XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
        {
            float x = (float) m_xinputState.Gamepad.sThumbLX / 32767.0f;
            m_moveCommand.x -= x / fabsf(x);
        }

        if (m_xinputState.Gamepad.sThumbLY > XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE ||
            m_xinputState.Gamepad.sThumbLY < -XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
        {
            float y = (float) m_xinputState.Gamepad.sThumbLY / 32767.0f;
            m_moveCommand.y += y / fabsf(y);
        }

        // Use the Right Thumb Joystick on the Xbox controller to control
        // the look at control.
        // The controller input goes from -32767 to 32767.  We will normalize
        // this from -1 to 1 and keep a dead spot in the middle to avoid drift.
        XMFLOAT2 pointerDelta;
        if (m_xinputState.Gamepad.sThumbRX > XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE ||
            m_xinputState.Gamepad.sThumbRX < -XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
        {
            pointerDelta.x = (float) m_xinputState.Gamepad.sThumbRX / 32767.0f;
            pointerDelta.x = pointerDelta.x * pointerDelta.x * pointerDelta.x;
        }
        else
        {
            pointerDelta.x = 0.0f;
        }
        if (m_xinputState.Gamepad.sThumbRY > XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE ||
            m_xinputState.Gamepad.sThumbRY < -XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
        {
            pointerDelta.y = (float) m_xinputState.Gamepad.sThumbRY / 32767.0f;
            pointerDelta.y = pointerDelta.y * pointerDelta.y * pointerDelta.y;
        }
        else
        {
            pointerDelta.y = 0.0f;
        }

        XMFLOAT2 rotationDelta;
        rotationDelta.x = pointerDelta.x *  0.08f;       // Scale for control sensitivity.
        rotationDelta.y = pointerDelta.y *  0.08f;

        // Update our orientation based on the command.
        m_pitch += rotationDelta.y;
        m_yaw += rotationDelta.x;

        // Limit pitch to straight up or straight down.
        m_pitch = __max(-XM_PI / 2.0f, m_pitch);
        m_pitch = __min(+XM_PI / 2.0f, m_pitch);

        // Check the state of the Right Trigger button.  This is used to indicate fire control.

        if (m_xinputState.Gamepad.bRightTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD)
        {
            if (!m_autoFire && !m_xinputTriggerInUse)
            {
                m_firePressed = true;
            }
            m_xinputTriggerInUse = true;
        }
        else
        {
            m_xinputTriggerInUse = false;
        }
        break;
    }
}

//----------------------------------------------------------------------
