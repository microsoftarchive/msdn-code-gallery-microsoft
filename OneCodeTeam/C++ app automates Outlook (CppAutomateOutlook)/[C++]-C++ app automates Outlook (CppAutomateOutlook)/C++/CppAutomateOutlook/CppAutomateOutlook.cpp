/****************************** Module Header ******************************\
* Module Name:  CppAutomateOutlook.cpp
* Project:      CppAutomateOutlook
* Copyright (c) Microsoft Corporation.
* 
* The CppAutomateOutlook example demonstrates how to write VC++ code to 
* automate Microsoft Outlook to log on with your profile, enumerate contacts, 
* send a mail, log off, close the Microsoft Outlook application and then 
* clean up unmanaged COM resources. 
* 
* There are three basic ways you can write VC++ automation codes:
* 
* 1. Automating Outlook using the #import directive and smart pointers 
* (Solution1.h/cpp)
* 2. Automating Outlook using C++ and the COM APIs (Solution2.h/cpp)
* 3. Automating Outlook using MFC (This is not covered in this sample)
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

#include "Solution1.h"		// The example of using the #import directive 
							// and smart pointers to automate Outlook
#include "Solution2.h"		// The example of using the raw COM API to  
							// automate Outlook
#pragma endregion


int wmain(int argc, wchar_t* argv[])
{
	HANDLE hThread;

	// Demonstrate automating Outlook using the #import directive and smart 
	// pointers in a separate thread.
	hThread = CreateThread(NULL, 0, AutomateOutlookByImport, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	// [-or-]

	// Demonstrate automating Outlook using C++ and the COM APIs in a 
	// separate thread.
	/*hThread = CreateThread(NULL, 0, AutomateOutlookByCOMAPI, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);*/

	return 0;
}