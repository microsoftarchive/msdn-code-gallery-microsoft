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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_oAuthFlickr.xaml.h"
#include <string>
#include <sstream>

using namespace SDKSample::WebAuthentication;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Authentication::Web;
//windows app specfic code
using namespace Windows::UI::Popups;
using namespace Windows::Security::Credentials;
using namespace Windows::Storage;
using namespace Windows::Web::Http;
using namespace Windows::Data::Json;


std::vector<std::wstring> &Split(const std::wstring &s, char delim, std::vector<std::wstring> &elems) {
    std::wstringstream ss(s);
    std::wstring item;
    while (std::getline<wchar_t>(ss, item, delim)) {
        elems.push_back(item);
    }
    return elems;
}


std::vector<std::wstring> Split(const std::wstring &s, char delim) {
    std::vector<std::wstring> elems;
    Split(s, delim, elems);
    return elems;
}

Scenario3::Scenario3()
{
    InitializeComponent();

}


/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}



/// <summary>
/// This is the event handler for links added to Accounts Settings pane. This method can do more work based on selected link.
/// </summary>
/// <param name="command">Link instance that triggered the event.</param>
void Scenario3::HandleAppSpecificCmd(IUICommand^ command) 
{ 
	
} 


