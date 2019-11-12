=============================================================================
    DYNAMIC LINK LIBRARY : CppShellExtCopyHookHandler Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates creating a Shell copy hook handler with C++. 

Normally, users and applications can copy, move, delete, or rename folders 
with few restrictions. By implementing a copy hook handler, you can control 
whether or not these operations take place. For instance, implementing such a 
handler allows you to prevent critical folders from being renamed or deleted. 
Copy hook handlers can also be implemented for printer objects.

Copy hook handlers are global. The Shell calls all registered handlers every 
time an application or user attempts to copy, move, delete, or rename a 
folder or printer object. The handler does not perform the operation itself. 
It only approves or vetoes it. If all handlers approve, the Shell does the 
operation. If any handler vetoes the operation, it is canceled and the 
remaining handlers are not called. Copy hook handlers are not informed of the 
success or failure of the operation, so they cannot be used to monitor file 
operations.

Like most Shell extension handlers, copy hook handlers are in-process 
Component Object Model (COM) objects implemented as DLLs. They export one 
interface in addition to IUnknown: ICopyHook. The Shell initializes the 
handler directly.

The example copy hook handler has the class ID (CLSID): 
    {9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}

It hooks the renaming operation of folders in Windows Explorer. When you are 
renaming a folder whose name contains "Test" in the Shell, the copy hook 
handler pops up a message box, asking if the user really wants to rename the 
folder. If the user clicks "Yes", the operation will proceed. If the user 
clicks "No" or "Cancel", the renaming operation is cancelled. 


/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

A. Setup

If you are going to use the Shell extension in a x64 Windows system, please 
configure the Visual C++ project to target 64-bit platforms using project 
configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). Only 
64-bit extension DLLs can be loaded in the 64-bit Windows Shell. If the 
extension is to be loaded in a 32-bit Windows system, you can use the default 
Win32 project configuration to build the project.

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtCopyHookHandler.dll and enter the 
command:

    Regsvr32.exe CppShellExtCopyHookHandler.dll

The copy hook handler is registered successfully if you see a message box 
saying:

    "DllRegisterServer in CppShellExtCopyHookHandler.dll succeeded."

NOTE:
After the registration, you must restart Windows Explorer (explorer.exe) so 
that the Shell loads the new copy hook handler. 

    Official way to restart explorer
    http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx

The reason is that the shell builds and caches a list of registered copy hook 
handlers the first time copy hook handlers are called in a process. Once the
list is created, there is no mechanism for updating or flushing the cache 
other than terminating the process. The best option that we can offer at this 
point is to restart the explorer.exe process or reboot the system after the 
copy hook handler is registered.

B. Removal

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtCopyHookHandler.dll and enter the 
command:

    Regsvr32.exe /u CppShellExtCopyHookHandler.dll

The copy hook handler is unregistered successfully if you see a message 
box saying:

    "DllUnregisterServer in CppShellExtCopyHookHandler.dll succeeded."

NOTE:
After the registration, you still need to restart Windows Explorer 
(explorer.exe) or reboot the system in order that the copy hook handler is 
unloaded from the Shell.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the copy hook handler 
code sample.

Step1. If you are going to use the Shell extension in a x64 Windows system, 
please configure the Visual C++ project to target 64-bit platforms using 
project configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). 
Only 64-bit extension DLLs can be loaded in the 64-bit Windows Shell. If the 
extension is to be loaded in a 32-bit Windows system, you can use the default 
Win32 project configuration.

Step2. After you successfully build the sample project in Visual Studio 2010, 
you will get a DLL: CppShellExtCopyHookHandler.dll. Start a command prompt 
as administrator, navigate to the folder that contains the file and enter the 
command:

    Regsvr32.exe CppShellExtCopyHookHandler.dll

The copy hook handler is registered successfully if you see a message box 
saying:

    "DllRegisterServer in CppShellExtCopyHookHandler.dll succeeded."

NOTE:
After the registration, you must restart Windows Explorer (explorer.exe) so 
that the Shell loads the new copy hook handler. 

    Official way to restart explorer
    http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx

Step3. Find a folder whose name contains "Test" in the Windows Explorer (e.g. 
the "TestFolder" folder in the sample directory). Rename the folder. A 
message box will be prompted with the message:

    "Are you sure to rename the folder '<Path of TestFolder>' as 
    '<New folder name>' ?" 

If you click "Yes", the operation will proceed. If you click "No" or "Cancel", 
the renaming operation is cancelled. 

Step4. In the same command prompt, run the command 

    Regsvr32.exe /u CppShellExtCopyHookHandler.dll

to unregister the Shell copy hook handler.

NOTE:
After the registration, you still need to restart Windows Explorer 
(explorer.exe) or reboot the system in order that the copy hook handler is 
unloaded from the Shell.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating and configuring the project

In Visual Studio 2010, create a Visual C++ / Win32 / Win32 Project named 
"CppShellExtCopyHookHandler". In the "Application Settings" page of Win32 
Application Wizard, select the application type as "DLL" and check the "Empty 
project" option. After you click the Finish button, an empty Win32 DLL 
project is created.

-----------------------------------------------------------------------------

B. Implementing a basic Component Object Model (COM) DLL

Shell extension handlers are all in-process COM objects implemented as DLLs. 
Making a basic COM includes implementing DllGetClassObject, DllCanUnloadNow, 
DllRegisterServer, and DllUnregisterServer in (and exporting them from) the 
DLL, adding a COM class with the basic implementation of the IUnknown 
interface, preparing the class factory for your COM class. The relevant files 
in this code sample are:

  dllmain.cpp - implements DllMain and the DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer functions that are necessary for a 
    COM DLL. 

  GlobalExportFunctions.def - exports the DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer functions from the DLL through the 
    module-definition file. You need to pass the .def file to the linker by 
    configuring the Module Definition File property in the project's Property 
    Pages / Linker / Input property page.

  Reg.h/cpp - defines the reusable helper functions to register or unregister 
    in-process COM components in the registry: 
    RegisterInprocServer, UnregisterInprocServer

  FolderCopyHook.h/cpp - defines the COM class. You can find the basic 
    implementation of the IUnknown interface in the files.

  ClassFactory.h/cpp - defines the class factory for the COM class. 

-----------------------------------------------------------------------------

C. Implementing the copy hook handler and registering it for folder objects.

-----------
Implementing the copy hook handler:

Like all Shell extension handlers, copy hook handlers are in-process COM 
objects implemented as DLLs. They export one interface in addition to 
IUnknown: ICopyHook. The Shell initializes the handler directly, so there is 
no need for an initialization interface such as IShellExtInit.

    class FolderCopyHook : public ICopyHook
    {
    public:
        // ICopyHook
        IFACEMETHODIMP_(UINT) CopyCallback(HWND hwnd, UINT wFunc, UINT wFlags, 
            LPCWSTR pszSrcFile, DWORD dwSrcAttribs, 
            LPCWSTR pszDestFile, DWORD dwDestAttribs);
    };

  1. Implementing ICopyHook

  The ICopyHook interface has a single method, ICopyHook::CopyCallback. When 
  a folder is about to be moved, the Shell calls this method. It passes in a 
  variety of information, including:

    * The folder's name. 
    * The folder's destination or new name. 
    * The operation that is being attempted. 
    * The attributes of the source and destination folders. 
    * A window handle that can be used to display a user interface. 

  When your handler's ICopyHook.CopyCallback method is called, it returns 
  one of the three following values to indicate to the Shell how it should 
  proceed.

    * IDYES: Allows the operation 
    * IDNO: Prevents the operation on this folder. The Shell can continue 
      with any other operations that have been approved, such as a batch 
      copy operation. 
    * IDCANCEL: Prevents the current operation and cancels any pending 
      operations.

  In this sample folder copy hook, we check if the folder is being renamed, 
  and if the original folder name contains "Test". If it's true, we prompt a 
  message box asking the user to confirm the operation. 
  
    IFACEMETHODIMP_(UINT) FolderCopyHook::CopyCallback(HWND hwnd, UINT wFunc, 
        UINT wFlags, LPCWSTR pszSrcFile, DWORD dwSrcAttribs, LPCWSTR pszDestFile, 
        DWORD dwDestAttribs)
    {
        int result = IDYES;

        // If the file name contains "Test" and it is being renamed...
        if (wcsstr(pszSrcFile, L"Test") != NULL && wFunc == FO_RENAME)
        {
            wchar_t szMessage[256];
            StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
                L"Are you sure to rename the folder %s as %s ?", 
                pszSrcFile, pszDestFile);

            result = MessageBox(hwnd, szMessage, L"CppShellExtCopyHookHandler", 
                MB_YESNOCANCEL);
        }

        assert(result == IDYES || result == IDNO || result == IDCANCEL);
        return result;
    }

A folder object can have multiple copy hook handlers. For example, even if 
the Shell already has a copy hook handler registered for a particular folder 
object, you can still register one of your own. If two or more copy hook 
handlers are registered for an object, the Shell simply calls each of them 
before performing one of the specified file system operations.

-----------
Registering the folder copy hook handler:

The CLSID of the handler is declared at the beginning of dllmain.cpp.

    // {9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}
    const CLSID CLSID_FolderCopyHook = 
    { 0x9EFFD3DF, 0x86B2, 0x488F, { 0xAB, 0x5B, 0x77, 0xD6, 0xFA, 0x92, 0x77, 0xD2 } };

When you write your own handler, you must create a new CLSID by using the 
"Create GUID" tool in the Tools menu, and specify the CLSID value here.

Copy hook handlers for folders are registered under the key:

    HKEY_CLASSES_ROOT\Directory\shellex\CopyHookHandlers

Create a subkey of CopyHookHandlers named for the handler, and set the 
subkey's default value to the string form of the handler's class identifier 
(CLSID) GUID.

The registration of the handler is implemented in the DllRegisterServer 
function of dllmain.cpp. DllRegisterServer first calls the 
RegisterInprocServer function in Reg.h/cpp to register the COM component. 
Next, it calls RegisterShellExtFolderCopyHookHandler to register the copy 
hook handler under HKEY_CLASSES_ROOT\Directory\shellex\CopyHookHandlers\.

The following keys and values are added in the registration process of the 
sample handler. 

    HKCR
    {
        NoRemove CLSID
        {
            ForceRemove {9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2} = 
                s 'CppShellExtCopyHookHandler.FolderCopyHook Class'
            {
                InprocServer32 = s '<Path of CppShellExtCopyHookHandler.DLL file>'
                {
                    val ThreadingModel = s 'Apartment'
                }
            }
        }
        NoRemove Directory
        {
            NoRemove shellex
            {
                NoRemove CopyHookHandlers
                {
                    CppShellExtCopyHookHandler = 
                        s '{9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}'
                }
            }
        }
    }

The shell builds and caches a list of registered copy hook handlers the first 
time copy hook handlers are called in a process. Once the list is created, 
there is no mechanism for updating or flushing the cache other than 
terminating the process. This applies to Windows Explorer and any other 
process that may call shell file functions, such as SHFileOperation. In other 
that your copy hook handler is recognized by the Shell after the handler is 
registered, the best option that we can offer at this point is to restart the 
Windows Explorer (explorer.exe) process or reboot the system.

The unregistration is implemented in the DllUnregisterServer function of 
dllmain.cpp. It removes the HKCR\CLSID\{<CLSID>} key and the <Name> key 
under HKCR\Directory\shellex\CopyHookHandlers. After the key is removed, you 
still need to restart the Windows Explorer (explorer.exe) process in order 
that the Shell unloads the copy hook handler.

Copy hook handlers for printer objects are registered in essentially the same 
way. The only difference is that you must register them under the 
HKEY_CLASSES_ROOT\Printers key. 


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Creating Copy Hook Handlers
http://msdn.microsoft.com/en-us/library/cc144063.aspx

MSDN: ICopyHook Interface
http://msdn.microsoft.com/en-us/library/bb776049.aspx


/////////////////////////////////////////////////////////////////////////////