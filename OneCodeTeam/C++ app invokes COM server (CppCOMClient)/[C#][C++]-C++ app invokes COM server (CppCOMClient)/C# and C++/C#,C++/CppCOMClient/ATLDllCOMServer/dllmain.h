// dllmain.h : Declaration of module class.

class CATLDllCOMServerModule : public CAtlDllModuleT< CATLDllCOMServerModule >
{
public :
	DECLARE_LIBID(LIBID_ATLDllCOMServerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLDLLCOMSERVER, "{9DD18FED-55F6-4741-AF25-798B90C4AED5}")
};

extern class CATLDllCOMServerModule _AtlModule;
