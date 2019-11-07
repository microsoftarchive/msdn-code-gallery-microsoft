/****************************** Module Header ******************************\
* Module Name:  Solution1.cpp
* Project:      CppAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution1.h/cpp demonstrates the use of #import to automate 
* Word. #import (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx),
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

#import "libid:0002E157-0000-0000-C000-000000000046"
// [-or-]
//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB"

using namespace VBIDE;

#import "libid:00020905-0000-0000-C000-000000000046" \
	rename("ExitWindows", "WordExitWindows") \
	rename("FindText", "WordFindText")
// [-or-]
//#import "C:\\Program Files\\Microsoft Office\\Office12\\MSWORD.OLB" \
//	rename("ExitWindows", "WordExitWindows") \
//	rename("FindText", "WordFindText")

#pragma endregion


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
//   FUNCTION: AutomateWordByImport(LPVOID)
//
//   PURPOSE: Automate Microsoft Word using the #import directive and smart 
//      pointers.
// 
DWORD WINAPI AutomateWordByImport(LPVOID lpParam)
{
	// Initializes the COM library on the current thread and identifies the
	// concurrency model as single-thread apartment (STA). 
	// [-or-] CoInitialize(NULL);
	// [-or-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	try
	{

		/////////////////////////////////////////////////////////////////////
		// Create the Word.Application COM object using the #import directive
		// and smart pointers.
		// 

		// Option 1) Create the object using the smart pointer's constructor
		// _ApplicationPtr is the original interface name, _Application, with a 
		// "Ptr" suffix.
		//Word::_ApplicationPtr spWordApp(
		//	__uuidof(Word::Application)	// CLSID of the component
		//	);
		
		// [-or-]

		// Option 2) Create the object using the smart pointer's function,
		// CreateInstance
		Word::_ApplicationPtr spWordApp;
		HRESULT hr = spWordApp.CreateInstance(__uuidof(Word::Application));
		if (FAILED(hr))
		{
			wprintf(L"CreateInstance failed w/err 0x%08lx\n", hr);
			return 1;
		}

		_putws(L"Word.Application is started");


		/////////////////////////////////////////////////////////////////////
		// Make Word invisible. (i.e. Application.Visible = 0)
		// 

		spWordApp->Visible = VARIANT_FALSE;


		/////////////////////////////////////////////////////////////////////
		// Create a new Document. (i.e. Application.Documents.Add)
		// 

		Word::DocumentsPtr spDocs = spWordApp->Documents;
		Word::_DocumentPtr spDoc = spDocs->Add();

		_putws(L"A new document is created");


		/////////////////////////////////////////////////////////////////////
		// Insert a paragraph.
		// 

		_putws(L"Insert a paragraph");

		Word::ParagraphsPtr spParas = spDoc->Paragraphs;
		Word::ParagraphPtr spPara = spParas->Add();
		Word::RangePtr spParaRng = spPara->Range;
		spParaRng->Text = _bstr_t(L"Heading 1");
		Word::_FontPtr spFont = spParaRng->Font;
		spFont->Bold = 1;
		spParaRng->InsertParagraphAfter();


		/////////////////////////////////////////////////////////////////////
		// Save the document as a docx file and close it.
		// 

		_putws(L"Save and close the document");

		// Make the file name

		// Get the directory of the current exe.
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory failed");
			return 1;
		}

		// Concat "Sample1.docx" to the directory
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample1.docx", 12);

		// Convert the NULL-terminated string to BSTR
		variant_t vtFileName(szFileName);

		spDoc->SaveAs(&vtFileName);

		spDoc->Close();


		/////////////////////////////////////////////////////////////////////
		// Quit the Word application.
		// 

		_putws(L"Quit the Word application");
		spWordApp->Quit();


		/////////////////////////////////////////////////////////////////////
		// Release the COM objects.
		// 

		// Releasing the references is not necessary for the smart pointers
		// ...
		// spWordApp.Release();
		// ...

	}
	catch (_com_error &err)
	{
		wprintf(L"Word throws the error: %s\n", err.ErrorMessage());
		wprintf(L"Description: %s\n", (LPCWSTR) err.Description());
	}

	// Uninitialize COM for this thread
	CoUninitialize();

	return 0;
}