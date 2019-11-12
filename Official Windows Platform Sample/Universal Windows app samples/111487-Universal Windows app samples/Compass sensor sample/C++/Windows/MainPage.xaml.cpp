// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"

using namespace SDKSample;

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
using namespace Windows::UI::Xaml::Interop;


MainPage^ MainPage::Current = nullptr;

MainPage::MainPage()
{
    InitializeComponent();
    SampleTitle->Text = FEATURE_NAME;

	// This is a static public property that allows downstream pages to get a handle to the MainPage instance
	// in order to call methods that are in this class.
	MainPage::Current = this; 
}

void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{	
	// Populate the ListBox with the scenarios as defined in SampleConfiguration.cpp.
	auto itemCollection = ref new Platform::Collections::Vector<Object^>();
	int i = 1;
	for each(Scenario s in MainPage::Current->scenarios)
	{
		// Create a textBlock to hold the content and apply the ListItemTextStyle from Styles.xaml
		TextBlock^ textBlock = ref new TextBlock();
		ListBoxItem^ item = ref new ListBoxItem();
		auto style = App::Current->Resources->Lookup("ListItemTextStyle");

		textBlock->Text = (i++).ToString() + ") " + s.Title;
		textBlock->Style = safe_cast<Windows::UI::Xaml::Style ^>(style);

		item->Name = s.ClassName;
		item->Content = textBlock;
		itemCollection->Append(item);
	}

	// Set the newly created itemCollection as the ListBox ItemSource.
	ScenarioControl->ItemsSource = itemCollection;

	// Set selected scenario to the first scenario (0) or if we resumed from a PLM termination set it to
	// the previously selected scenario.
	int startingScenarioIndex = 0;

	if (SuspensionManager::SessionState()->HasKey("SelectedScenarioIndex"))
	{
		startingScenarioIndex = safe_cast<int>(SuspensionManager::SessionState()->Lookup("SelectedScenarioIndex"));
	}

    ScenarioControl->SelectedIndex = startingScenarioIndex;
    ScenarioControl->ScrollIntoView(ScenarioControl->SelectedItem);
}


void MainPage::ScenarioControl_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
	ListBox^ scenarioListBox = safe_cast<ListBox^>(sender); //as ListBox;
	ListBoxItem^ item = dynamic_cast<ListBoxItem^>(scenarioListBox->SelectedItem);

	if (item != nullptr)
	{
		// Clear the status block when changing scenarios
		NotifyUser("", NotifyType::StatusMessage);

		// Store the selected scenario in SuspensionManager
		SuspensionManager::SessionState()->Insert("SelectedScenarioIndex", scenarioListBox->SelectedIndex);
		
		// Navigate to the selected scenario.
		TypeName scenarioType = { item->Name, TypeKind::Custom };		
		ScenarioFrame->Navigate(scenarioType, this);
	}
}

void MainPage::NotifyUser(String^ strMessage, NotifyType type)
{
	switch (type)
	{
	case NotifyType::StatusMessage:
		StatusBorder->Background = ref new SolidColorBrush(Windows::UI::Colors::Green);
		break;
	case NotifyType::ErrorMessage:
		StatusBorder->Background = ref new SolidColorBrush(Windows::UI::Colors::Red);
		break;
	default:
		break;
	}
	StatusBlock->Text = strMessage;

	// Collapse the StatusBlock if it has no text to conserve real estate.
	if (StatusBlock->Text != "")
	{
		StatusBorder->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
	else
	{
		StatusBorder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
	auto uri = ref new Uri((String^) ((HyperlinkButton^) sender)->Tag);
	Windows::System::Launcher::LaunchUriAsync(uri);
}