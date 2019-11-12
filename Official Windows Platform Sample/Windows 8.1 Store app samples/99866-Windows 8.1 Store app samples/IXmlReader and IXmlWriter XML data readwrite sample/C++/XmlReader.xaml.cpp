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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "XmlReader.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace SDKSample::XmlLiteSample;

Scenario1::Scenario1()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// A pointer back to the main page.  This is needed if you want to call methods in MainPage such
	// as NotifyUser()
	rootPage = MainPage::Current;
}

void SDKSample::XmlLiteSample::Scenario1::ReadXmlClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = safe_cast<Button^>(sender);
	if (b != nullptr)
	{
		StorageFolder^ installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
		task<StorageFile^> getFileTask(installFolder->GetFileAsync("Stocks.xml"));
		getFileTask.then([this](StorageFile^ file)
		{
			task<IRandomAccessStream^> openTask(file->OpenAsync(FileAccessMode::Read));
			openTask.then([this](IRandomAccessStream^ readStream)
			{
				HRESULT hr = ReadXml(readStream);
				if (FAILED(hr))
				{
					throw Exception::CreateException(hr);
				}
			});
		});
	}
}

HRESULT SDKSample::XmlLiteSample::Scenario1::ReadXml(IRandomAccessStream ^pStream)
{
	HRESULT hr;
	ComPtr<IStream> spStream;
	ComPtr<IXmlReader> spReader;
	XmlNodeType nodeType;
	LPCWSTR pwszPrefix = nullptr;
	LPCWSTR pwszLocalName = nullptr;
	LPCWSTR pwszValue = nullptr;
	UINT cwchPrefix = 0, cwchLocalName = 0, cwchValue = 0;

	ChkHr(::CreateStreamOverRandomAccessStream(pStream, IID_PPV_ARGS(&spStream)));
	ChkHr(::CreateXmlReader(IID_PPV_ARGS(&spReader), nullptr));
	ChkHr(spReader->SetProperty(XmlReaderProperty_DtdProcessing, DtdProcessing_Prohibit));
	ChkHr(spReader->SetInput(spStream.Get()));

	// Reads until there are no more nodes.
	while (S_OK == (hr = spReader->Read(&nodeType)))
	{
		switch (nodeType)
		{
		case XmlNodeType_XmlDeclaration:
			XmlTextbox->Text += L"XmlDeclaration\n";
			ChkHr(ReadAttributes(spReader.Get()));
			break;

		case XmlNodeType_Element:
			ChkHr(spReader->GetPrefix(&pwszPrefix, &cwchPrefix));
			ChkHr(spReader->GetLocalName(&pwszLocalName, &cwchLocalName));
			if (cwchPrefix > 0)
			{
				XmlTextbox->Text += L"Element: ";
				XmlTextbox->Text += ref new String(pwszPrefix, cwchPrefix);
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"\n";
			}
			else
			{
				XmlTextbox->Text += L"Element: ";
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"\n";
			}
			ChkHr(ReadAttributes(spReader.Get()));

			if (spReader->IsEmptyElement())
			{
				XmlTextbox->Text += L" (empty)";
			}
			break;

		case XmlNodeType_EndElement:
			ChkHr(spReader->GetPrefix(&pwszPrefix, &cwchPrefix));
			ChkHr(spReader->GetLocalName(&pwszLocalName, &cwchLocalName));
			if (cwchPrefix > 0)
			{
				XmlTextbox->Text += L"End Element: ";
				XmlTextbox->Text += ref new String(pwszPrefix, cwchPrefix);
				XmlTextbox->Text += L":";
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"\n";
			}
			else
			{
				XmlTextbox->Text += L"End Element: ";
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"\n";
			}
			break;
		case XmlNodeType_Text:
		case XmlNodeType_Whitespace:
			ChkHr(spReader->GetValue(&pwszValue, &cwchValue));
			XmlTextbox->Text += L"Text: >";
			XmlTextbox->Text += ref new String(pwszValue, cwchValue);
			XmlTextbox->Text += L"<\n";
			break;
		case XmlNodeType_CDATA:
			ChkHr(spReader->GetValue(&pwszValue, &cwchValue));
			XmlTextbox->Text += L"CDATA: ";
			XmlTextbox->Text += ref new String(pwszValue, cwchValue);
			XmlTextbox->Text += L"\n";
			break;
		case XmlNodeType_ProcessingInstruction:
			ChkHr(spReader->GetLocalName(&pwszLocalName, &cwchLocalName));
			ChkHr(spReader->GetValue(&pwszValue, &cwchValue));
			XmlTextbox->Text += L"Processing Instruction name:";
			XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
			XmlTextbox->Text += L"value:";
			XmlTextbox->Text += ref new String(pwszValue, cwchValue);
			XmlTextbox->Text += L"\n";
			break;
		case XmlNodeType_Comment:
			ChkHr(spReader->GetValue(&pwszValue, &cwchValue));
			XmlTextbox->Text += L"Comment: ";
			XmlTextbox->Text += ref new String(pwszValue, cwchValue);
			XmlTextbox->Text += L"\n";
			break;
		case XmlNodeType_DocumentType:
			XmlTextbox->Text += L"DOCTYPE is not printed\n";
			break;
		}
	}

	return hr;
}

HRESULT SDKSample::XmlLiteSample::Scenario1::ReadAttributes(IXmlReader *pReader)
{
	HRESULT hr = S_OK;
	LPCWSTR pwszPrefix = nullptr;
	LPCWSTR pwszLocalName = nullptr;
	LPCWSTR pwszValue = nullptr;
	UINT cwchPrefix = 0, cwchLocalName = 0, cwchValue = 0;

	ChkHr(pReader->MoveToFirstAttribute());
	if (S_FALSE == hr)
		return hr;

	while (true)
	{
		if (!pReader->IsDefault())
		{
			ChkHr(pReader->GetPrefix(&pwszPrefix, &cwchPrefix));
			ChkHr(pReader->GetLocalName(&pwszLocalName, &cwchLocalName));
			ChkHr(pReader->GetValue(&pwszValue, &cwchValue));

			if (cwchPrefix > 0)
			{
				XmlTextbox->Text += L"Attr: ";
				XmlTextbox->Text += ref new String(pwszPrefix, cwchPrefix);
				XmlTextbox->Text += L":";
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"=\"";
				XmlTextbox->Text += ref new String(pwszValue, cwchValue);
				XmlTextbox->Text += L"\"\n";
			}
			else
			{
				XmlTextbox->Text += L"Attr: ";
				XmlTextbox->Text += ref new String(pwszLocalName, cwchLocalName);
				XmlTextbox->Text += L"=\"";
				XmlTextbox->Text += ref new String(pwszValue, cwchValue);
				XmlTextbox->Text += L"\"\n";
			}
		}

		if (S_OK != (hr = pReader->MoveToNextAttribute()))
			break;
	}

	return hr;
}
