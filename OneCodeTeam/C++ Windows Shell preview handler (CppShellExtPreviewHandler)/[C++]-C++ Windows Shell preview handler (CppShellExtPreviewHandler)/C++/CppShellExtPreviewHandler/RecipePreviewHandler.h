/****************************** Module Header ******************************\
Module Name:  RecipePreviewHandler.h
Project:      CppShellExtPreviewHandler
Copyright (c) Microsoft Corporation.



This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>
#include <shlobj.h>
#include <thumbcache.h>
#include <wincodec.h>       // Windows Imaging Codecs

#pragma comment(lib, "windowscodecs.lib")


class RecipePreviewHandler : 
    public IInitializeWithStream, 
    public IPreviewHandler, 
    public IPreviewHandlerVisuals, 
    public IOleWindow, 
    public IObjectWithSite
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IInitializeWithStream
    IFACEMETHODIMP Initialize(IStream *pStream, DWORD grfMode);

    // IPreviewHandler
    IFACEMETHODIMP SetWindow(HWND hwnd, const RECT *prc);
    IFACEMETHODIMP SetFocus();
    IFACEMETHODIMP QueryFocus(HWND *phwnd);
    IFACEMETHODIMP TranslateAccelerator(MSG *pmsg);
    IFACEMETHODIMP SetRect(const RECT *prc);
    IFACEMETHODIMP DoPreview();
    IFACEMETHODIMP Unload();

    // IPreviewHandlerVisuals (Optional)
    IFACEMETHODIMP SetBackgroundColor(COLORREF color);
    IFACEMETHODIMP SetFont(const LOGFONTW *plf);
    IFACEMETHODIMP SetTextColor(COLORREF color);

    // IOleWindow
    IFACEMETHODIMP GetWindow(HWND *phwnd);
    IFACEMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);

    // IObjectWithSite
    IFACEMETHODIMP SetSite(IUnknown *punkSite);
    IFACEMETHODIMP GetSite(REFIID riid, void **ppv);

    RecipePreviewHandler();

protected:
    ~RecipePreviewHandler();

private:
    // Reference count of component.
    long m_cRef;

    // Provided during initialization.
    IStream *m_pStream;

    // Parent window that hosts the previewer window.
    // Note: do NOT DestroyWindow this.
    HWND m_hwndParent;

    // Bounding rect of the parent window.
    RECT m_rcParent;

    // The actual previewer window.
    HWND m_hwndPreview;

    // Site pointer from host, used to get IPreviewHandlerFrame.
    IUnknown *m_punkSite;

    HRESULT LoadXMLDocument(IXMLDOMDocument **ppXMLDoc);

    HRESULT GetRecipeTitle(IXMLDOMDocument *pXMLDoc, PWSTR *ppszResult);

    HRESULT GetRecipeComments(IXMLDOMDocument *pXMLDoc, PWSTR *ppszResult);

    HRESULT GetRecipeImage(
        IXMLDOMDocument *pXMLDoc, 
        HBITMAP *phbmp, 
        WTS_ALPHATYPE *pdwAlpha);

    HRESULT CreatePreviewWindow(
        PWSTR pszTitle, 
        PWSTR pszComments, 
        HBITMAP hImage);

    HRESULT GetBase64EncodedImageString(
        IXMLDOMDocument *pXMLDoc, 
        PWSTR *ppszResult);

    HRESULT GetStreamFromString(PCWSTR pszImageString, IStream **ppStream);

    HRESULT ConvertBitmapSourceTo32bppHBITMAP(
        IWICBitmapSource *pBitmapSource, 
        IWICImagingFactory *pImagingFactory, 
        HBITMAP *phbmp);

    HRESULT WICCreate32bppHBITMAP(
        IStream *pstm, 
        HBITMAP *phbmp, 
        WTS_ALPHATYPE *pdwAlpha);
};