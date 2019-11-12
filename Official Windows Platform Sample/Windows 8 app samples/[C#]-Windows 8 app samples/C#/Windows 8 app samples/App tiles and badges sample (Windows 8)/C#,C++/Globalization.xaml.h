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
// Globalization.xaml.h
// Declaration of the Globalization class
//

#pragma once

#include "pch.h"
#include "Globalization.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
    	public ref class Globalization sealed
    	{
    	public:
    		Globalization();
    
    	protected:
    		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    	private:
    		MainPage^ rootPage;
    		void ViewCurrentResources_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void SendTileNotificationWithQueryStrings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void SendTileNotificationImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void SendTileNotificationText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
