//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

using namespace Windows::ApplicationModel::Resources::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace SDKSample
{
	namespace WebViewControl
	{
		[Windows::UI::Xaml::Data::Bindable]
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class BookmarkItem sealed
		{
			Windows::Foundation::Uri^ _pageUrl;
			Windows::UI::Xaml::Media::Imaging::BitmapSource^ _preview;
			Platform::String^ _title;

			event PropertyChangedEventHandler^ _PropertyChanged;

		public:
			BookmarkItem(Platform::String^ title, Windows::UI::Xaml::Media::Imaging::BitmapSource^ preview, Windows::Foundation::Uri^ url);

			void OnPropertyChanged(Platform::String^ propertyName)
			{
				PropertyChangedEventArgs^ pcea = ref new  PropertyChangedEventArgs(propertyName);
				_PropertyChanged(this, pcea);
			}


			//Title
			property Platform::String^ Title
			{
				Platform::String^ get()
				{
					return _title;
				}
				void set(Platform::String^ value)
				{
					_title = value;
					OnPropertyChanged("Title");
				}
			}

			//Type
			property Windows::UI::Xaml::Media::Imaging::BitmapSource^ Preview
			{
				Windows::UI::Xaml::Media::Imaging::BitmapSource^ get()
				{
					return _preview;
				}
				void set(Windows::UI::Xaml::Media::Imaging::BitmapSource^ value)
				{
					_preview = value;
					OnPropertyChanged("Preview");
				}
			}

			//Picture, _picture is the path to Image
			property Windows::Foundation::Uri^ PageUrl
			{
				Windows::Foundation::Uri^ get()
				{
					return _pageUrl;
				}
				void set(Windows::Foundation::Uri^ value)
				{
					_pageUrl = value;
					OnPropertyChanged("PageUrl");
				}
			}
		};

		// Workaround: data binding works best with an enumeration of objects that does not implement IList
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class BookmarkCollection sealed
		{
		private:
			Platform::Collections::Vector<BookmarkItem^>^ itemCollection;
			unsigned int count;

		public:
			BookmarkCollection()
			{
				count = 0;
				itemCollection = ref new Platform::Collections::Vector<BookmarkItem^>();
			};

			void Add(BookmarkItem^ item)
			{
				itemCollection->Append(item);
				count++;
			}

			BookmarkItem^ GetItemAt(uint32 i)
			{
				return itemCollection->GetAt(i);
			}

			unsigned int GetCollectionCount()
			{
				return count;
			}

			Windows::UI::Xaml::Interop::IBindableObservableVector^ items()
			{
				return itemCollection;
			}
		};

		
	}
}
