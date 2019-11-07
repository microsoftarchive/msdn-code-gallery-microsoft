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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    ShowPropertiesButton->Click += ref new RoutedEventHandler(this, &Scenario5::ShowPropertiesButton_Click);
}

DateTimeFormatter^ Scenario5::dateFormat = ref new DateTimeFormatter("shortdate");
DateTimeFormatter^ Scenario5::timeFormat = ref new DateTimeFormatter("longtime");
String^ Scenario5::dateAccessedProperty = "System.DateAccessed";
String^ Scenario5::fileOwnerProperty    = "System.FileOwner";

void Scenario5::ShowPropertiesButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        // Get top level file properties
        OutputTextBlock->Text = "File name: " + file->Name;
        OutputTextBlock->Text += "\nFile type: " + file->FileType;

        // Get basic properties
        create_task(file->GetBasicPropertiesAsync()).then([this, file](task<BasicProperties^> task)
        {
            try
            {
                BasicProperties^ basicProperties = task.get();
                String^ dateModifiedString = dateFormat->Format(basicProperties->DateModified) + " " + timeFormat->Format(basicProperties->DateModified);
                OutputTextBlock->Text += "\nFile size: " + basicProperties->Size.ToString() + " bytes" + "\nDate modified: " + dateModifiedString;
            }
            catch(COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        }).then([this, file]()
        {
            // Get extra properties
            auto propertiesName = ref new Vector<String^>();
            propertiesName->Append(dateAccessedProperty);
            propertiesName->Append(fileOwnerProperty);
            return file->Properties->RetrievePropertiesAsync(propertiesName);
        }).then([this](IMap<String^, Object^>^ extraProperties)
        {
            auto propValue = extraProperties->Lookup(dateAccessedProperty);
            if (propValue != nullptr)
            {
                DateTime dateAccessed = (DateTime)propValue;
                OutputTextBlock->Text += "\nDate accessed: " + dateFormat->Format(dateAccessed) + " " + timeFormat->Format(dateAccessed);
            }
            propValue = extraProperties->Lookup(fileOwnerProperty);
            if (propValue != nullptr)
            {
                OutputTextBlock->Text += "\nFile owner: " + propValue;
            }
        });
    }
}
