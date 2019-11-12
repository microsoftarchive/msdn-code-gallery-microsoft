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
#pragma once
#include "WelcomePage.h"
#include "ComponentsPage.h"
#include "ProgressPage.h"
#include "FinishPage.h"

// SetupSheet

class SetupSheet : public CPropertySheet
{

public:
    SetupSheet(UINT nIDCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);
    SetupSheet(LPCTSTR pszCaption = NULL, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);
    virtual ~SetupSheet();
    virtual void OnClose();
    virtual BOOL OnInitDialog();
        
    ProgressPage _progressPage;    
    FinishPage _finishPage;

protected:
    DECLARE_MESSAGE_MAP()

private:
    WelcomePage _welcomePage;
    ComponentsPage _componentsPage;
};
