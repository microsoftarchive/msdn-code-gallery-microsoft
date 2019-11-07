/*++

Copyright (c) 1990-2003  Microsoft Corporation


Module Name:

    cpsui.h


Abstract:

    This module contains defines for cpsui.c


Author:

    03-Nov-1995 Fri 13:44:30 created  


[Environment:]

    GDI Device Driver - Plotter.


[Notes:]


Revision History:


--*/



typedef struct _EXTRAINFO {
    DWORD   Size;
    LPBYTE  pData;
    } EXTRAINFO, *PEXTRAINFO;

BOOL
CreateOPTTYPE(
    PPRINTERINFO    pPI,
    POPTITEM        pOptItem,
    POIDATA         pOIData,
    UINT            cLBCBItem,
    PEXTRAINFO      pExtraInfo
    );

POPTITEM
FindOptItem(
    POPTITEM    pOptItem,
    UINT        cOptItem,
    BYTE        DMPubID
    );

_Success_(return >= 0)
LONG
CallCommonPropertySheetUI(
                HWND            hWndOwner,
                PFNPROPSHEETUI  pfnPropSheetUI,
                LPARAM          lParam,
    _Out_opt_   LPDWORD         pResult
    );

LONG
DefCommonUIFunc(
    PPROPSHEETUI_INFO   pPSUIInfo,
    LPARAM              lParam,
    PPRINTERINFO        pPI,
    LONG_PTR            lData
    );
