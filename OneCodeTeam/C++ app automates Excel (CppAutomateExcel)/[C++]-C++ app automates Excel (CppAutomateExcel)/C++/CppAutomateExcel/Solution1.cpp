/****************************** Module Header ******************************\
* Module Name:  Solution1.cpp
* Project:      CppAutomateExcel
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution1.h/cpp demonstrates the use of #import to automate 
* Excel. #import (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx), 
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
//    rename("RGB", "MSORGB") \
//    rename("DocumentProperties", "MSODocumentProperties")

using namespace Office;

#import "libid:0002E157-0000-0000-C000-000000000046"
// [-or-]
//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB"

using namespace VBIDE;

#import "libid:00020813-0000-0000-C000-000000000046" \
    rename("DialogBox", "ExcelDialogBox") \
    rename("RGB", "ExcelRGB") \
    rename("CopyFile", "ExcelCopyFile") \
    rename("ReplaceText", "ExcelReplaceText") \
    no_auto_exclude
// [-or-]
//#import "C:\\Program Files\\Microsoft Office\\Office12\\EXCEL.EXE" \
//    rename("DialogBox", "ExcelDialogBox") \
//    rename("RGB", "ExcelRGB") \
//    rename("CopyFile", "ExcelCopyFile") \
//    rename("ReplaceText", "ExcelReplaceText") \
//    no_auto_exclude

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
//   FUNCTION: SafeArrayPutName(SAFEARRAY*, long, PCWSTR, PCWSTR);
//
//   PURPOSE: This is a helper function in the sample. It puts a user name 
//      (first name, last name) into a 2D safe array. The array is in this 
//      form:
//
//      John   Smith
//      Tom    Brown
//      Sue    Thomas
// 
//      Value in the first column is specified by pszFirstName. Value in the 
//      second column is specified by pszLastName. SafeArrayPutName is used 
//      add one entry (pszFirstName pszLastName) to the array on the row 
//      indicated by the index parameter.
//
//   PARAMETERS:
//      * psa - Pointer to an array descriptor created by SafeArrayCreate.
//      * index - The index of the name (first name, last name) in the array. 
//      i.e. the first dimension of the 2D array. 
//      * pszFirstName - The first name.
//      * pszLastName - The last name.
//
//   RETURN VALUE: An HRESULT value indicating whether the function succeeds 
//      or not. 
//
HRESULT SafeArrayPutName(SAFEARRAY* psa, long index, PCWSTR pszFirstName, 
                         PCWSTR pszLastName);


//
//   FUNCTION: AutomateExcelByImport(LPVOID)
//
//   PURPOSE: Automate Microsoft Excel using the #import directive and smart 
//      pointers.
// 
DWORD WINAPI AutomateExcelByImport(LPVOID lpParam)
{
    // Initializes the COM library on the current thread and identifies the
    // concurrency model as single-thread apartment (STA). 
    // [-or-] CoInitialize(NULL);
    // [-or-] CoCreateInstance(NULL);
    CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

    try
    {
        // Create the Excel.Application COM object using the #import 
        // directive and smart pointers.

        // Option 1) Create the object using the smart pointer's constructor
        // _ApplicationPtr is the original interface name, _Application, with 
        // a "Ptr" suffix.
        //Excel::_ApplicationPtr spXlApp(
        //    __uuidof(Excel::Application)    // CLSID of the component
        //    );

        // [-or-]

        // Option 2) Create the object using the smart pointer's function,
        // CreateInstance
        Excel::_ApplicationPtr spXlApp;
        HRESULT hr = spXlApp.CreateInstance(__uuidof(Excel::Application));
        if (FAILED(hr))
        {
            wprintf(L"CreateInstance failed w/err 0x%08lx\n", hr);
            return 1;
        }

        _putws(L"Excel.Application is started");

        // Make Excel invisible. (i.e. Application.Visible = 0)
        spXlApp->Visible[0] = VARIANT_FALSE;

        // Create a new Workbook. (i.e. Application.Workbooks.Add)
        Excel::WorkbooksPtr spXlBooks = spXlApp->Workbooks;
        Excel::_WorkbookPtr spXlBook = spXlBooks->Add();

        _putws(L"A new workbook is created");

        // Get the active Worksheet and set its name.
        Excel::_WorksheetPtr spXlSheet = spXlBook->ActiveSheet;
        spXlSheet->Name = _bstr_t(L"Report");
        _putws(L"The active worksheet is renamed as Report");

        // Fill data into the worksheet's cells.
        _putws(L"Filling data into the worksheet ...");

        // Construct a 5 x 2 safearray of user names
        VARIANT saNames;
        saNames.vt = VT_ARRAY | VT_VARIANT;
        {
            SAFEARRAYBOUND sab[2];
            sab[0].lLbound = 1; sab[0].cElements = 5;
            sab[1].lLbound = 1; sab[1].cElements = 2;
            saNames.parray = SafeArrayCreate(VT_VARIANT, 2, sab);

            SafeArrayPutName(saNames.parray, 1, L"John", L"Smith");
            SafeArrayPutName(saNames.parray, 2, L"Tom", L"Brown");
            SafeArrayPutName(saNames.parray, 3, L"Sue", L"Thomas");
            SafeArrayPutName(saNames.parray, 4, L"Jane", L"Jones");
            SafeArrayPutName(saNames.parray, 5, L"Adam", L"Johnson");
        }

        // Fill A2:B6 with the array of values (First and Last Names).

        // Get Range object for the Range A2:B6
        VARIANT param;
        param.vt = VT_BSTR;
        param.bstrVal = SysAllocString(L"A2:B6");
        Excel::RangePtr spXlRange = spXlSheet->Range[param];

        spXlRange->Value2 = saNames;

        // Clear the safearray
        VariantClear(&saNames);

        // Save the workbook as a xlsx file and close it.
        _putws(L"Save and close the workbook");

        // Make the file name:
        // Get the directory of the current exe.
        wchar_t szFileName[MAX_PATH];
        if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
        {
            _putws(L"GetModuleDirectory failed");
            return 1;
        }

        // Concat "Sample1.xlsx" to the directory
        wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample1.xlsx", 12);

        // Convert the NULL-terminated string to BSTR
        variant_t vtFileName(szFileName);

        spXlBook->SaveAs(vtFileName, Excel::xlOpenXMLWorkbook, vtMissing, 
            vtMissing, vtMissing, vtMissing, Excel::xlNoChange);

        spXlBook->Close();

        // Quit the Excel application. (i.e. Application.Quit)
        _putws(L"Quit the Excel application");
        spXlApp->Quit();

        // Release the COM objects.
        // Releasing the references is not necessary for the smart pointers
        // ...
        // spXlApp.Release();
        // ...
    }
    catch (_com_error &err)
    {
        wprintf(L"Excel throws the error: %s\n", err.ErrorMessage());
        wprintf(L"Description: %s\n", (LPCWSTR) err.Description());
    }

    // Uninitialize COM for this thread.
    CoUninitialize();

    return 0;
}