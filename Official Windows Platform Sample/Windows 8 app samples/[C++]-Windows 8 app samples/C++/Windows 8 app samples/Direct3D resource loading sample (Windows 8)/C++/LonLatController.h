//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#pragma once

#define LONLAT_GAIN    0.004f // sensitivity adjustment for mouse/touch
#define LONLATKEY_GAIN 0.04f // sensitivity adjustment for keys

// A basic longitude/latitude controller class for navigation on a sphere.
// It integrates touch, WASD keyboard, and mouse pointer movement into a
// single unified controller.
ref class LonLatController
{
private:
    // properties of the controller object
    float m_radius; // Distance from center of sphere
    float m_longitude; // Left-right coordinate on the sphere
    float m_latitude; // pole-to-pole coordinate on the sphere

    // properties of the LonLat control
    BOOL m_lonLatInUse; // the look control is in use
    float3 m_lonLatCommand; // the net command from the move control
    uint32 m_lonLatPointerID; // id of the pointer in this control
    float2 m_lonLatLastPoint; // last point (from last frame)
    float2 m_lonLatLastDelta; // for smoothing

    bool m_forward, m_back; // states for movement
    bool m_left, m_right;
    bool m_up, m_down;

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

    // set up the Controls supported by this controller
    void Initialize(_In_ Windows::UI::Core::CoreWindow^ window);

    // accessor to set position of controller
    void SetPosition(_In_ float3 pos);

    // accessor to set position of controller
    void SetOrientation(_In_ float latitude, _In_ float longitude);

    // returns the position of the controller object
    float3 get_Position();

    // returns the point at which the camera controller is facing
    float3 get_LookPoint();

    void Update();

    // methods to save and restore state
    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
};




