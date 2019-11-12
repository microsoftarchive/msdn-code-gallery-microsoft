// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

#pragma managed(off)

// Use the ATL Registrar to register the engine. 
class CSampleEngineModule : public CAtlDllModuleT< CSampleEngineModule >
{

};

CSampleEngineModule _SampleEngineModule;
HMODULE _hModThis;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
    _hModThis = hModule;
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

