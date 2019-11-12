//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "ThumbnailControl.h"
#include "Window.h"

class ShareDialog
{
public:
    static void Show(Hilo::WindowApiHelpers::IWindow* parent, const std::vector<ComPtr<IThumbnail>>* images);

private:
    static const std::vector<ComPtr<IThumbnail>> *m_images;
    static const HBRUSH m_dialogBackgroundColor;
    static HWND m_dialogHandle;
    static bool m_isUploadingActive;
    static std::wstring m_flickrToken;

    ShareDialog();
    ~ShareDialog();

    static INT_PTR CALLBACK DlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);

    static int GetImageUploadCount();
    static int ShowAuthorizationNeededDialog();
    static int ShowAuthorizationCompleteDialog();
    static bool UploadImages();

    static unsigned long WINAPI ImageUploadThreadProc(void* threadData);
};