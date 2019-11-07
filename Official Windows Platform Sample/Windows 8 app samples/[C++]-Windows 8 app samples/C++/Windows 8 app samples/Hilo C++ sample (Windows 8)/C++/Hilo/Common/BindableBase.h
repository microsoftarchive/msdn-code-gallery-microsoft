// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    namespace Common
    {
        /// <summary>
        /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class BindableBase : Windows::UI::Xaml::DependencyObject, Windows::UI::Xaml::Data::INotifyPropertyChanged, Windows::UI::Xaml::Data::ICustomPropertyProvider
        {
        public:
            virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        public:
            // ICustomPropertyProvider
            virtual Windows::UI::Xaml::Data::ICustomProperty^ GetCustomProperty(Platform::String^ name);
            virtual Windows::UI::Xaml::Data::ICustomProperty^ GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type);
            virtual Platform::String^ GetStringRepresentation();

            property Windows::UI::Xaml::Interop::TypeName Type
            {
                virtual Windows::UI::Xaml::Interop::TypeName get() { return this->GetType(); }
            }


        protected:
            virtual void OnPropertyChanged(Platform::String^ propertyName);
        };
    }
}
