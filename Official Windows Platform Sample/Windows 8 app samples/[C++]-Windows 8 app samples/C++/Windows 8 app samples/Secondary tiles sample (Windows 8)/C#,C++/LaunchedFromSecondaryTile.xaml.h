//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// LaunchedFromSecondaryTile.xaml.h
// Declaration of the LaunchedFromSecondaryTile class
//

#pragma once

#include "pch.h"
#include "LaunchedFromSecondaryTile.g.h"
#include "MainPage.xaml.h"

namespace SecondaryTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class LaunchedFromSecondaryTile sealed
    {
    public:
        LaunchedFromSecondaryTile();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        Windows::UI::Xaml::Controls::AppBar^ appBar;
    };
}
