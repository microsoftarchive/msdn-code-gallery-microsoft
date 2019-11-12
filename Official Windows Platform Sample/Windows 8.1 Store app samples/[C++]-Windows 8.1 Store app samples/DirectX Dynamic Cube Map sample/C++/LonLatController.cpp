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

// method for handling touch press events or mouse clicks
void LonLatController::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    if (!m_lonLatInUse) // if no pointer is in this control yet
    {
        m_lonLatLastPoint = float2( // save point for later move
            args->CurrentPoint->Position.X,
            args->CurrentPoint->Position.Y
            );
        m_lonLatPointerID = args->CurrentPoint->PointerId; // store the pointer using this control
        m_lonLatLastDelta.x = m_lonLatLastDelta.y = 0; // these are for smoothing
        m_lonLatInUse = TRUE;
    }
}

void LonLatController::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    if (args->CurrentPoint->PointerId == m_lonLatPointerID) // this is the look pointer
    {
        float2 position = float2(
            args->CurrentPoint->Position.X,
            args->CurrentPoint->Position.Y
            );

        float2 pointerDelta;
        pointerDelta = position - m_lonLatLastPoint; // how far did pointer move

        float2 rotationDelta;
        rotationDelta = pointerDelta * LONLAT_GAIN; // scale for control sensitivity
        m_lonLatLastPoint = position; // save for next time through

        // update our orientation based on the command
        m_latitude  += rotationDelta.y;
        m_longitude -= rotationDelta.x;

        // Limit lattitude to poles
        float limit = PI_F / 2.0f - 0.01f;
        m_latitude = (float) __max(-limit, m_latitude);
        m_latitude = (float) __min(+limit, m_latitude);

        // keep longitude in sane range by wrapping
        if (m_longitude >  PI_F)
        {
            m_longitude -= PI_F * 2.0f;
        }
        else if (m_longitude < -PI_F)
        {
            m_longitude += PI_F * 2.0f;
        }

        // limit us to positive radius
        m_radius = (float) __max(0.0, m_radius);
    }
}

void LonLatController::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    if (args->CurrentPoint->PointerId == m_lonLatPointerID) // this is the look pointer
    {
        m_lonLatInUse = FALSE;
        m_lonLatPointerID = 0;
    }
}

void LonLatController::OnKeyDown(
    _In_ CoreWindow^ sender,
    _In_ KeyEventArgs^ args
    )
{
    Windows::System::VirtualKey Key;
    Key = args->VirtualKey;

    // figure out the command from the keyboard
    if (Key == VirtualKey::W) // forward
    {
        m_forward = true;
    }
    if (Key == VirtualKey::S) // back
    {
        m_back = true;
    }
    if (Key == VirtualKey::A) // left
    {
        m_left = true;
    }
    if (Key == VirtualKey::D) // right
    {
        m_right = true;
    }
    if (Key == VirtualKey::Space) // up
    {
        m_up = true;
    }
    if (Key == VirtualKey::X) // down
    {
        m_down = true;
    }
}

void LonLatController::OnKeyUp(
    _In_ CoreWindow^ sender,
    _In_ KeyEventArgs^ args
    )
{
    Windows::System::VirtualKey Key;
    Key = args->VirtualKey;

    // figure out the command from the keyboard
    if (Key == VirtualKey::W) // forward
    {
        m_forward = false;
    }
    if (Key == VirtualKey::S) // back
    {
        m_back = false;
    }
    if (Key == VirtualKey::A) // left
    {
        m_left = false;
    }
    if (Key == VirtualKey::D) // right
    {
        m_right = false;
    }
    if (Key == VirtualKey::Space) // up
    {
        m_up = false;
    }
    if (Key == VirtualKey::X) // down
    {
        m_down = false;
    }
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

    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &LonLatController::OnKeyDown);

    window->KeyUp +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &LonLatController::OnKeyUp);

    // initialize controller state

    m_lonLatInUse = FALSE; // no pointer is in the Look control
    m_lonLatPointerID = 0;

    SetOrientation(0, 0); // look straight ahead
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
void LonLatController::SetOrientation(_In_ float latitude, _In_ float longitude)
{
    m_latitude = latitude;
    m_longitude = longitude;
}

// returns the position of the controller object
float3 LonLatController::get_Position()
{
    float y = m_radius * sinf(m_latitude);  // up-down
    float r = m_radius * cosf(m_latitude);  // in the plane
    float z = r * cosf(m_longitude);        // fwd-back
    float x = r * sinf(m_longitude);        // left-right

    return float3(x, y, z);
}

// returns the point at which the camera controller is facing
float3 LonLatController::get_LookPoint()
{
    return float3(0, 0, 0);
}

void LonLatController::Update()
{
    // poll our state bits set by the keyboard input events
    if (m_forward)
    {
        m_lonLatCommand.y += 1.0f;
    }
    if (m_back)
    {
        m_lonLatCommand.y -= 1.0f;
    }

    if (m_left)
    {
        m_lonLatCommand.x -= 1.0f;
    }
    if (m_right)
    {
        m_lonLatCommand.x += 1.0f;
    }

    if (m_up)
    {
        m_lonLatCommand.z += 1.0f;
    }
    if (m_down)
    {
        m_lonLatCommand.z -= 1.0f;
    }

    // scale for sensitivity adjustment
    float3 command = m_lonLatCommand*LONLATKEY_GAIN;
    command.y *= m_radius * 0.5f; // logarithmic gain

    // update state with command input
    m_latitude  += command.z;
    m_longitude += command.x;
    m_radius    -= command.y; // fwd stick is down towards object

    // Limit lattitude to poles
    float limit = PI_F / 2.0f - 0.01f;
    m_latitude = (float) __max(-limit, m_latitude);
    m_latitude = (float) __min(+limit, m_latitude);

    // keep longitude in sane range by wrapping
    if (m_longitude >  PI_F)
    {
        m_longitude -= PI_F * 2.0f;
    }
    else if (m_longitude < -PI_F)
    {
        m_longitude += PI_F * 2.0f;
    }

    // limit us to positive radius
    m_radius = (float) __max(0.0, m_radius);

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

