//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"

using namespace D2D3DTransforms;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Media3D;
using namespace Windows::UI::Xaml::Navigation;
using namespace Microsoft::WRL;

MainPage::MainPage()
{
    m_renderer = ref new D2D3DTransformsRenderer();
    m_renderer->Initialize(Window::Current->CoreWindow, this, DisplayProperties::LogicalDpi);

    InitializeComponent();

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &MainPage::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &MainPage::OnDisplayContentsInvalidated);

    Window::Current->CoreWindow->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &MainPage::OnWindowSizeChanged);

    InitializeValues();
    m_renderer->Render();
}

void MainPage::OnEffectSelectorSelectionChanged(
    Object^ sender,
    SelectionChangedEventArgs^ args
    )
{
    switch (EffectSelector->SelectedIndex)
    {
    case TransformEffect::D2D13DTransform:
        ScaleX->IsEnabled = true; // 3DTransform supports scaling via
        ScaleY->IsEnabled = true; // the Matrix4x4::Scale helper.
        break;
    case TransformEffect::D2D13DPerspectiveTransform:
        ScaleX->IsEnabled = false; // 3DPerspectiveTransform does not have a 'Scale' property. A similar effect
        ScaleY->IsEnabled = false; // can be achieved by changing the Z value on the Local/GlobalOffset property.
        break;
    }

    SetTransformEffect(static_cast<TransformEffect>(EffectSelector->SelectedIndex));
}

void MainPage::OnScaleXValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::ScaleX,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnScaleYValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::ScaleY,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnLocalOffsetXValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::LocalOffsetX,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnLocalOffsetYValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::LocalOffsetY,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnLocalOffsetZValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::LocalOffsetZ,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnRotationXValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::RotationX,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnRotationYValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::RotationY,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnRotationZValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::RotationZ,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnGlobalOffsetXValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::GlobalOffsetX,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnGlobalOffsetYValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::GlobalOffsetY,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnGlobalOffsetZValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::GlobalOffsetZ,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnPerspectiveValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ args)
{
    SetTransformProperty(
        TransformProperty::Perspective,
        static_cast<float>(args->NewValue)
        );
}

void MainPage::OnRestoreDefaultsClick(Object^ sender, RoutedEventArgs^ args)
{
    ResetValues();
    m_renderer->Render();
}

void MainPage::InitializeValues()
{
    // Retrieve user-manipulated variables from the LocalSettings collection if the app was previously terminated
    // after being suspended. If the app is resumed from a suspended state, the properties will be preserved regardless.
    PersistentState^ stateHelper = ref new PersistentState();
    stateHelper->Initialize(ApplicationData::Current->LocalSettings->Values, "appSettings");

    EffectSelector->SelectedIndex = stateHelper->LoadInt32("CurrentEffect", 0);
    ScaleX->Value = stateHelper->LoadSingle("ScaleX", 1);
    ScaleY->Value = stateHelper->LoadSingle("ScaleY", 1);
    LocalOffsetX->Value = stateHelper->LoadSingle("LocalOffsetX", 0);
    LocalOffsetY->Value = stateHelper->LoadSingle("LocalOffsetY", 0);
    LocalOffsetZ->Value = stateHelper->LoadSingle("LocalOffsetZ", 0);
    RotationX->Value = stateHelper->LoadSingle("RotationX", 0);
    RotationY->Value = stateHelper->LoadSingle("RotationY", 0);
    RotationZ->Value = stateHelper->LoadSingle("RotationZ", 0);
    GlobalOffsetX->Value = stateHelper->LoadSingle("GlobalOffsetX", 0);
    GlobalOffsetY->Value = stateHelper->LoadSingle("GlobalOffsetY", 0);
    GlobalOffsetZ->Value = stateHelper->LoadSingle("GlobalOffsetZ", 0);
    Perspective->Value = stateHelper->LoadSingle("Perspective", 1500);
}

void MainPage::ResetValues()
{
    EffectSelector->SelectedIndex = 0;
    ScaleX->Value = 1;
    ScaleY->Value = 1;
    LocalOffsetX->Value = 0;
    LocalOffsetY->Value = 0;
    LocalOffsetZ->Value = 0;
    RotationX->Value = 0;
    RotationY->Value = 0;
    RotationZ->Value = 0;
    GlobalOffsetX->Value = 0;
    GlobalOffsetY->Value = 0;
    GlobalOffsetZ->Value = 0;
    Perspective->Value = 1500;
}

void MainPage::SetTransformEffect(TransformEffect transformEffect)
{
    m_renderer->SetTransformEffect(transformEffect);
}

void MainPage::SetTransformProperty(TransformProperty transformProperty, float value)
{
    m_renderer->SetTransformProperty(transformProperty, value);
}

void MainPage::OnSuspending(
    Object^ sender,
    SuspendingEventArgs^ args
    )
{
    // Store user-manipulated properties in the LocalSettings collection.
    PersistentState^ stateHelper = ref new PersistentState();
    stateHelper->Initialize(ApplicationData::Current->LocalSettings->Values, "appSettings");

    stateHelper->SaveInt32("CurrentEffect", EffectSelector->SelectedIndex);
    stateHelper->SaveSingle("ScaleX", static_cast<float>(ScaleX->Value));
    stateHelper->SaveSingle("ScaleY", static_cast<float>(ScaleY->Value));
    stateHelper->SaveSingle("LocalOffsetX", static_cast<float>(LocalOffsetX->Value));
    stateHelper->SaveSingle("LocalOffsetY", static_cast<float>(LocalOffsetY->Value));
    stateHelper->SaveSingle("LocalOffsetZ", static_cast<float>(LocalOffsetZ->Value));
    stateHelper->SaveSingle("RotationX", static_cast<float>(RotationX->Value));
    stateHelper->SaveSingle("RotationY", static_cast<float>(RotationY->Value));
    stateHelper->SaveSingle("RotationZ", static_cast<float>(RotationZ->Value));
    stateHelper->SaveSingle("GlobalOffsetX", static_cast<float>(GlobalOffsetX->Value));
    stateHelper->SaveSingle("GlobalOffsetY", static_cast<float>(GlobalOffsetY->Value));
    stateHelper->SaveSingle("GlobalOffsetZ", static_cast<float>(GlobalOffsetZ->Value));
    stateHelper->SaveSingle("Perspective", static_cast<float>(Perspective->Value));
}

void MainPage::OnLogicalDpiChanged(
    Object^ sender
    )
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

void MainPage::OnDisplayContentsInvalidated(
    Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    m_renderer->ValidateDevice();

    m_renderer->Render();
}

void MainPage::OnWindowSizeChanged(
    CoreWindow^ sender,
    WindowSizeChangedEventArgs^ args
    )
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        VisualStateManager::GoToState(this->LayoutControl, "SnappedState", true);
    }
    else
    {
        VisualStateManager::GoToState(this->LayoutControl, "UnsnappedState", true);
    }

    m_renderer->UpdateForViewStateChanged(ApplicationView::Value);
    m_renderer->UpdateForWindowSizeChange();
}
