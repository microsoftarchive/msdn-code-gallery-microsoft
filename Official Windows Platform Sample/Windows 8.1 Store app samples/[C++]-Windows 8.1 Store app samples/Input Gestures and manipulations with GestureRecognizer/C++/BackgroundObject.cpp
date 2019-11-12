// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "BackgroundObject.h"
#include "DirectXSample.h"

///////////////////////////////////////////////////////////////////////////////
// Object constructor, destructor and initialization.
//

BackgroundObject::BackgroundObject(_In_opt_ ManipulatableObject^ parent, _In_ IRenderer^ renderer)
:   ManipulatableObject(parent),
    _renderer(renderer)
{
    Initialize();
}

BackgroundObject::~BackgroundObject()
{
}

void BackgroundObject::Initialize()
{
    _pointerDevice = nullptr;
    _deviceDesc[0] = L'\0';

    SetManipulationTransform(D2D1::Matrix3x2F::Identity());
    SetInitialPosition(D2D1::Point2F());
}

void BackgroundObject::SetDevice(_In_opt_ Windows::Devices::Input::IPointerDevice^ pointerDevice)
{
    _pointerDevice = pointerDevice;
}

///////////////////////////////////////////////////////////////////////////////
// Base class ManipulatableObject overrides
//

D2D1::Matrix3x2F BackgroundObject::Transform()
{
    Windows::Foundation::Size size = _renderer->ViewSize();
    SetInitialPosition(D2D1::Point2F(size.Width/2, size.Height/2));
    return __super::Transform();    
}

///////////////////////////////////////////////////////////////////////////////
// Object drawing methods.
//

void BackgroundObject::DrawBackground(
    _In_ ID2D1DeviceContext* deviceContext,
    _In_ ID2D1SolidColorBrush* brush)
{
    Windows::Foundation::Size clientSize = _renderer->ViewSize();
    D2D1::Matrix3x2F mxTransform = Transform();

    // Find out how many tiles we must render to cover visible area.
    int ixMin = 0, ixMax = 0;
    int iyMin = 0, iyMax = 0;
    if ((clientSize.Width != 0) && (clientSize.Height != 0))
    {
        D2D1::Matrix3x2F mxTransformInv = mxTransform;
        if (D2D1InvertMatrix(&mxTransformInv))
        {
            D2D1_POINT_2F clientPoint[] = {
                {0, 0},
                {clientSize.Width - 1, 0},
                {0,  clientSize.Height - 1},
                {clientSize.Width - 1, clientSize.Height - 1}};
            for (int i=0; i<ARRAYSIZE(clientPoint); ++i)
            {
                D2D1_POINT_2F pt = mxTransformInv.TransformPoint(clientPoint[i]);
                int ix = (int)floor(0.5 + pt.x / clientSize.Width);
                int iy = (int)floor(0.5 + pt.y / clientSize.Height);
                if (i == 0)
                {
                    ixMin = ixMax = ix;
                    iyMin = iyMax = iy;
                }
                else
                {
                    ixMin = min(ixMin, ix);
                    ixMax = max(ixMax, ix);
                    iyMin = min(iyMin, iy);
                    iyMax = max(iyMax, iy);
                }
            }
        }
    }

    // Render background tiles.
    deviceContext->SetTransform(&mxTransform);
    for (int ix = ixMin; ix <= ixMax+1; ++ix)
    {
        D2D1_POINT_2F pt0 = {(ix - 0.5f) * clientSize.Width, (iyMin - 0.5f) * clientSize.Height};
        D2D1_POINT_2F pt1 = {(ix - 0.5f) * clientSize.Width, (iyMax + 0.5f) * clientSize.Height};
        deviceContext->DrawLine(pt0, pt1, brush);
    }
    for (int iy = iyMin; iy <= iyMax+1; ++iy)
    {
        D2D1_POINT_2F pt0 = {(ixMin - 0.5f) * clientSize.Width, (iy - 0.5f) * clientSize.Height};
        D2D1_POINT_2F pt1 = {(ixMax + 0.5f) * clientSize.Width, (iy - 0.5f) * clientSize.Height};
        deviceContext->DrawLine(pt0, pt1, brush);        
    }
}

void BackgroundObject::DrawOverlay(
    _In_ ID2D1DeviceContext* deviceContext,
    _In_ IDWriteTextFormat* textFormat,
    _In_ ID2D1SolidColorBrush* brush)
{
    Windows::Foundation::Size clientSize = _renderer->ViewSize();
    brush->SetOpacity(1.0f);

    textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING);
    textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
    textFormat->SetWordWrapping(DWRITE_WORD_WRAPPING_WRAP);

    size_t deviceDescLen = 0;
    (void)StringCchLength(_deviceDesc , ARRAYSIZE(_deviceDesc), &deviceDescLen);
    if (deviceDescLen != 0)
    {
        deviceContext->DrawText(_deviceDesc, (UINT)deviceDescLen, textFormat,
            D2D1::RectF(0, 0, clientSize.Width, clientSize.Height), brush);
    }
}

///////////////////////////////////////////////////////////////////////////////
// Attach/detach GestureRecognizer to/from the object.
//

void BackgroundObject::Attach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer)
{
    // Configure gesture recognizer
    gestureRecognizer->GestureSettings =
        Windows::UI::Input::GestureSettings::Hold                           |
        Windows::UI::Input::GestureSettings::HoldWithMouse                  |
        Windows::UI::Input::GestureSettings::RightTap                       |
        Windows::UI::Input::GestureSettings::ManipulationTranslateX         |
        Windows::UI::Input::GestureSettings::ManipulationTranslateY         |
        Windows::UI::Input::GestureSettings::ManipulationScale              |
        Windows::UI::Input::GestureSettings::ManipulationRotate             |
        Windows::UI::Input::GestureSettings::ManipulationTranslateInertia   |
        Windows::UI::Input::GestureSettings::ManipulationScaleInertia       |
        Windows::UI::Input::GestureSettings::ManipulationRotateInertia;

    // Register all the delegates
    _tokenRightTapped = gestureRecognizer->RightTapped::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::RightTappedEventArgs^>(
                this, &BackgroundObject::OnRightTapped));

    _tokenHolding = gestureRecognizer->Holding::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::HoldingEventArgs^>(
                this, &BackgroundObject::OnHolding));

    _tokenManipulationStarted = gestureRecognizer->ManipulationStarted::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationStartedEventArgs^>(
                this, &BackgroundObject::OnManipulationStarted));

    _tokenManipulationUpdated = gestureRecognizer->ManipulationUpdated::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationUpdatedEventArgs^>(
                this, &BackgroundObject::OnManipulationUpdated));

    _tokenManipulationInertiaStarting = gestureRecognizer->ManipulationInertiaStarting::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationInertiaStartingEventArgs^>(
                this, &BackgroundObject::OnManipulationInertiaStarting));

    _tokenManipulationCompleted = gestureRecognizer->ManipulationCompleted::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationCompletedEventArgs^>(
                this, &BackgroundObject::OnManipulationCompleted));
}

void BackgroundObject::Detach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer)
{
    // Remove all the delegates
    gestureRecognizer->RightTapped::remove(_tokenRightTapped);
    gestureRecognizer->Holding::remove(_tokenHolding);
    gestureRecognizer->ManipulationStarted::remove(_tokenManipulationStarted);
    gestureRecognizer->ManipulationUpdated::remove(_tokenManipulationUpdated);
    gestureRecognizer->ManipulationInertiaStarting::remove(_tokenManipulationInertiaStarting);
    gestureRecognizer->ManipulationCompleted::remove(_tokenManipulationCompleted);
}

///////////////////////////////////////////////////////////////////////////////
// GestureRecognizer event handlers.
//

void BackgroundObject::OnRightTapped(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::RightTappedEventArgs^)
{
    Initialize();
    _renderer->RequestRedraw();
}

void BackgroundObject::OnHolding(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::HoldingEventArgs^ args)
{
    if (args->HoldingState == Windows::UI::Input::HoldingState::Started)
    {
        GetDeviceDescription();
    }
    else
    {
        _deviceDesc[0] = L'\0';
    }
    _renderer->RequestRedraw();
}

void BackgroundObject::OnManipulationStarted(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationStartedEventArgs^)
{
}

void BackgroundObject::OnManipulationUpdated(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args)
{
    UpdateManipulationTransform(
        LimitManipulationScale(args->Delta, 0.1f, 10.0f),
        args->Position);
    _renderer->RequestRedraw();
}

void BackgroundObject::OnManipulationInertiaStarting(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationInertiaStartingEventArgs^)
{
}

void BackgroundObject::OnManipulationCompleted(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationCompletedEventArgs^)
{
}

void BackgroundObject::GetDeviceDescription()
{
    try
    {
        DX::ThrowIfFailed((_pointerDevice != nullptr) ? S_OK : E_FAIL);

        Windows::Devices::Input::PointerDeviceType type = _pointerDevice->PointerDeviceType;
        boolean integrated = _pointerDevice->IsIntegrated;
        UINT32 maxContacts = _pointerDevice->MaxContacts;
        Windows::Foundation::Rect rcPhysical = _pointerDevice->PhysicalDeviceRect;
        Windows::Foundation::Rect rcScreen = _pointerDevice->ScreenRect;

        StringCchPrintfW(_deviceDesc, ARRAYSIZE(_deviceDesc), L"%s %s\nMaxContacts=%u\nPhysical=(%g,%g)(%gx%g)\nScreen=(%g,%g)(%gx%g)",
            (type == Windows::Devices::Input::PointerDeviceType::Touch) ? L"Touch" :
                (type == Windows::Devices::Input::PointerDeviceType::Pen)   ? L"Pen" :
                (type == Windows::Devices::Input::PointerDeviceType::Mouse) ? L"Mouse" :
                L"Unknown",
            integrated ? L"Integrated" : L"External",
            maxContacts,
            rcPhysical.X,
            rcPhysical.Y,
            rcPhysical.Width,
            rcPhysical.Height,
            rcScreen.X,
            rcScreen.Y,
            rcScreen.Width,
            rcScreen.Height);
    }
    catch (Platform::COMException^)
    {
        StringCchCopyW(_deviceDesc, ARRAYSIZE(_deviceDesc), L"?");
    }
}
