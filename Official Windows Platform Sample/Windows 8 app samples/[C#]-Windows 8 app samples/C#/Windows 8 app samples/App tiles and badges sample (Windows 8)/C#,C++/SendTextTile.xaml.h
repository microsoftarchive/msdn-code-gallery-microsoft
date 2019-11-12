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
// SendTextTile.xaml.h
// Declaration of the SendTextTile class
//

#pragma once

#include "pch.h"
#include "SendTextTile.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class SendTextTile sealed
    	{
    	public:
    		SendTextTile();
    
    	protected:
    		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    	private:
    		MainPage^ rootPage;		
    		void UpdateTileWithText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void UpdateTileWithTextWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
