/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

   util.c

--*/

#include "local.h"

LPWSTR
AllocSplStr(
    LPCWSTR pszSource
    )
{
    PWSTR pszRet = NULL;

    if (pszSource)
    {
        size_t  len = 0;
        HRESULT hr = StringCchLength(pszSource, STRSAFE_MAX_CCH, &len);

        if (SUCCEEDED(hr))
        {
            pszRet = (PWSTR)AllocSplMem((len + 1) * sizeof(WCHAR));

            if (pszRet)
            {
                hr = StringCchCopy(pszRet, len + 1, pszSource);

                if (FAILED(hr))
                {
                    FreeSplMem(pszRet);

                    pszRet = NULL;
                }
            }
        }
    }

    return pszRet;
}

LPVOID
AllocSplMem(
    SIZE_T cbAlloc
    )
{
    //
    // LPTR combines LMEM_FIXED and LMEM_ZEROINIT
    //
    return LocalAlloc(LPTR, cbAlloc);
}

VOID
FreeSplMem(
    _In_opt_ VOID *pMem
    )
{
    LocalFree(pMem);
}

VOID
FreeSplStr(
    _In_opt_ PWSTR lpStr
    )
{
    LocalFree(lpStr);
}

typedef BOOL    (WINAPI *pfnDrvQueryJobAttributes)(HANDLE, PDEVMODE, DWORD, LPBYTE);
typedef HMODULE (WINAPI *pfnLoadPrinterDriver)(HANDLE);
typedef BOOL    (WINAPI *pfnRefCntUnloadDriver)(HANDLE, BOOL);

//
// PrintProcGetJobAttributesEx function may be called multiple times during printing
// of a job.
//
_Success_(return)
BOOL
PrintProcGetJobAttributesEx(
    _In_  LPCWSTR           pPrinterName,
    _In_  LPDEVMODEW        pDevmode,
    _Out_ PATTRIBUTE_INFO_4 pAttributeInfo
    )
{
    static pfnLoadPrinterDriver  pfLoadDriver         = NULL;
    static pfnRefCntUnloadDriver pfRefCntUnloadDriver = NULL;

    HMODULE                  hDrvLib        = NULL;
    HANDLE                   hDrvPrinter    = NULL;
    BOOL                     bReturn        = FALSE;
    pfnDrvQueryJobAttributes pfDrvQueryJobAttributes = NULL;
    DWORD                    dwSuccessfulQueryLevel  = 0;

    if (!pfLoadDriver || !pfRefCntUnloadDriver)
    {
        HMODULE hWinspool = GetModuleHandle(L"winspool.drv");

        if (!hWinspool)
        {
            return FALSE;
        }

        pfLoadDriver         = (pfnLoadPrinterDriver)GetProcAddress(hWinspool, (LPCSTR)212);
        pfRefCntUnloadDriver = (pfnRefCntUnloadDriver)GetProcAddress(hWinspool, (LPCSTR)214);

        if (!pfLoadDriver || !pfRefCntUnloadDriver)
        {
            goto CleanUp;
        }
    }

    // Get a client side printer handle to pass to the driver
    if (!OpenPrinter((PWSTR)pPrinterName, &hDrvPrinter, NULL))
    {
        goto CleanUp;
    }

    hDrvLib = (*pfLoadDriver)(hDrvPrinter);

    // Load the driver config file
    if (!hDrvLib)
    {
        goto CleanUp;
    }

    pfDrvQueryJobAttributes = (pfnDrvQueryJobAttributes)GetProcAddress(hDrvLib, "DrvQueryJobAttributes");

    // Call the DrvQueryJobAtributes function in the driver
    if (pfDrvQueryJobAttributes)
    {
        bReturn = ((* pfDrvQueryJobAttributes)(hDrvPrinter, pDevmode, 4, (LPBYTE) pAttributeInfo));

        if (bReturn)
        {
            dwSuccessfulQueryLevel = 4; // No default to be filled.
        }
        else
        {
            bReturn = ((* pfDrvQueryJobAttributes)(hDrvPrinter, pDevmode, 3, (LPBYTE) pAttributeInfo));

            if (bReturn )
            {
                dwSuccessfulQueryLevel = 3;
            }
            else
            {
                bReturn = ((* pfDrvQueryJobAttributes) (hDrvPrinter, pDevmode, 2, (LPBYTE) pAttributeInfo));

                if (bReturn )
                {
                    dwSuccessfulQueryLevel = 2;
                }
                else
                {
                    bReturn = ((* pfDrvQueryJobAttributes) (hDrvPrinter, pDevmode, 1, (LPBYTE) pAttributeInfo)) ;

                    if (bReturn )
                    {
                        dwSuccessfulQueryLevel = 1;
                    }
                }
            }
        }
    }

    if (dwSuccessfulQueryLevel < 1 )
    {
        pAttributeInfo->dwJobNumberOfPagesPerSide = 1;
        pAttributeInfo->dwDrvNumberOfPagesPerSide = 1;
        pAttributeInfo->dwNupBorderFlags          = 0;
        pAttributeInfo->dwJobPageOrderFlags       = 0;
        pAttributeInfo->dwDrvPageOrderFlags       = 0;
        pAttributeInfo->dwJobNumberOfCopies       = pDevmode->dmCopies;
        pAttributeInfo->dwDrvNumberOfCopies       = pDevmode->dmCopies;
    }

    if (dwSuccessfulQueryLevel < 2 )
    {
        pAttributeInfo->dwColorOptimization = 0;
    }

    if (dwSuccessfulQueryLevel < 3 )
    {
        pAttributeInfo->dmPrintQuality = pDevmode->dmPrintQuality;
        pAttributeInfo->dmYResolution  = pDevmode->dmYResolution;
    }

    if (dwSuccessfulQueryLevel < 4 )
    {
        pAttributeInfo->dwNupDirection      = RIGHT_THEN_DOWN;
        pAttributeInfo->dwBookletFlags      = BOOKLET_EDGE_LEFT; //Default booklet
        pAttributeInfo->dwDuplexFlags       = 0;
        pAttributeInfo->dwScalingPercentX   = 100; // Scaling percentage.
        pAttributeInfo->dwScalingPercentY   = 100; // Scaling percentage.
    }

    bReturn = TRUE;

CleanUp:

    if (hDrvPrinter)
    {
        ClosePrinter(hDrvPrinter);
    }

    if (hDrvLib)
    {
        (*pfRefCntUnloadDriver)(hDrvLib, TRUE);
    }

    return bReturn;
}



