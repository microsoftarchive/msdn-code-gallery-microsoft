//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include <wrl\module.h>

#include "Microsoft.SDKSamples.Kitchen.h"
#include "OvenServer.h"

STDAPI DllGetActivationFactory(_In_ HSTRING activatibleClassId, _COM_Outptr_ IActivationFactory** factory)
{
    auto &module = Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule();
    return module.GetActivationFactory(activatibleClassId, factory);
}

STDAPI DllCanUnloadNow()
{
    auto &module = Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule();
    return module.Terminate() ? S_OK : S_FALSE;
}

namespace ABI { namespace Microsoft { namespace SDKSamples { namespace Kitchen {
    ActivatableClassWithFactory(Oven, OvenFactory)
} /* Kitchen */ } /* SDKSamples */ } /* Microsoft */ } /* ABI */
