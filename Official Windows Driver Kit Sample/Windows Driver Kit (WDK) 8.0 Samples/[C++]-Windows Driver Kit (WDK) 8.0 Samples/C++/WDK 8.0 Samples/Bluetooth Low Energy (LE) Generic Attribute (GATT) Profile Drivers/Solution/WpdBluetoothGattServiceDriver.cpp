/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdBluetoothGattServiceDriver.cpp
    
Abstract:

--*/


#include "stdafx.h"

#include "WpdBluetoothGattServiceDriver.tmh"

HINSTANCE g_hInstance = NULL;

class CWpdBluetoothGattServiceSampleDriverModule : public CAtlDllModuleT< CWpdBluetoothGattServiceSampleDriverModule >
{
public :
    DECLARE_REGISTRY_APPID_RESOURCEID(IDR_WpdBluetoothGattServiceSampleDriver, "{95B558CB-F6B1-4B37-A105-3B7B6A196FB5}")
    DECLARE_LIBID(LIBID_WpdBluetoothGattServiceSampleDriverLib)
};

CWpdBluetoothGattServiceSampleDriverModule _AtlModule;

// DLL Entry Point
extern "C" BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
    if(dwReason == DLL_PROCESS_ATTACH)
    {
        g_hInstance = hInstance;

        // Initialize tracing.
        WPP_INIT_TRACING(MYDRIVER_TRACING_ID);
    }
    else if (dwReason == DLL_PROCESS_DETACH)
    {
        // Cleanup tracing.
        WPP_CLEANUP();
    }

    return _AtlModule.DllMain(dwReason, lpReserved);
}

// Used to determine whether the DLL can be unloaded by OLE
STDAPI DllCanUnloadNow(void)
{
    return _AtlModule.DllCanUnloadNow();
}

// Returns a class factory to create an object of the requested type
STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID* ppv)
{
    return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}

// DllRegisterServer - Adds entries to the system registry
STDAPI DllRegisterServer(void)
{
    // registers object, typelib and all interfaces in typelib
    HRESULT hr = _AtlModule.DllRegisterServer();
    return hr;
}

// DllUnregisterServer - Removes entries from the system registry
STDAPI DllUnregisterServer(void)
{
    HRESULT hr = _AtlModule.DllUnregisterServer();
    return hr;
}

