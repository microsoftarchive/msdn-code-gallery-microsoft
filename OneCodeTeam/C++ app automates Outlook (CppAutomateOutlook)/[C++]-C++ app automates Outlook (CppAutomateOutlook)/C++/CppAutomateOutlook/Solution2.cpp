/****************************** Module Header ******************************\
* Module Name:  Solution2.cpp
* Project:      CppAutomateOutlook
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution2.h/cpp demontrates the use of C/C++ and the COM APIs 
* to automate Outlook. The raw automation is much more difficult, but it is 
* sometimes necessary to avoid the overhead with MFC, or problems with 
* #import. Basically, you work with such APIs as CoCreateInstance(), and COM 
* interfaces such as IDispatch and IUnknown.
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


DWORD WINAPI AutomateOutlookByCOMAPI(LPVOID lpParam)
{
	// Initializes the COM library on the current thread and identifies 
	// the concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	// [-or-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	// Define vtMissing for optional parameters in some calls.
	VARIANT vtMissing;
	vtMissing.vt = VT_EMPTY;


	/////////////////////////////////////////////////////////////////////////
	// Start Microsoft Outlook and log on with your profile.
	// 

	// Get CLSID of the server

	CLSID clsid;
	HRESULT hr;

	// Option 1. Get CLSID from ProgID using CLSIDFromProgID.
	LPCOLESTR progID = L"Outlook.Application";
	hr = CLSIDFromProgID(progID, &clsid);
	if (FAILED(hr))
	{
		wprintf(L"CLSIDFromProgID(\"%s\") failed w/err 0x%08lx\n", progID, hr);
		return 1;
	}
	// Option 2. Build the CLSID directly.
	/*const IID CLSID_Application = 
	{0x0006F03A,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
	clsid = CLSID_Application;*/

	// Start the server and get the IDispatch interface

	IDispatch *pOutlookApp = NULL;
	hr = CoCreateInstance(		// [-or-] CoCreateInstanceEx, CoGetObject
		clsid,					// CLSID of the server
		NULL,
		CLSCTX_LOCAL_SERVER,	// Outlook.Application is a local server
		IID_IDispatch,			// Query the IDispatch interface
		(void **)&pOutlookApp);	// Output

	if (FAILED(hr))
	{
		wprintf(L"Outlook is not registered properly w/err 0x%08lx\n", hr);
		return 1;
	}

	_putws(L"Outlook.Application is started");

	_putws(L"User logs on ...");

	// Get the namespace and the logon.

	// pNS = pOutlookApp->GetNamespace("MAPI")
	IDispatch *pNS = NULL;
	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"MAPI");

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pOutlookApp, L"GetNamespace", 1, x);
		pNS = result.pdispVal;

		VariantClear(&x);
	}

	// Log on by using a dialog box to choose the profile.
	// pNS->Logon(vtMissing, vtMissing, true, true)
	{
		VARIANT vtShowDialog;
		vtShowDialog.vt = VT_BOOL;
		vtShowDialog.boolVal = VARIANT_TRUE;
		VARIANT vtNewSession;
		vtNewSession.vt = VT_BOOL;
		vtNewSession.boolVal = VARIANT_TRUE;

		AutoWrap(DISPATCH_METHOD, NULL, pNS, L"Logon", 4, vtNewSession, 
			vtShowDialog, vtMissing, vtMissing);
	}

	// Alternative logon method that uses a specific profile.
	// If you use this logon method, change the profile name to an 
	// appropriate value. The second parameter of Logon is the password (if 
	// any) associated with the profile. This parameter exists only for 
	// backwards compatibility and for security reasons, and it is not 
	// recommended for use.
	// pNS->Logon(L"YourValidProfile", vtMissing, false, true);
	//{
	//	VARIANT vtProfile;
	//	vtProfile.vt = VT_BSTR;
	//	vtProfile.bstrVal = SysAllocString(L"YourValidProfile");
	//	VARIANT vtShowDialog;
	//	vtShowDialog.vt = VT_BOOL;
	//	vtShowDialog.boolVal = VARIANT_TRUE;
	//	VARIANT vtNewSession;
	//	vtNewSession.vt = VT_BOOL;
	//	vtNewSession.boolVal = VARIANT_TRUE;

	//	AutoWrap(DISPATCH_METHOD, NULL, pNS, L"Logon", 4, vtNewSession, 
	//		vtShowDialog, vtMissing, vtProfile);

	//	VariantClear(&vtProfile);
	//}

	wprintf(L"Press ENTER to continue when Outlook is ready.");
	getwchar();


	/////////////////////////////////////////////////////////////////////////
	// Enumerate the contact items.
	// 

	_putws(L"Enumerate the contact items");

	// pCtFolder = pNS->GetDefaultFolder(Outlook::olFolderContacts);
	IDispatch *pCtFolder = NULL;
	{
		VARIANT x;
		x.vt = VT_I4;
		x.lVal = 10;  // Outlook::olFolderContacts

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pNS, L"GetDefaultFolder", 1, x);
		pCtFolder = result.pdispVal;
	}

	// pCts = pCtFolder->Items;
	IDispatch *pCts = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pCtFolder, L"Items", 0);
		pCts = result.pdispVal;
	}

	// Enumerate the contact items.

	// Get the count of items.
	long lCtsCount;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pCts, L"Count", 0);
		lCtsCount = result.lVal;
	}

	for (long i = 1; i <= lCtsCount; i++)
	{
		// Get the item at i
		// pItem = pCts->Item(i);
		IDispatch *pItem = NULL;
		{
			VARIANT x;
			x.vt = VT_I4;
			x.lVal = i;

			VARIANT result;
			VariantInit(&result);
			AutoWrap(DISPATCH_PROPERTYGET, &result, pCts, L"Item", 1, x);
			pItem = result.pdispVal;
		}

		// Attemp to QI _ContactItem
		const IID IID_ContactItem = 
		{0x00063021,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
		IDispatch *pCt = NULL;
		{
			hr = pItem->QueryInterface(IID_ContactItem, (void **)&pCt);
			if (SUCCEEDED(hr))
			{
				// pCt->Email1Address
				VARIANT vtAddress;
				VariantInit(&vtAddress);
				AutoWrap(DISPATCH_PROPERTYGET, &vtAddress, pCt, 
					L"Email1Address", 0);
				wprintf(L"%s\n", (LPCWSTR)vtAddress.bstrVal);
				VariantClear(&vtAddress);

				pCt->Release();
			}
		}

		// Attemp to QI _DistListItem
		const IID IID_DistListItem = 
		{0x00063081,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
		IDispatch *pDL = NULL;
		{
			hr = pItem->QueryInterface(IID_DistListItem, (void **)&pDL);
			if (SUCCEEDED(hr))
			{
				// pDL->DLName
				VARIANT vtDLName;
				VariantInit(&vtDLName);
				AutoWrap(DISPATCH_PROPERTYGET, &vtDLName, pDL, L"DLName", 0);
				wprintf(L"%s\n", (LPCWSTR)vtDLName.bstrVal);
				VariantClear(&vtDLName);

				pDL->Release();
			}
		}

		// Release the interface.
		pItem->Release();
	}


	/////////////////////////////////////////////////////////////////////////
	// Create and send a new mail item.
	// 

	_putws(L"Create and send a new mail item");

	// pMail = pOutlookApp->CreateItem(Outlook::olMailItem);
	IDispatch *pMail = NULL;
	{
		VARIANT x;
		x.vt = VT_I4;
		x.lVal = 0;	 // Outlook::olMailItem

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pOutlookApp, L"CreateItem", 1, x);
		pMail = result.pdispVal;
	}

	// Set the properties of the email.

	// pMail->Subject = _bstr_t(L"Feedback of All-In-One Code Framework");
	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"Feedback of All-In-One Code Framework");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pMail, L"Subject", 1, x);
		VariantClear(&x);
	}

	// pMail->To = _bstr_t(L"codefxf@microsoft.com");
	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"codefxf@microsoft.com");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pMail, L"To", 1, x);
		VariantClear(&x);
	}

	// pMail->HTMLBody = _bstr_t(L"<b>Feedback:</b><br />");
	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"<b>Feedback:</b><br />");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pMail, L"HTMLBody", 1, x);
		VariantClear(&x);
	}

	// Displays a new Inspector object for the item and allows users to click 
	// on the Send button to send the mail manually. 
	// Modal = true makes the Inspector window modal.
	// pMail->Display(true);
	{
		VARIANT vtModal;
		vtModal.vt = VT_BOOL;
		vtModal.boolVal = VARIANT_TRUE;
		AutoWrap(DISPATCH_METHOD, NULL, pMail, L"Display", 1, vtModal);
	}

	// [-or-]

	// Automatically send the mail without a new Inspector window.
	// pMail->Send();
	/*AutoWrap(DISPATCH_METHOD, NULL, pMail, L"Send", 0);*/


	/////////////////////////////////////////////////////////////////////////
	// User logs off and quits Outlook.
	// 

	_putws(L"Log off and quit the Outlook application");

	// pNS->Logoff()
	AutoWrap(DISPATCH_METHOD, NULL, pNS, L"Logoff", 0);

	// pOutlookApp->Quit()
	AutoWrap(DISPATCH_METHOD, NULL, pOutlookApp, L"Quit", 0);


	/////////////////////////////////////////////////////////////////////
	// Release the COM objects.
	// 

	if (pMail != NULL)
	{
		pMail->Release();
	}
	if (pCts != NULL)
	{
		pCts->Release();
	}
	if (pCtFolder != NULL)
	{
		pCtFolder->Release();
	}
	if (pNS != NULL)
	{
		pNS->Release();
	}
	if (pOutlookApp != NULL)
	{
		pOutlookApp->Release();
	}

	// Uninitialize COM for this thread.
	CoUninitialize();

	return 0;
}