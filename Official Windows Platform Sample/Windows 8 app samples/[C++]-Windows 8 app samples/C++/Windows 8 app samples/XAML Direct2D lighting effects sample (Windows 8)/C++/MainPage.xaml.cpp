//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"

using namespace D2DLightingEffects;

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
using namespace Windows::UI::Input;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Microsoft::WRL;

MainPage::MainPage()
{
    m_renderer = ref new D2DLightingEffectsRenderer();
    m_renderer->Initialize(Window::Current->CoreWindow, this, DisplayProperties::LogicalDpi);

    InitializeComponent();

    PointerMoved += ref new PointerEventHandler(this, &MainPage::OnSwapChainPointerMoved);

    Window::Current->CoreWindow->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &MainPage::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &MainPage::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &MainPage::OnDisplayContentsInvalidated);

    InitializeValues();

    // Automatically position light where cursor is on
    // startup without waiting for mouse movement event.
    Point mousePosition = Window::Current->CoreWindow->PointerPosition;
    m_renderer->OnPointerMoved(mousePosition.X, mousePosition.Y);
    m_renderer->Render();
}

void MainPage::OnEffectSelectorSelectionChanged(
    Object^ sender,
    SelectionChangedEventArgs^ e
    )
{
    // Properties not used by the current lighting effect need to be hidden.
    // This is done using the VisualStateManager and the VisualStates defined in MainPage.xaml

    switch (EffectSelector->SelectedIndex)
    {
    case LightingEffect::PointSpecular:
        VisualStateManager::GoToState(this->LayoutControl, "SpecularState", true);
        VisualStateManager::GoToState(this->LayoutControl, "PointState", true);
        break;
    case LightingEffect::SpotSpecular:
        VisualStateManager::GoToState(this->LayoutControl, "SpecularState", true);
        VisualStateManager::GoToState(this->LayoutControl, "SpotState", true);
        break;
    case LightingEffect::DistantSpecular:
        VisualStateManager::GoToState(this->LayoutControl, "SpecularState", true);
        VisualStateManager::GoToState(this->LayoutControl, "DistantState", true);
        break;
    case LightingEffect::PointDiffuse:
        VisualStateManager::GoToState(this->LayoutControl, "DiffuseState", true);
        VisualStateManager::GoToState(this->LayoutControl, "PointState", true);
        break;
    case LightingEffect::SpotDiffuse:
        VisualStateManager::GoToState(this->LayoutControl, "DiffuseState", true);
        VisualStateManager::GoToState(this->LayoutControl, "SpotState", true);
        break;
    case LightingEffect::DistantDiffuse:
        VisualStateManager::GoToState(this->LayoutControl, "DiffuseState", true);
        VisualStateManager::GoToState(this->LayoutControl, "DistantState", true);
        break;
    default:
        throw ref new Platform::FailureException();
        break;
    }

    EffectControls->UpdateLayout();

    SetLightingEffect(static_cast<LightingEffect>(EffectSelector->SelectedIndex));
}

void MainPage::OnLightPositionZValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::LightPositionZ,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnSpecularConstantValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::SpecularConstant,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnSpecularExponentValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::SpecularExponent,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnDiffuseConstantValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::DiffuseConstant,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnSpotFocusValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::Focus,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnLimitingConeAngleValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::LimitingConeAngle,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnAzimuthValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::Azimuth,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnElevationValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::Elevation,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnSurfaceScaleValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    SetLightingProperty(
        LightingProperty::SurfaceScale,
        static_cast<float>(e->NewValue)
        );
}

void MainPage::OnRestoreDefaultsClick(Object^ sender, RoutedEventArgs^ e)
{
    ResetValues();
    m_renderer->Render();
}

void MainPage::InitializeValues()
{
    ResetValues();

    // Retrieve user-manipulated variables from the LocalSettings collection if the app was previously terminated after being suspended.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("CurrentEffect"))
    {
        EffectSelector->SelectedIndex = safe_cast<IPropertyValue^>(settingsValues->Lookup("CurrentEffect"))->GetInt32();
    }

    if (settingsValues->HasKey("LightPositionZ"))
    {
        LightPositionZ->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("LightPositionZ"))->GetDouble();
    }

    if (settingsValues->HasKey("SpecularConstant"))
    {
        SpecularConstant->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("SpecularConstant"))->GetDouble();
    }

    if (settingsValues->HasKey("SpecularExponent"))
    {
        SpecularExponent->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("SpecularExponent"))->GetDouble();
    }

    if (settingsValues->HasKey("DiffuseConstant"))
    {
        DiffuseConstant->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("DiffuseConstant"))->GetDouble();
    }

    if (settingsValues->HasKey("SpotFocus"))
    {
        SpotFocus->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("SpotFocus"))->GetDouble();
    }

    if (settingsValues->HasKey("LimitingConeAngle"))
    {
        LimitingConeAngle->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("LimitingConeAngle"))->GetDouble();
    }

    if (settingsValues->HasKey("Azimuth"))
    {
        Azimuth->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("Azimuth"))->GetDouble();
    }

    if (settingsValues->HasKey("Elevation"))
    {
        Elevation->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("Elevation"))->GetDouble();
    }

    if (settingsValues->HasKey("SurfaceScale"))
    {
        SurfaceScale->Value = safe_cast<IPropertyValue^>(settingsValues->Lookup("SurfaceScale"))->GetDouble();
    }
}

void MainPage::ResetValues()
{
    EffectSelector->SelectedIndex = 0;
    LightPositionZ->Value = 100;
    SpecularConstant->Value = 1;
    SpecularExponent->Value = 2;
    DiffuseConstant->Value = 1;
    SpotFocus->Value = 1;
    LimitingConeAngle->Value = 90;
    Azimuth->Value = 0;
    Elevation->Value = 0;
    SurfaceScale->Value = 3;
}

void MainPage::SetLightingEffect(LightingEffect lightingEffect)
{
    m_renderer->SetLightingEffect(lightingEffect);
    m_renderer->Render();
}

void MainPage::SetLightingProperty(LightingProperty lightingProperty, float value)
{
    m_renderer->SetLightingProperty(lightingProperty, value);
    m_renderer->Render();
}

void MainPage::OnSwapChainPointerMoved(
    Object^ sender,
    PointerRoutedEventArgs^ args
    )
{
    PointerPoint^ point = args->GetCurrentPoint(
        reinterpret_cast<SwapChainBackgroundPanel^>(sender)
        );

    m_renderer->OnPointerMoved(point->Position.X, point->Position.Y);
    m_renderer->Render();
}

void MainPage::OnSuspending(
    Object^ sender,
    SuspendingEventArgs^ args
    )
{
    // Store user-manipulated properties in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value. These values will be retrieved in the InitializeValues method.

    if (settingsValues->HasKey("CurrentEffect"))
    {
        settingsValues->Remove("CurrentEffect");
    }
    settingsValues->Insert("CurrentEffect", PropertyValue::CreateInt32(EffectSelector->SelectedIndex));

    if (settingsValues->HasKey("LightPositionZ"))
    {
        settingsValues->Remove("LightPositionZ");
    }
    settingsValues->Insert("LightPositionZ", PropertyValue::CreateDouble(LightPositionZ->Value));

    if (settingsValues->HasKey("SpecularConstant"))
    {
        settingsValues->Remove("SpecularConstant");
    }
    settingsValues->Insert("SpecularConstant", PropertyValue::CreateDouble(SpecularConstant->Value));

    if (settingsValues->HasKey("SpecularExponent"))
    {
        settingsValues->Remove("SpecularExponent");
    }
    settingsValues->Insert("SpecularExponent", PropertyValue::CreateDouble(SpecularExponent->Value));

    if (settingsValues->HasKey("DiffuseConstant"))
    {
        settingsValues->Remove("DiffuseConstant");
    }
    settingsValues->Insert("DiffuseConstant", PropertyValue::CreateDouble(DiffuseConstant->Value));

    if (settingsValues->HasKey("SpotFocus"))
    {
        settingsValues->Remove("SpotFocus");
    }
    settingsValues->Insert("SpotFocus", PropertyValue::CreateDouble(SpotFocus->Value));

    if (settingsValues->HasKey("LimitingConeAngle"))
    {
        settingsValues->Remove("LimitingConeAngle");
    }
    settingsValues->Insert("LimitingConeAngle", PropertyValue::CreateDouble(LimitingConeAngle->Value));

    if (settingsValues->HasKey("Azimuth"))
    {
        settingsValues->Remove("Azimuth");
    }
    settingsValues->Insert("Azimuth", PropertyValue::CreateDouble(Azimuth->Value));

    if (settingsValues->HasKey("Elevation"))
    {
        settingsValues->Remove("Elevation");
    }
    settingsValues->Insert("Elevation", PropertyValue::CreateDouble(Elevation->Value));

    if (settingsValues->HasKey("SurfaceScale"))
    {
        settingsValues->Remove("SurfaceScale");
    }
    settingsValues->Insert("SurfaceScale", PropertyValue::CreateDouble(SurfaceScale->Value));
}

void MainPage::OnLogicalDpiChanged(
    Object^ sender
    )
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
    m_renderer->Render();
}

void MainPage::OnDisplayContentsInvalidated(
    Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    m_renderer->ValidateDevice();

    if (m_renderer->NeedsResourceUpdate())
    {
        // Reset the effect properties since all resources were lost when the device was lost.
        m_renderer->SetLightingEffect(static_cast<LightingEffect>(EffectSelector->SelectedIndex));
        m_renderer->SetLightingProperty(LightingProperty::LightPositionZ, static_cast<float>(LightPositionZ->Value));
        m_renderer->SetLightingProperty(LightingProperty::SpecularConstant, static_cast<float>(SpecularConstant->Value));
        m_renderer->SetLightingProperty(LightingProperty::SpecularExponent, static_cast<float>(SpecularExponent->Value));
        m_renderer->SetLightingProperty(LightingProperty::DiffuseConstant, static_cast<float>(DiffuseConstant->Value));
        m_renderer->SetLightingProperty(LightingProperty::Focus, static_cast<float>(SpotFocus->Value));
        m_renderer->SetLightingProperty(LightingProperty::LimitingConeAngle, static_cast<float>(LimitingConeAngle->Value));
        m_renderer->SetLightingProperty(LightingProperty::Azimuth, static_cast<float>(Azimuth->Value));
        m_renderer->SetLightingProperty(LightingProperty::Elevation, static_cast<float>(Elevation->Value));
        m_renderer->SetLightingProperty(LightingProperty::SurfaceScale, static_cast<float>(SurfaceScale->Value));

        // Position light at the cursor's last position.
        Point mousePosition = Window::Current->CoreWindow->PointerPosition;
        m_renderer->OnPointerMoved(mousePosition.X, mousePosition.Y);
    }

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
    m_renderer->Render();
}
