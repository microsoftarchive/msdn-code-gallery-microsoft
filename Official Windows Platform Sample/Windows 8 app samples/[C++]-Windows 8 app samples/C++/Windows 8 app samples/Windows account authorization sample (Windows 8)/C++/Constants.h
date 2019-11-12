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

using namespace Windows::UI::Xaml::Controls;

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
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"Microsoft Account Authentication"); 
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
