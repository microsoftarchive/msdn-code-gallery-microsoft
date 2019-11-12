//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace DisablingScreenCapture
    {
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
        private:
            MainPage^ rootPage;
        };
    }
}
