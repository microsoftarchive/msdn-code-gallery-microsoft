// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace ScheduledNotificationsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput1 sealed
    {
    public:
        ScenarioInput1();

    protected:
	virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
	MainPage^ rootPage;
	void Schedule_Click(Object^, Windows::UI::Xaml::RoutedEventArgs^ e);
	void ScheduleToast(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
	void ScheduleTile(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
	void ScheduleToastWithStringManipulation(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
	void ScheduleTileWithStringManipulation(Platform::String^ updateString, Windows::Foundation::DateTime dueTime, Platform::String^ idNumberString);
    };
}
