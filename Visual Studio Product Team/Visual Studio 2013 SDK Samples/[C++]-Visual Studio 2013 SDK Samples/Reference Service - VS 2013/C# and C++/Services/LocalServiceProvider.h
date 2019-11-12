/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#pragma once

#include "Guids.h"

/*
LocalServiceProvider is a simple service provider that derives from ATL::IServiceProviderImpl and
ILocalService.

SLocalService is not available from VS's QueryService.
*/

class ATL_NO_VTABLE LocalServiceProvider : 
	// CComObjectRootEx and CComCoClass are used to implement a non-thread safe COM object, and 
	// a partial implementation for IUknown (the COM map below provides the rest).
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<LocalServiceProvider, &CLSID_LocalServiceProvider>,
	// Provides the implementation for IVsPackage to make this COM object into a VS Package.
	public ATL::IServiceProviderImpl<LocalServiceProvider>,
	// This is an interface defined for this project in Services.idl.
	public ILocalService,
	// Provides consumers of this object with the ability to determine which interfaces support
	// extended error information.
	public ATL::ISupportErrorInfoImpl<&__uuidof(ILocalService)>
{
public:

// Provides a portion of the implementation of IUnknown, in particular the list of interfaces
// the BasicPackage object will support via QueryInterface
BEGIN_COM_MAP(LocalServiceProvider)
	COM_INTERFACE_ENTRY(IServiceProvider)
	COM_INTERFACE_ENTRY(ILocalService)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// Provides the internal implementation of QueryService
BEGIN_SERVICE_MAP(LocalServiceProvider)
	SERVICE_ENTRY(__uuidof(SLocalService))
END_SERVICE_MAP()

// COM objects typically should not be cloned, and this prevents cloning by declaring the 
// copy constructor and assignment operator private (NOTE:  this macro includes the decleration of
// a private section, so everything following this macro and preceding a public or protected 
// section will be private).
VSL_DECLARE_NOT_COPYABLE(LocalServiceProvider)

public:
	LocalServiceProvider()
	{
	}
	
	~LocalServiceProvider()
	{
	}

	// ILocalService implementation
	STDMETHOD(ExecuteLocalService)()
	{
		VSL_STDMETHODTRY{

		CStringW strMessage;
		VSL_CHECKBOOL_GLE(strMessage.LoadString(_AtlBaseModule.GetResourceInstance(), IDS_SERVICE_LOCAL_SERVICE));
		m_OutputWindow.OutputMessageWithPreAndPostBarsOfEquals(strMessage);

		}VSL_STDMETHODCATCH()

		return VSL_GET_STDMETHOD_HRESULT();
	}

private:

	VsOutputWindowUtilities<VsUtilityGlobalSiteControl> m_OutputWindow;
};

