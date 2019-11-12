// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ContinuationPage.xaml.h
// Declaration of the ContinuationPage class
//

#pragma once

#include "ContinuationPage.g.h"

namespace PrintSample
{
    /// <summary>
    /// A paged used to flow text from a given text container
    /// Usage: Outputscenarios 1-5 might not fit entirely on a given "printer page"
    /// In that case simply add new continuation pages of the given size until all the content can be displayed
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ContinuationPage sealed
    {
    public:
        ContinuationPage();

        /// <summary>
        /// Creates a continuation page and links text-flow to a text flow container
        /// </summary>
        /// <param name="textLinkContainer">Text link container which will flow text into this page</param>
        ContinuationPage(Windows::UI::Xaml::Controls::RichTextBlockOverflow^ textLinkContainer);

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    };
}
