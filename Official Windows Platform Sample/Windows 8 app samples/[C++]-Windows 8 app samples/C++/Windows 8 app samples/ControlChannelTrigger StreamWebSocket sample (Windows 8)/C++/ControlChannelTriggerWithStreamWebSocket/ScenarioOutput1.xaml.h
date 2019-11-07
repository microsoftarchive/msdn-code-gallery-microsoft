//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ScenarioOutput1.xaml.h
// Declaration of the ScenarioOutput1 class
//

#pragma once

#include "pch.h"
#include "ScenarioOutput1.g.h"
#include "MainPage.xaml.h"

namespace ControlChannelTrigger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioOutput1 sealed
    {
    public:
        ScenarioOutput1();
        void rootPage_InputFrameLoaded(Object^ sender, Object^ e);

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
    };
}
