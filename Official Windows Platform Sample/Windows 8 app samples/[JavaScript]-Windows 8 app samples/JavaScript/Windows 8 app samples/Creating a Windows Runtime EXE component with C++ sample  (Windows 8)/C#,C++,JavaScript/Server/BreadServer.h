//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include <wrl\implements.h>

#include "Microsoft.SDKSamples.Kitchen.h"

namespace ABI { namespace Microsoft { namespace SDKSamples { namespace Kitchen {

    class Bread : public ::Microsoft::WRL::RuntimeClass<::ABI::Microsoft::SDKSamples::Kitchen::IBread>
    {
        InspectableClass(RuntimeClass_Microsoft_SDKSamples_Kitchen_Bread, TrustLevel::BaseTrust)

    public:
        // Non-projected method for setting up private state of Bread. This method is 
        // called by WRL::MakeAndInitialize
        HRESULT RuntimeClassInitialize(
            _In_ HSTRING hstrFlavor);

        // IBread::Flavor property
        IFACEMETHOD(get_Flavor)(
            _Out_ HSTRING *phstrFlavor);

    private:
        HSTRING _hstrFlavor;
    };

} /* Kitchen */ } /* SDKSamples */ } /* Microsoft */ } /* ABI */
