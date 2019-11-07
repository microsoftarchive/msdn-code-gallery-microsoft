/****************************** Module Header ******************************\
* Module Name:  CppAutomateExcel.cpp
* Project:      CppAutomateExcel
* Copyright (c) Microsoft Corporation.
* 
* The CppAutomateExcel example demonstrates how to write VC++ codes to create 
* a Microsoft Excel instance, create a workbook, fill data into a specific 
* range, save the workbook, close the Microsoft Excel application and then 
* clean up unmanaged COM resources.
* 
* There are three basic ways you can write VC++ automation codes:
* 
* 1. Automating Excel using the #import directive and smart pointers 
* (Solution1.h/cpp)
* 2. Automating Excel using C++ and the COM APIs (Solution2.h/cpp)
* 3. Automating Excel using MFC (This is not covered in this sample)
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

#include "Solution1.h"        // The example of using the #import directive 
                            // and smart pointers to automate Excel
#include "Solution2.h"        // The example of using the raw COM API to  
                            // automate Excel
#pragma endregion


int wmain(int argc, wchar_t* argv[])
{
    HANDLE hThread;

    // Demonstrate automating Excel using the #import directive and smart 
    // pointers in a separate thread.
    hThread = CreateThread(NULL, 0, AutomateExcelByImport, NULL, 0, NULL);
    WaitForSingleObject(hThread, INFINITE);
    CloseHandle(hThread);

    _putws(L"");

    // Demonstrate automating Word using C++ and the COM APIs in a separate 
    // thread.
    hThread = CreateThread(NULL, 0, AutomateExcelByCOMAPI, NULL, 0, NULL);
    WaitForSingleObject(hThread, INFINITE);
    CloseHandle(hThread);

    return 0;
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
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize)
{
    // Retrieve the path of the executable file of the current process.
    nSize = GetModuleFileName(NULL, pszDir, nSize);
    if (!nSize || GetLastError() == ERROR_INSUFFICIENT_BUFFER)
    {
        *pszDir = L'\0'; // Ensure it's NULL terminated
        return 0;
    }

    // Run through looking for the last slash in the file path.
    // When we find it, NULL it to truncate the following filename part.

    for (int i = nSize - 1; i >= 0; i--)
    {
        if (pszDir[i] == L'\\' || pszDir[i] == L'/')
        {
            pszDir[i + 1] = L'\0';
            nSize = i + 1;
            break;
        }
    }
    return nSize;
}


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
                         PCWSTR pszLastName)
{
    HRESULT hr;

    // Set the first name.
    long indices1[] = { index, 1 };
    VARIANT vtFirstName;
    vtFirstName.vt = VT_BSTR;
    vtFirstName.bstrVal = SysAllocString(pszFirstName);
    // Copies the VARIANT into the SafeArray
    hr = SafeArrayPutElement(psa, indices1, (void*)&vtFirstName);
    VariantClear(&vtFirstName);

    if (SUCCEEDED(hr))
    {
        // Next, set the last name.
        long indices2[] = { index, 2 };
        VARIANT vtLastName;
        vtLastName.vt = VT_BSTR;
        vtLastName.bstrVal = SysAllocString(pszLastName);
        hr = SafeArrayPutElement(psa, indices2, (void*)&vtLastName);
        VariantClear(&vtLastName);
    }

    return hr;
}