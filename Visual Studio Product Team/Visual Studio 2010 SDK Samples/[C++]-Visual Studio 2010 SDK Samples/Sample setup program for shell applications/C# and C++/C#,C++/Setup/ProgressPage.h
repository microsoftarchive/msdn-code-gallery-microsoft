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
#include "afxcmn.h"
#include "afxwin.h"


// ProgressPage dialog

class ProgressPage : public CPropertyPage
{

public:
    ProgressPage(UINT pParent = NULL);   // standard constructor
    virtual ~ProgressPage();
    BOOL OnInstallFinished();
    BOOL OnRebootRequired();
    void SetCurrentActionText(const CString text);
    void SetCurrentComponentText(const CString text);
    void StepProgressBar();
    void SetProgressBarPosition(const DWORD newPosition);
    DWORD GetProgressBarPosition();

// Dialog Data
    enum { IDD = IDD_PROGRESS_PAGE };

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual BOOL OnSetActive();
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()
private:
    CProgressCtrl _progressBar;
    CStatic _currentComponentText;
    CStatic _currentActionText;
};