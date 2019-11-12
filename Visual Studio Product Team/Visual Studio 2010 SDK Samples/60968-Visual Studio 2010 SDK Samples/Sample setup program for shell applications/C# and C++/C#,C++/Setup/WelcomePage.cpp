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

// WelcomePage.cpp : implementation file
//

#include "stdafx.h"
#include "Setup.h"
#include "WelcomePage.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// WelcomePage dialog

WelcomePage::WelcomePage(UINT pParent /*=NULL*/)
    : CPropertyPage(WelcomePage::IDD, pParent)
{
    CString boutiqueName(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString title(MAKEINTRESOURCE(IDS_TITLE));
    CString welcomeTitle(MAKEINTRESOURCE(IDS_TITLE_WELCOME));
    CString pageTitle, setupTitle;
    setupTitle.Format(title, boutiqueName);
    pageTitle.Format(welcomeTitle, setupTitle);
    
    
    m_psp.dwFlags |= PSP_USETITLE;
    m_psp.pszTitle = m_strCaption = pageTitle;
}

void WelcomePage::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_STATIC_WELCOMEMSG, _welcomeMessage);
}

BEGIN_MESSAGE_MAP(WelcomePage, CDialog)
    ON_WM_PAINT()
    ON_WM_QUERYDRAGICON()
    //}}AFX_MSG_MAP
END_MESSAGE_MAP()


// WelcomePage message handlers

BOOL WelcomePage::OnInitDialog()
{
    CPropertyPage::OnInitDialog();

    // Set the icon for this dialog.  The framework does this automatically
    //  when the application's main window is not a dialog
    SetIcon(m_hIcon, TRUE);            // Set big icon
    SetIcon(m_hIcon, FALSE);        // Set small icon
    
    return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL WelcomePage::OnSetActive() 
{
    SetWelcomeMsg();
    CPropertySheet* psheet = (CPropertySheet*) GetParent();   
    psheet->SetWizardButtons(PSWIZB_NEXT);
    
    return CPropertyPage::OnSetActive();
}


// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void WelcomePage::OnPaint()
{
    if (IsIconic())
    {
        CPaintDC dc(this); // device context for painting

        SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

        // Center icon in client rectangle
        int cxIcon = GetSystemMetrics(SM_CXICON);
        int cyIcon = GetSystemMetrics(SM_CYICON);
        CRect rect;
        GetClientRect(&rect);
        int x = (rect.Width() - cxIcon + 1) / 2;
        int y = (rect.Height() - cyIcon + 1) / 2;

        // Draw the icon
        dc.DrawIcon(x, y, m_hIcon);
    }
    else
    {
        CPropertyPage::OnPaint();
    }
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR WelcomePage::OnQueryDragIcon()
{
    return static_cast<HCURSOR>(m_hIcon);
}

void WelcomePage::SetWelcomeMsg()
{
    CString boutiqueName(MAKEINTRESOURCE(IDS_BOUTIQUE_NAME));
    CString msg(MAKEINTRESOURCE(IDS_WELCOME_MSG));
    CString value;
    CString setupName;
    
    value.Format(msg, boutiqueName);
    _welcomeMessage.SetWindowTextW(value);
}