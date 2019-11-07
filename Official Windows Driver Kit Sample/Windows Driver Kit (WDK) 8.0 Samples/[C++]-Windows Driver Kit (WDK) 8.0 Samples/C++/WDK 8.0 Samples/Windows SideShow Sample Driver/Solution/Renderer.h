//-----------------------------------------------------------------------
// <copyright file="Renderer.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Renderer.h
//
// Description:
//      This class implements the bitmap rendering portion of the driver.
//
//-----------------------------------------------------------------------


#pragma once


#include <windows.h>
#include "RenderedData.h"


class CNodeApplication;
class CNodeNotification;
class CNodeDefaultBackground;


// Singleton object
class CRendererBase
{
public:
    CRendererBase(void){}
    virtual ~CRendererBase(void){}

    //virtual HRESULT RenderData(...) = 0; Don't make virtual due to being a singleton

    // Sets the renderer capabilities and defines any renderer rules
    static HRESULT SetRendererCaps(const wchar_t* const wszDefaultBackgroundTitle, // The default title screen shown on the device display when no gadgets are running
                                   const wchar_t* const wszDefaultBackgroundBody, // The default body screen shown on the device display when no gadgets are running
                                   const wchar_t* const wszFontName = L"Tahoma", // The name of the font used for rendering
                                   const unsigned int wFontSizeInPixels = 11, // The size of the font used for rendering
                                   const unsigned int wLineHeightInPixels = 13, // The height of pixels for each line of text. This can be different than the font size
                                                                                // so that the text can be spaced out more if desired
                                   const unsigned int wNumberOfLinesDisplayedOnscreen = 3, // Number of lines of text displayed on screen at one time
                                   const unsigned int wScrollAmountInPixels = 13, // Amount of pixels to scroll each time the user presses the "more/less glance data" button
                                   const unsigned int wTextOffsetFromLeft = 3, // Number of pixels to offset the start of text from the left side of the device display
                                   const unsigned int wTextOffsetFromRight = 3, // Number of pixels to offset the end of text from the right side of the device display
                                   const unsigned int wTextOffsetFromTop = 3, // Number of pixels to offset the start of text from the top of the device display
                                   const unsigned int wTextOffsetFromBottom = 3); // Number of pixels to offset the end of text from the bottom of the device display

    // Releases memory used by RendererCaps and resets all values to 0 / NULL
    static HRESULT ReleaseRendererCaps(void);

    // RendererCaps Accessors
    static bool         AreRendererCapabilitiesDefined(void) {return m_fRendererCapabilitiesAreDefined;}
    static wchar_t*     GetDefaultBackgroundTitle(void) {return m_wszDefaultBackgroundTitle;}
    static wchar_t*     GetDefaultBackgroundBody(void) {return m_wszDefaultBackgroundBody;}
    static wchar_t*     GetFontName(void) {return m_wszFontName;}
    static unsigned int GetFontSizeInPixels(void) {return m_wFontSizeInPixels;}
    static unsigned int GetLineHeightInPixels(void) {return m_wLineHeightInPixels;}
    static unsigned int GetNumberOfLinesDisplayedOnscreen(void) {return m_wNumberOfLinesDisplayedOnscreen;}
    static unsigned int GetScrollAmountInPixels(void) {return m_wScrollAmountInPixels;}
    static unsigned int GetTextOffsetFromLeft(void) {return m_wTextOffsetFromLeft;}
    static unsigned int GetTextOffsetFromRight(void) {return m_wTextOffsetFromRight;}
    static unsigned int GetTextOffsetFromTop(void) {return m_wTextOffsetFromTop;}
    static unsigned int GetTextOffsetFromBottom(void) {return m_wTextOffsetFromBottom;}

    // Driver Extensibility Accessors
    static void SetInvertColors(const bool fInvertColors) {m_fInvertColors = fInvertColors; return;}
    static bool GetInvertColors(void) {return m_fInvertColors;}

    // GDIPlus Methods
    static HRESULT GDIPlusInitialization(void);
    static HRESULT GDIPlusShutdown(void);
    static HRESULT SetGlobalEncoderClsid(void);
    static int GetEncoderClsid(const wchar_t* format, CLSID* pClsid);

protected:
    HRESULT ConvertNodeToBuffer(const wchar_t* const wszTitle,
                                const size_t SizeOfTitle,
                                const wchar_t* const wszBody,
                                const size_t SizeOfBody,
                                CRenderedData* pRenderedData);

private:
    // Renderer Capabilities Members
    static bool         m_fRendererCapabilitiesAreDefined;
    static wchar_t*     m_wszDefaultBackgroundTitle;
    static wchar_t*     m_wszDefaultBackgroundBody;
    static wchar_t*     m_wszFontName;
    static unsigned int m_wFontSizeInPixels;
    static unsigned int m_wLineHeightInPixels;
    static unsigned int m_wNumberOfLinesDisplayedOnscreen;
    static unsigned int m_wScrollAmountInPixels;
    static unsigned int m_wTextOffsetFromLeft;
    static unsigned int m_wTextOffsetFromRight;
    static unsigned int m_wTextOffsetFromTop;
    static unsigned int m_wTextOffsetFromBottom;

    // Driver Extensibility Members
    volatile static bool m_fInvertColors;

    // GDIPlus Members
    static void* m_pGdiplusStartupInput;
    static ULONG_PTR m_gdiplusToken;
    static CLSID m_encoderClsid;
};


class CRendererApplication : public CRendererBase
{
public:
    // [in] pNodeToRender
    // [out] pRenderedData (caller must call pRenderedData->DeleteData)
    HRESULT RenderData(CNodeApplication* const pNodeToRender, CRenderedData* pRenderedData);
};


class CRendererNotification : public CRendererBase
{
public:
    // [in] pNodeToRender
    // [out] pRenderedData (caller must call pRenderedData->DeleteData)
    HRESULT RenderData(CNodeNotification* const pNodeToRender, CRenderedData* pRenderedData);
};


class CRendererDefaultBackground : public CRendererBase
{
public:
    // [in] pNodeToRender
    // [out] pRenderedData (caller must call pRenderedData->DeleteData)
    HRESULT RenderData(CNodeDefaultBackground* const pNodeToRender, CRenderedData* pRenderedData);
};
