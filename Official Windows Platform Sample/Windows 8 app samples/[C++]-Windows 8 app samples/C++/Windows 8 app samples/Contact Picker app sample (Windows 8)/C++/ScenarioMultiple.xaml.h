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
// ScenarioMultiple.xaml.h
// Declaration of the ScenarioMultiple class
//

#pragma once

#include "pch.h"
#include "ScenarioMultiple.g.h"
#include "MainPage.xaml.h"

namespace ContactPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public ref class ScenarioMultiple sealed
    {
    public:
        ScenarioMultiple();

    private:
        void PickContactsButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
