/****************************** Module Header ******************************\
* Module Name:  Solution2.h
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

#pragma once


/*!
* \brief
* Automate Microsoft Excel using C++ and the COM APIs.
* 
* \param lpParam
* \returns
* The prototype of a function that serves as the starting address for a 
* thread
*/
DWORD WINAPI AutomateExcelByCOMAPI(LPVOID lpParam);


/*!
* \brief
* Automation helper function.
* 
* \param autoType
* DISPATCH_PROPERTYGET || DISPATCH_PROPERTYPUT || DISPATCH_PROPERTYPUTREF
* || DISPATCH_METHOD
* 
* \param pvResult
* Holds the return value in a VARIANT
* 
* \param pDisp
* The IDispatch interface
*
* \param ptName
* The property/method name exposed by the interface
* 
* \param cArgs
* The count of the arguments
* 
* \returns
* HRESULT
* 
* The AutoWrap() function simplifies most of the low-level details involved 
* with using IDispatch directly. Feel free to use it in your own 
* implementations. One caveat is that if you pass multiple parameters, they 
* need to be passed in reverse-order.
* 
* \example
* AutoWrap(
*     DISPATCH_METHOD, NULL, pDisp, L"call", 3, parm[2], parm[1], parm[0]);
*/
HRESULT AutoWrap(int autoType, VARIANT *pvResult, IDispatch *pDisp,
                 LPOLESTR ptName, int cArgs...);