/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#include "stdafx.h"

#include "..\Services\CommonIncludes.h"

class ServicesTestModule :
    public CAtlExeModuleT<ServicesTestModule> 
{
};
ServicesTestModule _AtlModule;

VSL_DEFINE_SERVICE_MOCK(IVsUIShellServiceMock, IVsUIShellNotImpl);
VSL_DEFINE_SERVICE_MOCK(IVsShellServiceMock, IVsShellMockImpl);
VSL_DEFINE_SERVICE_MOCK(IProfferServiceMock, IProfferServiceMockImpl);

typedef InterfaceImplList<IVsOutputWindowPaneMockImpl, IUnknownInterfaceListTerminatorDefault> IVsOutputWindowPaneServiceMockIntefaceList;
VSL_DEFINE_SERVICE_MOCK_EX(IVsOutputWindowPaneServiceMock, IVsOutputWindowPaneServiceMockIntefaceList, SID_SVsGeneralOutputWindowPane);

typedef ServiceMockAdapter<&__uuidof(SGlobalService)> GlobalServiceAdapter;

typedef ServiceList<GlobalServiceAdapter, ServiceList<IVsOutputWindowPaneServiceMock, ServiceList<IProfferServiceMock, ServiceList<IVsShellServiceMock, ServiceList<IVsUIShellServiceMock, ServiceListTerminator> > > > > PackageIServiceProviderServiceList;
typedef InterfaceImplList<VSL::IServiceProviderImpl<PackageIServiceProviderServiceList>, IUnknownInterfaceListTerminator<IServiceProvider> > PackageIServiceProviderInterfaceList;

VSL_DECLARE_COM_MOCK(PackageIServiceProviderMock, PackageIServiceProviderInterfaceList){};

class ServicesTest :
	public UnitTestBase
{
public:
	ServicesTest(_In_opt_ const char* const szTestName):
		UnitTestBase(szTestName)
	{
		PackageIServiceProviderMock mock;
		CComQIPtr<IServiceProvider> spIServiceProvider = mock.GetIUnknownNoAddRef();

		CComObject<ServiceProviderPackage>* pServiceProviderPackage;
		CHKHR(CComObject<ServiceProviderPackage>::CreateInstance(&pServiceProviderPackage));
		CComPtr<CComObject<ServiceProviderPackage> > spServiceProviderPackage = pServiceProviderPackage;
		CComQIPtr<IServiceProvider> spServiceProviderPackageIServiceProvider = spServiceProviderPackage;

		DWORD dwIProfferServicCookie = 5;
		STARTVV(IProfferService, ProfferService)
			GlobalServiceAdapter::GetServiceID(),
			spServiceProviderPackageIServiceProvider,
			&dwIProfferServicCookie,
			S_OK
		ENDVVPUSH();

		DWORD_PTR dwModule = reinterpret_cast<DWORD_PTR>(_AtlBaseModule.GetModuleInstance());
		PushIVsShellLoadUILibrary(CLSID_ServiceProviderPackage, &dwModule);

		// Get the service provider properly initialized
		UTCHK(S_OK == pServiceProviderPackage->SetSite(spIServiceProvider));

		CHKHR(spServiceProviderPackage.QueryInterface(&GlobalServiceAdapter::GetAdapted()));

		CComObject<ServiceConsumerPackage>* pServiceConsumerPackage;
		CHKHR(CComObject<ServiceConsumerPackage>::CreateInstance(&pServiceConsumerPackage));
		CComPtr<CComObject<ServiceConsumerPackage> > spServiceConsumerPackage = pServiceConsumerPackage;

		PushIVsShellLoadUILibrary(CLSID_ServiceConsumerPackage, &dwModule);

		// Get the service consumer properly initialized
		UTCHK(S_OK == pServiceConsumerPackage->SetSite(spIServiceProvider));

		IVsOutputWindowPaneMockImpl::OutputStringValidValues outputStringValidValues =
		{
			VSL_VALIDVALUE_SIMPLE_VERIFY(LPCOLESTR),
			S_OK
		};
		IVsOutputWindowPaneMockImpl::SetValidValues(outputStringValidValues);

		// Test the service consumer's command handlers, which will test
		// both the local and global service provider
		pServiceConsumerPackage->OnExecuteGlobalService(NULL, 0, NULL, NULL);
		pServiceConsumerPackage->OnExecuteLocalService(NULL, 0, NULL, NULL);
		pServiceConsumerPackage->OnExecuteLocalUsingGlobal(NULL, 0, NULL, NULL);

		UTCHK(S_OK == pServiceConsumerPackage->SetSite(NULL));
		IUnknown* pIUnknown = GlobalServiceAdapter::GetAdapted().Detach();
		pIUnknown->Release();
		UTCHK(S_OK == pServiceProviderPackage->Close());
	}
};

int _cdecl _tmain(int argc, _In_count_(argc) _TCHAR* /*argv[]*/)
{
	UTRUN(ServicesTest);
	return FailureCounter::Get();
}
