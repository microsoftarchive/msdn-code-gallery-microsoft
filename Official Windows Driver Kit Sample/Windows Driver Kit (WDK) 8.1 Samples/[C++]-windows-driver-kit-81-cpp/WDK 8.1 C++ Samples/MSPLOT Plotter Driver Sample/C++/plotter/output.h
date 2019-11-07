/*++

Copyright (c) 1990-2003  Microsoft Corporation


Module Name:

    output.h


Abstract:

    This module contains exported definitions for the output.c module


Author:

    16-Nov-1993 Tue 04:16:47 created  


[Environment:]

    GDI Device Driver - Plotter.


[Notes:]


Revision History:


--*/

#ifndef _PLOTOUTPUT_
#define _PLOTOUTPUT_



#define OUTPUT_BUFFER_SIZE      (32 * 1024)

#define HS_FT_USER_DEFINED      (HS_DDI_MAX + 1)

#define PLOT_LT_UNDEFINED       0
#define PLOT_LT_SOLID           1
#define PLOT_LT_USERDEFINED     2


#define SETLINETYPESOLID(pPDev)                                 \
{                                                               \
    if ((pPDev)->LastLineType != PLOT_LT_SOLID) {               \
                                                                \
        (pPDev)->LastLineType = PLOT_LT_SOLID;                  \
        OutputString(pPDev, "LT");                              \
    }                                                           \
}

#define SPP_MODE_CENTER         0x00
#define SPP_MODE_EDGE           0x01
#define SPP_MODE_MASK           0x01
#define SPP_FORCE_SET           0x80


LONG
BestMatchNonWhitePen(
    PPDEV   pPDev,
    LONG    R,
    LONG    G,
    LONG    B
    );

VOID
GetFinalColor(
            PPDEV       pPDev,
    _Inout_ PPALENTRY   pPalEntry
    );

LONG
FindCachedPen(
            PPDEV       pPDev,
    _Inout_ PPALENTRY   pPalEntry
    );

BOOL
PlotCreatePalette(
    PPDEV   pPDev
    );

UINT
AllocOutBuffer(
    PPDEV   pPDev
    );

VOID
FreeOutBuffer(
    PPDEV   pPDev
    );

BOOL
FlushOutBuffer(
    PPDEV   pPDev
    );

LONG
OutputBytes(
                      PPDEV   pPDev,
    _In_reads_bytes_(cBuf) LPBYTE  pBuf,
                      LONG    cBuf
    );

LONG
OutputString(
    PPDEV   pPDev,
    LPCSTR  pszStr
    );

_Ret_range_(0, cchStr16 - 1)
_Success_(return != 0)
LONG
LONGToASCII(
                                          LONG    Number,
    _Out_writes_bytes_to_(cchStr16, return+1) LPSTR   pStr16,
                                          size_t  cchStr16,
                                          BYTE    NumType
    );

LONG
OutputXYParams(
                         PPDEV   pPDev,
    _In_reads_(cPoints) PPOINTL pPtXY,
    _In_opt_             PPOINTL pPtOffset,
    _In_opt_             PPOINTL pPtCurPos,
                         UINT    cPoints,
                         UINT    MaxCurPosSkips,
                         BYTE    NumType
    );

LONG
OutputLONGParams(
    PPDEV   pPDev,
    PLONG   pNumbers,
    UINT    cNumber,
    BYTE    NumType
    );

LONG
cdecl
OutputFormatStrDELI(
    PPDEV   pPDev,
    CHAR    NumFormatChar,
    LPCSTR  pszFormat,
    ...
    );

LONG
cdecl
OutputFormatStr(
    PPDEV   pPDev,
    LPCSTR  pszFormat,
    ...
    );

BOOL
OutputCommaSep(
    PPDEV   pPDev
    );

VOID
ClearClipWindow(
    PPDEV pPDev
    );

VOID
SetClipWindow(
    PPDEV   pPDev,
    PRECTL  pClipRectl
    );

VOID
SetPixelPlacement(
    PPDEV   pPDev,
    UINT    SetMode
    );

BOOL
SetRopMode(
    PPDEV   pPDev,
    DWORD   Rop
    );

BOOL
SetHSFillType(
    PPDEV   pPDev,
    DWORD   HSFillTypeIndex,
    LONG    lParam
    );

BOOL
SendPageHeader(
    PPDEV   pPDev
    );

BOOL
SendPageTrailer(
    PPDEV   pPDev
    );



#endif  // _PLOTOUTPUT_
