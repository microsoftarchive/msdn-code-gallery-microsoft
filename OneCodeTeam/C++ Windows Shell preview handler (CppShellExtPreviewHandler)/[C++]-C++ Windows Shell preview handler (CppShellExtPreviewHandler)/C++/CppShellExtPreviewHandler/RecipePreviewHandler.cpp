/****************************** Module Header ******************************\
Module Name:  RecipePreviewHandler.cpp
Project:      CppShellExtPreviewHandler
Copyright (c) Microsoft Corporation.



This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "RecipePreviewHandler.h"
#include <Shlwapi.h>
#include <Wincrypt.h>   // For CryptStringToBinary.
#include <msxml6.h>
#include <WindowsX.h>
#include <assert.h>
#include "resource.h"

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "Crypt32.lib")
#pragma comment(lib, "msxml6.lib")


extern HINSTANCE   g_hInst;
extern long g_cDllRef;


inline int RECTWIDTH(const RECT &rc)
{
    return (rc.right - rc.left);
}

inline int RECTHEIGHT(const RECT &rc )
{
    return (rc.bottom - rc.top);
}


RecipePreviewHandler::RecipePreviewHandler() : m_cRef(1), m_pStream(NULL), 
    m_hwndParent(NULL), m_hwndPreview(NULL), m_punkSite(NULL)
{
    InterlockedIncrement(&g_cDllRef);
}

RecipePreviewHandler::~RecipePreviewHandler()
{
    if (m_hwndPreview)
    {
        DestroyWindow(m_hwndPreview);
        m_hwndPreview = NULL;
    }
    if (m_punkSite)
    {
        m_punkSite->Release();
        m_punkSite = NULL;
    }
    if (m_pStream)
    {
        m_pStream->Release();
        m_pStream = NULL;
    }

    InterlockedDecrement(&g_cDllRef);
}


#pragma region IUnknown

// Query to the interface the component supported.
IFACEMETHODIMP RecipePreviewHandler::QueryInterface(REFIID riid, void **ppv)
{
    static const QITAB qit[] = 
    {
        QITABENT(RecipePreviewHandler, IPreviewHandler),
        QITABENT(RecipePreviewHandler, IInitializeWithStream), 
        QITABENT(RecipePreviewHandler, IPreviewHandlerVisuals), 
        QITABENT(RecipePreviewHandler, IOleWindow), 
        QITABENT(RecipePreviewHandler, IObjectWithSite), 
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

// Increase the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) RecipePreviewHandler::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

// Decrease the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) RecipePreviewHandler::Release()
{
    ULONG cRef = InterlockedDecrement(&m_cRef);
    if (0 == cRef)
    {
        delete this;
    }

    return cRef;
}

#pragma endregion


#pragma region IInitializeWithStream

// Initializes the preview handler with a stream. 
// Store the IStream and mode parameters so that you can read the item's data 
// when you are ready to preview the item. Do not load the data in Initialize. 
// Load the data in IPreviewHandler::DoPreview just before you render.
IFACEMETHODIMP RecipePreviewHandler::Initialize(IStream *pStream, DWORD grfMode)
{
    HRESULT hr = E_INVALIDARG;
    if (pStream)
    {
        // Initialize can be called more than once, so release existing valid 
        // m_pStream.
        if (m_pStream)
        {
            m_pStream->Release();
            m_pStream = NULL;
        }

        m_pStream = pStream;
        m_pStream->AddRef();
        hr = S_OK;
    }
    return hr;
}

#pragma endregion


#pragma region IPreviewHandler

// This method gets called when the previewer gets created. It sets the parent 
// window of the previewer window, as well as the area within the parent to be 
// used for the previewer window.
IFACEMETHODIMP RecipePreviewHandler::SetWindow(HWND hwnd, const RECT *prc)
{
    if (hwnd && prc)
    {
        m_hwndParent = hwnd;  // Cache the HWND for later use
        m_rcParent = *prc;    // Cache the RECT for later use

        if (m_hwndPreview)
        {
            // Update preview window parent and rect information
            SetParent(m_hwndPreview, m_hwndParent);
            SetWindowPos(m_hwndPreview, NULL, m_rcParent.left, m_rcParent.top, 
                RECTWIDTH(m_rcParent), RECTHEIGHT(m_rcParent), 
                SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }
    return S_OK;
}

// Directs the preview handler to set focus to itself.
IFACEMETHODIMP RecipePreviewHandler::SetFocus()
{
    HRESULT hr = S_FALSE;
    if (m_hwndPreview)
    {
        ::SetFocus(m_hwndPreview);
        hr = S_OK;
    }
    return hr;
}

// Directs the preview handler to return the HWND from calling the GetFocus 
// function.
IFACEMETHODIMP RecipePreviewHandler::QueryFocus(HWND *phwnd)
{
    HRESULT hr = E_INVALIDARG;
    if (phwnd)
    {
        *phwnd = ::GetFocus();
        if (*phwnd)
        {
            hr = S_OK;
        }
        else
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }
    return hr;
}

// Directs the preview handler to handle a keystroke passed up from the 
// message pump of the process in which the preview handler is running.
HRESULT RecipePreviewHandler::TranslateAccelerator(MSG *pmsg)
{
    HRESULT hr = S_FALSE;
    IPreviewHandlerFrame *pFrame = NULL;
    if (m_punkSite && SUCCEEDED(m_punkSite->QueryInterface(&pFrame)))
    {
        // If your previewer has multiple tab stops, you will need to do 
        // appropriate first/last child checking. This sample previewer has 
        // no tabstops, so we want to just forward this message out.
        hr = pFrame->TranslateAccelerator(pmsg);

        pFrame->Release();
    }
    return hr;
}

// This method gets called when the size of the previewer window changes 
// (user resizes the Reading Pane). It directs the preview handler to change 
// the area within the parent hwnd that it draws into.
IFACEMETHODIMP RecipePreviewHandler::SetRect(const RECT *prc)
{
    HRESULT hr = E_INVALIDARG;
    if (prc != NULL)
    {
        m_rcParent = *prc;
        if (m_hwndPreview)
        {
            // Preview window is already created, so set its size and position.
            SetWindowPos(m_hwndPreview, NULL, m_rcParent.left, m_rcParent.top,
                (m_rcParent.right - m_rcParent.left), // Width
                (m_rcParent.bottom - m_rcParent.top), // Height
                SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);
        }
        hr = S_OK;
    }
    return hr;
}

// The method directs the preview handler to load data from the source 
// specified in an earlier Initialize method call, and to begin rendering to 
// the previewer window.
IFACEMETHODIMP RecipePreviewHandler::DoPreview()
{
    // Cannot call more than once.
    // (Unload should be called before another DoPreview)
    if (m_hwndPreview != NULL || !m_pStream) 
    {
        return E_FAIL;
    }

    HRESULT hr = E_FAIL;
    IXMLDOMDocument *pXMLDoc = NULL;
    PWSTR pszRecipeTitle = NULL;
    PWSTR pszRecipeComments = NULL;
    HBITMAP hRecipeImage = NULL;
    
    // Load the XML document.
    hr = LoadXMLDocument(&pXMLDoc);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // Read the title of the recipe.
    hr = GetRecipeTitle(pXMLDoc, &pszRecipeTitle);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // Read the comments of the recipe.
    hr = GetRecipeComments(pXMLDoc, &pszRecipeComments);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // Read the embedded recipe image.
    WTS_ALPHATYPE dwAlpha;
    hr = GetRecipeImage(pXMLDoc, &hRecipeImage, &dwAlpha);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // Create the preview window.
    hr = CreatePreviewWindow(pszRecipeTitle, pszRecipeComments, hRecipeImage);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

Cleanup:
    // Clean up the allocated resources in a centralized place.
    if (pXMLDoc)
    {
        pXMLDoc->Release();
        pXMLDoc = NULL;
    }
    if (pszRecipeTitle)
    {
        CoTaskMemFree(pszRecipeTitle);
        pszRecipeTitle = NULL;
    }
    if (pszRecipeComments)
    {
        CoTaskMemFree(pszRecipeComments);
        pszRecipeComments = NULL;
    }
    if (hRecipeImage)
    {
        DeleteObject(hRecipeImage);
        hRecipeImage = NULL;
    }

    return hr;
}

// This method gets called when a shell item is de-selected. It directs the 
// preview handler to cease rendering a preview and to release all resources 
// that have been allocated based on the item passed in during the 
// initialization.
HRESULT RecipePreviewHandler::Unload()
{
    if (m_pStream)
    {
        m_pStream->Release();
        m_pStream = NULL;
    }

    if (m_hwndPreview)
    {
        DestroyWindow(m_hwndPreview);
        m_hwndPreview = NULL;
    }

    return S_OK;
}

#pragma endregion


#pragma region IPreviewHandlerVisuals (Optional)

// Sets the background color of the preview handler.
IFACEMETHODIMP RecipePreviewHandler::SetBackgroundColor(COLORREF color)
{
    return S_OK;
}

// Sets the font attributes to be used for text within the preview handler.
IFACEMETHODIMP RecipePreviewHandler::SetFont(const LOGFONTW *plf)
{
    return S_OK;
}

// Sets the color of the text within the preview handler.
IFACEMETHODIMP RecipePreviewHandler::SetTextColor(COLORREF color)
{
    return S_OK;
}

#pragma endregion


#pragma region IOleWindow

// Retrieves a handle to one of the windows participating in in-place 
// activation (frame, document, parent, or in-place object window).
IFACEMETHODIMP RecipePreviewHandler::GetWindow(HWND *phwnd)
{
    HRESULT hr = E_INVALIDARG;
    if (phwnd)
    {
        *phwnd = m_hwndParent;
        hr = S_OK;
    }
    return hr;
}

// Determines whether context-sensitive help mode should be entered during an 
// in-place activation session
IFACEMETHODIMP RecipePreviewHandler::ContextSensitiveHelp(BOOL fEnterMode)
{
    return E_NOTIMPL;
}

#pragma endregion


#pragma region IObjectWithSite

// Provides the site's IUnknown pointer to the object.
IFACEMETHODIMP RecipePreviewHandler::SetSite(IUnknown *punkSite)
{
    if (m_punkSite)
    {
        m_punkSite->Release();
        m_punkSite = NULL;
    }
    return punkSite ? punkSite->QueryInterface(&m_punkSite) : S_OK;
}

// Gets the last site set with IObjectWithSite::SetSite. If there is no known 
// site, the object returns a failure code.
IFACEMETHODIMP RecipePreviewHandler::GetSite(REFIID riid, void **ppv)
{
    *ppv = NULL;
    return m_punkSite ? m_punkSite->QueryInterface(riid, ppv) : E_FAIL;
}

#pragma endregion


#pragma region Helper Functions

// The function uses XML Document Object Model (DOM) APIs to retrieve the XML 
// document object from the stream.
HRESULT RecipePreviewHandler::LoadXMLDocument(IXMLDOMDocument **ppXMLDoc)
{
    *ppXMLDoc = NULL;

    IXMLDOMDocument *pXMLDoc;
    HRESULT hr = CoCreateInstance(CLSID_DOMDocument60, 0, 
        CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pXMLDoc));
    if (SUCCEEDED(hr))
    {
        IPersistStream *pps;
        hr = pXMLDoc->QueryInterface(&pps);
        if (SUCCEEDED(hr))
        {
            hr = pps->Load(m_pStream);
            if (SUCCEEDED(hr))
            {
                hr = pXMLDoc->QueryInterface(ppXMLDoc);
            }
            pps->Release();
        }
        pXMLDoc->Release();
    }
    return hr;
}


// The function reads the value of the Recipe/Title node in the specified XML 
// document, and outputs the title value to the caller. The caller should 
// free the string with CoTaskMemFree when it is no longer needed.
HRESULT RecipePreviewHandler::GetRecipeTitle(IXMLDOMDocument *pXMLDoc, 
    PWSTR *ppszResult)
{
    *ppszResult = NULL;
    HRESULT hr;

    // Get the node Recipe/Title in the XML document.
    BSTR bstrQuery = SysAllocString(L"Recipe/Title");
    hr = bstrQuery ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        IXMLDOMNode *pXMLNode;
        hr = pXMLDoc->selectSingleNode(bstrQuery, &pXMLNode);
        if (SUCCEEDED(hr))
        {
            IXMLDOMElement *pXMLElement;
            hr = pXMLNode->QueryInterface(&pXMLElement);
            if (SUCCEEDED(hr))
            {
                // Read the value of the XML node.
                BSTR bstrText;
                hr = pXMLElement->get_text(&bstrText);
                if (SUCCEEDED(hr))
                {
                    hr = SHStrDup(bstrText, ppszResult);
                    SysFreeString(bstrText);
                }
                pXMLElement->Release();
            }
            pXMLNode->Release();
        }
        SysFreeString(bstrQuery);
    }

    return hr;
}


// The function reads the value of the Recipe/Comments node in the specified 
// XML document, and outputs the comments value to the caller. The caller 
// should free the string with CoTaskMemFree when it is no longer needed.
HRESULT RecipePreviewHandler::GetRecipeComments(IXMLDOMDocument *pXMLDoc, 
    PWSTR *ppszResult)
{
    *ppszResult = NULL;
    HRESULT hr;

    // Get the node Recipe/Title in the XML document.
    BSTR bstrQuery = SysAllocString(L"Recipe/Comments");
    hr = bstrQuery ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        IXMLDOMNode *pXMLNode;
        hr = pXMLDoc->selectSingleNode(bstrQuery, &pXMLNode);
        if (SUCCEEDED(hr))
        {
            IXMLDOMElement *pXMLElement;
            hr = pXMLNode->QueryInterface(&pXMLElement);
            if (SUCCEEDED(hr))
            {
                // Read the value of the XML node.
                BSTR bstrText;
                hr = pXMLElement->get_text(&bstrText);
                if (SUCCEEDED(hr))
                {
                    hr = SHStrDup(bstrText, ppszResult);
                    SysFreeString(bstrText);
                }
                pXMLElement->Release();
            }
            pXMLNode->Release();
        }
        SysFreeString(bstrQuery);
    }

    return hr;
}


// The function reads the recipe image from the Source attribute of the 
// Recipe/Attachments/Picture node in the specified XML document. The caller 
// should free the image with DeleteObject when it is no longer needed.
HRESULT RecipePreviewHandler::GetRecipeImage(IXMLDOMDocument *pXMLDoc, 
    HBITMAP *phbmp, WTS_ALPHATYPE *pdwAlpha)
{
    PWSTR pszBase64EncodedImageString = NULL;
    HRESULT hr = GetBase64EncodedImageString(pXMLDoc, 
        &pszBase64EncodedImageString);
    if (SUCCEEDED(hr))
    {
        // Convert the base64-encoded string to a stream.
        IStream *pImageStream = NULL;
        hr = GetStreamFromString(pszBase64EncodedImageString, &pImageStream);
        if (SUCCEEDED(hr))
        {
            // Create a 32bpp HBITMAP from the stream.
            hr = WICCreate32bppHBITMAP(pImageStream, phbmp, pdwAlpha);
            pImageStream->Release();
        }
        CoTaskMemFree(pszBase64EncodedImageString);
    }

    return hr;
}


// Create the preview window based on the recipe information.
HRESULT RecipePreviewHandler::CreatePreviewWindow(PWSTR pszTitle, 
    PWSTR pszComments, HBITMAP hImage)
{
    assert(m_hwndPreview == NULL);
    assert(m_hwndParent != NULL);
    
    HRESULT hr = S_OK;

    m_hwndPreview = CreateDialog(g_hInst, MAKEINTRESOURCE(IDD_MAINDIALOG), 
        m_hwndParent, NULL);
    if (m_hwndPreview == NULL)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }

    if (SUCCEEDED(hr))
    {
        // Set the preview window position.
        SetWindowPos(m_hwndPreview, NULL, m_rcParent.left, m_rcParent.top,
            RECTWIDTH(m_rcParent), RECTHEIGHT(m_rcParent), 
            SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);

        // Set the title label on the window.
        HWND hLabelTitle = GetDlgItem(m_hwndPreview, IDC_STATIC_TITLE);
        Static_SetText(hLabelTitle, pszTitle);

        // Set the comments textbox on the window.
        HWND hEditComments = GetDlgItem(m_hwndPreview, IDC_EDIT_COMMENTS);
        Edit_SetText(hEditComments, pszComments);

        // Set the picture box on the window.
        HWND hPctbox = GetDlgItem(m_hwndPreview, IDC_STATIC_PICTURE);
        SendMessage(hPctbox, STM_SETIMAGE, IMAGE_BITMAP, reinterpret_cast<LPARAM>(hImage));

        // Show the preview window.
        ShowWindow(m_hwndPreview, SW_SHOW);
    }

    return hr;
}


// The function reads the Source attribute of the Recipe/Attachments/Picture 
// node in the specified XML document, and gets the base64-encoded string 
// value which represents the recipe image. The caller should free the string 
// with CoTaskMemFree when it is no longer needed. 
HRESULT RecipePreviewHandler::GetBase64EncodedImageString(
    IXMLDOMDocument *pXMLDoc, PWSTR *ppszResult)
{
    *ppszResult = NULL;
    HRESULT hr;

    // Get the node Recipe/Attachments/Picture in the XML document.
    BSTR bstrQuery = SysAllocString(L"Recipe/Attachments/Picture");
    hr = bstrQuery ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        IXMLDOMNode *pXMLNode;
        hr = pXMLDoc->selectSingleNode(bstrQuery, &pXMLNode);
        if (SUCCEEDED(hr))
        {
            IXMLDOMElement *pXMLElement;
            hr = pXMLNode->QueryInterface(&pXMLElement);
            if (SUCCEEDED(hr))
            {
                // Read the "Source" attribute of the XML node.
                BSTR bstrAttribute = SysAllocString(L"Source");
                hr = bstrAttribute ? S_OK : E_OUTOFMEMORY;
                if (SUCCEEDED(hr))
                {
                    VARIANT varValue;
                    hr = pXMLElement->getAttribute(bstrAttribute, &varValue);
                    if (SUCCEEDED(hr))
                    {
                        // Output the "Source" string attribute.
                        if ((varValue.vt == VT_BSTR) && varValue.bstrVal && 
                            varValue.bstrVal[0] /* not an empty string */)
                        {
                            hr = SHStrDup(varValue.bstrVal, ppszResult);
                        }
                        else
                        {
                            hr = E_FAIL;
                        }
                        VariantClear(&varValue);
                    }
                    SysFreeString(bstrAttribute);
                }
                pXMLElement->Release();
            }
            pXMLNode->Release();
        }
        SysFreeString(bstrQuery);
    }

    return hr;
}


// The function decodes the base64-encoded string to a stream.
HRESULT RecipePreviewHandler::GetStreamFromString(PCWSTR pszImageString, 
    IStream **ppImageStream)
{
    HRESULT hr = E_FAIL;

    DWORD dwDecodedImageSize = 0;
    DWORD dwSkipChars = 0;
    DWORD dwActualFormat = 0;

    // Base64-decode the string
    if (CryptStringToBinary(pszImageString, NULL, CRYPT_STRING_BASE64, 
        NULL, &dwDecodedImageSize, &dwSkipChars, &dwActualFormat))
    {
        BYTE *pbDecodedImage = static_cast<BYTE *>(LocalAlloc(LPTR, 
            dwDecodedImageSize));
        if (pbDecodedImage)
        {
            if (CryptStringToBinary(pszImageString, lstrlen(pszImageString), 
                CRYPT_STRING_BASE64, pbDecodedImage, &dwDecodedImageSize, 
                &dwSkipChars, &dwActualFormat))
            {
                *ppImageStream = SHCreateMemStream(pbDecodedImage, dwDecodedImageSize);
                if (*ppImageStream != NULL)
                {
                    hr = S_OK;
                }
            }
            LocalFree(pbDecodedImage);
        }
    }
    return hr;
}


HRESULT RecipePreviewHandler::ConvertBitmapSourceTo32bppHBITMAP(
    IWICBitmapSource *pBitmapSource, IWICImagingFactory *pImagingFactory, 
    HBITMAP *phbmp)
{
    *phbmp = NULL;

    IWICBitmapSource *pBitmapSourceConverted = NULL;
    WICPixelFormatGUID guidPixelFormatSource;
    HRESULT hr = pBitmapSource->GetPixelFormat(&guidPixelFormatSource);

    if (SUCCEEDED(hr) && (guidPixelFormatSource != GUID_WICPixelFormat32bppBGRA))
    {
        IWICFormatConverter *pFormatConverter;
        hr = pImagingFactory->CreateFormatConverter(&pFormatConverter);
        if (SUCCEEDED(hr))
        {
            // Create the appropriate pixel format converter.
            hr = pFormatConverter->Initialize(pBitmapSource, 
                GUID_WICPixelFormat32bppBGRA, WICBitmapDitherTypeNone, NULL, 
                0, WICBitmapPaletteTypeCustom);
            if (SUCCEEDED(hr))
            {
                hr = pFormatConverter->QueryInterface(&pBitmapSourceConverted);
            }
            pFormatConverter->Release();
        }
    }
    else
    {
        // No conversion is necessary.
        hr = pBitmapSource->QueryInterface(&pBitmapSourceConverted);
    }

    if (SUCCEEDED(hr))
    {
        UINT nWidth, nHeight;
        hr = pBitmapSourceConverted->GetSize(&nWidth, &nHeight);
        if (SUCCEEDED(hr))
        {
            BITMAPINFO bmi = { sizeof(bmi.bmiHeader) };
            bmi.bmiHeader.biWidth = nWidth;
            bmi.bmiHeader.biHeight = -static_cast<LONG>(nHeight);
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 32;
            bmi.bmiHeader.biCompression = BI_RGB;

            BYTE *pBits;
            HBITMAP hbmp = CreateDIBSection(NULL, &bmi, DIB_RGB_COLORS, 
                reinterpret_cast<void **>(&pBits), NULL, 0);
            hr = hbmp ? S_OK : E_OUTOFMEMORY;
            if (SUCCEEDED(hr))
            {
                WICRect rect = {0, 0, nWidth, nHeight};

                // Convert the pixels and store them in the HBITMAP.  
                // Note: the name of the function is a little misleading - 
                // we're not doing any extraneous copying here.  CopyPixels 
                // is actually converting the image into the given buffer.
                hr = pBitmapSourceConverted->CopyPixels(&rect, nWidth * 4, 
                    nWidth * nHeight * 4, pBits);
                if (SUCCEEDED(hr))
                {
                    *phbmp = hbmp;
                }
                else
                {
                    DeleteObject(hbmp);
                }
            }
        }
        pBitmapSourceConverted->Release();
    }
    return hr;
}


HRESULT RecipePreviewHandler::WICCreate32bppHBITMAP(IStream *pstm, 
    HBITMAP *phbmp, WTS_ALPHATYPE *pdwAlpha)
{
    *phbmp = NULL;

    // Create the COM imaging factory.
    IWICImagingFactory *pImagingFactory;
    HRESULT hr = CoCreateInstance(CLSID_WICImagingFactory, NULL, 
        CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pImagingFactory));
    if (SUCCEEDED(hr))
    {
        // Create an appropriate decoder.
        IWICBitmapDecoder *pDecoder;
        hr = pImagingFactory->CreateDecoderFromStream(pstm, 
            &GUID_VendorMicrosoft, WICDecodeMetadataCacheOnDemand, &pDecoder);
        if (SUCCEEDED(hr))
        {
            IWICBitmapFrameDecode *pBitmapFrameDecode;
            hr = pDecoder->GetFrame(0, &pBitmapFrameDecode);
            if (SUCCEEDED(hr))
            {
                hr = ConvertBitmapSourceTo32bppHBITMAP(pBitmapFrameDecode, 
                    pImagingFactory, phbmp);
                if (SUCCEEDED(hr))
                {
                    *pdwAlpha = WTSAT_ARGB;
                }
                pBitmapFrameDecode->Release();
            }
            pDecoder->Release();
        }
        pImagingFactory->Release();
    }
    return hr;
}

#pragma endregion