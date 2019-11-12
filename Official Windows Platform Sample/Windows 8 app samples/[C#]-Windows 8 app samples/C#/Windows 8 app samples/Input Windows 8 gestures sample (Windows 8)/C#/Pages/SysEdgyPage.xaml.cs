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
    public sealed partial class SysEdgyPage : GesturePageBase
    {
        public SysEdgyPage() :
            base(
                "SysEdgy",
                "Swipe in from the left or right for system commands",
                "Swipe in from the right side of the screen to show the charms (Search, Share, Start, Devices, and Settings).\nSwipe in from the left side of the screen to return to recently used apps.",
                "Similar to when you move the mouse pointer into the upper-left corner and click to switch apps, and when you move the pointer into the upper-right corner, move it down the edge, and then click the charm you want.",
                "Assets/sys_edgy.png")
        {
            this.InitializeComponent();

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["API: EdgeGesture class"] = new Uri("http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.input.edgegesture");
            this._links["API: EdgeGestureEventArgs class"] = new Uri("http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.input.edgegestureeventargs");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));
        }
    }
}
