// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// SettingsFlyout2.xaml.cpp
// Implementation of the SettingsFlyout2 class
//

#include "pch.h"
#include "SettingsFlyout2.xaml.h"

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

SettingsFlyout2::SettingsFlyout2()
{
	InitializeComponent();

	this->_navigationShortcutsRegistered = false;

	// Handle all key events when loaded into visual tree
	Loaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &SettingsFlyout2::OnLoaded);
	Unloaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &SettingsFlyout2::OnUnloaded);
	this->_isSecondContentLayer = false;
}

void SettingsFlyout2::OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	_acceleratorKeyEventToken = Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated +=
		ref new TypedEventHandler<CoreDispatcher^, AcceleratorKeyEventArgs^>(this,
		&SettingsFlyout2::SettingsFlyout2_AcceleratorKeyActivated);
	_navigationShortcutsRegistered = true;
}

void SettingsFlyout2::OnUnloaded(Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
	if (_navigationShortcutsRegistered)
	{
		Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated -= _acceleratorKeyEventToken;
		_navigationShortcutsRegistered = false;
	}
}

/// <summary>
/// This is the handler for the button Click event. Content of the second layer is dynamically 
/// generated and shown.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SDKSample::ApplicationSettings::SettingsFlyout2::button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = dynamic_cast<Button^>(sender);
	if (b != nullptr)
	{
		// Attach BackClick handler to override default back button behavior
		this->backClickEventRegistrationToken = this->BackClick += ref new BackClickEventHandler(this, &SettingsFlyout2::backClick);

		// Create second layer of content
		TextBlock^ header = ref new TextBlock();
		header->Text = "Layer 2 Content Header";
		header->Style = static_cast<Windows::UI::Xaml::Style^>(Application::Current->Resources->Lookup("TitleTextBlockStyle"));
		TextBlock^ tb = ref new TextBlock();
		tb->Text = "Layer 2 of content.  Click the back button to return to the previous content.";
		tb->Style = static_cast<Windows::UI::Xaml::Style^>(Application::Current->Resources->Lookup("BodyTextBlockStyle"));

		StackPanel^ sp = ref new StackPanel();
		sp->Children->Append(header);
		sp->Children->Append(tb);
		this->Content = sp;

		this->_isSecondContentLayer = true;
	}
}

/// <summary>
/// This is the handler for the SettingsFlyout2 BackClick event. Original content is restored 
/// and the event args are marked as handled.
/// This handler is only attached when the second content layer is visible.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SDKSample::ApplicationSettings::SettingsFlyout2::backClick(Platform::Object^ sender, BackClickEventArgs^ e)
{
	// Return to previous content and remove BackClick handler
	e->Handled = true;
	this->_isSecondContentLayer = false;
	this->Content = this->content1;
	this->BackClick -= this->backClickEventRegistrationToken;
}

/// <summary>
/// Invoked on every keystroke, including system keys such as Alt key combinations, when
/// this page is active and occupies the entire window.  Used to detect keyboard back 
/// navigation via Alt+Left key combination.
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
void SettingsFlyout2::SettingsFlyout2_AcceleratorKeyActivated(CoreDispatcher^ sender, AcceleratorKeyEventArgs^ args)
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

			// If in second content layer, return to previous content
			// Otherwise, dismiss the SettingsFlyout
			if (this->_isSecondContentLayer)
			{
				this->_isSecondContentLayer = false;
				this->Content = this->content1;
				this->BackClick -= this->backClickEventRegistrationToken;
			}
			else
			{
				this->Hide();
			}
		}
	}
}