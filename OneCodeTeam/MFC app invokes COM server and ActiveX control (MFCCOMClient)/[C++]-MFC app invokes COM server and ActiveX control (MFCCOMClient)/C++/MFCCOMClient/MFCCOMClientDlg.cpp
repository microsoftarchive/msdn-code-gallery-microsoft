/****************************** Module Header ******************************\
* Module Name:  MFCCOMClientDlg.cpp
* Project:      MFCCOMClient
* Copyright (c) Microsoft Corporation.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "stdafx.h"
#include "MFCCOMClient.h"
#include "MFCCOMClientDlg.h"
#pragma endregion


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CMFCCOMClientDlg dialog




CMFCCOMClientDlg::CMFCCOMClientDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCCOMClientDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCCOMClientDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TABS, m_tabCtrl);
}

BEGIN_MESSAGE_MAP(CMFCCOMClientDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_NOTIFY(TCN_SELCHANGE, IDC_TABS, &CMFCCOMClientDlg::OnTcnSelchangeTabs)
END_MESSAGE_MAP()


// CMFCCOMClientDlg message handlers

BOOL CMFCCOMClientDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here

#pragma region Setup the tab control

	// Insert the tabs
	m_tabCtrl.InsertItem(0, _T("MFC Create COM Object"));
	m_tabCtrl.InsertItem(1, _T("MFC ActiveX Control"));

	// Create each property page
	m_mfcCreateCOMPage.Create(IDD_CREATECOM_PAGE, &m_tabCtrl);
	m_mfcActiveXCtrlPage.Create(IDD_ACTIVEXCTRL_PAGE, &m_tabCtrl);

	// Set the location and size of the property pages in the tab control
	CRect tabRect; 
	m_tabCtrl.GetClientRect(&tabRect); 
	tabRect.top		+= 21;
	tabRect.bottom	-= 1;
	tabRect.left	+= 1;
	tabRect.right	-= 1;
	m_mfcCreateCOMPage.MoveWindow(&tabRect); 
	m_mfcActiveXCtrlPage.MoveWindow(&tabRect); 
	m_mfcCreateCOMPage.ShowWindow(TRUE);

	m_tabCtrl.SetCurSel(0);

#pragma endregion

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CMFCCOMClientDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCCOMClientDlg::OnPaint()
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
		CDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMFCCOMClientDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CMFCCOMClientDlg::OnTcnSelchangeTabs(NMHDR *pNMHDR, LRESULT *pResult)
{
	int curSel;
	curSel = m_tabCtrl.GetCurSel();  
	switch (curSel)
	{
	case 0:
		this->m_mfcCreateCOMPage.ShowWindow(TRUE);
		this->m_mfcActiveXCtrlPage.ShowWindow(FALSE);
		break;
	case 1:
		this->m_mfcCreateCOMPage.ShowWindow(FALSE);
		this->m_mfcActiveXCtrlPage.ShowWindow(TRUE);
	default: ;
	};

	*pResult = 0;
}
