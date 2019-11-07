/****************************** Module Header ******************************\
Module Name:  FileDragDropExt.h
Project:      CppShellExtDragDropHandler
Copyright (c) Microsoft Corporation.

The code sample demonstrates creating a Shell drag-and-drop handler with C++. 

When a user right-clicks a Shell object to drag an object, a context menu is 
displayed when the user attempts to drop the object. A drag-and-drop handler 
is a context menu handler that can add items to this context menu.

The example drag-and-drop handler adds the menu item "Create hard link here" to 
the context menu. When you right-click a file and drag the file to a directory or 
a drive or the desktop, a context menu will be displayed with the "Create hard 
link here" menu item. By clicking the menu item, the handler will create a hard 
link for the dragged file in the dropped location. The name of the link is "Hard 
link to <source file name>". 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MP.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>
#include <shlobj.h>     // For IShellExtInit and IContextMenu


class FileDragDropExt : public IShellExtInit, public IContextMenu
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IShellExtInit
    IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID);

    // IContextMenu
    IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
    IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
    IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax);

    FileDragDropExt();

protected:
    ~FileDragDropExt();

private:
    // Reference count of component.
    long m_cRef;

    // The file that is dragged.
    wchar_t m_szSrcFile[MAX_PATH];

    // The directory where the file is dropped to.
    wchar_t m_szTargetDir[MAX_PATH];

    // The method that handles the "Create hard link here" command.
    void OnCreateHardLink(HWND hWnd);

    PWSTR m_pszMenuText;
};