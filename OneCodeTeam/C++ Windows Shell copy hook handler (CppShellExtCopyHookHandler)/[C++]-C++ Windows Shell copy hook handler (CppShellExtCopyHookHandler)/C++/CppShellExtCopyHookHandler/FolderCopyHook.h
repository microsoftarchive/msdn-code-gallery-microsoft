/****************************** Module Header ******************************\
Module Name:  FolderCopyHook.h
Project:      CppShellExtCopyHookHandler
Copyright (c) Microsoft Corporation.

The code sample demonstrates creating a Shell folder copy hook handler with 
C++. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>
#include <shlobj.h>     // For ICopyHook(W)


class FolderCopyHook : public ICopyHook
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // ICopyHook
    IFACEMETHODIMP_(UINT) CopyCallback(HWND hwnd, UINT wFunc, UINT wFlags, 
        LPCWSTR pszSrcFile, DWORD dwSrcAttribs, 
        LPCWSTR pszDestFile, DWORD dwDestAttribs);

    FolderCopyHook();

protected:
    ~FolderCopyHook();

private:
    // Reference count of component.
    long m_cRef;
};