//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#pragma once

#define LONLAT_GAIN    0.003f // sensitivity adjustment for mouse/touch
#define LONLATKEY_GAIN 0.04f // sensitivity adjustment for keys

// A basic Longitude/Latitude Controller class for navigation on a sphere
// Uses touch, WASD keyboard, or mouse pointer movement

ref class LonLatController
{
private:
    // properties of the controller object
    float3 m_position;   // Position of the camera
    float m_radius;      // Distance from center of sphere
    float m_longitude;   // Left-right coordinate on the sphere
    float m_latitude;    // Pole-to-pole coordinate on the sphere

    // properties of the LonLat control
    BOOL m_lonLatInUse; // the look control is in use
    uint32 m_lonLatPointerID1; // id of the first pointer down
    uint32 m_lonLatPointerID2; // id of the second pointer down
    float2 m_lonLatLastPoint1; // last point (from last frame)
    float2 m_lonLatLastPoint2; // last point (from last frame)

    float3 m_lonLatCommand; // the net command from the move control
    uint32 m_numPointersDown;
    float  m_lastDistance;

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

    void OnPointerWheelChanged(
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

internal:
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

    void Update(_In_ Windows::UI::Core::CoreWindow^ window);

    // methods to save and restore state
    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
};
