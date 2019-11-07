//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

class FileRegistration
{
public:
    FileRegistration(const std::wstring& progId, const std::wstring& path, const std::wstring& friendlyName, const std::wstring& appUserModelID, int numExtensions, const wchar_t** extensions);
    ~FileRegistration(void);

    HRESULT RegisterFileAssociation();
    bool AreFileTypesRegistered();
    HRESULT UnregisterFileAssociation();

private:
    std::wstring m_progId;
    std::wstring m_appPath;
    std::wstring m_friendlyName;
    std::wstring m_userModelID;
    std::vector<std::wstring> m_extensionsToRegister;

    // Methods
    HRESULT RegisterProgID();
    HRESULT UnregisterProgID();
    HRESULT RegisterVerbsForFileExtension(const std::wstring& fileExtension, bool toRegister);
    HRESULT SetRegistryValue(__in HKEY registryKey, __in const wchar_t* keyName, __in const wchar_t* keyValue, __in const wchar_t* keyData);
};