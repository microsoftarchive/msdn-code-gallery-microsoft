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
// SendWebImageTile.xaml.h
// Declaration of the SendWebImageTile class
//

#pragma once

#include "pch.h"
#include "SendWebImageTile.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class SendWebImageTile sealed
    	{
    	public:
    		SendWebImageTile();
    
    	protected:
    		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    	private:
    		MainPage^ rootPage;
    		void UpdateTileWithWebImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void UpdateTileWithWebImageWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
