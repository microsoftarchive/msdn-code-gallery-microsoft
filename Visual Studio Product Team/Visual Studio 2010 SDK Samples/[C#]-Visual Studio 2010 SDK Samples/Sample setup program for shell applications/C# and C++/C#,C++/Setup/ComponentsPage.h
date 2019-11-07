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
#include "afxwin.h"


// ComponentsPage dialog

class ComponentsPage : public CPropertyPage
{

public:
    ComponentsPage(UINT pParent = NULL);   // standard constructor
    virtual ~ComponentsPage();

// Dialog Data
    enum { IDD = IDD_COMPONENTS_PAGE };

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual BOOL OnInitDialog();
    virtual BOOL OnSetActive();

    DECLARE_MESSAGE_MAP()
public:
    CListBox _componentsListBox;
};

//extern CSetupApp _setupApp;