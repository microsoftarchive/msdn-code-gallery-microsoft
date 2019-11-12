# C++ Windows Shell copy hook handler (CppShellExtCopyHookHandler)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Shell
## Topics
- Shell Extension
## Updated
- 01/15/2012
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>DYNAMIC LINK LIBRARY : CppShellExtCopyHookHandler Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The code sample demonstrates creating a Shell copy hook handler with C&#43;&#43;. <br>
<br>
Normally, users and applications can copy, move, delete, or rename folders <br>
with few restrictions. By implementing a copy hook handler, you can control <br>
whether or not these operations take place. For instance, implementing such a <br>
handler allows you to prevent critical folders from being renamed or deleted. <br>
Copy hook handlers can also be implemented for printer objects.<br>
<br>
Copy hook handlers are global. The Shell calls all registered handlers every <br>
time an application or user attempts to copy, move, delete, or rename a <br>
folder or printer object. The handler does not perform the operation itself. <br>
It only approves or vetoes it. If all handlers approve, the Shell does the <br>
operation. If any handler vetoes the operation, it is canceled and the <br>
remaining handlers are not called. Copy hook handlers are not informed of the <br>
success or failure of the operation, so they cannot be used to monitor file <br>
operations.<br>
<br>
Like most Shell extension handlers, copy hook handlers are in-process <br>
Component Object Model (COM) objects implemented as DLLs. They export one <br>
interface in addition to IUnknown: ICopyHook. The Shell initializes the <br>
handler directly.<br>
<br>
The example copy hook handler has the class ID (CLSID): <br>
&nbsp; &nbsp;{9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}<br>
<br>
It hooks the renaming operation of folders in Windows Explorer. When you are <br>
renaming a folder whose name contains &quot;Test&quot; in the Shell, the copy hook <br>
handler pops up a message box, asking if the user really wants to rename the <br>
folder. If the user clicks &quot;Yes&quot;, the operation will proceed. If the user <br>
clicks &quot;No&quot; or &quot;Cancel&quot;, the renaming operation is cancelled. <br>
<br>
</p>
<h3>Setup and Removal:</h3>
<p style="font-family:Courier New"><br>
A. Setup<br>
<br>
If you are going to use the Shell extension in a x64 Windows system, please <br>
configure the Visual C&#43;&#43; project to target 64-bit platforms using project <br>
configurations (<a href="&lt;a target=" target="_blank">http://msdn.microsoft.com/en-us/library/9yb4317s.aspx).</a>'&gt;<a href="http://msdn.microsoft.com/en-us/library/9yb4317s.aspx)." target="_blank">http://msdn.microsoft.com/en-us/library/9yb4317s.aspx).</a>
 Only <br>
64-bit extension DLLs can be loaded in the 64-bit Windows Shell. If the <br>
extension is to be loaded in a 32-bit Windows system, you can use the default <br>
Win32 project configuration to build the project.<br>
<br>
In a command prompt running as administrator, navigate to the folder that <br>
contains the build result CppShellExtCopyHookHandler.dll and enter the <br>
command:<br>
<br>
&nbsp; &nbsp;Regsvr32.exe CppShellExtCopyHookHandler.dll<br>
<br>
The copy hook handler is registered successfully if you see a message box <br>
saying:<br>
<br>
&nbsp; &nbsp;&quot;DllRegisterServer in CppShellExtCopyHookHandler.dll succeeded.&quot;<br>
<br>
NOTE:<br>
After the registration, you must restart Windows Explorer (explorer.exe) so <br>
that the Shell loads the new copy hook handler. <br>
<br>
&nbsp; &nbsp;Official way to restart explorer<br>
&nbsp; &nbsp;<a href="&lt;a target=" target="_blank">http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx</a>'&gt;<a href="http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx" target="_blank">http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx</a><br>
<br>
The reason is that the shell builds and caches a list of registered copy hook <br>
handlers the first time copy hook handlers are called in a process. Once the<br>
list is created, there is no mechanism for updating or flushing the cache <br>
other than terminating the process. The best option that we can offer at this <br>
point is to restart the explorer.exe process or reboot the system after the <br>
copy hook handler is registered.<br>
<br>
B. Removal<br>
<br>
In a command prompt running as administrator, navigate to the folder that <br>
contains the build result CppShellExtCopyHookHandler.dll and enter the <br>
command:<br>
<br>
&nbsp; &nbsp;Regsvr32.exe /u CppShellExtCopyHookHandler.dll<br>
<br>
The copy hook handler is unregistered successfully if you see a message <br>
box saying:<br>
<br>
&nbsp; &nbsp;&quot;DllUnregisterServer in CppShellExtCopyHookHandler.dll succeeded.&quot;<br>
<br>
NOTE:<br>
After the registration, you still need to restart Windows Explorer <br>
(explorer.exe) or reboot the system in order that the copy hook handler is <br>
unloaded from the Shell.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the copy hook handler <br>
code sample.<br>
<br>
Step1. If you are going to use the Shell extension in a x64 Windows system, <br>
please configure the Visual C&#43;&#43; project to target 64-bit platforms using <br>
project configurations (<a href="&lt;a target=" target="_blank">http://msdn.microsoft.com/en-us/library/9yb4317s.aspx).</a>'&gt;<a href="http://msdn.microsoft.com/en-us/library/9yb4317s.aspx)." target="_blank">http://msdn.microsoft.com/en-us/library/9yb4317s.aspx).</a>
<br>
Only 64-bit extension DLLs can be loaded in the 64-bit Windows Shell. If the <br>
extension is to be loaded in a 32-bit Windows system, you can use the default <br>
Win32 project configuration.<br>
<br>
Step2. After you successfully build the sample project in Visual Studio 2010, <br>
you will get a DLL: CppShellExtCopyHookHandler.dll. Start a command prompt <br>
as administrator, navigate to the folder that contains the file and enter the <br>
command:<br>
<br>
&nbsp; &nbsp;Regsvr32.exe CppShellExtCopyHookHandler.dll<br>
<br>
The copy hook handler is registered successfully if you see a message box <br>
saying:<br>
<br>
&nbsp; &nbsp;&quot;DllRegisterServer in CppShellExtCopyHookHandler.dll succeeded.&quot;<br>
<br>
NOTE:<br>
After the registration, you must restart Windows Explorer (explorer.exe) so <br>
that the Shell loads the new copy hook handler. <br>
<br>
&nbsp; &nbsp;Official way to restart explorer<br>
&nbsp; &nbsp;<a href="&lt;a target=" target="_blank">http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx</a>'&gt;<a href="http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx" target="_blank">http://weblogs.asp.net/whaggard/archive/2003/03/12/3729.aspx</a><br>
<br>
Step3. Find a folder whose name contains &quot;Test&quot; in the Windows Explorer (e.g. <br>
the &quot;TestFolder&quot; folder in the sample directory). Rename the folder. A <br>
message box will be prompted with the message:<br>
<br>
&nbsp; &nbsp;&quot;Are you sure to rename the folder '&lt;Path of TestFolder&gt;' as <br>
&nbsp; &nbsp;'&lt;New folder name&gt;' ?&quot; <br>
<br>
If you click &quot;Yes&quot;, the operation will proceed. If you click &quot;No&quot; or &quot;Cancel&quot;, <br>
the renaming operation is cancelled. <br>
<br>
Step4. In the same command prompt, run the command <br>
<br>
&nbsp; &nbsp;Regsvr32.exe /u CppShellExtCopyHookHandler.dll<br>
<br>
to unregister the Shell copy hook handler.<br>
<br>
NOTE:<br>
After the registration, you still need to restart Windows Explorer <br>
(explorer.exe) or reboot the system in order that the copy hook handler is <br>
unloaded from the Shell.<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
A. Creating and configuring the project<br>
<br>
In Visual Studio 2010, create a Visual C&#43;&#43; / Win32 / Win32 Project named <br>
&quot;CppShellExtCopyHookHandler&quot;. In the &quot;Application Settings&quot; page of Win32 <br>
Application Wizard, select the application type as &quot;DLL&quot; and check the &quot;Empty <br>
project&quot; option. After you click the Finish button, an empty Win32 DLL <br>
project is created.<br>
<br>
-----------------------------------------------------------------------------<br>
<br>
B. Implementing a basic Component Object Model (COM) DLL<br>
<br>
Shell extension handlers are all in-process COM objects implemented as DLLs. <br>
Making a basic COM includes implementing DllGetClassObject, DllCanUnloadNow, <br>
DllRegisterServer, and DllUnregisterServer in (and exporting them from) the <br>
DLL, adding a COM class with the basic implementation of the IUnknown <br>
interface, preparing the class factory for your COM class. The relevant files <br>
in this code sample are:<br>
<br>
&nbsp;dllmain.cpp - implements DllMain and the DllGetClassObject, DllCanUnloadNow,
<br>
&nbsp; &nbsp;DllRegisterServer, DllUnregisterServer functions that are necessary for a
<br>
&nbsp; &nbsp;COM DLL. <br>
<br>
&nbsp;GlobalExportFunctions.def - exports the DllGetClassObject, DllCanUnloadNow,
<br>
&nbsp; &nbsp;DllRegisterServer, DllUnregisterServer functions from the DLL through the
<br>
&nbsp; &nbsp;module-definition file. You need to pass the .def file to the linker by
<br>
&nbsp; &nbsp;configuring the Module Definition File property in the project's Property
<br>
&nbsp; &nbsp;Pages / Linker / Input property page.<br>
<br>
&nbsp;Reg.h/cpp - defines the reusable helper functions to register or unregister
<br>
&nbsp; &nbsp;in-process COM components in the registry: <br>
&nbsp; &nbsp;RegisterInprocServer, UnregisterInprocServer<br>
<br>
&nbsp;FolderCopyHook.h/cpp - defines the COM class. You can find the basic <br>
&nbsp; &nbsp;implementation of the IUnknown interface in the files.<br>
<br>
&nbsp;ClassFactory.h/cpp - defines the class factory for the COM class. <br>
<br>
-----------------------------------------------------------------------------<br>
<br>
C. Implementing the copy hook handler and registering it for folder objects.<br>
<br>
-----------<br>
Implementing the copy hook handler:<br>
<br>
Like all Shell extension handlers, copy hook handlers are in-process COM <br>
objects implemented as DLLs. They export one interface in addition to <br>
IUnknown: ICopyHook. The Shell initializes the handler directly, so there is <br>
no need for an initialization interface such as IShellExtInit.<br>
<br>
&nbsp; &nbsp;class FolderCopyHook : public ICopyHook<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp;public:<br>
&nbsp; &nbsp; &nbsp; &nbsp;// ICopyHook<br>
&nbsp; &nbsp; &nbsp; &nbsp;IFACEMETHODIMP_(UINT) CopyCallback(HWND hwnd, UINT wFunc, UINT wFlags,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;LPCWSTR pszSrcFile, DWORD dwSrcAttribs, <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;LPCWSTR pszDestFile, DWORD dwDestAttribs);<br>
&nbsp; &nbsp;};<br>
<br>
&nbsp;1. Implementing ICopyHook<br>
<br>
&nbsp;The ICopyHook interface has a single method, ICopyHook::CopyCallback. When <br>
&nbsp;a folder is about to be moved, the Shell calls this method. It passes in a <br>
&nbsp;variety of information, including:<br>
<br>
&nbsp; &nbsp;* The folder's name. <br>
&nbsp; &nbsp;* The folder's destination or new name. <br>
&nbsp; &nbsp;* The operation that is being attempted. <br>
&nbsp; &nbsp;* The attributes of the source and destination folders. <br>
&nbsp; &nbsp;* A window handle that can be used to display a user interface. <br>
<br>
&nbsp;When your handler's ICopyHook.CopyCallback method is called, it returns <br>
&nbsp;one of the three following values to indicate to the Shell how it should <br>
&nbsp;proceed.<br>
<br>
&nbsp; &nbsp;* IDYES: Allows the operation <br>
&nbsp; &nbsp;* IDNO: Prevents the operation on this folder. The Shell can continue
<br>
&nbsp; &nbsp; &nbsp;with any other operations that have been approved, such as a batch
<br>
&nbsp; &nbsp; &nbsp;copy operation. <br>
&nbsp; &nbsp;* IDCANCEL: Prevents the current operation and cancels any pending <br>
&nbsp; &nbsp; &nbsp;operations.<br>
<br>
&nbsp;In this sample folder copy hook, we check if the folder is being renamed, <br>
&nbsp;and if the original folder name contains &quot;Test&quot;. If it's true, we prompt a <br>
&nbsp;message box asking the user to confirm the operation. <br>
&nbsp;<br>
&nbsp; &nbsp;IFACEMETHODIMP_(UINT) FolderCopyHook::CopyCallback(HWND hwnd, UINT wFunc,
<br>
&nbsp; &nbsp; &nbsp; &nbsp;UINT wFlags, LPCWSTR pszSrcFile, DWORD dwSrcAttribs, LPCWSTR pszDestFile,
<br>
&nbsp; &nbsp; &nbsp; &nbsp;DWORD dwDestAttribs)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;int result = IDYES;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// If the file name contains &quot;Test&quot; and it is being renamed...<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (wcsstr(pszSrcFile, L&quot;Test&quot;) != NULL &amp;&amp; wFunc == FO_RENAME)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;wchar_t szMessage[256];<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;StringCchPrintf(szMessage, ARRAYSIZE(szMessage),
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;L&quot;Are you sure to rename the folder %s as %s ?&quot;,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszSrcFile, pszDestFile);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;result = MessageBox(hwnd, szMessage, L&quot;CppShellExtCopyHookHandler&quot;,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;MB_YESNOCANCEL);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;assert(result == IDYES || result == IDNO || result == IDCANCEL);<br>
&nbsp; &nbsp; &nbsp; &nbsp;return result;<br>
&nbsp; &nbsp;}<br>
<br>
A folder object can have multiple copy hook handlers. For example, even if <br>
the Shell already has a copy hook handler registered for a particular folder <br>
object, you can still register one of your own. If two or more copy hook <br>
handlers are registered for an object, the Shell simply calls each of them <br>
before performing one of the specified file system operations.<br>
<br>
-----------<br>
Registering the folder copy hook handler:<br>
<br>
The CLSID of the handler is declared at the beginning of dllmain.cpp.<br>
<br>
&nbsp; &nbsp;// {9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}<br>
&nbsp; &nbsp;const CLSID CLSID_FolderCopyHook = <br>
&nbsp; &nbsp;{ 0x9EFFD3DF, 0x86B2, 0x488F, { 0xAB, 0x5B, 0x77, 0xD6, 0xFA, 0x92, 0x77, 0xD2 } };<br>
<br>
When you write your own handler, you must create a new CLSID by using the <br>
&quot;Create GUID&quot; tool in the Tools menu, and specify the CLSID value here.<br>
<br>
Copy hook handlers for folders are registered under the key:<br>
<br>
&nbsp; &nbsp;HKEY_CLASSES_ROOT\Directory\shellex\CopyHookHandlers<br>
<br>
Create a subkey of CopyHookHandlers named for the handler, and set the <br>
subkey's default value to the string form of the handler's class identifier <br>
(CLSID) GUID.<br>
<br>
The registration of the handler is implemented in the DllRegisterServer <br>
function of dllmain.cpp. DllRegisterServer first calls the <br>
RegisterInprocServer function in Reg.h/cpp to register the COM component. <br>
Next, it calls RegisterShellExtFolderCopyHookHandler to register the copy <br>
hook handler under HKEY_CLASSES_ROOT\Directory\shellex\CopyHookHandlers\.<br>
<br>
The following keys and values are added in the registration process of the <br>
sample handler. <br>
<br>
&nbsp; &nbsp;HKCR<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;NoRemove CLSID<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ForceRemove {9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2} =
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;s 'CppShellExtCopyHookHandler.FolderCopyHook Class'<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;InprocServer32 = s '&lt;Path of CppShellExtCopyHookHandler.DLL file&gt;'<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;val ThreadingModel = s 'Apartment'<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;NoRemove Directory<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;NoRemove shellex<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;NoRemove CopyHookHandlers<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;CppShellExtCopyHookHandler =
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;s '{9EFFD3DF-86B2-488F-AB5B-77D6FA9277D2}'<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
<br>
The shell builds and caches a list of registered copy hook handlers the first <br>
time copy hook handlers are called in a process. Once the list is created, <br>
there is no mechanism for updating or flushing the cache other than <br>
terminating the process. This applies to Windows Explorer and any other <br>
process that may call shell file functions, such as SHFileOperation. In other <br>
that your copy hook handler is recognized by the Shell after the handler is <br>
registered, the best option that we can offer at this point is to restart the <br>
Windows Explorer (explorer.exe) process or reboot the system.<br>
<br>
The unregistration is implemented in the DllUnregisterServer function of <br>
dllmain.cpp. It removes the HKCR\CLSID\{&lt;CLSID&gt;} key and the &lt;Name&gt; key
<br>
under HKCR\Directory\shellex\CopyHookHandlers. After the key is removed, you <br>
still need to restart the Windows Explorer (explorer.exe) process in order <br>
that the Shell unloads the copy hook handler.<br>
<br>
Copy hook handlers for printer objects are registered in essentially the same <br>
way. The only difference is that you must register them under the <br>
HKEY_CLASSES_ROOT\Printers key. <br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: Creating Copy Hook Handlers<br>
<a href="http://msdn.microsoft.com/en-us/library/cc144063.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/cc144063.aspx</a><br>
<br>
MSDN: ICopyHook Interface<br>
<a href="http://msdn.microsoft.com/en-us/library/bb776049.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/bb776049.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
