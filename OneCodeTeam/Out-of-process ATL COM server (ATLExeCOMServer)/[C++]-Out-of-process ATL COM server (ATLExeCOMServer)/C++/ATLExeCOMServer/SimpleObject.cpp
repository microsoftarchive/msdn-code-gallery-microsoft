/****************************** Module Header ******************************\
* Module Name:  SimpleObject.cpp
* Project:      ATLExeCOMServer
* Copyright (c) Microsoft Corporation.
* 
* Define the component's implementation class CSimpleObject
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
#include "stdafx.h"
#include "SimpleObject.h"
#pragma endregion


STDMETHODIMP CSimpleObject::get_FloatProperty(FLOAT* pVal)
{
	// TODO: Add your implementation code here

	*pVal = m_fField;
	return S_OK;
}

STDMETHODIMP CSimpleObject::put_FloatProperty(FLOAT newVal)
{
	// TODO: Add your implementation code here

	// Fire the event, FloatPropertyChanging
	VARIANT_BOOL cancel = VARIANT_FALSE; 
	Fire_FloatPropertyChanging(newVal, &cancel);

	if (cancel == VARIANT_FALSE)
	{
		m_fField = newVal;	// Save the new value
	}
	// else, do nothing
	return S_OK;
}

STDMETHODIMP CSimpleObject::HelloWorld(BSTR* pRet)
{
	// TODO: Add your implementation code here

	// Allocate memory for the string: 
	*pRet = ::SysAllocString(L"HelloWorld");
	if (pRet == NULL)
		return E_OUTOFMEMORY;

	// The client is now responsible for freeing BSTR
	// @see Rules for Freeing BSTRs in OLE Automation
	// http://support.microsoft.com/kb/108934
	return S_OK;
}

STDMETHODIMP CSimpleObject::GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId)
{
	// TODO: Add your implementation code here

	*pdwProcessId = GetCurrentProcessId();
	*pdwThreadId = GetCurrentThreadId();
	return S_OK;
}
