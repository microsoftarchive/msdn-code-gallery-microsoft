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

namespace SecondaryTiles
{
    public value struct Scenario
    {
        Platform::String^ title;
        Platform::String^ className;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"Secondary Tile C++");
            }
        }

        static property Platform::String^ tileId
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"SecondaryTile.Logo");
            }
        }

        static property Platform::String^ dynamicTileId
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"SecondaryTile.LiveTile");
            }
        }

        static property Platform::String^ appbarTileId
        {
            Platform::String^ get()
            {
                return ref new Platform::String(L"SecondaryTile.AppBar");
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

        static Windows::Foundation::Rect GetElementRect(Windows::UI::Xaml::FrameworkElement^ element);

    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };


}
