//
// PopupInputContent.xaml.cpp
// Implementation of the PopupInputContent class
//

#include "pch.h"
#include "PopupInputContent.xaml.h"

using namespace SDKSample::XAMLPopup;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

PopupInputContent::PopupInputContent()
{
	InitializeComponent();
}

// Handles the Click event of the 'Save' button simulating a save and close
void SDKSample::XAMLPopup::PopupInputContent::SimulateSaveClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// in this example we assume the parent of the UserControl is a Popup
	Popup^ p = dynamic_cast<Popup^>(this->Parent);
	p->IsOpen = false; // close the Popup
}
