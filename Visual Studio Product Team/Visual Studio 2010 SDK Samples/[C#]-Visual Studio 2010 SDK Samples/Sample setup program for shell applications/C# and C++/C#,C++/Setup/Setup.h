//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************

// Setup.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
    #error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"        // main symbols
#include "Utilities.h"
#include "setupsheet.h"
#include "WelcomePage.h"
#include "ComponentsPage.h"
#include "ProgressPage.h"
#include "FinishPage.h"
#include "SetupCommandLineInfo.h"

// CSetupApp:
// See Setup.cpp for the implementation of this class
//

class CSetupApp : public CWinApp
{
public:
    BOOL _notAdmin;
    BOOL _boutiqueInstalled;
    SetupCommandLineInfo _cmdLineInfo;

    CSetupApp();
    void InstallComponents();
    
    ProgressPage& GetProgressPage();
    FinishPage& GetFinishPage();
    
    void SetRestartRequired();
    BOOL IsCancelRequested();
    void SetCancelRequested();
    BOOL IsCancelInitiated();
    void SetCancelInitiated();
    BOOL IsInstalling();
    void InstallFinished(UINT message);
    
    void OnInstallError(DWORD dwResult);
    void OnInstallError(CString message);
private:
    void ShowDialog();
    void RestartInstall();
    CString GetSetupTitle();

// Overrides
public:
    virtual BOOL InitInstance();

// Implementation
    DECLARE_MESSAGE_MAP()

private:
    CWinThread* _pInstallThread;
    
    BOOL _restartRequired;
    BOOL _cancelRequested;
    BOOL _cancelInitiated;
    BOOL _isInstalling;
};

extern CSetupApp setupApp;