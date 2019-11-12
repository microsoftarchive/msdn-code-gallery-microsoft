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
	namespace ListViewSampleDataSource
	{
		[Windows::UI::Xaml::Data::Bindable]
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class Item sealed
		{
			Platform::String^  _Title;
			Platform::String^  _Subtitle;
			Platform::String^  _Link;
			Platform::String^  _Category;
			Platform::String^ _Description;
			Platform::String^ _Content;
			ImageSource^ _Image;
			event PropertyChangedEventHandler^ _PropertyChanged; 


		public:
			Item();

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


		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class StoreData sealed 
		{
			Windows::UI::Xaml::Interop::IBindableObservableVector^  _items;

		public:
			StoreData()
			{
				_items = ref new Platform::Collections::Vector<Item^>();
				Windows::Foundation::Uri^ _baseUri = ref new Windows::Foundation::Uri("ms-appx:///");
				Platform::String^ LONG_LOREM_IPSUM = "";//"Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
			
				Item^ item;
				item = ref new Item();
				item->Title="Banana Blast Frozen Yogurt";
				item->Subtitle="Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet";
				item->SetImage(_baseUri, "SampleData/Images/60Banana.png");
				item->Link = "http://www.adatum.com/";
				item->Category = "Low-fat frozen yogurt";
				item->Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
				item->Content = LONG_LOREM_IPSUM;
				_items->Append(item);

				item = ref new Item();
				item->Title = "Lavish Lemon Ice";
				item->Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est";
				item->SetImage(_baseUri, "SampleData/Images/60Lemon.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Sorbet";
				item->Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci";
				item->Content = LONG_LOREM_IPSUM;
				_items->Append(item);


				item = ref new Item();
				item->Title = "Marvelous Mint";
				item->Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis";
				item->SetImage(_baseUri, "SampleData/Images/60Mint.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Gelato";
				item->Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum";
				item->Content = LONG_LOREM_IPSUM;
				_items->Append(item);

				item = ref new Item();
				item->Title = "Creamy Orange";
				item->Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis";
				item->SetImage(_baseUri, "SampleData/Images/60Orange.png");
				item->Link = "http://www.alpineskihouse.com/";
				item->Category = "Sorbet";
				item->Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Succulent Strawberry";
				item->Subtitle = "Senectus sem lacus erat sociosqu eros suscipit primis nibh nisi nisl gravida torquent";
				item->SetImage(_baseUri, "SampleData/Images/60Strawberry.png");
				item->Link = "http://www.baldwinmuseumofscience.com/";
				item->Category = "Sorbet";
				item->Description = "Est auctor inceptos congue interdum egestas scelerisque pellentesque fermentum ullamcorper cursus dictum lectus suspendisse condimentum libero vitae vestibulum lobortis ligula fringilla euismod class scelerisque feugiat habitasse diam litora adipiscing sollicitudin parturient hendrerit curae himenaeos imperdiet ullamcorper suspendisse nascetur hac gravida pharetra eget donec leo mus nec non malesuada vestibulum pellentesque elit penatibus vestibulum per condimentum porttitor sed adipiscing scelerisque ullamcorper etiam iaculis enim tincidunt erat parturient sem vestibulum eros";
				item->Content = LONG_LOREM_IPSUM;
				_items->Append(item);

				item = ref new Item();
				item->Title = "Very Vanilla";
				item->Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est";
				item->SetImage(_baseUri, "SampleData/Images/60Vanilla.png");
				item->Link = "http://www.blueyonderairlines.com/";
				item->Category = "Ice Cream";
				item->Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Creamy Caramel Frozen Yogurt";
				item->Subtitle = "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet";
				item->SetImage(_baseUri, "SampleData/Images/60SauceCaramel.png");
				item->Link = "http://www.adatum.com/";
				item->Category = "Low-fat frozen yogurt";
				item->Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Chocolate Lovers Frozen Yougurt";
				item->Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est";
				item->SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Low-fat frozen yogurt";
				item->Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Roma Strawberry";
				item->Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis";
				item->SetImage(_baseUri, "SampleData/Images/60Strawberry.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Gelato";
				item->Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Italian Rainbow";
				item->Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis";
				item->SetImage(_baseUri, "SampleData/Images/60SprinklesRainbow.png");
				item->Link = "http://www.alpineskihouse.com/";
				item->Category = "Gelato";
				item->Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Straweberry";
				item->Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est";
				item->SetImage(_baseUri, "SampleData/Images/60Strawberry.png");
				item->Link = "http://www.blueyonderairlines.com/";
				item->Category = "Ice Cream";
				item->Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Strawberry Frozen Yogurt";
				item->Subtitle = "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet";
				item->SetImage(_baseUri, "SampleData/Images/60Strawberry.png");
				item->Link = "http://www.adatum.com/";
				item->Category = "Low-fat frozen yogurt";
				item->Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Bongo Banana";
				item->Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est";
				item->SetImage(_baseUri, "SampleData/Images/60Banana.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Sorbet";
				item->Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Firenze Vanilla";
				item->Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis";
				item->SetImage(_baseUri, "SampleData/Images/60Vanilla.png");
				item->Link = "http://www.adventure-works.com/";
				item->Category = "Gelato";
				item->Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Choco-wocko";
				item->Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis";
				item->SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png");
				item->Link = "http://www.alpineskihouse.com/";
				item->Category = "Sorbet";
				item->Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant";
				item->Content = LONG_LOREM_IPSUM;
				 _items->Append(item);

				item = ref new Item();
				item->Title = "Chocolate";
				item->Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est";
				item->SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png");
				item->Link = "http://www.blueyonderairlines.com/";
				item->Category = "Ice Cream";
				item->Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac";
				item->Content = LONG_LOREM_IPSUM;
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