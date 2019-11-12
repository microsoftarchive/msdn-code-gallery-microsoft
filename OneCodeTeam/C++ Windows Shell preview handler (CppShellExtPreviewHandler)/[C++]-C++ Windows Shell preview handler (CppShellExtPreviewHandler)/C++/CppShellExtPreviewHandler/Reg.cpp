/****************************** Module Header ******************************\
Module Name:  Reg.cpp
Project:      CppShellExtPreviewHandler
Copyright (c) Microsoft Corporation.

The file implements the reusable helper functions to register and unregister 
in-process COM components and shell preview handlers in the registry.

RegisterInprocServer - register the in-process component in the registry.
UnregisterInprocServer - unregister the in-process component in the registry.
RegisterShellExtPreviewHandler - register the preview handler.
UnregisterShellExtPreviewHandler - unregister the preview handler.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "Reg.h"
#include <strsafe.h>


#pragma region Registry Helper Functions

//
//   FUNCTION: SetHKCRRegistryKeyAndValue
//
//   PURPOSE: The function creates a HKCR registry key and sets the specified 
//   registry value.
//
//   PARAMETERS:
//   * pszSubKey - specifies the registry key under HKCR. If the key does not 
//     exist, the function will create the registry key.
//   * pszValueName - specifies the registry value to be set. If pszValueName 
//     is NULL, the function will set the default value.
//   * pszData - specifies the string data of the registry value.
//   * dwType - specifies the type of data.
//
//   RETURN VALUE: 
//   If the function succeeds, it returns S_OK. Otherwise, it returns an 
//   HRESULT error code.
// 
HRESULT SetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PCWSTR pszData, DWORD dwType = REG_SZ)
{
    HRESULT hr;
    HKEY hKey = NULL;

    // Creates the specified registry key. If the key already exists, the 
    // function opens it. 
    hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL));

    if (SUCCEEDED(hr))
    {
        if (pszData != NULL)
        {
            // Set the specified value of the key.
            DWORD cbData = lstrlen(pszData) * sizeof(*pszData);
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, pszValueName, 0, 
                dwType, reinterpret_cast<const BYTE *>(pszData), cbData));
        }

        RegCloseKey(hKey);
    }

    return hr;
}


//
//   FUNCTION: GetHKCRRegistryKeyAndValue
//
//   PURPOSE: The function opens a HKCR registry key and gets the data for the 
//   specified registry value name.
//
//   PARAMETERS:
//   * pszSubKey - specifies the registry key under HKCR. If the key does not 
//     exist, the function returns an error.
//   * pszValueName - specifies the registry value to be retrieved. If 
//     pszValueName is NULL, the function will get the default value.
//   * pszData - a pointer to a buffer that receives the value's string data.
//   * cbData - specifies the size of the buffer in bytes.
//
//   RETURN VALUE:
//   If the function succeeds, it returns S_OK. Otherwise, it returns an 
//   HRESULT error code. For example, if the specified registry key does not 
//   exist or the data for the specified value name was not set, the function 
//   returns COR_E_FILENOTFOUND (0x80070002).
// 
HRESULT GetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PWSTR pszData, DWORD cbData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    // Try to open the specified registry key. 
    hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        KEY_READ, &hKey));

    if (SUCCEEDED(hr))
    {
        // Get the data for the specified value name.
        hr = HRESULT_FROM_WIN32(RegQueryValueEx(hKey, pszValueName, NULL, 
            NULL, reinterpret_cast<LPBYTE>(pszData), &cbData));

        RegCloseKey(hKey);
    }

    return hr;
}

#pragma endregion


//
//   FUNCTION: RegisterInprocServer
//
//   PURPOSE: Register the in-process component in the registry.
//
//   PARAMETERS:
//   * pszModule - Path of the module that contains the component
//   * clsid - Class ID of the component
//   * pszFriendlyName - Friendly name
//   * pszThreadModel - Threading model
//   * appId - AppID for Dll surrogate
//
//   NOTE: The function creates the HKCR\CLSID\{<CLSID>} key in the registry.
// 
//   HKCR
//   {
//      NoRemove CLSID
//      {
//          ForceRemove {<CLSID>} = s '<Friendly Name>'
//          {
//              val AppID = s '{<AppID>}'
//              InprocServer32 = s '%MODULE%'
//              {
//                  val ThreadingModel = s '<Thread Model>'
//              }
//          }
//      }
//      NoRemove AppID
//      {
//          ForceRemove {<AppID>}
//          {
//              val DllSurrogate = s '%SystemRoot%\system32\prevhost.exe'
//          }
//      }
//   }
//
HRESULT RegisterInprocServer(PCWSTR pszModule, const CLSID& clsid, 
    PCWSTR pszFriendlyName, PCWSTR pszThreadModel, const GUID& appId)
{
    if (pszModule == NULL || pszThreadModel == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szAppID[MAX_PATH];
    StringFromGUID2(appId, szAppID, ARRAYSIZE(szAppID));

    wchar_t szSubkey[MAX_PATH];

    // Create the HKCR\CLSID\{<CLSID>} key.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);

        // Set the AppID registry value.
        if (SUCCEEDED(hr))
        {
            hr = SetHKCRRegistryKeyAndValue(szSubkey, L"AppID", szAppID);
        }

        // Create the HKCR\CLSID\{<CLSID>}\InprocServer32 key.
        if (SUCCEEDED(hr))
        {
            hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
                L"CLSID\\%s\\InprocServer32", szCLSID);
            if (SUCCEEDED(hr))
            {
                // Set the default value of the InprocServer32 key to the 
                // path of the COM module.
                hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszModule);
                if (SUCCEEDED(hr))
                {
                    // Set the threading model of the component.
                    hr = SetHKCRRegistryKeyAndValue(szSubkey, 
                        L"ThreadingModel", pszThreadModel);
                }
            }
        }
    }

    // Create a new prevhost AppID so that this always runs in its own 
    // isolated process.
    if (SUCCEEDED(hr))
    {
        // Create the HKCR\AppID\{<AppID>} key, and set the DllSurrogate value.
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"AppID\\%s", szAppID);
        if (SUCCEEDED(hr))
        {
            hr = SetHKCRRegistryKeyAndValue(szSubkey, L"DllSurrogate", 
                L"%SystemRoot%\\system32\\prevhost.exe", REG_EXPAND_SZ);
        }
    }

    return hr;
}


//
//   FUNCTION: UnregisterInprocServer
//
//   PURPOSE: Unegister the in-process component in the registry.
//
//   PARAMETERS:
//   * clsid - Class ID of the component
//   * appId - AppID for Dll surrogate
//
//   NOTE: The function deletes the HKCR\CLSID\{<CLSID>} key and the 
//   HKCR\AppID\{<AppID>} key in the registry.
//
HRESULT UnregisterInprocServer(const CLSID& clsid, const GUID& appId)
{
    HRESULT hr = S_OK;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szAppID[MAX_PATH];
    StringFromGUID2(appId, szAppID, ARRAYSIZE(szAppID));

    wchar_t szSubkey[MAX_PATH];

    // Delete the HKCR\CLSID\{<CLSID>} key.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    // Delete the HKCR\AppID\{<AppID>} key.
    if (SUCCEEDED(hr))
    {
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"AppID\\%s", szAppID);
        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
        }
    }

    return hr;
}


//
//   FUNCTION: RegisterShellExtPreviewHandler
//
//   PURPOSE: Register the shell preview handler.
//
//   PARAMETERS:
//   * pszFileType - The file type that the context menu handler is 
//     associated with. For example, '.txt' means all .txt files. The 
//     parameter must not be NULL.
//   * clsid - Class ID of the component
//   * pszDescription - The description of the preview handler. 
//
//   NOTE: The function creates the following key in the registry.
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              {8895b1c6-b41f-4c1c-a562-0d564250836f} = s '{<CLSID>}'
//          }
//      }
//   }
//   HKCU
//   {
//      NoRemove SOFTWARE
//      {
//          NoRemove Microsoft
//          {
//              NoRemove Windows
//              {
//                  NoRemove CurrentVersion
//                  {
//                      NoRemove
//                      {
//                          PreviewHandlers
//                          {
//                              val {<CLSID>} = s '<Description>'
//                          }
//                      }
//                  }
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtPreviewHandler(PCWSTR pszFileType, 
    const CLSID& clsid, PCWSTR pszDescription)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // If pszFileType starts with '.', try to read the default value of the 
    // HKCR\<File Type> key which contains the ProgID to which the file type 
    // is linked.
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

        // If the key exists and its default value is not empty, use the 
        // ProgID as the file type.
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    // Create the registry key 
    // HKCR\<File Type>\shellex\{8895b1c6-b41f-4c1c-a562-0d564250836f}
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}", pszFileType);
    if (SUCCEEDED(hr))
    {
        // Set the default value of the key.
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, szCLSID);
    }

    // Add the preview handler to the list of all preview handlers in the 
    // HKLM or HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers
    // registry key. This list is used as an optimization by the system to 
    // enumerate all registered preview handlers for display purposes. Again, 
    // the default value is not required, it simply aids in the debugging 
    // process.
    if (SUCCEEDED(hr))
    {
        HKEY hKey = NULL;

        // Creates the registry key:
        // HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers 
        // If the key already exists, the function opens it. 
        hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CURRENT_USER, 
            L"Software\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers", 
            0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL));
        if (SUCCEEDED(hr))
        {
            // Set the registry value {<CLSID>} in the key.
            DWORD cbData = (pszDescription == NULL) ? 0 : 
                (lstrlen(pszDescription) * sizeof(*pszDescription));
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, szCLSID, 0, REG_SZ, 
                reinterpret_cast<const BYTE *>(pszDescription), cbData));

            RegCloseKey(hKey);
        }
    }

    return hr;
}


//
//   FUNCTION: UnregisterShellExtPreviewHandler
//
//   PURPOSE: Unregister the shell preview handler.
//
//   PARAMETERS:
//   * pszFileType - The file type that the context menu handler is 
//     associated with. For example, '.txt' means all .txt files. The 
//     parameter must not be NULL.
//   * clsid - Class ID of the component
//
//   NOTE: The function removes the registry key
//   HKCR\<File Type>\shellex\{8895b1c6-b41f-4c1c-a562-0d564250836f},
//   and remove the {<CLSID>} value from the preview handler list:
//   HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers
//
HRESULT UnregisterShellExtPreviewHandler(PCWSTR pszFileType, 
    const CLSID& clsid)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // If pszFileType starts with '.', try to read the default value of the 
    // HKCR\<File Type> key which contains the ProgID to which the file type 
    // is linked.
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

        // If the key exists and its default value is not empty, use the 
        // ProgID as the file type.
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    // Remove the registry key: 
    // HKCR\<File Type>\shellex\{8895b1c6-b41f-4c1c-a562-0d564250836f}
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}", pszFileType);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    // Remove the preview handler from the list of all preview handlers in the 
    // HKLM or HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers 
    // registry key.
    if (SUCCEEDED(hr))
    {
        HKEY hKey = NULL;

        // Open the registry key:
        // HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers 
        hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CURRENT_USER, 
            L"Software\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers", 
            0, KEY_WRITE, &hKey));
        if (SUCCEEDED(hr))
        {
            // Remove the {<CLSID>} registry value. 
            hr = HRESULT_FROM_WIN32(RegDeleteValue(hKey, szCLSID));
            if (hr == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
            {
                hr = S_OK;
            }

            RegCloseKey(hKey);
        }
        else if (hr == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
        {
            hr = S_OK;
        }
    }

    return hr;
}