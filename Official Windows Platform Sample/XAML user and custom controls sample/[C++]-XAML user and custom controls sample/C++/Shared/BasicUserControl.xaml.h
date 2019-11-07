//
// BasicUserControl.xaml.h
// Declaration of the BasicUserControl class
//

#pragma once

#include "pch.h"
#include "BasicUserControl.g.h"

namespace SDKSample
{
    namespace UserAndCustomControls
    {
    	public ref class BasicUserControl sealed
    	{
    	public:
    		BasicUserControl();
    	private:
    		void ClickMeButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
