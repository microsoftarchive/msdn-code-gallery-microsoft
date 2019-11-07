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
// ImageProtocols.xaml.h
// Declaration of the ImageProtocols class
//

#pragma once

#include "pch.h"
#include "ImageProtocols.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class ImageProtocols sealed
    	{
    	public:
    		ImageProtocols();
    
    	protected:
    		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    	private:
    		MainPage^ rootPage;
    		Platform::String^ imageRelativePath;
    		void ProtocolList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
    		void SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void PickImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
