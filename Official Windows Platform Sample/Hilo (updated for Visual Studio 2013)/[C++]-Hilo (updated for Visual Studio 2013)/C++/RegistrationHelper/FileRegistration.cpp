//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "FileRegistration.h"

FileRegistration::FileRegistration(
    const std::wstring& progId,
    const std::wstring& path,
    const std::wstring& friendlyName,
    const std::wstring& appUserModelID,
    int numExtensions,
    const wchar_t** extensions)
{
    m_progId = progId;
    m_appPath = path;
    m_friendlyName = friendlyName;
    m_userModelID = appUserModelID;

    for (int i = 0; i < numExtensions; i++)
    {
        m_extensionsToRegister.push_back(extensions[i]);
    }
}

FileRegistration::~FileRegistration(void)
{
}

// All ProgIDs that can handle a given file type should be listed under OpenWithProgids, even if listed
// as the default, so they can be enumerated in the Open With dialog, and so the Jump Lists can find
// the correct ProgID to use when relaunching a document with the specific application the Jump List is
// associated with.
HRESULT FileRegistration::RegisterFileAssociation()
{
    HRESULT hr = RegisterProgID();
    if (SUCCEEDED(hr))
    {
        for (unsigned int i = 0; SUCCEEDED(hr) && i < m_extensionsToRegister.size(); i++)
        {
            hr = RegisterVerbsForFileExtension(m_extensionsToRegister[i], true);
        }
        if (SUCCEEDED(hr))
        {
            // Notify that file associations have changed
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, nullptr, nullptr);
        }
    }
    return hr;
}

HRESULT FileRegistration::UnregisterFileAssociation()
{
    HRESULT hr = UnregisterProgID();
    if (SUCCEEDED(hr))
    {
        for (unsigned int i = 0; SUCCEEDED(hr) && i < m_extensionsToRegister.size(); i++)
        {
            // Unregister the file type
            hr = RegisterVerbsForFileExtension(m_extensionsToRegister[i].c_str(), false);
        }
        if (SUCCEEDED(hr))
        {
            // Notify that file associations have changed
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, nullptr, nullptr);
        }
    }
    return hr;
}

bool FileRegistration::AreFileTypesRegistered()
{
    bool result = false;
    HKEY progIdKey;
    if (SUCCEEDED(HRESULT_FROM_WIN32(RegOpenKey(HKEY_CLASSES_ROOT, m_progId.c_str(), &progIdKey))))
    {
        result = true;
        RegCloseKey(progIdKey);
    }
    return result;
}


HRESULT FileRegistration::RegisterProgID()
{
    HKEY progIdKey;
    // Create a registry key for the progId under HKEY_CLASSES_ROOT key
    HRESULT hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, m_progId.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE | KEY_CREATE_SUB_KEY , nullptr, &progIdKey, nullptr));
    // Set registry key values
    if (SUCCEEDED(hr))
    {
        hr = SetRegistryValue(progIdKey, nullptr, L"FriendlyTypeName", m_friendlyName.c_str());

        if (SUCCEEDED(hr))
        {
            hr = SetRegistryValue(progIdKey, nullptr, L"AppUserModelID", m_userModelID.c_str());
        }
        if (SUCCEEDED(hr))
        {
            hr = SetRegistryValue(progIdKey, L"DefaultIcon", nullptr, m_appPath.c_str());
        }
        if (SUCCEEDED(hr))
        {
            hr = SetRegistryValue(progIdKey, L"CurVer", nullptr, m_progId.c_str());
        }
        // Create a sub registry key under the progId key
        HKEY shellKey;
        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(RegCreateKeyEx(progIdKey, L"shell", 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE | KEY_CREATE_SUB_KEY, nullptr, &shellKey, nullptr));
            // Create the command line
            std::wstring commandLine = m_appPath + L" %1";
            // Set value for ProgID\shell\Open\Command
            hr = SetRegistryValue(shellKey, L"Open\\Command", nullptr, commandLine.c_str());
            // Set "Open" as the default verb for this ProgID.
            if (SUCCEEDED(hr))
            {
                hr = SetRegistryValue(shellKey, nullptr, nullptr, L"Open");
            }
            RegCloseKey(shellKey);
        }
        RegCloseKey(progIdKey);
    }
    return hr;
}

HRESULT FileRegistration::UnregisterProgID()
{
    // Delete the progId registry key
    long result = RegDeleteTree(HKEY_CLASSES_ROOT, m_progId.c_str());
    return (ERROR_SUCCESS == result || ERROR_FILE_NOT_FOUND == result) ? S_OK : HRESULT_FROM_WIN32(result);
}


HRESULT FileRegistration::RegisterVerbsForFileExtension(const std::wstring& fileExtension, bool toRegister)
{
    // Construct the registry key name that we want to open or create
    std::wstring registryKeyName = fileExtension + L"\\OpenWithProgids";
    HKEY openWithProgidsKey;
    // e.g. HKEY_CLASSES_ROOT\.jpeg\OpenWithProgids
    HRESULT hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, registryKeyName.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE, nullptr, &openWithProgidsKey, nullptr));
    if (SUCCEEDED(hr))
    {
        if (toRegister)
        {
            hr = HRESULT_FROM_WIN32(RegSetValueEx(openWithProgidsKey, m_progId.c_str(), 0, REG_NONE, nullptr, 0));
        }
        else
        {
            hr = HRESULT_FROM_WIN32(RegDeleteKeyValue(openWithProgidsKey, nullptr, m_progId.c_str()));
        }
        RegCloseKey(openWithProgidsKey);
    }

    return hr;
}

HRESULT FileRegistration::SetRegistryValue(
    HKEY registryKey,
    const wchar_t* keyName,
    const wchar_t* keyValue,
    const wchar_t* keyData)
{
    return HRESULT_FROM_WIN32(
        SHSetValue(
            registryKey,
            keyName,
            keyValue,
            REG_SZ,
            keyData,
            static_cast<unsigned long>((lstrlen(keyData) + 1) * sizeof(wchar_t))));
}
