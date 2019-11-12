//
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "SimplePageType1Cacheable.xaml.h"
#include "SimplePageType2Cacheable.xaml.h"
#include "SimplePageType3Cacheable.xaml.h"
#include "MainPage.xaml.h"

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
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

//This scenario shows how Navigation Cache works.
Scenario4::Scenario4()
{
	InitializeComponent();
	//We set this value to 2 to study the behavior of the navigation cache.
	MyFrame->CacheSize = 2;
}

void Scenario4::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	//These two methods will show info about the state of the navigation stacks
	//and also enable or disable buttons from the User Interface.
	ShowInfo();
	UpdateUI();
}

void Scenario4::Navigate1ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//Creating a message with the today date
	Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
	Windows::Foundation::DateTime today = calendar->GetDateTime();
	DateTimeFormatter^ dateTimeFormatter = ref new DateTimeFormatter("longtime");
	String^ message = "This Page was created on: " + dateTimeFormatter->Format(today);

	//SimplePageType1Cacheable has NavigationCacheMode property to Enabled
	//so an instance of the page will be created or reused depending the CacheSize
	this->MyFrame->Navigate(TypeName(SimplePageType1Cacheable::typeid), message);
	ShowInfo();
	UpdateUI();
}

void Scenario4::Navigate2ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//Creating a message with the today date
	Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
	Windows::Foundation::DateTime today = calendar->GetDateTime();
	DateTimeFormatter^ dateTimeFormatter = ref new DateTimeFormatter("longtime");
	String^ message = "This Page was created on: " + dateTimeFormatter->Format(today);

	//SimplePageType2Cacheable has NavigationCacheMode property to Enabled
	//so an instance of the page will be created or reused depending the CacheSize
	this->MyFrame->Navigate(TypeName(SimplePageType2Cacheable::typeid), message);
	ShowInfo();
	UpdateUI();
}

void Scenario4::Navigate3ButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//Creating a message with the today date
	Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
	Windows::Foundation::DateTime today = calendar->GetDateTime();
	DateTimeFormatter^ dateTimeFormatter = ref new DateTimeFormatter("longtime");
	String^ message = "This Page was created on: " + dateTimeFormatter->Format(today);

	//SimplePageType3Cacheable has NavigationCacheMode property to Enabled
	//so an instance of the page will be created or reused depending the CacheSize
	this->MyFrame->Navigate(TypeName(SimplePageType3Cacheable::typeid), message);
	ShowInfo();
	UpdateUI();

}

void Scenario4::GoBackButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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

void Scenario4::GoForwardButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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

void Scenario4::GoHomeButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Use the navigation frame to return to the topmost page
	if (this->MyFrame != nullptr)
	{
		while (this->MyFrame->CanGoBack) this->MyFrame->GoBack();
	}
	ShowInfo();
	UpdateUI();
}

void Scenario4::ClearStacksButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//We can clear the navigation stacks using the Clear method of each stack.
	MyFrame->ForwardStack->Clear();
	MyFrame->BackStack->Clear();
	ShowInfo();
	UpdateUI();
}

void Scenario4::ShowInfo()
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

void Scenario4::UpdateUI()
{
	GoHomeBtn->IsEnabled = MyFrame->CanGoBack;
	GoForwardBtn->IsEnabled = MyFrame->CanGoForward;
	GoBackBtn->IsEnabled = MyFrame->CanGoBack;
	ClearStacksBtn->IsEnabled = (MyFrame->BackStack->Size > 0 || MyFrame->ForwardStack->Size > 0) ? true : false;
}

