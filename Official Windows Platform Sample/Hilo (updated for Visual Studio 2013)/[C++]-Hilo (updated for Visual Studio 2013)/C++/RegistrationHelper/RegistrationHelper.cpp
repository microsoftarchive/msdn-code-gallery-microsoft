//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "FileRegistration.h"
#include "resource.h"

const int MaxResourceStringLength = 100;

// Print the usage 
void PrintUsage()
{
    wchar_t usage[MaxResourceStringLength];
    ::LoadString(GetModuleHandle(nullptr), IDS_USAGE, usage, MaxResourceStringLength);
    _tprintf(usage);
}

void DisplayMessage(HRESULT result, bool requestRegistration, const wchar_t* messageBoxTitle)
{
    wchar_t message[MaxResourceStringLength];
    // By default, registration is successful
    int resourceId = IDS_SUCCESS_REGISTRATION;
    // Interpret the result to the corresponding message information
    if (result == E_ACCESSDENIED)
    {
        resourceId = IDS_NEED_ELEVATION;
    }
    else if(FAILED(result))
    {
        if (requestRegistration)
        {
            resourceId = IDS_FAIL_REGISTRATION;
        }
        else
        {
            resourceId = IDS_FAIL_UNREGISTRATION;
        }
    }
    else
    {
        if (requestRegistration)
        {
            resourceId = IDS_SUCCESS_REGISTRATION;
        }
        else
        {
            resourceId = IDS_SUCCESS_UNREGISTRATION;
        }
    }
    ::LoadString(GetModuleHandle(nullptr), resourceId, message, MaxResourceStringLength);
    MessageBox(0, message, messageBoxTitle, MB_OK);

}

int _tmain(int argc, _TCHAR* argv[])
{
    if (argc < 6)
    {
        PrintUsage();
        return -1;
    }

    // Load the title of message box
    wchar_t messageBoxTitle[MaxResourceStringLength];
    ::LoadString(GetModuleHandle(nullptr), IDS_MESSAGE_TITLE, messageBoxTitle, MaxResourceStringLength);

    // Create an instance of FileRegistration class
    FileRegistration fileRegistration(argv[2], argv[3], argv[4], argv[5], argc - 6, (const wchar_t**) &argv[6]);
    bool requestRegistration = _wcsnicmp(L"TRUE", argv[1], sizeof(L"TRUE")) == 0;
    bool areFilesRegistered = fileRegistration.AreFileTypesRegistered();

    // Handle the user request
    HRESULT hr = S_OK;
    if(areFilesRegistered)
    {
        if (requestRegistration)
        {
            // Files were registered before, and users request to register the file association.
            // No need to register again, so pop up the message and exit.
            wchar_t message[MaxResourceStringLength];
            ::LoadString(GetModuleHandle(nullptr), IDS_NO_REGISTRATION, message, MaxResourceStringLength);
            MessageBox(0, message, messageBoxTitle, MB_OK);
            return 0;
        }
        else
        {
            // Files were registered before, and users request to unregister the file association.
            hr = fileRegistration.UnregisterFileAssociation();
        }
    }
    else
    {
        if (!requestRegistration)
        {
            // Files are not registered yet, and users request to unregister the file type.
            // No need to unregister, because the files were not registered before. So pop up the message and exit.
            wchar_t message[MaxResourceStringLength];
            ::LoadString(GetModuleHandle(nullptr), IDS_NO_UNREGISTRATION, message, MaxResourceStringLength);
            MessageBox(0, message, messageBoxTitle, MB_OK);
            return 0;
        }
        else
        {
            // Files are not registered yet, users want to register the file.
            hr = fileRegistration.RegisterFileAssociation();
        }
    }

    // Display the result
    DisplayMessage(hr, requestRegistration, messageBoxTitle);
    return SUCCEEDED(hr) ? 0 : -1;
}
