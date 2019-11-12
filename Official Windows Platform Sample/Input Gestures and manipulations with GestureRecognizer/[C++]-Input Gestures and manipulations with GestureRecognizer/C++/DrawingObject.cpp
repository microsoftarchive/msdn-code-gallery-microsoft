// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "DrawingObject.h"

///////////////////////////////////////////////////////////////////////////////
// Object constructor, destructor and initialization.
//

DrawingObject::DrawingObject(_In_opt_ ManipulatableObject^ parent, _In_ IRenderer^ renderer)
:   ManipulatableObject(parent),
    _renderer(renderer),
    _color(Color::MaxCount),
    _initColor(Color::MaxCount),
    _initX(0),
    _initY(0),
    _initDX(0),
    _initDY(0)
{
    _descText[0] = L'\0';
    _descPosition.X = 0;
    _descPosition.Y = 0;
}

DrawingObject::~DrawingObject()
{
}

void DrawingObject::Initialize(_In_ Color color, _In_ float x, _In_ float y, _In_ float dx, _In_ float dy)
{
    _color = color;
    _initColor = color;
    // Normalize rectangle
    if (dx < 0)
    {
        x += dx;
        dx = -dx;
    }
    if (dy < 0)
    {
        y += dy;
        dy = -dy;
    }
    _initX = x;
    _initY = y;
    _initDX = dx;
    _initDY = dy;
    _descText[0] = L'\0';
    _descPosition.X = 0;
    _descPosition.Y = 0;

    SetInitialPosition(D2D1::Point2F(_initX + _initDX/2, _initY + _initDX/2));
    SetManipulationTransform(D2D1::Matrix3x2F::Identity());
}

///////////////////////////////////////////////////////////////////////////////
// Object drawing methods.
//

void DrawingObject::Draw(
    _In_ ID2D1DeviceContext* deviceContext,
    _In_ ID2D1SolidColorBrush* brush)
{
    // Cumulative transformation matrix.
    D2D1::Matrix3x2F mxTransform = Transform();
    deviceContext->SetTransform(&mxTransform);

    // Draw the object.
    D2D1_RECT_F rect = D2D1::RectF(-_initDX/2, -_initDY/2, _initDX/2, _initDY/2);
    deviceContext->FillRectangle(rect, brush);
    deviceContext->DrawRectangle(rect, brush);

    // Restore our transform to nothing.
    deviceContext->SetTransform(D2D1::Matrix3x2F::Identity());
}

void DrawingObject::DrawOverlay(
    _In_ ID2D1DeviceContext* deviceContext,
    _In_ IDWriteTextFormat* textFormat,
    _In_ ID2D1SolidColorBrush* brush)
{
    D2D_POINT_2F position = D2D1::Point2F(_descPosition.X, _descPosition.Y);
    position = ParentTransform().TransformPoint(position);

    brush->SetOpacity(1.0f);

    textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
    textFormat->SetWordWrapping(DWRITE_WORD_WRAPPING_NO_WRAP);

    size_t descLen = 0;
    (void)StringCchLength(_descText , ARRAYSIZE(_descText), &descLen);
    if (descLen != 0)
    {
        D2D1_RECT_F descRect = D2D1::RectF(
            position.x - _initDX/2,
            position.y - _initDY/2,
            position.x + _initDX/2,
            position.y + _initDY/2);
        deviceContext->DrawText(_descText, (UINT)descLen, textFormat,
            descRect, brush);
    }
}

///////////////////////////////////////////////////////////////////////////////
// Object hit-testing.
//

bool DrawingObject::HitTest(_In_ Windows::Foundation::Point position) 
{
    D2D1::Matrix3x2F mxLocalInv = LocalTransform();
    if (D2D1InvertMatrix(&mxLocalInv))
    {
        // "Untransform" (x, y) from parent coordinate system to object's initial principal axes coordinate system.
        D2D1_POINT_2F pt = mxLocalInv.TransformPoint(D2D1::Point2F(position.X, position.Y));

        // Hit test against object rectangle
        D2D1_RECT_F rcInit = D2D1::RectF(-_initDX/2, -_initDY/2, _initDX/2, _initDY/2);   // object initial rectangle
        return (pt.x >= rcInit.left) && (pt.x <= rcInit.right) &&
               (pt.y >= rcInit.top) && (pt.y <= rcInit.bottom);
    }

    return false;
}

///////////////////////////////////////////////////////////////////////////////
// Attach/detach GestureRecognizer to/from the object.
//

void DrawingObject::Attach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer)
{
    // Configure gesture recognizer
    gestureRecognizer->GestureSettings =
        Windows::UI::Input::GestureSettings::Tap                            |
        Windows::UI::Input::GestureSettings::Hold                           |
        Windows::UI::Input::GestureSettings::HoldWithMouse                  |
        Windows::UI::Input::GestureSettings::RightTap                       |
        Windows::UI::Input::GestureSettings::ManipulationTranslateX         |
        Windows::UI::Input::GestureSettings::ManipulationTranslateY         |
        Windows::UI::Input::GestureSettings::ManipulationRotate             |
        Windows::UI::Input::GestureSettings::ManipulationScale              |
        Windows::UI::Input::GestureSettings::ManipulationTranslateInertia   |
        Windows::UI::Input::GestureSettings::ManipulationRotateInertia      |
        Windows::UI::Input::GestureSettings::ManipulationScaleInertia;

    // Register all the gesture event handlers
    _tokenTapped = gestureRecognizer->Tapped::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::TappedEventArgs^>(
                this, &DrawingObject::OnTapped));

    _tokenRightTapped = gestureRecognizer->RightTapped::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::RightTappedEventArgs^>(
                this, &DrawingObject::OnRightTapped));

    _tokenHolding = gestureRecognizer->Holding::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::HoldingEventArgs^>(
                this, &DrawingObject::OnHolding));

    _tokenManipulationStarted = gestureRecognizer->ManipulationStarted::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationStartedEventArgs^>(
                this, &DrawingObject::OnManipulationStarted));

    _tokenManipulationUpdated = gestureRecognizer->ManipulationUpdated::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationUpdatedEventArgs^>(
                this, &DrawingObject::OnManipulationUpdated));

    _tokenManipulationInertiaStarting = gestureRecognizer->ManipulationInertiaStarting::add(
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Input::GestureRecognizer^, Windows::UI::Input::ManipulationInertiaStartingEventArgs^>(
                this, &DrawingObject::OnManipulationInertiaStarting));

    _tokenManipulationCompleted = gestureRecognizer->ManipulationCompleted::add(
        ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::GestureRecognizer^,
        Windows::UI::Input::ManipulationCompletedEventArgs^>(this, &DrawingObject::OnManipulationCompleted));
}

void DrawingObject::Detach(_In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer)
{
    // Remove all the event handlers
    gestureRecognizer->Tapped::remove(_tokenTapped);
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

void DrawingObject::OnTapped(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::TappedEventArgs^)
{
    ++_color;
    if (_color >= Color::MaxCount)
    {
        _color = Color::First;
    }
    _renderer->RequestRedraw();
}

void DrawingObject::OnRightTapped(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::RightTappedEventArgs^)
{
    Initialize(_initColor, _initX, _initY, _initDX, _initDY);
    _renderer->RequestRedraw();
}

void DrawingObject::OnHolding(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::HoldingEventArgs^ args)
{
    if (args->HoldingState == Windows::UI::Input::HoldingState::Started)
    {
        GetObjectDescription();
        _descPosition = args->Position;
    }
    else
    {
        _descText[0] = L'\0';
    }
    _renderer->RequestRedraw();
}

void DrawingObject::OnManipulationStarted(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationStartedEventArgs^)
{
}

void DrawingObject::OnManipulationUpdated(
    _In_ Windows::UI::Input::GestureRecognizer^ sender,
    _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args)
{
    Windows::UI::Input::ManipulationDelta delta = args->Delta;
    Windows::Foundation::Point position = args->Position;

    delta = LimitManipulationScale(delta, 0.25f, 20.0f);
    position = SingleFingerRotation(sender, position, sqrt(_initDX*_initDX + _initDY*_initDY) / (2*3));

    UpdateManipulationTransform(delta, position);

    _renderer->RequestRedraw();
}

void DrawingObject::OnManipulationInertiaStarting(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationInertiaStartingEventArgs^)
{
}

void DrawingObject::OnManipulationCompleted(
    _In_ Windows::UI::Input::GestureRecognizer^,
    _In_ Windows::UI::Input::ManipulationCompletedEventArgs^)
{
}

///////////////////////////////////////////////////////////////////////////////
// Display object information.
//

inline double sgn_sqrt(double x)
{
    return ((x >= 0) ? 1 : -1) * sqrt(fabs(x));
}

void DrawingObject::GetObjectDescription()
{
    D2D1::Matrix3x2F mxManip = ManipulationTransform();
    float scale = sqrt(fabs(mxManip.Determinant()));
    float rotation = (float) (atan2(sgn_sqrt(-mxManip._12 * mxManip._21), sgn_sqrt(mxManip._11 * mxManip._22)) * 180 / M_PI);
    StringCchPrintfW(_descText, ARRAYSIZE(_descText), L"T=(%g,%g)\nS=%g\nR=%g",
        mxManip._31, mxManip._32, scale, rotation);
}

///////////////////////////////////////////////////////////////////////////////
// Serialization/deserialization methods.
//

void DrawingObject::Serialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties)
{
    __super::Serialize(properties);
    properties->Insert("Color", Windows::Foundation::PropertyValue::CreateUInt32((unsigned int)_color));
}

void DrawingObject::Deserialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties)
{
    __super::Deserialize(properties);
    if (properties->HasKey("Color"))
    {
        _color            = (Color) safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("Color"))->GetUInt32();
    }
}
