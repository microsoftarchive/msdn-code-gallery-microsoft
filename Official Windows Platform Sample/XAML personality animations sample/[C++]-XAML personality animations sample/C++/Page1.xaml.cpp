//
// Page1.xaml.cpp
// Implementation of the Page1 class
//

#include "pch.h"
#include "Page1.xaml.h"

using namespace SDKSample::PersonalityAnimations;

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Page1::Page1(): messageData(nullptr)
{
	InitializeComponent();

	messageData = ref new MessageData;
	GridView1->ItemsSource = messageData->Items;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Page1::OnNavigatedTo(NavigationEventArgs^ e)
{
	VisualStateManager::GoToState(this, "NavigatedTo", true);
	//(void) e;	// Unused parameter
}
