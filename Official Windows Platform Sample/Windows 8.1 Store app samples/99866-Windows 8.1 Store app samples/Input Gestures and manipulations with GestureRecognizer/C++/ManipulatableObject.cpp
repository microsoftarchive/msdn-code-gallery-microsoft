// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "ManipulatableObject.h"

///////////////////////////////////////////////////////////////////////////////
// ManipulatableObject base class.
//

ManipulatableObject::ManipulatableObject(_In_opt_ ManipulatableObject^ parent)
:   _parent(parent),
    _mxManip(D2D1::Matrix3x2F::Identity()),
    _ptInit(D2D1::Point2F())
{
}

///////////////////////////////////////////////////////////////////////////////
// Manipulation update calculations.
//

Windows::Foundation::Point ManipulatableObject::SingleFingerRotation(
    _In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer,
    _In_ Windows::Foundation::Point position,
    _In_ float pivotRadius)
{
    // Object's center in its own coordinate system is (0,0). In parent coordinate system it is:
    D2D1_POINT_2F objectCenter = LocalTransform().TransformPoint(D2D1::Point2F());

    if (gestureRecognizer->IsInertial)
    {
        // During the inertia phase, GestureRecognizer reports the same center point as it was on the last manipulation update. 
        // Override manipulation center point to be object's center.
        position = Windows::Foundation::Point(objectCenter.x, objectCenter.y);
    }
    else
    {
        // Set pivot for single finger rotation to be object's center.
        gestureRecognizer->PivotCenter = Windows::Foundation::Point(objectCenter.x, objectCenter.y);
        gestureRecognizer->PivotRadius = pivotRadius;        
    }

    return position;
}

Windows::UI::Input::ManipulationDelta ManipulatableObject::LimitManipulationScale(
    _In_ Windows::UI::Input::ManipulationDelta delta,
    _In_ float minScale,
    _In_ float maxScale)
{
    float scale = sqrt(fabs(_mxManip.Determinant()));
    float scaleNew = scale * delta.Scale;

    if ((minScale != 0.0f) && (scaleNew > maxScale) && (scale > 0))
    {
        delta.Scale = maxScale / scale;
    }

    if ((maxScale != 0.0f) && (scaleNew < minScale) && (scale > 0))
    {
        delta.Scale = minScale / scale;
    }

    return delta;
}

void ManipulatableObject::UpdateManipulationTransform(
    _In_ Windows::UI::Input::ManipulationDelta delta,
    _In_ Windows::Foundation::Point position)
{
    // delta and position are in parent's coordinate system
    D2D1_POINT_2F center = D2D1::Point2F(position.X, position.Y);

    D2D1::Matrix3x2F mxDelta = 
        D2D1::Matrix3x2F::Scale(delta.Scale, delta.Scale, center) *
        D2D1::Matrix3x2F::Rotation(delta.Rotation, center) *
        D2D1::Matrix3x2F::Translation(delta.Translation.X, delta.Translation.Y);

    _mxManip = _mxManip * mxDelta;
}

///////////////////////////////////////////////////////////////////////////////
// Serialization/deserialization methods.
//

void ManipulatableObject::Serialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties)
{
    properties->Insert("_11", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._11));
    properties->Insert("_12", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._12));
    properties->Insert("_21", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._21));
    properties->Insert("_22", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._22));
    properties->Insert("_31", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._31));
    properties->Insert("_32", Windows::Foundation::PropertyValue::CreateSingle(_mxManip._32));
}

void ManipulatableObject::Deserialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties)
{
    if (properties->HasKey("_11"))
    {
        _mxManip._11 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_11"))->GetSingle();
    }
    if (properties->HasKey("_12"))
    {
        _mxManip._12 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_12"))->GetSingle();
    }
    if (properties->HasKey("_21"))
    {
        _mxManip._21 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_21"))->GetSingle();
    }
    if (properties->HasKey("_22"))
    {
        _mxManip._22 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_22"))->GetSingle();
    }
    if (properties->HasKey("_31"))
    {
        _mxManip._31 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_31"))->GetSingle();
    }
    if (properties->HasKey("_32"))
    {
        _mxManip._32 = safe_cast<Windows::Foundation::IPropertyValue^>(properties->Lookup("_32"))->GetSingle();
    }
}

///////////////////////////////////////////////////////////////////////////////
// ManipulatableObjectTransform implementation.
//

ManipulatableObjectTransform::ManipulatableObjectTransform(_In_opt_ ManipulatableObject^ referentObject)
:   _matrix(referentObject ? referentObject->Transform() : D2D1::Matrix3x2F::Identity())
{
    // _matrix is IPointerPointTransform transform "input" transform: from client to local coordinate system
    // Transform matrix used by ManipulatableObject is "output" transform: from local to client coordinate system
    if (!_matrix.Invert())
    {
        _matrix = D2D1::Matrix3x2F::Identity();
    }
}

bool ManipulatableObjectTransform::TryTransform(_In_ Windows::Foundation::Point inPoint, _Out_ Windows::Foundation::Point* outPoint)
{
    D2D1_POINT_2F pt = _matrix.TransformPoint(D2D1::Point2F(inPoint.X, inPoint.Y));
    outPoint->X = pt.x;
    outPoint->Y = pt.y;
    return TRUE;
}

Windows::Foundation::Rect ManipulatableObjectTransform::TransformBounds(_In_ Windows::Foundation::Rect rect)
{
    Windows::Foundation::Point center(rect.X + rect.Width/2, rect.Y + rect.Height/2);
    float scale = sqrt(fabs(_matrix.Determinant()));
    if (TryTransform(center, &center) && (scale > 0))
    {
        rect.Width *= scale;
        rect.Height *= scale;
        rect.X = center.X - rect.Width/2;
        rect.Y = center.Y - rect.Height/2;
    }
    return rect;
}

Windows::UI::Input::IPointerPointTransform^ ManipulatableObjectTransform::Inverse::get()
{
    D2D1::Matrix3x2F matrixInv = _matrix;
    if (matrixInv.Invert())
    {
        ManipulatableObjectTransform^ transform = ref new ManipulatableObjectTransform(nullptr);
        transform->SetMatrix(matrixInv);
        return transform;
    }
    else
    {
        return nullptr;
    }
}
