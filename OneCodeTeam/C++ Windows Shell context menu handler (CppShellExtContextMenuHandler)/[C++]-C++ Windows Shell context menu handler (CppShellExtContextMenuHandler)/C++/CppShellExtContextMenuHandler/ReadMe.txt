=============================================================================
    DYNAMIC LINK LIBRARY : CppShellExtContextMenuHandler Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates creating a Shell context menu handler with C++. 

A context menu handler is a shell extension handler that adds commands to an 
existing context menu. Context menu handlers are associated with a particular 
file class and are called any time a context menu is displayed for a member 
of the class. While you can add items to a file class context menu with the 
registry, the items will be the same for all members of the class. By 
implementing and registering such a handler, you can dynamically add items to 
an object's context menu, customized for the particular object.

Context menu handler is the most powerful but also the most complicated method 
to implement. It is strongly encouraged that you implement a context menu 
using one of the static verb methods if applicable:
http://msdn.microsoft.com/en-us/library/dd758091.aspx

The example context menu handler has the class ID (CLSID): 
    {BFD98515-CD74-48A4-98E2-13D209E3EE4F}

It adds the menu item "Display File Name (C++)" with icon to the context menu 
when you right-click a .cpp file in the Windows Explorer. Clicking the menu 
item brings up a message box that displays the full path of the .cpp file.


/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

A. Setup

If you are going to use the Shell extension in a x64 Windows system, please 
configure the Visual C++ project to target 64-bit platforms using project 
configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). Only 
64-bit extension DLLs can be loaded in the 64-bit Windows Shell. 

If the extension is to be loaded in a 32-bit Windows system, you can use the 
default Win32 project configuration to build the project.

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtContextMenuHandler.dll and enter the 
command:

    Regsvr32.exe CppShellExtContextMenuHandler.dll

The context menu handler is registered successfully if you see a message box 
saying:

    "DllRegisterServer in CppShellExtContextMenuHandler.dll succeeded."

B. Removal

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtContextMenuHandler.dll and enter the 
command:

    Regsvr32.exe /u CppShellExtContextMenuHandler.dll

The context menu handler is unregistered successfully if you see a message 
box saying:

    "DllUnregisterServer in CppShellExtContextMenuHandler.dll succeeded."


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the context menu handler 
code sample.

Step1. If you are going to use the Shell extension in a x64 Windows system, 
please configure the Visual C++ project to target 64-bit platforms using 
project configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). 
Only 64-bit extension DLLs can be loaded in the 64-bit Windows Shell. 

If the extension is to be loaded in a 32-bit Windows system, you can use the 
default Win32 project configuration.

Step2. After you successfully build the sample project in Visual Studio 2010, 
you will get a DLL: CppShellExtContextMenuHandler.dll. Start a command prompt 
as administrator, navigate to the folder that contains the file and enter the 
command:

    Regsvr32.exe CppShellExtContextMenuHandler.dll

The context menu handler is registered successfully if you see a message box 
saying:

    "DllRegisterServer in CppShellExtContextMenuHandler.dll succeeded."

Step3. Find a .cpp file in the Windows Explorer (e.g. FileContextMenuExt.cpp 
in the sample folder), and right click it. You would see the "Display File 
Name (C++)" menu item with icon in the context menu and a menu seperator 
below it. Clicking the menu item brings up a message box that displays the 
full path of the .cpp file.

The "Display File Name (C++)" menu item is added and displayed when only one 
.cpp file is selected and right-clicked. If more than one file are selected 
in the Windows Explorer, you will not see the context menu item.

Step4. In the same command prompt, run the command 

    Regsvr32.exe /u CppShellExtContextMenuHandler.dll

to unregister the Shell context menu handler.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating and configuring the project

In Visual Studio 2010, create a Visual C++ / Win32 / Win32 Project named 
"CppShellExtContextMenuHandler". In the "Application Settings" page of Win32 
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

  FileContextMenuExt.h/cpp - defines the COM class. You can find the basic 
    implementation of the IUnknown interface in the files.

  ClassFactory.h/cpp - defines the class factory for the COM class. 

-----------------------------------------------------------------------------

C. Implementing the context menu handler and registering it for a certain 
file class

-----------
Implementing the context menu handler:

The FileContextMenuExt.h/cpp files define a context menu handler. The 
context menu handler must implement the IShellExtInit and IContextMenu 
interfaces. A context menu extension is instantiated when the user displays 
the context menu for an object of a class for which the context menu handler 
has been registered.

    class FileContextMenuExt : public IShellExtInit, public IContextMenu
    {
    public:
        // IShellExtInit
        IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT 
            pDataObj, HKEY hKeyProgID);

        // IContextMenu
        IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, 
            UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
        IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
        IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, 
            UINT *pwReserved, LPSTR pszName, UINT cchMax);
    };
	
  1. Implementing IShellExtInit

  After the context menu extension COM object is instantiated, the 
  IShellExtInit::Initialize method is called. IShellExtInit::Initialize 
  supplies the context menu extension with an IDataObject object that 
  holds one or more file names in CF_HDROP format. You can enumerate the 
  selected files and folders through the IDataObject object. If any value 
  other than S_OK is returned from IShellExtInit::Initialize, the context 
  menu extension will not be used.

  In the code sample, the FileContextMenuExt::Initialize method enumerates 
  the selected files and folders. If only one file is selected, the method 
  stores the file name for later use and returns S_OK to proceed. If more 
  than one file or no file are selected, the method returns E_FAIL to not use 
  the context menu extension.

  2. Implementing IContextMenu

  After IShellExtInit::Initialize returns S_OK, the 
  IContextMenu::QueryContextMenu method is called to obtain the menu item or 
  items that the context menu extension will add. The QueryContextMenu 
  implementation is fairly straightforward. The context menu extension adds 
  its menu items using the InsertMenuItem or similar functions. The menu 
  command identifiers must be greater than or equal to idCmdFirst and must be 
  less than idCmdLast. QueryContextMenu must return the greatest numeric 
  identifier added to the menu plus one. The best way to assign menu command 
  identifiers is to start at zero and work up in sequence. If the context 
  menu extension does not need to add any items to the menu, it should simply 
  return zero from QueryContextMenu.

  In this code sample, we insert the menu item "Display File Name (C++)" with 
  an icon, and add a menu seperator below it.

  IContextMenu::GetCommandString is called to retrieve textual data for the 
  menu item, such as help text to be displayed for the menu item. If a user 
  highlights one of the items added by the context menu handler, the handler's 
  IContextMenu::GetCommandString method is called to request a Help text 
  string that will be displayed on the Windows Explorer status bar. This 
  method can also be called to request the verb string that is assigned to a 
  command. Either ANSI or Unicode verb strings can be requested. This example 
  only implements support for the Unicode values of uFlags, because only 
  those have been used in Windows Explorer since Windows 2000.

  IContextMenu::InvokeCommand is called when one of the menu items installed 
  by the context menu extension is selected. The context menu performs or 
  initiates the desired actions in response to this method.

-----------
Registering the handler for a certain file class:

The CLSID of the handler is declared at the beginning of dllmain.cpp.

// {BFD98515-CD74-48A4-98E2-13D209E3EE4F}
const CLSID CLSID_FileContextMenuExt = 
{ 0xBFD98515, 0xCD74, 0x48A4, { 0x98, 0xE2, 0x13, 0xD2, 0x09, 0xE3, 0xEE, 0x4F } };

When you write your own handler, you must create a new CLSID by using the 
"Create GUID" tool in the Tools menu, and specify the CLSID value here.

Context menu handlers are associated with either a file class or a folder. 
For file classes, the handler is registered under the following subkey.

    HKEY_CLASSES_ROOT\<File Type>\shellex\ContextMenuHandlers

The registration of the context menu handler is implemented in the 
DllRegisterServer function of dllmain.cpp. DllRegisterServer first calls the 
RegisterInprocServer function in Reg.h/cpp to register the COM component. 
Next, it calls RegisterShellExtContextMenuHandler to associate the handler 
with a certain file type. If the file type starts with '.', try to read the 
default value of the HKCR\<File Type> key which may contain the Program ID to 
which the file type is linked. If the default value is not empty, use the 
Program ID as the file type to proceed the registration. 

For example, this code sample associates the handler with '.cpp' files. 
HKCR\.cpp has the default value 'VisualStudio.cpp.10.0' by default when 
Visual Studio 2010 is installed, so we proceed to register the handler under 
HKCR\VisualStudio.cpp.10.0\ instead of under HKCR\.cpp. The following keys 
and values are added in the registration process of the sample handler. 

    HKCR
    {
        NoRemove CLSID
        {
            ForceRemove {BFD98515-CD74-48A4-98E2-13D209E3EE4F} = 
                s 'CppShellExtContextMenuHandler.FileContextMenuExt Class'
            {
                InprocServer32 = s '<Path of CppShellExtContextMenuHandler.DLL file>'
                {
                    val ThreadingModel = s 'Apartment'
                }
            }
        }
        NoRemove .cpp = s 'VisualStudio.cpp.10.0'
        NoRemove VisualStudio.cpp.10.0
        {
            NoRemove shellex
            {
                NoRemove ContextMenuHandlers
                {
                    {BFD98515-CD74-48A4-98E2-13D209E3EE4F} = 
                        s 'CppShellExtContextMenuHandler.FileContextMenuExt'
                }
            }
        }
    }

The unregistration is implemented in the DllUnregisterServer function of 
dllmain.cpp. It removes the HKCR\CLSID\{<CLSID>} key and the {<CLSID>} key 
under HKCR\<File Type>\shellex\ContextMenuHandlers.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Initializing Shell Extensions
http://msdn.microsoft.com/en-us/library/cc144105.aspx

MSDN: Creating Context Menu Handlers
http://msdn.microsoft.com/en-us/library/bb776881.aspx

MSDN: Implementing the Context Menu COM Object
http://msdn.microsoft.com/en-us/library/ms677106.aspx

MSDN: Extending Shortcut Menus
http://msdn.microsoft.com/en-us/library/cc144101.aspx

MSDN: Choosing a Static or Dynamic Shortcut Menu Method
http://msdn.microsoft.com/en-us/library/dd758091.aspx

The Complete Idiot's Guide to Writing Shell Extensions
http://www.codeproject.com/KB/shell/shellextguide1.aspx
http://www.codeproject.com/KB/shell/shellextguide2.aspx
http://www.codeproject.com/KB/shell/shellextguide7.aspx

How to Use Submenus in a Context Menu Shell Extension
http://www.codeproject.com/KB/shell/ctxextsubmenu.aspx


/////////////////////////////////////////////////////////////////////////////