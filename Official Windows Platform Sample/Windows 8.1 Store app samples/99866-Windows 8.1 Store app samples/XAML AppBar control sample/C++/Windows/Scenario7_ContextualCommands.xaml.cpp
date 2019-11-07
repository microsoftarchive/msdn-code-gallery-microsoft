//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7_ContextualCommands.xaml.h"
#include "MainPage.xaml.h"
#include "GridViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AppBarControl;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;


SDKSample::AppBarControl::Scenario7::Scenario7()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}

void SDKSample::AppBarControl::Scenario7::OnNavigatedTo(NavigationEventArgs^ e)
{

}

void SDKSample::AppBarControl::Scenario7::Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TypeName type = { GridViewPage::typeid->FullName, TypeKind::Custom };
	rootPage->Frame->Navigate(type, rootPage);
}
