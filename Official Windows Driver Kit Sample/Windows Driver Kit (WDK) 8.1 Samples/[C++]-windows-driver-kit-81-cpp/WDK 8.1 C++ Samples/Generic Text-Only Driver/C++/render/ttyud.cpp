//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1997-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:   TTYUD.cpp
//
//  PURPOSE:  Main file for TTY rendering module.
//

#include "tty.h"
#include <stddef.h>
#include <stdlib.h>
#include <stdarg.h>
#include <winspool.h>
#include <tchar.h>
#include <excpt.h>
#include <PRINTOEM.H>
#include "ttyui.h"
#include "ttyud.h"
#include "debug.h"
#include <STRSAFE.H>

// This indicates to Prefast that this is a usermode driver file.
_Analysis_mode_(_Analysis_code_type_user_driver_);

#define     TTY_INFO_MARGINS  1
#define     TTY_INFO_CODEPAGE  2
#define     TTY_INFO_NUM_UFMS    3
#define     TTY_INFO_UFM_IDS    4

#define     NUM_UFMS    3  //  for internal use only.  use TTY_INFO_NUM_UFMS
                           //  to query number of font sizes supported.

////////////////////////////////////////////////////////
//      INTERNAL PROTOTYPES
////////////////////////////////////////////////////////


BOOL 
TextGetInfo(
    DWORD dwInfo, 
    _Out_writes_bytes_(cbSize) PVOID pBuffer, 
    DWORD cbSize,
    _Out_ PDWORD pcbNeeded
    )
{

    // Validate parameters.
    if( ( (OEMGI_GETSIGNATURE != dwInfo)
          &&
          (OEMGI_GETINTERFACEVERSION != dwInfo)
          &&
          (OEMGI_GETVERSION != dwInfo)
        )
        ||
        (NULL == pcbNeeded)
      )
    {
      VERBOSE(ERRORTEXT("TTYGetInfo() ERROR_INVALID_PARAMETER.\r\n"));

        // Did not write any bytes.
        if(NULL != pcbNeeded)
            *pcbNeeded = 0;

        return FALSE;
    }

    // Need/wrote 4 bytes.
    *pcbNeeded = 4;

    // Validate buffer size.  Minimum size is four bytes.
    if( (NULL == pBuffer)
        ||
        (4 > cbSize)
      )
    {
        // VERBOSE(ERRORTEXT("TTYGetInfo() ERROR_INSUFFICIENT_BUFFER.\r\n"));

        EngSetLastError(ERROR_INSUFFICIENT_BUFFER);
        return FALSE;
    }

    // Write information to buffer.
    switch(dwInfo)
    {
        case OEMGI_GETSIGNATURE:
            *(LPDWORD)pBuffer = OEM_SIGNATURE;
            break;

        case OEMGI_GETINTERFACEVERSION:
            *(LPDWORD)pBuffer = PRINTER_OEMINTF_VERSION;
            break;

        case OEMGI_GETVERSION:
            *(LPDWORD)pBuffer = OEM_VERSION;
            break;
    }

    return TRUE;
}

PDEVOEM 
TextEnablePDEV(
    PDEVOBJ pdevobj, 
    _In_ PWSTR pPrinterName, 
    ULONG cPatterns,
    _In_reads_(cPatterns) HSURF *phsurfPatterns, 
    ULONG cjGdiInfo, 
    _In_reads_bytes_(cjGdiInfo) GDIINFO *pGdiInfo,
    ULONG cjDevInfo, 
    _In_reads_bytes_(cjDevInfo) DEVINFO *pDevInfo, 
    DRVENABLEDATA *pded
    )
{
    VERBOSE(DLLTEXT("TextEnablePDEV() entry.\r\n"));

    UNREFERENCED_PARAMETER(pPrinterName);
    UNREFERENCED_PARAMETER(cPatterns);
    UNREFERENCED_PARAMETER(phsurfPatterns);
    UNREFERENCED_PARAMETER(cjGdiInfo);
    UNREFERENCED_PARAMETER(pGdiInfo);
    UNREFERENCED_PARAMETER(cjDevInfo);
    UNREFERENCED_PARAMETER(pDevInfo);
    UNREFERENCED_PARAMETER(pded);

    PREGSTRUCT  pMyStuff;           //  sebset of pGlobals
    DWORD   dwStatus, cbNeeded, dwType ;
    LPTSTR  pValueName = TEXT("TTY DeviceConfig");
                // these strings must match strings in oemui.cpp - VinitMyStuff()

    pMyStuff = (PREGSTRUCT)MemAlloc(sizeof(REGSTRUCT)) ;
    if(!pMyStuff)
        return ((PDEVOEM)NULL) ;  //  failed.

    pdevobj->pdevOEM = (PDEVOEM)pMyStuff ;

    dwStatus =  EngGetPrinterData(
    pdevobj->hPrinter, // handle of printer object
    pValueName, // address of value name
    &dwType,    // address receiving value type
    (LPBYTE)pMyStuff,
                // address of array of bytes that receives data
    sizeof(REGSTRUCT),  // size, in bytes, of array
    &cbNeeded   // address of variable
            //  with number of bytes retrieved (or required)
    );
    if (dwStatus != ERROR_SUCCESS || pMyStuff->dwVersion != TTYSTRUCT_VER
        ||  dwType !=  REG_BINARY
        ||  cbNeeded != sizeof(REGSTRUCT))
    {
        //  Init secret block with defaults

        pMyStuff->dwVersion = TTYSTRUCT_VER ;
        //  version stamp to avoid incompatible structures.

        pMyStuff->bIsMM = TRUE ;  // default to mm units
        //  read margin values from registry and store into temp RECT

        pMyStuff->iCodePage =  1252 ;  //   keep in sync with oemui.cpp - VinitMyStuff()
        pMyStuff->rcMargin.left  = pMyStuff->rcMargin.top  =
        pMyStuff->rcMargin.right  =  pMyStuff->rcMargin.bottom  = 0 ;
        pMyStuff->BeginJob.dwLen =
        pMyStuff->EndJob.dwLen =
        pMyStuff->PaperSelect.dwLen =
        pMyStuff->FeedSelect.dwLen =
        pMyStuff->Sel_10_cpi.dwLen =
        pMyStuff->Sel_12_cpi.dwLen =
        pMyStuff->Sel_17_cpi.dwLen =
        pMyStuff->Bold_ON.dwLen =
        pMyStuff->Bold_OFF.dwLen =
        pMyStuff->Underline_ON.dwLen =
        pMyStuff->Underline_OFF.dwLen = 0 ;

        // more fields here!
        pMyStuff->dwGlyphBufSiz =
        pMyStuff->dwSpoolBufSiz = 0 ;
        pMyStuff->aubGlyphBuf =
        pMyStuff->aubSpoolBuf  = NULL ;
    }

    return ((PDEVOEM)pMyStuff) ;
}


VOID 
TextDisablePDEV(
    _In_ PDEVOBJ pdevobj
    )
{
    VERBOSE(DLLTEXT("TTYDisablePDEV() entry.\r\n"));

    PREGSTRUCT  pMyStuff;           //  sebset of pGlobals

    pMyStuff = (PREGSTRUCT)pdevobj->pdevOEM ;
    if(pMyStuff->aubGlyphBuf)
        MemFree(pMyStuff->aubGlyphBuf) ;
    if(pMyStuff->aubSpoolBuf)
        MemFree(pMyStuff->aubSpoolBuf) ;
    MemFree(pdevobj->pdevOEM);
}


BOOL
TextEnableDriver(
    DWORD dwOEMintfVersion, 
    DWORD dwSize, 
    PDRVENABLEDATA pded
    )
{
    VERBOSE(DLLTEXT("TextEnableDriver() entry.\r\n"));

    // Validate paramters.
    if(   (PRINTER_OEMINTF_VERSION != dwOEMintfVersion) ||
        //  above check not needed for COM
        (sizeof(DRVENABLEDATA) > dwSize)
        ||
        (NULL == pded)
      )
    {
        //  VERBOSE(ERRORTEXT("TTYEnableDriver() ERROR_INVALID_PARAMETER.\r\n"));

        return FALSE;
    }

    pded->iDriverVersion =  PRINTER_OEMINTF_VERSION ; //   not  DDI_DRIVER_VERSION;
    pded->c = 0;

    return TRUE;
}


// This function exceeds the default stack usage threshold by a few bytes.
#pragma warning(push)
#pragma warning(disable:6262)
BOOL 
TextTTYGetInfo(
    _In_ PDEVOBJ pdevobj,    
    DWORD  dwInfoIndex,
    _Out_writes_bytes_(dwSize) PVOID   pOutBuf,  
    DWORD  dwSize, 
    _In_ DWORD  *pcbNeeded
    )
{
    PREGSTRUCT  pMyStuff;           //  sebset of pGlobals
    REGSTRUCT  MyStuff;   // temp  structure to hold registry data

    pMyStuff = (PREGSTRUCT)pdevobj->pdevOEM ;

    if(!pMyStuff)
    {
        DWORD   dwStatus, cbNeeded, dwType ;
        LPTSTR  pValueName = TEXT("TTY DeviceConfig");
                // these strings must match strings in oemui.cpp - VinitMyStuff()
                //  and TTYEnablePDEV()

        pMyStuff =    &MyStuff;
		
		pMyStuff->dwVersion = 0 ;

        dwStatus =  EngGetPrinterData(
            pdevobj->hPrinter, // handle of printer object
            pValueName, // address of value name
            &dwType,    // address receiving value type
            (LPBYTE)pMyStuff ,
                        // address of array of bytes that receives data
            sizeof(REGSTRUCT),  // size, in bytes, of array
            &cbNeeded   // address of variable
                    //  with number of bytes retrieved (or required)
            );
        if (dwStatus != ERROR_SUCCESS || pMyStuff->dwVersion != TTYSTRUCT_VER
                ||  dwType !=  REG_BINARY
                ||  cbNeeded != sizeof(REGSTRUCT))
        {
            //  set to defaults
            pMyStuff->iCodePage =  1252 ;  //   keep in sync with oemui.cpp - VinitMyStuff()
            pMyStuff->rcMargin.left  = pMyStuff->rcMargin.top  =
            pMyStuff->rcMargin.right  =  pMyStuff->rcMargin.bottom  = 0 ;
        }
    }

    switch  (dwInfoIndex)
    {
        case  TTY_INFO_MARGINS:
            *pcbNeeded = sizeof(RECT) ;
            if(!pOutBuf  ||  dwSize < *pcbNeeded)
                return(FALSE) ;
            *(LPRECT)pOutBuf = pMyStuff->rcMargin ;
            break;
        case  TTY_INFO_CODEPAGE:
            *pcbNeeded = sizeof(INT) ;
            if(!pOutBuf  ||  dwSize < *pcbNeeded)
                return(FALSE) ;
            *(INT *)pOutBuf = pMyStuff->iCodePage ;
            break;
        case  TTY_INFO_NUM_UFMS:
            *pcbNeeded = sizeof(DWORD) ;
            if(!pOutBuf  ||  dwSize < *pcbNeeded)
                return(FALSE) ;
            *(DWORD *)pOutBuf = NUM_UFMS ;  //  Number of resource IDs returned
            //  during a query for TTY_INFO_UFM_IDS
            break;
        case  TTY_INFO_UFM_IDS:
            // return resource IDs of the UFMs for 10,12, 17 cpi fonts.
            *pcbNeeded = NUM_UFMS * sizeof(DWORD) ;
            if(!pOutBuf  ||  dwSize < *pcbNeeded)
                return(FALSE) ;
            switch(pMyStuff->iCodePage)
            {
                
				case (-1):  // codepage 437
                    ((DWORD *)pOutBuf)[0] = 4 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 5;
                    ((DWORD *)pOutBuf)[2] = 6;
                    break;
                case (850):  // codepage 850
                    ((DWORD *)pOutBuf)[0] = 7;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 8;
                    ((DWORD *)pOutBuf)[2] = 9;
                    break;

				case (-3):  // codepage 863
                    ((DWORD *)pOutBuf)[0] = 7;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 8;
                    ((DWORD *)pOutBuf)[2] = 9;
                    break;

                case (-17):  // codepage 932
                    ((DWORD *)pOutBuf)[0] = 13 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 14 ;
                    ((DWORD *)pOutBuf)[2] = 15 ;
                    break;
                case (-16):  // codepage 936
                    ((DWORD *)pOutBuf)[0] = 16 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 17 ;
                    ((DWORD *)pOutBuf)[2] = 18 ;
                    break;
                case (-18):  // codepage 949
                    ((DWORD *)pOutBuf)[0] = 19 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 20 ;
                    ((DWORD *)pOutBuf)[2] = 21 ;
                    break;
                case (-10):  // codepage 950
                    ((DWORD *)pOutBuf)[0] = 22 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 23 ;
                    ((DWORD *)pOutBuf)[2] = 24 ;
                    break;
				case (1250):  // codepage 1250
                    ((DWORD *)pOutBuf)[0] = 25 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 26;
                    ((DWORD *)pOutBuf)[2] = 27;
                    break;
                case (1251):  // codepage 1251
                    ((DWORD *)pOutBuf)[0] = 28;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 29;
                    ((DWORD *)pOutBuf)[2] = 30;
                    break;
                case (1252):  // codepage 1252
                    ((DWORD *)pOutBuf)[0] = 31;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 32;
                    ((DWORD *)pOutBuf)[2] = 33;
                    break;
                case (1253):  // codepage 1253
                    ((DWORD *)pOutBuf)[0] = 34 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 35;
                    ((DWORD *)pOutBuf)[2] = 36;
                    break;
                case (1254):  // codepage 1254
                    ((DWORD *)pOutBuf)[0] = 37 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 38;
                    ((DWORD *)pOutBuf)[2] = 39;
                    break;
                case (852):	// codepage 852
					((DWORD *)pOutBuf)[0] = 40 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 41;
                    ((DWORD *)pOutBuf)[2] = 42;
		    
					break;
				case (857):	// codepage 857
					((DWORD *)pOutBuf)[0] = 43 ;           //  see  tty\rc\tty.rc for resID assignments
                    ((DWORD *)pOutBuf)[1] = 44;
                    ((DWORD *)pOutBuf)[2] = 45;
		    
					break;
                default:
                    ((DWORD *)pOutBuf)[0] = 1 ;  //  '10 cpi'
                    ((DWORD *)pOutBuf)[1] = 2 ;  //  '12 cpi'
                    ((DWORD *)pOutBuf)[2] = 3 ;  //  '17 cpi'
                    break;
            }
            break;
        default:
            *pcbNeeded  = 0 ;  // no data availible
            return(FALSE) ;   // unsupported index
    }
    return(TRUE);
}
#pragma warning(pop)
