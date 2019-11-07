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

using namespace DatagramSocketSample::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

/// <summary>
/// Initializes a new instance of the <see cref="LayoutAwarePage"/> class.
/// </summary>
LayoutAwarePage::LayoutAwarePage()
{
    if (Windows::ApplicationModel::DesignMode::DesignModeEnabled) return;

    // Create an empty default view model
    DefaultViewModel = ref new Map<String^, Object^>(std::less<String^>());

    // Map application view state to visual state for this page when it is part of the visual tree
    Loaded += ref new RoutedEventHandler(this, &LayoutAwarePage::StartLayoutUpdates);
    Unloaded += ref new RoutedEventHandler(this, &LayoutAwarePage::StopLayoutUpdates);
}

static TypeName _thisType = { LayoutAwarePage::typeid->FullName, TypeKind::Custom };
static TypeName _observableMapType = { IObservableMap<Platform::String^, Object^>::typeid->FullName, TypeKind::Metadata };
static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty =
    DependencyProperty::Register("DefaultViewModel", _observableMapType, _thisType, nullptr);

/// <summary>
/// Identifies the <see cref="DefaultViewModel"/> dependency property.
/// </summary>
DependencyProperty^ LayoutAwarePage::DefaultViewModelProperty::get()
{
    return _defaultViewModelProperty;
}

/// <summary>
/// Gets an implementation of <see cref="IObservableMap&lt;String, Object&gt;"/> designed to be
/// used as a trivial view model.
/// </summary>
IObservableMap<Platform::String^, Object^>^ LayoutAwarePage::DefaultViewModel::get()
{
    return safe_cast<Windows::Foundation::Collections::IObservableMap<Platform::String^, Object^>^>(GetValue(DefaultViewModelProperty));
}

/// <summary>
/// Sets an implementation of <see cref="IObservableMap&lt;String, Object&gt;"/> designed to be
/// used as a trivial view model.
/// </summary>
void LayoutAwarePage::DefaultViewModel::set(IObservableMap<Platform::String^, Object^>^ value)
{
    SetValue(DefaultViewModelProperty, value);
}

/// <summary>
/// Invoked as an event handler to navigate backward in the page's associated <see cref="Frame"/>
/// until it reaches the top of the navigation stack.
/// </summary>
/// <param name="sender">Instance that triggered the event.</param>
/// <param name="e">Event data describing the conditions that led to the event.</param>
void LayoutAwarePage::GoHome(Object^ sender, RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the topmost page
    if (Frame != nullptr)
    {
        while (Frame->CanGoBack) Frame->GoBack();
    }
}

/// <summary>
/// Invoked as an event handler to navigate backward in the page's associated <see cref="Frame"/>
/// to go back one step on the navigation stack.
/// </summary>
/// <param name="sender">Instance that triggered the event.</param>
/// <param name="e">Event data describing the conditions that led to the event.</param>
void LayoutAwarePage::GoBack(Object^ sender, RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the previous page
    if (Frame != nullptr && Frame->CanGoBack) Frame->GoBack();
}

/// <summary>
/// Invoked as an event handler, typically on the <see cref="Loaded"/> event of a
/// <see cref="Control"/> within the page, to indicate that the sender should start receiving
/// visual state management changes that correspond to application view state changes.
/// </summary>
/// <param name="sender">Instance of <see cref="Control"/> that supports visual state management
/// corresponding to view states.</param>
/// <param name="e">Event data that describes how the request was made.</param>
/// <remarks>The current view state will immediately be used to set the corresponding visual state
/// when layout updates are requested.  A corresponding <see cref="Unloaded"/> event handler
/// connected to <see cref="StopLayoutUpdates"/> is strongly encouraged.  Instances of
/// <see cref="LayoutAwarePage"/> automatically invoke these handlers in their Loaded and Unloaded
/// events.</remarks>
/// <seealso cref="DetermineVisualState"/>
/// <seealso cref="InvalidateVisualState"/>
void LayoutAwarePage::StartLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
    auto control = safe_cast<Control^>(sender);
    if (_layoutAwareControls == nullptr) {
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

/// <summary>
/// Invoked as an event handler, typically on the <see cref="Unloaded"/> event of a
/// <see cref="Control"/>, to indicate that the sender should start receiving visual state
/// management changes that correspond to application view state changes.
/// </summary>
/// <param name="sender">Instance of <see cref="Control"/> that supports visual state management
/// corresponding to view states.</param>
/// <param name="e">Event data that describes how the request was made.</param>
/// <remarks>The current view state will immediately be used to set the corresponding visual state
/// when layout updates are requested.</remarks>
/// <seealso cref="StartLayoutUpdates"/>
void LayoutAwarePage::StopLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
    auto control = safe_cast<Control^>(sender);
    unsigned int index;
    if (_layoutAwareControls != nullptr && _layoutAwareControls->IndexOf(control, &index))
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

/// <summary>
/// Translates <see cref="ApplicationViewState"/> values into strings for visual state management
/// within the page.  The default implementation uses the names of enum values.  Subclasses may
/// override this method to control the mapping scheme used.
/// </summary>
/// <param name="viewState">View state for which a visual state is desired.</param>
/// <returns>Visual state name used to drive the <see cref="VisualStateManager"/></returns>
/// <seealso cref="InvalidateVisualState"/>
Platform::String^ LayoutAwarePage::DetermineVisualState(ApplicationViewState viewState)
{
    switch (viewState)
    {
    case ApplicationViewState::Filled: return "Filled";
    case ApplicationViewState::Snapped: return "Snapped";
    case ApplicationViewState::FullScreenPortrait: return "FullScreenPortrait";
    default: case ApplicationViewState::FullScreenLandscape: return "FullScreenLandscape";
    }
}

/// <summary>
/// Updates all controls that are listening for visual state changes with the correct visual
/// state.
/// </summary>
/// <remarks>
/// Typically used in conjunction with overriding <see cref="DetermineVisualState"/> to
/// signal that a different value may be returned even though the view state has not changed.
/// </remarks>
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
