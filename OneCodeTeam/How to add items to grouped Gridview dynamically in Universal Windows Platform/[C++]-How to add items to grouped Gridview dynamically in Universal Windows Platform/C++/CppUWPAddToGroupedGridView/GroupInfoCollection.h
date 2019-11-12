#pragma once

#include "pch.h"
#include "Item.h"
using namespace CppUWPAddToGroupedGridView::SampleData;
using namespace Windows::Foundation::Collections;
using namespace Platform::Collections;
using namespace  Windows::Foundation::Collections;

namespace CppUWPAddToGroupedGridView
{
	namespace SampleData
	{
		[Windows::UI::Xaml::Data::Bindable]
	    public ref class GroupInfoCollection sealed
		{
		public:
			GroupInfoCollection();
			GroupInfoCollection(Platform::String^ key, Item^ item);
			property Platform::String^ Key
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			property Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ get();
				void set(Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ value);
			}

		private:
			Platform::Collections::Vector<Platform::Object^>^ _storage;
			Platform::String^ _key;
		};
	}
}