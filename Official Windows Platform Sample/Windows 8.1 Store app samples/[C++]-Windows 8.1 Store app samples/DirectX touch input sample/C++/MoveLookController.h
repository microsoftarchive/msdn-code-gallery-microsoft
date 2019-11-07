// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

#pragma once

#include "BasicMath.h"
#include "math.h"

// A basic Move/Look Controller class such as in an FPS
// horizontal (x-z-plane) movement on left virtual joystick
//          also supports WASD keyboard input
// steering and orientation via left mouse down or touch drag

namespace MoveLookControllerConstants
{
    static const float  MoveMaxXCoordinate  = 500.0f;
    static const float  MoveMinYCoordinate  = 580.0f;
};

ref class MoveLookController
{
private:
    // properties of the controller object
    float3 m_position;              // the position of the controller
    float m_pitch, m_yaw;           // orientation euler angles in radians

    // properties of the Move control
    bool m_moveInUse;               // the move control is in use
    uint32 m_movePointerID;         // id of the pointer in this control
    float2 m_moveFirstDown;         // point where initial contact occurred
    float2 m_movePointerPosition;   // point where the move pointer is currently located
    float3 m_moveCommand;           // the net command from the move control

    // properties of the Look control
    bool m_lookInUse;               // the look control is in use
    uint32 m_lookPointerID;         // id of the pointer in this control
    float2 m_lookLastPoint;         // last point (from last frame)
    float2 m_lookLastDelta;         // for smoothing

    // properties of the Mouse Look control
    bool m_mouseLookInUse;          // the mouse look control is in use
    uint32 m_mouseLookPointerID;    // id of the pointer in this control

internal:
    // Methods to get input from the UI pointers

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnKeyDown(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::KeyEventArgs^ args
        );

    void OnKeyUp(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::KeyEventArgs^ args
        );

    void OnMouseMoved(
        _In_ Windows::Devices::Input::MouseDevice^ mouseDevice,
        _In_ Windows::Devices::Input::MouseEventArgs^ args
        );

    // Set up the Controls supported by this controller.
    void Initialize(_In_ Windows::UI::Core::CoreWindow^ window);

    // Accessor to set position of controller.
    void SetPosition(_In_ float3 pos);

    // Accessor to set position of controller.
    void SetOrientation(_In_ float pitch, _In_ float yaw);

    // Returns the position of the controller object.
    float3 GetPosition();

    // Returns the point at which the controller is facing.
    float3 GetLookPoint();

    void Update(Windows::UI::Core::CoreWindow^ window);

    void SaveInternalValue(
        _In_ Windows::Foundation::Collections::IPropertySet^ state,
        _In_ Platform::String^ key,
        _In_ Platform::Object^ value
        );

    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

};  // class MoveLookController
