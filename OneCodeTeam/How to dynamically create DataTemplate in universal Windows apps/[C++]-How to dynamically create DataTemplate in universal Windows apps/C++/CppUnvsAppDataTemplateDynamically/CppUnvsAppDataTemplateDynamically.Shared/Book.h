/****************************** Module Header ******************************\
* Module Name:    Book.h
* Project:        CppUnvsAppDataTemplateDynamically
* Copyright (c) Microsoft Corporation.
*
* This class is used to initialize data in Book collection
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#pragma once
using namespace std;

namespace CppUnvsAppDataTemplateDynamically
{
	[Windows::UI::Xaml::Data::Bindable]
	public ref class Book sealed
	{
	public:
		Book(Platform::String^ author, float32 price, Platform::String^ name);
		static  Windows::Foundation::Collections::IObservableVector<Book^>^ GetBooks();
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
		property float32 Price
		{
			float32 get()
			{
				return _price;
			}
			void set(float32 value)
			{
				_price = value;
			}
		}
		property Platform::String^ Author
		{
			Platform::String^ get()
			{
				return _author;
			}
			void set(Platform::String^ value)
			{
				_author = value;
			}
		}
	private:
		Platform::String^ _name;
		float32 _price;
		Platform::String^ _author;
	};

}

