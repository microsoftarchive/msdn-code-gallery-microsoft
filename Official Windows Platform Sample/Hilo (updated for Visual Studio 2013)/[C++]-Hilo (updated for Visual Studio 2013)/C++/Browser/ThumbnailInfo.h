//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "AsyncLoader\CriticalSectionLocker.h"
#include "FileTypes.h"

//
// Provides information about a given thumbnail such as corresponding IShellItem, filename, caption
//
struct ThumbnailInfo
{
public:
    ComPtr<IShellItem> shellItem;
    ShellFileType fileType;
    std::wstring title;

    ThumbnailInfo(IShellItem* shellItemPtr = nullptr) :
        shellItem(shellItemPtr),
        title(L""),
        fileType(FileTypeImage),
        m_fileName(L""),
        m_criticalSection(nullptr)
    {
        if (nullptr != shellItemPtr)
        {
            WCHAR* name;
            HRESULT hr = S_OK;
            hr = shellItemPtr->GetDisplayName(SIGDN_FILESYSPATH, &name);
            if (SUCCEEDED(hr) && (nullptr != name))
            {
                m_fileName = name;
                title = name;
                CoTaskMemFree(name);
            }

            hr = shellItemPtr->GetDisplayName(SIGDN_NORMALDISPLAY, &name);
            if (SUCCEEDED(hr) && (nullptr != name))
            {
                title = name;
                ::CoTaskMemFree(name);
            }
        }
    }

    void SetCriticalSection(CRITICAL_SECTION* criticalSection)
    {
        m_criticalSection = criticalSection;
    }

    std::wstring GetFileName()
    {
        Hilo::AsyncLoader::CriticalSectionLocker l(m_criticalSection);
        return m_fileName;
    }

    void SetFileName(std::wstring filename)
    {
        Hilo::AsyncLoader::CriticalSectionLocker l(m_criticalSection);
        m_fileName = filename;
    }

private:
    CRITICAL_SECTION* m_criticalSection;
    std::wstring m_fileName;
};

//
// Collection of Direct2D resources used in drawing thumbnails
//
struct RenderingParameters
{
    ComPtr<ID2D1HwndRenderTarget> renderTarget;
    ComPtr<IDWriteTextLayout> textLayout;
    ComPtr<IDWriteTextFormat> textFormat;
    ComPtr<ID2D1SolidColorBrush> solidBrush;
};
