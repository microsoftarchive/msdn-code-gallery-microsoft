// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "ScenarioList.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// Page that displays available sample scenarios in a ListBox
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class ScenarioList sealed
	{
	public:
		ScenarioList();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		void ScenarioControl_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
