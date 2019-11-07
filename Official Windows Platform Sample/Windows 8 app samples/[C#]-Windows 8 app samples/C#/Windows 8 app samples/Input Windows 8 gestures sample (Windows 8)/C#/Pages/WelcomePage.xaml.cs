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
    public sealed partial class WelcomePage : GesturePageBase
    {
        public WelcomePage() :
            base( 
                "Welcome",
                "Hi",
                "This app gives you a quick hands-on overview of the Windows 8 touch language.",
                "",
                "Assets/welcome.png")
        {
            this.InitializeComponent();
        }
    }
}
