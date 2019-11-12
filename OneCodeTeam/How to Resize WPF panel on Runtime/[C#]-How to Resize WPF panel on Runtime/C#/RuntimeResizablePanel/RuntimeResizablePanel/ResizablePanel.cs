/****************************** Module Header ******************************\
Module Name:  ResizablePanel.cs
Project:      RuntimeResizablePanel
Copyright (c) Microsoft Corporation.

ResizablePanel is a custom panel control. 
This panel will be the panel which user would resize on runtime.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Windows;
using System.Windows.Controls;

namespace RuntimeResizablePanel
{
    public class ResizablePanel : ContentControl
    {
        static ResizablePanel()
        {
            // This will allow us to create a Style in Generic.xaml with target type ResizablePanel.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizablePanel), new FrameworkPropertyMetadata(typeof(ResizablePanel)));            
        }
    }
}