// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Team.h
// Declaration of the Team class
//

#pragma once

namespace SDKSample
{
    namespace DataBinding
    {
    	// This Data Source is used in Scenarios 4, 5 and 6.
    	[Windows::UI::Xaml::Data::Bindable]
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class Team sealed 
    		: Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^> 
    	{
    	public:
    		Team();
    		property Platform::String^ Name
    		{
    			Platform::String^ get()
    			{
    				return _name;
    			}
    			void set(Platform::String^ value)
    			{
    				_name = value;
    			}
    		}
    		property Platform::String^ City
    		{
    			Platform::String^ get()
    			{
    				return _city;
    			}
    			void set(Platform::String^ value)
    			{
    				_city = value;
    			}
    		}
    		property Windows::UI::Xaml::Media::SolidColorBrush^ Color 
    		{ 
    			Windows::UI::Xaml::Media::SolidColorBrush^ get()
    			{
    				return _color;
    			}
    			void set(Windows::UI::Xaml::Media::SolidColorBrush^ value)
    			{
    				_color = value;
    			}
    		}
 

    		virtual void Clear()
    		{
    			_propBag->Clear();
    		}
    		virtual Windows::Foundation::Collections::IMapView<Platform::String^, Platform::Object^>^ GetView()
    		{
    			return _propBag->GetView();
    		}
    		virtual bool HasKey(Platform::String^ key)
    		{
    			return _propBag->HasKey(key);
    		}
    		virtual bool Insert(Platform::String^ key, Platform::Object^ value)
    		{
    			return _propBag->Insert(key, value);
    		}
    		virtual Object^ Lookup(Platform::String^ key)
    		{
    			return _propBag->Lookup(key);
    		}
    		virtual void Remove(Platform::String^ key)
    		{
    			return _propBag->Remove(key);
    		}
    		virtual property default::uint32 Size 
    		{ 
    			default::uint32 get()
    			{
    				return _propBag->Size;
    			}
    		}
    		virtual Windows::Foundation::Collections::IIterator<Windows::Foundation::Collections::IKeyValuePair<Platform::String^, Platform::Object^>^>^ First()
    		{
    			return _propBag->First();
    		}
    
    	private:
    		Platform::String^ _name;
    		Platform::String^ _city;
    		Windows::UI::Xaml::Media::SolidColorBrush^ _color;
    		Windows::Foundation::Collections::PropertySet^ _propBag;
    
    	};
    
    	// This class is used to demonstrate grouping.
    	[Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
    	public ref class Teams sealed  
    		: Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^> 
    	{
    	public:
    		Teams();
    		property Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ Items
    		{
    			Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ get()
    			{
    				return _items;
    			}
    		}

    		virtual void Clear()
    		{
    			_propBag->Clear();
    		}
    		virtual Windows::Foundation::Collections::IMapView<Platform::String^, Platform::Object^>^ GetView()
    		{
    			return _propBag->GetView();
    		}
    		virtual bool HasKey(Platform::String^ key)
    		{
    			return _propBag->HasKey(key);
    		}
    		virtual bool Insert(Platform::String^ key, Platform::Object^ value)
    		{
    			return _propBag->Insert(key, value);
    		}
    		virtual Platform::Object^ Lookup(Platform::String^ key)
    		{
    			return _propBag->Lookup(key);
    		}
    		virtual void Remove(Platform::String^ key)
    		{
    			return _propBag->Remove(key);
    		}
    		virtual property default::uint32 Size 
    		{ 
    			default::uint32 get()
    			{
    				return _propBag->Size;
    			}
    		}
    		virtual Windows::Foundation::Collections::IIterator<Windows::Foundation::Collections::IKeyValuePair<Platform::String^, Platform::Object^>^>^ First()
    		{
    			return _propBag->First();
    		}
    
    	private:
    		Windows::Foundation::Collections::PropertySet^ _propBag;
    		Platform::Collections::Vector<Platform::Object^>^ _items;
    	};
    
    	/// <summary>
    	/// Generic group data model.
    	/// </summary>
    	[Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
    	public ref class TeamGroup sealed 
    	{
    	public:
    		TeamGroup(Platform::String^ name);
    		property Platform::String^ Key
    		{
    			Platform::String^ get()
    			{
    				return _name;
    			}
    			void set(Platform::String^ value)
    			{
    				_name = value;
    			}
    		}
    		property Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ Items
    		{
    			Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ get()
    			{
    				return _items;
    			}
    		}
    	private:
    		Platform::String^ _name;
    		Platform::Collections::Vector<Platform::Object^>^ _items;
    	};
    
    	/// <summary>
    	/// Creates a collection of groups and items.
    	/// </summary>
    	[Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
    	public ref class TeamDataSource sealed 
    	{
    	public:
    		TeamDataSource();
    		property Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ ItemGroups
    		{
    			Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ get()
    			{
    				return _itemGroups;
    			}
    		}
    	private:
    		Platform::Collections::Vector<Platform::Object^>^ _itemGroups;
    	};
    
    }
}
