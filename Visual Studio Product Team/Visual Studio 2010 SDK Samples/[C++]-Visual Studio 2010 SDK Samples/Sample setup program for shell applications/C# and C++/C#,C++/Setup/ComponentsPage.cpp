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
// ComponentsPage.cpp : implementation file
//

#include "stdafx.h"
#include "Setup.h"
#include "ComponentsPage.h"


// ComponentsPage dialog

ComponentsPage::ComponentsPage(UINT pParent /*=NULL*/)
    : CPropertyPage(ComponentsPage::IDD, pParent)
{
    CString boutiqueName(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString title(MAKEINTRESOURCE(IDS_TITLE));
    CString componentsTitle(MAKEINTRESOURCE(IDS_TITLE_COMPONENTS));
    CString pageTitle, setupTitle;
    setupTitle.Format(title, boutiqueName);
    pageTitle.Format(componentsTitle, setupTitle);
    
    m_psp.dwFlags |= PSP_USETITLE;
    m_psp.pszTitle = m_strCaption = pageTitle;
}

ComponentsPage::~ComponentsPage()
{
}

BOOL ComponentsPage::OnInitDialog()
{
    CPropertyPage::OnInitDialog();
    
    if (ShellIsoInstallRequired())
    {
        CString shell(MAKEINTRESOURCE(IDS_ISOSHELL_NAME));
        _componentsListBox.AddString(shell);
    }

    if (ShellIntInstallRequired())
    {
        CString shell(MAKEINTRESOURCE(IDS_INTSHELL_NAME));
        _componentsListBox.AddString(shell);
    }

    //TODO: You may want to check if  your application is already installed.
    CString boutique(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    _componentsListBox.AddString(boutique);
    
    //TODO: Add any other components here

    return TRUE;
}

void ComponentsPage::DoDataExchange(CDataExchange* pDX)
{
    CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_COMPONENTS_LIST, _componentsListBox);
}

BOOL ComponentsPage::OnSetActive() 
{
       CPropertySheet* psheet = (CPropertySheet*) GetParent();
       psheet->SetWizardButtons(PSWIZB_BACK | PSWIZB_NEXT);

       return CPropertyPage::OnSetActive();
}

BEGIN_MESSAGE_MAP(ComponentsPage, CDialog)
END_MESSAGE_MAP()


// ComponentsPage message handlers
