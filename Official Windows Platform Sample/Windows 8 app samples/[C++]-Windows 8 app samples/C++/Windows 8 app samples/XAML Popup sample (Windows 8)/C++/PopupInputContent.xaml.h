//
// PopupInputContent.xaml.h
// Declaration of the PopupInputContent class
//

#pragma once

#include "PopupInputContent.g.h"

namespace SDKSample
{
    namespace XAMLPopup
    {
    	public ref class PopupInputContent sealed
    	{
    	public:
    		PopupInputContent();
    	private:
    		void SimulateSaveClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
