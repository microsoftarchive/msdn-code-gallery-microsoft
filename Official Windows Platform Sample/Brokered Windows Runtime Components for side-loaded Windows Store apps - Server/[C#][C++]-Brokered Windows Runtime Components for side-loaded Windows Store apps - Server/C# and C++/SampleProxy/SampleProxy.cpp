#define ENTRY_PREFIX Proxy

//
// Make rpcproxy.h compile for C++!
//
typedef void *IRpcStubBufferVtbl;
typedef void *IPSFactoryBufferVtbl;

#include "dlldata.c"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    return ProxyDllMain(hModule, ul_reason_for_call, lpReserved);
}

STDAPI DllRegisterServer(void)
{
    return ProxyDllRegisterServer();
}

STDAPI DllUnregisterServer(void)
{
    return ProxyDllUnregisterServer();
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID *ppv)
{
    return ProxyDllGetClassObject(rclsid, riid, ppv);
}

STDAPI DllCanUnloadNow(void)
{
    return ProxyDllCanUnloadNow();
}

