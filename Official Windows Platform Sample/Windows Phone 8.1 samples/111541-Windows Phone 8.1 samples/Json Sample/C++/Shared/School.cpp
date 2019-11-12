//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "School.h"

using namespace Platform;
using namespace SDKSample::Json;
using namespace Windows::Data::Json;

String^ School::idKey = "id";
String^ School::nameKey = "name";
String^ School::schoolKey = "school";
String^ School::typeKey = "type";

School::School(void) : id(""), name(""), type("")
{
}

School::School(JsonObject^ jsonObject)
{
    JsonObject^ schoolObject = jsonObject->GetNamedObject(schoolKey, nullptr);
    if (schoolObject != nullptr)
    {
        Id = schoolObject->GetNamedString(idKey, "");
        Name= schoolObject->GetNamedString(nameKey, "");
    }
    Type = jsonObject->GetNamedString(typeKey);
}

JsonObject^ School::ToJsonObject()
{
    JsonObject^ schoolObject = ref new JsonObject();
    schoolObject->SetNamedValue(idKey, JsonValue::CreateStringValue(Id));
    schoolObject->SetNamedValue(nameKey, JsonValue::CreateStringValue(Name));

    JsonObject^ jsonObject = ref new JsonObject();
    jsonObject->SetNamedValue(schoolKey, schoolObject);
    jsonObject->SetNamedValue(typeKey, JsonValue::CreateStringValue(Type));

    return jsonObject;
}

String^ School::Id::get()
{
    return id;
}

void School::Id::set(String^ value)
{
    id = value;
}

String^ School::Name::get()
{
    return name;
}

void School::Name::set(String^ value)
{
    name = value;
}

String^ School::Type::get()
{
    return type;
}

void School::Type::set(String^ value)
{
    type = value;
}
