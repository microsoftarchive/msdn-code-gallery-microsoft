#pragma once

#include "pch.h"
namespace CppUWPAddToGroupedGridView
{
	namespace SampleData
	{

		[Windows::UI::Xaml::Data::Bindable]
		public ref class Item sealed
			: public Windows::UI::Xaml::Data::INotifyPropertyChanged
		{
		public:
			Item();
			Platform::String^ ToString() override;
			
			property Platform::String^ Title 
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			property Platform::String^ ImageUrl
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			property Platform::String^ Category
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}
		internal:
			Item(Platform::String^ title, Platform::String^ category);

		private:
			Platform::String^ _title;
			Platform::String^ _category;
			Platform::String^ _imageUrl;

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

			// In c++, it is not necessary to include definitions of add, remove, and raise.
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