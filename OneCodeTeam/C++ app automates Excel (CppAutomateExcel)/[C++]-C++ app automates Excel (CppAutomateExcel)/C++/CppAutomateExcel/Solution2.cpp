/****************************** Module Header ******************************\
* Module Name:  Solution2.cpp
* Project:      CppAutomateExcel
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution2.h/cpp demontrates the use of C/C++ and the COM APIs 
* to automate Excel. The raw automation is much more difficult, but it is  
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
//   FUNCTION: AutomateExcelByCOMAPI(LPVOID)
//
//   PURPOSE: Automate Microsoft Excel using C++ and the COM APIs.
//
DWORD WINAPI AutomateExcelByCOMAPI(LPVOID lpParam)
{
    // Initializes the COM library on the current thread and identifies 
    // the concurrency model as single-thread apartment (STA). 
    // [-or-] CoInitialize(NULL);
    // [-or-] CoCreateInstance(NULL);
    CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

    // Create the Excel.Application COM object using C++ and the COM APIs. 

    // Get CLSID of the server
    CLSID clsid;
    HRESULT hr;

    // Option 1. Get CLSID from ProgID using CLSIDFromProgID.
    LPCOLESTR progID = L"Excel.Application";
    hr = CLSIDFromProgID(progID, &clsid);
    if (FAILED(hr))
    {
        wprintf(L"CLSIDFromProgID(\"%s\") failed w/err 0x%08lx\n", progID, hr);
        return 1;
    }
    // Option 2. Build the CLSID directly.
    /*const IID CLSID_Application = 
    {0x00024500,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
    clsid = CLSID_Application;*/

    // Start the server and get the IDispatch interface

    IDispatch *pXlApp = NULL;
    hr = CoCreateInstance(      // [-or-] CoCreateInstanceEx, CoGetObject
        clsid,                  // CLSID of the server
        NULL,
        CLSCTX_LOCAL_SERVER,    // Excel.Application is a local server
        IID_PPV_ARGS(&pXlApp));

    if (FAILED(hr))
    {
        wprintf(L"Excel is not registered properly w/err 0x%08lx\n", hr);
        return 1;
    }

    _putws(L"Excel.Application is started");

    // Make Excel invisible. (i.e. Application.Visible = 0)
    {
        VARIANT x;
        x.vt = VT_I4;
        x.lVal = 0;
        hr = AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlApp, L"Visible", 1, x);
    }

    // Create a new Workbook. (i.e. Application.Workbooks.Add)
    // Get the Workbooks collection
    IDispatch *pXlBooks = NULL;
    {
        VARIANT result;
        VariantInit(&result);
        AutoWrap(DISPATCH_PROPERTYGET, &result, pXlApp, L"Workbooks", 0);
        pXlBooks = result.pdispVal;
    }

    // Call Workbooks.Add() to get a new workbook
    IDispatch *pXlBook = NULL;
    {
        VARIANT result;
        VariantInit(&result);
        AutoWrap(DISPATCH_METHOD, &result, pXlBooks, L"Add", 0);
        pXlBook = result.pdispVal;
    }

    _putws(L"A new workbook is created");

    // Get the active Worksheet and set its name.
    IDispatch *pXlSheet = NULL;
    {
        VARIANT result;
        VariantInit(&result);
        AutoWrap(DISPATCH_PROPERTYGET, &result, pXlApp, L"ActiveSheet", 0);
        pXlSheet = result.pdispVal;
    }

    {
        VARIANT vtSheetName;
        vtSheetName.vt = VT_BSTR;
        vtSheetName.bstrVal = SysAllocString(L"Report");
        AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlSheet, L"Name", 1, vtSheetName);
        VariantClear(&vtSheetName);
    }

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
    IDispatch *pXlRange = NULL;
    {
        VARIANT param;
        param.vt = VT_BSTR;
        param.bstrVal = SysAllocString(L"A2:B6");

        VARIANT result;
        VariantInit(&result);
        AutoWrap(DISPATCH_PROPERTYGET, &result, pXlSheet, L"Range", 1, param);
        pXlRange = result.pdispVal;

        VariantClear(&param);
    }

    // Set range with the safearray.
    AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlRange, L"Value2", 1, saNames);

    // Clear the safearray
    VariantClear(&saNames);

    // Save the workbook as a xlsx file and close it.
    _putws(L"Save and close the workbook");

    // pXlBook->SaveAs
    {
        // Make the file name

        // Get the directory of the current exe.
        wchar_t szFileName[MAX_PATH];
        if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
        {
            _putws(L"GetModuleDirectory failed");
            return 1;
        }

        // Concat "Sample2.xlsx" to the directory.
        wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample2.xlsx", 12);

        // Convert the NULL-terminated string to BSTR.
        VARIANT vtFileName;
        vtFileName.vt = VT_BSTR;
        vtFileName.bstrVal = SysAllocString(szFileName);

        VARIANT vtFormat;
        vtFormat.vt = VT_I4;
        vtFormat.lVal = 51;     // XlFileFormat::xlOpenXMLWorkbook

        // If there are more than 1 parameters passed, they MUST be pass in 
        // reversed order. Otherwise, you may get the error 0x80020009.
        AutoWrap(DISPATCH_METHOD, NULL, pXlBook, L"SaveAs", 2, vtFormat, 
            vtFileName);

        VariantClear(&vtFileName);
    }

    // pXlBook->Close()
    AutoWrap(DISPATCH_METHOD, NULL, pXlBook, L"Close", 0);

    // Quit the Excel application. (i.e. Application.Quit())
    _putws(L"Quit the Excel application");
    AutoWrap(DISPATCH_METHOD, NULL, pXlApp, L"Quit", 0);

    // Release the COM objects.
    if (pXlRange != NULL)
    {
        pXlRange->Release();
    }
    if (pXlSheet != NULL)
    {
        pXlSheet->Release();
    }
    if (pXlBook != NULL)
    {
        pXlBook->Release();
    }
    if (pXlBooks != NULL)
    {
        pXlBooks->Release();
    }
    if (pXlApp != NULL)
    {
        pXlApp->Release();
    }

    // Uninitialize COM for this thread.
    CoUninitialize();

    return hr;
}