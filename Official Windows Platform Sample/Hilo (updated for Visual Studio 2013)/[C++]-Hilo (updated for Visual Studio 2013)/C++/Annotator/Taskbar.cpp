//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "Taskbar.h"
#include "Strsafe.h"
#include "resource.h"

Taskbar::Taskbar(HWND hWnd)
{
    m_hWnd = hWnd;
}


Taskbar::~Taskbar(void)
{
}

// Initialize the taskbar
HRESULT Taskbar::Initialize()
{
    HRESULT hr = S_OK;
    if (!m_taskbarList)
    {
        hr = CoCreateInstance(CLSID_TaskbarList, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&m_taskbarList));
        if (SUCCEEDED(hr))
        {
            hr = m_taskbarList->HrInit();
        }
    }
    return hr;
}
//
// Create the thumbnail tool bar for the specified top-level window
//
HRESULT Taskbar::CreateThumbnailToolbarButtons(ThumbnailToobarButton backButton, ThumbnailToobarButton nextButton)
{
    HRESULT hr = Initialize();
    // Set the icon/images for thumbnail toolbar buttons
    if (SUCCEEDED(hr))
    {
        hr = SetThumbnailToolbarImage();
    }
    if (SUCCEEDED(hr))
    {
        THUMBBUTTON buttons[2] = {};
        // First button
        buttons[0].dwMask = THB_BITMAP | THB_TOOLTIP | THB_FLAGS;
        buttons[0].dwFlags = THBF_ENABLED | THBF_DISMISSONCLICK;;
        buttons[0].iId = backButton.buttonId;
        buttons[0].iBitmap = 0;
        StringCchCopyW(buttons[0].szTip, ARRAYSIZE(buttons[0].szTip), L"Backward Button");
        // Second button
        buttons[1].dwMask = THB_BITMAP | THB_TOOLTIP | THB_FLAGS;
        buttons[1].dwFlags = THBF_ENABLED | THBF_DISMISSONCLICK;
        buttons[1].iId = nextButton.buttonId;
        buttons[1].iBitmap = 1;
        StringCchCopyW(buttons[1].szTip, ARRAYSIZE(buttons[1].szTip), L"Forward Button");
        // Set the buttons to be the thumbnail toolbar
        hr = m_taskbarList->ThumbBarAddButtons(m_hWnd, ARRAYSIZE(buttons), buttons);
    }
    return hr;
}
//
// Disable or enable the buttons of thumbnail toolbar
//
HRESULT Taskbar::EnableThumbnailToolbarButtons(ThumbnailToobarButton backButton, ThumbnailToobarButton nextButton)
{
    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        THUMBBUTTON buttons[2] = {};
        // First button
        buttons[0].dwMask = THB_BITMAP | THB_TOOLTIP | THB_FLAGS;
        if (backButton.enabled)
        {
            buttons[0].dwFlags = THBF_ENABLED | THBF_DISMISSONCLICK;
        }
        else
        {
            buttons[0].dwFlags = THBF_DISABLED;
        }
        buttons[0].iId = backButton.buttonId;
        buttons[0].iBitmap = 0;
        StringCchCopyW(buttons[0].szTip, ARRAYSIZE(buttons[0].szTip), L"Backward Button");
        // Second button
        buttons[1].dwMask = THB_BITMAP | THB_TOOLTIP | THB_FLAGS;
        if (nextButton.enabled)
        {
            buttons[1].dwFlags = THBF_ENABLED | THBF_DISMISSONCLICK;
        }
        else
        {
            buttons[1].dwFlags = THBF_DISABLED;
        }
        buttons[1].iId = nextButton.buttonId;
        buttons[1].iBitmap = 1;
        StringCchCopyW(buttons[1].szTip, ARRAYSIZE(buttons[1].szTip), L"Forward Button");
        // Update the buttons of the thumbnail toolbar
        hr = m_taskbarList->ThumbBarUpdateButtons(m_hWnd, ARRAYSIZE(buttons), buttons);
    }
    return hr;
}

//
// Set the thumbnail image on the taskbar
//
HRESULT Taskbar::SetThumbnailClip(RECT* rect)
{
    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        // Zoom the image to the thumbnail only
        hr = m_taskbarList->SetThumbnailClip(m_hWnd, rect);
    }
    return hr;
}
// 
// Set toolbar button images
//
HRESULT Taskbar::SetThumbnailToolbarImage()
{
    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        // Get the recommended width of a small icon in pixels
        int const smallIconWidth = GetSystemMetrics(SM_CXSMICON);
        // Load the bitmap based on the system's small icon width
        HIMAGELIST imageList;
        if (smallIconWidth <= 16)
        {
            imageList = ImageList_LoadImage(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDB_BITMAP_TOOLBAR_16), 16, 0, RGB(255, 0, 255), IMAGE_BITMAP, LR_CREATEDIBSECTION);
        }
        else
        {
            imageList = ImageList_LoadImage(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDB_BITMAP_TOOLBAR_24), 24, 0, RGB(255, 0, 255), IMAGE_BITMAP, LR_CREATEDIBSECTION);
        }
        // Add the tool bar buttons to the taskbar
        if (imageList)
        {
            hr = m_taskbarList->ThumbBarSetImageList(m_hWnd, imageList);
        }
        ImageList_Destroy(imageList);
    }
    return hr;
}
//
// Helper functions
//
namespace TaskbarHelper
{
    //
    // A Helper function to check if the file types are registered already
    //
    bool AreFileTypesRegistered(const wchar_t* progId)
    {
        bool result = false;
        HKEY progIdKey;
        if (SUCCEEDED(HRESULT_FROM_WIN32(RegOpenKey(HKEY_CLASSES_ROOT, progId, &progIdKey))))
        {
            result = true;
            RegCloseKey(progIdKey);
        }
        return result;
    }
}