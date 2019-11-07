=============================================================================
      DYNAMIC LINK LIBRARY : CppShellExtPreviewHandler Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates the C++ implementation of a preview handler for 
a new file type registered with the .recipe extension. 

Preview handlers are called when an item is selected to show a lightweight, 
rich, read-only preview of the file's contents in the view's reading pane. 
This is done without launching the file's associated application. Windows 
Vista and later operating systems support preview handlers. 

To be a valid preview handler, several interfaces must be implemented. This 
includes IPreviewHandler (shobjidl.h); IInitializeWithFile, 
IInitializeWithStream, or IInitializeWithItem (propsys.h); IObjectWithSite 
(ocidl.h); and IOleWindow (oleidl.h). There are also optional interfaces, 
such as IPreviewHandlerVisuals (shobjidl.h), that a preview handler can 
implement to provide extended support.

The example preview handler has the class ID (CLSID): 
    {78A573CA-297E-4D9F-A5FC-7F6E5EEA6FC9}

It provides previews for .recipe files. The .recipe file type is simply an 
XML file registered as a unique file name extension. It includes the title of 
the recipe, its author, difficulty, preparation time, cook time, nutrition 
information, comments, an embedded preview image, and so on. The preview 
handler extracts the title, comments, and the embedded image, and display 
them in a preview window.


/////////////////////////////////////////////////////////////////////////////
Prerequisite:

The example preview handler must be registered on Windows Vista or newer 
operating systems.


/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

A. Setup

If you are going to use the Shell extension in a x64 Windows system, please 
configure the Visual C++ project to target 64-bit platforms using project 
configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). 

If the extension is to be loaded in a 32-bit Windows system, you can use the 
default Win32 project configuration to build the project.

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtPreviewHandler.dll and enter the command:

    Regsvr32.exe CppShellExtPreviewHandler.dll

The preview handler is registered successfully if you see a message box 
saying:

    "DllRegisterServer in CppShellExtPreviewHandler.dll succeeded."

B. Removal

In a command prompt running as administrator, navigate to the folder that 
contains the build result CppShellExtPreviewHandler.dll and enter the command:

    Regsvr32.exe /u CppShellExtPreviewHandler.dll

The preview handler is unregistered successfully if you see a message box 
saying:

    "DllUnregisterServer in CppShellExtPreviewHandler.dll succeeded."


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the preview handler code 
sample.

Step1. If you are going to use the Shell extension in a x64 Windows system, 
please configure the Visual C++ project to target 64-bit platforms using 
project configurations (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx). 
If the extension is to be loaded in a 32-bit Windows system, you can use the 
default Win32 project configuration.

Step2. After you successfully build the sample project in Visual Studio 2010, 
you will get a DLL: CppShellExtPreviewHandler.dll. Start a command prompt as 
administrator, navigate to the folder that contains the file and enter the 
command:

    Regsvr32.exe CppShellExtPreviewHandler.dll

The preview handler is registered successfully if you see a message box saying:

    "DllRegisterServer in CppShellExtPreviewHandler.dll succeeded."

Step3. Find the chocolatechipcookies.recipe file in the sample folder. Turn 
on Windows Explorer Preview pane, and select the chocolatechipcookies.recipe 
file. You will see a picture of chocoate chip cookies, and the title and 
comments of the recipe in the preview pane. 

The .recipe file type is simply an XML file registered as a unique file name 
extension. It includes the title of the recipe, its author, difficulty, 
preparation time, cook time, nutrition information, comments, an embedded 
preview image, and so on. The preview handler extracts the title, comments, 
and the embedded image, and display them in a preview window.

Step4. In the same command prompt, run the command 

    Regsvr32.exe /u CppShellExtPreviewHandler.dll

to unregister the Shell preview handler.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating and configuring the project

In Visual Studio 2010, create a Visual C++ / Win32 / Win32 Project named 
"CppShellExtPreviewHandler". In the "Application Settings" page of Win32 
Application Wizard, select the application type as "DLL" and check the "Empty 
project" option. After you click the Finish button, an empty Win32 DLL 
project is created.

-----------------------------------------------------------------------------

B. Implementing a basic Component Object Model (COM) DLL

Shell extension handlers are COM objects implemented as DLLs. Making a basic 
COM includes implementing DllGetClassObject, DllCanUnloadNow, 
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
    COM components in the registry: 
    RegisterInprocServer, UnregisterInprocServer

  RecipePreviewHandler.h/cpp - defines the COM class. You can find the basic 
    implementation of the IUnknown interface in the files.

  ClassFactory.h/cpp - defines the class factory for the COM class. 

-----------------------------------------------------------------------------

C. Implementing the preview handler and registering it for a certain file 
class

-----------
Implementing the preview handler:

A preview handler must implement the following interfaces:

    IInitializeWithStream::Initialize (strongly preferred), 
        IInitializeWithFile, or IInitializeWithItem 
    IObjectWithSite 
    IOleWindow 
    IPreviewHandler 

If your preview handler supports visual settings provided by the host such as 
background color and font, it must also implement the following interface:

    IPreviewHandlerVisuals 

The RecipePreviewProvider.h/cpp files define the preview handler for .recipe 
files.

    class RecipePreviewHandler : 
        public IInitializeWithStream, 
        public IPreviewHandler, 
        public IPreviewHandlerVisuals, 
        public IOleWindow, 
        public IObjectWithSite
    {
    public:
        // IInitializeWithStream
        IFACEMETHODIMP Initialize(IStream *pStream, DWORD grfMode);

        // IPreviewHandler
        IFACEMETHODIMP SetWindow(HWND hwnd, const RECT *prc);
        IFACEMETHODIMP SetFocus();
        IFACEMETHODIMP QueryFocus(HWND *phwnd);
        IFACEMETHODIMP TranslateAccelerator(MSG *pmsg);
        IFACEMETHODIMP SetRect(const RECT *prc);
        IFACEMETHODIMP DoPreview();
        IFACEMETHODIMP Unload();

        // IPreviewHandlerVisuals (Optional)
        IFACEMETHODIMP SetBackgroundColor(COLORREF color);
        IFACEMETHODIMP SetFont(const LOGFONTW *plf);
        IFACEMETHODIMP SetTextColor(COLORREF color);

        // IOleWindow
        IFACEMETHODIMP GetWindow(HWND *phwnd);
        IFACEMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);

        // IObjectWithSite
        IFACEMETHODIMP SetSite(IUnknown *punkSite);
        IFACEMETHODIMP GetSite(REFIID riid, void **ppv);
    };

  1. Implementing IPreviewHandler

  IPreviewHandler::SetWindow - this method gets called when the previewer 
    gets created. It sets the parent window of the previewer window, as well 
    as the area within the parent to be used for the previewer window. You 
    need to resize your preview so that it renders only in the area described 
    by the prc parameter.

  IPreviewHandler::SetRect - this method directs the preview handler to set 
    focus to itself.

  IPreviewHandler::DoPreview - this is where the real work is done. Since a 
    preview is dynamic, the preview content should only be loaded when it is 
    needed. Do not load content in the initialization. If the preview  
    handler window does not exist, create it. Your preview handler's windows 
    should be children of the window provided by IPreviewHandler::SetWindow. 
    They should be the size provided by IPreviewHandler::SetWindow and 
    IPreviewHandler::SetRect (whichever was called most recently). Once you 
    have a window, load the data from the IStream that the preview handler 
    was initialized with, and render that data to your preview handler's 
    window.

    In the code sample, we load the XML document from the stream of the 
    .recipe file, extract the recipe title, comments and picture from the XML 
    document, and populate the information on the preview window. 
    
        hr = LoadXMLDocument(&pXMLDoc);
        ...
        hr = GetRecipeTitle(pXMLDoc, &pszRecipeTitle);
        ...
        hr = GetRecipeComments(pXMLDoc, &pszRecipeComments);
        ...
        hr = GetRecipeImage(pXMLDoc, &hRecipeImage, &dwAlpha);
        ...
        hr = CreatePreviewWindow(pszRecipeTitle, pszRecipeComments, hRecipeImage);

    The CreateDialog API is used to create the preview window from the dialog 
    box template IDD_MAINDIALOG. 
    
        m_hwndPreview = CreateDialog(g_hInst, MAKEINTRESOURCE(IDD_MAINDIALOG), 
            m_hwndParent, NULL);
    
    The dialog box template must set the following properties

        Border = None
        Control = True
        Style = Child

  IPreviewHandler::SetFocus - this method is called when the focus enters the 
    reading pane through a tab action. Since it can be entered as a forward 
    tab or reverse tab, use the current state of the SHIFT key to decide 
    whether the first or last tab stop in the reading pane should receive the 
    focus.

  IPreviewHandler::QueryFocus - this method should call the GetFocus function 
    and return the result of that call in the phwnd parameter.

  IPreviewHandler::TranslateAccelerator - this method directs the preview 
    handler to handle a keystroke passed up from the message pump of the 
    process in which the preview handler is running.

  IPreviewHandler::Unload - this method gets called when a shell item is de-
    selected. It directs the preview handler to cease rendering a preview and 
    to release all resources that have been allocated based on the item 
    passed in during the initialization.

  2. Implementing IInitializeWithStream/IInitializeWithItem/IInitializeWithFile

  IPreviewHandler must always be implemented in concert with one of these 
  interfaces: 
  
    IInitializeWithStream - provides the file stream
    IInitializeWithItem - provides the IShellItem
    IInitializeWithFile - provides the file path

  Whenever possible, it is recommended that initialization be done through a 
  stream using IInitializeWithStream. Benefits of this include increased 
  security and stability. In the method, store the IStream and mode parameters 
  so that you can read the item's data when you are ready to preview the item. 
  Do not load the data in Initialize. Load the data in 
  IPreviewHandler::DoPreview just before you render.

  3. Implementing IObjectWithSite

  IObjectWithSite::SetSite - it provides the site's IUnknown pointer to the 
    object.

  IObjectWithSite::GetSite - it gets the last site set with 
    IObjectWithSite::SetSite. If there is no known site, the object returns a 
    failure code.

  4. Implementing IOleWindow

  IOleWindow::GetWindow - this method retrieves a handle to one of the 
    windows participating in in-place activation (frame, document, parent, or 
    in-place object window).

  IOleWindow::ContextSensitiveHelp - this method determines whether context-
    sensitive help mode should be entered during an in-place activation 
    session.

  5. Implementing IPreviewHandlerVisuals (Optional)

  This interface should be implemented when directing the preview handler to 
  respond to the host's color and font schemes. The host queries the handler 
  for IPreviewHandlerVisuals. If found to be supported, the host provides it 
  with color, font, and text color. In this code sample, the interface is not 
  used.

  IPreviewHandlerVisuals::SetBackgroundColor - store this color and use it 
    during rendering when you want to provide a background color.

  IPreviewHandlerVisuals::SetFont - store this font information and use it 
    during rendering when you want to display text consistent with the 
    current Windows Vista settings.

  IPreviewHandlerVisuals::SetTextColor - store this text color information 
    and use it during rendering when you want to display text consistent with 
    the current Windows Vista settings.

-----------
Registering the handler for a certain file class:

The registration and unregistration of the preview handler are implemented in 
the dllmain.cpp and Reg.h/cpp files.

Preview handlers can be associated with certain file classes. The MSDN article 
http://msdn.microsoft.com/en-us/library/cc144144.aspx illustrates the 
registration of preview handlers in detail. The handler is registered by 
setting the default value of the following registry key to be the CLSID the 
handler class. 

    HKEY_CLASSES_ROOT\<File Type>\shellex\{8895b1c6-b41f-4c1c-a562-0d564250836f}

For example, 

    HKCR
    {
        NoRemove .recipe
        {
            NoRemove shellex
            {
                {8895b1c6-b41f-4c1c-a562-0d564250836f} = 
                    s '{78A573CA-297E-4D9F-A5FC-7F6E5EEA6FC9}'
            }
        }
    }

Next, add the subkey under CLSID for your preview handler. Create a new 
prevhost AppID so that the preview handler always runs in its own isolated 
process. An example is shown here. 


    HKCR
    {
        NoRemove CLSID
        {
            {78A573CA-297E-4D9F-A5FC-7F6E5EEA6FC9} = 
                s 'CppShellExtPreviewHandler.RecipePreviewHandler Class'
            {
                val AppID = s '{2992DE27-3526-48C5-B765-E55278ECBE9D}'
                InprocServer32 = s '<path of CppShellExtPreviewHandler.dll>'
                {
                    val ThreadingModel = s 'Apartment'
                }
            }
        }
        NoRemove AppID
        {
            {2992DE27-3526-48C5-B765-E55278ECBE9D}
            {
                val DllSurrogate = s '%SystemRoot%\system32\prevhost.exe'
            }
        }
    }

Finally, the preview handler must be added to the list of all preview 
handlers in the following registry key.

    HKLM or HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers

This list is used as an optimization by the system to enumerate all 
registered preview handlers for display purposes. Again, the default value is 
not required, it simply aids in the debugging process.

    HKLM or HKCU
    {
        NoRemove SOFTWARE
        {
            NoRemove Microsoft
            {
                NoRemove Windows
                {
                    NoRemove CurrentVersion
                    {
                        NoRemove
                        {
                            PreviewHandlers
                            {
                                val {78A573CA-297E-4D9F-A5FC-7F6E5EEA6FC9} = 
                                    s 'RecipePreviewHandler'
                            }
                        }
                    }
                }
            }
        }
    }

/////////////////////////////////////////////////////////////////////////////
Diagnostic:

If you have followed the recommendations to implement your preview handler as 
an in-process server, to debug your preview handler, you can attach to 
Prevhost.exe. As mentioned earlier, be aware that there could be two 
instances of Prevhost.exe, one for normal low IL processes and one for those 
handlers that have opted out of running as a low IL process.

If you do not find Prevhost.exe in your list of available processes, it 
probably has not been loaded at that point. Clicking on a file for a preview 
loads the surrogate and it should then appear as an attachable process.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Preview Handlers and Shell Preview Host
http://msdn.microsoft.com/en-us/library/cc144143.aspx

MSDN: Building Preview Handlers
http://msdn.microsoft.com/en-us/library/cc144139.aspx

MSDN: Registering Preview Handlers
http://msdn.microsoft.com/en-us/library/cc144144.aspx


/////////////////////////////////////////////////////////////////////////////