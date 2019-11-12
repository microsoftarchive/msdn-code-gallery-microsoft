/****************************** Module Header ******************************\
* Module Name:  Solution2.h
* Project:      CppAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* The code in Solution2.h/cpp demonstrates the use of C/C++ and the COM APIs 
* to automate Word. The raw automation is much more difficult, but it is 
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

#pragma once


//
//   FUNCTION: AutomateWordByCOMAPI(LPVOID)
//
//   PURPOSE: Automate Microsoft Word using C++ and the COM APIs.
//
//   PARAMETERS:
//      * lpParam - The thread data passed to the function using the 
//      lpParameter parameter when creating a thread. 
//      (http://msdn.microsoft.com/en-us/library/ms686736.aspx)
//
//   RETURN VALUE: The return value indicates the success or failure of the 
//      function. 
//
DWORD WINAPI AutomateWordByCOMAPI(LPVOID lpParam);