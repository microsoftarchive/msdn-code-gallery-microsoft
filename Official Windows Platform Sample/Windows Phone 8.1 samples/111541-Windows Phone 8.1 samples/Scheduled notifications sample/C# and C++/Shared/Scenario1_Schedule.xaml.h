// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario1_Schedule.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario1_Schedule sealed
	{
	public:
		Scenario1_Schedule();
	private:
		MainPage^ rootPage;
		void Schedule_Click(Object^, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ScheduleToast(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
		void ScheduleTile(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
		void ScheduleToastWithStringManipulation(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
		void ScheduleTileWithStringManipulation(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
	};
}
