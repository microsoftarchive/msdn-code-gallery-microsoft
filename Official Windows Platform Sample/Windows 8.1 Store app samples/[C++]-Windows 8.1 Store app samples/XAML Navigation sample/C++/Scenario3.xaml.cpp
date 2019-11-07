//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "CancelNavigationPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Navigation;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// This scenario shows how navigation can be cancelled
Scenario3::Scenario3()
{
	InitializeComponent();
}

void Scenario3::NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// CancelNavigationPage will implement the logic to cancel a navigation
	this->MyFrame->Navigate(TypeName(CancelNavigationPage::typeid));
}
