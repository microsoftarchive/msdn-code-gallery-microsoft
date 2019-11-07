/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    DllMain.cpp

Abstract:

    This module contains the implementation of the SPB accerlerometer's 
    entry point and its exported functions for providing COM support.

    This module is dependent on the following defines:

        MYDRIVER_TRACING_ID -   A wide string passed to WPP when initializing 
                                tracing.  
--*/
#include "Internal.h"

#pragma warning(push)
#pragma warning(disable:28930) // ControlGuid unreferenced due to WPP_INIT_TRACING() but WPP_PRIVATE_ENABLE_CALLBACK not defined
#include "DllMain.tmh"
#pragma warning(pop)

class CSpbAccelerometerDriverModule : public CAtlDllModuleT<CSpbAccelerometerDriverModule> {} _AtlModule;

/////////////////////////////////////////////////////////////////////////
//
//  DllMain
//
//  This is the main DLL Entry Point.
//
//  Parameters:
//      hInstance  - Handle to the DLL module
//      dwReason   - Indicates why the DLL entry point is being called
//      lpReserved - Additional information based on dwReason
//
//  Return Values:
//      TRUE  = initialization succeeds
//      FALSE = initialization fails
//
/////////////////////////////////////////////////////////////////////////
extern "C" BOOL WINAPI DllMain(HINSTANCE    hInstance,
                               DWORD        dwReason,
                               LPVOID       lpReserved)
{
    (lpReserved);

    switch (dwReason)
    {
        case DLL_PROCESS_ATTACH:
            // Initialize tracing
            WPP_INIT_TRACING(MYDRIVER_TRACING_ID);
            DisableThreadLibraryCalls(hInstance);
            break;

        case DLL_PROCESS_DETACH:
            // Cleanup tracing.
            WPP_CLEANUP();
            _AtlModule.Term();
            break;

        default:
            break;
    }

    // Call the ATL module class so it can initialize
    return _AtlModule.DllMain(dwReason, lpReserved); 
}

/////////////////////////////////////////////////////////////////////////
//
//  DllCanUnloadNow
//
//  Used to determine whether the DLL can be unloaded by OLE
//
//  Parameters:
//      void - (unused argument)
//
//  Return Values:
//      S_OK: DLL can be unloaded
//      S_FALSE: DLL cannot be unloaded now
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllCanUnloadNow(void)
{
    return _AtlModule.DllCanUnloadNow();
}

/////////////////////////////////////////////////////////////////////////
//
//  DllGetClassObject
//
//  Returns a class factory to create an object of the requested type
//
//  Parameters:
//      rclsid - CLSID that will associate the correct data and code
//      riid   - Reference to the IID the caller will use
//      ppv    - pointer to an interface pointer requested in riid
//
//  Return Values:
//      S_OK: The object was retrieved successfully.
//      CLASS_E_CLASSNOTAVAILABLE: The DLL does not support the class
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID* ppv)
{
    return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}

/////////////////////////////////////////////////////////////////////////
//
//  DllRegisterServer
//
//  Adds entries to the system registry
//
//  Parameters:
//      void - (unused argument)
//
//  Return Values:
//      S_OK: The registry entries were created successfully
//      SELFREG_E_TYPELIB: The server was unable to complete the
//          registration of all the type libraries used by its classes
//      SELFREG_E_CLASS: The server was unable to complete the
//          registration of all the object classes
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllRegisterServer(void)
{
    return S_OK;
}

/////////////////////////////////////////////////////////////////////////
//
//  DllUnregisterServer
//
//  Removes entries from the system registry
//
//  Parameters:
//      void - (unused argument)
//
//  Return Values:
//      S_OK: The registry entries were removed successfully
//      S_FALSE: Unregistration of known entries was successful, but
//          other entries still exist for this server's classes
//      SELFREG_E_TYPELIB: The server was unable to remove the entries
//          of all the type libraries used by its classes
//      SELFREG_E_CLASS: The server was unable to to remove the entries
//          of all the object classes
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllUnregisterServer(void)
{
    return S_OK;
}
