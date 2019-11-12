// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

#include "pch.h"
#include <DirectXMath.h>

using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Devices::Input;

#include "MoveLookController.h"

#define ROTATION_GAIN 0.004f    // sensitivity adjustment for look controller
#define MOVEMENT_GAIN 0.07f     // sensitivity adjustment for move controller
#define DEADSPOT (16.0f)        // dead space in move control (virtual joystick) center

// A basic Move/Look Controller class such as in an FPS
// horizontal (x-z-plane) movement on left virtual joystick
//          also supports WASD keyboard input
// steering and orientation via left mouse down or touch drag

// Methods to get input from the UI pointers
void MoveLookController::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    // get the current pointer position
    uint32 pointerID = args->CurrentPoint->PointerId;
    float2 position = float2(args->CurrentPoint->Position.X, args->CurrentPoint->Position.Y);

    auto device = args->CurrentPoint->PointerDevice;
    auto deviceType = device->PointerDeviceType;
    if (deviceType == PointerDeviceType::Mouse)
    {
        if (args->CurrentPoint->Properties->IsLeftButtonPressed && !m_mouseLookInUse)
        {
            // Left mouse button has been pressed, so enable mouse look mode.
            m_mouseLookInUse = true;
            m_mouseLookPointerID = pointerID;

            // Turn mouse cursor off (hidden).  This locks the cursor position.
            // In this mode the cursor will never leave the Window allowing the look controller
            // to rotate continuously around.
            CoreWindow::GetForCurrentThread()->PointerCursor = nullptr;
        }
        return;
    }

    if (position.x < MoveLookControllerConstants::MoveMaxXCoordinate &&
        position.y > MoveLookControllerConstants::MoveMinYCoordinate)
    {
        // Pointer is in the move control area.
        if (!m_moveInUse)   // if no pointer is in this control yet
        {
            // process a DPad touch down event
            m_moveFirstDown = position;                 // save location of initial contact
            m_movePointerPosition = position;
            m_movePointerID = pointerID;                // store the id of pointer using this control
            m_moveInUse = TRUE;
        }
    }
    else
    {
        // this pointer must be in the look control
        if (!m_lookInUse)   // if no pointer is in this control yet
        {
            m_lookLastPoint = position;                         // save point for later move
            m_lookPointerID = pointerID;                        // store the id of pointer using this control
            m_lookLastDelta.x = m_lookLastDelta.y = 0;          // these are for smoothing
            m_lookInUse = TRUE;
        }
    }
}

void MoveLookController::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    PointerPoint^ point = args->CurrentPoint;
    PointerDevice^ pointerDevice = point->PointerDevice;
    PointerDeviceType pointerDeviceType = pointerDevice->PointerDeviceType;
    if (pointerDeviceType == PointerDeviceType::Mouse)
    {
        return;         // mouse moves are handled by OnMouseMoved, not by generic pointer events
    }

    float2 position = float2(point->Position.X, point->Position.Y);

    // decide which control this pointer is operating
    uint32 pointerID = point->PointerId;
    if (pointerID == m_movePointerID)           // this is the move pointer
    {
        // Move control
        m_movePointerPosition = position;       // save current position

    }
    else if (pointerID == m_lookPointerID)      // this is the look pointer
    {
        // Look control

        float2 pointerDelta;
        pointerDelta = position - m_lookLastPoint;      // how far did pointer move

        float2 rotationDelta;
        rotationDelta = pointerDelta * ROTATION_GAIN;   // scale for control sensitivity
        m_lookLastPoint = position;                     // save for next time through

        // update our orientation based on the command
        m_pitch -= rotationDelta.y;                     // mouse y increases down, but pitch increases up
        m_yaw   -= rotationDelta.x;                     // yaw defined as CCW around y-axis

        // limit pitch to straight up or straight down
        float limit = PI_F / 2.0f - 0.01f;
        m_pitch = __max(-limit, m_pitch);
        m_pitch = __min(+limit, m_pitch);

        // keep longitude in sane range by wrapping
        if (m_yaw >  PI_F)
        {
            m_yaw -= PI_F * 2.0f;
        }
        else if (m_yaw < -PI_F)
        {
            m_yaw += PI_F * 2.0f;
        }
    }
}

void MoveLookController::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    uint32 pointerID = args->CurrentPoint->PointerId;
    float2 position = float2(args->CurrentPoint->Position.X, args->CurrentPoint->Position.Y);

    if (pointerID == m_movePointerID)       // this was the move pointer
    {
        m_moveInUse = FALSE;
        m_movePointerID = 0;
    }
    else if (pointerID == m_lookPointerID)  // this was the look pointer
    {
        m_lookInUse = FALSE;
        m_lookPointerID = 0;
    }
    else if (pointerID == m_mouseLookPointerID)
    {
        if (!args->CurrentPoint->Properties->IsLeftButtonPressed)
        {
            // Left mouse button has been released, so leave mouse look mode.
            m_mouseLookInUse = false;
            m_mouseLookPointerID = 0;

            // Turn mouse cursor on.  This unlocks the cursor position.
            CoreWindow::GetForCurrentThread()->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);
        }
    }
}

// Handle Mouse Input via dedicated relative movement handler
void MoveLookController::OnMouseMoved(
    _In_ MouseDevice^ mouseDevice,
    _In_ MouseEventArgs^ args
    )
{
    if (!m_mouseLookInUse)
    {
        // Ignore mouse movement events when not in mouse look mode.
        return;
    }

    float2 pointerDelta;
    pointerDelta.x = static_cast<float>(args->MouseDelta.X);
    pointerDelta.y = static_cast<float>(args->MouseDelta.Y);

    float2 rotationDelta;
    rotationDelta = pointerDelta * ROTATION_GAIN;       // scale for control sensitivity

    // update our orientation based on the command
    m_pitch -= rotationDelta.y;                         // mouse y increases down, but pitch increases up
    m_yaw   -= rotationDelta.x;                         // yaw defined as CCW around y-axis

    // limit pitch to straight up or straight down
    float limit = PI_F / 2.0f - 0.01f;
    m_pitch = __max(-limit, m_pitch);
    m_pitch = __min(+limit, m_pitch);

    // keep longitude in sane range by wrapping
    if (m_yaw >  PI_F)
    {
        m_yaw -= PI_F * 2.0f;
    }
    else if (m_yaw < -PI_F)
    {
        m_yaw += PI_F * 2.0f;
    }
}

// set up the Controls supported by this controller
void MoveLookController::Initialize(_In_ CoreWindow^ window)
{
    // opt in to recieve touch/mouse events
    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookController::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookController::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MoveLookController::OnPointerReleased);

    // for mouse-only use
    // register handler for relative mouse movement events
    Windows::Devices::Input::MouseDevice::GetForCurrentView()->MouseMoved +=
        ref new TypedEventHandler<MouseDevice^, MouseEventArgs^>(this, &MoveLookController::OnMouseMoved);

    // Initialize state of the controller
    m_moveInUse = FALSE;                // no pointer is in the Move control
    m_movePointerID = 0;

    m_lookInUse = FALSE;                // no pointer is in the Look control
    m_lookPointerID = 0;

    // Need to init this as it is reset every frame
    m_moveCommand = float3(0.0f, 0.0f, 0.0f);

    SetOrientation(0, 0);               // look straight ahead when starting
}

// accessor to set position of controller
void MoveLookController::SetPosition(_In_ float3 pos)
{
    m_position = pos;
}

// accessor to set position of controller
void MoveLookController::SetOrientation(_In_ float pitch, _In_ float yaw)
{
    m_pitch = pitch;
    m_yaw = yaw;
}

// returns the position of the controller object
float3 MoveLookController::GetPosition()
{
    return m_position;
}

// returns the point at which the camera controller is facing
float3 MoveLookController::GetLookPoint()
{
    float y = sinf(m_pitch);        // vertical
    float r = cosf(m_pitch);        // in the plane
    float z = r*cosf(m_yaw);        // fwd-back
    float x = r*sinf(m_yaw);        // left-right

    return m_position + float3(x, y, z);
}


void MoveLookController::Update(CoreWindow^ window)
{
    // check for input from the Move control
    if (m_moveInUse)
    {
        float2 pointerDelta = m_movePointerPosition - m_moveFirstDown;

        // figure out the command from the touch-based virtual joystick
        if (pointerDelta.x > DEADSPOT)          // leave 32 pixel-wide dead spot for being still
        {
            m_moveCommand.x =  1.0f;
        }
        else if (pointerDelta.x < -DEADSPOT)
        {
            m_moveCommand.x = -1.0f;
        }

        if (pointerDelta.y > DEADSPOT)          // joystick y is up so change sign
        {
            m_moveCommand.y = -1.0f;
        }
        else if (pointerDelta.y < -DEADSPOT)
        {
            m_moveCommand.y =  1.0f;
        }
    }

    // poll our state set by the keyboard input events
    Windows::UI::Core::CoreVirtualKeyStates KeyState;

    // Arrow keys
    KeyState = window->GetAsyncKeyState(VirtualKey::Up);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.y += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Down);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.y -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Left);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.x -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Right);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.x += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::PageUp);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.z += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::PageDown);
    if (bool(KeyState == CoreVirtualKeyStates::Down))
    {
        m_moveCommand.z -= 1.0f;
    }

// WASD controls
    KeyState = window->GetAsyncKeyState(VirtualKey::W);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.y += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::A);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.x -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::S);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.y -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::D);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.x += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Space);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_moveCommand.z += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::X);
    if (bool(KeyState == CoreVirtualKeyStates::Down))
    {
        m_moveCommand.z -= 1.0f;
    }


    // make sure that 45degree cases are not faster
    float3 command = m_moveCommand;
    if (fabsf(command.x) > DEADSPOT || fabsf(command.y) > DEADSPOT || fabsf(command.z) > DEADSPOT)
    {
        command = normalize(command);
    }

    // rotate command to align with our direction (world coordinates)
    float3 wCommand;
    wCommand.x = command.x*cosf(m_yaw) - command.y*sinf(m_yaw);
    wCommand.y = command.x*sinf(m_yaw) + command.y*cosf(m_yaw);
    wCommand.z = command.z;

    // scale for sensitivity adjustment
    wCommand = wCommand*MOVEMENT_GAIN;

    // our velocity is based on the command, y is up
    float3 velocity;
    velocity.x = -wCommand.x;
    velocity.z =  wCommand.y;
    velocity.y =  wCommand.z;

    // integrate
    m_position = m_position + velocity;

    // clear movement input accumulator for use during next frame
    m_moveCommand = float3(0.0f, 0.0f, 0.0f);
}

void MoveLookController::SaveInternalValue(
    _In_ IPropertySet^ state,
    _In_ Platform::String^ key,
    _In_ Platform::Object^ value
    )
{
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, value);
}

void MoveLookController::SaveInternalState(_In_ IPropertySet^ state)
{
    SaveInternalValue(state, "m_position.x", PropertyValue::CreateSingle(m_position.x));
    SaveInternalValue(state, "m_position.y", PropertyValue::CreateSingle(m_position.y));
    SaveInternalValue(state, "m_position.z", PropertyValue::CreateSingle(m_position.z));
    SaveInternalValue(state, "m_pitch", PropertyValue::CreateSingle(m_pitch));
    SaveInternalValue(state, "m_yaw", PropertyValue::CreateSingle(m_yaw));
}

void MoveLookController::LoadInternalState(_In_ IPropertySet^ state)
{
    if (state->HasKey("m_position.x") &&
        state->HasKey("m_position.y") &&
        state->HasKey("m_position.z") &&
        state->HasKey("m_pitch") &&
        state->HasKey("m_yaw"))
    {
        m_position.x = safe_cast<IPropertyValue^>(state->Lookup("m_position.x"))->GetSingle();
        m_position.y = safe_cast<IPropertyValue^>(state->Lookup("m_position.y"))->GetSingle();
        m_position.z = safe_cast<IPropertyValue^>(state->Lookup("m_position.z"))->GetSingle();
        m_pitch = safe_cast<IPropertyValue^>(state->Lookup("m_pitch"))->GetSingle();
        m_yaw = safe_cast<IPropertyValue^>(state->Lookup("m_yaw"))->GetSingle();
    }
}
