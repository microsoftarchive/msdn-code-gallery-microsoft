//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_AddRoutePolicy.xaml.cpp
// Implementation of the S2_AddRoutePolicy class
//

#include "pch.h"
#include "S2_AddRoutePolicy.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ConnectivityManager;

using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;


S2_AddRoutePolicy::S2_AddRoutePolicy()
{
    InitializeComponent();
}


//
//	Add a route policy for the currently connected session
//
void S2_AddRoutePolicy::AddRoutePolicy_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // bail out if no connected session
    if (MainPage::Current->connectionSession == nullptr)
    {
        DisplayWarning("Please establish a connection using the \"Create Connection\" scenario first");
        return;
    }
    // Create an HostName using the provide DNS suffix
    String^ strHostName =  DnsName->Text;
    HostName^ hostName = nullptr;
    if (!strHostName->IsEmpty())
    {
        hostName = ref new HostName(strHostName);
    }

    DomainNameType domainNameType = ParseDomainNameType(((ComboBoxItem^) (DomainNameTypeComboBox->SelectedItem))->Content->ToString());

    // create a RoutePolicy for the specified <HostName, ConnectionProfile>
    RoutePolicy^ routePolicy = ref new RoutePolicy(MainPage::Current->connectionSession->ConnectionProfile,
                                                    hostName,
                                                    domainNameType);
    // finally add the RoutePolicy to the current application
    try
    {
        Windows::Networking::Connectivity::ConnectivityManager::AddHttpRoutePolicy(routePolicy);
        DisplayInfo("Succesfully added a route policy for: " + strHostName);
    }
    catch (Exception^ e)
    {
        DisplayError(e->ToString());
    }
}


//
//	Attempt to remove a route policy for the currently connected session
//
void S2_AddRoutePolicy::RemoveRoutePolicy_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // bail out if no connected session
    if (MainPage::Current->connectionSession == nullptr)
    {
        DisplayWarning("Please establish a connection using the \"Create Connection\" scenario first");
        return;
    }

    // Create an HostName using the provide DNS suffix
    String^ strHostName =  DnsName->Text;
    HostName^ hostName = nullptr;
    if (!strHostName->IsEmpty())
    {
        hostName = ref new HostName(strHostName);
    }

    DomainNameType domainNameType = ParseDomainNameType(((ComboBoxItem^) (DomainNameTypeComboBox->SelectedItem))->Content->ToString());

    // create a RoutePolicy for the specified <HostName, ConnectionProfile>
    RoutePolicy^ routePolicy = ref new RoutePolicy(MainPage::Current->connectionSession->ConnectionProfile,
                                                    hostName,
                                                    domainNameType);
    // finally remove the RoutePolicy to the current application
    try
    {
        Windows::Networking::Connectivity::ConnectivityManager::RemoveHttpRoutePolicy(routePolicy);
        DisplayInfo("Succesfully removed a route policy for: " + strHostName);
    }
    catch (Exception^ e)
    {
        DisplayError(e->ToString());
    }
}

void S2_AddRoutePolicy::DisplayError(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Red);
    OutputText->Text = message;
}

void S2_AddRoutePolicy::DisplayWarning(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Blue);
    OutputText->Text = message;
}

void S2_AddRoutePolicy::DisplayInfo(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Green);
    OutputText->Text = message;
}

DomainNameType S2_AddRoutePolicy::ParseDomainNameType(String^ input)
{
    if (input == "Suffix")
    {
        return DomainNameType::Suffix;
    }
    else
    {
        return DomainNameType::FullyQualified;
    }
}