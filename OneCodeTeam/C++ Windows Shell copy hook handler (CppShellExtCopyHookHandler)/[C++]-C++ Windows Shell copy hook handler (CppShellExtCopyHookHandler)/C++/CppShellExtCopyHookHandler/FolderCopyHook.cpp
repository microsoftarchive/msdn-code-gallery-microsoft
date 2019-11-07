/****************************** Module Header ******************************\
Module Name:  FolderCopyHook.cpp
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

#include "FolderCopyHook.h"
#include <strsafe.h>
#include <assert.h>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")


extern long g_cDllRef;


FolderCopyHook::FolderCopyHook(void) : m_cRef(1)
{
    InterlockedIncrement(&g_cDllRef);
}

FolderCopyHook::~FolderCopyHook(void)
{
    InterlockedDecrement(&g_cDllRef);
}


#pragma region IUnknown

// Query to the interface the component supported.
IFACEMETHODIMP FolderCopyHook::QueryInterface(REFIID riid, void **ppv)
{
    static const QITAB qit[] = 
    {
        QITABENT(FolderCopyHook, ICopyHookW),
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

// Increase the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) FolderCopyHook::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

// Decrease the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) FolderCopyHook::Release()
{
    ULONG cRef = InterlockedDecrement(&m_cRef);
    if (0 == cRef)
    {
        delete this;
    }

    return cRef;
}

#pragma endregion


#pragma region ICopyHook

IFACEMETHODIMP_(UINT) FolderCopyHook::CopyCallback(HWND hwnd, UINT wFunc, 
    UINT wFlags, LPCWSTR pszSrcFile, DWORD dwSrcAttribs, LPCWSTR pszDestFile, 
    DWORD dwDestAttribs)
{
    int result = IDYES;

    // If the file name contains "Test" and it is being renamed...
    if (wcsstr(pszSrcFile, L"Test") != NULL && wFunc == FO_RENAME)
    {
        wchar_t szMessage[256];
        StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
            L"Are you sure to rename the folder %s as %s ?", 
            pszSrcFile, pszDestFile);

        result = MessageBox(hwnd, szMessage, L"CppShellExtCopyHookHandler", 
            MB_YESNOCANCEL);
    }

    assert(result == IDYES || result == IDNO || result == IDCANCEL);
    return result;
}

#pragma endregion