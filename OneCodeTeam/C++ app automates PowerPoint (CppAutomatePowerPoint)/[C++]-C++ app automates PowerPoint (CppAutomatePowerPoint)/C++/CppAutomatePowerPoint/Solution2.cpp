/****************************** Module Header ******************************\
* Module Name:  Solution2.cpp
* Project:      CppAutomatePowerPoint
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution2.h/cpp demontrates the use of C/C++ and the COM APIs 
* to automate PowerPoint. The raw automation is much more difficult, but it 
* is sometimes necessary to avoid the overhead with MFC, or problems with 
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
//   FUNCTION: AutomatePowerPointByCOMAPI(LPVOID)
//
//   PURPOSE: Automate Microsoft PowerPoint using C++ and the COM APIs.
//
DWORD WINAPI AutomatePowerPointByCOMAPI(LPVOID lpParam)
{
	// Initializes the COM library on the current thread and identifies 
	// the concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	// [-or-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);


	/////////////////////////////////////////////////////////////////////////
	// Create the PowerPoint.Application COM object using C++ and COM APIs.
	// 

	// Get CLSID of the server
	
	CLSID clsid;
	HRESULT hr;
	
	// Option 1. Get CLSID from ProgID using CLSIDFromProgID.
	LPCOLESTR progID = L"PowerPoint.Application";
	hr = CLSIDFromProgID(progID, &clsid);
	if (FAILED(hr))
	{
		wprintf(L"CLSIDFromProgID(\"%s\") failed w/err 0x%08lx\n", progID, hr);
		return 1;
	}
	// Option 2. Build the CLSID directly.
	/*const IID CLSID_Application = 
	{0x91493441,0x5A91,0x11CF,{0x87,0x00,0x00,0xAA,0x00,0x60,0x26,0x3B}};
	clsid = CLSID_Application;*/

	// Start the server and get the IDispatch interface

	IDispatch *pPpApp = NULL;
	hr = CoCreateInstance(		// [-or-] CoCreateInstanceEx, CoGetObject
		clsid,					// CLSID of the server
		NULL,
		CLSCTX_LOCAL_SERVER,	// PowerPoint.Application is a local server
		IID_IDispatch,			// Query the IDispatch interface
		(void **)&pPpApp);		// Output

	if (FAILED(hr))
	{
		wprintf(L"PowerPoint is not registered properly w/err 0x%08lx\n", hr);
		return 1;
	}

	_putws(L"PowerPoint.Application is started");


	/////////////////////////////////////////////////////////////////////////
	// Make PowerPoint invisible. (i.e. Application.Visible = 0)
	// 

	// By default PowerPoint is invisible, till you make it visible:
	//{
	//	VARIANT x;
	//	x.vt = VT_I4;
	//	x.lVal = 0;	// Office::MsoTriState::msoFalse
	//	hr = AutoWrap(DISPATCH_PROPERTYPUT, NULL, pPpApp, L"Visible", 1, x);
	//}


	/////////////////////////////////////////////////////////////////////////
	// Create a new Presentation. (i.e. Application.Presentations.Add)
	// 

	// Get the Presentations collection
	IDispatch *pPres = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pPpApp, L"Presentations", 0);
		pPres = result.pdispVal;
	}

	// Call Presentations.Add to create a new presentation
	IDispatch *pPre = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pPres, L"Add", 0);
		pPre = result.pdispVal;
	}

	_putws(L"A new presentation is created");


	/////////////////////////////////////////////////////////////////////////
	// Insert a new Slide and add some text to it.
	// 

	// Get the Slides collection
	IDispatch *pSlides = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pPre, L"Slides", 0);
		pSlides = result.pdispVal;
	}

	// Insert a new slide
	_putws(L"Insert a slide");

	IDispatch *pSlide = NULL;
	{
		VARIANT vtIndex;
		vtIndex.vt = VT_I4;
		vtIndex.lVal = 1;
		
		VARIANT vtLayout;
		vtLayout.vt = VT_I4;
		vtLayout.lVal = 2;	// PowerPoint::PpSlideLayout::ppLayoutText

		VARIANT result;
		VariantInit(&result);
		// If there are more than 1 parameters passed, they MUST be pass in 
		// reversed order. Otherwise, you may get the error 0x80020009.
		AutoWrap(DISPATCH_METHOD, &result, pSlides, L"Add", 2, vtLayout, vtIndex);
		pSlide = result.pdispVal;
	}

	// Add some texts to the slide
	_putws(L"Add some texts");

	IDispatch *pShapes = NULL;		// pSlide->Shapes
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pSlide, L"Shapes", 0);
		pShapes = result.pdispVal;
	}

	IDispatch *pShape = NULL;		// pShapes->Item(1)
	{
		VARIANT vtIndex;
		vtIndex.vt = VT_I4;
		vtIndex.lVal = 1;

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pShapes, L"Item", 1, vtIndex);
		pShape = result.pdispVal;
	}

	IDispatch *pTxtFrame = NULL;	// pShape->TextFrame
	{
		VARIANT result;
		VariantInit(&result);
		hr = AutoWrap(DISPATCH_PROPERTYGET, &result, pShape, L"TextFrame", 0);
		pTxtFrame = result.pdispVal;
	}

	IDispatch *pTxtRange = NULL;	// pTxtFrame->TextRange
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pTxtFrame, L"TextRange", 0);
		pTxtRange = result.pdispVal;
	}

	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"All-In-One Code Framework");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pTxtRange, L"Text", 1, x);
		VariantClear(&x);
	}


	/////////////////////////////////////////////////////////////////////////
	// Save the presentation as a pptx file and close it.
	// 

	_putws(L"Save and close the presentation");

	{
		// Make the file name

		// Get the directory of the current exe.
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory failed");
			return 1;
		}

		// Concat "Sample2.pptx" to the directory
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample2.pptx", 12);

		VARIANT vtFileName;
		vtFileName.vt = VT_BSTR;
		vtFileName.bstrVal = SysAllocString(szFileName);

		VARIANT vtFormat;
		vtFormat.vt = VT_I4;
		vtFormat.lVal = 24;	// PpSaveAsFileType::ppSaveAsOpenXMLPresentation

		VARIANT vtEmbedFont;
		vtEmbedFont.vt = VT_I4;
		vtEmbedFont.lVal = -2;	// MsoTriState::msoTriStateMixed

		// If there are more than 1 parameters passed, they MUST be pass in 
		// reversed order. Otherwise, you may get the error 0x80020009.
		AutoWrap(DISPATCH_METHOD, NULL, pPre, L"SaveAs", 3, vtEmbedFont, 
			vtFormat, vtFileName);

		VariantClear(&vtFileName);
	}

	// pPre->Close()
	AutoWrap(DISPATCH_METHOD, NULL, pPre, L"Close", 0);


	/////////////////////////////////////////////////////////////////////////
	// Quit the PowerPoint application. (i.e. Application.Quit())
	// 

	_putws(L"Quit the PowerPoint application");
	AutoWrap(DISPATCH_METHOD, NULL, pPpApp, L"Quit", 0);


	/////////////////////////////////////////////////////////////////////////
	// Release the COM objects.
	// 

	if (pTxtRange != NULL)
	{
		pTxtRange->Release();
	}
	if (pTxtFrame != NULL)
	{
		pTxtFrame->Release();
	}
	if (pShape != NULL)
	{
		pShape->Release();
	}
	if (pShapes != NULL)
	{
		pShapes->Release();
	}
	if (pSlide != NULL)
	{
		pSlide->Release();
	}
	if (pSlides != NULL)
	{
		pSlides->Release();
	}
	if (pPre != NULL)
	{
		pPre->Release();
	}
	if (pPres != NULL)
	{
		pPres->Release();
	}
	if (pPpApp != NULL)
	{
		pPpApp->Release();
	}

	// Uninitialize COM for this thread
	CoUninitialize();

	return 0;
}