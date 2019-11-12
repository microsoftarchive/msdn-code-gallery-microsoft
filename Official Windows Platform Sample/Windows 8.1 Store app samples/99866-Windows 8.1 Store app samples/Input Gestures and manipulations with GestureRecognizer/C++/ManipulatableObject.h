// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

interface class IRenderer
{
    virtual void RequestRedraw() = 0;
    virtual Windows::Foundation::Size ViewSize() = 0;
};

ref class ManipulatableObject
{
internal:
    ManipulatableObject(_In_opt_ ManipulatableObject^ parent);

public:

    ManipulatableObject^ Parent() { return _parent; }

    // Manipulation update calculations.
    // Handle single finger rotation. Returns modified position.
    Windows::Foundation::Point SingleFingerRotation(
        _In_ Windows::UI::Input::GestureRecognizer^ gestureRecognizer,
        _In_ Windows::Foundation::Point position,
        _In_ float pivotRadius);
    // Limits total manipulation scale to range [min, max]. 0.0f disables min or max limit.
    // Returns modified manipulation delta.
    Windows::UI::Input::ManipulationDelta LimitManipulationScale(
        _In_ Windows::UI::Input::ManipulationDelta delta,
        _In_ float minScale,
        _In_ float maxScale);
    // Update manipulation transform, given manipulation delta and position in parent coordinate system.
    void UpdateManipulationTransform(
        _In_ Windows::UI::Input::ManipulationDelta delta,
        _In_ Windows::Foundation::Point position);

    // Serialization/deserialization
    virtual void Serialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties);
    virtual void Deserialize(_In_ Windows::Foundation::Collections::IPropertySet^ properties);

internal:
    // Local to client(global) coordinate system transform
    // This transform includes all transforms in the object parent chain
    virtual D2D1::Matrix3x2F Transform() { return LocalTransform() * ParentTransform(); }

    // Components of the transform.
    // Local transform, transforms from this object's intrinsic coordinate system to parent coordinate system.
    D2D1::Matrix3x2F LocalTransform() { return D2D1::Matrix3x2F::Translation(_ptInit.x, _ptInit.y) * _mxManip; }
    // Parent transform, transforms from parent coordinate system to client(global) coordinate system.
    D2D1::Matrix3x2F ParentTransform() { return _parent ? _parent->Transform() : D2D1::Matrix3x2F::Identity(); }

    // Components of the local transform.
    void SetInitialPosition(_In_ D2D1_POINT_2F ptInit) { _ptInit = ptInit; }
    void SetManipulationTransform(_In_ D2D1::Matrix3x2F mxManip) { _mxManip = mxManip; }
    D2D1::Matrix3x2F ManipulationTransform() { return _mxManip; }

private:
    ManipulatableObject^ _parent;   // parent object
    D2D1::Matrix3x2F _mxManip;      // manipulation transform
    D2D1_POINT_2F _ptInit;          // initial position
};

ref class ManipulatableObjectTransform sealed : public Windows::UI::Input::IPointerPointTransform
{
public:
    ManipulatableObjectTransform(_In_opt_ ManipulatableObject^ referentObject);

    virtual bool TryTransform(_In_ Windows::Foundation::Point inPoint, _Out_ Windows::Foundation::Point* outPoint);
    virtual Windows::Foundation::Rect TransformBounds(_In_ Windows::Foundation::Rect rect);
    virtual property Windows::UI::Input::IPointerPointTransform^ Inverse { Windows::UI::Input::IPointerPointTransform^ get(); }

private:
    void SetMatrix(D2D1::Matrix3x2F matrix) { _matrix = matrix; }

    D2D1::Matrix3x2F _matrix; // client (global) to object parent coordinate system transform
};
