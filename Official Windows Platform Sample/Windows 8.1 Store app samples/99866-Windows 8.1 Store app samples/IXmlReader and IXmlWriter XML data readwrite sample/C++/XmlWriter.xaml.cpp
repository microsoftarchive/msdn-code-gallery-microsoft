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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "XmlWriter.xaml.h"

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

Scenario2::Scenario2()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
	// A pointer back to the main page.  This is needed if you want to call methods in MainPage such
	// as NotifyUser()
	rootPage = MainPage::Current;
}


void SDKSample::XmlLiteSample::Scenario2::DoSomething_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = safe_cast<Button^>(sender);
	if (b != nullptr)
	{
        StorageFolder^ folder = Windows::Storage::ApplicationData::Current->LocalFolder;
		task<StorageFile^> createFileTask (folder->CreateFileAsync(L"Stocks.xml", CreationCollisionOption::OpenIfExists));
		createFileTask.then([this](StorageFile^ file)
		{
			task<IRandomAccessStream^> openTask(file->OpenAsync(FileAccessMode::ReadWrite));
			openTask.then([=](IRandomAccessStream^ writeStream)
			{
				HRESULT hr = WriteXml(writeStream);
				if (FAILED(hr))
				{
					throw Exception::CreateException(hr);
				}

                OutputTextBlock1->Text = L"File is written to " + file->Path + L" successfully";
			});

		});
	}
}

HRESULT SDKSample::XmlLiteSample::Scenario2::WriteXml(IRandomAccessStream ^pStream)
{
	HRESULT hr;
	ComPtr<IStream> spStream;
	ComPtr<IXmlWriter> spWriter;
	LPCWSTR pwszPrefix = nullptr;
	LPCWSTR pwszLocalName = nullptr;

	ChkHr(::CreateStreamOverRandomAccessStream(pStream, IID_PPV_ARGS(&spStream)));
	ChkHr(::CreateXmlWriter(IID_PPV_ARGS(&spWriter), nullptr));
	ChkHr(spWriter->SetOutput(spStream.Get()));
	ChkHr(spWriter->SetProperty(XmlWriterProperty_Indent, TRUE));
	ChkHr(spWriter->WriteStartDocument(XmlStandalone_Omit));
	ChkHr(spWriter->WriteDocType(L"root", nullptr, nullptr, nullptr));
	ChkHr(spWriter->WriteProcessingInstruction(L"xml-stylesheet",
		L"href=\"mystyle.css\" title=\"Compact\" type=\"text/css\""));
	ChkHr(spWriter->WriteStartElement(nullptr, L"root", nullptr));
	ChkHr(spWriter->WriteStartElement(nullptr, L"sub", nullptr));
	ChkHr(spWriter->WriteAttributeString(nullptr, L"myAttr", nullptr, L"1234"));
	ChkHr(spWriter->WriteString(L"Markup is <escaped> for this string"));
	ChkHr(spWriter->WriteFullEndElement());
	ChkHr(spWriter->WriteStartElement(nullptr, L"anotherChild", nullptr));
	ChkHr(spWriter->WriteString(L"some text"));
	ChkHr(spWriter->WriteFullEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteCData(L"This is CDATA text."));
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteStartElement(nullptr, L"containsCharacterEntity", nullptr));
	ChkHr(spWriter->WriteCharEntity(L'a'));
	ChkHr(spWriter->WriteFullEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteStartElement(nullptr, L"containsChars", nullptr));
	ChkHr(spWriter->WriteChars(L"abcdefghijklm", 5));
	ChkHr(spWriter->WriteFullEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteStartElement(nullptr, L"containsName", nullptr));
	ChkHr(spWriter->WriteName(L"myName"));
	ChkHr(spWriter->WriteEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteStartElement(nullptr, L"containsNmToken", nullptr));
	ChkHr(spWriter->WriteNmToken(L"myNmToken"));
	ChkHr(spWriter->WriteEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteComment(L"This is a comment"));
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteRaw(L"<elementWrittenRaw/>"));
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteRawChars(L"<rawCharacters/>", 16));
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteElementString(nullptr, L"myElement", nullptr, L"myValue"));
	ChkHr(spWriter->WriteFullEndElement());
	ChkHr(spWriter->WriteWhitespace(L"\n"));
	ChkHr(spWriter->WriteEndDocument());
	ChkHr(spWriter->Flush());

	return hr;
}
