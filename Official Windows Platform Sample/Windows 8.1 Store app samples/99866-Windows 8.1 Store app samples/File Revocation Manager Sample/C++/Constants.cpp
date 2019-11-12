//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::FileRevocation;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    // { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Protect a file or folder with an enterprise identity", "SDKSample.FileRevocation.S1_Protect" },
    { "Copy enterprise protection", "SDKSample.FileRevocation.S2_CopyProtection" },
    { "Get the protection status of the files and folders", "SDKSample.FileRevocation.S3_GetStatus" },
    { "Revoke an enterprise identity", "SDKSample.FileRevocation.S4_Revoke" },
    { "Cleanup the files and folders", "SDKSample.FileRevocation.S5_Cleanup" }
};

void MainPage::NotifyUserFileNotExist()
{
    NotifyUser("A file or folder used by the application does not exist.\n" +
                "Please try again after clicking the Setup Button in the Protect a file or folder with an Enterprise Identity scenario.",
                NotifyType::ErrorMessage);
}

void MainPage::HandleFileNotFoundException(Platform::COMException^ e)
{
    if (e->HResult == 0x80070002) // Catch FileNotExistException
    {
        NotifyUserFileNotExist();
    }
    else
    {
        throw e;
    }
}