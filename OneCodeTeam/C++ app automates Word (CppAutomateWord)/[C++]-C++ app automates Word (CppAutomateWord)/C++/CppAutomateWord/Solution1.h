/****************************** Module Header ******************************\
* Module Name:  Solution1.h
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

#pragma once


//
//   FUNCTION: AutomateWordByImport(LPVOID)
//
//   PURPOSE: Automate Microsoft Word using the #import directive and smart 
//      pointers.
//
//   PARAMETERS:
//      * lpParam - The thread data passed to the function using the 
//      lpParameter parameter when creating a thread. 
//      (http://msdn.microsoft.com/en-us/library/ms686736.aspx)
//
//   RETURN VALUE: The return value indicates the success or failure of the 
//      function. 
//
DWORD WINAPI AutomateWordByImport(LPVOID lpParam);