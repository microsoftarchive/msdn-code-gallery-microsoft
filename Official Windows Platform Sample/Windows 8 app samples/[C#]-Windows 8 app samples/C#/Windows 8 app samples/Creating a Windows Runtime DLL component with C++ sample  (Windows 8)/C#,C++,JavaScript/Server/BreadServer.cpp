// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "Microsoft.SDKSamples.Kitchen.h"
#include "BreadServer.h"

namespace ABI { namespace Microsoft { namespace SDKSamples { namespace Kitchen {

    HRESULT Bread::RuntimeClassInitialize(
        _In_ HSTRING flavor)
    {
        return ::WindowsDuplicateString(flavor, &_hstrFlavor);
    }

    IFACEMETHODIMP Bread::get_Flavor(
        _Out_ HSTRING *flavor)
    {
        return ::WindowsDuplicateString(_hstrFlavor, flavor);
    }

} /* Kitchen */ } /* SDKSamples */ } /* Microsoft */ } /* ABI */
