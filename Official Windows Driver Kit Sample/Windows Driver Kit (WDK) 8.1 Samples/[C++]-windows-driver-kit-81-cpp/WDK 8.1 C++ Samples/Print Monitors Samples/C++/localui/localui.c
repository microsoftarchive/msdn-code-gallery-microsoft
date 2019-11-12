/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    localui.c

--*/
#include "precomp.h"
#pragma hdrstop

#include "localui.h"

HINSTANCE hInst;

WCHAR szCOM[]     = L"COM";
WCHAR szLPT[]     = L"LPT";

MONITORUI MonitorUI =
{
    sizeof(MONITORUI),
    AddPortUI,
    ConfigurePortUI,
    DeletePortUI
};

extern WCHAR szWindows[];
extern WCHAR szINIKey_TransmissionRetryTimeout[];

BOOL
DllMain(
    HINSTANCE hModule,
    DWORD     dwReason,
    LPVOID    lpRes)
{
    INITCOMMONCONTROLSEX icc;

    switch (dwReason) {

    case DLL_PROCESS_ATTACH:
        hInst = hModule;


        DisableThreadLibraryCalls(hModule);

        //
        // Initialize the common controls, needed for fusion applications
        // because standard controls were moved to comctl32.dll
        //
        InitCommonControls();

        icc.dwSize = sizeof(INITCOMMONCONTROLSEX);
        icc.dwICC = ICC_STANDARD_CLASSES;
        InitCommonControlsEx(&icc);

        return TRUE;

    case DLL_PROCESS_DETACH:
        return TRUE;
    }

    UNREFERENCED_PARAMETER( lpRes );
    return TRUE;
}

PMONITORUI
InitializePrintMonitorUI(
    VOID
    )
{
    return &MonitorUI;
}

