//
// BasicUserControl.xaml.cpp
// Implementation of the BasicUserControl class
//

#include "pch.h"
#include "BasicUserControl.xaml.h"

using namespace SDKSample::UserAndCustomControls;

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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

BasicUserControl::BasicUserControl()
{
	InitializeComponent();
}


void SDKSample::UserAndCustomControls::BasicUserControl::ClickMeButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	OutputText->Text = "Hello " + NameInput->Text;
}