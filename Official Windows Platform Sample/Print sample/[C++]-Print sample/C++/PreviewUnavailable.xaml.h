// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// PreviewUnavailable.xaml.h
// Declaration of the PreviewUnavailable class
//

#pragma once

#include "PreviewUnavailable.g.h"

namespace PrintSample
{
    public ref class PreviewUnavailable sealed
    {
    public:
        PreviewUnavailable();

        /// <summary>
        /// Preview unavailable page constructor
        /// </summary>
        /// <param name="paperSize">The printing page paper size</param>
        /// <param name="printableSize">The usable/"printable" area on the page</param>
        PreviewUnavailable(Windows::Foundation::Size paperSize, Windows::Foundation::Size printableSize);
    };
}
