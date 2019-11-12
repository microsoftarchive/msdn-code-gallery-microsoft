/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// BasicPackage.h

#pragma once

#include <atlstr.h>
#include <VSLCommandTarget.h>


#include "resource.h"       // main symbols
#include "Guids.h"
#include "..\UIDll\Resource.h"

#include "..\UIDll\CommandIds.h"



using namespace VSL;

/*
The BasicPackage class demonstrates how to create the most basic Visual Studio Package in C++.

--IVsPackage--

The IVsPackage implementation, which is required to be a Visual Studio Package, is inherited 
from the VSL::IVSPackageImpl class.

--Package Registration--

The implemenation of the registration and unregistration is provided by the combination of
BasicPackage.rgs, and VSDllRegisterServer and VSDllUnregisterServer in Package.cpp, which are
invoked by the tool Regit.exe, and the registry macros below, which provide the ATL required
implementation of static HRESULT WINAPI UpdateRegistry(BOOL bRegister).  There is post build
step included in the Package project to call Regit.exe to register this package in the experimental
hive.

The standard DllRegisterServer and DllUnregisterServer are also provided in Package.cpp; however,
registering with regsvr32.exe does not allow the package to be registered under the experimental
registry hive.

Because BasicPackage is specifically registered with VS, an instance can not be created via
CoCreateInstance.

CLSID_BasicPackage is GUID for this particular package, which is required by ATL and VSL.  The
value CLSID_BasicPackage will the key the package is registered under in the VS registry hive.

To register the package we call 'Devenv.exe /rootsuffix Exp ' in order to get the logo and product name
for the package to show up in the Help About dialog. 
The "/rootsuffix Exp" part of the command is to let VisualStudio know that we want to use the 
experimental registry hive.  The "/setup" lets VisualStudio know that we are running in the mode 
where it is looking for added/removed component to update itself.

When deploying a package to an end user machine, regit.exe can not be utilized to register the 
package.  Instead the Windows Installer should be used to write out the registry keys to normal 
Visual Studio registry hive and then run command 'Devenv.exe /setup'.

--IVsInstalledProduct--

The IVsInstalledProduct implementation, which is the programatic mean (versus the registry means,
which is not shown here) of having an entry on the Help About dialog, is inherited from the 
VSL::IVsInstalledProductImpl class.

*/

class ATL_NO_VTABLE BasicPackage : 
	// CComObjectRootEx and CComCoClass are used to implement a non-thread safe COM object, and 
	// a partial implementation for IUknown (the COM map below provides the rest).
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<BasicPackage, &CLSID_Package>,
	// Provides the implementation for IVsPackage to make this COM object into a VS Package.
	public IVsPackageImpl<BasicPackage, &CLSID_Package>,
	public IOleCommandTargetImpl<BasicPackage>,
	// Provides consumers of this object with the ability to determine which interfaces support
	// extended error information.
	public ATL::ISupportErrorInfoImpl<&__uuidof(IVsPackage)>
{
public:

// Provides a portion of the implementation of IUnknown, in particular the list of interfaces
// the BasicPackage object will support via QueryInterface
BEGIN_COM_MAP(BasicPackage)
	COM_INTERFACE_ENTRY(IVsPackage)
	COM_INTERFACE_ENTRY(IOleCommandTarget)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// COM objects typically should not be cloned, and this prevents cloning by declaring the 
// copy constructor and assignment operator private (NOTE:  this macro includes the decleration of
// a private section, so everything following this macro and preceding a public or protected 
// section will be private).
VSL_DECLARE_NOT_COPYABLE(BasicPackage)

public:
	BasicPackage()
	{
	}
	
	~BasicPackage()
	{
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

// NOTE - the arguments passed to these macros can not have names longer then 30 characters
// Definition of the commands handled by this package
VSL_BEGIN_COMMAND_MAP()


VSL_END_VSCOMMAND_MAP()



private:



};

// This exposes BasicPackage for instantiation via DllGetClassObject; however, an instance
// can not be created by CoCreateInstance, as BasicPackage is specfically registered with
// VS, not the the system in general.
OBJECT_ENTRY_AUTO(CLSID_Package, BasicPackage)
