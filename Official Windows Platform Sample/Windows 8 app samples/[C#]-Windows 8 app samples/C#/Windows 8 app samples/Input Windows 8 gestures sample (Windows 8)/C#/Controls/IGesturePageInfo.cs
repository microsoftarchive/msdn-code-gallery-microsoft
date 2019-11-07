//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;

namespace GesturesApp.Controls
{
    public interface IGesturePageInfo
    {
        String UniqueId { get; }
        String Title { get; }
        String Description { get; }
        String SimilarTo { get; }
        Windows.UI.Xaml.Media.ImageSource Image { get; }
        GesturesApp.Controls.GesturePageBase PlayArea { get; }
    }
}
