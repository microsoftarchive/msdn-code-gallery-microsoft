// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "ScenarioList.xaml.h"

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
using namespace Windows::Phone::UI::Input;

ScenarioList::ScenarioList()
{
	InitializeComponent();
}

/// <summary>
/// On navigation to this frame populate the scenario list.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ScenarioList::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter
	
	SampleTitle->Text = MainPage::FEATURE_NAME;

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
}

void ScenarioList::ScenarioControl_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
	ListBox^ scenarioListBox = safe_cast<ListBox^>(sender);
	ListBoxItem^ item = dynamic_cast<ListBoxItem^>(scenarioListBox->SelectedItem);

	if (item != nullptr)
	{	
		// Navigate to the selected scenario.
		auto scenarioFrame = safe_cast<Windows::UI::Xaml::Controls::Frame^>(MainPage::Current->FindName("ScenarioFrame"));
		TypeName scenarioType = { item->Name, TypeKind::Custom };
		scenarioFrame->Navigate(scenarioType);

		// Clear the selection before we navigate away
		scenarioListBox->SelectedItem = nullptr;
	}
}


void ScenarioList::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
	auto uri = ref new Uri((String^) ((HyperlinkButton^) sender)->Tag);
	Windows::System::Launcher::LaunchUriAsync(uri);
}

