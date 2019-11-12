// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ThumbnailConverter.h
// Declaration of the ThumbnailConverter class
//

#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace DataSourceAdapter
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class ThumbnailConverter sealed : Windows::UI::Xaml::Data::IValueConverter
        {
        public:
            ThumbnailConverter();
    
            virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
            virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
    
    	private:
            ~ThumbnailConverter();
        };
    }
}
