//-----------------------------------------------------------------------
// <copyright file="Driver.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Driver.cpp
//
// Description:
//      This file implements the ATL module and DLL entry points.
//
//-----------------------------------------------------------------------


#include "Common.h"


extern ATL::_ATL_OBJMAP_ENTRY* ObjectMap;


CComModule _Module; // Required by ATLCOM.h


/////////////////////////////////////////////////////////////////////////
//
// DllMain
//
// This is the main DLL Entry Point.
//
// Parameters:
//      hInstance  - Handle to the DLL module
//      dwReason   - Indicates why the DLL entry point is being called
//      lpReserved - Additional information based on dwReason
//
// Return Values:
//      TRUE  = initialization succeeds
//      FALSE = initialization fails
//
/////////////////////////////////////////////////////////////////////////
extern "C" BOOL WINAPI DllMain(HINSTANCE    hInstance,
                               DWORD        dwReason,
                               LPVOID       lpReserved)
{
    (lpReserved);

    switch(dwReason)
    {
        case DLL_PROCESS_ATTACH:
            _Module.Init(ObjectMap, hInstance);
            DisableThreadLibraryCalls(hInstance);
            break;

        case DLL_PROCESS_DETACH:
            _Module.Term();
            break;

        default:
            break;
    }

    return TRUE;
}

/////////////////////////////////////////////////////////////////////////
//
// DllCanUnloadNow
//
// Used to determine whether the DLL can be unloaded by OLE
//
// Parameters:
//      void - (unused argument)
//
// Return Values:
//      S_OK: DLL can be unloaded
//      S_FALSE: DLL cannot be unloaded now
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllCanUnloadNow(void)
{
    return ((0 == _Module.GetLockCount()) ? S_OK : S_FALSE);
}

/////////////////////////////////////////////////////////////////////////
//
// DllGetClassObject
//
// Returns a class factory to create an object of the requested type
//
// Parameters:
//      rclsid - CLSID that will associate the correct data and code
//      riid   - Reference to the IID the caller will use
//      ppv    - pointer to an interface pointer requested in riid
//
// Return Values:
//      S_OK: The object was retrieved successfully.
//      CLASS_E_CLASSNOTAVAILABLE: The DLL does not support the class
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID FAR* ppv)
{
    return _Module.GetClassObject(rclsid, riid, ppv);
}

/////////////////////////////////////////////////////////////////////////
//
// DllRegisterServer
//
// Adds entries to the system registry
//
// Parameters:
//      void - (unused argument)
//
// Return Values:
//      S_OK: The registry entries were created successfully
//      SELFREG_E_TYPELIB: The server was unable to complete the
//          registration of all the type libraries used by its classes
//      SELFREG_E_CLASS: The server was unable to complete the
//          registration of all the object classes
//
/////////////////////////////////////////////////////////////////////////
STDAPI DllRegisterServer(void)
{
    //
    // Registers object; pass FALSE so as not to register the typelib
    //

    return _Module.RegisterServer(FALSE);
}

/////////////////////////////////////////////////////////////////////////
//
// DllUnregisterServer
//
// Removes entries from the system registry
//
// Parameters:
//      void - (unused argument)
//
// Return Values:
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
    //
    // Unregisters objects; pass FALSE so as not to unregister the typelib
    //

    return _Module.UnregisterServer(FALSE);
}
