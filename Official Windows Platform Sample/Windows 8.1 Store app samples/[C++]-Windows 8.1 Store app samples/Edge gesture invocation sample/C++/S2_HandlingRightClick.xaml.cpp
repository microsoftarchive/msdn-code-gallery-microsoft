//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "S2_HandlingRightClick.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::EdgeGesture;

S2_HandlingRightClick::S2_HandlingRightClick()
{
    InitializeComponent();
    InitializeEventHandlers();
}

S2_HandlingRightClick::~S2_HandlingRightClick()
{
    UninitializeEventHandlers();
}

void S2_HandlingRightClick::InitializeEventHandlers()
{
    Windows::UI::Input::EdgeGesture^ edgeGesture = Windows::UI::Input::EdgeGesture::GetForCurrentView();
    EdgeGestureStartingEventToken = edgeGesture->Starting += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S2_HandlingRightClick::OnStarting);
    EdgeGestureCompletedEventToken = edgeGesture->Completed += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S2_HandlingRightClick::OnCompleted);
    EdgeGestureCanceledEventToken = edgeGesture->Canceled += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S2_HandlingRightClick::OnCanceled);

    RightTappedEventToken = MainPage::Current->RightTapped += ref new Windows::UI::Xaml::Input::RightTappedEventHandler(this, &S2_HandlingRightClick::OnRightTapped);
    RightTappedOverrideEventToken = this->RightClickRegion->RightTapped += ref new Windows::UI::Xaml::Input::RightTappedEventHandler(this, &S2_HandlingRightClick::OnRightTappedOverride);

    this->OutputTextBlock1->Text = "Sample initialized and events registered.";
}

void S2_HandlingRightClick::UninitializeEventHandlers()
{
    Windows::UI::Input::EdgeGesture^ edgeGesture = Windows::UI::Input::EdgeGesture::GetForCurrentView();
    edgeGesture->Starting -= EdgeGestureStartingEventToken;
    edgeGesture->Completed -= EdgeGestureCompletedEventToken;
    edgeGesture->Canceled -= EdgeGestureCanceledEventToken;

    MainPage::Current->RightTapped -= RightTappedEventToken;
    this->RightClickRegion->RightTapped -= RightTappedOverrideEventToken;
}

void S2_HandlingRightClick::OnStarting(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
{
    this->OutputTextBlock1->Text = "Invoking with touch.";
}

void S2_HandlingRightClick::OnCompleted(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
{
    // Determine whether it was touch or keyboard invocation
    if (e->Kind == Windows::UI::Input::EdgeGestureKind::Touch)
    {
        this->OutputTextBlock1->Text = "Invoked with touch.";
    }
    else if (e->Kind == Windows::UI::Input::EdgeGestureKind::Keyboard)
    {
        this->OutputTextBlock1->Text = "Invoked with keyboard.";
    }
    else if (e->Kind == Windows::UI::Input::EdgeGestureKind::Mouse)
    {
        this->OutputTextBlock1->Text = "Invoked with right-click.";
    }
}

void S2_HandlingRightClick::OnCanceled(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
{
    this->OutputTextBlock1->Text = "Canceled with touch.";
}

void S2_HandlingRightClick::OnRightTapped(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e)
{
    // With XAML, handling right-click explicitly on the page is required.
    this->OutputTextBlock1->Text = "Invoked with right-click.";
}

void S2_HandlingRightClick::OnRightTappedOverride(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e)
{
    this->OutputTextBlock1->Text = "The RightTapped event was handled.  The page-wide RightTapped event will not fire.";
    e->Handled = true;
}
