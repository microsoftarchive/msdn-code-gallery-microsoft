/****************************** Module Header ******************************\
* Module Name:  Solution1.cpp
* Project:      CppAutomateOutlook
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution1.h/cpp demonstrates the use of #import to automate 
* Outlook. #import (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx),
* a new directive that became available with Visual C++ 5.0, creates VC++ 
* "smart pointers" from a specified type library. It is very powerful, but 
* often not recommended because of reference-counting problems that typically 
* occur when used with the Microsoft Office applications. Unlike the direct 
* API approach in Solution2.h/cpp, smart pointers enable us to benefit from 
* the type info to early/late bind the object. #import takes care of adding  
* the messy guids to the project and the COM APIs are encapsulated in custom 
* classes that the #import directive generates.
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
#include "Solution1.h"
#pragma endregion


#pragma region Import the type libraries

#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52" \
	rename("RGB", "MSORGB") \
	rename("DocumentProperties", "MSODocumentProperties")
// [-or-]
//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL" \
//	rename("RGB", "MSORGB") \
//	rename("DocumentProperties", "MSODocumentProperties")

using namespace Office;

#import "progid:Outlook.Application" \
	rename("CopyFile", "OutlookCopyFile") \
	rename("PlaySound", "OutlookPlaySound")
// [-or-]
//#import "libid:00062FFF-0000-0000-C000-000000000046" \
//	rename("CopyFile", "OutlookCopyFile") \
//	rename("PlaySound", "OutlookPlaySound")
// [-or-]
//#import "C:\\Program Files\\Microsoft Office\\Office12\\MSOUTL.OLB"	\
//	rename("CopyFile", "OutlookCopyFile") \
//	rename("PlaySound", "OutlookPlaySound")

#pragma endregion


DWORD WINAPI AutomateOutlookByImport(LPVOID lpParam)
{
	// Initializes the COM library on the current thread and identifies the
	// concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	// [-or-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	try
	{

		/////////////////////////////////////////////////////////////////////
		// Start Microsoft Outlook and log on with your profile.
		// 

		HRESULT hr;

		// Option 1) Create the object using the smart pointer's constructor
		// 
		// _ApplicationPtr is the original interface name, _Application, with a 
		// "Ptr" suffix.
		//Outlook::_ApplicationPtr spOutlookApp(
		//	__uuidof(Outlook::Application)	// CLSID of the component
		//	);

		// Option 2) Create the object using the smart pointer's function,
		// CreateInstance
		Outlook::_ApplicationPtr spOutlookApp;
		hr = spOutlookApp.CreateInstance(__uuidof(Outlook::Application));
		if (FAILED(hr))
		{
			wprintf(L"CreateInstance failed w/err 0x%08lx\n", hr);
			return 1;
		}

		_putws(L"Outlook.Application is started");

		_putws(L"User logs on ...");

		// Get the namespace.
		Outlook::_NameSpacePtr spNS = spOutlookApp->GetNamespace(
			_bstr_t(L"MAPI"));

		// Log on by using a dialog box to choose the profile.
		spNS->Logon(vtMissing, vtMissing, true, true);

		// Alternative logon method that uses a specific profile.
		// If you use this logon method, change the profile name to an 
		// appropriate value. The second parameter of Logon is the password 
		// (if any) associated with the profile. This parameter exists only 
		// for backwards compatibility and for security reasons, and it is 
		// not recommended for use.
		//spNS->Logon(L"YourValidProfile", vtMissing, false, true);

		wprintf(L"Press ENTER to continue when Outlook is ready.");
		getwchar();


		/////////////////////////////////////////////////////////////////////
		// Enumerate the contact items.
		// 

		_putws(L"Enumerate the contact items");

		Outlook::MAPIFolderPtr spCtFolder = spNS->GetDefaultFolder(
			Outlook::olFolderContacts);
		Outlook::_ItemsPtr spCts = spCtFolder->Items;

		// Enumerate the contact items.
		for (long i = 1; i <= spCts->Count; i++)
		{
			IDispatchPtr spItem = spCts->Item(i);

			// Attemp to QI _ContactItem
			Outlook::_ContactItemPtr spCt;
			hr = spItem->QueryInterface(__uuidof(Outlook::_ContactItem), 
				(void **)&spCt);
			if (SUCCEEDED(hr)) // If the item is a Contact item
			{
				wprintf(L"%s\n", (LPCWSTR)spCt->Email1Address);
			}

			// Attemp to QI _DistListItem
			Outlook::_DistListItemPtr spDL;
			hr = spItem->QueryInterface(__uuidof(Outlook::_DistListItem), 
				(void **)&spDL);
			if (SUCCEEDED(hr)) // If the item is a DistList item
			{
				wprintf(L"%s\n", (LPCWSTR)spDL->DLName);
			}
		}


		/////////////////////////////////////////////////////////////////////
		// Create and send a new mail item.
		// 

		_putws(L"Create and send a new mail item");

		Outlook::_MailItemPtr spMail = spOutlookApp->CreateItem(
			Outlook::olMailItem);

		// Set the properties of the email.
		spMail->Subject = _bstr_t(L"Feedback of All-In-One Code Framework");
		spMail->To = _bstr_t(L"codefxf@microsoft.com");
		spMail->HTMLBody = _bstr_t(L"<b>Feedback:</b><br />");

		// Displays a new Inspector object for the item and allows users to 
		// click on the Send button to send the mail manually.
		// Modal = true makes the Inspector window modal.
		spMail->Display(true);

		// [-or-]

		// Automatically send the mail without a new Inspector window.
		//spMail->Send();


		/////////////////////////////////////////////////////////////////////
		// User logs off and quits Outlook.
		// 

		_putws(L"Log off and quit the Outlook application");
		spNS->Logoff();
		spOutlookApp->Quit();


		/////////////////////////////////////////////////////////////////////
		// Release the COM objects.
		// 

		// Releasing the references is not necessary for the smart pointers
		// ...
		// spOutlookApp.Release();
		// ...

	}
	catch (_com_error &err)
	{
		wprintf(L"Outlook throws the error: %s\n", err.ErrorMessage());
		wprintf(L"Description: %s\n", (LPCWSTR) err.Description());
	}

	// Uninitialize COM for this thread.
	CoUninitialize();

	return 0;
}