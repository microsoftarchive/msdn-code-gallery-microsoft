//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include <ShlObj.h>
#include <vector>
#include "ComPtr.h"
#include "Window.h"

using namespace Hilo::WindowApiHelpers;

//
// Provides high level methods to show an open or save file dialog. The save dialog can open one or more image files.
//
class ShellFileDialog
{
public:
    ShellFileDialog();
    virtual ~ShellFileDialog();

    HRESULT SetDefaultFolder(const GUID& folderId);
    HRESULT ShowOpenDialog(IWindow *parentWindow, std::vector<ComPtr<IShellItem> >* shellItems);
    HRESULT ShowSaveDialog(IWindow *parentWindow, IShellItem* initialItem, IShellItem **shellItem);

private:
    bool IsValidExtention(std::wstring extension);
    HRESULT ProcessOpenDialogResults(IShellItemArray* results, bool* invalidFilesDetected, std::wstring* invalidFilesString, std::vector<ComPtr<IShellItem> >* shellItems);

    // Pre-defined filters...
    static const COMDLG_FILTERSPEC m_openPictureFilter[];
    static const COMDLG_FILTERSPEC m_savePictureFilter[];

    // The shell dialog object
    ComPtr<IShellItem> m_defaultFolder;
};

