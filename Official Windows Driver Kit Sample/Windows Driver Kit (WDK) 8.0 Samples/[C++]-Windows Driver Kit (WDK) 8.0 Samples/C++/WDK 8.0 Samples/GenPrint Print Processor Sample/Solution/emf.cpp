/*++

Copyright (c) 1990-2007  Microsoft Corporation
All rights reserved


Abstract:

    Routines to facilitate printing of EMF jobs.

--*/

#include "local.h"
#include "stddef.h"
#include <windef.h>
#include "emf.h"

extern "C" {
#include <winppi.h>
}


#ifndef ASSERT

#include <assert.h>

#define ASSERT assert

#endif


#define EMF_DUP_NONE 0
#define EMF_DUP_VERT 1
#define EMF_DUP_HORZ 2

#define EMF_DEGREE_90   0x0001
#define EMF_DEGREE_270  0x0002
#define EMF_DEGREE_SWAP 0x8000

// There are several features supported by new printers that cause additional
// pages to be added in the course of printing the job.
// Such a driver would not want the dmCollate field to be disabled for single page, multi copy jobs.
// To accomplish this, a driver should set this regkey by calling SetPrinterData API.
// Winprint in turn would honour this reg key and not disable the dmCollate field for single page, multi copy jobs

#define gszDriverKeepCollate  L"SinglePageKeepCollate"

//
// IS_DMSIZE_VALID returns TRUE if the size of the devmode is atleast as much as to
// be able to access field x in it without AV. It is assumed that the devmode is
// atleast the size pdm->dmSize
//
#define IS_DMSIZE_VALID(pdm,x)  ( ( (pdm)->dmSize >= (FIELD_OFFSET(DEVMODEW, x ) + sizeof((pdm)->x )))? TRUE:FALSE)


//
// function pointer to PrintOneSideReverseEMF. Used for booklet, reverse printing
//
typedef BOOL (*PFPRINTONESIDEREVERSEEMF)(HANDLE, \
                                         HDC,    \
                                         PEMF_ATTRIBUTE_INFO, \
                                         PPAGE_NUMBER, \
                                         BOOL, \
                                         DWORD, \
                                         LPDEVMODE);

//
// function pointer for forward printing. Used for drivers that do their own n-up for non-traditional nup
//
typedef DWORD (*PFPRINTONESIDEFORWARDEMF)(HANDLE, \
                                         HDC,    \
                                         PEMF_ATTRIBUTE_INFO, \
                                         BOOL, \
                                         DWORD, \
                                         DWORD, \
                                         LPBOOL, \
                                         LPDEVMODE);



#define MAX_NUP             16 //Max is 16-up
#define MAX_NUP_DIRECTIONS  4
#define MAX_NUP_OPTIONS     8  // MAX_NUP_DIRECTIONS * 2 (for Portrait, Landscape).
                               //the portrait and landscape used only for 2-up and 6-up.

const DWORD dwIndexIntoUpdateRect[MAX_NUP] = {           //npps=Number of Pages Per Side
                                            0xFFFFFFFF, //npps=1  //Nothing to do.
                                            0,          //npps=2
                                            0xFFFFFFFF, //npps=3  //Invalid Option
                                            1,          //npps=4
                                            0xFFFFFFFF, //npps=5  //Invalid Option
                                            2,          //npps=6
                                            0xFFFFFFFF, //npps=7  //Invalid Option
                                            0xFFFFFFFF, //npps=8  //Invalid Option
                                            3,          //npps=9
                                            0xFFFFFFFF, //npps=10  //Invalid Option
                                            0xFFFFFFFF, //npps=11  //Invalid Option
                                            0xFFFFFFFF, //npps=12  //Invalid Option
                                            0xFFFFFFFF, //npps=13  //Invalid Option
                                            0xFFFFFFFF, //npps=14  //Invalid Option
                                            0xFFFFFFFF, //npps=15  //Invalid Option
                                            4           //npps=16
                                        };



//
// Postscript based drivers prefer to handle n-up themselves. (i.e. they
// can do rotation, scaling etc. So the only thing Print Processor needs to
// do is to play back the pages in proper order.
// For example, if page order is down then right for 4-up, the pages
// should be played back in the order 1,3,2,4. PScript driver will place the pages
// as if the n-up direction is the traditional right then down direction. So
// it will place pages as follow
//    1   3
//    2   4
// thus achieving the down then right affect.
//
// Mostly the order of sending page is not dependent on portrait or landscape.
// But in 6 up, it is. See below
//        -------------
//        | 1 | 3 | 5 |
//        ============= Portrait for down then right in 6-up
//        | 2 | 4 | 6 | Pages played back in order 1,3,5,2,4,6
//        -------------
//
//        ---------
//        | 1 | 4 |
//        ---------
//        | 2 | 5 |   Landscape for down then right in 6-up
//        ---------  Pages played back in order 1,4,2,5,3,6
//        | 3 | 6 |
//        ---------

const DWORD gPageOrderPlayBackForDriver[][MAX_NUP_OPTIONS][MAX_NUP] =
    {
        { //2-up
            //Portrait
            { 1,2 },                                        // gPageOrderPlayBackForDriver[0][0]
            { 1,2 },                                        // [0][1]
            { 2,1 },                                        // [0][2]
            { 2,1 },                                        // [0][3]

            //Landscape
            { 1,2 },                                        // gPageOrderPlayBackForDriver[0][4]
            { 1,2 },                                        // [0][5]
            { 2,1 },                                        // [0][6]
            { 2,1 }                                         // [0][7]
        },

        { // 4up
            //Portrait
            { 1,2,3,4 },                                    // [1][0] //Right Then Down
            { 1,3,2,4 },                                    // [1][1] //Down Then Right
            { 2,1,4,3 },                                    // [1][2] //Left Then Down
            { 3,1,4,2 }                                     // [1][3] //Down Then Left
            //Landscape
            //Skipping entries since landscape portrait are same.
        },

        { // 6up
            //Portrait
            { 1,2,3,4,5,6 },                                // [2][0]
            { 1,3,5,2,4,6 },                                // [2][1]
            { 3,2,1,6,5,4 },                                // [2][2]
            { 5,3,1,6,4,2 },                                // [2][3]

            //Landscape
            { 1,2,3,4,5,6 },                                // [2][4]
            { 1,4,2,5,3,6 },                                // [2][5]
            { 2,1,4,3,6,5 },                                // [2][6]
            { 4,1,5,2,6,3 }                                 // [2][7]
        },

        { // 9up
            { 1,2,3,4,5,6,7,8,9 },                          // [3][0]
            { 1,4,7,2,5,8,3,6,9 },                          // [3][1]
            { 3,2,1,6,5,4,9,8,7 },                          // [3][2]
            { 7,4,1,8,5,2,9,6,3 }                           // [3][3]
        },

        { // 16up
            { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 },     // [4][0]
            { 1,5,9,13,2,6,10,14,3,7,11,15,4,8,12,16 },     // [4][1]
            { 4,3,2,1,8,7,6,5,12,11,10,9,16,15,14,13 },     // [4][2]
            { 13,9,5,1,14,10,6,2,15,11,7,3,16,12,8,4 }      // [4][3]
        }
    };

//
// Local function declaration
//
_Success_(return != FALSE)
BOOL GdiGetDevmodeForPagePvt(
    _In_            HANDLE              hSpoolHandle,
    _In_            DWORD               dwPageNumber,
    _Outptr_     PDEVMODEW           *ppCurrDM,
    _Outptr_opt_result_maybenull_ PDEVMODEW           *ppLastDM
  );

BOOL BIsDevmodeOfLeastAcceptableSize(
    _In_ PDEVMODE pdm) ;

_Success_(return)
BOOL
GetStartPageListForwardForDriver(
    _In_            PEMF_ATTRIBUTE_INFO    pEMFAttr,
    _In_            LPDEVMODEW             pDevmode,
    _Outptr_        PPAGE_NUMBER          *ppMemoryHead,
    _Outptr_        PPAGE_NUMBER          *ppPageListHead,
    _In_            DWORD                  dwSmallestPageOnSheet
    );


PDWORD
GetPlaybackPageOrderForDriverNup (
    _In_ DWORD                  dwNumberOfPagesPerSide,
    _In_ ENupDirection          eNupDirection,
    _In_ INT                    dmOrientation
    )
{
    DWORD dwNupDirection = (DWORD) eNupDirection;

    if ( DMORIENT_LANDSCAPE == dmOrientation &&
         ( 2                == dwNumberOfPagesPerSide ||
           6                == dwNumberOfPagesPerSide)
       )
    {
        dwNupDirection += MAX_NUP_DIRECTIONS;
    }

    return (PDWORD) gPageOrderPlayBackForDriver[dwIndexIntoUpdateRect[dwNumberOfPagesPerSide-1]][dwNupDirection];
}

BOOL
ValidNumberForNUp(
    _In_ DWORD  dwPages)

/*++
Function Description: Checks if the number of pages printed on a single side is Valid.

Parameters: dwPages - Number of pages printed on a single side

Return Values: TRUE if (dwPages = 1|2|4|6|9|16)
               FALSE otherwise.
--*/

{

    return ((dwPages == 1) || (dwPages == 2) || (dwPages == 4) ||
            (dwPages == 6) || (dwPages == 9) || (dwPages == 16));
}


//+------------------------------------------------------------------------------------------------------
//  Function:
//      CNupLayout::CNupLayout
//
//  Synopsis:
//      Calculate m_uRow/m_uColumn based on Nup pages per side and orientation
//
//-------------------------------------------------------------------------------------------------------

CNupLayout::CNupLayout(
    unsigned uPagePerSide,
    bool     fLandscape)
{
    m_uRow    = 1;
    m_uColumn = 1;

    // Switch can be extended or changed to add support for more Nup-cases
    switch (uPagePerSide)
    {
    case 1:
        break;

    case 2:
    case 4:
        m_uRow = 2;
        break;

    case 6:
    case 9:
        m_uRow = 3;
        break;

    case 16:
        m_uRow = 4;
        break;

    default: // Should Not Occur.
        ASSERT(FALSE);
        m_uRow = 0;
    }

    if (m_uRow >= 1)
    {
        if (fLandscape) // swap row and column for landscape mode
        {
            m_uRow = uPagePerSide / m_uRow;
        }

        m_uColumn = uPagePerSide / m_uRow;

        ASSERT((m_uColumn * m_uRow) == uPagePerSide);
    }
}


//+------------------------------------------------------------------------------------------------------
//  Function:
//      CNupLayout::GetPageCoordinate
//
//  Synopsis:
//      Calculate coordinate (x, y) for logical page uCurrentPage, under direction eNupDirection
//
//  Returns:
//      * pX in [0 .. m_uColumn -1]
//      * pY in [0 .. m_uRow -1]
//      true if succedded, false if invalid input
//-------------------------------------------------------------------------------------------------------

bool CNupLayout::GetPageCoordinate(
    _In_  unsigned       uCurrentPageNumber,
    _In_  ENupDirection  eNupDirection,
    _Out_ unsigned      *pX,
    _Out_ unsigned      *pY) const
{
    ASSERT( (pX != NULL) && (pY != NULL));
    ASSERT((m_uRow >= 1) && (m_uColumn >= 1));

    uCurrentPageNumber %= (m_uRow * m_uColumn);

    *pX = 0;
    *pY = 0;

    switch (eNupDirection)
    {
        // 0 1
        // 2 3
        // 4
    case kRightThenDown:
        *pX = uCurrentPageNumber % m_uColumn;
        *pY = uCurrentPageNumber / m_uColumn;
        break;

        // 0 3
        // 1 4
        // 2
    case kDownThenRight:
        *pX = uCurrentPageNumber / m_uRow;
        *pY = uCurrentPageNumber % m_uRow;
        break;

    case kLeftThenDown:
        // 1 0
        // 3 2
        // 4
        *pX = m_uColumn - 1 - uCurrentPageNumber % m_uColumn;
        *pY =                 uCurrentPageNumber / m_uColumn;
        break;

        // 3 0
        // 4 1
        //   2
    case kDownThenLeft:
        *pX = m_uColumn - 1 - uCurrentPageNumber / m_uRow;
        *pY =                 uCurrentPageNumber % m_uRow;
        break;

    default:
        return false;
    }

    ASSERT(*pX < m_uColumn);
    ASSERT(*pY < m_uRow);

    return true;
}


//+------------------------------------------------------------------------------------------------------
//  Function:
//      CNupLayout::GetRotatePageCoordinate
//
//  Synopsis:
//      Calculate coordinate (x, y) for rotated logical page uCurrentPage, under direction eNupDirection
//      Logical pages should be in the right direction after the page is rotated
//
//  Returns:
//      * pX in [0 .. m_uColumn -1]
//      * pY in [0 .. m_uRow -1]
//      true if succedded, false if invalid input
//-------------------------------------------------------------------------------------------------------

bool CNupLayout::GetRotatePageCoordinate(
    _In_  unsigned       uCurrentPageNumber,
    _In_  ENupDirection  eNupDirection,
    _Out_ unsigned      *pX,
    _Out_ unsigned      *pY) const
{
    ASSERT((pX != NULL) && (pY != NULL));
    ASSERT((m_uRow >= 1) && (m_uColumn >= 1));

    uCurrentPageNumber %= (m_uRow * m_uColumn);

    *pX = 0;
    *pY = 0;

    switch (eNupDirection)
    {
        // 2
        // 1 4
        // 0 3
    case kRightThenDown:
        *pX =              uCurrentPageNumber / m_uRow;
        *pY = m_uRow - 1 - uCurrentPageNumber % m_uRow;
        break;

        // 4
        // 2 3
        // 0 1
    case kDownThenRight:
        *pX =              uCurrentPageNumber % m_uColumn;
        *pY = m_uRow - 1 - uCurrentPageNumber / m_uColumn;
        break;

    case kLeftThenDown:
        // 0 3
        // 1 4
        // 2
        *pX = uCurrentPageNumber / m_uRow;
        *pY = uCurrentPageNumber % m_uRow;
        break;

        // 0 1
        // 2 3
        // 4
    case kDownThenLeft:
        *pX = uCurrentPageNumber % m_uColumn;
        *pY = uCurrentPageNumber / m_uColumn;
        break;

    default:
        return false;
    }

    ASSERT(*pX < m_uColumn);
    ASSERT(*pY < m_uRow);

    return true;
}


_Success_(return)
BOOL
GetPageCoordinatesForNUp(
    _In_        HDC                    hPrinterDC,
    _In_        PEMF_ATTRIBUTE_INFO    pEMFAttr,
    _Out_       RECT                  *rectDocument,
    _Inout_opt_ RECT                  *rectBorder,
    _In_        UINT                   uCurrentPageNumber,
    _Out_       LPBOOL                 pbRotate
    )

/*++
Function Description: GetPageCoordinatesForNUp computes the rectangle on the Page where the
                      EMF file is to be played. It also determines if the picture is to
                      rotated.

Parameters:  hPrinterDC           - Printer Device Context
             pEMFAttr             - Attributes like n-up, n-up Border flags. Assumed to be valid.
             *rectDocument        - pointer to RECT where the coordinates to play the
                                     page will be returned.
             *rectBorder          - pointer to RECT where the page borders are to drawn.
             uCurrentPageNumber   - 1 based page number on the side.
             pbRotate             - pointer to BOOL which indicates if the picture must be
                                    rotated.

Return Values:  TRUE if successful
--*/

{
    LONG          ltemp,ldX,ldY;

    DWORD         dwTotalNumberOfPages = pEMFAttr->dwNumberOfPagesPerSide; //total pages on 1 side
    DWORD         dwNupBorderFlags     = pEMFAttr->dwNupBorderFlags;
    ENupDirection eNupDirection        = pEMFAttr->eNupDirection;

    *pbRotate = FALSE;

    // Return full printable are for single page
    if (dwTotalNumberOfPages == 1)
    {
        rectDocument->top    = 0;
        rectDocument->left   = 0;
        rectDocument->bottom = pEMFAttr->lYPrintArea;
        rectDocument->right  = pEMFAttr->lXPrintArea;

        return TRUE;
    }

    // Change to zero-based page number
    ASSERT(uCurrentPageNumber >= 1);
    uCurrentPageNumber --;

    const int lYPrintPage = pEMFAttr->lYPrintArea - 1;
    const int lXPrintPage = pEMFAttr->lXPrintArea - 1;

    const LONG lXPhyPage = pEMFAttr->lXPhyPage;
    const LONG lYPhyPage = pEMFAttr->lYPhyPage;

    //
    // Down in the code, we are dividing by these values, which can lead
    // to divide-by-zero errors.
    //
    if ((0 == lXPhyPage) || (0 == lYPhyPage))
    {
        return FALSE;
    }

    CNupLayout layout(dwTotalNumberOfPages, lXPrintPage > lYPrintPage);

    const unsigned uCol = layout.GetColumn();
    const unsigned uRow = layout.GetRow();

    if ((uCol < 1) || (uRow < 1))
    {
         ODS(("Unsupported nup page per side\n"));

         return FALSE;
    }

    // Size of each logical page on the physical page
    long lXFrame = lXPrintPage / uCol;
    long lYFrame = lYPrintPage / uRow;


    // Set the flag if the picture has to be rotated
    *pbRotate = !((lXPhyPage >= lYPhyPage) && (lXFrame >= lYFrame)) &&
                !((lXPhyPage < lYPhyPage) && (lXFrame < lYFrame));

    // Coordinate of current logical page
    unsigned x = 0;
    unsigned y = 0;

    bool rslt;

    if (*pbRotate)
    {
        rslt = layout.GetRotatePageCoordinate(uCurrentPageNumber, eNupDirection, & x, & y);
    }
    else
    {
        rslt = layout.GetPageCoordinate(uCurrentPageNumber, eNupDirection, & x, & y);
    }

    if (! rslt)
    {
        return FALSE;
    }

    // Set the Page Coordinates.

    rectDocument->left   = (int) (lXPrintPage * x       * 1.0 / uCol);
    rectDocument->right  = (int) (lXPrintPage * (x + 1) * 1.0 / uCol);

    rectDocument->top    = (int) (lYPrintPage * y       * 1.0 / uRow);
    rectDocument->bottom = (int) (lYPrintPage * (y + 1) * 1.0 / uRow);

    // If the page border has to drawn, return the corresponding coordinates in rectBorder.

    if ((dwNupBorderFlags == BORDER_PRINT) && (rectBorder != NULL))
    {
        rectBorder->top    = rectDocument->top;
        rectBorder->bottom = rectDocument->bottom - 1;
        rectBorder->left   = rectDocument->left;
        rectBorder->right  = rectDocument->right - 1;
    }

    if (*pbRotate)
    {
        ltemp = lXFrame; lXFrame = lYFrame; lYFrame = ltemp;
    }

    // Get the new size of the rectangle to keep the X/Y ratio constant.
    if ( ((LONG) (lYFrame*((lXPhyPage*1.0)/lYPhyPage))) >= lXFrame)
    {
         ldX = 0;
         ldY = lYFrame - ((LONG) (lXFrame*((lYPhyPage*1.0)/lXPhyPage)));
    }
    else
    {
         ldY = 0;
         ldX = lXFrame - ((LONG) (lYFrame*((lXPhyPage*1.0)/lYPhyPage)));
    }

    // Adjust the position of the rectangle.

    if (*pbRotate)
    {
        if (ldX)
        {
            rectDocument->bottom -= ldX / 2;
            rectDocument->top    += ldX / 2;
        }
        else
        {
           rectDocument->right   -= ldY / 2;
           rectDocument->left    += ldY / 2;
        }
    }
    else
    {
        if (ldX)
        {
           rectDocument->left    += ldX / 2;
           rectDocument->right   -= ldX / 2;
        }
        else
        {
           rectDocument->top     += ldY / 2;
           rectDocument->bottom  -= ldY / 2;
        }
    }

    // Adjust to get the Printable Area on the rectangle

    int lXOffset = GetDeviceCaps(hPrinterDC, PHYSICALOFFSETX);
    int lYOffset = GetDeviceCaps(hPrinterDC, PHYSICALOFFSETY);

    double dXleft   = ( lXOffset * 1.0) / lXPhyPage;
    double dYtop    = ( lYOffset * 1.0) / lYPhyPage;
    double dXright  = ((lXPhyPage - (lXOffset + lXPrintPage)) * 1.0) / lXPhyPage;
    double dYbottom = ((lYPhyPage - (lYOffset + lYPrintPage)) * 1.0) / lYPhyPage;

    int lXNewPhyPage = rectDocument->right  - rectDocument->left;
    int lYNewPhyPage = rectDocument->bottom - rectDocument->top;

    if (*pbRotate)
    {
       rectDocument->left   += (LONG) (dYtop    * lXNewPhyPage);
       rectDocument->right  -= (LONG) (dYbottom * lXNewPhyPage);
       rectDocument->top    += (LONG) (dXright  * lYNewPhyPage);
       rectDocument->bottom -= (LONG) (dXleft   * lYNewPhyPage);
    }
    else
    {
       rectDocument->left   += (LONG) (dXleft   * lXNewPhyPage);
       rectDocument->right  -= (LONG) (dXright  * lXNewPhyPage);
       rectDocument->top    += (LONG) (dYtop    * lYNewPhyPage);
       rectDocument->bottom -= (LONG) (dYbottom * lYNewPhyPage);
    }

    return TRUE;
}

BOOL
PlayEMFPage(
    _In_       HANDLE                hSpoolHandle,
    _In_       HDC                   hPrinterDC,
    _In_       HANDLE                hEMF,
    _In_ const PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_       DWORD                 dwPageIndex,
    _In_       DWORD                 dwAngle)

/*++
Function Description: PlayEMFPage plays the EMF in the appropriate rectangle. It performs
                      the required scaling, rotation and translation.

Parameters:   hSpoolHandle           -- handle the spool file handle
              hPrinterDC             -- handle to the printer device context
              hEMF                   -- handle to the contents of the page in the spool file
              pEMFAttr               -- Attributes like n-up, Border Flags etc. Assumed nonnull.
              dwPageIndex            -- page number in the side. (1 based)
              dwAngle                -- angle for rotation (if neccesary)

Return Values:  TRUE if successful
                FALSE otherwise
--*/
{
    BOOL         bReturn = FALSE, bRotate;
    RECT         rectDocument, rectPrinter, rectBorder = {-1, -1, -1, -1};
    RECT         *prectClip = NULL;
    XFORM        TransXForm = {1, 0, 0, 1, 0, 0}, RotateXForm = {0, -1, 1, 0, 0, 0};
    XFORM        ScaleXForm = {1, 0, 0, 1, 0, 0};

    DWORD        dwNumberOfPagesPerSide = pEMFAttr->dwNumberOfPagesPerSide;
    BOOL         bSetWorldTransformDone = FALSE;

    // Compute the rectangle for one page.
    if ( FALSE == GetPageCoordinatesForNUp(hPrinterDC,
                            pEMFAttr,
                            &rectDocument,
                            &rectBorder,
                            dwPageIndex,
                            &bRotate) )
    {
        goto CleanUp;
    }

    // If swap flag is set, reverse rotate flag
    //
    if (dwAngle & EMF_DEGREE_SWAP)
        bRotate = !bRotate;

    if (dwAngle & EMF_DEGREE_270) {
        RotateXForm.eM12 = 1;
        RotateXForm.eM21 = -1;
    }   // EMF_DEGREE_90 case is the initialization

    if (bRotate) {

        rectPrinter.top = 0;
        rectPrinter.bottom = rectDocument.right - rectDocument.left;
        rectPrinter.left = 0;
        rectPrinter.right = rectDocument.bottom - rectDocument.top;

        // Set the translation matrix
        if (dwAngle & EMF_DEGREE_270) {
            TransXForm.eDx = (float) rectDocument.right;
            TransXForm.eDy = (float) rectDocument.top;
        } else {
            // EMF_DEGREE_90
            TransXForm.eDx = (float) rectDocument.left;
            TransXForm.eDy = (float) rectDocument.bottom;
        }

        // Set the transformation matrix
        if (!SetWorldTransform(hPrinterDC, &RotateXForm) ||
           !ModifyWorldTransform(hPrinterDC, &TransXForm, MWT_RIGHTMULTIPLY)) {

            ODS(("Setting transformation matrix failed\n"));
            goto CleanUp;
        }

        bSetWorldTransformDone = TRUE;
    }

    // Add clipping for Nup
    if (dwNumberOfPagesPerSide != 1) {

        prectClip = &rectDocument;
    }

    // Scaling is done only if there is no n-up
    if ( 1 == dwNumberOfPagesPerSide )
    {
        // Since we support only square scaling, we assume x and
        // y scaling are equal. So we ignore the
        // y scaling factor.
        ScaleXForm.eM11 = pEMFAttr->fScalingFactorX;
        ScaleXForm.eM22 = ScaleXForm.eM11;

        if ( !bSetWorldTransformDone )
        {
            if (!SetWorldTransform(hPrinterDC, &ScaleXForm) )
            {
                {
                    ODS(("Setting transformation matrix for scaling failed\n"));
                    goto CleanUp;
                }
            }
        }
        else
        {
            if (!ModifyWorldTransform(hPrinterDC, &ScaleXForm, MWT_RIGHTMULTIPLY) )
            {
                {
                    ODS(("Modifying transformation matrix for scaling failed\n"));
                    goto CleanUp;
                }
            }
        }
    }

    // Print the page.
    if (bRotate)
    {
        GdiPlayPageEMF(hSpoolHandle, hEMF, &rectPrinter, &rectBorder, prectClip);
    }
    else
    {
        GdiPlayPageEMF(hSpoolHandle, hEMF, &rectDocument, &rectBorder, prectClip);
    }

    bReturn = TRUE;

CleanUp:

    if (!ModifyWorldTransform(hPrinterDC, NULL, MWT_IDENTITY))
    {
        ODS(("Setting Identity Transformation failed\n"));
        bReturn = FALSE;
    }

    return bReturn;
}

BOOL
SetDrvCopies(
    _In_    HDC          hPrinterDC,
    _Inout_ LPDEVMODEW   pDevmode,
    _In_    DWORD        dwNumberOfCopies)

/*++
Function Description: SetDrvCopies sets the dmCopies field in pDevmode and resets
                      hPrinterDC with this devmode

Parameters: hPrinterDC             -- handle to the printer device context
            pDevmode               -- pointer to devmode
            dwNumberOfCopies       -- value for dmCopies

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    BOOL     bReturn;
    DWORD    dmFields;

    if ((pDevmode->dmFields & DM_COPIES) &&
        (pDevmode->dmCopies == (short) dwNumberOfCopies)) {

         return TRUE;
    }

    // Save the old fields structure
    dmFields = pDevmode->dmFields;
    pDevmode->dmFields |= DM_COPIES;
    pDevmode->dmCopies = (short) dwNumberOfCopies;

    if (!ResetDC(hPrinterDC, pDevmode))  {
        bReturn = FALSE;
    } else {
        bReturn = TRUE;
    }

    // Restore the fields structure
    pDevmode->dmFields = dmFields;

    if (!SetGraphicsMode(hPrinterDC,GM_ADVANCED)) {
        ODS(("Setting graphics mode failed\n"));
        bReturn = FALSE;
    }

    return bReturn;
}

BOOL
DifferentDevmodes(
    _In_opt_ LPDEVMODE    pDevmode1,
    _In_opt_ LPDEVMODE    pDevmode2
    )

/*++
Function Description: Compares the devmodes for differences other than dmTTOption

Parameters:  pDevmode1    -   devmode 1
             pDevmode2    -   devmode 2

Return Values: TRUE if different ; FALSE otherwise
--*/

{
    DWORD   dwSize1, dwSize2, dwTTOffset, dwSpecOffset, dwLogOffset;

    // Same pointers are the same devmode
    if (pDevmode1 == pDevmode2) {
        return FALSE;
    }

    // Check for Null devmodes
    if (!pDevmode1 || !pDevmode2) {
        return TRUE;
    }

    dwSize1 = pDevmode1->dmSize + pDevmode1->dmDriverExtra;
    dwSize2 = pDevmode2->dmSize + pDevmode2->dmDriverExtra;

    // Compare devmode sizes
    if (dwSize1 != dwSize2) {
        return TRUE;
    }

    dwTTOffset   = FIELD_OFFSET(DEVMODE, dmTTOption);
    dwSpecOffset = FIELD_OFFSET(DEVMODE, dmSpecVersion);
    dwLogOffset  = FIELD_OFFSET(DEVMODE, dmLogPixels);

    if (wcscmp(pDevmode1->dmDeviceName,
               pDevmode2->dmDeviceName)) {
        // device names are different
        return TRUE;
    }

    if (dwTTOffset < dwSpecOffset ||
        dwSize1 < dwLogOffset) {

        // incorrent devmode offsets
        return TRUE;
    }

    if (memcmp((LPBYTE) pDevmode1 + dwSpecOffset,
               (LPBYTE) pDevmode2 + dwSpecOffset,
               dwTTOffset - dwSpecOffset)) {
        // Front half is different
        return TRUE;
    }

    // Ignore the dmTTOption setting.

    if ((pDevmode1->dmCollate != pDevmode2->dmCollate) ||
        wcscmp(pDevmode1->dmFormName, pDevmode2->dmFormName)) {

        // form name or collate option is different
        return TRUE;
    }

    if (memcmp((LPBYTE) pDevmode1 + dwLogOffset,
               (LPBYTE) pDevmode2 + dwLogOffset,
               dwSize1 - dwLogOffset)) {
        // Back half is different
        return TRUE;
    }

    return FALSE;
}

_Success_(return)
BOOL
ResetDCForNewDevmode(
    _In_            HANDLE                hSpoolHandle,
    _In_            HDC                   hPrinterDC,
    _In_            PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_            DWORD                 dwPageNumber,
    _In_            BOOL                  bInsidePage,
    _In_            DWORD                 dwOptimization,
    _Out_           LPBOOL                pbNewDevmode,
    _In_            LPDEVMODE             pDevmode,
    _Outptr_result_maybenull_ LPDEVMODE            *ppCurrentDevmode
    )

/*++
Function Description: Determines if the devmode for the page is different from the
                      current devmode for the printer dc and resets the dc if necessary.
                      The parameters allow dmTTOption to be ignored in devmode comparison.

Parameters: hSpoolHandle         -  spool file handle
            hPrinterDC           -  printer dc
            pEMFAttr             -  Job Attributes
            dwPageNumber         -  page number before which we search for the devmode
            bInsidePage          -  flag to ignore changes in TT options and call EndPage
                                       before ResetDC
            dwOptimization       -  optimization flags
            pbNewDevmode         -  pointer to flag to indicate if ResetDC was called
            pDevmode             -  The devmode active when this function called.
            ppCurrentDevmode     -  The devmode that should be active after exiting from this func.

Return Values: TRUE if successful; FALSE otherwise
--*/

{
    BOOL           bReturn = FALSE;
    LPDEVMODE      pLastDM = NULL, pCurrDM = NULL;

    // Initialize OUT parameters
    *pbNewDevmode = FALSE;

    // Get the devmode just before the page
    if (!GdiGetDevmodeForPagePvt(hSpoolHandle,
                                 dwPageNumber,
                                 &pCurrDM,
                                 &pLastDM)) {

        ODS(("GdiGetDevmodeForPagePvt failed\n"));
        return bReturn;
    }

    // Save pointer to current devmode
    if (ppCurrentDevmode)
        *ppCurrentDevmode = pCurrDM;

    // Check if the devmodes are different
    // If pointers are same, devmodes are evidently same
    // If pointers are different, we test the contents of the devmode
    if (pLastDM != pCurrDM) {

        if (!bInsidePage ||
            DifferentDevmodes(pLastDM, pCurrDM)) {

            *pbNewDevmode = TRUE;
        }
    }

    // Call ResetDC on the hPrinterDC if necessary
    if (*pbNewDevmode) {

        if (bInsidePage &&
            !GdiEndPageEMF(hSpoolHandle, dwOptimization)) {

            ODS(("EndPage failed\n"));
            return bReturn;
        }

        if (pCurrDM) {
            pCurrDM->dmPrintQuality = pDevmode->dmPrintQuality;
            pCurrDM->dmYResolution = pDevmode->dmYResolution;
            pCurrDM->dmCopies = pDevmode->dmCopies;


            if ( IS_DMSIZE_VALID ( pCurrDM, dmCollate ) )
            {
                if ( IS_DMSIZE_VALID ( pDevmode, dmCollate ) )
                {
                    pCurrDM->dmCollate = pDevmode->dmCollate;
                }
                else
                {
                    pCurrDM->dmCollate = DMCOLLATE_FALSE;
                }

            }
        }

        // Ignore the return values of ResetDC and SetGraphicsMode
        GdiResetDCEMF(hSpoolHandle, pCurrDM);
        SetGraphicsMode(hPrinterDC, GM_ADVANCED);

        BUpdateAttributes(hPrinterDC, pEMFAttr);
    }

    bReturn = TRUE;

    return bReturn;
}

DWORD
PrintOneSideForwardEMF(
    _In_  HANDLE                  hSpoolHandle,
    _In_  HDC                     hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_  BOOL                    bDuplex,
    _In_  DWORD                   dwOptimization,
    _In_  DWORD                   dwPageNumber,
    _Out_ LPBOOL                  pbComplete,
    _In_  LPDEVMODE               pDevmode)

/*++
Function Description: PrintOneSideForwardEMF plays the next physical page in the same order
                      as the spool file.

Parameters: hSpoolHandle              -- handle the spool file handle
            hPrinterDC                -- handle to the printer device context
            pEMFAttr                  --
            bDuplex                   -- flag to indicate duplex printing
            dwOptimization            -- optimization flags
            dwPageNumber              -- pointer to the starting page number
            pbComplete                -- pointer to the flag to indicate completion
            pDevmode                  -- devmode with resolution settings

Return Values:  Last Page Number if successful
                0 on job completion (pbReturn set to TRUE) and
                  on failure (pbReturn remains FALSE)
--*/

{
    DWORD              dwPageIndex, dwPageType;
    DWORD              dwReturn = 0;
    LPDEVMODEW         pCurrDM;
    HANDLE             hEMF = NULL;
    DWORD              dwSides;
    BOOL               bNewDevmode;
    DWORD              cPagesToPlay;
    DWORD              dwAngle;
    INT                dmOrientation = pDevmode->dmOrientation;
    DWORD              dwNumberOfPagesPerSide    = pEMFAttr->dwNumberOfPagesPerSide;
    DWORD              dwDrvNumberOfPagesPerSide = pEMFAttr->dwDrvNumberOfPagesPerSide;
    DWORD              dwJobNumberOfCopies       = pEMFAttr->dwJobNumberOfCopies;


    // set the number of sides on this page;
    dwSides = bDuplex ? 2 : 1;
    *pbComplete = FALSE;

    for ( ; dwSides && !*pbComplete ; --dwSides) {

       // loop for a single side
       for (dwPageIndex = 1;
            dwPageIndex <= dwNumberOfPagesPerSide;
            ++dwPageIndex, ++dwPageNumber) {

            if ((hEMF = GdiGetPageHandle(hSpoolHandle,
                                          dwPageNumber,
                                          &dwPageType))
                == NULL) {

                if (GetLastError() == ERROR_NO_MORE_ITEMS) {
                     // End of the print job
                     *pbComplete = TRUE;
                     break;
                }

                ODS(("GdiGetPageHandle failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                goto CleanUp;
            }

            dwAngle = EMF_DEGREE_90;

            if (dwPageIndex == 1)
            {
                // Process new devmodes in the spool file that appear before this page
                if (!ResetDCForNewDevmode(hSpoolHandle,
                                      hPrinterDC,
                                      pEMFAttr,
                                      dwPageNumber,
                                      (dwPageIndex != 1),
                                      dwOptimization,
                                      &bNewDevmode,
                                      pDevmode,
                                      &pCurrDM)) {

                    goto CleanUp;
                }

                if (pCurrDM)
                    dmOrientation = pCurrDM->dmOrientation;
            }
            // in case of orientation switch we need to keep track of what
            // we started with and what it is now
            else if (dwNumberOfPagesPerSide > 1)
            {
                if (GdiGetDevmodeForPagePvt(hSpoolHandle,
                                            dwPageNumber,
                                            &pCurrDM,
                                            NULL))
                {
                    if (pCurrDM && pCurrDM->dmOrientation != dmOrientation)
                    {
                        dwAngle = EMF_DEGREE_SWAP | EMF_DEGREE_90;
                        BUpdateAttributes(hPrinterDC, pEMFAttr);
                    }
                }
            }

            // Call StartPage for each new page
            if ((dwPageIndex == 1) &&
                !GdiStartPageEMF(hSpoolHandle)) {

                ODS(("StartPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                goto CleanUp;
            }

            if (!PlayEMFPage(hSpoolHandle,
                             hPrinterDC,
                             hEMF,
                             pEMFAttr,
                             dwPageIndex,
                             dwAngle)) {

                ODS(("PlayEMFPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                goto CleanUp;
            }
       }

       //
       // Explaination of the scinario set for the conditions on
       // dwPageIndex1 , pbComplete and bDuplex.
       // N.B. we are naming them cond.1 and cond.2
       //     dwPageIndex!=1    pbComplete   bDuplex    Condition
       //           0               0           0       None
       //           0               0           1       None
       //           0               1           0       None
       //           0               1           1       Cond2 on Second Side i.e. dwsides==1
       //           1               0           0       Cond1
       //           1               0           1       Cond1
       //           1               1           0       Cond1
       //           1               1           1       Cond1 & Cond2 on First Side i.e. dwsides==2
       //


       // cond.1
       if (dwPageIndex != 1) {

           // Call EndPage if we played any pages
           if (!GdiEndPageEMF(hSpoolHandle, dwOptimization)) {

               ODS(("EndPage failed\n"));
               *pbComplete = FALSE;
               goto CleanUp;
           }
       }

       // cond.2
       // play empty page on the back of duplex
       if (*pbComplete && bDuplex && dwDrvNumberOfPagesPerSide==1) {

            //
           // Checking dwsides against 2 or 1.
           // depends on whether it is n-up or not.
           //
           if (((dwPageIndex!=1)?(dwSides==2):(dwSides==1))) {

                if ( !BAnyReasonNotToPrintBlankPage(pEMFAttr, dwPageNumber - 1) )
                {
                   if (!GdiStartPageEMF(hSpoolHandle) ||
                       !GdiEndPageEMF(hSpoolHandle, dwOptimization)) {

                       ODS(("EndPage failed\n"));
                       *pbComplete = FALSE;
                       goto CleanUp;
                   }
                }
           }
        }
    }

    if ( *pbComplete                    &&
         dwNumberOfPagesPerSide    ==1  &&
         dwDrvNumberOfPagesPerSide !=1  &&
         dwJobNumberOfCopies       !=1  )
    {
        cPagesToPlay = dwDrvNumberOfPagesPerSide * (bDuplex ? 2 : 1);
        if ((dwPageNumber-1) % cPagesToPlay)
        {
            //
            // Number of pages played on last physical page
            //
            cPagesToPlay = cPagesToPlay - ((dwPageNumber-1) % cPagesToPlay);

            ODS(("\nPS with N-up!\nMust fill in %u pages\n", cPagesToPlay));

            for (;cPagesToPlay;cPagesToPlay--)
            {
                if (!GdiStartPageEMF(hSpoolHandle) || !GdiEndPageEMF(hSpoolHandle, dwOptimization))
                {
                    ODS(("EndPage failed\n"));
                    goto CleanUp;
                }
            }
        }
    }

    if (!(*pbComplete)) dwReturn = dwPageNumber;

CleanUp:

    return dwReturn;
}

_Success_(return || *pbComplete != 0)
DWORD
PrintOneSideForwardForDriverEMF(
    _In_  HANDLE                  hSpoolHandle,
    _In_  HDC                     hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_  BOOL                    bDuplex,
    _In_  DWORD                   dwOptimization,
    _In_  DWORD                   dwSmallestPageOnSheet,
    _Out_ LPBOOL                  pbComplete,
    _In_  LPDEVMODE               pDevmode)

/*++
Function Description:
    PrintOneSideForwardForDriverEMF plays the next physical page in an order determined
    by the order of pages in spool file and the nupDirection. If the nup direction
    is kRightThenDown, then the order of play back is same as the order in spool
    file. For that this function shouldn't be called. PrintOneSideForwardEMF should
    work well for that case. This is limited to the case where driver does nup,
    nup>1 and nupDirection is not kRightThenDown

Parameters: hSpoolHandle              -- handle the spool file handle
            hPrinterDC                -- handle to the printer device context
            pEMFAttr                  --
            bDuplex                   -- flag to indicate duplex printing
            dwOptimization            -- optimization flags
            dwPageNumber              -- pointer to the starting page number
            pbComplete                -- pointer to the flag to indicate completion
            pDevmode                  -- devmode with resolution settings

Return Values:  Last Page Number if successful
                0 on job completion (pbComplete set to TRUE) and
                  on failure (pbComplete remains FALSE)
--*/

{
    DWORD              dwReturn                  = 0;
    DWORD              dwSides                   = 1;
    PPAGE_NUMBER       pMemoryHead               = NULL, pHeadNewSide = NULL;
    BOOL               bAtleastOneEmptyPageFound = FALSE;
    DWORD              dwNumberOfPagesPerSide    = 0;

    *pbComplete = FALSE;

    // This function written just for the case where driver does nup,
    // nup>1 and nupDirection is not kRightThenDown
    if ( pEMFAttr->dwDrvNumberOfPagesPerSide <= 1             ||
         pEMFAttr->dwNumberOfPagesPerSide    >  1             ||
         pEMFAttr->eNupDirection             == kRightThenDown )
    {
        ODS(("This function should not be called for non-driver nup and traditional nup"));
        return 0;
    }

    dwNumberOfPagesPerSide = pEMFAttr->dwDrvNumberOfPagesPerSide;

    // set the number of sides on this page;
    dwSides = bDuplex ? 2 : 1;

    if (!GetStartPageListForwardForDriver(
                                          pEMFAttr,
                                          pDevmode,
                                          &pMemoryHead,   //Where memory for the list starts.
                                          &pHeadNewSide, //where the actual head of list starts.
                                          dwSmallestPageOnSheet
                                          )
       )
    {
        ODS(("Cannot get Page Order for Forward Printing"));
        goto CleanUp;
    }

    if ( PrintOneSheetPreDeterminedForDriverEMF (
                                    hSpoolHandle,
                                    hPrinterDC,
                                    pEMFAttr,
                                    bDuplex,
                                    pHeadNewSide,
                                    dwOptimization,
                                    &bAtleastOneEmptyPageFound,
                                    pDevmode)
        )
    {
        if ( bAtleastOneEmptyPageFound )
        {
            // We found one empty page on the side, so that means no more pages to print.
            *pbComplete = TRUE;
        }
        else
        {
            dwSmallestPageOnSheet += dwNumberOfPagesPerSide*dwSides; //Prepare for the next time this function is called.
            dwReturn = dwSmallestPageOnSheet;
        }
    }

CleanUp:

    FreeSplMem(pMemoryHead);
    return dwReturn;
}


BOOL
PrintForwardEMF(
    _In_  HANDLE                  hSpoolHandle,
    _In_  HDC                     hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_  BOOL                    bDuplex,
    _In_  DWORD                   dwOptimization,
    _In_  LPDEVMODEW              pDevmode,
    _In_  PPRINTPROCESSORDATA     pData)

/*++
Function Description: PrintForwardEMF plays the EMF files in the order in which they
                      were spooled.

Parameters: hSpoolHandle              -- handle the spool file handle
            hPrinterDC                -- handle to the printer device context
            pEMFAttr                  --
            bDuplex                   -- flag for duplex printing
            dwOptimization            -- optimization flags
            pDevmode                  -- pointer to devmode for changing the copy count
            pData                     -- needed for status and the handle of the event: pause, resume etc.

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    DWORD              dwLastPageNumber = 1, dwPageNumber, dwRemainingCopies;
    BOOL               bReturn = FALSE;

    BOOL               bCollate                   = pEMFAttr->bCollate;
    DWORD              dwJobNumberOfCopies        = pEMFAttr->dwJobNumberOfCopies;
    DWORD              dwDrvNumberOfCopies        = pEMFAttr->dwDrvNumberOfCopies;
    DWORD              dwDrvNumberOfPagesPerSide  = pEMFAttr->dwDrvNumberOfPagesPerSide;
    PFPRINTONESIDEFORWARDEMF   pfPrintOneSideForwardEMF = NULL;

    if ( 1              != dwDrvNumberOfPagesPerSide &&
         kRightThenDown != pEMFAttr->eNupDirection )
    {
        //
        // Special treatment for postscript based printers for non-traditional nup.
        //
        pfPrintOneSideForwardEMF = PrintOneSideForwardForDriverEMF;
    }
    else
    {
        pfPrintOneSideForwardEMF = PrintOneSideForwardEMF;
    }


    // Keep printing as long as the spool file contains EMF handles.
    while (dwLastPageNumber) {

        //
        // If the print processor is paused, wait for it to be resumed
        //
        if (pData->fsStatus & PRINTPROCESSOR_PAUSED) {
            WaitForSingleObject(pData->semPaused, INFINITE);
        }

        dwPageNumber = dwLastPageNumber;

        if (bCollate) {

           dwLastPageNumber = pfPrintOneSideForwardEMF(hSpoolHandle,
                                                     hPrinterDC,
                                                     pEMFAttr,
                                                     bDuplex,
                                                     dwOptimization,
                                                     dwPageNumber,
                                                     &bReturn,
                                                     pDevmode);
        } else {

           dwRemainingCopies = dwJobNumberOfCopies;

           while (dwRemainingCopies) {

               if (dwRemainingCopies <= dwDrvNumberOfCopies) {
                  SetDrvCopies(hPrinterDC, pDevmode, dwRemainingCopies);
                  dwRemainingCopies = 0;
               } else {
                  SetDrvCopies(hPrinterDC, pDevmode, dwDrvNumberOfCopies);
                  dwRemainingCopies -= dwDrvNumberOfCopies;
               }

               if (((dwLastPageNumber = pfPrintOneSideForwardEMF(hSpoolHandle,
                                                                hPrinterDC,
                                                                pEMFAttr,
                                                                bDuplex,
                                                                dwOptimization,
                                                                dwPageNumber,
                                                                &bReturn,
                                                                pDevmode))
                       == 0) &&
                   !bReturn) {

                    goto CleanUp;
               }
           }
        }
    }

CleanUp:

    return bReturn;
}

BOOL
PrintOneSideReverseForDriverEMF(
    _In_  HANDLE                hSpoolHandle,
    _In_  HDC                   hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_  PPAGE_NUMBER          pHead,
    _In_  BOOL                  bDuplex,
    _In_  DWORD                 dwOptimization,
    _In_  LPDEVMODE             pDevmode)

/*++
Function Description: PrintOneSideReverseForDriverEMF plays the EMF pages on the next
                      physical page, in the order dictated by pHead for the driver which does the
                      Nup transformations.

Parameters: hSpoolHandle           -- handle the spool file handle
            hPrinterDC             -- handle to the printer device context
            pEMFAttr               --
            pHead                  -- Linked list of page numbers
            bDuplex                -- flag to indicate duplex printing
            dwOptimization         -- optimization flags
            pDevmode               -- devmode with resolution settings

Return Values:  TRUE if successful
                FALSE otherwise
--*/
{
    return PrintOneSheetPreDeterminedForDriverEMF (
                                    hSpoolHandle,
                                    hPrinterDC,
                                    pEMFAttr,
                                    bDuplex,
                                    pHead,
                                    dwOptimization,
                                    NULL,
                                    pDevmode);
}



BOOL
PrintEMFInPredeterminedOrder(
    _In_        HANDLE                  hSpoolHandle,
    _In_        HDC                     hPrinterDC,
    _In_        PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_opt_    PPAGE_NUMBER            pHead,
    _In_        BOOL                    bDuplex,
    _In_        DWORD                   dwOptimization,
    _In_        LPDEVMODEW              pDevmode,
    _In_        PPRINTPROCESSORDATA     pData)

/*++
Function Description: PrintEMFInPredeterminedOrder plays the EMF pages in the order dictated by pHead.

Parameters: hSpoolHandle           -- handle the spool file handle
            hPrinterDC             -- handle to the printer device context
            pEMFAttr               -- Job Attributes
            pHead                  -- pointer to a linked list containing the starting
                                       page numbers for each of the sides
            bDuplex                -- flag to indicate duplex printing
            dwOptimization         -- optimization flags
            pDevmode               -- pointer to devmode for changing the copy count
            pData                  -- needed for status and the handle of the event: pause, resume etc.

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    DWORD         dwPageNumber, dwRemainingCopies;
    BOOL          bReturn = FALSE;

    /*++
            dwJobNumberOfCopies    -- number of copies of the job to be printed
            dwDrvNumberOfCopies    -- number of copies that the driver can print
            bCollate               -- flag for collating the copies
    --*/

    DWORD   dwDrvNumberOfCopies        = pEMFAttr->dwDrvNumberOfCopies;
    DWORD   dwJobNumberOfCopies        = pEMFAttr->dwJobNumberOfCopies;
    BOOL    bCollate                   = pEMFAttr->bCollate;
    DWORD   dwDrvNumberOfPagesPerSide  = pEMFAttr->dwDrvNumberOfPagesPerSide;
    PFPRINTONESIDEREVERSEEMF   pfPrintOneSideReverseEMF = NULL;

    if (!pHead) {
        ODS(("pHead is NULL.\n"));
        bReturn = FALSE;
        goto CleanUp;
    }

    //
    // This function is called for Booklet, Reverse Printing in which printer does
    // n-up, and reverse printing in which print processor simulates n-up. So lets
    // select which of the cases it is called for, and setup the appropriate functions.
    //
    if ( pEMFAttr->bBookletPrint )
    {
        pfPrintOneSideReverseEMF = PrintOneSideBookletEMF;
    }
    else if ( dwDrvNumberOfPagesPerSide != 1 )
    {
        //
        // If the printer can handle n-up
        //
        pfPrintOneSideReverseEMF = PrintOneSideReverseForDriverEMF;
    }
    else
    {
        pfPrintOneSideReverseEMF = PrintOneSideReverseEMF;
    }


    // play the sides in reverse order
    while (pHead) {
        //
        // If the print processor is paused, wait for it to be resumed
        //
        if (pData->fsStatus & PRINTPROCESSOR_PAUSED) {
            WaitForSingleObject(pData->semPaused, INFINITE);
        }

        // set the page number
        dwPageNumber = pHead->dwPageNumber;

        if (bCollate) {

           if (!pfPrintOneSideReverseEMF(hSpoolHandle,
                                         hPrinterDC,
                                         pEMFAttr,
                                         pHead,
                                         bDuplex,
                                         dwOptimization,
                                         pDevmode)) {
               goto CleanUp;
           }
        } else {

           dwRemainingCopies = dwJobNumberOfCopies;

           while (dwRemainingCopies) {

               if (dwRemainingCopies <= dwDrvNumberOfCopies) {
                  SetDrvCopies(hPrinterDC, pDevmode, dwRemainingCopies);
                  dwRemainingCopies = 0;
               } else {
                  SetDrvCopies(hPrinterDC, pDevmode, dwDrvNumberOfCopies);
                  dwRemainingCopies -= dwDrvNumberOfCopies;
               }

               if (!pfPrintOneSideReverseEMF(hSpoolHandle,
                                             hPrinterDC,
                                             pEMFAttr,
                                             pHead,
                                             bDuplex,
                                             dwOptimization,
                                             pDevmode)) {
                   goto CleanUp;
               }
           }
        }

        //
        // go to the next node(page) in the linked list. For duplex printing
        // we go ahead 2 nodes.
        //
        pHead = pHead->pNextSide;
        if (bDuplex && pHead) {
            pHead = pHead->pNextSide;
        }
    } //while pHead

    bReturn = TRUE;

CleanUp:

    return bReturn;
}

BOOL
PrintOneSideReverseEMF(
    _In_  HANDLE                hSpoolHandle,
    _In_  HDC                   hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_  PPAGE_NUMBER          pHead,
    _In_  BOOL                  bDuplex,
    _In_  DWORD                 dwOptimization,
    _In_  LPDEVMODE             pDevmode)

/*++
Function Description: PrintOneSideReverseEMF plays the EMF pages for the next physical page.

Parameters: hSpoolHandle           -- handle the spool file handle
            hPrinterDC             -- handle to the printer device context
            pEMFAttr               -- Job Attributes
            pHead                  -- List of pages in the order in which they have to be printed.
            bDuplex                -- flag to indicate duplex printing
            dwOptimization         -- optimization flags
            pDevmode               -- devmode with resolution settings

Return Values:  TRUE if successful
                FALSE otherwise
--*/
{
    DWORD         dwPageNumber, dwPageIndex, dwPageType;
    DWORD         dwSides, dwAngle;
    BOOL          bStartPageInitiated     = FALSE;

    BOOL          bReturn                 = FALSE, bNewDevmode;
    LPDEVMODEW    pCurrDM                 = NULL;
    HANDLE        hEMF                    = NULL;
    INT           dmOrientation           = pDevmode->dmOrientation;

    DWORD         dwNumberOfPagesPerSide  = pEMFAttr->dwNumberOfPagesPerSide;
    DWORD         dwTotalNumberOfPages    = pEMFAttr->dwTotalNumberOfPages;
    PPAGE_NUMBER  pHeadLogical            = pHead;


    if ( !pHead )
    {
        return TRUE;
    }

    dwSides = bDuplex ? 2 : 1;

    //
    // For each side, print all the logical pages.
    //
    for (;  dwSides;  --dwSides, pHead = pHead->pNextSide) {

        pHeadLogical = pHead;

        if ( NULL == pHeadLogical )
        {
            ODS(("Unexpected end of ReversePrintList %ws\n", pDevmode->dmDeviceName));
            goto CleanUp;
        }

        dwPageNumber = pHeadLogical->dwPageNumber;

        for (dwPageIndex = 1;
              ( (pHeadLogical != NULL) && (dwPageIndex <= dwNumberOfPagesPerSide));
              pHeadLogical = pHeadLogical->pNextLogPage, ++dwPageIndex)
        {
            dwPageNumber = pHeadLogical->dwPageNumber;

            if ( dwPageNumber == 0 )
            {
                dwPageNumber = 0xFFFFFFFF; //some big number.
            }

            //
            // Get PageHandle only if the page number is less than or equal to
            // total number of pages. A bigger  page number indicates
            // empty/non-existent page for which there can be no handle.
            //
            if (dwPageNumber <= dwTotalNumberOfPages )
            {
                if ((hEMF = GdiGetPageHandle(hSpoolHandle,
                                                 dwPageNumber,
                                                 &dwPageType))
                    == NULL) {

                    ODS(("GdiGetPageHandle failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                    goto CleanUp;
                }
            }

            dwAngle = EMF_DEGREE_90;
            if (dwPageIndex == 1)
            {
                //
                // We now need to give a ResetDC call. ResetDC should be given
                // at most once on begining of each side. If side is empty, no
                // ResetDC is done. If side is half filled (e.g. n-up = 2 but
                // only one page has to be printed), then we do ResetDC for
                // first available valid page.
                //
                DWORD dwPageNumberForResetDC = 0xFFFFFFFF;
                PPAGE_NUMBER pHeadMoving  = NULL; //moves thru the list.

                if ( dwPageNumber <= dwTotalNumberOfPages )
                {
                    dwPageNumberForResetDC = dwPageNumber;
                }
                else
                {
                    //
                    // Go to first non-empty logical page.
                    // Note: Empty page means a blank page generated by print processor.
                    //
                    for ( pHeadMoving = pHeadLogical; pHeadMoving; pHeadMoving = pHeadMoving->pNextLogPage )
                    {
                        dwPageNumberForResetDC = pHeadMoving->dwPageNumber;

                        if ( dwPageNumberForResetDC > 0 && dwPageNumberForResetDC <= dwTotalNumberOfPages )
                        {
                            break; //Valid page number found.
                        }
                    } //for
                }

                if ( dwPageNumberForResetDC > 0 && dwPageNumberForResetDC <= dwTotalNumberOfPages )
                {
                    // Process devmodes in the spool file
                    if (!ResetDCForNewDevmode(hSpoolHandle,
                                                 hPrinterDC,
                                                 pEMFAttr,
                                                 dwPageNumberForResetDC,
                                                 FALSE,
                                                 dwOptimization,
                                                 &bNewDevmode,
                                                 pDevmode,
                                                 &pCurrDM) )
                    {
                        goto CleanUp;
                    }
                }

                //
                // Lets do a startPage. This has to be done even if side is empty.
                // Except if printer doesn't want needless blank pages
                //
                if ( dwPageNumber >  dwTotalNumberOfPages    && // A quick check to see if printing a blank page.
                     TRUE         == BAnyReasonNotToPrintBlankPage(pEMFAttr, dwTotalNumberOfPages) &&
                     TRUE         == BIsEveryPageOnThisSideBlank(pHeadLogical, dwTotalNumberOfPages)
                   )
                {
                    // All conditions indicate we should not print blank page. So no reason to do a StartPage
                }
                else
                {
                    if ( !GdiStartPageEMF(hSpoolHandle)) {

                       ODS(("StartPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                       goto CleanUp;
                    }

                    bStartPageInitiated = TRUE;
                }

                if (pCurrDM)
                    dmOrientation = pCurrDM->dmOrientation;
            }

            else if (dwNumberOfPagesPerSide > 1 && dwPageNumber <= dwTotalNumberOfPages)
            {
                // in case of orientation switch we need to keep track of what
                // we started with and what it is now
                if (GdiGetDevmodeForPagePvt(hSpoolHandle,
                                            dwPageNumber,
                                            &pCurrDM,
                                            NULL))
                {
                    if (pCurrDM && pCurrDM->dmOrientation != dmOrientation)
                    {
                        dwAngle = EMF_DEGREE_SWAP | EMF_DEGREE_90;
                        BUpdateAttributes(hPrinterDC, pEMFAttr);
                    }
                }
            }

            if ( dwPageNumber <= dwTotalNumberOfPages )
            {
                _Analysis_assume_(hEMF != NULL);

                if (!PlayEMFPage(hSpoolHandle,
                                    hPrinterDC,
                                    hEMF,
                                    pEMFAttr,
                                    dwPageIndex,
                                    dwAngle)) {

                    ODS(("PlayEMFPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                    goto CleanUp;
                }
            }
         }


        //
        // All pages on the side have been printed. If some sheets are blank
        // a StartPageEMF was sent but PlayEMFPage was not done.
        //
        if (bStartPageInitiated)
        {
            if (!GdiEndPageEMF(hSpoolHandle, dwOptimization)) {
                ODS(("EndPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                goto CleanUp;
            }
            bStartPageInitiated = FALSE;
        }

    } //for dwSides

    bReturn = TRUE;

CleanUp:

    return bReturn;
}


BOOL
PrintOneSideBookletEMF(
    _In_  HANDLE                hSpoolHandle,
    _In_  HDC                   hPrinterDC,
    _In_  PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_  PPAGE_NUMBER          pHead,
    _In_  BOOL                  bDuplex,
    _In_  DWORD                 dwOptimization,
    _In_  LPDEVMODE             pDevmode)

/*++
Function Description: PrintOneSideBookletEMF prints one page of the booklet job.

Parameters: hSpoolHandle           -- handle the spool file handle
            hPrinterDC             -- handle to the printer device context
            pEMFAttr               -- Job Attributes
            pHead                  -- List of pages in the order in which they have to be printed.
            bDuplex                -- Whether duplex is supported in hardware
            dwOptimization         -- optimization flags
            pDevmode               -- devmode with resolution settings

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    DWORD       dwPageIndex, dwAngle = 0, dwPageType, dwPageNumber = 0;
    HANDLE      hEMF = NULL;
    LPDEVMODEW  pCurrDM;
    BOOL        bReturn = FALSE ,bNewDevmode;
    INT         dmOrientation;

    DWORD       dwSides;
    PPAGE_NUMBER pHeadLogical, pHeadMoving;

    DWORD       dwNumberOfPagesPerSide    = pEMFAttr->dwNumberOfPagesPerSide;
    DWORD       dwDrvNumberOfPagesPerSide = pEMFAttr->dwDrvNumberOfPagesPerSide;
    DWORD       dwTotalNumberOfPages      = pEMFAttr->dwTotalNumberOfPages;
    DWORD       dwDuplexMode              = pEMFAttr->dwDuplexMode;
    BOOL        bDeviceNup                = dwDrvNumberOfPagesPerSide > 1 ? TRUE: FALSE; //hware supported nup
    DWORD       dwLimit                   = min(dwTotalNumberOfPages,dwDrvNumberOfPagesPerSide);

    if ( !pHead )
    {
        return TRUE;
    }

    //
    // There are 2 n-up options here. One is when the printer supports it
    // (dwDrvNumberOfPagesPerSide > 1) and the other is when print processor
    // simulates it (dwNumberOfPagesPerSide > 1). If both of them are
    // present, then it is an unexpected case. For the time being, I'll let
    // the printer n-up be superior.
    //
    if ( dwDrvNumberOfPagesPerSide > 1 ) //cud also hvae used bDeviceNup
    {
        if ( dwNumberOfPagesPerSide > 1)
        {
            ODS(("PrintOneSideBookletEMF... Both printer hardware n-up and simulated n-up present\n"));
        }

        dwNumberOfPagesPerSide           = dwDrvNumberOfPagesPerSide;
    }

    dwSides = bDuplex ? 2 : 1;

    //
    // Lets reset the devmode for the page number that is
    // first in the pHead list. Note that sometimes,
    // this page number is larger than dwTotalNumberOfPages
    // (to indicate an empty page).
    // Therefore, we need to keep moving pHead to next node
    // till we get a valid dwPageNumber
    //
    for ( pHeadMoving = pHead; pHeadMoving ; pHeadMoving = pHeadMoving->pNextSide)
    {
        for ( pHeadLogical = pHeadMoving; pHeadLogical; pHeadLogical = pHeadLogical->pNextLogPage )
        {
            dwPageNumber = pHeadLogical->dwPageNumber;

            if ( dwPageNumber > 0 && dwPageNumber <= dwTotalNumberOfPages )
            {
                break; //Valid page number found.
            }
        }

        if ( pHeadLogical != NULL )
        {
            break;
        }
    }

    if ( pHeadMoving == NULL )
    {
        ODS (("pHeadMoving is NULL.\n"));
        return FALSE;
    }

    // Process devmodes in the spool file
    if (!ResetDCForNewDevmode(hSpoolHandle,
                              hPrinterDC,
                              pEMFAttr,
                              dwPageNumber,
                              FALSE,
                              dwOptimization,
                              &bNewDevmode,
                              pDevmode,
                              &pCurrDM)) {
        goto CleanUp;
    }

    if (pCurrDM)
        dmOrientation = pCurrDM->dmOrientation;
    else
        dmOrientation = pDevmode->dmOrientation;

    for (;  dwSides && pHead;  --dwSides, pHead = pHead->pNextSide)
    {
        pHeadLogical = pHead;
        dwPageNumber = pHeadLogical->dwPageNumber;

        for (dwPageIndex = 1;
              ( (pHeadLogical != NULL) && (dwPageIndex <= dwNumberOfPagesPerSide));
              pHeadLogical = pHeadLogical->pNextLogPage, ++dwPageIndex)
        {
            dwPageNumber = pHeadLogical->dwPageNumber;

            if ( dwPageNumber == 0 )
            {
                dwPageNumber = 0xFFFFFFFF; //some big number.
            }

            if (dwPageNumber <= dwTotalNumberOfPages)
            {
                if ((hEMF = GdiGetPageHandle(hSpoolHandle,
                                              dwPageNumber,
                                              &dwPageType))
                    == NULL)
                {
                     ODS(("GdiGetPageHandle failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                     goto CleanUp;
                }
            }

            if (dwPageIndex == 1 || bDeviceNup)
            {
                if (!GdiStartPageEMF(hSpoolHandle))
                {
                     ODS(("StartPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                    goto CleanUp;
                }
            }

            //
            // Simulating n-up in print processor. Does not support
            // device n-up.
            //
            if (dwPageNumber <= dwTotalNumberOfPages)
            {
                _Analysis_assume_(hEMF != NULL);

                dwAngle = 0; //reset it for every page.

                //
                // in case of orientation switch we need to keep track of what
                // we started with and what it is now
                //
                if (GdiGetDevmodeForPagePvt(hSpoolHandle,
                                            dwPageNumber,
                                            &pCurrDM,
                                            NULL))
                {
                    if (pCurrDM && pCurrDM->dmOrientation != dmOrientation)
                    {
                        dwAngle |= EMF_DEGREE_SWAP;
                        BUpdateAttributes(hPrinterDC, pEMFAttr);
                    }
                }

                //
                // For flip on long edge, the reverse side has to be rotated 180 degree
                // w.r.t. the 1st side
                //
                if ( (dwSides == 1) && (dwDuplexMode == EMF_DUP_VERT) )
                {
                    dwAngle |= EMF_DEGREE_270;

                    if ( bDeviceNup && (pDevmode->dmOrientation == DMORIENT_LANDSCAPE) )
                    {
                        dwAngle |= EMF_DEGREE_SWAP;
                    }
                }
                else
                {
                    // EMF_DUP_HORZ or 1st side
                    dwAngle |= EMF_DEGREE_90;
                }

                if (!PlayEMFPage(hSpoolHandle,
                                  hPrinterDC,
                                  hEMF,
                                  pEMFAttr,
                                  dwPageIndex,
                                  dwAngle)) {

                     ODS(("PlayEMFPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                     goto CleanUp;
                }
            }

            if (dwPageIndex == dwNumberOfPagesPerSide || bDeviceNup) {

                if (!GdiEndPageEMF(hSpoolHandle, dwOptimization)) {
                     ODS(("EndPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                     goto CleanUp;
                }
            }

            if ( bDeviceNup && pHeadLogical == NULL && dwPageIndex<=dwLimit )
            {
                //
                // Suppose the n-up is 4 (dwLimit is 4) and the n-up is done by the device
                // ( dwDrvNumberOfPagesPerSide > 1)
                // i.e. device needs 4 pages to finish printing a side and eject paper.
                // Suppose pHeadLogical list has only 2 pages (i.e. the pHeadLogical is now NULL after
                // going thru above loop).
                // So we need to send the printer 2 empty pages (logical pages)
                //
                BPlayEmptyPages(hSpoolHandle, (dwLimit-dwPageIndex+1), dwOptimization);
            }
       }
    }

    //
    // dwSides == 0 implies that there were as many sides in the list
    // as we expected. If dwSides != 0, then the linked list ran out
    // of nodes before we expect. This should never happen.
    //
    ASSERT(dwSides == 0);

    bReturn = TRUE;

CleanUp:

    return bReturn;
}

BOOL
PrintEMFSingleCopy(
    _In_        HANDLE                hSpoolHandle,
    _In_        HDC                   hPrinterDC,
    _In_        PEMF_ATTRIBUTE_INFO   pEMFAttr,
    _In_opt_    PPAGE_NUMBER          pHead,
    _In_        DWORD                 dwOptimization,
    _In_        LPDEVMODEW            pDevmode,
    _In_        PPRINTPROCESSORDATA   pData)

/*++
Function Description: PrintEMFSingleCopy plays one copy of the job on hPrinterDC.

Parameters: hSpoolHandle           -- handle the spool file handle
            hPrinterDC             -- handle to the printer device context
            pEMFAttr               -- Job Attributes
            pHead                  -- pointer to a linked list containing the starting
                                       page numbers for each of the sides
                                      (valid for booklet,reverse printing)
            dwOptimization         -- optimization flags
            pDevmode               -- pointer to devmode for changing the copy count
            pData                  -- needed for status and the handle of the event: pause, resume etc.

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    /*++
            dwDrvNumberOfPagesPerSide -- number of pages the driver will print per side
            dwNumberOfPagesPerSide -- number of pages to be printed per side by the print
                                       processor
            dwTotalNumberOfPages   -- number of pages in the document
            bCollate               -- flag for collating the copies
            bBookletPrint          -- flag for booklet printing
            dwDuplexMode           -- duplex printing mode (none|horz|vert)
    --*/
    BOOL  bReverseOrderPrinting      = pEMFAttr->bReverseOrderPrinting;
    BOOL  bBookletPrint              = pEMFAttr->bBookletPrint;
    DWORD dwDuplexMode               = pEMFAttr->dwDuplexMode;

    BOOL  bDuplex = (dwDuplexMode != EMF_DUP_NONE);

    if (bBookletPrint || bReverseOrderPrinting) {

          // Reverse printing or booklet
          return PrintEMFInPredeterminedOrder(hSpoolHandle,
                                 hPrinterDC,
                                 pEMFAttr,
                                 pHead,
                                 bDuplex,
                                 dwOptimization,
                                 pDevmode,
                                 pData);
    } else {

       // Normal printing
       return PrintForwardEMF(hSpoolHandle,
                              hPrinterDC,
                              pEMFAttr,
                              bDuplex,
                              dwOptimization,
                              pDevmode,
                              pData);
    }
}

_Success_(return)
BOOL
GetStartPageListForwardForDriver(
    _In_            PEMF_ATTRIBUTE_INFO    pEMFAttr,
    _In_            LPDEVMODEW             pDevmode,
    _Outptr_        PPAGE_NUMBER          *ppMemoryHead,
    _Outptr_        PPAGE_NUMBER          *ppPageListHead,
    _In_            DWORD                  dwSmallestPageOnSheet
    )
{

    DWORD           dwPageIndex;
    PPAGE_NUMBER    pHeadMoving                 = NULL; //moves thru the list.
    PPAGE_NUMBER    pHeadNewSide                = NULL; //ptr keeps track of new sides.
    BOOL            bReturn                     = FALSE;
    DWORD           dwNumberOfPagesPerSide      = pEMFAttr->dwNumberOfPagesPerSide;
    DWORD           dwDrvNumberOfPagesPerSide   = pEMFAttr->dwDrvNumberOfPagesPerSide;
    DWORD           dwDuplexMode                = pEMFAttr->dwDuplexMode;
    DWORD           dwNumberSides               = 1;
    DWORD           dwMaxLogicalPages           = 0;                       // MaxLogicalPagesPerSheet
    ENupDirection   eNupDirection               = pEMFAttr->eNupDirection;
    PDWORD          pNupDirectionBasedPageOrder = NULL;
    DWORD           dwMemoryForPageList         = 0;
    DWORD           dwSide                      = 1; //Loop counter

    //
    // This function assumes that dwTotalNumberOfPages is set to a valid
    // value.
    //
    if ( ppMemoryHead         == NULL       ||
         ppPageListHead       == NULL
       )
    {
        return FALSE;
    }

    *ppMemoryHead = NULL;
    *ppPageListHead = NULL;

    //
    // There are 2 n-up options here. One is when the printer supports it
    // (dwDrvNumberOfPagesPerSide) and the other is when print processor
    // simulates it (dwNumberOfPagesPerSide). If both of them are
    // present, then it is an unexpected case. For the time being, I'll let
    // the printer n-up be superior.
    //
    if ( dwDrvNumberOfPagesPerSide <= 1 )
    {
        ODS(("This function is only to be called for driver doing n-up, not for simulated n-up"));
    }

    dwNumberOfPagesPerSide  = dwDrvNumberOfPagesPerSide;
    dwNumberSides           = (dwDuplexMode == EMF_DUP_NONE) ?1:2 ;

    if ( ! SUCCEEDED( DWordMult(dwNumberOfPagesPerSide, dwNumberSides      , &dwMaxLogicalPages   ) ) ||
         ! SUCCEEDED( DWordMult(dwMaxLogicalPages     , sizeof(PAGE_NUMBER), &dwMemoryForPageList ) )
       )
    {
        ODS(("GetStartPageListForwardForDriver - Possible Arithmetic overflow "));
        goto CleanUp;
    }

    //
    // allocate the memory for all logical pages. AllocSplMem also zero inits it.
    //
    if ((pHeadMoving = (PPAGE_NUMBER)AllocSplMem( dwMemoryForPageList ))
        == NULL)
    {
        ODS(("GetStartPageListForwardForDriver - Run out of memory"));
        goto CleanUp;
    }

    *ppMemoryHead               = pHeadMoving;
    *ppPageListHead             = pHeadMoving;
    pHeadNewSide                = pHeadMoving;
    pNupDirectionBasedPageOrder = GetPlaybackPageOrderForDriverNup(dwNumberOfPagesPerSide,
                                                                   eNupDirection,
                                                                   pDevmode->dmOrientation);

    //
    // The following loop will arrange pages as below.
    // Assuming
    //      1) 5 pages with 4-up kDownThenLeft
    //
    //
    //        ------------------pNextSide
    //       |
    //       |
    //       \/
    //     3 -->  7
    //     |      | <-- pNextLogicalPage
    //     |      |
    //     \/     \/
    //     1      5
    //     |      | <-- pNextLogicalPage
    //     |      |
    //     \/     \/
    //     4      8
    //     |      | <-- pNextLogicalPage
    //     |      |
    //     \/     \/
    //     2      6
    // Pages 7,8,6 are marked but they should be treated as empty pages.
    // since maximum pages are 5. But note that in forward printing, we don't
    // know the maximum pages. So it should be assumed that when print processor
    // asks for a page n and Gdi can't provide it, it means that page doesn't exist
    // i.e. the total number of pages in job is less than n.
    //


    //
    // While loop advances a side of the sheet.
    //
    for ( dwSide = 1; dwSide <= dwNumberSides; dwSide++, dwSmallestPageOnSheet += dwNumberOfPagesPerSide )
    {
        //
        // For loop fills in information for each page in the side.
        //
        for (dwPageIndex = 1;
            (dwPageIndex <= dwNumberOfPagesPerSide) ;
            ++dwPageIndex)
        {
            // Create a node for the start of a side
            if (dwPageIndex == 1)
            {
                //
                // A new side starts here. i.e. this page has to
                // be printed on a new side.
                //

                pHeadMoving->pNextSide    = NULL;

                if ( 2 == dwSide )
                {
                    pHeadNewSide->pNextSide   = pHeadMoving;
                }
            }
            else
            {
                //
                // This is a new logical page to be printed
                // on the same side as the previous logical page.
                //
                (pHeadMoving-1)->pNextLogPage = pHeadMoving;
            }

            //
            // Note page number here can exceed max pages in the document (dwTotalNumberOfPages).
            // The max. page possible here is dwMaxLogicalPages which can be
            // greater than dwTotalNumberOfPages. e.g. in cond 2 above, dwTotalNumberOfPages = 5
            // but dwMaxLogicalPages = 6.
            //

            pHeadMoving->dwPageNumber = dwSmallestPageOnSheet + pNupDirectionBasedPageOrder[dwPageIndex-1] - 1;

            pHeadMoving++;
        } //end of for.

     } //end of while

    bReturn = TRUE;

CleanUp:

    if ( FALSE == bReturn )
    {
        if ( *ppMemoryHead )
        {
            FreeSplMem (*ppMemoryHead);
        }

        *ppMemoryHead   = NULL;
        *ppPageListHead = NULL;
    }

    return bReturn;
}

_Success_(return)
BOOL
GetStartPageListBooklet(
    _In_            PEMF_ATTRIBUTE_INFO    pEMFAttr,
    _Outptr_result_maybenull_ PPAGE_NUMBER          *ppMemoryHead,
    _Outptr_result_maybenull_ PPAGE_NUMBER          *ppPageListHead)

/*++
Function Description: GetStartPageListBooklet generates an ordered list of page numbers which
                      should appear on each side of the job. This takes
                      into consideration the ResetDC calls that may appear before the
                      end of the page. The list generated by GetStartPageListBooklet is used
                      to play the job in BookletMode.

Parameters: hSpoolHandle           -- handle the spool file handle
            pEMFAttr               -- Job Attributes
            ppPageListHead         -- pointer to a pointer to a linked list containing the
                                       starting page numbers for each of the sides
            ppMemoryHead           -- pointer to pointer to memory that has to be releazed by
                                      calling function after it is done processing the pages in
                                      ppPageListHead

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    PPAGE_NUMBER pMemoryHead        = NULL;
    PPAGE_NUMBER pHeadMoving        = NULL;
    BOOL         bReturn            = FALSE;
    DWORD        dwTotalSheets      = 1;
    DWORD        dwMaxLogicalPages  = 4; // = dwTotalSheets * 4.
    DWORD        dwMemoryForPageList= 0;


    DWORD        dwTotalNumberOfPages  = pEMFAttr->dwTotalNumberOfPages;
    EBookletMode eBookletMode          = pEMFAttr->eBookletMode;
    DWORD        dwDuplexMode          = pEMFAttr->dwDuplexMode;
    ULONG        ulNodeNumber          = 0;


    //
    // Total number of pages in the job should have been initialized properly.
    // Also ppHead should be valid.
    //
    if ( dwTotalNumberOfPages == 0          ||
         dwTotalNumberOfPages == 0xFFFFFFFF ||
         ppMemoryHead         == NULL       ||
         ppPageListHead       == NULL   )
    {
        ODS(("Parameters to GetStartPageListBooklet are not proper.\n"));
        return FALSE;
    }

    *ppMemoryHead   = NULL;
    *ppPageListHead = NULL;

    //
    // 1) Booklet printing needs some special page ordering.
    // 2) The n-up is always 2.
    // 3) There are 2 n-up options. One is when the printer supports it
    // (dwDrvNumberOfPagesPerSide) and the other is when print processor
    // simulates it (dwNumberOfPagesPerSide). If both of them are
    // present, then it is an unexpected case. For the time being, I'll let
    // the printer n-up be superior.
    //

    //
    // Assume dwTotalSheets is the minimum number of sheets required for doing booklet
    // We print 2 logical pages on each side and we print on
    // both sides. i.e. on a single sheet of paper we print 4 logical
    // pages. So let dwMaxLogicalPages = dwTotalSheets*4.
    // Not all the dwMaxLogicalPages need to be printed (e.g a book can have 3 pages,
    // the 4th page is just empty page).
    // So we need a linked list with dwMaxLogicalPages nodes.
    //
    // The book can be left edge binding (for latin and most other languages)
    // or right edge binding (for hebrew).
    //

    dwTotalSheets = dwTotalNumberOfPages/4;

    if ( dwTotalSheets * 4 != dwTotalNumberOfPages )
    {
        dwTotalSheets++;
    }

    if ( ! SUCCEEDED( DWordMult(dwTotalSheets    , 4                  , &dwMaxLogicalPages  ) ) ||
         ! SUCCEEDED( DWordMult(dwMaxLogicalPages, sizeof(PAGE_NUMBER), &dwMemoryForPageList) )
       )
    {
        ODS(("GetStartPageListBooklet - Possible Arithmetic overflow."));
        goto CleanUp;
    }

    //
    // AllocSplMem initializes the memory to 0.
    //
    if ((pMemoryHead = (PPAGE_NUMBER)AllocSplMem( dwMemoryForPageList )) == NULL) {
        ODS(("GetStartPageListBooklet - Run out of memory"));
        goto CleanUp;
    }

    //
    // Now lets start populating the linked list.
    // For booklet printing of 11 pages, we need 3 sheets. The ordering is
    // (Assuming left edge binding)
    // blank (page 12), 1 - first sheet
    // 11, 2              - back of 1st sheet
    // 10,3               - second sheet
    // 9 ,4               - back of 2nd sheet
    // 8 ,5               - third sheet
    // 7, 6               - back of 3rd sheet.
    //
    // The following loop arranges page numbers. The arrangement can be visualized
    // as follows. (for left edge binding, job with 7 pages, to be printed on 2 sheets).
    // pages 8,1 on 1st side 1st sheet
    // pages 7,2 on reverse side of 1st sheet.
    // pages 6,3 on 1st side of 2nd sheet.
    // pages 5,4 on reverse side of 2nd sheet.
    //
    //                  -----------------------------------(pNextSide)
    //                  |     |
    //                  |     |
    // *ppPageListHead  |     |
    //        |         |     |
    //       \/        \/    \/
    //        8 --> 7  --->6 ----> 5
    //        |     |      |       |  <---- (pNextLogicalPage)
    //       \/    \/      \/      \/
    //        1     2      3       4
    //

    *ppMemoryHead = *ppPageListHead = pHeadMoving = pMemoryHead;

    if ( eBookletMode == kNoBooklet ||
         eBookletMode == kBookletLeftEdge)
    {
        for ( ulNodeNumber = 0 ; ulNodeNumber < dwMaxLogicalPages/2; ulNodeNumber++)
        {
            pHeadMoving->dwPageNumber = dwMaxLogicalPages - ulNodeNumber;
            pHeadMoving->pNextLogPage = pHeadMoving + 1;
            pHeadMoving->pNextSide    = pHeadMoving + 2;
            pHeadMoving++;

            pHeadMoving->dwPageNumber = ulNodeNumber+1;
            pHeadMoving->pNextSide = NULL;
            pHeadMoving->pNextLogPage = NULL;
            pHeadMoving++;
        }

        (pHeadMoving-2)->pNextSide = NULL;
    }
    else if ( eBookletMode == kBookletRightEdge ) //Right Edge binding.
    {
        for ( ulNodeNumber = 0 ; ulNodeNumber < dwMaxLogicalPages/2; ulNodeNumber++)
        {
            pHeadMoving->dwPageNumber = ulNodeNumber+1;
            pHeadMoving->pNextLogPage = pHeadMoving + 1;
            pHeadMoving->pNextSide    = pHeadMoving + 2;
            pHeadMoving++;

            pHeadMoving->dwPageNumber = dwMaxLogicalPages - ulNodeNumber;
            pHeadMoving->pNextSide    = NULL;
            pHeadMoving->pNextLogPage = NULL;
            pHeadMoving++;
        }

        (pHeadMoving-2)->pNextSide = NULL;
    }


    if ( dwDuplexMode == EMF_DUP_HORZ ) //Flip on Short Edge.
    {
        // Q. In booklet, there is no concept of long edge or short edge,
        //    why do we even need this piece of code.
        // A. The driver UI gives user option to select both long/short edge with booklet.
        //    Whatever edge is selected by user, the printer is initialized appropriately
        //    by the renderer. So print processor has to play pages depending
        //    on how printer is rotates its pages.
        //    If driver config would not allow user to select the edge, then
        //    this code may not be required.
        //
        // The pages need to be re-arranged as follows.
        // If the pages are not flipped, the printout is not proper.
        // The sheet that has the first page should not be flipped.
        // For Forward printing, the *ppPageListHead points to the first
        // logical page. So we start switching from *ppPageListHead->pNextSide
        // For Reverse Printing, *ppPageListHead points to the last sheet.'
        // The first sheet has the logical page number 1. There are even
        // logical pages (numofsheets * 4) and therefore there are even sides
        // ( (numofsheets * 4)/2). The last side has page number 1. This side
        // cannot be switched. So we have to start switching right from
        // *ppPageListHead.
        //
        pHeadMoving = *ppPageListHead;


            //                  -----------------------------------(pNextSide)
            //                  |     |
            //                  |     |
            // *ppPageListHead  |     |
            //        |         |     |
            //       \/        \/    \/
            //        8 --> 2  --->6 ----> 4
            //        |     |      |       |  <---- (pNextLogicalPage)
            //       \/    \/      \/      \/
            //        1     7      3       5
            // Note pages 2,7 have been flipped, and so have 4,5
            //
            while ( pHeadMoving && pHeadMoving->pNextSide )
            {
                pHeadMoving                 = pHeadMoving->pNextSide;

                if ( pHeadMoving->pNextLogPage )
                {
                    DWORD      dwTempPageNumber = pHeadMoving->dwPageNumber;
                    pHeadMoving->dwPageNumber   = pHeadMoving->pNextLogPage->dwPageNumber;
                    pHeadMoving->pNextLogPage->dwPageNumber = dwTempPageNumber;
                }

                pHeadMoving = pHeadMoving->pNextSide;
            }
    }

    bReturn = TRUE;

CleanUp:

    if ( bReturn == FALSE)
    {
        if ( pMemoryHead )
        {
            FreeSplMem (pMemoryHead);
        }

        *ppMemoryHead   = NULL;
        *ppPageListHead = NULL;
    }

    return bReturn;
}

_Success_(return)
BOOL
GetStartPageListReverseOrder(
    _In_            HANDLE                 hSpoolHandle,
    _In_            PEMF_ATTRIBUTE_INFO    pEMFAttr,
    _In_            LPDEVMODEW             pDevmode,
    _Outptr_result_maybenull_        PPAGE_NUMBER          *ppMemoryHead,
    _Outptr_result_maybenull_        PPAGE_NUMBER          *ppPageListHead)

/*++
Function Description:
    GetStartPageListReverseOrder generates a list of the page numbers which
    should appear on the start of each side of the job. This takes
    into consideration the ResetDC calls that may appear before the
    end of the page. The list generated by GetStartPageListReverseOrder is used
    to play the job in reverse order.

Parameters:
    hSpoolHandle           -- handle the spool file handle
    pEMFAttr               -- Job Attributes
    ppMemoryHead           -- pointer to a pointer to allocated memory. This memory holds the page list.
    ppPageListHead         -- pointer to a pointer to a linked list containing the
                               starting page numbers for each of the sides
    dwTotalNumberOfPages   -- number of pages in the document
    dwNumberOfPagesPerSide -- number of pages to be printed per side by the print
                              processor

Return Values:  TRUE if successful
                FALSE otherwise
--*/

{
    DWORD         dwPageIndex, dwPageNumber   = 1;
    LPDEVMODEW    pCurrDM, pLastDM            = NULL;
    PPAGE_NUMBER  pHeadMoving                 = NULL; //moves thru the list.
    PPAGE_NUMBER  pHeadNewSide                = NULL; //ptr keeps track of new sides.
    BOOL          bReturn                     = FALSE;
    BOOL          bCheckDevmode               = FALSE;
    DWORD         dwTotalNumberOfPages        = pEMFAttr->dwTotalNumberOfPages;
    DWORD         dwNumberOfPagesPerSide      = pEMFAttr->dwNumberOfPagesPerSide;
    DWORD         dwDrvNumberOfPagesPerSide   = pEMFAttr->dwDrvNumberOfPagesPerSide;
    DWORD         dwDuplexMode                = pEMFAttr->dwDuplexMode;
    ULONG         ulNumNodes                  = 0;
    DWORD         dwMaxLogicalPagesPerSheet   = 1;
    DWORD         dwTotalSheets               = 0;
    DWORD         dwMaxLogicalPages           = 0;
    ENupDirection eNupDirection               = pEMFAttr->eNupDirection;
    PDWORD        pNupDirectionBasedPageOrder = NULL;
    DWORD         dwMemoryForPageList         = 0;

    //
    // This function assumes that dwTotalNumberOfPages is set to a valid
    // value.
    //
    if ( dwTotalNumberOfPages == 0 ||
         dwTotalNumberOfPages == 0xFFFFFFFF ||
         ppMemoryHead         == NULL       ||
         ppPageListHead       == NULL    )
    {
        return FALSE;
    }

    *ppMemoryHead   = NULL;
    *ppPageListHead = NULL;

    //
    // There are 2 n-up options here. One is when the printer supports it
    // (dwDrvNumberOfPagesPerSide) and the other is when print processor
    // simulates it (dwNumberOfPagesPerSide). If both of them are
    // present, then it is an unexpected case. For the time being, I'll let
    // the printer n-up be superior.
    //
    if ( dwDrvNumberOfPagesPerSide > 1 )
    {
        if ( dwNumberOfPagesPerSide > 1)
        {
            ODS(("GetStartPageListReverseOrder..Both printer hardware n-up and simulated n-up present\n"));
        }
        dwNumberOfPagesPerSide           = dwDrvNumberOfPagesPerSide;
    }


    //
    // First see how many sheets of paper will be required. Some examples are.
    // Cond. Id       Condition.              Sheets required.
    // --------     ---------------         -------------------
    // 1.          5 pg, 1-up, no duplex        5
    // 2.          5 pg, 2-up, no duplex        3
    // 3.          5 pg, 2-up, duplex           2
    // 4.          7 pg, 4-up, duplex           1
    //
    // If dwDuplexMode is not zero, i.e. duplex is on, then we can fit double pages per sheet of paper.

    if ( ! SUCCEEDED ( DWordMult(dwNumberOfPagesPerSide,
                                 ((dwDuplexMode == EMF_DUP_NONE) ? 1 : 2) ,
                                 &dwMaxLogicalPagesPerSheet) )
       )
    {
        goto CleanUp;
    }

    dwTotalSheets             = dwTotalNumberOfPages/dwMaxLogicalPagesPerSheet;

    if ( dwTotalSheets * dwMaxLogicalPagesPerSheet != dwTotalNumberOfPages )
    {
        dwTotalSheets++;
    }

    if ( ! SUCCEEDED ( DWordMult(dwTotalSheets    , dwMaxLogicalPagesPerSheet, &dwMaxLogicalPages  ) ) ||
         ! SUCCEEDED ( DWordMult(dwMaxLogicalPages, sizeof(PAGE_NUMBER)      , &dwMemoryForPageList) )
       )
    {
        ODS(("GetStartPageListForwardForDriver - Possible Arithmetic overflow"));
        goto CleanUp;
    }

    //
    // allocate the memory for all logical pages. AllocSplMem also zero inits it.
    //
    if ((pHeadMoving = (PPAGE_NUMBER)AllocSplMem(dwMemoryForPageList))
        == NULL)
    {
        ODS(("GetStartPageListBooklet - Run out of memory"));
        goto CleanUp;
    }

    *ppMemoryHead     = pHeadMoving;
    pHeadMoving += (dwMaxLogicalPages - 1) ; //Go to the last block.
    pHeadNewSide = NULL;

    //
    // The following loop will arrange pages as below.
    // Assuming
    //      1) 5 pages with 2-up
    //      2) DifferentDevmodes() always fails.
    //
    //
    //                  ------------------pNextSide
    //                  |
    //                  |
    //                  \/
    //     7 -->  5  -->  3  -->  1
    //     |      |       |       |  <-- pNextLogicalPage
    //     |      |       |       |
    //     \/     \/      \/      \/
    //     8      6       4       2
    //
    // Pages 7,8,6 are marked but they should be treated as empty pages.
    // since maximum pages are 5.
    //


    pNupDirectionBasedPageOrder = GetPlaybackPageOrderForDriverNup(dwNumberOfPagesPerSide, eNupDirection, pDevmode->dmOrientation);

    //
    // While loop advances a side of the sheet.
    //
    while (dwPageNumber <= dwMaxLogicalPages && pHeadMoving >= *ppMemoryHead) {

        //
        // For loop fills in information for each page in the side.
        //
        for (dwPageIndex = 1;
            (dwPageIndex <= dwNumberOfPagesPerSide) && (dwPageNumber <= dwMaxLogicalPages);
            ++dwPageIndex, ++dwPageNumber)
        {

            if (bCheckDevmode && (dwPageNumber <= dwTotalNumberOfPages) )
            {
                // Check if the devmode has changed requiring a new page
                if (!GdiGetDevmodeForPagePvt(hSpoolHandle, dwPageNumber,
                                               &pCurrDM, NULL)) {
                    ODS(("Get devmodes failed\n"));
                    goto CleanUp;
                }

                if (dwPageIndex == 1) {
                    // Save the Devmode for the first page on a side
                    pLastDM = pCurrDM;
                } else {
                     // If the Devmode changes in a side, start a new page
                        if (DifferentDevmodes(pCurrDM, pLastDM)) {

                            dwPageIndex = 1;
                            pLastDM = pCurrDM;
                        }
                }
            } //if (bCheckDevmode)

            // Create a node for the start of a side
            if (dwPageIndex == 1)
            {
                //
                // A new side starts here. i.e. this page has to
                // be printed on a new side.
                //

                pHeadMoving->pNextSide    = pHeadNewSide;
                pHeadNewSide              = pHeadMoving;
                ulNumNodes++;
            }
            else
            {
                //
                // This is a new logical page to be printed
                // on the same side as the previous logical page.
                //
                (pHeadMoving+1)->pNextLogPage = pHeadMoving;
            }

            //
            // Note page number here can exceed max pages in the document (dwTotalNumberOfPages).
            // The max. page possible here is dwMaxLogicalPages which can be
            // greater than dwTotalNumberOfPages. e.g. in cond 2 above, dwTotalNumberOfPages = 5
            // but dwMaxLogicalPages = 6.
            // Also note that one when n-up is not RIGHT_THEN_DOWN and if the printer
            // can do its own n-up (i.e. print processor doesn't need to simulate it)
            // we have to order the pages in a specific way (Read the comment before
            // gPageOrderPlayBackForDriver for more info.

            if ( dwDrvNumberOfPagesPerSide > 1 &&
                 eNupDirection != kRightThenDown )
            {
                DWORD dwSideNumber        = (dwPageNumber-1) / dwNumberOfPagesPerSide;
                DWORD dwNewPageIndex      = pNupDirectionBasedPageOrder[dwPageIndex-1]; //dwPageIndex is 1 based
                DWORD dwNewPageNumber     = (dwSideNumber * dwNumberOfPagesPerSide) + dwNewPageIndex;
                pHeadMoving->dwPageNumber = dwNewPageNumber;

            }
            else
            {
                pHeadMoving->dwPageNumber = dwPageNumber;
            }

            pHeadMoving--;
        } //end of for.

     } //end of while


    //
    // If we are printing duplex, then do some specific page
    // order manipulation for specific types of printers.
    //
    if ( EMF_DUP_NONE         != pEMFAttr->dwDuplexMode &&
         kFaceUp_NewPageUnder == pEMFAttr->ePaperFace )
    {
        pHeadNewSide = BReverseSidesOnSheet(pEMFAttr, pHeadNewSide);
    }

    *ppPageListHead = pHeadNewSide;

    bReturn = TRUE;

CleanUp:

    if ( FALSE == bReturn )
    {
        if ( *ppMemoryHead )
        {
            FreeSplMem (*ppMemoryHead);
        }

        *ppMemoryHead   = NULL;
        *ppPageListHead = NULL;
    }

    return bReturn;
}

/*++
Function Description:
    This function switches the order in which the 2 sides are printed on the sheet.

Parameters:
     pEMFAttr               -- Job Attributes
     pPageList              -- pointer to the page order

Return Values:  The new pointer to page order.
                NULL if some error is encountered
--*/
_Success_(return != NULL)
PPAGE_NUMBER
BReverseSidesOnSheet(
        _In_         PEMF_ATTRIBUTE_INFO    pEMFAttr,
        _Inout_opt_  PPAGE_NUMBER           pPageList)
{
    PPAGE_NUMBER  pHeadMoving = pPageList;

    //
    // First make sure the job is duplex. If not,
    // this function was called in error.
    //
    if ( NULL          == pEMFAttr               ||
         NULL          == pPageList              ||
         EMF_DUP_NONE  == pEMFAttr->dwDuplexMode )
    {
        return NULL;
    }

    // INPUT
    // Assuming
    //      1) 5 pages with 2-up
    //
    //
    // pPageList        ------------------pNextSide (equiv to NextSheet)
    //     |            |
    //     |            |
    //     \/           \/
    //     7 -->  5  -->  3  -->  1
    //     |      |       |       |  <-- pNextLogicalPage
    //     |      |       |       |
    //     \/     \/      \/      \/
    //     8      6       4       2
    //
    // Pages 7,8,6 are marked but they should be treated as empty pages.
    // since maximum pages are 5.
    //

    // OUTPUT
    //
    // pPageList        ------------------pNextSide (equiv to NextSheet)
    //     |            |
    //     |            |    |------------pNextSide (other side of same sheet)
    //     \/           \/  \/
    //     5 -->  7  -->  1  -->  3
    //     |      |       |       |  <-- pNextLogicalPage
    //     |      |       |       |
    //     \/     \/      \/      \/
    //     6      8       2       4
    //
    // Pages 7,8,6 are marked but they should be treated as empty pages.
    // since maximum pages are 5.
    //

    //
    // Reset the pPageList to the new Head
    //
    if ( pPageList && pPageList->pNextSide )
    {
        pPageList = pPageList->pNextSide;
    }


    // Switch the two sides
    //
    while ( pHeadMoving && pHeadMoving->pNextSide )
    {
        PPAGE_NUMBER pFirstSide  = pHeadMoving;
        PPAGE_NUMBER pSecondSide = pFirstSide->pNextSide;

        //
        // Make pHeadMoving point to the first side of next sheet.
        // Then pHeadMoving will be equivalent to saying pNextSheet.
        //
        pHeadMoving = pSecondSide->pNextSide;

        //
        // Now switch first side and second side.
        //
        if ( NULL == pHeadMoving ) // If there is no next sheet (i.e. this is last sheet).
        {
            pFirstSide->pNextSide = NULL;
        }
        else
        {
            //
            // Point current first side to 2nd side of the next sheet
            //
            pFirstSide->pNextSide  = pHeadMoving->pNextSide;
        }
        pSecondSide->pNextSide = pFirstSide;

    }


    return pPageList;

}

BOOL
CopyDevmode(
    _In_            PPRINTPROCESSORDATA pData,
    _Outptr_result_maybenull_ LPDEVMODEW         *pDevmode)

/*++
Function Description: Copies the devmode in pData or the default devmode into pDevmode.

Parameters:   pData           - Data structure for the print job
              pDevmode        - pointer to devmode

Return Value:  TRUE  if successful
               FALSE otherwise
--*/

{
    HANDLE           hDrvPrinter = NULL;
    BOOL             bReturn = FALSE;
    LONG             lNeeded;

    *pDevmode = NULL;

    if (pData->pDevmode) {

        lNeeded = pData->pDevmode->dmSize +  pData->pDevmode->dmDriverExtra;

        if ((*pDevmode = (LPDEVMODEW) AllocSplMem(lNeeded)) != NULL) {
            memcpy(*pDevmode, pData->pDevmode, lNeeded);
        } else {
            goto CleanUp;
        }
    } else {
        // Get the default devmode
        // Get a client side printer handle to pass to the driver
        if (!OpenPrinter(pData->pPrinterName, &hDrvPrinter, NULL)) {
            ODS(("Open printer failed\nPrinter %ws\n", pData->pPrinterName));
            goto CleanUp;
        }

        lNeeded = DocumentProperties(NULL, hDrvPrinter, pData->pPrinterName, NULL, NULL, 0);

        if ((lNeeded <= 0) ||
            ((*pDevmode = (LPDEVMODEW) AllocSplMem(lNeeded)) == NULL) ||
            (DocumentProperties(NULL, hDrvPrinter, pData->pPrinterName, *pDevmode, NULL, DM_OUT_BUFFER) < 0)) {

             if (*pDevmode) {
                FreeSplMem(*pDevmode);
                *pDevmode = NULL;
             }

             ODS(("DocumentProperties failed\nPrinter %ws\n",pData->pPrinterName));
             goto CleanUp;
        }
    }

    bReturn = TRUE;

CleanUp:

    if (hDrvPrinter) {
        ClosePrinter(hDrvPrinter);
    }

    return bReturn;
}

BOOL
PrintEMFJob(
    _In_ PPRINTPROCESSORDATA pData,
    _In_ LPWSTR              pDocumentName)

/*++
Function Description: Prints out a job with EMF data type.

Parameters:   pData           - Data structure for this job
              pDocumentName   - Name of this document

Return Value:  TRUE  if successful
               FALSE if failed - GetLastError() will return reason.
--*/

{
    HANDLE             hSpoolHandle = NULL;
    DWORD              LastError = ERROR_SUCCESS;
    HDC                hPrinterDC = NULL;

    BOOL               bReverseOrderPrinting, bReturn = FALSE, bSetWorldXform = TRUE;
    BOOL               bDuplex, bBookletPrint;
    BOOL               bUpdateAttributes = FALSE;
    SHORT              dmCollate,dmCopies;

    DWORD              dwJobNumberOfPagesPerSide,dwDrvNumberOfCopies;
    DWORD              dwDrvNumberOfPagesPerSide;
    DWORD              dwJobNumberOfCopies;
    DWORD              dwTotalNumberOfPages, dwNupBorderFlags, dwDuplexMode, dwNumberOfPagesPerSide;
    DWORD              dwJobOrder, dwDrvOrder, dwOptimization;

    XFORM              OldXForm = {0};
    PPAGE_NUMBER       pMemoryHead = NULL, pPageListHead = NULL;
    ATTRIBUTE_INFO_4   AttributeInfo;
    LPDEVMODEW         pDevmode = NULL, pFirstDM = NULL, pCopyDM;
    EMF_ATTRIBUTE_INFO EMFAttr;

    DWORD dwData = 0;
    DWORD cbSize = 0;
    HANDLE   hPrinter = NULL;

    memset(&EMFAttr, 0, sizeof(EMF_ATTRIBUTE_INFO));
    EMFAttr.dwTotalNumberOfPages = 0xFFFFFFFF; //since this attribute is not always
                                               //valid, 0xfffffff means uninitialized.
    EMFAttr.dwSignature  = EMF_ATTRIBUTE_INFO_SIGNATURE;


    // Copy the devmode into pDevMode
    if (!CopyDevmode(pData, &pDevmode)) {

        ODS(("CopyDevmode failed\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }

    if ( ! BIsDevmodeOfLeastAcceptableSize (pDevmode) )
    {
        ODS(("Devmode not big enough. Failing job.\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }

    // Update resolution before CreateDC for monochrome optimization
    if (!PrintProcGetJobAttributesEx(pData->pPrinterName, pDevmode, &AttributeInfo))
    {
        ODS(("PrintProcGetJobAttributesEx failed\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }
    else
    {
        if (AttributeInfo.dwColorOptimization)
        {
            if (pDevmode->dmPrintQuality != AttributeInfo.dmPrintQuality ||
                pDevmode->dmYResolution != AttributeInfo.dmYResolution)
            {
                pDevmode->dmPrintQuality =  AttributeInfo.dmPrintQuality;
                pDevmode->dmYResolution =  AttributeInfo.dmYResolution;
                bUpdateAttributes = TRUE;
            }
        }

        if (pDevmode->dmFields & DM_COLLATE)
            dmCollate = pDevmode->dmCollate;
        else
            dmCollate = DMCOLLATE_FALSE;

        if (pDevmode->dmFields & DM_COPIES)
            dmCopies = pDevmode->dmCopies;
        else
            dmCopies = 0;
    }

    // Get spool file handle and printer device context from GDI
    __try {

        hSpoolHandle = GdiGetSpoolFileHandle(pData->pPrinterName,
                                             pDevmode,
                                             pDocumentName);
        if (hSpoolHandle) {
            hPrinterDC = GdiGetDC(hSpoolHandle);
        }

#pragma prefast(suppress:__WARNING_EXCEPTIONEXECUTEHANDLER, "Here we handle all exceptions.")
    } __except (EXCEPTION_EXECUTE_HANDLER) {

        ODS(("PrintEMFJob gave an exceptionPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }

    if (!hPrinterDC || !hSpoolHandle) {
        goto CleanUp;
    }


    BUpdateAttributes(hPrinterDC, &EMFAttr);


    //
    // Use the first devmode in the spool file to update the copy count
    // and the collate setting
    //
    if (GdiGetDevmodeForPagePvt(hSpoolHandle, 1, &pFirstDM, NULL) &&
        pFirstDM) {

        if (pFirstDM->dmFields & DM_COPIES) {
            pDevmode->dmFields |= DM_COPIES;
            pDevmode->dmCopies = pFirstDM->dmCopies;
        }

        if ( (pFirstDM->dmFields & DM_COLLATE) &&
             IS_DMSIZE_VALID ( pDevmode, dmCollate) )
        {
            pDevmode->dmFields |= DM_COLLATE;
            pDevmode->dmCollate = pFirstDM->dmCollate;
        }
    }

    // The number of copies of the print job is the product of the number of copies set
    // from the driver UI (present in the devmode) and the number of copies in pData struct
    dwJobNumberOfCopies = (pDevmode->dmFields & DM_COPIES) ? pData->Copies*pDevmode->dmCopies
                                                           : pData->Copies;
    pDevmode->dmCopies = (short) dwJobNumberOfCopies;
    pDevmode->dmFields |=  DM_COPIES;

    // If collate is true this limits the ability of the driver to do multiple copies
    // and causes the driver (PS) supported n-up to print blank page borders for reverse printing.
    // Therefore we disable collate for 1 page multiple copy jobs or no copies but n-up since
    // collate has no meaning in those cases.
    //
    if ((pDevmode->dmFields & DM_COLLATE) && pDevmode->dmCollate == DMCOLLATE_TRUE)
    {
        if (dwJobNumberOfCopies > 1)
        {
            // Get the number of pages in the job. This call waits till the
            // last page is spooled.
            __try {

                dwTotalNumberOfPages = GdiGetPageCount(hSpoolHandle);

#pragma prefast(suppress:__WARNING_EXCEPTIONEXECUTEHANDLER, "Here we handle all exceptions.")
            } __except (EXCEPTION_EXECUTE_HANDLER) {

                ODS(("PrintEMFJob gave an exceptionPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
                goto SkipCollateDisable;
            }

            if (dwTotalNumberOfPages > AttributeInfo.dwDrvNumberOfPagesPerSide)
                goto SkipCollateDisable;
        }
        // if copies == 1 and driver n-up we will disable collate
        //
        else if (AttributeInfo.dwDrvNumberOfPagesPerSide <= 1 && dmCollate == DMCOLLATE_TRUE)
            goto SkipCollateDisable;

        if( OpenPrinter(pData->pPrinterName,
                        &hPrinter,
                        NULL))
        {
               if( GetPrinterData( hPrinter,
                                const_cast<LPTSTR>(gszDriverKeepCollate),
                                NULL,
                                reinterpret_cast<LPBYTE>( &dwData ),
                                sizeof(dwData),
                                &cbSize ) == ERROR_SUCCESS )
               {

                    if( dwData == 1)
                    {
                        goto SkipCollateDisable;
                    }
               }
        }

        pDevmode->dmCollate = DMCOLLATE_FALSE;

        if (pFirstDM &&
            IS_DMSIZE_VALID ( pFirstDM, dmCollate) )
        {
            pFirstDM->dmCollate = DMCOLLATE_FALSE;
        }
    }

SkipCollateDisable:
    // Update the job attributes but only if something has changed. This is an expensive
    // call so we only make a second call to GetJobAttributes if something has changed.
    //
    if (bUpdateAttributes || pDevmode->dmCopies != dmCopies ||
            ((pDevmode->dmFields & DM_COLLATE) && (pDevmode->dmCollate != dmCollate)))
    {
        if (!PrintProcGetJobAttributesEx(pData->pPrinterName, pDevmode, &AttributeInfo))
        {
            ODS(("PrintProcGetJobAttributesEx failed\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
            goto CleanUp;
        }
    }

    // Initialize bReverseOrderPrinting, dwJobNumberOfPagesPerSide,
    // dwDrvNumberOfPagesPerSide, dwNupBorderFlags, dwJobNumberOfCopies,
    // dwDrvNumberOfCopies and bCollate

    dwJobNumberOfPagesPerSide = AttributeInfo.dwJobNumberOfPagesPerSide;
    dwDrvNumberOfPagesPerSide = AttributeInfo.dwDrvNumberOfPagesPerSide;
    dwNupBorderFlags          = AttributeInfo.dwNupBorderFlags;
    dwJobNumberOfCopies       = AttributeInfo.dwJobNumberOfCopies;
    dwDrvNumberOfCopies       = AttributeInfo.dwDrvNumberOfCopies;

    dwJobOrder                = AttributeInfo.dwJobPageOrderFlags & ( NORMAL_PRINT | REVERSE_PRINT);
    dwDrvOrder                = AttributeInfo.dwDrvPageOrderFlags & ( NORMAL_PRINT | REVERSE_PRINT);
    bReverseOrderPrinting     = (dwJobOrder != dwDrvOrder);

    dwJobOrder                = AttributeInfo.dwJobPageOrderFlags & BOOKLET_PRINT;
    dwDrvOrder                = AttributeInfo.dwDrvPageOrderFlags & BOOKLET_PRINT;
    bBookletPrint             = (dwJobOrder != dwDrvOrder);

    EMFAttr.bCollate          = (pDevmode->dmFields & DM_COLLATE) &&
                                  (pDevmode->dmCollate == DMCOLLATE_TRUE);

    bDuplex                   = (pDevmode->dmFields & DM_DUPLEX) &&
                                  (pDevmode->dmDuplex != DMDUP_SIMPLEX);

    if (!dwJobNumberOfCopies) {
        //
        // Some applications can set the copy count to 0.
        // In this case we exit.
        //
        bReturn = TRUE;
        goto CleanUp;
    }

    if (bDuplex) {
        dwDuplexMode = (pDevmode->dmDuplex == DMDUP_HORIZONTAL) ? EMF_DUP_HORZ
                                                                : EMF_DUP_VERT;
    } else {
        dwDuplexMode = EMF_DUP_NONE;
    }

    if (bBookletPrint) {
        if (!bDuplex) {
            // Not supported w/o duplex printing. Use default settings.
            bBookletPrint = FALSE;
            dwDrvNumberOfPagesPerSide = 1;
            dwJobNumberOfPagesPerSide = 1;
        } else {
            // Fixed settings for pages per side.
            dwDrvNumberOfPagesPerSide = 1;
            dwJobNumberOfPagesPerSide = 2;
        }
    }

    // Number of pages per side that the print processor has to play
    dwNumberOfPagesPerSide = (dwDrvNumberOfPagesPerSide == 1)
                                               ? dwJobNumberOfPagesPerSide
                                               : 1;


    if (dwNumberOfPagesPerSide == 1) {
        // if the print processor is not doing nup, don't draw borders
        dwNupBorderFlags = NO_BORDER_PRINT;
    }

    //
    // Color optimization may cause wrong output with duplex
    //
    dwOptimization = (AttributeInfo.dwColorOptimization == COLOR_OPTIMIZATION &&
                                           !bDuplex && dwJobNumberOfPagesPerSide == 1)
                                           ? EMF_PP_COLOR_OPTIMIZATION
                                           : 0;

    // Check for Valid Option for n-up printing
    if (!ValidNumberForNUp(dwNumberOfPagesPerSide)) {
        ODS(("Invalid N-up option\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }

    //
    // From local variables, put the values in EMFAttr.
    // Note: The value for dwTotalNumberOfPages has been initialized
    // to 0xFFFFFFFF which means undetermined. Later on, this value
    // may be overwritten.
    //
    EMFAttr.eNupDirection             = ExtractNupDirectionFromAttributeInfo(&AttributeInfo, bBookletPrint);
    EMFAttr.eBookletMode              = ExtractBookletModeFromAttributeInfo(&AttributeInfo, bBookletPrint);
    EMFAttr.bBookletPrint             = bBookletPrint;
    EMFAttr.bReverseOrderPrinting     = bReverseOrderPrinting;
    EMFAttr.dwDrvNumberOfPagesPerSide = dwDrvNumberOfPagesPerSide;
    EMFAttr.dwNupBorderFlags          = dwNupBorderFlags;
    EMFAttr.dwJobNumberOfCopies       = dwJobNumberOfCopies;
    EMFAttr.dwDuplexMode              = dwDuplexMode;
    EMFAttr.dwNumberOfPagesPerSide    = dwNumberOfPagesPerSide;
    EMFAttr.dwDrvNumberOfCopies       = dwDrvNumberOfCopies;
    ExtractScalingFactorFromAttributeInfo(&AttributeInfo, &EMFAttr);
    ExtractDuplexModeInformationFromAttributeInfo(&AttributeInfo, &EMFAttr);


    // pCopyDM will be used for changing the copy count
    pCopyDM = pFirstDM ? pFirstDM : pDevmode;
    pCopyDM->dmPrintQuality = pDevmode->dmPrintQuality;
    pCopyDM->dmYResolution = pDevmode->dmYResolution;

    //
    // For reverse, booklet printing, we need to know the total number
    // of pages. So we have to wait till all the pages are spooled.
    //
    if (bReverseOrderPrinting || bBookletPrint) {

       // Get the number of pages in the job. This call waits till the
       // last page is spooled.
       __try {

           dwTotalNumberOfPages= GdiGetPageCount(hSpoolHandle);

#pragma prefast(suppress:__WARNING_EXCEPTIONEXECUTEHANDLER, "Here we handle all exceptions.")
       } __except (EXCEPTION_EXECUTE_HANDLER) {

           ODS(("PrintEMFJob gave an exceptionPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
           goto CleanUp;
       }

        EMFAttr.dwTotalNumberOfPages      = dwTotalNumberOfPages;

        //
        // GetStartPageList for reverse/booklet printing
        // Check for a change of devmode between pages only if Nup and PCL driver
        // If both booklet and reverse printing are present,
        // we choose booklet.
        //
        if ( bBookletPrint )
        {
            if (!GetStartPageListBooklet(
                                         &EMFAttr,
                                         &pMemoryHead,   //Where memory for the list starts.
                                         &pPageListHead  //where the actual head of list starts.
                                         )) {
                 goto CleanUp;
            }
        }
        else if ( bReverseOrderPrinting )
        {
            //
            // Get start page list. For certain "normal" printing scenarios,
            // we dont really need the list. So pHead will be returned as NULL.
            // (We could still poplulate pHead, but that will mean needlessly
            // going through memory allocation code etc...).
            // But for others ( like reverse printing/booklet printing)
            // it is good to get the page order here and make things simpler
            // in the future.
            //
            if (!GetStartPageListReverseOrder(hSpoolHandle,
                                  &EMFAttr,
                                  pCopyDM,
                                  &pMemoryHead, //Where memory for the list starts.
                                  &pPageListHead //where the actual head of list starts.
                                  )) {
                 goto CleanUp;
            }
       }
    }

    // Save the old transformation on hPrinterDC
    if (!SetGraphicsMode(hPrinterDC,GM_ADVANCED) ||
        !GetWorldTransform(hPrinterDC,&OldXForm)) {

         bSetWorldXform = FALSE;
         ODS(("Transformation matrix can't be set\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
         goto CleanUp;
    }


    bReturn = BPrintEMFJobNow (
                            hSpoolHandle,
                            hPrinterDC,
                            &EMFAttr,
                            dwOptimization,
                            pCopyDM,
                            pPageListHead,
                            pData );

    //
    // Preserve the last error
    //
    LastError = bReturn ? ERROR_SUCCESS : GetLastError();


CleanUp:

    if(hPrinter)
    {
        ClosePrinter(hPrinter);
    }

    if (bSetWorldXform && hPrinterDC) {
       SetWorldTransform(hPrinterDC, &OldXForm);
    }

    if (pMemoryHead)
    {
        FreeSplMem(pMemoryHead);
    }

    if (pDevmode)  {
       FreeSplMem(pDevmode);
    }

    __try {
        if (hSpoolHandle) {
           GdiDeleteSpoolFileHandle(hSpoolHandle);
        }

#pragma prefast(suppress:__WARNING_EXCEPTIONEXECUTEHANDLER, "Here we handle all exceptions.")
    } __except (EXCEPTION_EXECUTE_HANDLER) {

        ODS(("GdiDeleteSpoolFileHandle failed\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
    }

    SetLastError(LastError);

    return bReturn;
}

/*++
Function Name
    BPrintEMFJobNow

Function Description.

Parameters:
            hSpoolHandle       -- the handle to the spool file
            hPrinterDC         -- the devmode related to this page number is requested.
            pEMFAttr           -- the devmode for the dwPageNumber is placed here.
                               -- devmode for dwPageNumber-1 is placed here. Can be NULL. (if n
            dwOptimization
            pDevmode
            pPageList
            pData

Return Values:  TRUE if the emf is printed
                FALSE otherwise
--*/

BOOL
BPrintEMFJobNow (
        _In_      HANDLE                hSpoolHandle,
        _In_      HDC                   hPrinterDC,
        _In_      PEMF_ATTRIBUTE_INFO   pEMFAttr,
        _In_      DWORD                 dwOptimization,
        _In_      LPDEVMODEW            pDevmode,
        _In_opt_  PPAGE_NUMBER          pPageList,
        _In_      PPRINTPROCESSORDATA   pData )
{
    DOCINFOW    DocInfo;
    DWORD       LastError;
    BOOL        bRetVal              = FALSE;
    BOOL        bStartDoc            = FALSE;
    DWORD       dwRemainingCopies    = 0;
    DWORD       dwJobNumberOfCopies  = pEMFAttr->dwJobNumberOfCopies;
    DWORD       dwDrvNumberOfCopies  = pEMFAttr->dwDrvNumberOfCopies;
    BOOL        bStartNewDocOnCopies = pEMFAttr->dwDivideCopiesIntoMultipleDocuments;

    static INT  iThrowDriverException = -1;

    //
    // Refer to KB article KB934885 for more details on the usage of the "ThrowDriverException" registry value.
    //
    if (iThrowDriverException == -1)
    {
        HKEY hKey;

        if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,
                                          TEXT("System\\CurrentControlSet\\Control\\Print"),
                                          0,
                                          KEY_READ,
                                          &hKey))
        {
            DWORD dwData, cbData;

            cbData = sizeof(dwData);
            if (ERROR_SUCCESS == RegQueryValueEx(hKey,
                                                 TEXT("ThrowDriverException"),
                                                 0,
                                                 NULL,
                                                 (LPBYTE)&dwData,
                                                 &cbData)                         &&
                dwData == 1)
            {
                iThrowDriverException = 1;
            }
            else
            {
                iThrowDriverException = 0;
            }

            RegCloseKey(hKey);
        }
    }

    __try {

        DocInfo.cbSize = sizeof(DOCINFOW);
        DocInfo.lpszDocName  = pData->pDocument;
        DocInfo.lpszOutput   = pData->pOutputFile;
        DocInfo.lpszDatatype = NULL;

        if (!GdiStartDocEMF(hSpoolHandle, &DocInfo)) goto CleanUp;
        bStartDoc = TRUE;

/*++ Only For OSes later than Windows Server 2003. This is an example of how
     IHVs may use the multiple start doc feature

        //
        // If for some reason, we need to break a job into multiple documents,
        // then we have to do something special. e.g. In manual duplex
        // the user may take some time to walk over to the printer
        // and invert the pages, this may cause a TCP timeout and the
        // the job will abandon. To prevent a TCP timeout, we have to
        // first set JOB_CONTROL_RETAIN flag for the job and then
        // do JOB_CONTROL_RELEASE once everything is printed.
        //
        if ( <Multiple StartDoc needs to be done>)

        {
            if (! OpenPrinterW(pData->pPrinterName,&hPrinter,&Defaults) )
            {
                goto CleanUp;
            }
            SetJobW(hPrinter, pData->JobId, 0, NULL, JOB_CONTROL_RETAIN);
        }
--*/

        if (pEMFAttr->bCollate) {

            dwRemainingCopies = dwJobNumberOfCopies & 0x0000FFFF ;

            while (dwRemainingCopies) {
                //
                // We do a new startDoc only if this is not the first time we are going thru this loop,
                //
                if ( bStartNewDocOnCopies && ( dwRemainingCopies != (dwJobNumberOfCopies & 0x0000FFFF ) ) )
                {
                    //
                    // End the previous job and start a new one.
                    //
                    if (!GdiEndDocEMF(hSpoolHandle) || !GdiStartDocEMF(hSpoolHandle, &DocInfo) )
                    {
                        bStartDoc = FALSE;
                        goto CleanUp;
                    }
                }

               if (dwRemainingCopies <= dwDrvNumberOfCopies) {
                  SetDrvCopies(hPrinterDC, pDevmode, dwRemainingCopies);
                  dwRemainingCopies = 0;
               } else {
                  SetDrvCopies(hPrinterDC, pDevmode, dwDrvNumberOfCopies);
                  dwRemainingCopies -= dwDrvNumberOfCopies;
               }

               if (!PrintEMFSingleCopy(hSpoolHandle,
                                       hPrinterDC,
                                       pEMFAttr,
                                       pPageList,
                                       dwOptimization,
                                       pDevmode,
                                       pData)) {
                   goto CleanUp;
               }

            } //while

        } else {

           if (!PrintEMFSingleCopy(hSpoolHandle,
                                   hPrinterDC,
                                   pEMFAttr,
                                   pPageList,
                                   dwOptimization,
                                   pDevmode,
                                   pData)) {

               goto CleanUp;
           }
        }

    } __except (iThrowDriverException == 1 ? EXCEPTION_CONTINUE_SEARCH : EXCEPTION_EXECUTE_HANDLER) {

        ODS(("PrintEMFSingleCopy gave an exception\nPrinter %ws\nDocument %ws\nJobID %u\n", pData->pDevmode->dmDeviceName, pData->pDocument, pData->JobId));
        goto CleanUp;
    }

    bRetVal = TRUE;

CleanUp:

    //
    // Preserve the last error
    //
    LastError = bRetVal ? ERROR_SUCCESS : GetLastError();

    if ( bStartDoc ) {

/*++ Only for OSes after Windows 2003. IHV's may uncomment it if they are doing multiple start docs.
        if ( hPrinter && bMultipleStartDoc ) //hPrinter could be NULL if OpenPrinter failed.
        {
            SetJobW(hPrinter, pData->JobId, 0, NULL, JOB_CONTROL_RELEASE);
        }
--*/
        GdiEndDocEMF(hSpoolHandle);
    }

    SetLastError(LastError);

    return bRetVal;
}


/*++
Function Name
    GdiGetDevmodeForPagePvt

Function Description.
    In some cases, GDI's GdiGetDevmodeForPage returns a devmode
    that is based on an old format of devmode. e.g. Win3.1 format. The size of such a devmode
    can be smaller than the latest Devmode. This can lead to unpredictable issues.
    Also, sometimes the devmode returned is even smaller than Win3.1 format (due to possible
    corruption).
    This function is a wrapper around GDI's GdiGetDevmodeForPage and partially takes care of this
    situation by doing an extra checking for devmode.

Parameters:
            hSpoolHandle           -- the handle to the spool file
            dwPageNumber           -- the devmode related to this page number is requested.
            ppCurrDM               -- the devmode for the dwPageNumber is placed here.
            ppLastDM               -- devmode for dwPageNumber-1 is placed here. Can be NULL. (if n
ot NULL)

Return Values:  TRUE if a valid devmode was obtained from GDI
                FALSE otherwise
--*/

_Success_(return != FALSE)
BOOL GdiGetDevmodeForPagePvt(
    _In_            HANDLE              hSpoolHandle,
    _In_            DWORD               dwPageNumber,
    _Outptr_     PDEVMODEW           *ppCurrDM,
    _Outptr_opt_result_maybenull_ PDEVMODEW           *ppLastDM
  )
{
    if ( NULL == ppCurrDM )
    {
        return FALSE;
    }

    *ppCurrDM = NULL;

    if ( ppLastDM )
    {
        *ppLastDM = NULL;
    }

    if (!GdiGetDevmodeForPage(hSpoolHandle,
                              dwPageNumber,
                              ppCurrDM,
                              ppLastDM) )
    {
        ODS(("GdiGetDevmodeForPage failed\n"));
        return FALSE;
    }
    //
    // If GdiGetDevmodeForPage has succeeded, then *ppCurrDM should have valid values
    //
    // GDI guarantees that the size of the devmode is atleast dmSize+dmDriverExtra.
    // So we dont need to check for that. But we still need to check some other dependencies
    //
    //

    if ( NULL  == *ppCurrDM ||
         FALSE == BIsDevmodeOfLeastAcceptableSize (*ppCurrDM)
       )
    {
        return FALSE;
    }

    //
    // It is possible for GdiGetDevmodeForPage to return TRUE (i.e. success)
    // but still not fill in the *ppLastDM. So NULL *ppLastDM is not an error
    //

    if ( ppLastDM && *ppLastDM &&
         FALSE == BIsDevmodeOfLeastAcceptableSize (*ppLastDM)
       )
    {

        return FALSE;
    }

    return TRUE;
}


/*++
Function Name
    BIsDevmodeOfLeastAcceptableSize

Function Description.
Parameters:
    pdm  -- the pointer to the devmode.

Return Values:  TRUE if devmode is of least acceptable size.
                FALSE otherwise
--*/

BOOL BIsDevmodeOfLeastAcceptableSize(
    _In_ PDEVMODE pdm)
{
    if ( NULL == pdm )
    {
        return FALSE;
    }

    if ( IS_DMSIZE_VALID((pdm),dmYResolution) )
    {
        return TRUE;
    }
    return FALSE;
}

/*++
Function Name
    BPlayEmptyPages

Function Description.
    Plays empty logical pages. e.g. n-up = 4 but
    app plays only one page. So we'll play 3 empty pages so that printer thinks
    4 pages have been recieved.

Parameters:

Return Values:  TRUE if successful
                FALSE otherwise
--*/

BOOL BPlayEmptyPages(
        _In_  HANDLE                 hSpoolHandle,
        _In_  DWORD                  dwNumPages,
        _In_  DWORD                  dwOptimization)
{
    ULONG ulNPCtr = 0;

    for ( ulNPCtr = 0; ulNPCtr < dwNumPages; ulNPCtr++)
    {
        //
        // To play back an empty page, we need to call
        // GdiStartPageEMF and GdiEndPageEMF.
        //
        if (!GdiStartPageEMF(hSpoolHandle) || !GdiEndPageEMF(hSpoolHandle, dwOptimization))
        {
            ODS(("Start Page / EndPage failed\n"));
        }
    }

    return TRUE;
}


/*++
Function Name
    BUpdateAttributes

Function Description.
    Sometimes when orientation of pages change within the job, the x,y printable area changes.
    So we need to get the new values.
    Note: Currently this is used only to get the x,y resolution and x,y printable area.
    It may need to be expaned in the future.

Parameters:

    pEMFAttr : The Job Attributes.

Return Values:  TRUE if successful
                FALSE otherwise

Assumption:
    1. pEMFAttr will always be valid.
    2. GetDeviceCaps will not return random values. This is a good assumption because MSDN
       does not define any error return values.
--*/

BOOL
BUpdateAttributes(
    _In_     HDC                   hPrinterDC,
    _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr)
{
    pEMFAttr->lXResolution = GetDeviceCaps(hPrinterDC, LOGPIXELSX)     ; //dpi in x direction.
    pEMFAttr->lYResolution = GetDeviceCaps(hPrinterDC, LOGPIXELSY)     ; //dpi in y direction.
    pEMFAttr->lXPrintArea  = GetDeviceCaps(hPrinterDC, DESKTOPHORZRES) ; //numpixels in printable area in x direction.
    pEMFAttr->lYPrintArea  = GetDeviceCaps(hPrinterDC, DESKTOPVERTRES) ; //numpixels in printable area in y direction

    pEMFAttr->lXPhyPage    = GetDeviceCaps(hPrinterDC, PHYSICALWIDTH)  ;
    pEMFAttr->lYPhyPage    = GetDeviceCaps(hPrinterDC, PHYSICALHEIGHT) ;

    return TRUE;
}

ENupDirection
ExtractNupDirectionFromAttributeInfo(
        _In_  PATTRIBUTE_INFO_4     pAttributeInfo,
        _In_  BOOL                  bBookletPrint
        )
{
    ENupDirection eNupDirection = kRightThenDown; // Default

    //
    // If doing booklet, ignore user specified n-up direction because
    // some directions invert the booklet edge.
    // e.g. If nup direction is kLeftThenDown(or kDownThenLeft)
    // and booklet edge is Left, the output comes as if booklet edge is right.
    //
    if ( !bBookletPrint )
    {
        switch (pAttributeInfo->dwNupDirection)
        {
            case DOWN_THEN_RIGHT:
                eNupDirection = kDownThenRight;
                break;

            case LEFT_THEN_DOWN:
                eNupDirection = kLeftThenDown;
                break;

            case DOWN_THEN_LEFT:
                eNupDirection = kDownThenLeft;
                break;

            case RIGHT_THEN_DOWN:
            case 0:
                break; // kRightThenDown already set

            default:
                ODS(("Unrecognized N-up Direction. Using default"));
                break;
        }
    }

    return eNupDirection;
}


EBookletMode
ExtractBookletModeFromAttributeInfo(
        _In_  PATTRIBUTE_INFO_4     pAttributeInfo,
        _In_  BOOL                  bBookletPrint
        )
{
    EBookletMode eBookletMode = kNoBooklet;

    if ( bBookletPrint )
    {
        switch ( pAttributeInfo->dwBookletFlags )
        {
            case BOOKLET_EDGE_LEFT:
                eBookletMode = kBookletLeftEdge;
                break;

            case BOOKLET_EDGE_RIGHT:
                eBookletMode = kBookletRightEdge;
                break;

            default:
                ODS(("Unrecognized Booklet Option. Using default i.e Booklet Left Edge."));
                eBookletMode = kBookletLeftEdge;
                break;
        } // end of switch
    }

    return eBookletMode;
}

DWORD
EnsureProperValuesForScalingPercent(
        _In_ DWORD dwScalingPercent
        )
{
#define LEAST_SCALING_PERCENT_ALLOWED 1
#define MAX_SCALING_PERCENT_ALLOWED   1000

    if ( 0 == dwScalingPercent )
    {
        dwScalingPercent = 100;
    }

    if ( dwScalingPercent > MAX_SCALING_PERCENT_ALLOWED )
    {
        dwScalingPercent = MAX_SCALING_PERCENT_ALLOWED;
    }

    if ( dwScalingPercent < LEAST_SCALING_PERCENT_ALLOWED )
    {
        dwScalingPercent = LEAST_SCALING_PERCENT_ALLOWED;
    }

    return dwScalingPercent;
}

VOID
ExtractScalingFactorFromAttributeInfo(
        _In_     const PATTRIBUTE_INFO_4 pAttributeInfo,
        _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr
        )
{
    DWORD dwScalingPercentX = pAttributeInfo->dwScalingPercentX;
    DWORD dwScalingPercentY = pAttributeInfo->dwScalingPercentY;

    dwScalingPercentX = EnsureProperValuesForScalingPercent(dwScalingPercentX);
    dwScalingPercentY = EnsureProperValuesForScalingPercent(dwScalingPercentY);

    pEMFAttr->fScalingFactorX = (FLOAT)dwScalingPercentX/100;
    pEMFAttr->fScalingFactorY = (FLOAT)dwScalingPercentY/100;
}

/*++
Function Name:
    ExtractDuplexModeInformationFromAttributeInfo

Function Description:
    This function looks at the pAttributeInfo->dwDuplexFlags and accordingly
    fills in some values in the pEMFAttr structure.

Parameters:
    pAttributeInfo : ATTRIBUTE_INFO_4 that has been previously filled by call to RetrieveJobAttributes.
    pEMFAttr       : Print Processors local settings and state maintenance structure.


Return Values:  <Nothing>
--*/

VOID
ExtractDuplexModeInformationFromAttributeInfo(
        _In_     PATTRIBUTE_INFO_4     pAttributeInfo,
        _Inout_  PEMF_ATTRIBUTE_INFO   pEMFAttr
        )
{
    pEMFAttr->ePaperFace = kFaceDown_NewPageOver; //Default

    if ( pAttributeInfo->dwDuplexFlags & REVERSE_PAGES_FOR_REVERSE_DUPLEX )
    {
        pEMFAttr->ePaperFace = kFaceUp_NewPageUnder;
    }

    if ( pAttributeInfo->dwDuplexFlags & DONT_SEND_EXTRA_PAGES_FOR_DUPLEX )
    {
        pEMFAttr->dwDuplexModeFlags |=  DONT_SEND_EXTRA_PAGES_FOR_DUPLEX ;
    }
}


/*++
Function Name:
    PrintOneSheetPreDeterminedForDriverEMF

Function Description:

Parameters: hSpoolHandle              -- handle the spool file handle
            hPrinterDC                -- handle to the printer device context
            pEMFAttr                  --
            bDuplex                   -- flag to indicate duplex printing
            dwOptimization            -- optimization flags
            pbEmptyPageEncountered    -- whether we queried for a page number bigger than the number of pages in the EMF
            pDevmode                  -- devmode with resolution settings

Return Values:  TRUE  if success
                FALSE on failure
--*/
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
    _In_      LPDEVMODE               pDevmode)

{
    HANDLE      hEMF                        = NULL;
    BOOL        BeSmart                     = FALSE;
    DWORD       dwPageType;
    DWORD       dwDrvNumberOfPagesPerSide = pEMFAttr->dwDrvNumberOfPagesPerSide;
    DWORD       dwSides                   = bDuplex ? 2 : 1;
    DWORD       dwNumberOfPagesPerSheet   = dwDrvNumberOfPagesPerSide * dwSides;
    DWORD       dwTotalNumberOfPages      = pEMFAttr->dwTotalNumberOfPages;
    BOOL        bRetVal                   = TRUE;
    BOOL        bNewDevmode               = FALSE;
    LPDEVMODEW  pCurrDM                   = NULL;
    BOOL        bAtleastOneEmptyPageFound = FALSE;
    BOOL        bReverseOrderPrinting     = pEMFAttr->bReverseOrderPrinting;
    DWORD       dwEmptyPagesToPrintBeforeNextValidPage = 0;

    for ( ; dwSides && NULL != pHead ; dwSides--, pHead = pHead->pNextSide )
    {
        PPAGE_NUMBER pHeadLogical = NULL;

        for (pHeadLogical = pHead;NULL != pHeadLogical; pHeadLogical = pHeadLogical->pNextLogPage)
        {
            DWORD dwPageNumber = pHeadLogical->dwPageNumber;
            //
            // if dwPageNumber is 0 or 0xFFFFFFFF, it means empty page.
            //
            dwPageNumber = pHeadLogical->dwPageNumber;

            if ( dwPageNumber == 0 )
            {
                dwPageNumber = 0xFFFFFFFF; //big number
            }

            if ((hEMF = GdiGetPageHandle(hSpoolHandle,
                                          dwPageNumber,
                                          &dwPageType)) == NULL)
            {
                if (GetLastError() == ERROR_NO_MORE_ITEMS) {
                     // End of the print job
                    dwEmptyPagesToPrintBeforeNextValidPage++;
                    bAtleastOneEmptyPageFound = TRUE;
                }
                else
                {
                    ODS(("GdiGetPageHandle failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                    bRetVal = FALSE;
                    goto CleanUp;
                }
            }


            // Suppose a job has 4 pages and we print 4 up in down then left configuration.
            // Since it is forward printing, we don't know in advance that there are only 4 pages
            // So we try to print page 7th (because it is down then left, the pages
            // are printed in 7,5,8,6 order). We would also try to print page 7 if the job
            // were a 5 page job. But difference is that in 5 page job, we can cycle
            // through all pages and print a sheet with only one page. But for a 4 page job,
            // we don't even want to print an empty sheet. So we try to take care of that situation
            // here.
            //

            if ( hEMF )
            {
                if ( dwEmptyPagesToPrintBeforeNextValidPage )
                {
                    if ( !BPlayEmptyPages(hSpoolHandle, dwEmptyPagesToPrintBeforeNextValidPage, dwOptimization) )
                    {
                        ODS(("BPlayEmptyPages returned error"));
                        bRetVal = FALSE;
                        goto CleanUp;
                    }
                    dwEmptyPagesToPrintBeforeNextValidPage = 0;
                }

                // Process new devmodes in the spool file that appear before this page
                if (!ResetDCForNewDevmode(hSpoolHandle,
                                      hPrinterDC,
                                      pEMFAttr,
                                      dwPageNumber,
                                      FALSE,        //Not within pages
                                      dwOptimization,
                                      &bNewDevmode,
                                      pDevmode,
                                      &pCurrDM)) {

                    bRetVal = FALSE;
                    goto CleanUp;
                }

                // Call StartPage for each new page
                if (!GdiStartPageEMF(hSpoolHandle))
                {
                    ODS(("StartPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                    bRetVal = FALSE;
                    goto CleanUp;
                }

                if ( hEMF)
                {
                    if (!PlayEMFPage(hSpoolHandle,
                                     hPrinterDC,
                                     hEMF,
                                     pEMFAttr,
                                     1,
                                     EMF_DEGREE_90)) {

                        ODS(("PlayEMFPage failed\nPrinter %ws\n", pDevmode->dmDeviceName));
                        bRetVal = FALSE;
                        goto CleanUp;
                    }
                }

                // Call EndPage irrespective of whether we played any pages
                if ( !GdiEndPageEMF(hSpoolHandle, dwOptimization))
                {
                   ODS(("EndPage failed\n"));
                   bRetVal = FALSE;
                   goto CleanUp;
                }
            }
        } //for pHeadLogical i.e. Within a side.

        //
        // Now we've reached the end of the page. Assuming we are doing 6-up, we
        // may have sent only 3 pages to driver. But printer is expecting 6 pages, so
        // we send remaining 3. But we need to print those extra 3 pages only if we there are
        // multiple copies to be printed OR we are doing duplex OR numerous other conditions.
        //

        if ( 0 != dwEmptyPagesToPrintBeforeNextValidPage )
        {
            BOOL bPrintEmptyPages = FALSE;
            BOOL bCollate =  IS_DMSIZE_VALID(pDevmode, dmCollate)            &&
                             (pDevmode->dmCollate != DMCOLLATE_TRUE);

            //
            // If printing forward, if all the pages are empty, then no need to print them.
            // (because in forward printing, total number of pages are not known before hand
            //  so we may be called to print pages that don't exist).
            //
            if ( !bReverseOrderPrinting ) // Forward Printing.
            {
                if ( 1 == dwSides ) //i.e. 2nd of the 2 sides or 1st side of one sided printing.
                {
                    if ( dwNumberOfPagesPerSheet == dwEmptyPagesToPrintBeforeNextValidPage )
                    {
                        // All pages on this sheet are empty, so no need to print anything.
                    }
                    else
                    {

                        bPrintEmptyPages = TRUE;
                    }
                }
            }
            else //Reverse Printing.
            {
                //
                // If the document will fit on one physical page, then this variable will prevent
                // the printer from playing extra pages just to fill in one physical page
                // The exception is when the pages fit on a single physical page, but they must
                // be collated. Then because of design, the printer will also draw borders for the
                // empty pages which are played so that the page gets ejected.
                //

                BeSmart =  bCollate &&
                           (dwTotalNumberOfPages<=dwDrvNumberOfPagesPerSide);

                if ( !BeSmart)
                {
                    bPrintEmptyPages = TRUE;
                }
            }

            if ( TRUE == bPrintEmptyPages )
            {
                BPlayEmptyPages(hSpoolHandle, dwEmptyPagesToPrintBeforeNextValidPage, dwOptimization);
                dwEmptyPagesToPrintBeforeNextValidPage = 0;
            }
        }

    } // for pHead (Side)

    if ( pbEmptyPageEncountered)
    {
        *pbEmptyPageEncountered = bAtleastOneEmptyPageFound;
    }

CleanUp:
    return bRetVal;
}



/*++
Function Name:
    BAnyReasonNotToPrintBlankPage.

Function Description:
    Determines if we there is a reason to suppress blank page generation.
    Print Processor historically generated a blank page if required (e.g.
    odd page job on duplex). But now, in some cases, that blank page
    generation can be suppressed (e.g. if gpd says so AND some other conditions
    are satisfied). So this function looks at those things and determines
    whether blank page can be suppressed.


Parameters:



Return Values:  TRUE  if an generation of empty page can be suppressed.
                FALSE otherwise
--*/

BOOL
BAnyReasonNotToPrintBlankPage(
    _In_      PEMF_ATTRIBUTE_INFO     pEMFAttr,
    _In_      DWORD                   dwNumberOfPagesInJob
    )
{
    BOOL bReasonNotPrintBlank           = FALSE;   //no reason not to i.e. we should print blank

    if ( pEMFAttr->dwDuplexModeFlags   &  DONT_SEND_EXTRA_PAGES_FOR_DUPLEX &&
         pEMFAttr->dwJobNumberOfCopies <= pEMFAttr->dwDrvNumberOfCopies
       )
    {
        //
        // For multiple sheet print jobs (e.g. 3 page jobs requiring 2 sheets),
        // we need an extra blank page for reverse, but not for forward.
        //
        // For duplex jobs that fit on a single side, we don't need to generate extra empty page.
        //
        if (  FALSE                == pEMFAttr->bReverseOrderPrinting  ||
              dwNumberOfPagesInJob <= pEMFAttr->dwNumberOfPagesPerSide
           )
        {
            bReasonNotPrintBlank = TRUE; //found a reason not to print blank
        }
    }

    return bReasonNotPrintBlank;
}

/*++
Function Name:
    BIsEveryPageOnThisSideBlank

Function Description:
    This function determines if every page pointed out out by pLogicalPage is Blank.
    By Blank, we mean the page number (as indicated by the nodes in the
    linked list pLogicalPage) is more than the total number of pages in the valid job.
    By Blank, we DONT mean that the page as spooled by the application has no content.
    This function does not analyze the contents of the page.

Parameters:
    pHeadLogical         : The linked last where each node in the list has a certain page number.
    dwTotalNumberOfPages : The total number of pages in the job. This number is the number
                           of logical pages as spooled by the application for a single copy job.

Return Values:  TRUE  if each page is empty
                FALSE otherwise
--*/

BOOL
BIsEveryPageOnThisSideBlank(
    _In_  PPAGE_NUMBER  pHeadLogical,
    _In_  DWORD         dwTotalNumberOfPages)
{
    BOOL    bAllPagesBlank      = TRUE;
    DWORD   dwLogicalPageNumber = 0;

    for ( ; pHeadLogical; pHeadLogical = pHeadLogical->pNextLogPage )
    {
        dwLogicalPageNumber = pHeadLogical->dwPageNumber;

        if ( dwLogicalPageNumber > 0 && dwLogicalPageNumber <= dwTotalNumberOfPages )
        {
            bAllPagesBlank = FALSE;
            break;
        }
    }

    return bAllPagesBlank;
}

