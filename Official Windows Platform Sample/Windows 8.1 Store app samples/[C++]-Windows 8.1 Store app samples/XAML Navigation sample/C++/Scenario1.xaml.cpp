//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"
#include "SimplePage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Navigation;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

// This Scenario shows the basic usage of Navigation in XAML. 
Scenario1::Scenario1()
{
    InitializeComponent();
}

void Scenario1::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	//These two methods will show info about the state of the navigation stacks
	//and also enable or disable buttons from the User Interface.
	ShowInfo();
	UpdateUI();
}

void Scenario1::NavigateButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//Navigate method will display SimplePage content inside the MyFrame element.
	this->MyFrame->Navigate(TypeName(SDKSample::SimplePage::typeid));
	ShowInfo();
	UpdateUI();
}

void Scenario1::GoBackButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//We should verify if there are pages in the navigation back stack 
	//before navigating to the previous page.
	if (this->MyFrame != nullptr && MyFrame->CanGoBack)
	{
	    //Using the GoBack method, the frame navigates to the previous page.
		MyFrame->GoBack();
	}
	ShowInfo();
	UpdateUI();
}

void Scenario1::GoForwardButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//We should verify if there are pages in the navigation forward stack 
	//before navigating to the forward page.
	if (this->MyFrame != nullptr && MyFrame->CanGoForward)
	{
		//Using the GoForward method, the frame navigates to the forward page.
		MyFrame->GoForward();
	}
	ShowInfo();
	UpdateUI();
}

void Scenario1::GoHomeButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the topmost page
	if (this->MyFrame != nullptr)
	{
		while (this->MyFrame->CanGoBack) this->MyFrame->GoBack();
	}
	ShowInfo();
	UpdateUI();
}

void Scenario1::ClearStacksButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    //We can clear the navigation stacks using the Clear method of each stack.
	MyFrame->ForwardStack->Clear();
	MyFrame->BackStack->Clear();
	ShowInfo();
	UpdateUI();
}

void Scenario1::ShowInfo()
{
	if (MyFrame != nullptr)
	{
		int backStackSize = this->MyFrame->BackStack->Size;
		BackStackText->Text = "\nEntries in the navigation Back Stack: #" + backStackSize.ToString();

		int fordwardStackSize = this->MyFrame->ForwardStack->Size;
		ForwardStackText->Text = "\nEntries in the navigation Forward Stack: #" + fordwardStackSize.ToString();

		BackStackListView->Items->Clear();
		//Add the pages from the back stack
		for (int i = 0; i < backStackSize; i++)
		{
			BackStackListView->Items->Append(this->MyFrame->BackStack->GetAt(i)->SourcePageType.Name);
		}
	
		ForwardStackListView->Items->Clear();
		//Add the pages from the forward stack
		for (int i = 0; i < fordwardStackSize; i++)
		{
			ForwardStackListView->Items->Append(this->MyFrame->ForwardStack->GetAt(i)->SourcePageType.Name);
		}
	}
}

void Scenario1::UpdateUI()
{
	GoHomeBtn->IsEnabled = MyFrame->CanGoBack;
	GoForwardBtn->IsEnabled = MyFrame->CanGoForward;
	GoBackBtn->IsEnabled = MyFrame->CanGoBack;
	ClearStacksBtn->IsEnabled = (MyFrame->BackStack->Size > 0 || MyFrame->ForwardStack->Size > 0) ? true : false;
}

