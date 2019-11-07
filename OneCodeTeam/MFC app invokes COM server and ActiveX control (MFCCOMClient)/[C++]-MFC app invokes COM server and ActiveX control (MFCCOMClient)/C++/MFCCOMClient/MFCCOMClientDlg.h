/****************************** Module Header ******************************\
* Module Name:  MFCCOMClientDlg.h
* Project:      MFCCOMClient
* Copyright (c) Microsoft Corporation.
* 
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


// MFCCOMClientDlg.h : header file
//

#pragma once
#include "afxcmn.h"
#include "mfccreatecompage.h"
#include "mfcactivexctrlpage.h"


// CMFCCOMClientDlg dialog
class CMFCCOMClientDlg : public CDialog
{
// Construction
public:
	CMFCCOMClientDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_MFCCOMCLIENT_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	// The tab control in the dialog
	CTabCtrl m_tabCtrl;
	// The page that demonstrates the creation of a COM object using MFC
	CMFCCreateCOMPage m_mfcCreateCOMPage;
	// The page that demonstrates the load of a MFC ActiveX control
	CMFCActiveXCtrlPage m_mfcActiveXCtrlPage;
	afx_msg void OnTcnSelchangeTabs(NMHDR *pNMHDR, LRESULT *pResult);
};
