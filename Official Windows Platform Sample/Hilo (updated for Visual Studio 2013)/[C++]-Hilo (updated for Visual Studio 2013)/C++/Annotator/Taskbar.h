//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

struct ThumbnailToobarButton
{
    unsigned int buttonId;
    bool enabled;
};

class Taskbar
{
public:
    // Constructor / destructor
    Taskbar(HWND hWnd);
    ~Taskbar();

    HRESULT CreateThumbnailToolbarButtons(ThumbnailToobarButton backButton, ThumbnailToobarButton nextButton);
    HRESULT EnableThumbnailToolbarButtons(ThumbnailToobarButton backButton, ThumbnailToobarButton nextButton);
    HRESULT SetThumbnailClip(RECT* rect);

private:
    // Variables
    ComPtr<ITaskbarList3> m_taskbarList;
    HWND m_hWnd;

    // Methods
    HRESULT Initialize();
    HRESULT SetThumbnailToolbarImage();
};

namespace TaskbarHelper
{
    bool AreFileTypesRegistered(__in const wchar_t* progId);
}
