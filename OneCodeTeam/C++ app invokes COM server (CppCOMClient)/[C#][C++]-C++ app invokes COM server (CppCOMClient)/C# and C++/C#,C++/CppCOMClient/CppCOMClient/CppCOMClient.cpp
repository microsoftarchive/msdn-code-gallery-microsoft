/****************************** Module Header ******************************\
* Module Name:  CppCOMClient.cpp
* Project:      CppCOMClient
* Copyright (c) Microsoft Corporation.
* 
* The C++ code sample demonstrates using the raw COM APIs and using the C++ 
* #import directive to automate the COM server ATLDllCOMServer.
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

#include "RawAPI.h"				// The examples of using the raw COM API to  
								// consume a COM server
#include "ImportDirective.h"	// The examples of using the #import directive 
								// and smart pointers to consume a COM server
#pragma endregion


int wmain(int argc, wchar_t *argv[])
{
	HANDLE hThread;

	hThread = CreateThread(NULL, 0, RawConsumeSTAComponent, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	hThread = CreateThread(NULL, 0, ImportCSharpComponent, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);	
	CloseHandle(hThread);

	return 0;
}

