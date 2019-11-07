/****************************** Module Header ******************************\
* Module Name:  SimpleObject.h
* Project:      ATLExeCOMServer
* Copyright (c) Microsoft Corporation.
* 
* Declare the component's implementation class CSimpleObject
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

#pragma region Includes
#include "resource.h"       // main symbols

#include "ATLExeCOMServer_i.h"
#include "_ISimpleObjectEvents_CP.h"
#pragma endregion


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif



// CSimpleObject

class ATL_NO_VTABLE CSimpleObject :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CSimpleObject, &CLSID_SimpleObject>,
	public IConnectionPointContainerImpl<CSimpleObject>,
	public CProxy_ISimpleObjectEvents<CSimpleObject>,
	public IDispatchImpl<ISimpleObject, &IID_ISimpleObject, &LIBID_ATLExeCOMServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CSimpleObject() : m_fField(0.0f)
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ATLSIMPLEOBJECTSTA)


BEGIN_COM_MAP(CSimpleObject)
	COM_INTERFACE_ENTRY(ISimpleObject)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IConnectionPointContainer)
END_COM_MAP()

BEGIN_CONNECTION_POINT_MAP(CSimpleObject)
	CONNECTION_POINT_ENTRY(__uuidof(_ISimpleObjectEvents))
END_CONNECTION_POINT_MAP()


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

protected:
	// Used by FloatProperty
	float m_fField;

public:

	STDMETHOD(get_FloatProperty)(FLOAT* pVal);
	STDMETHOD(put_FloatProperty)(FLOAT newVal);
	STDMETHOD(HelloWorld)(BSTR* pRet);
	STDMETHOD(GetProcessThreadID)(LONG* pdwProcessId, LONG* pdwThreadId);
};

OBJECT_ENTRY_AUTO(__uuidof(SimpleObject), CSimpleObject)
