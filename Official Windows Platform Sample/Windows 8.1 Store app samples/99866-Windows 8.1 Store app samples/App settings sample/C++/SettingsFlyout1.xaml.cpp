// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// SettingsFlyout1.xaml.cpp
// Implementation of the SettingsFlyout1 class
//

#include "pch.h"
#include "SettingsFlyout1.xaml.h"

using namespace SDKSample::ApplicationSettings;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;
using namespace Windows::UI::Core;
using namespace Windows::System;

// The SettingsFlyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

SettingsFlyout1::SettingsFlyout1()
{
	InitializeComponent();

	this->_navigationShortcutsRegistered = false;

	Loaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &SettingsFlyout1::OnLoaded);
	Unloaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &SettingsFlyout1::OnUnloaded);
}

void SettingsFlyout1::OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	_acceleratorKeyEventToken = Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated +=
		ref new TypedEventHandler<CoreDispatcher^, AcceleratorKeyEventArgs^>(this,
		&SettingsFlyout1::SettingsFlyout1_AcceleratorKeyActivated);
	_navigationShortcutsRegistered = true;
}

void SettingsFlyout1::OnUnloaded(Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
	if (_navigationShortcutsRegistered)
	{
		Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated -= _acceleratorKeyEventToken;
		_navigationShortcutsRegistered = false;
	}
}

/// <summary>
/// Invoked on every keystroke, including system keys such as Alt key combinations, when
/// this page is active and occupies the entire window.  Used to detect keyboard back 
/// navigation via Alt+Left key combination.
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
void SettingsFlyout1::SettingsFlyout1_AcceleratorKeyActivated(CoreDispatcher^ sender, AcceleratorKeyEventArgs^ args)
{
	// Only investigate further when Left is pressed
	if (args->EventType == CoreAcceleratorKeyEventType::SystemKeyDown &&
		args->VirtualKey == VirtualKey::Left)
	{
		auto coreWindow = Window::Current->CoreWindow;
		auto downState = Windows::UI::Core::CoreVirtualKeyStates::Down;

		// Check for modifier keys
		// The Menu VirtualKey signifies Alt
		bool menuKey = (coreWindow->GetKeyState(VirtualKey::Menu) & downState) == downState;
		bool controlKey = (coreWindow->GetKeyState(VirtualKey::Control) & downState) == downState;
		bool shiftKey = (coreWindow->GetKeyState(VirtualKey::Shift) & downState) == downState;

		if (menuKey && !controlKey && !shiftKey)
		{
			args->Handled = true;
			this->Hide();
		}
	}
}

