/****************************** Module Header ******************************\
* Module Name:  MFCClipboardDlg.cpp
* Project:      MFCClipboard
* Copyright (c) Microsoft Corporation.
* 
* The clipboard is a set of functions and messages that enable applications to 
* transfer data. Because all applications have access to the clipboard, data 
* can be easily transferred between applications or within an application.
*
* A user typically carries out clipboard operations by choosing commands from 
* an application's Edit menu. Following is a brief description of the standard 
* clipboard commands.
* 
* Cut: Places a copy of the current selection on the clipboard and deletes the 
* selection from the document. The previous content of the clipboard is 
* destroyed.
* 
* Copy: Places a copy of the current selection on the clipboard. The document 
* remains unchanged. The previous content of the clipboard is destroyed.
* 
* Paste: Replaces the current selection with the content of the clipboard. 
* The content of the clipboard is not changed.
* 
* Delete: Deletes the current selection from the document. The content of the 
* clipboard is not changed. This command does not involve the clipboard, but 
* it should appear with the clipboard commands on the Edit menu.
* 
* The sample demostrates how to copy and paste simple text programmatically.
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 3/23/2009 11:04 AM Hongye Sun Created
\***************************************************************************/


#include "stdafx.h"
#include "MFCClipboard.h"
#include "MFCClipboardDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCClipboardDlg dialog




CMFCClipboardDlg::CMFCClipboardDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCClipboardDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

}

void CMFCClipboardDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TARGET_EDIT, m_editTarget);
	DDX_Control(pDX, IDC_SOURCE_EDIT, m_editSource);
}

BEGIN_MESSAGE_MAP(CMFCClipboardDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_COPY_BUTTON, &CMFCClipboardDlg::OnBnClickedCopyButton)
	ON_BN_CLICKED(IDC_CUT_BUTTON, &CMFCClipboardDlg::OnBnClickedCutButton)
	ON_BN_CLICKED(IDC_PASTE_BUTTON, &CMFCClipboardDlg::OnBnClickedPasteButton)
END_MESSAGE_MAP()


// CMFCClipboardDlg message handlers

BOOL CMFCClipboardDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCClipboardDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, 
			reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

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

// The system calls this function to obtain the cursor to display while the 
// user drags the minimized window.
HCURSOR CMFCClipboardDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CMFCClipboardDlg::OnBnClickedCopyButton()
{
	/////////////////////////////////////////////////////////////////////////
	// 1. Get text from edit control.
	// 

	CString strData;
	m_editSource.GetWindowTextW(strData);

	int len = strData.GetLength();

	if (len <= 0)
		return;


	/////////////////////////////////////////////////////////////////////////
	// 2. Open and empty clipboard. (OpenClipboard, EmptyClipboard)
	// 

	if (!OpenClipboard())
		return;

	EmptyClipboard(); 


	/////////////////////////////////////////////////////////////////////////
	// 3. Create global buffer. (GlobalAlloc)
	// 

	HGLOBAL hglbCopy = GlobalAlloc(GMEM_MOVEABLE, (len + 1));
	
	if (hglbCopy == NULL) 
    { 
        CloseClipboard(); 
        return; 
    }


	/////////////////////////////////////////////////////////////////////////
	// 4. Lock the buffer. (GlobalLock)
	// 

	char* lptstrCopy = (char*)GlobalLock(hglbCopy); 


	/////////////////////////////////////////////////////////////////////////
	// 5. Copy text to the buffer. (strcpy)
	// 

	strcpy(lptstrCopy, (CStringA)strData);


	/////////////////////////////////////////////////////////////////////////
	// 6. Unlock the buffer. (GlobalUnlock)
	// 

	GlobalUnlock(hglbCopy); 


	/////////////////////////////////////////////////////////////////////////
	// 7. Set buffer data to clipboard. (SetClipboardData)
	// 

	SetClipboardData(CF_TEXT, hglbCopy); 


	/////////////////////////////////////////////////////////////////////////
	// 8. Close clipboard. (CloseClipboard)
	// 

	CloseClipboard(); 	
}

void CMFCClipboardDlg::OnBnClickedCutButton()
{
	/////////////////////////////////////////////////////////////////////////
	// 1. Copy
	// 

	OnBnClickedCopyButton();


	/////////////////////////////////////////////////////////////////////////
	// 2. Clear the text.
	// 

	m_editSource.SetWindowTextW(CString(""));
}

void CMFCClipboardDlg::OnBnClickedPasteButton()
{
	/////////////////////////////////////////////////////////////////////////
	// 1. Check and open clipboard. (IsClipboardFormatAvailable, 
	// OpenClipboard)
	// 

	if (!IsClipboardFormatAvailable(CF_TEXT)) 
		return; 

	if (!OpenClipboard()) 
		return;


	/////////////////////////////////////////////////////////////////////////
	// 2. Get clipboard data. (GetClipboardData)
	// 

	HGLOBAL hglb = GetClipboardData(CF_TEXT);


	/////////////////////////////////////////////////////////////////////////
	// 3. Set the data into edit control.
	// 

	if (hglb != NULL) 
	{ 
		char* lptstr = (char*) GlobalLock(hglb); 
		if (lptstr != NULL) 
		{ 
			// Call the application-defined ReplaceSelection 
			// function to insert the text and repaint the 
			// window. 
			CString displayData = CString(lptstr);
			m_editTarget.SetWindowTextW(displayData);
			GlobalUnlock(hglb); 
		} 
	} 


	/////////////////////////////////////////////////////////////////////////
	// 4. Close clipboard. (CloseClipboard)
	// 

	CloseClipboard(); 

	return; 
}
