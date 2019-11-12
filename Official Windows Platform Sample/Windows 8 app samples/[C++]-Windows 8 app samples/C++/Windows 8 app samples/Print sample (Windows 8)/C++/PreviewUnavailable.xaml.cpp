// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// PreviewUnavailable.xaml.cpp
// Implementation of the PreviewUnavailable class
//

#include "pch.h"
#include "PreviewUnavailable.xaml.h"

using namespace PrintSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

PreviewUnavailable::PreviewUnavailable()
{
    InitializeComponent();
}

/// <summary>
/// Preview unavailable page constructor
/// </summary>
/// <param name="paperSize">The printing page paper size</param>
/// <param name="printableSize">The usable/"printable" area on the page</param>
PreviewUnavailable::PreviewUnavailable(Size paperSize, Size printableSize)
{
    InitializeComponent();

    page->Width = paperSize.Width;
    page->Height  = paperSize.Height;
    
    printablePage->Width = printableSize.Width;
    printablePage->Height = printableSize.Height;
}
