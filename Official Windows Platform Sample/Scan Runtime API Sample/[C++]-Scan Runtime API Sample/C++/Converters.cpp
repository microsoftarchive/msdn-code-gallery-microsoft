//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "Converters.h"

using namespace SDKSample::Common;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;

Object^ UIntToVisibilityConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    return (safe_cast<unsigned int>(value) > 0 ? Visibility::Visible : Visibility::Collapsed);
}

Object^ UIntToVisibilityConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    throw ref new NotImplementedException("UIntToVisibilityConverter::ConvertBack not implemented");
}

Object^ OutputHeightConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    // Return 85% of the input value
    return (safe_cast<double>(value) * 0.85);
}

Object^ OutputHeightConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    throw ref new NotImplementedException("OutputHeightConverter::ConvertBack not implemented");
}
