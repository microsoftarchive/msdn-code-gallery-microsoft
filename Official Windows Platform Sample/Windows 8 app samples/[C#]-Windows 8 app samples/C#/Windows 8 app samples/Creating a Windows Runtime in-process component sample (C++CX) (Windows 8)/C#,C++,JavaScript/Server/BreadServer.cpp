//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"

namespace Microsoft { namespace SDKSamples { namespace Kitchen {

    Bread::Bread(Platform::String^ flavor) :
        _flavor(flavor)
    {
    }

    Platform::String^ Bread::Flavor::get()
    {
        return _flavor;
    }

} /* Kitchen */ } /* SDKSamples */ } /* Microsoft */
