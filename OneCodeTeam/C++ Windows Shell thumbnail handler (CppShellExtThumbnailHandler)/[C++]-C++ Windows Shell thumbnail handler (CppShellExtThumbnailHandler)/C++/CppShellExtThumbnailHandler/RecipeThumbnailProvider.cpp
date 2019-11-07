/******************************** Module Header ********************************\
Module Name:  RecipeThumbnailProvider.cpp
Project:      CppShellExtThumbnailHandler
Copyright (c) Microsoft Corporation.

The code sample demonstrates the C++ implementation of a thumbnail handler 
for a new file type registered with the .recipe extension. 

A thumbnail image handler provides an image to represent the item. It lets you 
customize the thumbnail of files with a specific file extension. Windows Vista 
and newer operating systems make greater use of file-specific thumbnail images 
than earlier versions of Windows. Thumbnails of 32-bit resolution and as large 
as 256x256 pixels are often used. File format owners should be prepared to 
display their thumbnails at that size. 

The example thumbnail handler implements the IInitializeWithStream and 
IThumbnailProvider interfaces, and provides thumbnails for .recipe files. 
The .recipe file type is simply an XML file registered as a unique file name 
extension. It includes an element called "Picture", embedding an image file. 
The thumbnail handler extracts the embedded image and asks the Shell to 
display it as a thumbnail.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#include "RecipeThumbnailProvider.h"
#include <Shlwapi.h>
#include <Wincrypt.h>   // For CryptStringToBinary.
#include <msxml6.h>

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "Crypt32.lib")
#pragma comment(lib, "msxml6.lib")


extern HINSTANCE g_hInst;
extern long g_cDllRef;


RecipeThumbnailProvider::RecipeThumbnailProvider() : m_cRef(1), m_pStream(NULL)
{
    InterlockedIncrement(&g_cDllRef);
}


RecipeThumbnailProvider::~RecipeThumbnailProvider()
{
    InterlockedDecrement(&g_cDllRef);
}


#pragma region IUnknown

// Query to the interface the component supported.
IFACEMETHODIMP RecipeThumbnailProvider::QueryInterface(REFIID riid, void **ppv)
{
    static const QITAB qit[] = 
    {
        QITABENT(RecipeThumbnailProvider, IThumbnailProvider),
        QITABENT(RecipeThumbnailProvider, IInitializeWithStream), 
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

// Increase the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) RecipeThumbnailProvider::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

// Decrease the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) RecipeThumbnailProvider::Release()
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

// Initializes the thumbnail handler with a stream.
IFACEMETHODIMP RecipeThumbnailProvider::Initialize(IStream *pStream, DWORD grfMode)
{
    // A handler instance should be initialized only once in its lifetime. 
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
    if (m_pStream == NULL)
    {
        // Take a reference to the stream if it has not been initialized yet.
        hr = pStream->QueryInterface(&m_pStream);
    }
    return hr;
}

#pragma endregion


#pragma region IThumbnailProvider

// Gets a thumbnail image and alpha type. The GetThumbnail is called with the 
// largest desired size of the image, in pixels. Although the parameter is 
// called cx, this is used as the maximum size of both the x and y dimensions. 
// If the retrieved thumbnail is not square, then the longer axis is limited 
// by cx and the aspect ratio of the original image respected. On exit, 
// GetThumbnail provides a handle to the retrieved image. It also provides a 
// value that indicates the color format of the image and whether it has 
// valid alpha information.
IFACEMETHODIMP RecipeThumbnailProvider::GetThumbnail(UINT cx, HBITMAP *phbmp, 
    WTS_ALPHATYPE *pdwAlpha)
{
    // Load the XML document.
    IXMLDOMDocument *pXMLDoc = NULL;
    HRESULT hr = LoadXMLDocument(&pXMLDoc);
    if (SUCCEEDED(hr))
    {
        // Read the preview image from the XML document.
        hr = GetRecipeImage(pXMLDoc, cx, phbmp, pdwAlpha);
        pXMLDoc->Release();
    }
    return hr;
}

#pragma endregion


#pragma region Helper Functions

// The function uses XML Document Object Model (DOM) APIs to retrieve the XML 
// document object from the stream.
HRESULT RecipeThumbnailProvider::LoadXMLDocument(IXMLDOMDocument **ppXMLDoc)
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


// The function reads the recipe image from the Source attribute of the 
// Recipe/Attachments/Picture node in the specified XML document. The caller 
// should free the image with DeleteObject when it is no longer needed.
HRESULT RecipeThumbnailProvider::GetRecipeImage(IXMLDOMDocument *pXMLDoc, 
    UINT cx, HBITMAP *phbmp, WTS_ALPHATYPE *pdwAlpha)
{
    // Read the base64-encoded image string in the XML document.
    PWSTR pszBase64EncodedImageString = NULL;
    HRESULT hr = GetBase64EncodedImageString(pXMLDoc, cx, 
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


// The function reads the Source attribute of the Recipe/Attachments/Picture 
// node in the specified XML document, and gets the base64-encoded string 
// value which represents the thumbnail image. The cx paramter specifies the 
// desired size of the image. For simplicity, the sample omits the cx 
// paramter and provides only one image for all situations.
HRESULT RecipeThumbnailProvider::GetBase64EncodedImageString(
    IXMLDOMDocument *pXMLDoc, UINT /*cx*/, PWSTR *ppszResult)
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
HRESULT RecipeThumbnailProvider::GetStreamFromString(PCWSTR pszImageString, 
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


HRESULT RecipeThumbnailProvider::ConvertBitmapSourceTo32bppHBITMAP(
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


HRESULT RecipeThumbnailProvider::WICCreate32bppHBITMAP(IStream *pstm, 
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