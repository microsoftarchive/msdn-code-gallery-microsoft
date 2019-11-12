//-----------------------------------------------------------------------
// <copyright file="Renderer.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Renderer.cpp
//
// Description:
//
//-----------------------------------------------------------------------


#include "Common.h"
#include "Device.h"
#include "Node.h"
#include "Renderer.h"
#include <gdiplus.h>
using namespace Gdiplus;


// GDIPlus related variables and helper functions
const int RETURN_ERROR = -1;
#define ENCODER_BITMAP L"image/bmp"
const int MAX_GLANCE_DATA_LINES = 20;

// Define defaults for Renderer Capabilities static members
bool         CRendererBase::m_fRendererCapabilitiesAreDefined = false;
wchar_t*     CRendererBase::m_wszDefaultBackgroundTitle = NULL;
wchar_t*     CRendererBase::m_wszDefaultBackgroundBody = NULL;
wchar_t*     CRendererBase::m_wszFontName = NULL;
unsigned int CRendererBase::m_wFontSizeInPixels = 0;
unsigned int CRendererBase::m_wLineHeightInPixels = 0;
unsigned int CRendererBase::m_wNumberOfLinesDisplayedOnscreen = 0;
unsigned int CRendererBase::m_wScrollAmountInPixels = 0;
unsigned int CRendererBase::m_wTextOffsetFromLeft = 0;
unsigned int CRendererBase::m_wTextOffsetFromRight = 0;
unsigned int CRendererBase::m_wTextOffsetFromTop = 0;
unsigned int CRendererBase::m_wTextOffsetFromBottom = 0;

// Define defaults for Driver Extensibility static members
volatile bool CRendererBase::m_fInvertColors = false;

// Define defaults for GDIPlus static members
void*     CRendererBase::m_pGdiplusStartupInput = NULL;
ULONG_PTR CRendererBase::m_gdiplusToken = NULL;
CLSID     CRendererBase::m_encoderClsid;


HRESULT CRendererBase::SetRendererCaps(const wchar_t* const wszDefaultBackgroundTitle,
                                       const wchar_t* const wszDefaultBackgroundBody,
                                       const wchar_t* const wszFontName,
                                       const unsigned int wFontSizeInPixels,
                                       const unsigned int wLineHeightInPixels,
                                       const unsigned int wNumberOfLinesDisplayedOnscreen,
                                       const unsigned int wScrollAmountInPixels,
                                       const unsigned int wTextOffsetFromLeft,
                                       const unsigned int wTextOffsetFromRight,
                                       const unsigned int wTextOffsetFromTop,
                                       const unsigned int wTextOffsetFromBottom)
{
    size_t cchDefaultBackgroundTitle = wcslen(wszDefaultBackgroundTitle) + 1;
    m_wszDefaultBackgroundTitle = new(std::nothrow) wchar_t[cchDefaultBackgroundTitle];
    if (NULL == m_wszDefaultBackgroundTitle)
    {
        ReleaseRendererCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszDefaultBackgroundTitle, cchDefaultBackgroundTitle, wszDefaultBackgroundTitle);

    size_t cchDefaultBackgroundBody = wcslen(wszDefaultBackgroundBody) + 1;
    m_wszDefaultBackgroundBody = new(std::nothrow) wchar_t[cchDefaultBackgroundBody];
    if (NULL == m_wszDefaultBackgroundBody)
    {
        ReleaseRendererCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszDefaultBackgroundBody, cchDefaultBackgroundBody, wszDefaultBackgroundBody);

    size_t cchFontName = wcslen(wszFontName) + 1;
    m_wszFontName = new(std::nothrow) wchar_t[cchFontName];
    if (NULL == m_wszFontName)
    {
        ReleaseRendererCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszFontName, cchFontName, wszFontName);

    m_wFontSizeInPixels = wFontSizeInPixels;
    m_wLineHeightInPixels = wLineHeightInPixels;
    m_wNumberOfLinesDisplayedOnscreen = wNumberOfLinesDisplayedOnscreen;
    m_wScrollAmountInPixels = wScrollAmountInPixels;
    m_wTextOffsetFromLeft = wTextOffsetFromLeft;
    m_wTextOffsetFromRight = wTextOffsetFromRight;
    m_wTextOffsetFromTop = wTextOffsetFromTop;
    m_wTextOffsetFromBottom = wTextOffsetFromBottom;
    
    m_fRendererCapabilitiesAreDefined = true;
    return S_OK;
}


HRESULT CRendererBase::ReleaseRendererCaps(void)
{
    if (NULL == m_wszDefaultBackgroundTitle)
    {
        delete [] m_wszDefaultBackgroundTitle;
        m_wszDefaultBackgroundTitle = NULL;
    }

    if (NULL == m_wszDefaultBackgroundBody)
    {
        delete [] m_wszDefaultBackgroundBody;
        m_wszDefaultBackgroundBody = NULL;
    }

    if (NULL == m_wszFontName)
    {
        delete [] m_wszFontName;
        m_wszFontName = NULL;
    }

    m_wFontSizeInPixels = 0;
    m_wLineHeightInPixels = 0;
    m_wNumberOfLinesDisplayedOnscreen = 0;
    m_wScrollAmountInPixels = 0;
    m_wTextOffsetFromLeft = 0;
    m_wTextOffsetFromRight = 0;
    m_wTextOffsetFromTop = 0;
    m_wTextOffsetFromBottom = 0;

    m_fRendererCapabilitiesAreDefined = false;
    return S_OK;
}


// [in] NodeToRender
// [out] pRenderedData (caller must call pRenderedData->DeleteData). If pRenderedData isn't empty, then this will leak.
HRESULT CRendererApplication::RenderData(CNodeApplication* const pNodeToRender, CRenderedData* pRenderedData)
{
    HRESULT hr = S_OK;

    if ((NULL == pNodeToRender) ||
        (NULL == pRenderedData))
    {
        return E_POINTER;
    }

    pRenderedData->DataType = CRenderedData::DataTypeApplication;
    hr = ConvertNodeToBuffer(pNodeToRender->m_wszTitle,
                             pNodeToRender->m_SizeOfTitle,
                             pNodeToRender->m_wszBody,
                             pNodeToRender->m_SizeOfBody,
                             pRenderedData);

    pNodeToRender->m_fNeedToUpdateDevice = false;

    return hr;
}


// [in] NodeToRender
// [out] pRenderedData (caller must call pRenderedData->DeleteData). If pRenderedData isn't empty, then this will leak.
HRESULT CRendererNotification::RenderData(CNodeNotification* const pNodeToRender, CRenderedData* pRenderedData)
{
    HRESULT hr = S_OK;

    if ((NULL == pNodeToRender) ||
        (NULL == pRenderedData))
    {
        return E_POINTER;
    }

    pRenderedData->DataType = CRenderedData::DataTypeNotification;
    hr = ConvertNodeToBuffer(pNodeToRender->m_wszTitle,
                             pNodeToRender->m_SizeOfTitle,
                             pNodeToRender->m_wszBody,
                             pNodeToRender->m_SizeOfBody,
                             pRenderedData);

    pNodeToRender->m_fNeedToUpdateDevice = false;

    return hr;
}


// [in] NodeToRender
// [out] pRenderedData (caller must call pRenderedData->DeleteData). If pRenderedData isn't empty, then this will leak.
HRESULT CRendererDefaultBackground::RenderData(CNodeDefaultBackground* const pNodeToRender, CRenderedData* pRenderedData)
{
    HRESULT hr = S_OK;

    if ((NULL == pNodeToRender) ||
        (NULL == pRenderedData))
    {
        return E_POINTER;
    }

    pRenderedData->DataType = CRenderedData::DataTypeDefaultBackground;
    hr = ConvertNodeToBuffer(pNodeToRender->m_wszTitle,
                             pNodeToRender->m_SizeOfTitle,
                             pNodeToRender->m_wszBody,
                             pNodeToRender->m_SizeOfBody,
                             pRenderedData);

    pNodeToRender->m_fNeedToUpdateDevice = false;

    return hr;
}


HRESULT CRendererBase::GDIPlusInitialization(void)
{
    m_pGdiplusStartupInput = new(std::nothrow) GdiplusStartupInput;
    if (NULL == m_pGdiplusStartupInput)
    {
        return E_OUTOFMEMORY;
    }

    if (Ok == GdiplusStartup(&m_gdiplusToken, (GdiplusStartupInput*)m_pGdiplusStartupInput, NULL))
    {
        return SetGlobalEncoderClsid();
    }
    else
    {
        return E_FAIL;
    }
}


HRESULT CRendererBase::GDIPlusShutdown(void)
{
    GdiplusShutdown(m_gdiplusToken);

    if (NULL != m_pGdiplusStartupInput)
    {
        delete m_pGdiplusStartupInput;
        m_pGdiplusStartupInput = NULL;
    }

    return S_OK;
}


HRESULT CRendererBase::SetGlobalEncoderClsid(void)
{
    int nResult = GetEncoderClsid(ENCODER_BITMAP, &m_encoderClsid);
    if (RETURN_ERROR != nResult)
    {
        return S_OK;
    }
    else
    {
        return E_FAIL;
    }
}


int CRendererBase::GetEncoderClsid(const wchar_t* format, CLSID* pClsid)
{
    UINT uiNumberOfEncoders = 0; // Number of image encoders
    UINT uiSizeOfEncodersArray = 0; // Size of the image encoder array in bytes
    ImageCodecInfo* pImageCodecInfo = NULL;

    GetImageEncodersSize(&uiNumberOfEncoders, &uiSizeOfEncodersArray);
    if (0 == uiSizeOfEncodersArray)
    {
        return RETURN_ERROR; // Failure
    }

    pImageCodecInfo = (ImageCodecInfo*)(malloc(uiSizeOfEncodersArray));
    if (NULL == pImageCodecInfo)
    {
        return RETURN_ERROR; // Failure
    }

    GetImageEncoders(uiNumberOfEncoders, uiSizeOfEncodersArray, pImageCodecInfo);

    for(UINT uiCounter = 0; uiCounter < uiNumberOfEncoders; uiCounter++)
    {
        if (0 == wcscmp(pImageCodecInfo[uiCounter].MimeType, format))
        {
            *pClsid = pImageCodecInfo[uiCounter].Clsid;
            free(pImageCodecInfo);
            return uiCounter; // Success
        }
    }

    free(pImageCodecInfo);
    return RETURN_ERROR; // Failure
}


// [in] wszTitle
// [in] size_t SizeOfTitle
// [in] wszBody
// [in] size_t SizeOfBody
// [out] pRenderedData (caller must call pRenderedData->DeleteData). If pRenderedData isn't empty, then this will leak.
//
// Return Values:
//      S_OK    : Succeeded
//      HRESULT_FROM_WIN32(ERROR_APP_INIT_FAILURE): You must first call CDevice::SetDeviceCaps(...) and
//                                                  CRendererBase::SetRendererCaps(...) before using the driver.

HRESULT CRendererBase::ConvertNodeToBuffer(const wchar_t* const wszTitle,
                                           const size_t SizeOfTitle,
                                           const wchar_t* const wszBody,
                                           const size_t SizeOfBody,
                                           CRenderedData* pRenderedData)
{
    // If the DeviceCaps or the RendererCaps have not been set, then this method cannot render properly
    if ((false == CDevice::AreDeviceCapalitiesDefined()) ||
        (false == m_fRendererCapabilitiesAreDefined))
    {
        return HRESULT_FROM_WIN32(ERROR_APP_INIT_FAILURE);
    }

    if ((NULL == wszTitle) ||
        (NULL == wszBody) ||
        (NULL == pRenderedData))
    {
        return E_POINTER;
    }

    // One or the other can be blank, but not both
    if ((0 == SizeOfTitle) &&
        (0 == SizeOfBody))
    {
        return E_INVALIDARG;
    }

    pRenderedData->wszAlphanumericTitle = new(std::nothrow) wchar_t[SizeOfTitle];
    if (NULL != pRenderedData->wszAlphanumericTitle)
    {
        wcscpy_s(pRenderedData->wszAlphanumericTitle, SizeOfTitle, wszTitle);
        pRenderedData->cElementsAlphanumericTitle = SizeOfTitle;
    }

    pRenderedData->wszAlphanumericBody = new(std::nothrow) wchar_t[SizeOfBody];
    if (NULL != pRenderedData->wszAlphanumericBody)
    {
        wcscpy_s(pRenderedData->wszAlphanumericBody, SizeOfBody, wszBody);
        pRenderedData->cElementsAlphanumericBody = SizeOfBody;
    }

    const DWORD dwXDimension = CDevice::GetDeviceHorizontalResolution();
    const DWORD dwYDimension = CDevice::GetDeviceVerticalResolution() + (MAX_GLANCE_DATA_LINES * m_wLineHeightInPixels); // Allows for Enhanced Glance Data
    const DWORD dwBitDepth = CDevice::GetDeviceBitDepth();

    HDC hdcCanvas = CreateDC(L"DISPLAY", NULL, NULL, NULL);
    HDC hdcCompatible = CreateCompatibleDC(hdcCanvas);
    HBITMAP hbmBitmap = CreateBitmap(dwXDimension,
                                     dwYDimension,
                                     0,
                                     dwBitDepth,
                                     NULL);
    HGDIOBJ hPreviousBitmap = SelectObject(hdcCompatible, hbmBitmap);

    if (1 == dwBitDepth)
    {
        // If the device is monochrome, then pick white text and black background. The device
        // will probably invert these colors and show black text on a white background on the device.
        SetTextColor(hdcCompatible, RGB(255, 255, 255));
        SetBkColor(hdcCompatible, RGB(0, 0, 0));
    }
    else
    {
        // If the device is color, then use the system colors for text and background.
        SetTextColor(hdcCompatible, GetSysColor(COLOR_WINDOWTEXT));
        SetBkColor(hdcCompatible, GetSysColor(COLOR_WINDOW));
    }

    LOGFONT logFont;
    ZeroMemory(&logFont, sizeof(logFont));
    logFont.lfHeight = m_wFontSizeInPixels;
    logFont.lfWeight = FW_NORMAL;
    wcsncpy_s(logFont.lfFaceName, LF_FACESIZE, m_wszFontName, wcslen(m_wszFontName) + 1);

    HFONT hFont = CreateFontIndirect(&logFont); // Expensive
    HFONT hPreviousFont = (HFONT)SelectObject(hdcCompatible, hFont); // Expensive

    RECT rectTitle = {m_wTextOffsetFromLeft,
                      m_wTextOffsetFromTop,
                      dwXDimension - m_wTextOffsetFromRight,
                      m_wTextOffsetFromTop + m_wFontSizeInPixels};
    DrawText(hdcCompatible, // If return is 0, this function failed
             wszTitle,
             (int)SizeOfTitle - 1,
             &rectTitle,
             DT_NOPREFIX | DT_NOCLIP | DT_SINGLELINE | DT_TOP | DT_WORD_ELLIPSIS);

    pRenderedData->NumberOfRenderedTextLines = 0;
    if (0 < SizeOfBody) // Only try to display the body if it is greater than 0 characters
    {
        // Split wszBody into more lines if there is an embedded newline (specifically, a 0x000A) in the string
        const wchar_t wchNewline = 0x000A;
        int nBeginningOfLineIndex = 0;
        size_t SizeOfBody_Line[MAX_GLANCE_DATA_LINES] = {SizeOfBody, 0};
        int nNumberOfRenderedBodyLines = 1;
        for (unsigned int nIndex = 0; nIndex < SizeOfBody; nIndex++) // nIndex is the current cursor position in the string
        {
            // This if statement checks if we've hit the end of a line
            if ((wchNewline == wszBody[nIndex]) || // If the current character is a newline...
                (L'\0' == wszBody[nIndex]) || // or if the current character is a terminating NULL...
                (SizeOfBody <= nIndex + 1)) // or if we've hit the end of the buffer...
            {
                SizeOfBody_Line[nNumberOfRenderedBodyLines - 1] = nIndex + 1 - nBeginningOfLineIndex; // ...then set the size of the current line
                if ((SizeOfBody <= nIndex + 1) || // If this is the end of the buffer
                    (L'\0' == wszBody[nIndex + 1]) || // or if the next character is a terminating NULL
                    (MAX_GLANCE_DATA_LINES <= nNumberOfRenderedBodyLines)) // or if we've hit the maximum number of renderable lines
                {
                    break; // Exit the loop. nNumberOfRenderedLines is set correctly
                }
                // Else there is at least one more line

                nNumberOfRenderedBodyLines++;
                nBeginningOfLineIndex = nIndex + 1; // set the 'beginning of the line' index
            }
        }

        nBeginningOfLineIndex = 0;
        int nY_Offset = m_wTextOffsetFromTop + m_wLineHeightInPixels;
        for (int nLine = 0; nLine < nNumberOfRenderedBodyLines; nLine++)
        {
            // Display wszBody Line x if it exists

            RECT rectBody = {m_wTextOffsetFromLeft,
                             nY_Offset,
                             dwXDimension - m_wTextOffsetFromRight,
                             nY_Offset + m_wFontSizeInPixels};
            int SizeOfCurrentBodyLine = (((int)SizeOfBody_Line[nLine] - 1) < (int)SizeOfBody_Line[nLine]) ? ((int)SizeOfBody_Line[nLine] - 1) : (0);
            DrawText(hdcCompatible, // If return is 0, this function failed
                     wszBody + nBeginningOfLineIndex,
                     SizeOfCurrentBodyLine,
                     &rectBody,
                     DT_NOPREFIX | DT_NOCLIP | DT_SINGLELINE | DT_TOP | DT_WORD_ELLIPSIS);
            nBeginningOfLineIndex += (int)SizeOfBody_Line[nLine];
            nY_Offset += m_wLineHeightInPixels;
        }

        pRenderedData->NumberOfRenderedTextLines = nNumberOfRenderedBodyLines + 1; // Set the number of rendered lines. +1 accounts for the Title line
    }

    SelectObject(hdcCompatible, hPreviousFont); // Select the previous object
    SelectObject(hdcCompatible, hPreviousBitmap); // Select the previous object

    // Sample code for converting the image to a Bitmap and then saving it to a file
    //      Image *pImage = Bitmap::FromHBITMAP(hbmBitmap, NULL);
    //      pImage->Save(L"BitmapDisplay.bmp", &m_encoderClsid, NULL);
    //      delete pImage;
    //      pImage = NULL;

    Bitmap* pBitmap = Bitmap::FromHBITMAP(hbmBitmap, NULL);
    if (NULL == pBitmap)
    {
        return E_FAIL;
    }
    BitmapData* pBitmapData = new(std::nothrow) BitmapData;
    if (NULL == pBitmapData)
    {
        return E_FAIL;
    }

    // The following code only works with monochrome bitmaps. It can be changed to facilitate all
    // color depths
    const Rect rect(0, 0, pBitmap->GetWidth(), pBitmap->GetHeight());
    pBitmap->LockBits(&rect,
                      ImageLockModeRead,
                      PixelFormat1bppIndexed,
                      pBitmapData);

    BYTE* pPixels = (BYTE*)pBitmapData->Scan0;

    // pPixels is a pointer to the top left pixel. This allows you to draw on the raw
    // surface which will subsequently be sent to the bitmap device.

    // If the image is monochrome, then *pPixels is the first 8 pixels.
    // Format: High bit to low bit represent the left to right pixels.

    // Other useful info:
    // BitmapData members:
    //      Width (UINT) Number of pixels in one scan line of the bitmap.
    //      Height (UINT) Number of scan lines in the bitmap.
    //      Stride (INT) Offset, in bytes, between consecutive scan lines of the bitmap.
    //          If the stride is positive, the bitmap is top-down. If the stride is negative, the bitmap is bottom-up.
    //          For example, if Stride is -32, then the next row of pixels will be at address (pPixels - 32)
    //      PixelFormat (PixelFormat) Integer that specifies the pixel format of the bitmap.
    //      Scan0 (void*) Pointer to the first (index 0) scan line of the bitmap.

    // It is assumed scan lines are byte-aligned (this is important for odd dimension displays).
    // For example: a monochrome display with a width of 17 pixels will use 3 bytes per row of pixels.

    // Compute the X Dimension in bytes
    size_t XDimensionInBytes = (dwXDimension * dwBitDepth) / 8;
    // Compensate if there's a non-byte aligned width screen dimension (add one byte to store remaining pixels)
    if (0 != (dwXDimension * dwBitDepth) % 8)
    {
        XDimensionInBytes++;
    }

    // Create the buffer that will hold the bitmap. Note this buffer only contains the raw
    // payload of the bitmap, not the header or any palette information.
    pRenderedData->cbBitmapData = XDimensionInBytes * dwYDimension;
    pRenderedData->pbBitmapData = new(std::nothrow) BYTE[pRenderedData->cbBitmapData];
    if (NULL == pRenderedData->pbBitmapData)
    {
        // Optional error handling here
    }

    // Copy the bitmap data from pBitmap to our buffer.
    // Note, if pBitmap stored the bitmap bottom-up in memory (e.g. Stride is negative),
    // this copy will convert it to top-down in the new buffer.
    int nYPixelOffset = 0;
    for (DWORD nY = 0; nY < dwYDimension; nY++)
    {
        memcpy(pRenderedData->pbBitmapData + (nY * XDimensionInBytes), pPixels + nYPixelOffset, XDimensionInBytes);
        nYPixelOffset += pBitmapData->Stride;
    }

    // Optionally invert the colors
    if ((true == GetInvertColors()) && (0 != dwYDimension))
    {
        for (size_t iBitmapData = 0; iBitmapData < pRenderedData->cbBitmapData; iBitmapData++)
        {
            pRenderedData->pbBitmapData[iBitmapData] = ~(pRenderedData->pbBitmapData[iBitmapData]);
        }
    }

    // Cleanup
    pBitmap->UnlockBits(pBitmapData);

    if (NULL != pBitmapData)
    {
        delete pBitmapData;
        pBitmapData = NULL;
    }
    if (NULL != pBitmap)
    {
        delete pBitmap;
        pBitmap = NULL;
    }

    DeleteObject(hFont);
    DeleteObject(hbmBitmap);
    DeleteObject(hdcCompatible);
    DeleteObject(hdcCanvas);

    return S_OK;
}
