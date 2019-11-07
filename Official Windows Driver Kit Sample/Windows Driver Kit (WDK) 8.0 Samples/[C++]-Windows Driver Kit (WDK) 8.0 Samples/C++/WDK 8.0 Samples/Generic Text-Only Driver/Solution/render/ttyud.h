//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1997-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:   TTYUD.H
//
//  PURPOSE:    Define common data types, and external function prototypes
//              for TTYUD rendering Module.
//
#pragma once


////////////////////////////////////////////////////////
//      Text Driver Function Prototypes
////////////////////////////////////////////////////////

BOOL 
TextGetInfo(
    DWORD dwInfo, 
    _Out_writes_bytes_(cbSize) PVOID pBuffer, 
    DWORD cbSize,
    _Out_ PDWORD pcbNeeded
    );

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
    );

VOID 
TextDisablePDEV(
    _In_ PDEVOBJ pdevobj
    );

BOOL
TextEnableDriver(
    DWORD dwOEMintfVersion, 
    DWORD dwSize, 
    PDRVENABLEDATA pded
    );

BOOL 
TextTTYGetInfo(
    _In_ PDEVOBJ pdevobj,    
    DWORD  dwInfoIndex,
    _Out_writes_bytes_(dwSize) PVOID   pOutBuf,  
    DWORD  dwSize, 
    _In_ DWORD  *pcbNeeded
    );
