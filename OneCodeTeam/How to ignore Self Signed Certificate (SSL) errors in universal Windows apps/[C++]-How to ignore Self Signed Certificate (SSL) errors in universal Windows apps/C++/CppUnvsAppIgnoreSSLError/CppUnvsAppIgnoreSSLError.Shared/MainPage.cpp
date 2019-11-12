/****************************** Module Header ******************************\
* Module Name:  MainPage.cpp
* Project:      CppUnvsAppIgnoreSSLError
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to ignore SSL errors in universal Windows apps.
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
using namespace Windows::Web::Http::Filters;
using namespace concurrency;
using namespace Windows::Security::Cryptography::Certificates;
using namespace CppUnvsAppIgnoreSSLError;

void MainPage::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	m_httpClient = ref new HttpClient();
	// Create a Base Protocol Filter to add certificate errors I want to ignore...
	m_httpBaseProtocolFilter = ref new HttpBaseProtocolFilter();
}

void MainPage::OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{

}

#if WINAPI_FAMILY==WINAPI_FAMILY_PC_APP
void MainPage::Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
	if (e->NewSize.Width < 700.0)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (e->NewSize.Width < e->NewSize.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}
#endif

void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri(((HyperlinkButton^)sender)->Tag->ToString()));
}

void MainPage::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Uri^ targetUri = ref new Uri(txtURI->Text);
	// Simple GET for URI passed in
	HttpRequestMessage^ request = ref new HttpRequestMessage(HttpMethod::Get, targetUri);

	txtResult->Text = "trying to GET";

	HttpClient^ httpClient = ref new HttpClient();
	create_task(httpClient->SendRequestAsync(request)).then([&](HttpResponseMessage^ response){
		// hit here if no exceptions!
		txtResult->Text = "No Cert errors";
	}).then([=](task<void> t){
		try
		{
			t.get();
		}
		catch (Platform::COMException^ ex)
		{
			txtResult->Text = ex->Message;

			// Mask the HResult and if this is error code 12045 which means there was a certificate error
			if ((ex->HResult & 65535) == 12045)
			{
				// Get a list of the server cert errors
				Windows::Foundation::Collections::IVectorView<ChainValidationResult>^ errors = request->TransportInformation->ServerCertificateErrors;

				if (errors != nullptr)
				{
					unsigned int size = errors->Size;

					for (unsigned int i = 0; i < size; ++i)
					{
						m_httpBaseProtocolFilter->IgnorableServerCertificateErrors->Append(errors->GetAt(i));
					}
				}
			}
		}
	});
}

void MainPage::btnIgnore_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Uri^ targetUri = ref new Uri(txtURI->Text);

	try
	{
		// Create a Client to use just for this request and ignore some cert errors.
		HttpClient^ aTempClient = ref new HttpClient(m_httpBaseProtocolFilter);
		auto errors = m_httpBaseProtocolFilter->IgnorableServerCertificateErrors;
		unsigned int size = errors->Size;
		if (size != 0)
		{
			// Try to execute the request (should not fail now for those two errors)
			HttpRequestMessage^ aTempReq = ref new HttpRequestMessage(HttpMethod::Get, targetUri);

			create_task(aTempClient->SendRequestAsync(aTempReq)).then([&](HttpResponseMessage^ aResp2){

				txtResult->Text = "Ignored errors";
			});
		}		
	}
	catch (COMException^ ex)
	{
		// some other exception occurred
		txtResult->Text = ex->Message;
	}
}