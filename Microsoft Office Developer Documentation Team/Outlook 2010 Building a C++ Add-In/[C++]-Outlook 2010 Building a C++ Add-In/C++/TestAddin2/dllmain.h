// dllmain.h : Declaration of module class.

class CTestAddin2Module : public CAtlDllModuleT< CTestAddin2Module >
{
public :
	DECLARE_LIBID(LIBID_TestAddin2Lib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_TESTADDIN2, "{ED3F34A9-68B2-482A-9419-75DD961D82FB}")
};

extern class CTestAddin2Module _AtlModule;
