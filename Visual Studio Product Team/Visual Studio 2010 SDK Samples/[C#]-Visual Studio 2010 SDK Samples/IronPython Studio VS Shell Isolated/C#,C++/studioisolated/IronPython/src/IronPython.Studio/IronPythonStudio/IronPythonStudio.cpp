// IronPythonStudio.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "IronPythonStudio.h"

#define MAX_LOADSTRING 100

typedef int (__cdecl  *STARTFCN)(LPSTR, LPWSTR, int, GUID *, WCHAR *pszSettings);

void ShowNoComponentError(HINSTANCE hInstance)
{
	WCHAR szErrorString[1000];
	WCHAR szCaption[1000];
	LoadStringW(hInstance, IDS_ERR_MSG_FATAL, szErrorString, 1000);
	LoadStringW(hInstance, IDS_ERR_FATAL_CAPTION, szCaption, 1000);

	MessageBoxW(NULL, szErrorString, szCaption, MB_OK|MB_ICONERROR);
}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	int nRetVal = -1;
	WCHAR szExeFilePath[MAX_PATH];
	HKEY hKeyAppEnv10Hive = NULL;

	if(RegOpenKeyExW(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\AppEnv\\10.0", 0, KEY_READ, &hKeyAppEnv10Hive) == ERROR_SUCCESS)
	{
		DWORD dwType;
		DWORD dwSize = MAX_PATH;
		RegQueryValueExW(hKeyAppEnv10Hive, L"AppenvStubDLLInstallPath", NULL, &dwType, (LPBYTE)szExeFilePath, &dwSize);
		RegCloseKey(hKeyAppEnv10Hive);
	}

	if(GetFileAttributesW(szExeFilePath) == INVALID_FILE_ATTRIBUTES)
	{
		//If we cannot find it at a registered location, then try in the same directory as the application
		GetModuleFileNameW(NULL, szExeFilePath, MAX_PATH);
		WCHAR *pszStartOfFileName = wcsrchr(szExeFilePath, '\\');
		if(!pszStartOfFileName)
		{
			return -1;
		}
		*pszStartOfFileName = 0;
		wcscat_s(szExeFilePath, MAX_PATH, L"\\appenvstub.dll");

		if(GetFileAttributesW(szExeFilePath) == INVALID_FILE_ATTRIBUTES)
		{
			//If the file cannot be found in the same directory as the calling exe, then error out.
			ShowNoComponentError(hInstance);
			return -1;
		}
	}

	HMODULE hModStubDLL = LoadLibraryW(szExeFilePath);
	if(!hModStubDLL)
	{
		ShowNoComponentError(hInstance);
		return -1;
	}

	STARTFCN Start = (STARTFCN)GetProcAddress(hModStubDLL, "Start");
	if(!Start)
	{
		ShowNoComponentError(hInstance);
		return -1;
	}

	nRetVal = Start(lpCmdLine, L"IronPythonStudio", nCmdShow, NULL, NULL);

	FreeLibrary(hModStubDLL);

	return nRetVal;
}
