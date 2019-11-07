/****************************** Module Header ******************************\
* Module Name:	MFCTaskDialog.cpp
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
#include <afxtaskdialog.h>
#include "MFCTaskDialog.h"
#include "MFCTaskDialogDlg.h"
#pragma endregion

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCTaskDialogApp

BEGIN_MESSAGE_MAP(CMFCTaskDialogApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CMFCTaskDialogApp construction

CMFCTaskDialogApp::CMFCTaskDialogApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CMFCTaskDialogApp object

CMFCTaskDialogApp theApp;


// CMFCTaskDialogApp initialization

BOOL CMFCTaskDialogApp::InitInstance()
{
	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	AfxEnableControlContainer();

	// Create the shell manager, in case the dialog contains
	// any shell tree view or shell list view controls.
	CShellManager *pShellManager = new CShellManager;

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("Local AppWizard-Generated Applications"));

	// Determine at runtime whether the current system can display the task 
	// dialog box. The CTaskDialog is available starting with Windows Vista. 
	// Earlier versions of Windows cannot display the CTaskDialog object.
	if (!CTaskDialog::IsSupported())
	{
		MessageBox(NULL, _T("CTaskDialog is not supported in this operating system."), 
			_T(""), MB_OK);
		return FALSE; // Exit directly
	}

	CMFCTaskDialogDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with OK
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with Cancel
	}

	// Delete the shell manager created above.
	if (pShellManager != NULL)
	{
		delete pShellManager;
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
