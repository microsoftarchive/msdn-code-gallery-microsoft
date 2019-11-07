//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using GesturesApp.Controls;
using System;

namespace GesturesApp.Pages
{
    public sealed partial class AppEdgyPage : GesturePageBase
    {
        public AppEdgyPage() :
            base(
                "AppEdgy",
                "Swipe in from the top or bottom for app commands or to close an app",
                "Swipe from the bottom or top edge to show the app commands (New, Delete, etc.). The easiest way to include commands in your app is to use the app bar.\nA longer swipe down from the top forces an app to close.",
                "Similar to when you use a mouse and right-click within the app.",
                "Assets/app_edgy.png")
        {
            this.InitializeComponent();

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["API: EdgeGesture class"] = new Uri("http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.input.edgegesture");
            this._links["API: EdgeGestureEventArgs"] = new Uri("http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.input.edgegestureeventargs");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));
        }
    }
}
