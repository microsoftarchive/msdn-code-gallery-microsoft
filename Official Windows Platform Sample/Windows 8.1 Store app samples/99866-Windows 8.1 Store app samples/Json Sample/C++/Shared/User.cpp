//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "User.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace SDKSample::Json;
using namespace Windows::Data::Json;
using namespace Windows::Foundation::Collections;

String^ User::idKey = "id";
String^ User::nameKey = "name";
String^ User::educationKey = "education";
String^ User::timezoneKey = "timezone";
String^ User::verifiedKey = "verified";

User::User(void) : id(""), name(""), timezone(0), verified(0)
{
    education = ref new Vector<School^>();
}

User::User(String^ jsonString)
{
    JsonObject^ jsonObject = JsonObject::Parse(jsonString);
    Id = jsonObject->GetNamedString(idKey, L"");
    Name = jsonObject->GetNamedString(nameKey, "");
    Timezone = jsonObject->GetNamedNumber(timezoneKey, 0);
    Verified = jsonObject->GetNamedBoolean(verifiedKey, false);

    education = ref new Vector<School^>();

    JsonArray^ jsonArray = jsonObject->GetNamedArray(educationKey, ref new JsonArray());
    for (unsigned int i = 0; i < jsonArray->Size; i++)
    {
        IJsonValue^ jsonValue = jsonArray->GetAt(i);
        if (jsonValue->ValueType == JsonValueType::Object)
        {
            education->Append(ref new School(jsonValue->GetObject()));
        }
    }
}

Platform::String^ User::Stringify()
{
    JsonArray^ jsonArray = ref new JsonArray();
    for (unsigned int i = 0; i < Education->Size; i++)
    {
        jsonArray->Append(Education->GetAt(i)->ToJsonObject());
    }

    JsonObject^ jsonObject = ref new JsonObject();
    jsonObject->SetNamedValue(idKey, JsonValue::CreateStringValue(Id));
    jsonObject->SetNamedValue(nameKey, JsonValue::CreateStringValue(Name));
    jsonObject->SetNamedValue(educationKey, jsonArray);
    jsonObject->SetNamedValue(timezoneKey, JsonValue::CreateNumberValue(Timezone));
    jsonObject->SetNamedValue(verifiedKey, JsonValue::CreateBooleanValue(Verified));

    return jsonObject->Stringify();
}

String^ User::Id::get()
{
    return id;
}

void User::Id::set(String^ value)
{
    id = value;
}

String^ User::Name::get()
{
    return name;
}

void User::Name::set(String^ value)
{
    name = value;
}

IVector<School^>^ User::Education::get()
{
    return education;
}

double User::Timezone::get()
{
    return timezone;
}

void User::Timezone::set(double value)
{
    timezone = value;
}

bool User::Verified::get()
{
    return verified;
}

void User::Verified::set(bool value)
{
    verified = value;
}
