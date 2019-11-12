//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_ParseAndStringify.xaml.cpp
// Implementation of the S1_ParseAndStringify class
//

#include "pch.h"
#include <assert.h>
#include "Scenario1_ParseAndStringify.xaml.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace SDKSample;
using namespace SDKSample::Json;
using namespace Windows::Data::Json;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

S1_ParseAndStringify::S1_ParseAndStringify()
{
    InitializeComponent();

    JsonInput->Text = "{\r\n" +
        "  \"id\": \"1146217767\",\r\n" +
        "  \"name\": \"Julie Larson-Green\",\r\n" +
        "  \"education\": [\r\n" +
        "    {\r\n" +
        "      \"school\": {\r\n" +
        "        \"id\": \"204165836287254\",\r\n" +
        "        \"name\": \"Contoso High School\"\r\n" +
        "      },\r\n" +
        "      \"type\": \"High School\"\r\n" +
        "    },\r\n" +
        "    {\r\n" +
        "      \"school\": {\r\n" +
        "        \"id\": \"116138758396662\",\r\n" +
        "        \"name\": \"Contoso University\"\r\n" +
        "      },\r\n" +
        "      \"type\": \"College\"\r\n" +
        "    }\r\n" +
        "  ],\r\n" +
        "  \"timezone\": -8,\r\n" +
        "  \"verified\": true\r\n" +
        "}";

    MainPage::Current->DataContext = ref new User();
}

void Json::S1_ParseAndStringify::Parse_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        MainPage::Current->DataContext = ref new User(JsonInput->Text);
        MainPage::Current->NotifyUser("JSON string parsed successfully.", NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        if (!IsExceptionHandled(ex))
        {
            throw ex;
        }
    }
}

void Json::S1_ParseAndStringify::Stringify_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    JsonInput->Text = "";

    assert(MainPage::Current->DataContext != nullptr);
    User^ user = dynamic_cast<User^>(MainPage::Current->DataContext);

    JsonInput->Text = user->Stringify();
    MainPage::Current->NotifyUser("JSON object serialized to string successfully.", NotifyType::StatusMessage);
}

void Json::S1_ParseAndStringify::Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    assert(MainPage::Current->DataContext != nullptr);
    User^ user = dynamic_cast<User^>(MainPage::Current->DataContext);

    user->Education->Append(ref new School());
    MainPage::Current->NotifyUser("New row added.", NotifyType::StatusMessage);
}

bool Json::S1_ParseAndStringify::IsExceptionHandled(Platform::Exception^ ex)
{
    JsonErrorStatus error = JsonError::GetJsonStatus(ex->HResult);
    if (error == JsonErrorStatus::Unknown)
    {
        return false;
    }

    MainPage::Current->NotifyUser(ex->Message, NotifyType::ErrorMessage);
    return true;
}
