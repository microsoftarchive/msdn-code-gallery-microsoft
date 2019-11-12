//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5_CustomContent.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AppBarControl;

using namespace Windows::UI::Xaml;
using namespace Platform::Collections;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;


Scenario5::Scenario5()
{
    InitializeComponent();
	rootPage = MainPage::Current;
	leftItems = ref new Vector<UIElement^>();
}

void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Add some custom (non-AppBarButton) content
	leftPanel = safe_cast<StackPanel^>(rootPage->FindName("LeftPanel"));

	// Save off old content
	for each (UIElement^ element in leftPanel->Children)
	{
		leftItems->Append(element);
	}

	leftPanel->Children->Clear();

	// Create a combo box
	ComboBox^ cb = ref new ComboBox();
	cb->Height = 32.0;
	cb->Width = 100.0;
	cb->Items->Append("Baked");
	cb->Items->Append("Fried");
	cb->Items->Append("Frozen");
	cb->Items->Append("Chilled");

	cb->SelectedIndex = 0;

	leftPanel->Children->Append(cb);

	// Create a text box
	TextBox^ tb = ref new TextBox();
	tb->Text = "Search for desserts.";
	tb->Width = 300.0;
	tb->Height = 30.0;
	tb->Margin = Thickness(10.0, 0.0, 0.0, 0.0);
	tb->GotFocus += ref new RoutedEventHandler(this, &Scenario5::tb_GotFocus);

	leftPanel->Children->Append(tb);

	// Add a button
	Button^ b = ref new Button();
	b->Content = "Search";
	b->Click += ref new RoutedEventHandler(this, &Scenario5::b_Click);

	leftPanel->Children->Append(b);
}

void Scenario5::OnNavigatedFrom(NavigationEventArgs^ e)
{
	leftPanel->Children->Clear();
	for each (UIElement^ element in leftItems)
	{
		leftPanel->Children->Append(element);
	}
}

void Scenario5::tb_GotFocus(Object^ sender, RoutedEventArgs^ e)
{
	TextBox^ tb = safe_cast<TextBox^>(sender);
	tb->Text = "";
}

void Scenario5::b_Click(Object^ sender, RoutedEventArgs^ e)
{
	rootPage->NotifyUser("Search button pressed", NotifyType::StatusMessage);
}