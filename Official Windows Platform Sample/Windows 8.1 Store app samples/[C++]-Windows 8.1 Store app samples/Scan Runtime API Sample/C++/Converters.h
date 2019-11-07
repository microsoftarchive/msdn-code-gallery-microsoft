//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
#pragma once
namespace SDKSample
{
    namespace Common
    {
        /// <summary>
        /// Value converter that translates non-zero uint to <see cref="Visibility::Visible"/> and 0
        /// to <see cref="Visibility::Collapsed"/>.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class UIntToVisibilityConverter sealed : Windows::UI::Xaml::Data::IValueConverter
        {
        public:
            virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
            virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
        };

        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class OutputHeightConverter sealed : Windows::UI::Xaml::Data::IValueConverter
        {
        public:
            virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
            virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
        };
    }
}