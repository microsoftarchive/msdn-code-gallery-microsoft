//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once
#include "ComPtr.h"
#include <shobjidl.h>

class JumpList
{
public:
    JumpList(const wchar_t* appUserModeId);
    ~JumpList();

    HRESULT AddUserTask(const wchar_t* applicationPath, const wchar_t* title, const wchar_t* commandLine);

private:
    HRESULT CreateUserTask(const wchar_t* applicationPath, const wchar_t* title, const wchar_t* commandLine);

    // Variables
    ComPtr<ICustomDestinationList> m_destinationList; // A collection of destinations in the jump list
    ComPtr<IObjectArray> m_objectArray; // A collection of IShellItem and IShellLink objects to be added in the jump list
    std::wstring m_appId; // Associated application Id
};


