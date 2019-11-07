// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// S2Formatter.h
// Declaration of the S2Formatter class
//

#pragma once

#include "pch.h"

using namespace Platform;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;


namespace SDKSample
{
    namespace DataBinding
    {
		// This value converter is used in Scenario 2. For more information on Value Converters, see http://go.microsoft.com/fwlink/?LinkId=254639#data_conversions
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class S2Formatter sealed : Windows::UI::Xaml::Data::IValueConverter
    	{
    	public:
    		S2Formatter();
    		virtual Object^ Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language);
    		virtual Object^ ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language);
    	private:
    		~S2Formatter();
    	};
    }
}
