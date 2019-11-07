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
// ProgressPage.cpp : implementation file
//

#include "stdafx.h"
#include "Setup.h"
#include "ProgressPage.h"
#include "SetupSheet.h"


// ProgressPage dialog

ProgressPage::ProgressPage(UINT pParent /*=NULL*/)
    : CPropertyPage(ProgressPage::IDD, pParent)
{
    CString boutiqueName(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString title(MAKEINTRESOURCE(IDS_TITLE));
    CString progressTitle(MAKEINTRESOURCE(IDS_TITLE_PROGRESS));
    CString pageTitle, setupTitle;
    setupTitle.Format(title, boutiqueName);
    pageTitle.Format(progressTitle, setupTitle);
        
    m_psp.dwFlags |= PSP_USETITLE;
    m_psp.pszTitle = m_strCaption = pageTitle;
}

ProgressPage::~ProgressPage()
{
}

void ProgressPage::DoDataExchange(CDataExchange* pDX)
{
    CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_PROGRESS1, _progressBar);
    DDX_Control(pDX, IDC_STATIC_CURRCOMP, _currentComponentText);
    DDX_Control(pDX, IDC_STATIC_DETAILINFO, _currentActionText);
}

BOOL ProgressPage::OnSetActive() 
{
    CPropertySheet* psheet = (CPropertySheet*) GetParent();   
    CString actionTemplate(MAKEINTRESOURCE(IDS_PROGRESS_CURRACTION));
    CString details(MAKEINTRESOURCE(IDS_PROGRESS_INIT));
    CString currentAction;
    currentAction.Format(actionTemplate, details);
    
    SetCurrentActionText(currentAction);    
    psheet->SetWizardButtons(PSWIZB_DISABLEDFINISH);

    return CPropertyPage::OnSetActive();
}

BOOL ProgressPage::OnRebootRequired()
{
    CPropertySheet* psheet = (CPropertySheet*) GetParent();   
    setupApp.SetRestartRequired();    
    if (setupApp._cmdLineInfo.IsPassive())
    {
        psheet->CloseWindow();
    }
    else
    {
        CString restartNow(MAKEINTRESOURCE(IDS_RESTART_NOW));
            
        psheet->SetActivePage(3);
        psheet->SetFinishText(restartNow);

        setupApp.InstallFinished(IDS_PENDING_REBOOT);
    }
    return TRUE;
}

BOOL ProgressPage::OnInstallFinished()
{
    CPropertySheet* psheet = (CPropertySheet*) GetParent();       
    if (setupApp._cmdLineInfo.IsPassive())
    {
        psheet->CloseWindow();
    }
    else
    {
        psheet->SetWizardButtons(PSWIZB_FINISH);    
        psheet->SetActivePage(3);
        setupApp.InstallFinished(IDS_FINISH_SUCCESS);
    }
    
                
    return TRUE;
}

BOOL ProgressPage::OnInitDialog()
{
    CPropertyPage::OnInitDialog();

    _progressBar.SetRange(1, 100);
    _progressBar.SetStep(1);
    
    setupApp.InstallComponents();

    return TRUE;
}

void ProgressPage::SetCurrentActionText(const CString text)
{
    _currentActionText.SetWindowTextW(text);
}

void ProgressPage::SetCurrentComponentText(const CString text)
{
    _currentComponentText.SetWindowTextW(text);
}

DWORD ProgressPage::GetProgressBarPosition()
{
    return _progressBar.GetPos();
}
void ProgressPage::SetProgressBarPosition(const DWORD newPosition)
{
    _progressBar.SetPos(newPosition);
}

void ProgressPage::StepProgressBar()
{
    _progressBar.StepIt();
}

BEGIN_MESSAGE_MAP(ProgressPage, CDialog)
END_MESSAGE_MAP()


// ProgressPage message handlers
