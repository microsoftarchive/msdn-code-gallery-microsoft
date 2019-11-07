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

#include <collection.h>

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
		static Windows::UI::Color ConvertIndexToColor(int index)
		{
			// Converts between ComboBox indices and colors
				switch (index)
				{
					case 0:
						return Windows::UI::Colors::Red;
					case 1:			 
						return Windows::UI::Colors::Green;
					case 2:			 
						return Windows::UI::Colors::Blue;
					case 3:			 
						return Windows::UI::Colors::Gray;
					default:		 
						return Windows::UI::Colors::White;
				}
		}
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"XAML Drawing Sample"); 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };
}
