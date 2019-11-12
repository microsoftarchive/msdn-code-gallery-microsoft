//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "stdafx.h"
#include <wrl\module.h>
#include "StspSchemeHandler.h"
#include "StspMediaSink.h"

using namespace Stsp;

// COM proxy/stubs
extern "C" HRESULT WINAPI PrxDllGetClassObject(REFCLSID, REFIID, _Outptr_ LPVOID*);
extern "C" BOOL WINAPI PrxDllMain(_In_ HINSTANCE, DWORD, _In_opt_ LPVOID);
extern "C" HRESULT WINAPI PrxDllCanUnloadNow();

namespace Microsoft { namespace Samples { namespace SimpleCommunication {
    ActivatableClass(CSchemeHandler);
    ActivatableClass(CMediaSink);
}}}


BOOL WINAPI DllMain( _In_ HINSTANCE hInstance, _In_ DWORD dwReason, _In_opt_ LPVOID lpReserved )
{
    if( DLL_PROCESS_ATTACH == dwReason )
    {
        //
        //  Don't need per-thread callbacks
        //
        DisableThreadLibraryCalls( hInstance );

        Module<InProc>::GetModule().Create();
    }
    else if( DLL_PROCESS_DETACH == dwReason )
    {
        Module<InProc>::GetModule().Terminate();
    }

    PrxDllMain( hInstance, dwReason, lpReserved );

    return TRUE;
}

HRESULT WINAPI DllGetActivationFactory( _In_ HSTRING activatibleClassId, _Outptr_ IActivationFactory** factory )
{
    auto &module = Microsoft::WRL::Module< Microsoft::WRL::InProc >::GetModule();
    return module.GetActivationFactory( activatibleClassId, factory );
}

HRESULT WINAPI DllCanUnloadNow()
{
    auto &module = Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule();    
    return module.Terminate() ? PrxDllCanUnloadNow() : S_FALSE;
}

STDAPI DllGetClassObject( _In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID FAR* ppv )
{
    auto &module = Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule();
    HRESULT hr = module.GetClassObject( rclsid, riid, ppv );
    if (FAILED(hr))
    {
        hr = PrxDllGetClassObject( rclsid, riid, ppv );
    }
    return hr;
}
