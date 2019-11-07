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
#include "stdafx.h"
#include <msi.h>

#define IMMEDIDATE_REBOOT_REQUIRED 3015L

const int ACCESS_READ  = 1;
const int ACCESS_WRITE = 2;

HRESULT AddSetupToStartup();
DWORD ExecCmd(const CString cmd, const BOOL setCurrentDirectory);
BOOL IsUserAdministrator();
BOOL RebootMachine();
BOOL InitiateReboot();
BOOL GetInstallStatusFromReg(CString key,CString value);
CString GetProductDirFromReg();
CString GetSetupPath();
BOOL ShellIsoInstallRequired();
BOOL ShellIntInstallRequired();
BOOL BoutiqueInstallRequired();
UINT Install(LPVOID pParam);
DWORD InstallIsoShellSfx();
DWORD InstallIntShellSfx();
DWORD InstallShellBoutique();
int CALLBACK SetupUIHandler(void* pvContext, UINT iMessageType, MSIHANDLE hrecMsg);
void SetTSInInstallMode();
BOOL IsTerminalServicesEnabled();
BOOL ValidateProductSuite(LPSTR lpszSuiteToValidate);