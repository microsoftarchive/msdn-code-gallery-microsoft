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
    namespace FlipViewData // namespace Controls_FlipView.Data in C#
    {
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class SampleDataItem sealed
        {
            Platform::String^ _title;
            Platform::String^ _type;
            Platform::String^ _picture;
            ImageSource^ _image;
            static Uri _baseUri;

            event PropertyChangedEventHandler^ _PropertyChanged; 

        public:
            SampleDataItem(Platform::String^ title,Platform::String^ type,Platform::String^ picture);

            void OnPropertyChanged(Platform::String^ propertyName)
            {
                PropertyChangedEventArgs^ pcea = ref new  PropertyChangedEventArgs(propertyName);
                _PropertyChanged(this,pcea);
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
            property Platform::String^ Type
            {
                Platform::String^ get()
                {
                    return _type;
                }
                void set(Platform::String^ value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }

            //Picture, _picture is the path to Image
            property Platform::String^ Picture
            {
                Platform::String^ get()
                {
                    return _picture;
                }
                void set(Platform::String^ value)
                {
                    _picture = value;
                    OnPropertyChanged("Picture");
                }
            }

            //Image
            property ImageSource^ Image
            {
                ImageSource^ get()
                {
                    if(_image==nullptr && _picture!=nullptr)
                    {
                        Windows::Foundation::Uri^ uri = ref new Windows::Foundation::Uri("ms-appx:///" + _picture);				
                        _image = ref new BitmapImage(uri);
                    }
                    return _image;
                }
                void set(ImageSource^ value)
                {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
        };

        // Workaround: data binding works best with an enumeration of objects that does not implement IList
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class ItemCollection sealed
        {
        private:
            Windows::Foundation::Collections::IObservableVector<SampleDataItem^>^ itemCollection;
            UINT count;

        public:
            ItemCollection():count(0)
            {
                itemCollection = ref new Platform::Collections::Vector<SampleDataItem^>();
            };

            void Add(SampleDataItem^ item)
            {
                itemCollection->Append(item);
                count++;
            }    

            SampleDataItem^ GetItemAt(UINT i)
            {
                return itemCollection->GetAt(i);
            }

            UINT GetCollectionCount()
            {
                return count;
            }
        };

        /// <summary>
        /// Creates a collection of groups and items with hard-coded content.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class SampleDataSource sealed
        {
            Windows::UI::Xaml::Interop::IBindableObservableVector^ _items;

        public:
            SampleDataSource()
            {
                _items = ref new Platform::Collections::Vector<Object^>();

                SampleDataItem^ item;
                item = ref new SampleDataItem("Cliff", "item", "Assets/Cliff.jpg");
                _items->Append(item);

                item = ref new SampleDataItem("Grapes", "item", "Assets/Grapes.jpg");
                _items->Append(item);

                item = ref new SampleDataItem("Rainier", "item", "Assets/Rainier.jpg");
                _items->Append(item);

                item = ref new SampleDataItem("Sunset", "item", "Assets/Sunset.jpg");
                _items->Append(item);

                item = ref new SampleDataItem("Valley", "item", "Assets/Valley.jpg");
                _items->Append(item);
            }

            property Windows::UI::Xaml::Interop::IBindableObservableVector^ Items
            {
                Windows::UI::Xaml::Interop::IBindableObservableVector^ get()
                {
                    return _items;
                }

            }
        };	
    }
}
