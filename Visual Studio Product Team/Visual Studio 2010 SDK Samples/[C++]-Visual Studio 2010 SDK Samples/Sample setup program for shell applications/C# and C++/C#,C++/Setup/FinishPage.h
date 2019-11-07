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


// FinishPage dialog

class FinishPage : public CPropertyPage
{

public:
    FinishPage(UINT pParent = NULL);   // standard constructor
    virtual ~FinishPage();
    void SetFinishMessage(const CString text);

// Dialog Data
    enum { IDD = IDD_FINISH_PAGE };

protected:
    virtual BOOL OnInitDialog();
    virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual BOOL OnSetActive();

    DECLARE_MESSAGE_MAP()
public:
    CStatic _finishMessage;
};
