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

// Setup.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "Setup.h"
#include "SetupDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CSetupApp

BEGIN_MESSAGE_MAP(CSetupApp, CWinApp)
    ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CSetupApp construction

CSetupApp::CSetupApp()
{
    _restartRequired = false;
    _cancelRequested = false;
    _cancelInitiated = false;
}

// The one and only CSetupApp object

CSetupApp setupApp;

// CSetupApp initialization

BOOL CSetupApp::InitInstance()
{
    CWinApp::InitInstance();

    // Create the shell manager, in case the dialog contains
    // any shell tree view or shell list view controls.
    CShellManager *pShellManager = new CShellManager;

    // Standard initialization
    //TODO: put your own unique string below.
    CString name(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    SetRegistryKey(name);
        
    // put ourselves in install mode if running on Terminal Server.
    SetTSInInstallMode();

    ParseCommandLine(_cmdLineInfo);    
    if (_cmdLineInfo.IsRestart())
    {
        RestartInstall();
    }
    else
    {
        ShowDialog();
    }

    // Delete the shell manager created above.
    if (pShellManager != NULL)
    {
        delete pShellManager;
    }

    if (setupApp._restartRequired)
    {        
        RebootMachine();
    }

    // Since the dialog has been closed, return FALSE so that we exit the
    //  application, rather than start the application's message pump.
    return FALSE;
}
void CSetupApp::OnInstallError(DWORD dwResult)
{
    CString message;
    message.Format(IDS_FINISH_FAIL, dwResult);
    setupApp.OnInstallError(message);
}

void CSetupApp::OnInstallError(CString message)
{
    //TODO: Write out any information or failures to a log file for later troubleshooting

    CPropertySheet* psheet = (CPropertySheet*)m_pMainWnd;        
    if (_cmdLineInfo.IsPassive())
    {
        psheet->CloseWindow();
    }
    else
    {
        psheet->SetActivePage(3);
        setupApp.GetFinishPage().SetFinishMessage(message);
    }
}

void CSetupApp::RestartInstall()
{
    TCHAR path[MAX_PATH]={0};
    GetModuleFileName(NULL, path, MAX_PATH * sizeof(TCHAR));    
    ShellExecute( NULL, _T( "open" ), path, _T("/install"), 0, SW_SHOWNORMAL );
}

void CSetupApp::ShowDialog()
{
    CString name = GetSetupTitle();
    SetupSheet sheet(name);
    _boutiqueInstalled = !BoutiqueInstallRequired();
    _notAdmin = !IsUserAdministrator();
    if (_cmdLineInfo.IsPassive() && (_boutiqueInstalled || _notAdmin))
    {
        return;
    }
    if (_boutiqueInstalled)
    {
        sheet.SetActivePage(3);        
    }
    else 
    {
        if (_notAdmin)
        {
            sheet.SetActivePage(3);            
        }
        else 
        {
            if (_cmdLineInfo.IsInstall() || _cmdLineInfo.IsPassive())
            {
                sheet.SetActivePage(2);
            }
        }
    }

    m_pMainWnd = &sheet;
    
    sheet.SetWizardMode();
    
    INT_PTR nResponse = sheet.DoModal();
    // We care about IDCANCEL since that means we need to potentially cancel a current install action
    if (nResponse == IDCANCEL)
    {
        _restartRequired = false;
        //TODO: show how send a cancel request to the sfx install..
    }
}

ProgressPage& CSetupApp::GetProgressPage()
{
    SetupSheet* psheet = (SetupSheet*)m_pMainWnd;

    return psheet->_progressPage;
}
FinishPage& CSetupApp::GetFinishPage()
{
    SetupSheet* psheet = (SetupSheet*)m_pMainWnd;

    return psheet->_finishPage;
}

void CSetupApp::SetRestartRequired()
{
    _restartRequired = true;
}

BOOL CSetupApp::IsCancelRequested()
{
    return _cancelRequested;
}

void CSetupApp::SetCancelRequested()
{
    _cancelRequested = true;
}

BOOL CSetupApp::IsCancelInitiated()
{
    return _cancelInitiated;
}

void CSetupApp::SetCancelInitiated()
{
    _cancelInitiated = true;
}

BOOL CSetupApp::IsInstalling()
{
    return _isInstalling;
}

void CSetupApp::InstallFinished(UINT message)
{
    _isInstalling = false;

    CString msg(MAKEINTRESOURCE(message));
    setupApp.GetFinishPage().SetFinishMessage(msg);
}

void CSetupApp::InstallComponents()
{
    _isInstalling = true;
    _pInstallThread = AfxBeginThread(Install, this, THREAD_PRIORITY_NORMAL,
            0, CREATE_SUSPENDED, NULL);
    _pInstallThread->ResumeThread();
}

CString CSetupApp::GetSetupTitle()
{
    CString name(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString title(MAKEINTRESOURCE(IDS_TITLE));
    CString value;
    value.Format(title, name);
    
    return value;
}
