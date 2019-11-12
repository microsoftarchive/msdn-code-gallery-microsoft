// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#include "ManipulatableObject.h"

ref class DrawingObject sealed : public ManipulatableObject
{
public:
    DrawingObject(_In_opt_ ManipulatableObject^ parent, _In_ IRenderer^ renderer);

    // Returns true if point (x,y) belongs to the object. The point should be in parent coordinate system.
    bool HitTest(_In_ Windows::Foundation::Point position);

    // Attaches/detaches GestureRecognizer to/from the object.
    void Attach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer);
    void Detach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer);

    // Serialization/deserialization methods
    virtual void Serialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties) override;
    virtual void Deserialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties) override;

internal:
    enum class Color
    {
        First = 0,
        Green = First,
        Blue,
        Orange,
        Purple,
        Yellow,
        Red,
        MaxCount
    };

    // Initialize object state: color, initial position and size. Initial position is relative to view centerpoint.
    void Initialize(_In_ Color color, _In_ float x, _In_ float y, _In_ float dx, _In_ float dy);

    Color GetColor()  { return _color; }

    // Draw the object, given D2D objects.
    void Draw(
        _In_ ID2D1DeviceContext* deviceContext,
        _In_ ID2D1SolidColorBrush* brush);

    // Draw overlay text, given D2D objects.
    void DrawOverlay(
        _In_ ID2D1DeviceContext* deviceContext,
        _In_ IDWriteTextFormat* textFormat,
        _In_ ID2D1SolidColorBrush* brush);

private:
    // GestureRecognizer event handlers.
    void OnTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::TappedEventArgs^ args);
    void OnRightTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::RightTappedEventArgs^ args);
    void OnHolding(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::HoldingEventArgs^ args);
    void OnManipulationStarted(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::ManipulationStartedEventArgs^ args);
    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args);
    void OnManipulationInertiaStarting(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::ManipulationInertiaStartingEventArgs^ args);
    void OnManipulationCompleted(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::ManipulationCompletedEventArgs^ args);
	~DrawingObject();

    // Get object's description.
    void GetObjectDescription();

private:
    IRenderer^ _renderer;

    // Data
    Color _color;   // object's current color
    Color _initColor; // object's initial color
    float _initX;   // object's initial position x, relative to view centerpoint
    float _initY;   // object's initial position y, relative to view centerpoint
    float _initDX;  // object's initial width
    float _initDY;  // object's initial height
    WCHAR _descText[128];
    Windows::Foundation::Point _descPosition; // text position

    // Event tokens for GestureRecognizer events
    Windows::Foundation::EventRegistrationToken _tokenTapped;
    Windows::Foundation::EventRegistrationToken _tokenRightTapped;
    Windows::Foundation::EventRegistrationToken _tokenHolding;
    Windows::Foundation::EventRegistrationToken _tokenManipulationStarted;
    Windows::Foundation::EventRegistrationToken _tokenManipulationUpdated;
    Windows::Foundation::EventRegistrationToken _tokenManipulationInertiaStarting;
    Windows::Foundation::EventRegistrationToken _tokenManipulationCompleted;
};
