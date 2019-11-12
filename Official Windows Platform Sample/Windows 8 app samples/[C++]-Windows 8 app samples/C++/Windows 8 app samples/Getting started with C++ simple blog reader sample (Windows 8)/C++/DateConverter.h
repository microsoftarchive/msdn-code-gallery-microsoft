// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


//dateconverter.h

#pragma once

namespace SimpleBlogReader
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DateConverter sealed : public Windows::UI::Xaml::Data::IValueConverter  
    {
    public:
        virtual Platform::Object^ Convert(Platform::Object^ value,
            Windows::UI::Xaml::Interop::TypeName targetType,
            Platform::Object^ parameter,
            Platform::String^ language)
        {		
            if (value == nullptr)
            {
                throw ref new Platform::InvalidArgumentException();
            }
            Windows::Foundation::DateTime dt = (Windows::Foundation::DateTime) value;
            Platform::String^ param = safe_cast<Platform::String^>(parameter);
            Platform::String^ result;

            // This is called by the items page which requires simple formatting.
            if (param == nullptr)
            {
                Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ dtf =
                        Windows::Globalization::DateTimeFormatting::DateTimeFormatter::ShortDate::get();
                result = dtf->Format(dt);
            }

            // These are called by the split page to format each date element separately:
            else if (wcscmp(param->Data(), L"month") == 0)
            {
                Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ month = 
                    ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("{month.abbreviated(3)}");
                result = month->Format(dt);
            }
            else if (wcscmp(param->Data(), L"day") == 0)
            {
                Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ month = 
                    ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("{day.integer(2)}");
                result = month->Format(dt);
            }
            else if (wcscmp(param->Data(), L"year") == 0)
            {
                Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ month = 
                    ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("{year.full}");
                result = month->Format(dt);
            }
            else
            {
                // We don't handle other format types currently.
                throw ref new Platform::InvalidArgumentException();
            }

            return result; 
        }

        virtual Platform::Object^ ConvertBack(Platform::Object^ value,
            Windows::UI::Xaml::Interop::TypeName targetType,
            Platform::Object^ parameter,
            Platform::String^ language)
        {   
            // Not needed in SimpleBlogReader. Left as an exercise.
            throw ref new Platform::NotImplementedException();
        }
    };
}