/******************************** Module Header ********************************\
Module Name:  FileDragDropExt.cpp
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
\*******************************************************************************/

#include "FileDragDropExt.h"
#include <strsafe.h>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")

extern long g_cDllRef;


#define IDM_RUNFROMHERE            0  // The command's identifier offset

FileDragDropExt::FileDragDropExt() : m_cRef(1), 
    m_pszMenuText(L"Create hard link here")
{
    InterlockedIncrement(&g_cDllRef);
}


FileDragDropExt::~FileDragDropExt()
{
    InterlockedDecrement(&g_cDllRef);
}


void FileDragDropExt::OnCreateHardLink(HWND hWnd)
{
    // Get only the file name of the dragged file, with the path portion 
    // removed. For example, "D:\test\file.txt" is reduced to "file.txt".
    wchar_t szExistingFileName[MAX_PATH];
    StringCchCopy(szExistingFileName, ARRAYSIZE(szExistingFileName), m_szSrcFile);
    PathStripPath(szExistingFileName);

    // Make the new link name.
    wchar_t szNewLinkName[MAX_PATH];
    HRESULT hr = StringCchPrintf(szNewLinkName, ARRAYSIZE(szNewLinkName), 
        L"%s\\Hard link to %s", m_szTargetDir, szExistingFileName);
    if (FAILED(hr))
    {
        MessageBox(hWnd, L"The new link name is too long. The operation failed.", 
            L"CppShellExtDragDropHandler", MB_ICONERROR);
        return;
    }

    // Check if a file with this name exists.
    if (PathFileExists(szNewLinkName))
    {
        MessageBox(hWnd, L"There is already a file with the same name in this location.", 
            L"CppShellExtDragDropHandler", MB_ICONERROR);
        return;
    }

    // Establish a hard link between an existing file and a new file.
    if (!CreateHardLink(szNewLinkName, m_szSrcFile, NULL))
    {
        wchar_t szMessage[260];
        DWORD dwError = GetLastError();
        
        if (dwError == ERROR_NOT_SAME_DEVICE)
        {
            StringCchCopy(szMessage, ARRAYSIZE(szMessage), 
                L"The hard link cannot be established because all hard links to " \
                L"a file must be on the same volume");
        }
        else
        {
            StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
                L"The hard link cannot be established w/err 0x%08lx", dwError);
        }
        
        MessageBox(hWnd, szMessage, L"CppShellExtDragDropHandler", MB_ICONERROR);
    }
}


#pragma region IUnknown

// Query to the interface the component supported.
IFACEMETHODIMP FileDragDropExt::QueryInterface(REFIID riid, void **ppv)
{
    static const QITAB qit[] = 
    {
        QITABENT(FileDragDropExt, IContextMenu),
        QITABENT(FileDragDropExt, IShellExtInit), 
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

// Increase the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) FileDragDropExt::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

// Decrease the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) FileDragDropExt::Release()
{
    ULONG cRef = InterlockedDecrement(&m_cRef);
    if (0 == cRef)
    {
        delete this;
    }

    return cRef;
}

#pragma endregion


#pragma region IShellExtInit

// Initialize the drag and drop handler.
IFACEMETHODIMP FileDragDropExt::Initialize(
    LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID)
{
    // Get the directory where the file is dropped to.
    if (!SHGetPathFromIDList(pidlFolder, this->m_szTargetDir))
    {
		return E_FAIL;
    }

    // Get the file(s) being dragged.
    if (NULL == pDataObj)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = E_FAIL;

    FORMATETC fe = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
    STGMEDIUM stm;

    // The pDataObj pointer contains the objects being acted upon. In this 
    // example, we get an HDROP handle for enumerating the dragged files and 
    // folders.
    if (SUCCEEDED(pDataObj->GetData(&fe, &stm)))
    {
        // Get an HDROP handle.
        HDROP hDrop = static_cast<HDROP>(GlobalLock(stm.hGlobal));
        if (hDrop != NULL)
        {
            // Determine how many files are involved in this operation. This 
            // code sample displays the menu item when only one file (not 
            // directory) is dragged.
            UINT nFiles = DragQueryFile(hDrop, 0xFFFFFFFF, NULL, 0);
            if (nFiles == 1)
            {
                // Get the path of the file.
                if (0 != DragQueryFile(hDrop, 0, m_szSrcFile, ARRAYSIZE(m_szSrcFile)))
                {
                    // The path must not be a directory because hard link is only 
                    // for files, not directories.
                    if (!PathIsDirectory(m_szSrcFile))
                    {
                        hr = S_OK;
                    }
                }
            }

            GlobalUnlock(stm.hGlobal);
        }

        ReleaseStgMedium(&stm);
    }

    // If any value other than S_OK is returned from the method, the menu 
    // item is not displayed.
    return hr;
}

#pragma endregion


#pragma region IContextMenu

//
//   FUNCTION: FileDragDropExt::QueryContextMenu
//
//   PURPOSE: The Shell calls IContextMenu::QueryContextMenu to allow the 
//            context menu handler to add its menu items to the menu. It 
//            passes in the HMENU handle in the hmenu parameter. The 
//            indexMenu parameter is set to the index to be used for the 
//            first menu item that is to be added.
//
IFACEMETHODIMP FileDragDropExt::QueryContextMenu(
    HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags)
{
    // If uFlags include CMF_DEFAULTONLY then we should not do anything.
    if (CMF_DEFAULTONLY & uFlags)
    {
        return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(0));
    }

    // Use either InsertMenu or InsertMenuItem to add menu items.

    MENUITEMINFO mii = { sizeof(mii) };
    mii.fMask = MIIM_ID | MIIM_TYPE | MIIM_STATE;
    mii.wID = idCmdFirst + IDM_RUNFROMHERE;
    mii.fType = MFT_STRING;
    mii.dwTypeData = m_pszMenuText;
    mii.fState = MFS_ENABLED;
    if (!InsertMenuItem(hMenu, indexMenu, TRUE, &mii))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    // Return an HRESULT value with the severity set to SEVERITY_SUCCESS. 
    // Set the code value to the offset of the largest command identifier 
    // that was assigned, plus one (1).
    return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(IDM_RUNFROMHERE + 1));
}


//
//   FUNCTION: FileDragDropExt::InvokeCommand
//
//   PURPOSE: This method is called when a user clicks a menu item to tell 
//            the handler to run the associated command. The lpcmi parameter 
//            points to a structure that contains the needed information.
//
IFACEMETHODIMP FileDragDropExt::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
    // The high-word of pici->lpVerb must be NULL because we did not 
	// implement IContextMenu::GetCommandString to specify any verb for the 
	// command.
    if (NULL != HIWORD(pici->lpVerb))
	{
		return E_INVALIDARG;
	}

    // Then, the low-word of lpcmi->lpVerb should contain the command's 
	// identifier offset.
    if (LOWORD(pici->lpVerb) == IDM_RUNFROMHERE)
	{
		OnCreateHardLink(pici->hwnd);
	}
	else
	{
		// If the verb is not recognized by the drag-and-drop handler, it 
		// must return E_FAIL to allow it to be passed on to the other 
		// drag-and-drop handlers that might implement that verb.
		return E_FAIL;
	}

	return S_OK;
}


//
//   FUNCTION: FileDragDropExt::GetCommandString
//
//   PURPOSE: If a user highlights one of the items added by a context menu 
//            handler, the handler's IContextMenu::GetCommandString method is 
//            called to request a Help text string that will be displayed on 
//            the Windows Explorer status bar. This method can also be called 
//            to request the verb string that is assigned to a command. 
//            Either ANSI or Unicode verb strings can be requested. This 
//            example does not need to specify any verb for the command, so 
//            the method returns E_NOTIMPL directly. 
//
IFACEMETHODIMP FileDragDropExt::GetCommandString(UINT_PTR idCommand, 
    UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax)
{
    return E_NOTIMPL;
}

#pragma endregion