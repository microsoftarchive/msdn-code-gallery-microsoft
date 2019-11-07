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
// SetupSheet.cpp : implementation file
//

#include "stdafx.h"
#include "Setup.h"
#include "SetupSheet.h"


// SetupSheet

SetupSheet::SetupSheet(UINT nIDCaption, CWnd* pParentWnd, UINT iSelectPage)
    :CPropertySheet(nIDCaption, pParentWnd, iSelectPage)
{

}

SetupSheet::SetupSheet(LPCTSTR pszCaption, CWnd* pParentWnd, UINT iSelectPage)
    :CPropertySheet(pszCaption, pParentWnd, iSelectPage)
{
    AddPage(&_welcomePage);     //page 0
    AddPage(&_componentsPage);  //page 1
    AddPage(&_progressPage);    //page 2
    if (!setupApp._cmdLineInfo.IsPassive())
    {
        AddPage(&_finishPage);  //page 3
    }
}

SetupSheet::~SetupSheet()
{
}

BOOL SetupSheet::OnInitDialog()
{    
    CPropertySheet::OnInitDialog();

    //TODO: If you want to enable help for your installer, remove these lines.
    CButton* btn=(CButton*)GetDlgItem(IDHELP);
    btn->ShowWindow(SW_HIDE);
    
    return TRUE;
}

void SetupSheet::OnClose()
{
    if (setupApp.IsInstalling())
    {
        setupApp.SetCancelRequested();
    }
    else
    {
        SetupSheet::OnClose();
    }
}

BEGIN_MESSAGE_MAP(SetupSheet, CPropertySheet)
END_MESSAGE_MAP()


// SetupSheet message handlers
