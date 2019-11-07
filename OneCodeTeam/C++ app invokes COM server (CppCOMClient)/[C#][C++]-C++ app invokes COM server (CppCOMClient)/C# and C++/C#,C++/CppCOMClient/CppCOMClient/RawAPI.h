/****************************** Module Header ******************************\
* Module Name:  RawAPI.h
* Project:      CppCOMClient
* Copyright (c) Microsoft Corporation.
* 
* This file demontrates the use of C/C++ and the COM APIs to automate a server. 
* C/C++ Automation is much more difficult, but sometimes necessary to avoid 
* overhead with MFC, or problems with #import. Basically, you work with such 
* APIs as CoCreateInstance(), and COM interfaces such as IDispatch and IUnknown.
* 
* References
*  http://support.microsoft.com/kb/216686
*  http://support.microsoft.com/kb/238393
*  http://support.microsoft.com/kb/216388
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

#pragma region Includes
#include <ole2.h> // OLE2 Definitions
#pragma endregion


/*!
 * \brief
 * RawConsumeSTAComponent - Create and access a STA COM object by calling the 
 * COM API directly from C++.
 * 
 * \param lpParam
 * \returns
 * The prototype of a function that serves as the starting address for a 
 * thread
 */
DWORD WINAPI RawConsumeSTAComponent(LPVOID lpParam);


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