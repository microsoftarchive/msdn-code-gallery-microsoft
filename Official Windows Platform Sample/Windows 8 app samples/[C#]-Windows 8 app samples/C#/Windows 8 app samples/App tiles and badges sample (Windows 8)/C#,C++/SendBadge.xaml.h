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
// SendBadge.xaml.h
// Declaration of the SendBadge class
//

#pragma once

#include "pch.h"
#include "SendBadge.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class SendBadge sealed
    	{
    	public:
    		SendBadge();
    
    	protected:
    		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    	private:
		MainPage^ rootPage;
		void UpdateBadgeWithNumber(int number);
		void UpdateBadgeWithGlyph(int glyphIndex);
		void UpdateBadgeWithNumberWithStringManipulation(int number);
		void UpdateBadgeWithGlyphWithStringManipulation();
		void UpdateBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ClearBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void NumberOrGlyph_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
    	};
    }
}
