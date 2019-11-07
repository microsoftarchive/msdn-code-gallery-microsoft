/****************************** Module Header ******************************\
* Module Name:	MFCTaskDialogDlg.h
* Project:		MFCTaskDialog
* Copyright (c) Microsoft Corporation.
* 
* The CTaskDialog class replaces the standard Windows message box and has 
* additional functionality such as new controls to gather information from 
* the user. This class is in the MFC library in Visual Studio 2010. The 
* CTaskDialog is available starting with Windows Vista. Earlier versions of 
* Windows cannot display the CTaskDialog object. Use CTaskDialog::IsSupported 
* to determine at runtime whether the current user can display the task 
* dialog box. The standard Windows message box is still supported in Visual 
* Studio 2010.
* 
* This sample demonstrates the usages of CTaskDialog:
* 
* 1. Basic usages
* 2. A relatively complete usuage of most controls on CTaskDialog
* 3. Progress bar and marquee progress bar on CTaskDialog
* 4. MessageBox usage
* 5. Navigation usage
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 10/25/2009 10:04 AM Jialiang Ge Created
\***************************************************************************/

#pragma once


// CMFCTaskDialogDlg dialog
class CMFCTaskDialogDlg : public CDialog
{
// Construction
public:
	CMFCTaskDialogDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_MFCTASKDIALOG_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	void DisplayResult(INT_PTR nResult, int nRadioButtonId, BOOL bChecked);

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnBnClickedBasicUsage1();
	afx_msg void OnBnClickedBasicUsage2();
	afx_msg void OnBnClickedCompleteUsage();
	afx_msg void OnBnClickedProgressbarUsage();
	afx_msg void OnBnClickedMarqueeUsage();
	afx_msg void OnBnClickedMessageBoxUsage();
	afx_msg void OnBnClickedNavigationUsage();
};
