//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "LayoutAwarePage.h"

using namespace Clipboard::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

LayoutAwarePage::LayoutAwarePage()
{
    if (Windows::ApplicationModel::DesignMode::DesignModeEnabled)
    {
        return;
    }

    // Create an empty default view model
    DefaultViewModel = ref new Map<String^, Object^>(std::less<String^>());

    // Map application view state to visual state for this page when it is part of the visual tree
    Loaded += ref new RoutedEventHandler(this, &LayoutAwarePage::StartLayoutUpdates);
    Unloaded += ref new RoutedEventHandler(this, &LayoutAwarePage::StopLayoutUpdates);
}

static TypeName _thisType = { LayoutAwarePage::typeid->FullName, TypeKind::Custom };
static TypeName _observableMapType = { IObservableMap<Platform::String^, Object^>::typeid->FullName, TypeKind::Metadata };
static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty = DependencyProperty::Register("DefaultViewModel", _observableMapType, _thisType, nullptr);

DependencyProperty^ LayoutAwarePage::DefaultViewModelProperty::get()
{
    return _defaultViewModelProperty;
}

IObservableMap<Platform::String^, Object^>^ LayoutAwarePage::DefaultViewModel::get()
{
    return safe_cast<Windows::Foundation::Collections::IObservableMap<Platform::String^, Object^>^>(GetValue(DefaultViewModelProperty));
}

void LayoutAwarePage::DefaultViewModel::set(IObservableMap<Platform::String^, Object^>^ value)
{
    SetValue(DefaultViewModelProperty, value);
}

void LayoutAwarePage::GoHome(Object^ sender, RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the topmost page
    if (Frame != nullptr)
    {
        while (Frame->CanGoBack)
        {
            Frame->GoBack();
        }
    }
}

void LayoutAwarePage::GoBack(Object^ sender, RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the previous page
    if ((Frame != nullptr) && (Frame->CanGoBack))
    {
        Frame->GoBack();
    }
}

void LayoutAwarePage::StartLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
    auto control = safe_cast<Control^>(sender);
    if (_layoutAwareControls == nullptr)
    {
        // Start listening to view state changes when there are controls interested in updates
        _layoutAwareControls = ref new Vector<Control^>();
        _windowSizeEventToken = Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &LayoutAwarePage::WindowSizeChanged);
    }

    _layoutAwareControls->Append(control);

    // Set the initial visual state of the control
    VisualStateManager::GoToState(control, DetermineVisualState(ApplicationView::Value), false);
}

void LayoutAwarePage::WindowSizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
    InvalidateVisualState();
}

void LayoutAwarePage::StopLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
    auto control = safe_cast<Control^>(sender);
    unsigned int index;
    if ((_layoutAwareControls != nullptr) && (_layoutAwareControls->IndexOf(control, &index)))
    {
        _layoutAwareControls->RemoveAt(index);
        if (_layoutAwareControls->Size == 0)
        {
            // Stop listening to view state changes when no controls are interested in updates
            Window::Current->SizeChanged -= _windowSizeEventToken;
            _layoutAwareControls = nullptr;
        }
    }
}

Platform::String^ LayoutAwarePage::DetermineVisualState(ApplicationViewState viewState)
{
    switch (viewState)
    {
    case ApplicationViewState::Filled:
        {
            return "Filled";
        }

    case ApplicationViewState::Snapped:
        {
           return "Snapped";
        }

    case ApplicationViewState::FullScreenPortrait:
        {
            return "FullScreenPortrait";
        }

    case ApplicationViewState::FullScreenLandscape:
    default:
        {
            return "FullScreenLandscape";
        }
    }
}

void LayoutAwarePage::InvalidateVisualState()
{
    if (_layoutAwareControls != nullptr)
    {
        String^ visualState = DetermineVisualState(ApplicationView::Value);
        auto controlIterator = _layoutAwareControls->First();
        while (controlIterator->HasCurrent)
        {
            auto control = controlIterator->Current;
            VisualStateManager::GoToState(control, visualState, false);
            controlIterator->MoveNext();
        }
    }
}
