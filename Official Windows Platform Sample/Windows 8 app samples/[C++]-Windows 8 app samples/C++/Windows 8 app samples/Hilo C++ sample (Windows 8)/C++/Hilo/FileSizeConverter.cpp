// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "FileSizeConverter.h"
#include "LocalResourceLoader.h"

using namespace Hilo;
using namespace Platform;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::ApplicationModel::Resources;

// See http://go.microsoft.com/fwlink/?LinkId=267279 for info about the FileSizeConverter class
// and other data converters.

FileSizeConverter::FileSizeConverter()
{
    m_loader = ref new LocalResourceLoader();
}

FileSizeConverter::FileSizeConverter(Hilo::IResourceLoader^ loader) : m_loader(loader)
{
}

Object^ FileSizeConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^)
{
    float64 size = static_cast<float64>(safe_cast<uint64>(value));
    std::array<String^, 3> units = 
    { 
        m_loader->GetString("BytesUnit"), 
        m_loader->GetString("KilobytesUnit"), 
        m_loader->GetString("MegabytesUnit") 
    };
    unsigned int index = 0;

    while (size >= 1024)
    {
        size /= 1024;
        index++;
    }

    return ToTwoDecimalPlaces(size) + " " + units[index];
}

float64 FileSizeConverter::ToTwoDecimalPlaces(float64 value)
{
    float64 f;
    float64 intpart;
    float64 fractpart;
    fractpart = modf(value, &intpart);
    f = floor(fractpart * 100 + 0.5) / 100.0;
    return intpart + f;
}

Object^ FileSizeConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^)
{
    throw ref new NotImplementedException();
}
