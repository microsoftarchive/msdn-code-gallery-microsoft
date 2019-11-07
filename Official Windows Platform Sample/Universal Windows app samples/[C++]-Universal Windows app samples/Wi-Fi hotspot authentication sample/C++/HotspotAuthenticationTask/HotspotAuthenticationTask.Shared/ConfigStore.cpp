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
// ConfigStore.cpp
// Utility for retrieving configuration parameters
//

#include "pch.h"
#include "ConfigStore.h"

using namespace HotspotAuthenticationTask;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::Storage;

// For the sake of simplicity of the sample, the following authentication parameters are hard coded:
String^ ConfigStore::AuthenticationHost::get()
{
    return "login.contoso.com";
}

String^ ConfigStore::UserName::get()
{
    return "MyUserName";
}

String^ ConfigStore::Password::get()
{
    return "MyPassword";
}

String^ ConfigStore::ExtraParameters::get()
{
    return "";
}

bool ConfigStore::MarkAsManualConnect::get()
{
    return false;
}

bool ConfigStore::AuthenticateThroughBackgroundTask::get()
{
    IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
    if (set->HasKey("background"))
    {
        bool value = (safe_cast<IPropertyValue^>(set->Lookup("background")))->GetBoolean();
        return value;
    }
    return true; // default value
}

void ConfigStore::AuthenticateThroughBackgroundTask::set(bool value) {
    IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
    set->Insert("background", PropertyValue::CreateBoolean(value));
}

String^ ConfigStore::AuthenticationToken::get()
{
    IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
    if (set->HasKey("token"))
    {
        String^ value = (safe_cast<IPropertyValue^>(set->Lookup("token")))->GetString();
        return value;
    }
    return "";
}

void ConfigStore::AuthenticationToken::set(String^ value) {
    IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
    set->Insert("token", PropertyValue::CreateString(value));
}
