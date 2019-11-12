/****************************** Module Header ******************************\
* Module Name:	MFCTaskDialogDlg.cpp
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

#pragma region Includes
#include "stdafx.h"
#include "MFCTaskDialog.h"
#include "MFCTaskDialogDlg.h"
#include "Resource.h"
#include <afxtaskdialog.h>
#pragma endregion


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


CMFCTaskDialogDlg::CMFCTaskDialogDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCTaskDialogDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCTaskDialogDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCTaskDialogDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BASICUSAGE1_BN, &CMFCTaskDialogDlg::OnBnClickedBasicUsage1)
	ON_BN_CLICKED(IDC_BASICUSAGE2_BN, &CMFCTaskDialogDlg::OnBnClickedBasicUsage2)
	ON_BN_CLICKED(IDC_COMPLETEUSAGE_BN, &CMFCTaskDialogDlg::OnBnClickedCompleteUsage)
	ON_BN_CLICKED(IDC_PROGRESSBARUSAGE_BN, &CMFCTaskDialogDlg::OnBnClickedProgressbarUsage)
	ON_BN_CLICKED(IDC_MARQUEEUSAGE_BN, &CMFCTaskDialogDlg::OnBnClickedMarqueeUsage)
	ON_BN_CLICKED(IDC_MESSAGEBOXUSAGE_BN, &CMFCTaskDialogDlg::OnBnClickedMessageBoxUsage)
	ON_BN_CLICKED(IDC_NAVIGATIONUSAGE_BN, &CMFCTaskDialogDlg::OnBnClickedNavigationUsage)
END_MESSAGE_MAP()


// CMFCTaskDialogDlg message handlers

BOOL CMFCTaskDialogDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCTaskDialogDlg::OnPaint()
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
HCURSOR CMFCTaskDialogDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CMFCTaskDialogDlg::DisplayResult(INT_PTR nResult, int nRadioButtonId, BOOL bChecked)
{
	CString strTmp;
	strTmp.Format(_T("%d"), nResult);
	SetDlgItemText(IDC_RESULTBUTTONID_STATIC, strTmp);

	strTmp.Format(_T("%d"), nRadioButtonId);
	SetDlgItemText(IDC_RESULTRADIOID_STATIC, strTmp);

	CheckDlgButton(IDC_RESULTVERIFICATION_CHECK, bChecked ? BST_CHECKED : BST_UNCHECKED);
}


#pragma region CTaskDialog Basic Usages

void CMFCTaskDialogDlg::OnBnClickedBasicUsage1()
{
	CString strContent = _T("This is an important message to the user.");
	CString strMainInstruction = _T("Important!\nPlease read!");
	CString strTitle = _T("Basic Usage 1");

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_ALLOW_DIALOG_CANCELLATION);
	INT_PTR nResult = dlg.DoModal();

	// [-or-]

	//INT_PTR nResult = CTaskDialog::ShowDialog(strContent, strMainInstruction, 
	//	strTitle, 0, 0, TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
	//	TDF_ENABLE_HYPERLINKS | TDF_ALLOW_DIALOG_CANCELLATION);

	DisplayResult(nResult, 0, FALSE);
}


void CMFCTaskDialogDlg::OnBnClickedBasicUsage2()
{
	CString strContent = _T("This is an important message to the user.");
	CString strMainInstruction = _T("Important!\nPlease read!");
	CString strTitle = _T("Basic Usage 2");

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		IDS_COMMAND1, IDS_COMMAND2, TDCBF_CANCEL_BUTTON);
	INT_PTR nResult = dlg.DoModal();

	// [-or-]

	//INT_PTR nResult = CTaskDialog::ShowDialog(strContent, strMainInstruction, 
	//	strTitle, IDS_COMMAND1, IDS_COMMAND2, TDCBF_CANCEL_BUTTON);
	
	DisplayResult(nResult, 0, FALSE);
}

#pragma endregion


#pragma region CTaskDialog Complete Usage

void CMFCTaskDialogDlg::OnBnClickedCompleteUsage()
{
	CString strContent = _T("This is an important message to the user of ") \
		_T("<a href=\"http://cfx.codeplex.com\">All-In-One Code Framework</a>.");
	CString strMainInstruction = _T("Important!\nPlease read!");
	CString strTitle = _T("CTaskDialog Complete Usage");
	CString strFooter = _T("Here is some supplementary information for the user.");

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON | TDCBF_CANCEL_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_USE_COMMAND_LINKS | TDF_SHOW_MARQUEE_PROGRESS_BAR, 
		strFooter);

	// Set icons
	dlg.SetFooterIcon(TD_INFORMATION_ICON);
	dlg.SetMainIcon(TD_WARNING_ICON);

	// Set the marquee progress bar
	dlg.SetProgressBarMarquee(TRUE , 2);

	// Load radio button controls
	// a) Load with code
	dlg.AddRadioButton(IDS_RADIO1, _T("First possible radio"));
	dlg.AddRadioButton(IDS_RADIO2, _T("Second possible radio"));
	// [-or-]
	// b) Load from string resource
	//dlg.LoadRadioButtons(IDS_RADIO1, IDS_RADIO2);

	// Load command button controls
	// a) Load with code
	dlg.AddCommandControl(IDS_COMMAND1, _T("First command link option"));
	dlg.AddCommandControl(IDS_COMMAND2, _T("Second command link option"));
	// [-or-]
	// b) Load from string resource
	//dlg.LoadCommandControls(IDS_COMMAND1, IDS_COMMAND2);

	// Set the expansion area
	dlg.SetExpansionArea(
		_T("Supplementary to the user\ntyped over two lines."), 
		_T("Get some additional information."), 
		_T("Hide the additional information."));

	// Set the verification checkbox
	dlg.SetVerificationCheckboxText(_T("Remember the user's settings."));
	dlg.SetVerificationCheckbox(TRUE);

	// Show the task dialog
	INT_PTR nResult = dlg.DoModal();

	DisplayResult(nResult, dlg.GetSelectedRadioButtonID(), 
		dlg.GetVerificationCheckboxState());
}

#pragma endregion


#pragma region CTaskDialog Progressbar Usage

class CMyProgressBarTaskDialog : public CTaskDialog
{
public:
	CMyProgressBarTaskDialog() : CTaskDialog(
		_T("This is an important message to the user."), 
		_T("Important!\nPlease read!"), 
		_T("Progressbar Usage"), 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_CALLBACK_TIMER | TDF_SHOW_PROGRESS_BAR)
	{
		SetProgressBarRange(0, 300);
	}

protected:
	virtual HRESULT OnTimer(long /*lTick */)
	{
		static int iCounter = 0;
		if (iCounter == 80)
		{
			SetProgressBarState(PBST_ERROR);
		}
		else if (iCounter == 190)
		{
			SetProgressBarState(PBST_PAUSED);
		}
		else if (iCounter == 260)
		{
			SetProgressBarState(PBST_NORMAL);
		}
		
		SetProgressBarPosition(iCounter);

		if (iCounter == 300)
		{
			iCounter = 0; // Reset to 0
		}
		else
		{
			iCounter += 5;
		}

		return S_OK;
	}
};

void CMFCTaskDialogDlg::OnBnClickedProgressbarUsage()
{
	CMyProgressBarTaskDialog dlg;
	INT_PTR nResult = dlg.DoModal();

	DisplayResult(nResult, dlg.GetSelectedRadioButtonID(), 
		dlg.GetVerificationCheckboxState());
}

#pragma endregion


#pragma region CTaskDialog Marquee Progressbar Usage

void CMFCTaskDialogDlg::OnBnClickedMarqueeUsage()
{
	CString strContent = _T("This is an important message to the user.");
	CString strMainInstruction = _T("Important!\nPlease read!");
	CString strTitle = _T("Marquee Progressbar Usage");

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_SHOW_MARQUEE_PROGRESS_BAR);
	dlg.SetProgressBarMarquee(TRUE , 2);

	INT_PTR nResult = dlg.DoModal();

	DisplayResult(nResult, dlg.GetSelectedRadioButtonID(), 
		dlg.GetVerificationCheckboxState());
}

#pragma endregion


#pragma region CTaskDialog MessageBox Usage

void CMFCTaskDialogDlg::OnBnClickedMessageBoxUsage()
{
	INT_PTR nResult = CTaskDialog::ShowDialog(
		_T("Do you like the MFCTaskDialog sample?"), _T(""), 
		_T("MessageBox Usage"), 0, 0, TDCBF_YES_BUTTON | TDCBF_NO_BUTTON);
	
	DisplayResult(nResult, 0, FALSE);
}

#pragma endregion


#pragma region CTaskDialog Navigation Usage

class CSecondNavigationDialog : public CTaskDialog 
{
private:
	CTaskDialog* m_pprevNavigationDialog;

public:
	CSecondNavigationDialog() : CTaskDialog(
		_T("This is the second navigation dialog."), _T("Step 2"), 
		_T("Navigation Usage"), TDCBF_CANCEL_BUTTON,
		TDF_ENABLE_HYPERLINKS | TDF_USE_COMMAND_LINKS)
	{
		AddCommandControl(101, _T("Go back!\nGo to the first navigation dialog."));
		AddCommandControl(102, _T("Choice 1"));
		AddCommandControl(103, _T("Choice 2"));
	}

	void SetPreviousDialog(CTaskDialog *pprev)
	{
		m_pprevNavigationDialog = pprev;
	}

protected:
	HRESULT OnCommandControlClick(int iButtonId)
	{
		if (iButtonId == 101)
		{
			NavigateTo(*m_pprevNavigationDialog);
			return S_FALSE;
		}
		return S_OK;
	}
};

class CFirstNavigationDialog : public CTaskDialog
{
private:
	CSecondNavigationDialog m_nextNavigationDialog;

public:
	CFirstNavigationDialog() : CTaskDialog(
		_T("This is the first navigation dialog."), _T("Step 1"), 
		_T("Navigation Usage"), TDCBF_CANCEL_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_USE_COMMAND_LINKS)
	{
		AddCommandControl(101 , _T("Go next!\nGo to the second navigation dialog."));
	}

protected:
	HRESULT OnCommandControlClick(int iButtonId)
	{
		if (iButtonId == 101)
		{
			m_nextNavigationDialog.SetPreviousDialog(this);
			NavigateTo(m_nextNavigationDialog);
			return S_FALSE;
		}
		return S_OK;
	}
};

void CMFCTaskDialogDlg::OnBnClickedNavigationUsage()
{
	CFirstNavigationDialog dlg;
	INT_PTR nResult = dlg.DoModal();

	DisplayResult(nResult, dlg.GetSelectedRadioButtonID(), 
		dlg.GetVerificationCheckboxState());
}

#pragma endregion