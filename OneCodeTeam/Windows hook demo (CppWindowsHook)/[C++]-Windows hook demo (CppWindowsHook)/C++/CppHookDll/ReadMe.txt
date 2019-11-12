========================================================================
    DYNAMIC LINK LIBRARY : CppHookDll Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

This sample Win32 DLL exports the hook data and methods that is used by the 
CppWindowsHook project. 


/////////////////////////////////////////////////////////////////////////////
Project Relation:

CppWindowsHook -> CppHookDll
CppHookDll posts a private message(WM_KEYSTROKE or WM_KEYINPUT) to 
CppWindowsHook's main window(CppWindowsHookDlg).WM_KEYINPUT is sent when a 
real key is stroked.


/////////////////////////////////////////////////////////////////////////////
Creation:

A. Creating the project

Step1. Create a Visual C++ / Win32 / Win32 Project named CppHookDll in 
Visual Studio 2008.

Step2. In the page "Application Settings" of Win32 Application Wizard, select
Application type as DLL, and check the Export symbols checkbox. Click Finish.

B. Exporting symbols from a DLL using .DEF files

Step1. Declare the data, methods to be exported in the header file. Define 
them in the corresponding .cpp file.

Step2. Add a .DEF file named HookDll.def to the project. Modify the .DEF
file based on this skeleton

Step3. In order that the DLL project recognizes the existence of the .DEF file, 
right-click the project and open its Properties dialog. In the page Linker / 
Input, set the value of Module Definition File (/DEF:) to be HookDll.DEF.

C. Exporting symbols from a DLL using __declspec(dllexport)

Step1. Create the following #ifdef block in the header file to make exporting 
& importing from a DLL simpler. (This should be automatically added if we 
check the Export symbols checkbox when creating the project.)

	#ifdef HOOKDLL_EXPORTS
	#define HOOKDLL_API __declspec(dllexport)
	#else
	#define HOOKDLL_API __declspec(dllimport)
	#endif

Step2. Add SYMBOL_DECLSPEC to the left of the calling-convention keyword of 
the symbols to be exported with __declspec(dllexport). For example, 

	BOOL HOOKDLL_API WINAPI SetKeyboardHook(BOOL bInstall, 
			DWORD dwThreadId = 0, HWND hWndCaller = NULL);


/////////////////////////////////////////////////////////////////////////////
References:

Exporting from a DLL
http://msdn.microsoft.com/en-us/library/z4zxe9k8.aspx

Exporting from a DLL Using DEF Files
http://msdn.microsoft.com/en-us/library/d91k01sh.aspx

Module-Definition (.def) Files
http://msdn.microsoft.com/en-us/library/28d6s79h.aspx

Exporting from a DLL Using __declspec(dllexport)
http://msdn.microsoft.com/en-us/library/a90k134d.aspx

Creating and Using a Dynamic Link Library (C++)
http://msdn.microsoft.com/en-us/library/ms235636.aspx

HOWTO: How To Export Data from a DLL or an Application
http://support.microsoft.com/kb/90530

Dynamic-link library
http://en.wikipedia.org/wiki/Dynamic_link_library

Stdcall and DLL tools of MSVC and MinGW
http://wyw.dcweb.cn/stdcall.htm

A Crash Course In Exporting From A DLL
http://home.hiwaay.net/~georgech/WhitePapers/Exporting/Exp.htm


/////////////////////////////////////////////////////////////////////////////