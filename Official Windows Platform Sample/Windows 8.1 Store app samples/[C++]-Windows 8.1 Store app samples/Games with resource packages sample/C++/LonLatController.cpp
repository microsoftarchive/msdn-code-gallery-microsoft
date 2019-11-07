// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

#include "pch.h"
#include "LonLatController.h"

using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Devices::Input;
using namespace Windows::ApplicationModel;

// A basic Longitude/Latitude Controller class for navigation on a sphere
// Uses touch, WASD keyboard, or mouse pointer movement

// method for handling touch press events or mouse clicks
void LonLatController::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_numPointersDown++;
    float2 position = float2(                           // position of contact
        args->CurrentPoint->Position.X,
        args->CurrentPoint->Position.Y
        );

    if (m_numPointersDown == 1)                         // if this is first pointer down
    {
        m_lonLatLastPoint1 = position;                  // save for computing relative delta
        m_lonLatPointerID1 = args->CurrentPoint->PointerId;
    }
    else if (m_numPointersDown == 2)                    // if this is the second
    {
        m_lonLatLastPoint2 = position;                  // save for computing relative delta
        m_lonLatPointerID2 = args->CurrentPoint->PointerId;
        float2 del = m_lonLatLastPoint2 - m_lonLatLastPoint1;
        m_lastDistance = hypotf(del.x, del.y);
    }
    // else //  ignore any 3rd or more pointers
}

void LonLatController::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    float2 position = float2(
        args->CurrentPoint->Position.X,
        args->CurrentPoint->Position.Y
        );

    uint32 pointerID = args->CurrentPoint->PointerId;       // ID of pointer that moved

    if (m_numPointersDown == 1)   // then drag/rotate
    {
        float2 pointerDelta;
        pointerDelta = position - m_lonLatLastPoint1;       // how far did pointer move

        float2 rotationDelta;
        rotationDelta = pointerDelta * LONLAT_GAIN;         // scale for control sensitivity
        m_lonLatLastPoint1 = position;                      // save for next time through

        // update our orientation based on the command
        float altitude = m_radius - 1;

        m_latitude  += rotationDelta.y *altitude*0.5f;
        m_longitude -= rotationDelta.x *altitude*0.5f;

        // Limit lattitude to poles
        float limit = PI_F/2 - 0.01f;
        m_latitude = __max(-limit, m_latitude);
        m_latitude = __min(+limit, m_latitude);

        // keep longitude in sane range by wrapping
        if (m_longitude > PI_F)
        {
            m_longitude -= PI_F*2;
        }
        else if (m_longitude < -PI_F)
        {
            m_longitude += PI_F*2;
        }

        // limit us to positive radius
        m_radius = __max(1.011f, m_radius);
    }
    else if (m_numPointersDown >= 2)       // pinch/zoom mode
    {
        if (pointerID == m_lonLatPointerID1)
        {
            m_lonLatLastPoint1 = position;                  // save for computing relative delta
        }
        else if (pointerID == m_lonLatPointerID2)
        {
            m_lonLatLastPoint2 = position;                  // save for computing relative delta
        }
        float2 del = m_lonLatLastPoint2 - m_lonLatLastPoint1;
        float distance = hypotf(del.x, del.y);

        float delta = distance - m_lastDistance;
        m_lonLatCommand.y += delta*0.1f;                    // zoom in/out
        m_lastDistance = distance;                          // save for next frame
    }
}

void LonLatController::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_numPointersDown--;
    if (args->CurrentPoint->PointerId == m_lonLatPointerID1)    // this was the first down pointer
    {
        m_lonLatPointerID1 = m_lonLatPointerID2;                // use remaining contact as rotate, not zoom
        m_lonLatLastPoint1 = m_lonLatLastPoint2;
    }
}
void LonLatController::OnPointerWheelChanged(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    int wheelDelta = args->CurrentPoint->Properties->MouseWheelDelta;
    m_lonLatCommand.y += wheelDelta*0.1f;      // zoom in/out
}

// set up the Controls supported by this controller
void LonLatController::Initialize(_In_ CoreWindow^ window)
{
    // opt in to recieve touch/mouse/key events

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &LonLatController::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &LonLatController::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &LonLatController::OnPointerReleased);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &LonLatController::OnPointerWheelChanged);

    // initialize controller state
    m_lonLatInUse = FALSE;          // no pointer is in the Look control
    m_lonLatPointerID1 = 0;
    m_lonLatPointerID2 = 0;

    SetOrientation(0, 0);           // look straight ahead

    m_numPointersDown = 0;
    m_radius = 1.0f;
}

// accessor to set position of controller
void LonLatController::SetPosition(_In_ float3 pos)
{
    m_longitude = atan2f(pos.x, pos.z);
    float r = sqrtf(pos.x*pos.x + pos.z*pos.z);
    m_latitude = atan2f(pos.y, r);
    m_radius = sqrtf(pos.x*pos.x + pos.y*pos.y + pos.z*pos.z);
}

// accessor to set position of controller
void LonLatController::SetOrientation(
    _In_ float latitude,
    _In_ float longitude
    )
{
    m_latitude = latitude;
    m_longitude = longitude;
}

// returns the position of the controller object
float3 LonLatController::get_Position()
{
    float y = m_radius*sinf(m_latitude);    // up-down
    float r = m_radius*cosf(m_latitude);    // in the plane
    float z = r*cosf(m_longitude);          // fwd-back
    float x = r*sinf(m_longitude);          // left-right

    return float3(x, y, z);
}

// returns the point at which the camera controller is facing
float3 LonLatController::get_LookPoint()
{
    return float3(0, 0, 0);
}

void LonLatController::Update(_In_ CoreWindow^ window)
{
    // poll keyboard state
    Windows::UI::Core::CoreVirtualKeyStates KeyState;

    // Arrow keys
    KeyState = window->GetAsyncKeyState(VirtualKey::Up);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.z += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Down);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.z -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Left);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.x -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Right);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.x += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::PageUp);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.y += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::PageDown);
    if (bool(KeyState == CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.y -= 1.0f;
    }

    // WASD keys
    KeyState = window->GetAsyncKeyState(VirtualKey::W);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.y += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::A);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.x -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::S);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.y -= 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::D);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.x += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::Space);
    if (bool(KeyState & CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.z += 1.0f;
    }

    KeyState = window->GetAsyncKeyState(VirtualKey::X);
    if (bool(KeyState == CoreVirtualKeyStates::Down))
    {
        m_lonLatCommand.z -= 1.0f;
    }

    // scale for sensitivity adjustment
    float3 command = m_lonLatCommand*LONLATKEY_GAIN;
    float altitude = m_radius - 1;
    command = command*altitude*0.5f;    // logarithmic gain towards unit sphere

    // update state with command input
    m_latitude  += command.z;
    m_longitude += command.x;
    m_radius    -= command.y;           // fwd stick is down towards object

    // Limit lattitude to poles
    float limit = PI_F/2 - 0.01f;
    m_latitude = __max(-limit, m_latitude);
    m_latitude = __min(+limit, m_latitude);

    // keep longitude in sane range by wrapping
    if (m_longitude > PI_F)
    {
        m_longitude -= PI_F*2;
    }
    else if (m_longitude < -PI_F)
    {
        m_longitude += PI_F*2;
    }

    // limit us to positive radius
    m_radius = __max(1.011f, m_radius);

    // clear movement input accumulator for use during next frame
    m_lonLatCommand = float3(0.0f, 0.0f, 0.0f);
}

void LonLatController::SaveInternalState(_In_ IPropertySet^ state)
{
    if (state->HasKey("m_longitude"))
    {
        state->Remove("m_longitude");
    }
    if (state->HasKey("m_latitude"))
    {
        state->Remove("m_latitude");
    }
    if (state->HasKey("m_radius"))
    {
        state->Remove("m_radius");
    }
    state->Insert("m_longitude", PropertyValue::CreateSingle(m_longitude));
    state->Insert("m_latitude", PropertyValue::CreateSingle(m_latitude));
    state->Insert("m_radius", PropertyValue::CreateSingle(m_radius));
}

void LonLatController::LoadInternalState(_In_ IPropertySet^ state)
{
    if (state->HasKey("m_longitude") && state->HasKey("m_latitude") && state->HasKey("m_radius"))
    {
        m_longitude = safe_cast<IPropertyValue^>(state->Lookup("m_longitude"))->GetSingle();
        m_latitude = safe_cast<IPropertyValue^>(state->Lookup("m_latitude"))->GetSingle();
        m_radius = safe_cast<IPropertyValue^>(state->Lookup("m_radius"))->GetSingle();
    }
}
