// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Employee.h
// Declaration of the Employee class
//

#pragma once

namespace SDKSample
{
    namespace DataBinding
    {	
        [Windows::Foundation::Metadata::WebHostHidden]
        [Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
        public ref class Employee sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
        {
        public:
            property Platform::String^ Name
            {
                Platform::String^ get()
                { 
                    return _name;
                }
                void set(Platform::String^ value)
                {
					if (!_name->Equals(value))
					{
						_name = value;
						Employee::OnPropertyChanged("Name");
					}
                }
            }

            property Platform::String^ Organization
            {
                Platform::String^ get()
                {
                    return _organization;
                }
                void set(Platform::String^ value)
                {
					if (!_organization->Equals(value))
					{
					_organization = value;
                    Employee::OnPropertyChanged("Organization");
					}
                 }
            }
    
			property Platform::IBox<int>^ Age
			{
				Platform::IBox<int>^ get()
				{
					return _age;
				}
				void set(Platform::IBox<int>^ value)
				{
					if (_age != value)
					{
						_age = value;
						Employee::OnPropertyChanged("Age");
					}
				}
			}

        private:
            Platform::String^ _name;
            Platform::String^ _organization;
			//In C++/CX, you can use the Platform::IBox<T> type to expose public methods that support nullable value type parameters. 
			Platform::IBox<int>^ _age;

#pragma region INotifyPropertyChanged
        private:
            bool _isPropertyChangedObserved;
            event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ _privatePropertyChanged;

        protected:
            /// <summary>
            /// Notifies listeners that a property value has changed.
            /// </summary>
            /// <param name="propertyName">Name of the property used to notify listeners.</param>
            void OnPropertyChanged(Platform::String^ propertyName)
            {
                if (_isPropertyChangedObserved)
                {
					PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
                }
            }

        public:
            Employee()
            {
                _isPropertyChangedObserved = false;
            }

            // in c++, it is not necessary to include definitions of add, remove, and raise.
            //  these definitions have been made explicitly here so that we can check if the 
            //  event has listeners before firing the event
            virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged
            {
                virtual Windows::Foundation::EventRegistrationToken add(Windows::UI::Xaml::Data::PropertyChangedEventHandler^ e)
                {
                    _isPropertyChangedObserved = true;
                    return _privatePropertyChanged += e;
                }
                virtual void remove(Windows::Foundation::EventRegistrationToken t)
                {
                    _privatePropertyChanged -= t;
                }

        protected:
                virtual void raise(Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e)
                {
                    if (_isPropertyChangedObserved)
                    {
                        _privatePropertyChanged(sender, e);
                    }
                }
            }
#pragma endregion

        };
    }
}
