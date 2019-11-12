//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "JumpList.h"
#include <propkey.h>
#include <propvarutil.h>
#include <shlobj.h>

//
// A helper function to create a shell link for the specified task
//
namespace
{
    HRESULT CreateShellLink(
        const wchar_t* applicationPath,
        const wchar_t* commandLine,
        const wchar_t* title,
        IShellLink** shellLinkAddress)
    {
        ComPtr<IShellLink> shellLink;

        HRESULT hr = CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&shellLink));
        if (SUCCEEDED(hr))
        {
            // Set the path and file name of the shell link object
            hr = shellLink->SetPath(applicationPath);
        }

        // Set the command-line arguments for the shell link object
        if (SUCCEEDED(hr))
        {
            hr = shellLink->SetArguments(commandLine);
        }

        // Set the name of the shell link object
        if (SUCCEEDED(hr))
        {
            ComPtr<IPropertyStore> propertyStore;
            hr = shellLink->QueryInterface(&propertyStore);
            if (SUCCEEDED(hr))
            {
                PROPVARIANT propertyValue;
                hr = InitPropVariantFromString(title, &propertyValue);
                if (SUCCEEDED(hr))
                {
                    hr = propertyStore->SetValue(PKEY_Title, propertyValue);
                }
                if (SUCCEEDED(hr))
                {
                    hr = propertyStore->Commit();
                }
                if (SUCCEEDED(hr))
                {
                    hr = shellLink->QueryInterface(shellLinkAddress);
                }
                PropVariantClear(&propertyValue);
            }
        }
        return hr;
    }
}


// Constructor 
JumpList::JumpList(const wchar_t* appUserModeId)
{
    m_appId = appUserModeId;
}

// Destructor
JumpList::~JumpList()
{
}

//
// Customize the jump list and add a user task to Task category
//
HRESULT JumpList::AddUserTask(const wchar_t* applicationPath, const wchar_t* title, const wchar_t* commandLine)
{
    // Create a destination list
    HRESULT hr = CoCreateInstance(CLSID_DestinationList, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&m_destinationList));

    // Set application Id
    if (SUCCEEDED(hr))
    {
        hr = m_destinationList->SetAppID(m_appId.c_str());
    }

    // Begin to build a session for the jump list
    if (SUCCEEDED(hr))
    {
        UINT cMinSlots;
        hr = m_destinationList->BeginList(&cMinSlots, IID_PPV_ARGS(&m_objectArray));
    }

    // Add User tasks
    if (SUCCEEDED(hr))
    {
        hr = CreateUserTask(applicationPath, title, commandLine);
    }

    // Commit list
    if (SUCCEEDED(hr))
    {
        hr = m_destinationList->CommitList();
    }

    return hr;
}

//
// Create a task and add it to the jump list
//
HRESULT JumpList::CreateUserTask(const wchar_t* applicationPath, const wchar_t* title, const wchar_t* commandLine)
{
    ComPtr<IObjectCollection> shellObjectCollection;
    HRESULT hr = CoCreateInstance(CLSID_EnumerableObjectCollection, nullptr, CLSCTX_INPROC, IID_PPV_ARGS(&shellObjectCollection));
    if (SUCCEEDED(hr))
    {
        // Create shell link first
        ComPtr<IShellLink> shellLink;
        hr = CreateShellLink(applicationPath, title, commandLine, &shellLink);
        if (SUCCEEDED(hr))
        {
            hr = shellObjectCollection->AddObject(shellLink);
        }
        // Add the specified user task to the Task category of a jump list
        if (SUCCEEDED(hr))
        {
            ComPtr<IObjectArray> userTask;
            hr = shellObjectCollection->QueryInterface(&userTask);
            if (SUCCEEDED(hr))
            {
                hr = m_destinationList->AddUserTasks(userTask);
            }
        }
    }
    return hr;
}
