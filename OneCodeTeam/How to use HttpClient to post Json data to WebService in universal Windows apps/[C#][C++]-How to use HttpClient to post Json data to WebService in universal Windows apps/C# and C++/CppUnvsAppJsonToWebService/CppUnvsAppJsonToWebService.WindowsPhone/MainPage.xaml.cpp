/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUnvsAppJsonToWebService.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* The sample demonstrates how to use the HttpClient and JsonObject class to
* post JSON data to a web service.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "pch.h"
#include "MainPage.xaml.h"
#include "Person.h"
using namespace CppUnvsAppJsonToWebService;

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

using namespace Windows::Web::Http;
using namespace Windows::Data::Json;
using namespace concurrency;
using namespace Windows::Storage::Streams;
MainPage::MainPage()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter

	// TODO: Prepare page for display here.

	// TODO: If your application contains multiple pages, ensure that you are
	// handling the hardware Back button by registering for the
	// Windows::Phone::UI::Input::HardwareButtons.BackPressed event.
	// If you are using the NavigationHelper provided by some templates,
	// this event is handled for you.
}

void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri(((HyperlinkButton^)sender)->Tag->ToString()));
}

// Start to Call WCF service
void MainPage::Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Clear text of Output textbox
	this->OutputField->Text = "";
	this->statusText->Text = "";
	int age = _wtoi(this->Agetxt->Text->Data());
	if (age == 0)
	{
		NotifyUser("Age Must input number");
		return;
	}
	if (age>120 || age<0)
	{
		NotifyUser(L"Age must be between 0 and 120");
		return;
	}

	this->StartButton->IsEnabled = false;
	HttpClient^ httpClient = ref new HttpClient();
	Uri^ uri = ref new Uri("http://localhost:4848/WCFService.svc/GetData");
	Person^ p = ref new Person();
	p->Name = this->Nametxt->Text;
	p->Age = age;

	JsonObject^ postData = ref new JsonObject();
	postData->SetNamedValue("Name", JsonValue::CreateStringValue(p->Name));
	postData->SetNamedValue("Age", JsonValue::CreateStringValue(p->Age.ToString()));

	// async send "get" request to get response string form service
	IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress> ^accessSQLOp = httpClient->PostAsync(uri, ref new HttpStringContent(postData->Stringify(), UnicodeEncoding::Utf8, "application/json"));
	auto operationTask = create_task(accessSQLOp);
	operationTask.then([this](HttpResponseMessage^ response){
		if (response->StatusCode == HttpStatusCode::Ok)
		{
			try
			{

				auto asyncOperationWithProgress = response->Content->ReadAsStringAsync();
				create_task(asyncOperationWithProgress).then([this](Platform::String^ responJsonText)
				{
					m_result = GetJsonValue(responJsonText);
					Windows::UI::Core::CoreWindow::GetForCurrentThread()->Dispatcher->
						RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
						ref new Windows::UI::Core::DispatchedHandler([this]()
					{
						this->OutputField->Text = m_result;
						this->StartButton->IsEnabled = true;
					}));
				});
			}
			catch (Exception^ ex)
			{
				NotifyUser(ex->Message);
				this->StartButton->IsEnabled = true;
			}
		}
	});
}

/// <summary>
/// Get Result from Json String
/// </summary>
/// <param name="jsonString">Json string which returns from WCF Service</param>
/// <returns>Result string</returns>
Platform::String^ MainPage::GetJsonValue(Platform::String^ jsonString)
{
	std::wstring wstring = jsonString->Data();
	int ValueLength = wstring.find_last_of(L"\"") - (wstring.find(L":") + 2);
	std::wstring resultString = wstring.substr(wstring.find(L":") + 2, ValueLength);
	Platform::String^  value = ref new Platform::String(resultString.c_str());
	return value;
}

void MainPage::NotifyUser(Platform::String^ strMessage)
{
	this->statusText->Text = strMessage;
}