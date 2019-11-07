// dllmain.h : Declaration of module class.

class CATLActiveXModule : public CAtlDllModuleT< CATLActiveXModule >
{
public :
    DECLARE_LIBID(LIBID_ATLActiveXLib)
    DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLACTIVEX, "{C7950237-8615-4BE5-B76C-B1373AF66FF0}")
};

extern class CATLActiveXModule _AtlModule;
