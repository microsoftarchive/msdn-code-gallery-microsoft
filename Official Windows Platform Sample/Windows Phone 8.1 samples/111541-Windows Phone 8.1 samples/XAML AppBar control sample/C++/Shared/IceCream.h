//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// App.xaml.h
// Declaration of the App.xaml class.
//

#pragma once

#include "pch.h"
#include "App.g.h"

namespace SDKSample
{
	namespace AppBarControl
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::BindableAttribute]
		public ref class IceCream sealed
		{
		public:
			property Platform::String^ Name
			{
				Platform::String^ get()
				{
					return name;
				}

				void set(Platform::String^ value)
				{
					name = value;
				}
			}
			property Platform::String^ Type
			{
				Platform::String^ get()
				{
					return type;
				}

				void set(Platform::String^ value)
				{
					type = value;
				}
			}
			property Platform::String^ Image
			{
				Platform::String^ get()
				{
					return image;
				}

				void set(Platform::String^ value)
				{
					image = value;
				}
			}

		private:
			Platform::String^ name;
			Platform::String^ type;
			Platform::String^ image;
		};
	}
}
