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
// PageWithAppBar.xaml.cpp
// Implementation of the PageWithAppBar class
//

#include "pch.h"
#include "PageWithAppBar.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

PageWithAppBar::PageWithAppBar()
{
    InitializeComponent();

     // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Hook the opned and closed events on the AppBar so we know when
    // to adjust the WebView.
    BottomAppBar->Opened += ref new EventHandler<Object^>(this, &PageWithAppBar::BottomAppBar_Opened);
    BottomAppBar->Closed += ref new EventHandler<Object^>(this, &PageWithAppBar::BottomAppBar_Closed);
}


// Invoked when this page is about to be displayed in a Frame.
void PageWithAppBar::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Navigate to some web site
    WebView8->Navigate(ref new Uri("http://www.microsoft.com"));
}

void PageWithAppBar::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

void SDKSample::WebViewControl::PageWithAppBar::BottomAppBar_Opened(Object^ sender, Object^ obj)
{
    // AppBar has Opened so we need to put the WebView back to its
    // original size/location.
    AppBar^ bottomAppBar = (AppBar^) sender;
    if (bottomAppBar != nullptr)
    {
        // Force layout so that we can guarantee that our AppBar's
        // actual height has height
        this->UpdateLayout();
        // Get the height of the AppBar
        double appBarHeight = bottomAppBar->ActualHeight;
        // Reduce the height of the WebView to allow for the AppBar
        WebView8->Height = WebView8->ActualHeight - appBarHeight;
        // Translate the WebView in the Y direction to reclaim the space occupied by the AppBar.  
        TranslateYOpen->To = -appBarHeight / 2.0;
        // Run our translate animation to match the AppBar
        OpenAppBar->Begin();
    }
}

void SDKSample::WebViewControl::PageWithAppBar::BottomAppBar_Closed(Object^ sender, Object^ obj)
{
    // AppBar has closed so we need to put the WebView back to its
    // original size/location.
    AppBar^ bottomAppBar = (AppBar^) sender;
    if (bottomAppBar != nullptr)
    {
        // Force layout so that we can guarantee that our AppBar's
        // actual height has height
        this->UpdateLayout();
        // Get the height of the AppBar
        double appBarHeight = bottomAppBar->ActualHeight;
        // Increase the height of the WebView to allow for the space
        // that was occupied by the AppBar
        WebView8->Height = WebView8->ActualHeight + appBarHeight;
        // Translate the WebView in the Y direction to allow for 
        TranslateYOpen->To = appBarHeight / 2.0;
        // Run our translate animation to match the AppBar
        CloseAppBar->Begin();                            
    }
}


// Click handler for home button
void SDKSample::WebViewControl::PageWithAppBar::Home_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (rootPage->Frame->CanGoBack)
    {
        rootPage->Frame->GoBack();
    }
}
