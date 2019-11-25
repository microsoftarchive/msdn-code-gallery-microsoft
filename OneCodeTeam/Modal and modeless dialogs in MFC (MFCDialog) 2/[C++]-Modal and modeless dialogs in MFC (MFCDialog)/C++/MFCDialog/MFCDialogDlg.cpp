/****************************** Module Header ******************************\
* Module Name:	MFCDialogDlg.cpp
* Project:		MFCDialog
* Copyright (c) Microsoft Corporation.
* 
* The MFCDialog example demonstrates the creation of modal and modeless 
* dialog boxes in MFC.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* *********************************************************************************/

#pragma region Includes
#include "stdafx.h"
#include "MFCDialog.h"
#include "MFCDialogDlg.h"
#include "ModalDlg.h"
#include "ModelessDlg.h"
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


// CMFCDialogDlg dialog




CMFCDialogDlg::CMFCDialogDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCDialogDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCDialogDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCDialogDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_SHOWMODAL_BN, &CMFCDialogDlg::OnBnClickedShowModalDialog)
	ON_BN_CLICKED(IDC_SHOWMODELESS_BN, &CMFCDialogDlg::OnBnClickedShowModelessDialog)
END_MESSAGE_MAP()


// CMFCDialogDlg message handlers

BOOL CMFCDialogDlg::OnInitDialog()
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

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CMFCDialogDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CMFCDialogDlg::OnPaint()
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
HCURSOR CMFCDialogDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


// Create a Modal Dialog Box.
// To create a modal dialog box, call either of the two public constructors 
// declared in CDialog. Next, call the dialog object's DoModal member 
// function to display the dialog box and manage interaction with it until 
// the user chooses OK or Cancel. This management by DoModal is what makes 
// the dialog box modal. For modal dialog boxes, DoModal loads the dialog 
// resource.

void CMFCDialogDlg::OnBnClickedShowModalDialog()
{
	CModalDlg dlgModal;
	INT_PTR nResponse = dlgModal.DoModal();
	if (nResponse == IDOK)
	{
		// Complete the command;
		MessageBox(_T("OK"), _T("ModalDialog Result"), MB_OK);
	}
	else if (nResponse == IDCANCEL)
	{
		// Cancel the command;
		MessageBox(_T("Cancel"), _T("ModalDialog Result"), MB_OK);
	}
}


// Create a Modeless Dialog Box.
// For a modeless dialog box, you must provide your own public constructor in 
// your dialog class. To create a modeless dialog box, call your public 
// constructor and then call the dialog object's Create member function to 
// load the dialog resource. You can call Create either during or after the 
// constructor call. If the dialog resource has the property WS_VISIBLE, the 
// dialog box appears immediately. If not, you must call its ShowWindow 
// member function.

void CMFCDialogDlg::OnBnClickedShowModelessDialog()
{
	CModelessDlg* pdlgModeless = new CModelessDlg(this);
	pdlgModeless->Create(IDD_MODELESSDIALOG);
	pdlgModeless->ShowWindow(SW_SHOWNORMAL);
}