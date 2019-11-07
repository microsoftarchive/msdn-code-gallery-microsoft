//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario8.xaml.h
// Declaration of the Scenario8 class
//

#pragma once
#include "Scenario8_Localizing.g.h"
#include "MainPage.xaml.h"
#include "IceCream.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario8 sealed
        {
        public:
            Scenario8();

		private:
			MainPage^ rootPage;
		};
    }
}
