//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************
// PhotoStudio.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "commctrl.h"
#include "ShellSection.h"
#include "PhotoStudio.h"

#define MAX_LOADSTRING 100

typedef int (__cdecl  *StartWithCommandLineFCN)(LPWSTR, LPWSTR, int, GUID *, WCHAR *pszSettings);

void ShowNoComponentError(HINSTANCE hInstance)
{
	INITCOMMONCONTROLSEX InitCmnCtrlEx;
	InitCmnCtrlEx.dwICC = ICC_WIN95_CLASSES;
	InitCmnCtrlEx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	InitCommonControlsEx(&InitCmnCtrlEx);

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

	InitializeVSShellSection();

	int nRetVal = -1;
    WCHAR szExeFilePath[MAX_PATH];
    HKEY hKeyAppEnv11Hive = NULL;

    if(RegOpenKeyExW(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\AppEnv\\11.0", 0, KEY_READ, &hKeyAppEnv11Hive) == ERROR_SUCCESS)
    {
        DWORD dwType;
        DWORD dwSize = MAX_PATH;
        RegQueryValueExW(hKeyAppEnv11Hive, L"AppenvStubDLLInstallPath", NULL, &dwType, (LPBYTE)szExeFilePath, &dwSize);
        RegCloseKey(hKeyAppEnv11Hive);
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

	//Get full Unicode command line to pass to StartWithCommandLine function.
	LPWSTR lpwCmdLine = GetCommandLineW();

    HMODULE hModStubDLL = LoadLibraryW(szExeFilePath);
    if(!hModStubDLL)
    {
        ShowNoComponentError(hInstance);
        return -1;
    }

	StartWithCommandLineFCN StartWithCommandLine = (StartWithCommandLineFCN)GetProcAddress(hModStubDLL, "StartWithCommandLine");
	if(!StartWithCommandLine)
    {
        ShowNoComponentError(hInstance);
        return -1;
    }

	nRetVal = StartWithCommandLine(lpwCmdLine, L"VSShellStub1", nCmdShow, NULL, NULL);

    FreeLibrary(hModStubDLL);

    return nRetVal;
}
