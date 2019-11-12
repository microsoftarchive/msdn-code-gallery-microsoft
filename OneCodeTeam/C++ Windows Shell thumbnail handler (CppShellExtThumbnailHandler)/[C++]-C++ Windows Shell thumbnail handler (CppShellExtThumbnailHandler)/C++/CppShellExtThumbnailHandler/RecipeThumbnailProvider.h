/****************************** Module Header ******************************\
Module Name:  RecipeThumbnailProvider.h
Project:      CppShellExtThumbnailHandler
Copyright (c) Microsoft Corporation.

The code sample demonstrates the C++ implementation of a thumbnail handler 
for a new file type registered with the .recipe extension. 

A thumbnail image handler provides an image to represent the item. It lets 
you customize the thumbnail of files with a specific file extension. Windows 
Vista and newer operating systems make greater use of file-specific thumbnail 
images than earlier versions of Windows. Thumbnails of 32-bit resolution and 
as large as 256x256 pixels are often used. File format owners should be 
prepared to display their thumbnails at that size. 

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
\***************************************************************************/

#pragma once

#include <windows.h>
#include <thumbcache.h>     // For IThumbnailProvider
#include <wincodec.h>       // Windows Imaging Codecs

#pragma comment(lib, "windowscodecs.lib")


class RecipeThumbnailProvider : 
    public IInitializeWithStream, 
    public IThumbnailProvider
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IInitializeWithStream
    IFACEMETHODIMP Initialize(IStream *pStream, DWORD grfMode);

    // IThumbnailProvider
    IFACEMETHODIMP GetThumbnail(UINT cx, HBITMAP *phbmp, WTS_ALPHATYPE *pdwAlpha);

    RecipeThumbnailProvider();

protected:
    ~RecipeThumbnailProvider();

private:
    // Reference count of component.
    long m_cRef;

    // Provided during initialization.
    IStream *m_pStream;

    HRESULT LoadXMLDocument(IXMLDOMDocument **ppXMLDoc);

    HRESULT GetRecipeImage(
        IXMLDOMDocument *pXMLDoc, 
        UINT cx, 
        HBITMAP *phbmp, 
        WTS_ALPHATYPE *pdwAlpha);

    HRESULT GetBase64EncodedImageString(
        IXMLDOMDocument *pXMLDoc, 
        UINT cx, 
        PWSTR *ppszResult);

    HRESULT GetStreamFromString(PCWSTR pszImageString, IStream **ppStream);

    HRESULT ConvertBitmapSourceTo32bppHBITMAP(
        IWICBitmapSource *pBitmapSource, 
        IWICImagingFactory *pImagingFactory, 
        HBITMAP *phbmp);

    HRESULT WICCreate32bppHBITMAP(IStream *pstm, HBITMAP *phbmp, 
        WTS_ALPHATYPE *pdwAlpha);
};