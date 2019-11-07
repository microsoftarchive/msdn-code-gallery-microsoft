// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "LocalResourceLoader.h"

using namespace Hilo;
using namespace Platform;
using namespace Windows::ApplicationModel::Resources;

String^ LocalResourceLoader::GetString(String^ value)
{
    auto loader = ref new ResourceLoader();
    return loader->GetString(value);
}
