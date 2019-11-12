//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1996-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:	oemcom.cpp
//    
//  PURPOSE:  UI plug-in implementation.
//

#include "stddef.h"
#include "stdlib.h"
#include "objbase.h"
#include <windows.h>
#include <assert.h>
#include <prsht.h>
#include <compstui.h>
#include <winddiui.h>
#include <printoem.h>
#include <initguid.h>
#include <prcomoem.h>
#include "oemcomui.h"
#include "ttyui.h"
#include "debug.h"
#include "name.h"
#include <strsafe.h>

// This indicates to Prefast that this is a usermode driver file.
_Analysis_mode_(_Analysis_code_type_user_driver_);

// Globals
static HMODULE g_hModule = NULL ;   // DLL module handle
static long g_cComponents = 0 ;     // Count of active components
static long g_cServerLocks = 0 ;    // Count of locks



////////////////////////////////////////////////////////////////////////////////
//
// IOemCB body
//
HRESULT __stdcall IOemCB::QueryInterface(const IID& iid, void** ppv)
{
    VERBOSE(DLLTEXT("IOemCB:QueryInterface entry.\n\n"));
    if (iid == IID_IUnknown)
    {
        *ppv = static_cast<IUnknown*>(this);
        VERBOSE(DLLTEXT("IOemCB:Return pointer to IUnknown.\n\n"));
    }
    else if (iid == IID_IPrintOemUI)
    {
        *ppv = static_cast<IPrintOemUI*>(this) ;
        VERBOSE(DLLTEXT("IOemCB:Return pointer to IPrintOemUI.\n"));
    }
    else
    {
        *ppv = NULL ;
        WARNING(DLLTEXT("IOemCB:No Interface. Return NULL.\n"));
        return E_NOINTERFACE ;
    }
    reinterpret_cast<IUnknown*>(*ppv)->AddRef() ;
    return S_OK ;
}

ULONG __stdcall IOemCB::AddRef()
{
    VERBOSE(DLLTEXT("IOemCB:AddRef entry.\n"));
    return InterlockedIncrement(&m_cRef) ;
}

_At_(this, __drv_freesMem(object)) 
ULONG __stdcall IOemCB::Release()
{
   VERBOSE(DLLTEXT("IOemCB:Release entry.\n"));
   ASSERT( 0 != m_cRef);
   ULONG cRef = InterlockedDecrement(&m_cRef);
   if (0 == cRef)
   {
      delete this;
        
   }
   return cRef;
}


IOemCB::~IOemCB()
{
    //
    // Make sure that driver's helper function interface is released.
    //
    if(NULL != pOEMHelp)
    {
        pOEMHelp->Release();
        pOEMHelp = NULL;
    }

    //
    // If this instance of the object is being deleted, then the reference
    // count should be zero.
    //
    assert (0 == m_cRef) ;
   
}

LONG __stdcall IOemCB::PublishDriverInterface(
    IUnknown *pIUnknown)
{
    VERBOSE(DLLTEXT("IOemCB::PublishDriverInterface() entry.\r\n"));

    //
    // Need to store pointer to Driver Helper functions, if we already haven't.
    //
    if (this->pOEMHelp == NULL)
    {
        HRESULT hResult;

        //
        // Get Interface to Helper Functions.
        //
        hResult = pIUnknown->QueryInterface(IID_IPrintOemDriverUI, (void** ) &(this->pOEMHelp));

        if(!SUCCEEDED(hResult))
        {
            //
            // Make sure that interface pointer reflects interface query failure.
            //
            this->pOEMHelp = NULL;
        }
    }

    if (this->pOEMHelp)
        return S_OK;
    else
        return S_FALSE;
}

LONG __stdcall IOemCB::GetInfo(
    DWORD  dwMode,
    PVOID  pBuffer,
    DWORD  cbSize,
    PDWORD pcbNeeded)
{
	VERBOSE(DLLTEXT("IOemCB:GetInfo entry.\n\n"));

    if (TextUIGetInfo(dwMode, pBuffer, cbSize, pcbNeeded))
        return S_OK;
    else
        return S_FALSE;
}

LONG __stdcall IOemCB::DevMode(
    DWORD  dwMode,
    POEMDMPARAM pOemDMParam)
{
    VERBOSE(DLLTEXT("IOemCB:DevMode entry.\n\n"));

    UNREFERENCED_PARAMETER(dwMode);
    UNREFERENCED_PARAMETER(pOemDMParam);

    return E_NOTIMPL;

}


LONG __stdcall IOemCB::CommonUIProp(
    DWORD  dwMode,
    POEMCUIPPARAM   pOemCUIPParam)
{
    VERBOSE(DLLTEXT("IOemCB:CommonUIProp entry.\n\n"));

    UNREFERENCED_PARAMETER(dwMode);
    UNREFERENCED_PARAMETER(pOemCUIPParam);

    return E_NOTIMPL;

}


LONG __stdcall IOemCB::DocumentPropertySheets(
    PPROPSHEETUI_INFO   pPSUIInfo,
    LPARAM              lParam)
{
    VERBOSE(DLLTEXT("IOemCB:DocumentPropertySheets entry.\n\n"));

    UNREFERENCED_PARAMETER(pPSUIInfo);
    UNREFERENCED_PARAMETER(lParam);

    return E_NOTIMPL;

}

LONG __stdcall IOemCB::DevicePropertySheets(
    PPROPSHEETUI_INFO   pPSUIInfo,
    LPARAM              lParam)
{
	VERBOSE(DLLTEXT("IOemCB:DevicePropertySheets entry.\n\n"));

    if(TextUIDevicePropertySheets(pPSUIInfo, lParam) == 1)
        return S_OK;
    return   S_FALSE;
}

LONG __stdcall IOemCB::DeviceCapabilities(
                 POEMUIOBJ   poemuiobj,
                 HANDLE      hPrinter,
            _In_ PWSTR       pDeviceName,
                 WORD        wCapability,
                 PVOID       pOutput,
                 PDEVMODE    pPublicDM,
                 PVOID       pOEMDM,
                 DWORD       dwOld,
                 DWORD       *dwResult)
{
    VERBOSE(DLLTEXT("IOemCB:DeviceCapabilities entry.\n"));

    UNREFERENCED_PARAMETER(poemuiobj);
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pDeviceName);
    UNREFERENCED_PARAMETER(wCapability);
    UNREFERENCED_PARAMETER(pOutput);
    UNREFERENCED_PARAMETER(pPublicDM);
    UNREFERENCED_PARAMETER(pOEMDM);
    UNREFERENCED_PARAMETER(dwOld);
    UNREFERENCED_PARAMETER(dwResult);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::DevQueryPrintEx(
    POEMUIOBJ               poemuiobj,
    PDEVQUERYPRINT_INFO     pDQPInfo,
    PDEVMODE                pPublicDM,
    PVOID                   pOEMDM)
{
    VERBOSE(DLLTEXT("IOemCB:DevQueryPrintEx entry.\n\n"));

    UNREFERENCED_PARAMETER(poemuiobj);
    UNREFERENCED_PARAMETER(pDQPInfo);
    UNREFERENCED_PARAMETER(pPublicDM);
    UNREFERENCED_PARAMETER(pOEMDM);

    return E_NOTIMPL;

}

LONG __stdcall IOemCB::UpgradePrinter(
    DWORD   dwLevel,
    PBYTE   pDriverUpgradeInfo)
{
    VERBOSE(DLLTEXT("IOemCB:UpgradePrinter entry.\n\n"));

    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pDriverUpgradeInfo);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::PrinterEvent(
    _In_ PWSTR   pPrinterName,
         INT     iDriverEvent,
         DWORD   dwFlags,
         LPARAM  lParam)
{
    VERBOSE(DLLTEXT("IOemCB:PrinterEvent entry.\n\n"));

    UNREFERENCED_PARAMETER(pPrinterName);
    UNREFERENCED_PARAMETER(iDriverEvent);
    UNREFERENCED_PARAMETER(dwFlags);
    UNREFERENCED_PARAMETER(lParam);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::DriverEvent(
    DWORD   dwDriverEvent,
    DWORD   dwLevel,
    LPBYTE  pDriverInfo,
    LPARAM  lParam)
{
    VERBOSE(DLLTEXT("IOemCB:DriverEvent entry.\n"));
    
    UNREFERENCED_PARAMETER(dwDriverEvent);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pDriverInfo);
    UNREFERENCED_PARAMETER(lParam);

    return E_NOTIMPL;
};

LONG __stdcall IOemCB::QueryColorProfile(
            HANDLE      hPrinter,
            POEMUIOBJ   poemuiobj,
            PDEVMODE    pPublicDM,
            PVOID       pOEMDM,
            ULONG       ulReserved,
            VOID       *pvProfileData,
            ULONG      *pcbProfileData,
            FLONG      *pflProfileData)
{
    VERBOSE(DLLTEXT("IOemCB:QueryColorProfile entry.\n"));
    
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(poemuiobj);
    UNREFERENCED_PARAMETER(pPublicDM);
    UNREFERENCED_PARAMETER(pOEMDM);
    UNREFERENCED_PARAMETER(ulReserved);
    UNREFERENCED_PARAMETER(pvProfileData);
    UNREFERENCED_PARAMETER(pcbProfileData);
    UNREFERENCED_PARAMETER(pflProfileData);

    return E_NOTIMPL;
};


    //
    // UpdateExternalFonts
    //

LONG __stdcall IOemCB::UpdateExternalFonts(
                 HANDLE  hPrinter,
                 HANDLE  hHeap,
            _In_ PWSTR   pwstrCartridges)
{
    VERBOSE(DLLTEXT("IOemCB:UpdateExternalFonts entry.\n\n"));
    
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(hHeap);
    UNREFERENCED_PARAMETER(pwstrCartridges);

    return E_NOTIMPL;
};



HRESULT __stdcall IOemCB::FontInstallerDlgProc(
        HWND    hWnd,
        UINT    usMsg,
        WPARAM  wParam,
        LPARAM  lParam
        )
{
    VERBOSE(DLLTEXT("IOemCB:FontInstallerDlgProc entry.\n"));
    
    UNREFERENCED_PARAMETER(hWnd);
    UNREFERENCED_PARAMETER(usMsg);
    UNREFERENCED_PARAMETER(wParam);
    UNREFERENCED_PARAMETER(lParam);

    return E_NOTIMPL;
};

////////////////////////////////////////////////////////////////////////////////
//
// oem class factory
//
class IOemCF : public IClassFactory
{
public:
    // *** IUnknown methods ***

    STDMETHOD(QueryInterface) (THIS_ REFIID riid, LPVOID FAR* ppvObj);

    STDMETHOD_(ULONG,AddRef)  (THIS);

    // the _At_ tag here tells prefast that once release 
    // is called, the memory should not be considered leaked
    _At_(this, __drv_freesMem(object)) 
    STDMETHOD_(ULONG,Release) (THIS);

    // *** IClassFactory methods ***
    STDMETHOD(CreateInstance) (THIS_
                               LPUNKNOWN pUnkOuter,
                               REFIID riid,
                               LPVOID FAR* ppvObject);
    STDMETHOD(LockServer)     (THIS_ BOOL bLock);


    // Constructor
    IOemCF(): m_cRef(1) { };

    ~IOemCF() { };

protected:
    LONG m_cRef;

};

///////////////////////////////////////////////////////////
//
// Class factory body
//
HRESULT __stdcall IOemCF::QueryInterface(const IID& iid, void** ppv)
{
    if ((iid == IID_IUnknown) || (iid == IID_IClassFactory))
    {
        *ppv = static_cast<IOemCF*>(this) ;
    }
    else
    {
        *ppv = NULL ;
        return E_NOINTERFACE ;
    }
    reinterpret_cast<IUnknown*>(*ppv)->AddRef() ;
    return S_OK ;
}

ULONG __stdcall IOemCF::AddRef()
{
    return InterlockedIncrement(&m_cRef) ;
}

_At_(this, __drv_freesMem(object)) 
ULONG __stdcall IOemCF::Release()
{
   ASSERT( 0 != m_cRef);
   ULONG cRef = InterlockedDecrement(&m_cRef);
   if (0 == cRef)
   {
      delete this;
        
   }
   return cRef;
}

// IClassFactory implementation
HRESULT __stdcall IOemCF::CreateInstance(IUnknown* pUnknownOuter,
                                           const IID& iid,
                                           void** ppv)
{
    VERBOSE(DLLTEXT("Class factory:\t\tCreate component.")) ;

    if (ppv == NULL)
    {
        return E_INVALIDARG;
    }
    *ppv = NULL;

    // Cannot aggregate.
    if (pUnknownOuter != NULL)
    {
        return CLASS_E_NOAGGREGATION ;
    }

    // Create component.
    IOemCB* pOemCB = new IOemCB ;
    if (pOemCB == NULL)
    {
        return E_OUTOFMEMORY ;
    }
    // Get the requested interface.
    HRESULT hr = pOemCB->QueryInterface(iid, ppv) ;

    // Release the IUnknown pointer.
    // (If QueryInterface failed, component will delete itself.)
    pOemCB->Release() ;
    return hr ;
}

// LockServer
HRESULT __stdcall IOemCF::LockServer(BOOL bLock)
{
    if (bLock)
    {
        InterlockedIncrement(&g_cServerLocks) ;
    }
    else
    {
        InterlockedDecrement(&g_cServerLocks) ;
    }
    return S_OK ;
}

///////////////////////////////////////////////////////////


//
// Registration functions
// Testing purpose
//

// Can DLL unload now?
//
STDAPI DllCanUnloadNow()
{
    if ((g_cComponents == 0) && (g_cServerLocks == 0))
    {
        return S_OK ;
    }
    else
    {
        return S_FALSE ;
    }
}

//
// Get class factory
//
STDAPI  DllGetClassObject(
    _In_ REFCLSID clsid, 
    _In_ REFIID iid, 
    _Outptr_ LPVOID* ppv)
{
    //  VERBOSE(DLLTEXT("DllGetClassObject:Create class factory.\n"));

    // Can we create this component?
    if (ppv == NULL)
    {
        return E_INVALIDARG;
    }
    *ppv = NULL;

    if (clsid != CLSID_OEMUI)
    {
        return CLASS_E_CLASSNOTAVAILABLE ;
    }

    // Create class factory.
    IOemCF* pFontCF = new IOemCF ;  // Reference count set to 1
                                         // in constructor
    if (pFontCF == NULL)
    {
        return E_OUTOFMEMORY ;
    }

    // Get requested interface.
    HRESULT hr = pFontCF->QueryInterface(iid, ppv) ;
    pFontCF->Release() ;

    return hr ;
}
