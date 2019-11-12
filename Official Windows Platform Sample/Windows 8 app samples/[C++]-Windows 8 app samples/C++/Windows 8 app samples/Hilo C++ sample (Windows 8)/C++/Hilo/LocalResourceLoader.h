// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IResourceLoader.h"

namespace Hilo
{
    // The LocalResourceLoader provides a helper method that gets strings from the app's resource file.
    public ref class LocalResourceLoader sealed : public IResourceLoader
    {
    public:
        virtual Platform::String^ GetString(Platform::String^ value);
    };
}