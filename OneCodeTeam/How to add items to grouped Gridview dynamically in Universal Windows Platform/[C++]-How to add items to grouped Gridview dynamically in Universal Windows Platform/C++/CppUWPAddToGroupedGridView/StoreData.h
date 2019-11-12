#pragma once
#include "pch.h"
#include "Item.h"
#include "GroupInfoCollection.h"

using namespace CppUWPAddToGroupedGridView::SampleData;
using namespace Windows::Foundation::Collections;
using namespace Platform::Collections;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Media;

namespace CppUWPAddToGroupedGridView
{
	namespace SampleData
	{
		[Windows::UI::Xaml::Data::Bindable]
		public ref class StoreData sealed
		{
		public:
		StoreData();
		property Windows::Foundation::Collections::IObservableVector<Item^>^ Collection
		{
			Windows::Foundation::Collections::IObservableVector<Item^>^ get();
			void set(Windows::Foundation::Collections::IObservableVector<Item^>^ value);
		}
		internal:
			Platform::Collections::Vector<GroupInfoCollection^>^ GetGroupsByCategory();

		private:
			Platform::Collections::Vector<Item^>^ _collection;
		};
	}
}