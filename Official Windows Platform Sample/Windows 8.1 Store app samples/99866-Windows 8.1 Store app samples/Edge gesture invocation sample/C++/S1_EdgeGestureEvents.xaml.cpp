//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "S1_EdgeGestureEvents.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::EdgeGesture;

S1_EdgeGestureEvents::S1_EdgeGestureEvents()
{
    InitializeComponent();
    InitializeEventHandlers();
}

S1_EdgeGestureEvents::~S1_EdgeGestureEvents()
{
    UninitializeEventHandlers();
}

void S1_EdgeGestureEvents::InitializeEventHandlers()
{
    Windows::UI::Input::EdgeGesture^ edgeGesture = Windows::UI::Input::EdgeGesture::GetForCurrentView();
    EdgeGestureStartingEventToken = edgeGesture->Starting += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S1_EdgeGestureEvents::OnStarting);
    EdgeGestureCompletedEventToken = edgeGesture->Completed += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S1_EdgeGestureEvents::OnCompleted);
    EdgeGestureCanceledEventToken = edgeGesture->Canceled += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Input::EdgeGesture^, Windows::UI::Input::EdgeGestureEventArgs^>(this, &S1_EdgeGestureEvents::OnCanceled);

    RightTappedEventToken = MainPage::Current->RightTapped += ref new Windows::UI::Xaml::Input::RightTappedEventHandler(this, &S1_EdgeGestureEvents::OnRightTapped);

    this->OutputTextBlock1->Text = "Sample initialized and events registered.";
}

void S1_EdgeGestureEvents::UninitializeEventHandlers()
{
    Windows::UI::Input::EdgeGesture^ edgeGesture = Windows::UI::Input::EdgeGesture::GetForCurrentView();
    edgeGesture->Starting -= EdgeGestureStartingEventToken;
    edgeGesture->Completed -= EdgeGestureCompletedEventToken;
    edgeGesture->Canceled -= EdgeGestureCanceledEventToken;

    MainPage::Current->RightTapped -= RightTappedEventToken;
}

void S1_EdgeGestureEvents::OnStarting(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
{
    this->OutputTextBlock1->Text = "Invoking with touch.";
}

void S1_EdgeGestureEvents::OnCompleted(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
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

void S1_EdgeGestureEvents::OnCanceled(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e)
{
    this->OutputTextBlock1->Text = "Canceled with touch.";
}

void S1_EdgeGestureEvents::OnRightTapped(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e)
{
    // With XAML, handling right-click explicitly on the page is required.
    this->OutputTextBlock1->Text = "Invoked with right-click.";
}
