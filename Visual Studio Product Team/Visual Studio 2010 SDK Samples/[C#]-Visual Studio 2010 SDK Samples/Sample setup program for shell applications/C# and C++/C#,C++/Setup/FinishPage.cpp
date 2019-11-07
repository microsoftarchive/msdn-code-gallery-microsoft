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
// FinishPage.cpp : implementation file
//

#include "stdafx.h"
#include "Setup.h"
#include "FinishPage.h"


// FinishPage dialog

FinishPage::FinishPage(UINT pParent /*=NULL*/)
    : CPropertyPage(FinishPage::IDD, pParent)
{
    CString boutiqueName(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString title(MAKEINTRESOURCE(IDS_TITLE));
    CString finishTitle(MAKEINTRESOURCE(IDS_TITLE_FINISH));
    CString pageTitle, setupTitle;
    setupTitle.Format(title, boutiqueName);
    pageTitle.Format(finishTitle, setupTitle);
    
    m_psp.dwFlags |= PSP_USETITLE;
    m_psp.pszTitle = m_strCaption = pageTitle;
}

FinishPage::~FinishPage()
{
}

void FinishPage::DoDataExchange(CDataExchange* pDX)
{
    CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_STATIC_MSG, _finishMessage);
}

BOOL FinishPage::OnInitDialog()
{
    CPropertyPage::OnInitDialog();

    return TRUE;
}

BOOL FinishPage::OnSetActive() 
{
   CPropertySheet* psheet = (CPropertySheet*) GetParent();   
   psheet->SetWizardButtons(PSWIZB_FINISH);

   if (setupApp._notAdmin)
   {
       setupApp.InstallFinished(IDS_FINISH_NOTADMIN);
   }
   else if (setupApp._boutiqueInstalled)
   {
           CString error;
        CString name(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
        CString alreadyInstalled(MAKEINTRESOURCE(IDS_ALREADY_INSTALLED));
        error.Format(alreadyInstalled, name);
        setupApp.GetFinishPage().SetFinishMessage(error);
   }

   return CPropertyPage::OnSetActive();
}

void FinishPage::SetFinishMessage(const CString text)
{
    _finishMessage.SetWindowTextW(text);
}

BEGIN_MESSAGE_MAP(FinishPage, CDialog)
END_MESSAGE_MAP()

// FinishPage message handlers
