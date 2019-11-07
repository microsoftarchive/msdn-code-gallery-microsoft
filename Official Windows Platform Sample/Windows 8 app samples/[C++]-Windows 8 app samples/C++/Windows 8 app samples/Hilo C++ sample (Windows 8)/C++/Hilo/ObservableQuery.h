// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "PageType.h"

namespace Hilo
{
    // The ObservableQuery class provides an interface for view models to receive notifications of file system changes.
    class ObservableQuery
    {
    public:
        virtual void AddObserver(const std::function<void()> callback, PageType pageType) = 0;
        virtual void RemoveObserver(PageType pageType) = 0;
    };
}