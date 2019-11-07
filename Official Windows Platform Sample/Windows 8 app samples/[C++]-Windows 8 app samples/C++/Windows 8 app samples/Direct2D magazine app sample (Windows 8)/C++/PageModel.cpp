//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Magazine;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;

PageModel::PageModel(
    _In_ Element^ content,
    _In_ Document^ document
    )
{
    // Get the first <layer> element of the <page> element. At least two layers are required
    // to complete a page view. One for the page content and another for background.

    Element^ child = content->GetFirstChild();

    if (child != nullptr)
    {
        // Background image is opaque. By passing true as the last parameter, the virtual
        // surface image source can optimize for rendering performance accordingly. 
        m_background = new ContentImageSource(child, document, true);
        child = child->GetNextSibling();
    }

    if (child != nullptr)
    {
        m_content = new ContentImageSource(child, document, false);
    }
}

void PageModel::UpdateContent(_In_ Element^ content)
{
    Element^ child = content->GetFirstChild();

    if (child != nullptr)
    {
        m_background->UpdateContent(child);

        PropertyChanged(this, ref new PropertyChangedEventArgs("Background"));

        child = child->GetNextSibling();
    }

    if (child != nullptr)
    {
        // Top-level content is always resized to fit with the current window size.
        m_content->UpdateContent(child);

        PropertyChanged(this, ref new PropertyChangedEventArgs("Content"));
        PropertyChanged(this, ref new PropertyChangedEventArgs("ContentWidth"));
        PropertyChanged(this, ref new PropertyChangedEventArgs("ContentHeight"));
    }
}
