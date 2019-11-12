/****************************** Module Header ******************************\
* Module Name:  Solution2.cpp
* Project:      CppAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution2.h/cpp demonstrates the use of C/C++ and the COM APIs 
* to automate Word. The raw automation is much more difficult, but it is 
* sometimes necessary to avoid the overhead with MFC, or problems with 
* #import. Basically, you work with such APIs as CoCreateInstance(), and COM 
* interfaces such as IDispatch and IUnknown.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <stdio.h>
#include <windows.h>
#include "Solution2.h"
#pragma endregion


//
//   FUNCTION: AutoWrap(int, VARIANT*, IDispatch*, LPOLESTR, int,...)
//
//   PURPOSE: Automation helper function. It simplifies most of the low-level 
//      details involved with using IDispatch directly. Feel free to use it 
//      in your own implementations. One caveat is that if you pass multiple 
//      parameters, they need to be passed in reverse-order.
//
//   PARAMETERS:
//      * autoType - Could be one of these values: DISPATCH_PROPERTYGET, 
//      DISPATCH_PROPERTYPUT, DISPATCH_PROPERTYPUTREF, DISPATCH_METHOD.
//      * pvResult - Holds the return value in a VARIANT.
//      * pDisp - The IDispatch interface.
//      * ptName - The property/method name exposed by the interface.
//      * cArgs - The count of the arguments.
//
//   RETURN VALUE: An HRESULT value indicating whether the function succeeds 
//      or not. 
//
//   EXAMPLE: 
//      AutoWrap(DISPATCH_METHOD, NULL, pDisp, L"call", 2, parm[1], parm[0]);
//
HRESULT AutoWrap(int autoType, VARIANT *pvResult, IDispatch *pDisp, 
				 LPOLESTR ptName, int cArgs...) 
{
	// Begin variable-argument list
	va_list marker;
	va_start(marker, cArgs);

	if (!pDisp) 
	{
		_putws(L"NULL IDispatch passed to AutoWrap()");
		_exit(0);
		return E_INVALIDARG;
	}

	// Variables used
	DISPPARAMS dp = { NULL, NULL, 0, 0 };
	DISPID dispidNamed = DISPID_PROPERTYPUT;
	DISPID dispID;
	HRESULT hr;

	// Get DISPID for name passed
	hr = pDisp->GetIDsOfNames(IID_NULL, &ptName, 1, LOCALE_USER_DEFAULT, &dispID);
	if (FAILED(hr))
	{
		wprintf(L"IDispatch::GetIDsOfNames(\"%s\") failed w/err 0x%08lx\n", 
			ptName, hr);
		_exit(0);
		return hr;
	}

	// Allocate memory for arguments
	VARIANT *pArgs = new VARIANT[cArgs + 1];
	// Extract arguments...
	for(int i=0; i < cArgs; i++) 
	{
		pArgs[i] = va_arg(marker, VARIANT);
	}

	// Build DISPPARAMS
	dp.cArgs = cArgs;
	dp.rgvarg = pArgs;

	// Handle special-case for property-puts
	if (autoType & DISPATCH_PROPERTYPUT)
	{
		dp.cNamedArgs = 1;
		dp.rgdispidNamedArgs = &dispidNamed;
	}

	// Make the call
	hr = pDisp->Invoke(dispID, IID_NULL, LOCALE_SYSTEM_DEFAULT,
		autoType, &dp, pvResult, NULL, NULL);
	if (FAILED(hr)) 
	{
		wprintf(L"IDispatch::Invoke(\"%s\"=%08lx) failed w/err 0x%08lx\n", 
			ptName, dispID, hr);
		_exit(0);
		return hr;
	}

	// End variable-argument section
	va_end(marker);

	delete[] pArgs;

	return hr;
}


//
//   FUNCTION: GetModuleDirectory(LPWSTR, DWORD);
//
//   PURPOSE: This is a helper function in this sample. It retrieves the 
//      fully-qualified path for the directory that contains the executable 
//      file of the current process. For example, "D:\Samples\".
//
//   PARAMETERS:
//      * pszDir - A pointer to a buffer that receives the fully-qualified 
//      path for the directory taht contains the executable file of the 
//      current process. If the length of the path is less than the size that 
//      the nSize parameter specifies, the function succeeds and the path is 
//      returned as a null-terminated string.
//      * nSize - The size of the lpFilename buffer, in characters.
//
//   RETURN VALUE: If the function succeeds, the return value is the length 
//      of the string that is copied to the buffer, in characters, not 
//      including the terminating null character. If the buffer is too small 
//      to hold the directory name, the function returns 0 and sets the last 
//      error to ERROR_INSUFFICIENT_BUFFER. If the function fails, the return 
//      value is 0 (zero). To get extended error information, call 
//      GetLastError.
//
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize);


//
//   FUNCTION: AutomateWordByCOMAPI(LPVOID)
//
//   PURPOSE: Automate Microsoft Word using C++ and the COM APIs.
//
DWORD WINAPI AutomateWordByCOMAPI(LPVOID lpParam)
{
	// Initializes the COM library on the current thread and identifies the 
	// concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	// [-or-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);


	/////////////////////////////////////////////////////////////////////////
	// Create the Word.Application COM object using C++ and the COM APIs.
	// 

	// Get CLSID of the server
	
	CLSID clsid;
	HRESULT hr;
	
	// Option 1. Get CLSID from ProgID using CLSIDFromProgID.
	LPCOLESTR progID = L"Word.Application";
	hr = CLSIDFromProgID(progID, &clsid);
	if (FAILED(hr))
	{
		wprintf(L"CLSIDFromProgID(\"%s\") failed w/err 0x%08lx\n", progID, hr);
		return 1;
	}
	// Option 2. Build the CLSID directly.
	/*const IID CLSID_Application = 
	{0x000209FF,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
	clsid = CLSID_Application;*/

	// Start the server and get the IDispatch interface

	IDispatch *pWordApp = NULL;
	hr = CoCreateInstance(		// [-or-] CoCreateInstanceEx, CoGetObject
		clsid,					// CLSID of the server
		NULL,
		CLSCTX_LOCAL_SERVER,	// Word.Application is a local server
		IID_IDispatch,			// Query the IDispatch interface
		(void **)&pWordApp);	// Output

	if (FAILED(hr))
	{
		wprintf(L"Word is not registered properly w/err 0x%08lx\n", hr);
		return 1;
	}

	_putws(L"Word.Application is started");


	/////////////////////////////////////////////////////////////////////////
	// Make Word invisible. (i.e. Application.Visible = 0)
	// 

	{
		VARIANT x;
		x.vt = VT_I4;
		x.lVal = 0;
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pWordApp, L"Visible", 1, x);
	}


	/////////////////////////////////////////////////////////////////////////
	// Create a new Document. (i.e. Application.Documents.Add)
	// 

	// Get the Documents collection
	IDispatch *pDocs = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pWordApp, L"Documents", 0);
		pDocs = result.pdispVal;
	}

	// Call Documents.Add() to get a new document
	IDispatch *pDoc = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pDocs, L"Add", 0);
		pDoc = result.pdispVal;
	}

	_putws(L"A new document is created");


	/////////////////////////////////////////////////////////////////////////
	// Insert a paragraph.
	// 

	_putws(L"Insert a paragraph");

	// pParas = pDoc->Paragraphs
	IDispatch *pParas = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pDoc, L"Paragraphs", 0);
		pParas = result.pdispVal;
	}

	// pPara = pParas->Add
	IDispatch *pPara = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pParas, L"Add", 0);
		pPara = result.pdispVal;
	}

	// pParaRng = pPara->Range
	IDispatch *pParaRng = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pPara, L"Range", 0);
		pParaRng = result.pdispVal;
	}

	// pParaRng->Text = "Heading 1"
	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = ::SysAllocString(L"Heading 1");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pParaRng, L"Text", 1, x);
		VariantClear(&x);
	}

	// pFont = pParaRng->Font
	IDispatch *pFont = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pParaRng, L"Font", 0);
		pFont = result.pdispVal;
	}

	// pFont->Bold = 1
	{
		VARIANT x;
		x.vt = VT_I4;
		x.lVal = 1;
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pFont, L"Bold", 1, x);
	}

	// pParaRng->InsertParagraphAfter();
	AutoWrap(DISPATCH_METHOD, NULL, pParaRng, L"InsertParagraphAfter", 0);


	/////////////////////////////////////////////////////////////////////////
	// Save the document as a docx file and close it.
	// 

	_putws(L"Save and close the document");

	// pDoc->SaveAs
	{
		// Make the file name

		// Get the directory of the current exe.
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory failed");
			return 1;
		}

		// Concat "Sample2.docx" to the directory.
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample2.docx", 12);

		// Convert the NULL-terminated string to BSTR.
		VARIANT vtFileName;
		vtFileName.vt = VT_BSTR;
		vtFileName.bstrVal = SysAllocString(szFileName);
		AutoWrap(DISPATCH_METHOD, NULL, pDoc, L"SaveAs", 1, vtFileName);
		VariantClear(&vtFileName);
	}

	// pDoc->Close()
	AutoWrap(DISPATCH_METHOD, NULL, pDoc, L"Close", 0);


	/////////////////////////////////////////////////////////////////////////
	// Quit the Word application. (i.e. Application.Quit())
	// 

	_putws(L"Quit the Word application");
	AutoWrap(DISPATCH_METHOD, NULL, pWordApp, L"Quit", 0);


	/////////////////////////////////////////////////////////////////////////
	// Release the COM objects.
	// 

	if (pFont != NULL)
	{
		pFont->Release();
	}
	if (pParaRng != NULL)
	{
		pParaRng->Release();
	}
	if (pPara != NULL)
	{
		pPara->Release();
	}
	if (pParas != NULL)
	{
		pParas->Release();
	}
	if (pDoc != NULL)
	{
		pDoc->Release();
	}
	if (pDocs != NULL)
	{
		pDocs->Release();
	}
	if (pWordApp != NULL)
	{
		pWordApp->Release();
	}

	// Uninitialize COM for this thread.
	CoUninitialize();

	return 0;
}