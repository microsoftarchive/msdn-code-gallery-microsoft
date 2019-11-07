//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1997-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:   comoem.cpp
//
//  PURPOSE:  Implementation of text rendering plug-in.
//


#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0500
#endif

#include <tty.h>
#include <printoem.h>
#include <prntfont.h>
#include "..\inc\name.h"
#include <initguid.h>
#include <prcomoem.h>
#include <assert.h>
#include "oemcom.h"
#include "ttyui.h"
#include "ttyud.h"
#include "debug.h"
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
    VERBOSE(DLLTEXT("IOemCB: QueryInterface entry\n"));
    if (iid == IID_IUnknown)
    {
        *ppv = static_cast<IUnknown*>(this);
        VERBOSE(DLLTEXT("IOemCB:Return pointer to IUnknown.\n")) ;
    }
    else if (iid == IID_IPrintOemUni2)
    {
        *ppv = static_cast<IPrintOemUni2*>(this) ;
        VERBOSE(DLLTEXT("IOemCB:Return pointer to IPrintOemUni.\n")) ;
    }
    else
    {
        *ppv = NULL ;
        WARNING(DLLTEXT("IOemCB:Return NULL.\n")) ;
        return E_NOINTERFACE ;
    }
    reinterpret_cast<IUnknown*>(*ppv)->AddRef() ;
    return S_OK ;
}

ULONG __stdcall IOemCB::AddRef()
{
    VERBOSE(DLLTEXT("IOemCB::AddRef() entry.\r\n"));
    return InterlockedIncrement(&m_cRef) ;
}

_At_(this, __drv_freesMem(object)) 
ULONG __stdcall IOemCB::Release()
{
   VERBOSE(DLLTEXT("IOemCB::Release() entry.\r\n"));
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

    assert(0 == m_cRef) ;
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
        hResult = pIUnknown->QueryInterface(IID_IPrintOemDriverUni, (void** ) &(this->pOEMHelp));

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
        return E_FAIL;
}


LONG __stdcall IOemCB::EnableDriver(DWORD          dwDriverVersion,
                                    DWORD          cbSize,
                                    PDRVENABLEDATA pded)
{
    VERBOSE(DLLTEXT("IOemCB::EnableDriver() entry.\r\n"));

    if(TextEnableDriver(dwDriverVersion, cbSize, pded) )
        return S_OK;
    return E_FAIL;
}

LONG __stdcall IOemCB::DisableDriver(VOID)
{
    VERBOSE(DLLTEXT("IOemCB::DisaleDriver() entry.\r\n"));

    if (this->pOEMHelp)
    {
        this->pOEMHelp->Release();
        this->pOEMHelp = NULL ;
    }
    return S_OK;
}


// the following OEM function is called by Unidrv to provide
// the OEM dll with an interface it can use to access Unidrv's
// helper functions like WriteSpoolBuf etc.
//  warning: calling this more per IOemCB object than once will result
//  in mismatched reference counts.


LONG __stdcall IOemCB::EnablePDEV(
         PDEVOBJ         pdevobj,
    _In_ PWSTR           pPrinterName,
         ULONG           cPatterns,
         HSURF          *phsurfPatterns,
         ULONG           cjGdiInfo,
         GDIINFO        *pGdiInfo,
         ULONG           cjDevInfo,
         DEVINFO        *pDevInfo,
         DRVENABLEDATA  *pded,
     OUT PDEVOEM    *ppDevOem)
{
    VERBOSE(DLLTEXT("IOemCB::EnablePDEV() entry.\r\n"));

    if((*ppDevOem = TextEnablePDEV(pdevobj, pPrinterName, cPatterns,
            phsurfPatterns, cjGdiInfo, pGdiInfo, cjDevInfo, pDevInfo, pded))
        != NULL)
        return S_OK;
    return E_FAIL;

}

LONG __stdcall IOemCB::ResetPDEV(
    PDEVOBJ         pdevobjOld,
    PDEVOBJ        pdevobjNew)
{
    UNREFERENCED_PARAMETER(pdevobjOld);
    UNREFERENCED_PARAMETER(pdevobjNew);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::DisablePDEV(
    PDEVOBJ         pdevobj)
{
    VERBOSE(DLLTEXT("IOemCB::DisablePDEV() entry.\r\n"));

    TextDisablePDEV(pdevobj) ;
    return S_OK;
};

LONG __stdcall IOemCB::GetInfo (
    DWORD   dwMode,
    PVOID   pBuffer,
    DWORD   cbSize,
    PDWORD  pcbNeeded)
{
    VERBOSE(DLLTEXT("IOemCB::GetInfo() entry.\r\n"));

    if (TextGetInfo(dwMode, pBuffer, cbSize, pcbNeeded))
        return S_OK;
    else
        return E_FAIL;
}


LONG __stdcall IOemCB::GetImplementedMethod(
    _In_ PSTR pMethodName)
{

    LONG lReturn = FALSE;
    VERBOSE(DLLTEXT("IOemCB::GetImplementedMethod() entry.\r\n"));
    VERBOSE(DLLTEXT("        Function:%s:"),pMethodName);

    if (pMethodName == NULL)
    {
        lReturn = FALSE;
    }
    else
    {
        switch (*pMethodName)
        {
            case (WCHAR)'C':
                if (!strcmp(pstrCommandCallback, pMethodName))
                    lReturn = TRUE;
                break;

            case (WCHAR)'D':
                if (!strcmp(pstrDisableDriver, pMethodName))
                    lReturn = TRUE;
                else if (!strcmp(pstrDisablePDEV, pMethodName))
                    lReturn = TRUE;
                else if (!strcmp(pstrDownloadCharGlyph, pMethodName))
                    lReturn = TRUE;
                break;

            case (WCHAR)'E':
                if (!strcmp(pstrEnableDriver, pMethodName))
                    lReturn = TRUE;
                else if (!strcmp(pstrEnablePDEV, pMethodName))
                    lReturn = TRUE;
                break;

            case (WCHAR)'G':
                if (!strcmp(pstrGetInfo, pMethodName))
                    lReturn = TRUE;
                break;

            case (WCHAR)'O':
                if (!strcmp(pstrOutputCharStr, pMethodName))
                   lReturn = TRUE;
                break;

            case (WCHAR)'S':
                if (!strcmp(pstrSendFontCmd, pMethodName))
                           lReturn = TRUE;
                break;

            case (WCHAR)'T':
                if (!strcmp(pstrTTYGetInfo, pMethodName))
                    lReturn = TRUE;
                break;
            default:
                lReturn = FALSE;
                break;
        }
    }

    if (lReturn)
    {
        VERBOSE(__TEXT("Supported\r\n"));
        return S_OK;
    }
    else
    {
        VERBOSE(__TEXT("NOT supported\r\n"));
        return S_FALSE;
    }
}

LONG __stdcall IOemCB::DevMode(
    DWORD       dwMode,
    POEMDMPARAM pOemDMParam)
{
    VERBOSE(DLLTEXT("IOemCB::DevMode() entry.\r\n"));

    UNREFERENCED_PARAMETER(dwMode);
    UNREFERENCED_PARAMETER(pOemDMParam);

    return E_NOTIMPL;
}


LONG __stdcall IOemCB::CommandCallback(
    PDEVOBJ     pdevobj,
    DWORD       dwCallbackID,
    DWORD       dwCount,
    PDWORD      pdwParams,
    OUT INT     *piResult)
{
    VERBOSE(DLLTEXT("IOemCB::CommandCallback() entry.\r\n"));
    VERBOSE(DLLTEXT("        dwCallbackID = %d\r\n"), dwCallbackID);

    UNREFERENCED_PARAMETER(dwCount);
    UNREFERENCED_PARAMETER(pdwParams);

    DWORD dwResult;
    PREGSTRUCT  pMyStuff;           //  sebset of pGlobals
    PCMDSTR      pSelectCmd = NULL ;   // points to one of the command structures
                                                //  in pMyStuff.

    *piResult =  0 ;   //  for all non movement commands
    pMyStuff = (PREGSTRUCT)pdevobj->pdevOEM ;

    switch(dwCallbackID)
    {
        case  TTY_CB_BEGINJOB:
            pSelectCmd = &pMyStuff->BeginJob ;
            break;
        case  TTY_CB_ENDJOB:
            pSelectCmd = &pMyStuff->EndJob ;
            break;
        case  TTY_CB_PAPERSELECT:
            pSelectCmd = &pMyStuff->PaperSelect ;
            break;
        case  TTY_CB_FEEDSELECT:
            pSelectCmd = &pMyStuff->FeedSelect ;
            break;
        case  TTY_CB_BOLD_ON:
            pSelectCmd = &pMyStuff->Bold_ON ;
            break;
        case  TTY_CB_BOLD_OFF:
            pSelectCmd = &pMyStuff->Bold_OFF ;
            break;
        case  TTY_CB_UNDERLINE_ON:
            pSelectCmd = &pMyStuff->Underline_ON ;
            break;
        case  TTY_CB_UNDERLINE_OFF:
            pSelectCmd = &pMyStuff->Underline_OFF ;
            break;

        default:
            return S_OK;
    }

    if(pSelectCmd)
         pOEMHelp->DrvWriteSpoolBuf(pdevobj, pSelectCmd->strCmd, pSelectCmd->dwLen,
                &dwResult);

    return S_OK;
}

LONG __stdcall IOemCB::ImageProcessing(
    PDEVOBJ             pdevobj,
    PBYTE               pSrcBitmap,
    PBITMAPINFOHEADER   pBitmapInfoHeader,
    PBYTE               pColorTable,
    DWORD               dwCallbackID,
    PIPPARAMS           pIPParams,
    OUT PBYTE           *ppbResult)
{
    VERBOSE(DLLTEXT("IOemCB::ImageProcessing() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pSrcBitmap);
    UNREFERENCED_PARAMETER(pBitmapInfoHeader);
    UNREFERENCED_PARAMETER(pColorTable);
    UNREFERENCED_PARAMETER(dwCallbackID);
    UNREFERENCED_PARAMETER(pIPParams);
    UNREFERENCED_PARAMETER(ppbResult);

    return E_NOTIMPL ;
}

LONG __stdcall IOemCB::FilterGraphics(
    PDEVOBJ     pdevobj,
    PBYTE       pBuf,
    DWORD       dwLen)
{
    VERBOSE(DLLTEXT("IOemCB::FilterGraphis() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pBuf);
    UNREFERENCED_PARAMETER(dwLen);

    return E_NOTIMPL ;
}

LONG __stdcall IOemCB::Compression(
    PDEVOBJ     pdevobj,
    PBYTE       pInBuf,
    PBYTE       pOutBuf,
    DWORD       dwInLen,
    DWORD       dwOutLen,
    OUT INT     *piResult)
{
    VERBOSE(DLLTEXT("IOemCB::Compression() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pInBuf);
    UNREFERENCED_PARAMETER(pOutBuf);
    UNREFERENCED_PARAMETER(dwInLen);
    UNREFERENCED_PARAMETER(dwOutLen);
    UNREFERENCED_PARAMETER(piResult);

    return E_NOTIMPL;
}


LONG __stdcall IOemCB::HalftonePattern(
    PDEVOBJ     pdevobj,
    PBYTE       pHTPattern,
    DWORD       dwHTPatternX,
    DWORD       dwHTPatternY,
    DWORD       dwHTNumPatterns,
    DWORD       dwCallbackID,
    PBYTE       pResource,
    DWORD       dwResourceSize)
{
    VERBOSE(DLLTEXT("IOemCB::HalftonePattern() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pHTPattern);
    UNREFERENCED_PARAMETER(dwHTPatternX);
    UNREFERENCED_PARAMETER(dwHTPatternY);
    UNREFERENCED_PARAMETER(dwHTNumPatterns);
    UNREFERENCED_PARAMETER(dwCallbackID);
    UNREFERENCED_PARAMETER(pResource);
    UNREFERENCED_PARAMETER(dwResourceSize);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::MemoryUsage(
    PDEVOBJ         pdevobj,
    POEMMEMORYUSAGE pMemoryUsage)
{
    VERBOSE(DLLTEXT("IOemCB::MemoryUsage() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pMemoryUsage);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::DownloadFontHeader(
    PDEVOBJ     pdevobj,
    PUNIFONTOBJ pUFObj,
    OUT DWORD   *pdwResult)
{
    VERBOSE(DLLTEXT("IOemCB::DownloadFontHeader() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pUFObj);
    UNREFERENCED_PARAMETER(pdwResult);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::DownloadCharGlyph(
    PDEVOBJ     pdevobj,
    PUNIFONTOBJ pUFObj,
    HGLYPH      hGlyph,
    PDWORD      pdwWidth,
    OUT DWORD   *pdwResult)
{
    VERBOSE(DLLTEXT("IOemCB::DownloadCharGlyph() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pUFObj);
    UNREFERENCED_PARAMETER(hGlyph);
    UNREFERENCED_PARAMETER(pdwWidth);
    UNREFERENCED_PARAMETER(pdwResult);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::TTDownloadMethod(
    PDEVOBJ     pdevobj,
    PUNIFONTOBJ pUFObj,
    OUT DWORD   *pdwResult)
{
    VERBOSE(DLLTEXT("IOemCB::TTDownloadMethod() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pUFObj);
    UNREFERENCED_PARAMETER(pdwResult);

    return E_NOTIMPL;
}



LONG __stdcall IOemCB::OutputCharStr(
    PDEVOBJ     pdevobj,
    PUNIFONTOBJ pUFObj,
    DWORD       dwType,
    DWORD       dwCount,
    PVOID       pGlyph)
{
    GETINFO_GLYPHSTRING GStr;
    PTRANSDATA pTrans = NULL;
    DWORD  dwI, dwGetInfo, dwResult, dwGlyphBufSiz, dwSpoolBufSiz, dwDst;
    PREGSTRUCT pMyStuff;

    VERBOSE(DLLTEXT("OEMOutputCharStr() entry.\r\n"));

    pMyStuff = (PREGSTRUCT)pdevobj->pdevOEM ;

    //  is the glyph buffers cache large enough?

    dwGlyphBufSiz = dwCount * sizeof(TRANSDATA);

    if(dwGlyphBufSiz > pMyStuff->dwGlyphBufSiz)   //  need to realloc mem.
    {
        if(pMyStuff->aubGlyphBuf)
        {
            MemFree(pMyStuff->aubGlyphBuf) ;
            pMyStuff->dwGlyphBufSiz = 0 ;
        }
        if((pMyStuff->aubGlyphBuf = (PBYTE)MemAlloc(dwGlyphBufSiz))
            != NULL)
            pMyStuff->dwGlyphBufSiz = dwGlyphBufSiz ;
        else
            return(E_FAIL);   //  unable to alloc needed buffers.
    }


    dwSpoolBufSiz = dwCount * sizeof(WORD);  // assume worst case.

    if(dwSpoolBufSiz > pMyStuff->dwSpoolBufSiz)   //  need to realloc mem.
    {
        if(pMyStuff->aubSpoolBuf)
        {
            MemFree(pMyStuff->aubSpoolBuf) ;
            pMyStuff->dwSpoolBufSiz = 0 ;
        }
        if((pMyStuff->aubSpoolBuf = (PBYTE)MemAlloc(dwSpoolBufSiz))
            != NULL)
            pMyStuff->dwSpoolBufSiz = dwSpoolBufSiz ;
        else
            return(E_FAIL);   //  unable to alloc needed buffers.
    }

    if(dwType != TYPE_GLYPHHANDLE)
        return(E_FAIL);  // E_FAIL  can only handle device fonts.

        GStr.dwSize    = sizeof(GETINFO_GLYPHSTRING);
        GStr.dwCount   = dwCount;
        GStr.dwTypeIn  = dwType;
        GStr.pGlyphIn  = pGlyph;
        GStr.dwTypeOut = TYPE_TRANSDATA;
        GStr.pGlyphOut = pMyStuff->aubGlyphBuf ;
        GStr.dwGlyphOutSize = dwGlyphBufSiz;   // instead of pMyStuff->dwGlyphBufSiz, which cause AV
        dwGetInfo = GStr.dwSize;
        if (!pUFObj->pfnGetInfo(pUFObj, UFO_GETINFO_GLYPHSTRING, &GStr,
            dwGetInfo, &dwGetInfo))
        {
            WARNING(DLLTEXT("UNIFONTOBJ_GetInfo:UFO_GETINFO_GLYPHSTRING failed.\r\n"));
            return(E_FAIL);
        }


        pTrans = (PTRANSDATA)pMyStuff->aubGlyphBuf;

        for (dwDst =  dwI = 0 ; dwI < dwCount; dwI++, pTrans++)
        {
            VERBOSE(DLLTEXT("TYPE_TRANSDATA:ubCodePageID:0x%x\n"),pTrans->ubCodePageID);
            VERBOSE(DLLTEXT("TYPE_TRANSDATA:ubType:0x%x\n"),pTrans->ubType);
            switch (pTrans->ubType & MTYPE_FORMAT_MASK)
            {
                case MTYPE_DIRECT:
                    pMyStuff->aubSpoolBuf[dwDst++] =  pTrans->uCode.ubCode ;
                    break;
                case MTYPE_PAIRED:
                    VERBOSE(DLLTEXT("TYPE_TRANSDATA:ubPairs:0x%x\n"),*(PWORD)(pTrans->uCode.ubPairs));
                    pMyStuff->aubSpoolBuf[dwDst++] =  pTrans->uCode.ubPairs[0] ;
                    pMyStuff->aubSpoolBuf[dwDst++] =  pTrans->uCode.ubPairs[1] ;
                    break;
                default:
                    return (E_FAIL);  // no other MTYPE is supported.
                    break;
            }
        }


        // send dst bytes to printer.
        pOEMHelp->DrvWriteSpoolBuf(pdevobj, pMyStuff->aubSpoolBuf, dwDst,
           &dwResult);

        if(dwResult == dwDst)
            return S_OK;
        return E_FAIL ;
}


LONG __stdcall IOemCB::SendFontCmd(
    PDEVOBJ      pdevobj,
    PUNIFONTOBJ  pUFObj,
    PFINVOCATION pFInv)
{
    VERBOSE(DLLTEXT("IOemCB::SendFontCmd() entry.\r\n"));

    UNREFERENCED_PARAMETER(pUFObj);

    PREGSTRUCT  pMyStuff;           //  sebset of pGlobals
    PCMDSTR      pSelectCmd ;   // points to one of the font selection
            // command structures in pMyStuff.
    DWORD dwResult;

     //  I expect pFInv to contain either "10", "12" or "17"

    pMyStuff = (PREGSTRUCT)pdevobj->pdevOEM ;

    if(pFInv->dwCount == 2  &&  pFInv->pubCommand[0] == '1')
    {
        if (pFInv->pubCommand[1] == '2')
            pSelectCmd = &pMyStuff->Sel_12_cpi ;
        else if (pFInv->pubCommand[1] == '7')
            pSelectCmd = &pMyStuff->Sel_17_cpi ;
        else if (pFInv->pubCommand[1] == '0')
            pSelectCmd = &pMyStuff->Sel_10_cpi ;
        else
            return S_OK;    //  maybe asking to deselect a font.  Don't do anything!
    }
    else
        return S_OK;    //  maybe asking to deselect a font.  Don't do anything!

     pOEMHelp->DrvWriteSpoolBuf(pdevobj, pSelectCmd->strCmd, pSelectCmd->dwLen,
                &dwResult);

    return S_OK;
}



LONG __stdcall IOemCB::DriverDMS(
    PVOID   pDevObj,
    PVOID   pBuffer,
    DWORD   cbSize,
    PDWORD  pcbNeeded)
{
    VERBOSE(DLLTEXT("IOemCB::DriverDMS() entry.\r\n"));

    UNREFERENCED_PARAMETER(pDevObj);
    UNREFERENCED_PARAMETER(pBuffer);
    UNREFERENCED_PARAMETER(cbSize);
    UNREFERENCED_PARAMETER(pcbNeeded);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::TextOutAsBitmap(
    SURFOBJ    *pso,
    STROBJ     *pstro,
    FONTOBJ    *pfo,
    CLIPOBJ    *pco,
    RECTL      *prclExtra,
    RECTL      *prclOpaque,
    BRUSHOBJ   *pboFore,
    BRUSHOBJ   *pboOpaque,
    POINTL     *pptlOrg,
    MIX         mix)
{
    VERBOSE(DLLTEXT("IOemCB::TextOutAsBitmap() entry.\r\n"));

    UNREFERENCED_PARAMETER(pso);
    UNREFERENCED_PARAMETER(pstro);
    UNREFERENCED_PARAMETER(pfo);
    UNREFERENCED_PARAMETER(pco);
    UNREFERENCED_PARAMETER(prclExtra);
    UNREFERENCED_PARAMETER(prclOpaque);
    UNREFERENCED_PARAMETER(pboFore);
    UNREFERENCED_PARAMETER(pboOpaque);
    UNREFERENCED_PARAMETER(pptlOrg);
    UNREFERENCED_PARAMETER(mix);

    return E_NOTIMPL;
}

LONG __stdcall IOemCB::TTYGetInfo(
    PDEVOBJ     pdevobj,
    DWORD       dwInfoIndex,
    PVOID       pOutputBuf,
    DWORD       dwSize,
    DWORD       *pcbcNeeded
    )
{
    VERBOSE(DLLTEXT("IOemCB::TTYGetInfo() entry.\r\n"));

    if(TextTTYGetInfo( pdevobj,      dwInfoIndex,
           pOutputBuf,    dwSize,   pcbcNeeded) )
        return S_OK;
    return E_FAIL ;
}


LONG __stdcall  IOemCB:: WritePrinter (  PDEVOBJ    pdevobj,
                                      PVOID      pBuf,
                                     DWORD      cbBuffer,
                                    PDWORD     pcbWritten)
{
    VERBOSE(DLLTEXT("IOemCB::WritePrinter() entry.\r\n"));

    UNREFERENCED_PARAMETER(pdevobj);
    UNREFERENCED_PARAMETER(pBuf);
    UNREFERENCED_PARAMETER(cbBuffer);
    UNREFERENCED_PARAMETER(pcbWritten);

    return E_NOTIMPL;
}


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
    VERBOSE(DLLTEXT("DllGetClassObject:\tCreate class factory.")) ;

    if (ppv == NULL)
    {
        return E_INVALIDARG;
    }
    *ppv = NULL;

    // Can we create this component?
    if (clsid != CLSID_OEMRENDER)
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

///////////////////////////////////////////////////////////
//
// DLL module information
//
BOOL APIENTRY DllMain(HANDLE hModule,
                      DWORD dwReason,
                      void* lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);

    if (dwReason == DLL_PROCESS_ATTACH)
    {
        g_hModule = (HMODULE)hModule ;
    }

    return true;
}
