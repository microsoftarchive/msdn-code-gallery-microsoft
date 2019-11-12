//
// ContinuationPage.xaml.cpp
// Implementation of the ContinuationPage class
//

#include "pch.h"
#include "ContinuationPage.xaml.h"

using namespace PrintSample;

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

ContinuationPage::ContinuationPage()
{
    InitializeComponent();
}

/// <summary>
/// Creates a continuation page and links text-flow to a text flow container
/// </summary>
/// <param name="textLinkContainer">Text link container which will flow text into this page</param>
ContinuationPage::ContinuationPage(RichTextBlockOverflow^ textLinkContainer)
{
    InitializeComponent();
    textLinkContainer->OverflowContentTarget = continuationPageLinkedContainer;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ContinuationPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    (void) e;    // Unused parameter
}
