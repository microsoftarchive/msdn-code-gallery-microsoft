/*++

Copyright (c) 1990-2007  Microsoft Corporation
All rights reserved

FileName:

    emf.h

Abstract:

    Header for emf.cpp

--*/

#ifndef _EMF_H
#define _EMF_H

typedef enum _EBookletMode {
    kNoBooklet        = 0,
    kBookletLeftEdge  = 1,
    kBookletRightEdge = 2
} EBookletMode, *PEBookletMode;

//
// Four n-up directions supported. The numbers given with each option are not to be changed
//
typedef enum _ENupDirection {
    kRightThenDown = 0,
    kDownThenRight = 1,
    kLeftThenDown  = 2,
    kDownThenLeft  = 3
}ENupDirection, *PENupDirection;

//
// Once the sheet comes out of printer, is it face up or down.
// And how are the subsequent sheets placed in relation to the previous sheet.
// This is necessary to know when doing duplex printing
//
// kFaceUp_NewPageUnder :
//     (Assuming no duplex)
//     When page 1 is printed, it comes out face up. When 2 is printed
//     it comes out under page 1. I don't know any printer that uses this format,
//     but it is easy to visualize page order if explained this way.
//     (When duplex is on)
//     The way it is used is that some printers switch the order when printing duplex.
//     So if you are printing a 4 page job backwards, then for this printer,
//     you should print in the order 3,4,2,1 (instead of the more intuitive 4,3,2,1).
//     Printer prints 3,4 and then turns over the sheet so that when sheet falls on tray,
//     page 3 is on top, even though page 4 is the last page printed.
//     This appears to be the default for winprint for Window Server 2003 and earlier version of Windows.
// kFaceDown_NewPageOver :
//     (Assuming no duplex)
//     When page 1 is printed, it comes out face down. When 2 is
//     printed, it comes on top of page 1. This is the format for HP LaserJet 5Si.
//     (When duplex is on)
//     For duplex reverse, you should print in the order 4,3,2,1
// kFaceUp_NewPageOver :
//     (Assuming no duplex)
//     When page 1 is printed, it comes out face up. When page 2 is
//     printed, it comes on top of page 1. This is the format for most deskjets.
//
typedef enum _EOutputPaperFace {
    kFaceUp_NewPageUnder     = 0,
    kFaceDown_NewPageOver    = 1,
    kFaceUp_NewPageOver      = 2
}EOutputPaperFace, *PEOutputPaperFace;

#define EMF_ATTRIBUTE_INFO_SIGNATURE '1fme' //in debugger, displayed as emf1


//   PAGE_NUMBER is used to save a list of the page numbers to start new sides while
//   Reverse Printing.

typedef struct _PAGE_NUMBER {
    struct _PAGE_NUMBER *pNextSide;     //first page on the next side.
    struct _PAGE_NUMBER *pNextLogPage;  //next logical page on the same side.
    DWORD               dwPageNumber;
} PAGE_NUMBER, *PPAGE_NUMBER;

typedef struct _EMF_ATTRIBUTE_INFO
{
    DWORD            dwSignature;                  //EMF_ATTRIBUTE_INFO_SIGNATURE

    LONG             lXResolution;                 //dpi in x-direction
    LONG             lYResolution;                 //dpi in y-direction
    LONG             lXPhyPage;                    //Physical width of paper  (in number of pixels).
    LONG             lYPhyPage;                    //Physical length of paper (in number of pixels).
    LONG             lXPrintArea;                  //Printable region width  (i.e. x direction) (in number of pixels).
    LONG             lYPrintArea;                  //Printable region length (i.e. y direction) (in number of pixels).

    DWORD            dwTotalNumberOfPages;         //Number of Pages in the job. Not always valid.
                                                   //To get this value, we need to wait for the
                                                   //job to finish spooling first. So this is
                                                   //valid only for certain cases like reverse
                                                   //printing.
    DWORD            dwNumberOfPagesPerSide;       //n-up
    DWORD            dwDrvNumberOfPagesPerSide;    //For printers that handle n-up themselves.
    DWORD            dwNupBorderFlags;             //What kind of borders to draw
    DWORD            dwJobNumberOfCopies;          //Num of copies (simulated by print proc)
    DWORD            dwDrvNumberOfCopies;          //Num of copies (simulated by driver)
    DWORD            dwDuplexMode;
    DWORD            dwDuplexModeFlags;            //Any specific treatment of duplex mode.
    ENupDirection    eNupDirection;                //Direction:Left then down, right then down.
                                                   //down then right, down then left.
    EBookletMode     eBookletMode;                 //Left edge binding(=0) or right edge(=1)
    DWORD            dwDivideCopiesIntoMultipleDocuments;  //Whether each new copy of job should be a new document
    BOOL             bReverseOrderPrinting;        //To print last page first (mostly)
    BOOL             bBookletPrint;
    EOutputPaperFace ePaperFace;
    BOOL             bCollate;                     //In case of multiple copies, should we play
                                                   // n-copies of page1, ncopies of page 2
                                                   // (bCollate = 0) OR
                                                   // print full job at a time (bCollate = 1)
    FLOAT            fScalingFactorX;
    FLOAT            fScalingFactorY;

} EMF_ATTRIBUTE_INFO, *PEMF_ATTRIBUTE_INFO;


BOOL BPlayEmptyPages(
        _In_  HANDLE                 hSpoolHandle,
        _In_  DWORD                  dwNumPages,
        _In_  DWORD                  dwOptimization);


BOOL
PrintOneSideReverseEMF(
    _In_ HANDLE                hSpoolHandle,
    _In_ HDC                   hPrinterDC,
    _In_ PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_ PPAGE_NUMBER          pHead,
    _In_ BOOL                  bDuplex,
    _In_ DWORD                 dwOptimization,
    _In_ LPDEVMODE             pDevmode);

BOOL
PrintOneSideReverseForDriverEMF(
    _In_ HANDLE                hSpoolHandle,
    _In_ HDC                   hPrinterDC,
    _In_ PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_ PPAGE_NUMBER          pHead,
    _In_ BOOL                  bDuplex,
    _In_ DWORD                 dwOptimization,
    _In_ LPDEVMODE             pDevmode);

BOOL
PrintOneSideBookletEMF(
    _In_ HANDLE                hSpoolHandle,
    _In_ HDC                   hPrinterDC,
    _In_ PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_ PPAGE_NUMBER          pHead,
    _In_ BOOL                  bDuplex,
    _In_ DWORD                 dwOptimization,
    _In_ LPDEVMODE             pDevmode);



BOOL
BPrintEMFJobNow (
        _In_     HANDLE                hSpoolHandle,
        _In_     HDC                   hPrinterDC,
        _In_     PEMF_ATTRIBUTE_INFO   pEMFAttr,
        _In_     DWORD                 dwOptimization,
        _In_     LPDEVMODEW            pDevmode,
        _In_opt_ PPAGE_NUMBER          pPageList,
        _In_     PPRINTPROCESSORDATA   pData );

BOOL
BUpdateAttributes(
        _In_     HDC                   hPrinterDC,
        _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr);

_Success_(return != NULL)
PPAGE_NUMBER
BReverseSidesOnSheet(
        _In_        PEMF_ATTRIBUTE_INFO   pEMFAttr,
        _Inout_opt_ PPAGE_NUMBER          pHead);

BOOL
BRetrieveJobAttributes(
    _In_  PEMF_ATTRIBUTE_INFO  pEMFAttr,
    _In_  LPCWSTR              pPrinterName,
    _In_  LPDEVMODE            pDevmode,
    _Out_ PATTRIBUTE_INFO_4    pAttributeInfo);


ENupDirection
ExtractNupDirectionFromAttributeInfo(
        _In_      PATTRIBUTE_INFO_4     pAttributeInfo,
        _In_      BOOL                  bBookletPrint);


EBookletMode
ExtractBookletModeFromAttributeInfo(
        _In_      PATTRIBUTE_INFO_4     pAttributeInfo,
        _In_      BOOL                  bBookletPrint);


VOID
ExtractScalingFactorFromAttributeInfo(
        _In_     PATTRIBUTE_INFO_4     pAttributeInfo,
        _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr
    );

VOID
ExtractDuplexModeInformationFromAttributeInfo(
        _In_     PATTRIBUTE_INFO_4     pAttributeInfo,
        _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr
    );

_Success_(return)
BOOL
PrintOneSheetPreDeterminedForDriverEMF (
    _In_      HANDLE                  hSpoolHandle,
    _In_      HDC                     hPrinterDC,
    _In_      PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_      BOOL                    bDuplex,
    _In_      PPAGE_NUMBER            pHead,
    _In_      DWORD                   dwOptimization,
    _Out_opt_ LPBOOL                  pbEmptyPageEncountered,
    _In_      LPDEVMODE               pDevmode);

BOOL
BAnyReasonNotToPrintBlankPage(
    _In_      PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_      DWORD                   dwNumberOfPagesInJob
    );

BOOL
BIsEveryPageOnThisSideBlank(
    _In_  PPAGE_NUMBER  pHeadLogical,
    _In_  DWORD         dwTotalNumberOfPages);


/*++

Class Description: Class for handling mapping from logical pages to coordinates on physical page in Nup

--*/

class CNupLayout
{
    unsigned m_uRow;
    unsigned m_uColumn;

public:

    unsigned GetRow(void) const
    {
        return m_uRow;
    }

    unsigned GetColumn(void) const
    {
        return m_uColumn;
    }

    CNupLayout(unsigned uPagePerSide, bool fLandscape);

    bool GetPageCoordinate(
        _In_  unsigned       uCurrentPageNumber,
        _In_  ENupDirection  eNupDirection,
        _Out_ unsigned      *pX,
        _Out_ unsigned      *pY) const;

    bool GetRotatePageCoordinate(
        _In_  UINT           uCurrentPageNumber,
        _In_  ENupDirection  eNupDirection,
        _Out_ unsigned      *pX,
        _Out_ unsigned      *pY) const;

};

#endif


