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
#include "LocalServiceProvider.h"

/*
ServiceProviderPackage is a VS Package that provides the services in this project.

In this case, ServiceProviderPackage implements the service itself, so it inherits from
IGlobalService and from	ATL::IServiceProviderImpl, with the later providing the implementation
for the IServiceProvider.

In order to make SGlobalService a global service, the service must registered with VS
and it must be proffered to VS via the service SProfferService.

ServiceProviderPackage::PostSited(IVsPackageEnums::SetSiteResult result) proffers the service to
VS.

ServiceProviderPackage also has the member m_spLocalServiceProvider, which is a local service
provider that unreachable via from other VS Packages via VS's QueryService, but it is reachable
to anything with direct pointer to an instance of this ServiceProviderPackage.

See Reference.Package for details on implementing a VS Package.

See Reference.MenuAndCommands for details on implementing a menus.
*/

class ATL_NO_VTABLE ServiceProviderPackage : 
	// CComObjectRootEx and CComCoClass are used to implement a non-thread safe COM object, and 
	// a partial implementation for IUknown (the COM map below provides the rest).
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<ServiceProviderPackage, &CLSID_ServiceProviderPackage>,
	// Provides the implementation for IVsPackage to make this COM object into a VS Package.
	public IVsPackageImpl<ServiceProviderPackage, &CLSID_ServiceProviderPackage>,
	// Provides the implementation of IServiceProviderImpl to make this COM object into a 
	public ATL::IServiceProviderImpl<ServiceProviderPackage>,
	// This is an interface defined for this project in Services.idl.
	public IGlobalService,
	// Provides consumers of this object with the ability to determine which interfaces support
	// extended error information.
	public ATL::ISupportErrorInfoImpl<&__uuidof(IVsPackage)>
{
public:

// Provides a portion of the implementation of IUnknown, in particular the list of interfaces
// the BasicPackage object will support via QueryInterface
BEGIN_COM_MAP(ServiceProviderPackage)
	COM_INTERFACE_ENTRY(IVsPackage)
	COM_INTERFACE_ENTRY(IServiceProvider)
	COM_INTERFACE_ENTRY(IGlobalService)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// Provides the internal implementation of QueryService
BEGIN_SERVICE_MAP(ServiceProviderPackage)
	SERVICE_ENTRY(__uuidof(SGlobalService))
	SERVICE_ENTRY_CHAIN(m_spLocalServiceProvider)
END_SERVICE_MAP()

// COM objects typically should not be cloned, and this prevents cloning by declaring the 
// copy constructor and assignment operator private (NOTE:  this macro includes the decleration of
// a private section, so everything following this macro and preceding a public or protected 
// section will be private).
VSL_DECLARE_NOT_COPYABLE(ServiceProviderPackage)

public:
	ServiceProviderPackage():
		m_OutputWindow(),
		m_spLocalServiceProvider()
	{
		LocalServiceProvider::CreateInstance(&m_spLocalServiceProvider);
	}
	
	~ServiceProviderPackage()
	{
	}

	// This method will be called after IVsPackage::SetSite is called with a valid site
	void PostSited(IVsPackageEnums::SetSiteResult /*result*/)
	{
		// Register the service with VS (this is in addition to the registry entry)
		ProfferServiceUtilities utility(GetVsSiteCache());
		utility.ProfferService(__uuidof(SGlobalService), static_cast<IServiceProvider*>(this));

		// Initialize the output window utility class
		m_OutputWindow.SetSite(GetVsSiteCache());
	}

	// This function provides the error information if it is not possible to load
	// the UI dll. It is for this reason that the resource IDS_E_BADINSTALL must
	// be defined inside this dll's resources.
	static const LoadUILibrary::ExtendedErrorInfo& GetLoadUILibraryErrorInfo()
	{
		static LoadUILibrary::ExtendedErrorInfo errorInfo(IDS_E_BADINSTALL);
		return errorInfo;
	}

	// DLL is registered with VS via a pkgdef file. Don't do anything if asked to
	// self-register.
	static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
	{
		return S_OK;
	}

	// IGlobalService implementation
	STDMETHOD(ExecuteGlobalService)()
	{
		VSL_STDMETHODTRY{

		CStringW strMessage;
		VSL_CHECKBOOL_GLE(strMessage.LoadString(_AtlBaseModule.GetResourceInstance(), IDS_SERVICE_GLOBAL_SERVICE));
		m_OutputWindow.OutputMessageWithPreAndPostBarsOfEquals(strMessage);

		}VSL_STDMETHODCATCH()

		return VSL_GET_STDMETHOD_HRESULT();
	}
        
	STDMETHOD(ExecuteLocalServiceThroughGlobalService)()
	{
		VSL_STDMETHODTRY{

		// QueryService to get the local service
		CComPtr<ILocalService> spILocalService;
		HRESULT hr = QueryService(__uuidof(SLocalService), __uuidof(ILocalService), reinterpret_cast<void**>(&spILocalService));
		CHKHR(hr);
		CHK(spILocalService != NULL, E_FAIL); // paranoid

		// Execute it
		hr = spILocalService->ExecuteLocalService();
		CHKHR(hr);

		}VSL_STDMETHODCATCH()

		return VSL_GET_STDMETHOD_HRESULT();
	}

private:

	VsOutputWindowUtilities<> m_OutputWindow;
	CComPtr<IUnknown> m_spLocalServiceProvider;
};

// This exposes ServiceProviderPackage for instantiation via DllGetClassObject; however, an instance
// can not be created by CoCreateInstance, as ServiceProviderPackage is specfically registered with
// VS, not the the system in general.
OBJECT_ENTRY_AUTO(CLSID_ServiceProviderPackage, ServiceProviderPackage)
