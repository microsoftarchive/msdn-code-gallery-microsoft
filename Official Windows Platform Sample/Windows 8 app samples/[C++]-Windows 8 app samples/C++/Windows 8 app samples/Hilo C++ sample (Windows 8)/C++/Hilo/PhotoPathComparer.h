// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IPhoto;

    // The PhotoPathComparer struct provides a comparison operator for Hilo images.
    struct PhotoPathComparer : public std::equal_to<IPhoto^>
    {
        bool operator()(IPhoto^ left, IPhoto^ right) const;
    };
}