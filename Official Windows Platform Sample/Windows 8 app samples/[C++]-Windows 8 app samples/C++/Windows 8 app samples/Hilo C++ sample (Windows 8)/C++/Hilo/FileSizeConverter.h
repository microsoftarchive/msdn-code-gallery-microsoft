// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IResourceLoader;

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info about the FileSizeConverter class
    // and other data converters.

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class FileSizeConverter sealed : public Windows::UI::Xaml::Data::IValueConverter
    {
    public:
        FileSizeConverter();
        FileSizeConverter(IResourceLoader^ loader);

        virtual Object^ Convert(Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Object^ parameter, Platform::String^ language);
        virtual Object^ ConvertBack(Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Object^ parameter, Platform::String^ language);

    private:
        float64 ToTwoDecimalPlaces(float64 value);
        IResourceLoader^ m_loader;
    };
}
