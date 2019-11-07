/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#pragma once

#include <atlstr.h>
#include <VSLCommandTarget.h>

#include "resource.h"       // main symbols
#include "Guids.h"
#include "..\ServicesUI\Resource.h"

#include "..\ServicesUI\CommandIds.h"

using namespace VSL;

/*
ServiceConsumerPackage is a VS Package that provides command handlers for this project's three 
menu items:
OnExecuteGlobalService - Gets the service SGlobalService and then calls
	IGlobalService->ExecuteGlobalService().
OnExecuteLocalService - Fails to get the service SLocalService, since it is not proffered.
OnExecuteLocalUsingGlobal - Gets the service SGlobalService and then calls
	IGlobalService->ExecuteLocalServiceThroughGlobalService(), which in turn gets the
	service SLocalService and calls LocalService->ExecuteLocalService().

See Reference.Package for details on implementing a VS Package.

See Reference.MenuAndCommands for details on implementing a menus.
*/

class ATL_NO_VTABLE ServiceConsumerPackage : 
	// CComObjectRootEx and CComCoClass are used to implement a non-thread safe COM object, and 
	// a partial implementation for IUknown (the COM map below provides the rest).
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<ServiceConsumerPackage, &CLSID_ServiceConsumerPackage>,
	// Provides the implementation for IVsPackage to make this COM object into a VS Package.
	public IVsPackageImpl<ServiceConsumerPackage, &CLSID_ServiceConsumerPackage>,
	public IOleCommandTargetImpl<ServiceConsumerPackage>,
	// Provides consumers of this object with the ability to determine which interfaces support
	// extended error information.
	public ATL::ISupportErrorInfoImpl<&__uuidof(IVsPackage)>
{
public:

// Provides a portion of the implementation of IUnknown, in particular the list of interfaces
// the ServiceConsumerPackage object will support via QueryInterface
BEGIN_COM_MAP(ServiceConsumerPackage)
	COM_INTERFACE_ENTRY(IVsPackage)
	COM_INTERFACE_ENTRY(IOleCommandTarget)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// COM objects typically should not be cloned, and this prevents cloning by declaring the 
// copy constructor and assignment operator private (NOTE:  this macro includes the decleration of
// a private section, so everything following this macro and preceding a public or protected 
// section will be private).
VSL_DECLARE_NOT_COPYABLE(ServiceConsumerPackage)

public:
	ServiceConsumerPackage()
	{
	}
	
	~ServiceConsumerPackage()
	{
	}

	// This method will be called after IVsPackage::SetSite is called with a valid site
	void PostSited(IVsPackageEnums::SetSiteResult /*result*/)
	{
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

	////////////////////////////////////////////////////////////////////////////////
	// Callback functions used by the command handlers.

	void OnExecuteGlobalService(CommandHandler* /*pSender*/, DWORD /*flags*/, VARIANT* /*pIn*/, VARIANT* /*pOut*/)
	{
		// Query for the global service
		CComPtr<IGlobalService> spIGlobalService;
		HRESULT hr = GetVsSiteCache().QueryCachedService<IGlobalService, __uuidof(SGlobalService)>(&spIGlobalService);
		CHKHR(hr);
		CHK(spIGlobalService != NULL, E_FAIL); // paranoid

		// Execute the method
		hr = spIGlobalService->ExecuteGlobalService();
		CHKHR(hr);
	}

	void OnExecuteLocalService(CommandHandler* /*pSender*/, DWORD /*flags*/, VARIANT* /*pIn*/, VARIANT* /*pOut*/)
	{
		// Querying for the local service is expected to fail
		CComPtr<ILocalService> spILocalService;
		HRESULT hr = GetVsSiteCache().QueryCachedService<ILocalService, __uuidof(SLocalService)>(&spILocalService);

		// Make sure it failed
		CHK(FAILED(hr), E_UNEXPECTED); 
		CHK(spILocalService == NULL, E_UNEXPECTED);

		// Write to the Output window that it failed.
		CStringW strMessage;
		VSL_CHECKBOOL_GLE(strMessage.LoadString(_AtlBaseModule.GetResourceInstance(), IDS_SERVICE_GET_LOCAL_SERVICE_FAILED));
		m_OutputWindow.OutputMessageWithPreAndPostBarsOfEquals(strMessage);
	}

	void OnExecuteLocalUsingGlobal(CommandHandler* /*pSender*/, DWORD /*flags*/, VARIANT* /*pIn*/, VARIANT* /*pOut*/)
	{
		// Query for the global service
		CComPtr<IGlobalService> spIGlobalService;
		HRESULT hr = GetVsSiteCache().QueryCachedService<IGlobalService, __uuidof(SGlobalService)>(&spIGlobalService);
		CHKHR(hr);
		CHK(spIGlobalService != NULL, E_FAIL); // paranoid

		// Execute the method
		hr = spIGlobalService->ExecuteLocalServiceThroughGlobalService();
		CHKHR(hr);
	}

// NOTE - the arguments passed to these macros can not have names longer then 30 characters
// Definition of the commands handled by this package
VSL_BEGIN_COMMAND_MAP()

	VSL_COMMAND_MAP_ENTRY(guidConsumerCommandSet, cmdidClientExecuteGlobalService, NULL, CommandHandler::ExecHandler(&OnExecuteGlobalService))
	VSL_COMMAND_MAP_ENTRY(guidConsumerCommandSet, cmdidClientExecuteLocalService, NULL, CommandHandler::ExecHandler(&OnExecuteLocalService))
	VSL_COMMAND_MAP_ENTRY(guidConsumerCommandSet, cmdidClientExecuteLocalUsingGlobal, NULL, CommandHandler::ExecHandler(&OnExecuteLocalUsingGlobal))

VSL_END_VSCOMMAND_MAP()


private:

	VsOutputWindowUtilities<> m_OutputWindow;

};

// This exposes ServiceConsumerPackage for instantiation via DllGetClassObject; however, an instance
// can not be created by CoCreateInstance, as ServiceConsumerPackage is specfically registered with
// VS, not the the system in general.
OBJECT_ENTRY_AUTO(CLSID_ServiceConsumerPackage, ServiceConsumerPackage)
