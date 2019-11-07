#pragma once
#include <string>
#include <sstream>
#include <algorithm>
#include <collection.h>
#include <ppltasks.h>

namespace SDKSample
{
	namespace ExpressionBlendSampleDataSampleDataSource
	{
		using namespace Platform;
		using namespace Windows::UI::Xaml::Data;
		using namespace Windows::UI::Xaml::Media;
		using namespace Windows::UI::Xaml::Media::Imaging;
		using namespace Windows::Foundation::Collections;

		[Windows::UI::Xaml::Data::Bindable]
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Item sealed 
		{
		public:
			Item();

		private:
			Platform::String^  _Title;
			Platform::String^  _Subtitle;
			Platform::String^  _Link;
			Platform::String^  _Category;
			Platform::String^ _Description;
			Platform::String^ _Content;
			ImageSource^ _Image;
			event PropertyChangedEventHandler^ _PropertyChanged; 

		public:
			//void OnApplyTemplate();
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
					return _Title;
				}
				void set(Platform::String^ value)
				{
					_Title = value;
					OnPropertyChanged("Title");
				}
			}

			//Subtitle
			property Platform::String^ Subtitle
			{
				Platform::String^ get()
				{
					return _Subtitle;
				}
				void set(Platform::String^ value)
				{
					_Subtitle = value;
					OnPropertyChanged("Subtitle");
				}
			}

			//Link
			property Platform::String^ Link
			{
				Platform::String^ get()
				{
					return _Link;
				}
				void set(Platform::String^ value)
				{
					_Link = value;
					OnPropertyChanged("Link");
				}
			}

		
			//Category
			property Platform::String^ Category
			{
				Platform::String^ get()
				{
					return _Category;
				}
				void set(Platform::String^ value)
				{
					_Category = value;
					OnPropertyChanged("Category");
				}
			}

			//Description
			property Platform::String^ Description
			{
				Platform::String^ get()
				{
					return _Description;
				}
				void set(Platform::String^ value)
				{
					_Description = value;
					OnPropertyChanged("Description");
				}
			}

			//Content
			property Platform::String^ Content
			{
				Platform::String^ get()
				{
					return _Content;
				}
				void set(Platform::String^ value)
				{
					_Content = value;
					OnPropertyChanged("Content");
				}
			}

			//Image
			property ImageSource^ Image
			{
				ImageSource^ get()
				{
					return _Image;
				}
				void set(ImageSource^ value)
				{
					_Image = value;
					OnPropertyChanged("Image");
				}
			}

			void SetImage(Windows::Foundation::Uri^ baseUri, Platform::String^ path) //SetImage() in C# code
			{
				Windows::Foundation::Uri^ uri = ref new Windows::Foundation::Uri("ms-appx:///" + path);		
				Image = ref new BitmapImage(uri);
			}

		};

	
		// Workaround: data binding works best with an enumeration of objects that does not implement IList
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class ItemCollection sealed
		{
		private:
			Platform::Collections::Vector<Item^>^ itemCollection;

		public:
			ItemCollection()
			{
					itemCollection = ref new Platform::Collections::Vector<Item^>();
			};

			void Add(Item^ item)
			{
				itemCollection->Append(item);
			}    
		};

		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class MessageData sealed 
		{
			//ItemCollection^ _Collection;
			Windows::UI::Xaml::Interop::IBindableObservableVector^  _items;

		public:
			MessageData()
			{
				_items = ref new Platform::Collections::Vector<Item^>();
				Windows::Foundation::Uri^ _baseUri = ref new Windows::Foundation::Uri("ms-appx:///");
			
				Item^ item;
				item = ref new Item();
				item->Title="New Flavors out this week!";
				item->Subtitle="Adam Barr";
				item->SetImage(_baseUri, "SampleData/Images/image1.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum";
				_items->Append(item);

				item = ref new Item();
				item->Title="Check out this topping!";
				item->Subtitle="David Alexander";
				item->SetImage(_baseUri, "SampleData/Images/image2.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue";
				_items->Append(item);

				item = ref new Item();
				item->Title="Come to the Ice Cream Party";
				item->Subtitle="Josh Bailey";
				item->SetImage(_baseUri, "SampleData/Images/image3.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse";
				_items->Append(item);

				item = ref new Item();
				item->Title="How about gluten free?";
				item->Subtitle="Chris Berry";
				item->SetImage(_baseUri, "SampleData/Images/image4.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
				_items->Append(item);

				item = ref new Item();
				item->Title= "Summer promotion - BYGO";
				item->Subtitle="Sean Bentley";
				item->SetImage(_baseUri, "SampleData/Images/image5.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat";
				_items->Append(item);

				item = ref new Item();
				item->Title="Awesome flavor combination";
				item->Subtitle="Adrian Lannin";
				item->SetImage(_baseUri, "SampleData/Images/image6.jpg");
				item->Content="Curabitur class aliquam vestibulum nam curae maecenas sed integer";
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
