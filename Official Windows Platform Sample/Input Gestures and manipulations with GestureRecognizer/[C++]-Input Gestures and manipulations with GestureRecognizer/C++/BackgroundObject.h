// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#include "ManipulatableObject.h"

ref class BackgroundObject sealed : public ManipulatableObject
{
public:
    BackgroundObject(_In_opt_ ManipulatableObject^ parent, _In_ IRenderer^ renderer);

    // Initialize object state.
    void Initialize();

    // Set current pointer device at the beginning of the interaction.
    void SetDevice(_In_opt_ Windows::Devices::Input::IPointerDevice^ pointerDevice);

    // Attaches/detaches gesture recognizer to/from the object.
    void Attach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer);
    void Detach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer);

internal:
    // Base class ManipulatableObject overrides.
    virtual D2D1::Matrix3x2F Transform() override;

    // Draw background, given D2D objects.
    void DrawBackground(
        _In_ ID2D1DeviceContext* deviceContext,
        _In_ ID2D1SolidColorBrush* brush);

    // Draw overlay text, given D2D objects.
    void DrawOverlay(
        _In_ ID2D1DeviceContext* deviceContext,
        _In_ IDWriteTextFormat* textFormat,
        _In_ ID2D1SolidColorBrush* brush);

private:
    void GetDeviceDescription();
    ~BackgroundObject();

    // GestureRecognizer event handlers.
    void OnHolding(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::HoldingEventArgs^ args);
    void OnRightTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::RightTappedEventArgs^ args);
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

private:
    IRenderer^ _renderer;

    // Data
    Windows::Devices::Input::IPointerDevice^ _pointerDevice;
    WCHAR _deviceDesc[1024];

    // Event tokens for GestureRecognizer events
    Windows::Foundation::EventRegistrationToken _tokenRightTapped;
    Windows::Foundation::EventRegistrationToken _tokenHolding;
    Windows::Foundation::EventRegistrationToken _tokenManipulationStarted;
    Windows::Foundation::EventRegistrationToken _tokenManipulationUpdated;
    Windows::Foundation::EventRegistrationToken _tokenManipulationInertiaStarting;
    Windows::Foundation::EventRegistrationToken _tokenManipulationCompleted;
};
