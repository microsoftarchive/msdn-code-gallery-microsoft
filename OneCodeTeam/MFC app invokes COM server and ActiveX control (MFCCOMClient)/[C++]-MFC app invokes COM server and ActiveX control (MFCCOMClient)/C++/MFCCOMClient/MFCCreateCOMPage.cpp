/****************************** Module Header ******************************\
* Module Name:  MFCCreateCOMPage.cpp
* Project:      MFCCOMClient
* Copyright (c) Microsoft Corporation.
* 
* With MFC, use the Visual C++ Class Wizard to generate "wrapper classes" from  
* the type libraries. These classes, as well as other MFC classes, such as 
* COleVariant, COleSafeArray, and COleException, simplify the tasks of Automation.
* This method is usually recommended over the others, and most of the Microsoft
* Knowledge Base examples use MFC.
* 
* Examples
*  http://support.microsoft.com/kb/220600
*  http://support.microsoft.com/kb/179494
*  http://support.microsoft.com/kb/307473
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
#include <WindowsX.h>

#include "MFCCOMClient.h"
#include "MFCCreateCOMPage.h"

// Include the header file created from a type library using the MFC class wizard.
#include "CATLExeSimpleObjectWrapper.h" 
#pragma endregion


// CMFCCreateCOMPage dialog

IMPLEMENT_DYNAMIC(CMFCCreateCOMPage, CDialog)

CMFCCreateCOMPage::CMFCCreateCOMPage(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCCreateCOMPage::IDD, pParent)
{

}

CMFCCreateCOMPage::~CMFCCreateCOMPage()
{
}

void CMFCCreateCOMPage::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CMFCCreateCOMPage, CDialog)
	ON_BN_CLICKED(IDC_ATLEXESTA_BUTTON, &CMFCCreateCOMPage::OnBnClickedATLExeCOMServerButton)
END_MESSAGE_MAP()


void PrintText(HWND hwndEdit, PCTSTR pszFormat, ...) 
{
	ASSERT(hwndEdit);

	va_list argList;
	va_start(argList, pszFormat);

	TCHAR szMessage[4096];
	_vstprintf_s(szMessage, _countof(szMessage), pszFormat, argList);

	// get the initial text length
	int length = ::Edit_GetTextLength(hwndEdit);
	// put the selection at the end of text
	Edit_SetSel(hwndEdit, length, length);
	// replace the selection
	Edit_ReplaceSel(hwndEdit, szMessage);

	va_end(argList);
}


/*!
 * \brief
 * CallATLExeCOMServerThreadProc - Create and access a STA out-of-process COM 
 * object using MFC. We use the class wizard to create a C++ class that wraps 
 * the COM object.
 *
 * \param pParam
 * \returns
 * The prototype of a function that serves as the thread procedure passed to 
 * AfxBeginThread.
 */
UINT CallATLExeCOMServerThreadProc(LPVOID pParam)
{
	HWND hwndOutputCtrl = (HWND)pParam;

	// Initializes the COM library on the current thread and identifies the
	// concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);


	/////////////////////////////////////////////////////////////////////////
	// Create a COM object using MFC
	// 

	COleException *e = new COleException;
	CATLExeSimpleObjectWrapper simpleObjWrapper;

	if (!simpleObjWrapper.CreateDispatch(__uuidof(SimpleObject), e))
		throw e;
	// [-or-]
	/*if (!simpleObjWrapper.CreateDispatch(_T(
		"ATLExeCOMServer.SimpleObject"), e))
		throw e;*/


	/////////////////////////////////////////////////////////////////////////
	// Consume the COM object
	// 

	try
	{
		// Set the property: FloatProperty
		PrintText(hwndOutputCtrl, _T("Set FloatProperty = %f\r\n"), 1.2f);
		simpleObjWrapper.put_FloatProperty(1.2f);

		// Get the property: FloatProperty
		PrintText(hwndOutputCtrl, _T("Get FloatProperty = %f\r\n"), 
			simpleObjWrapper.get_FloatProperty());

		// Call the method: HelloWorld, that returns a CString
		// CString automatically frees the memory of the string when the object
		// is out of the code scope.
		CString strResult = simpleObjWrapper.HelloWorld();
		PrintText(hwndOutputCtrl, _T("Call HelloWorld => %s\r\n"), strResult);

		PrintText(hwndOutputCtrl, _T(
			"The client process and thread: %ld, %ld\r\n"), 
			GetCurrentProcessId(), GetCurrentThreadId());
		// Call the method: GetProcessThreadID, that outputs two DWORDs
		long lProcessId, lThreadId;
		simpleObjWrapper.GetProcessThreadID(&lProcessId, &lThreadId);
		PrintText(hwndOutputCtrl, _T(
			"Call GetProcessThreadID => %ld, %ld\r\n"), lProcessId, lThreadId);

		PrintText(hwndOutputCtrl, _T("\r\n"));
	}
	catch (COleException *e)
	{
		_com_error err(e->m_sc);
		AfxMessageBox(err.ErrorMessage());
		e->Delete();
	}

	// Uninitialize COM for this thread
	CoUninitialize();
	
	return 0; // Thread completed successfully
}


// CMFCCreateCOMPage message handlers

void CMFCCreateCOMPage::OnBnClickedATLExeCOMServerButton()
{
	// Prepare the window handle of the output control as the parameter
	HWND hwndOutputCtrl = ::GetDlgItem(this->m_hWnd, IDC_CREATECOMOUTPUT);

	AfxBeginThread(
		CallATLExeCOMServerThreadProc,	// thread procedure
		hwndOutputCtrl,					// thread parameter. sizeof(HWND) == sizeof(PVOID),
										// so we pass HWND directly here.
		THREAD_PRIORITY_NORMAL,			// thread priority
		0,								// stack size of the thread (1MB in NT by default)
		0,								// specifies whether the thread will run immediately 
										// or will be created in a suspended state
		NULL							// the security attributes for the thread
		);
}
