/****************************** Module Header ******************************\
* Module Name:  Solution2.h
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

#pragma once


//
//   FUNCTION: AutomateOutlookByCOMAPI(LPVOID)
//
//   PURPOSE: Automate Microsoft Outlook using C++ and the COM APIs.
//
//   PARAMETERS:
//      * lpParam - The thread data passed to the function using the 
//      lpParameter parameter when creating a thread. 
//      (http://msdn.microsoft.com/en-us/library/ms686736.aspx)
//
//   RETURN VALUE: The return value indicates the success or failure of the 
//      function. 
//
DWORD WINAPI AutomateOutlookByCOMAPI(LPVOID lpParam);