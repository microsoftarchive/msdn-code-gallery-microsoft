//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "AnnotatorApplication.h"
#include "ImageEditor.h"
#include "Resource.h"
#include "UICommandHandler.h"
#include "WindowMessageHandlerImpl.h"
#include "Taskbar.h"

const std::wstring ProgID = L"Microsoft.Hilo.AnnotatorProgID";
const std::wstring FriendlyName = L"Microsoft Hilo Annotator";
const std::wstring AppUserModelID = L"Microsoft.Hilo.Annotator";
const std::wstring FileTypeExtensions[] =
{
    L".bmp",
    L".dib",
    L".jpg",
    L".jpeg",
    L".jpe",
    L".jfif",
    L".gif",
    L".tif",
    L".tiff",
    L".png"
};

using namespace Hilo::WindowApiHelpers;
using namespace Hilo::Direct2DHelpers;

//
// Constructor
//
AnnotatorApplication::AnnotatorApplication()
{
}

//
// Destructor
//
AnnotatorApplication::~AnnotatorApplication()
{
    ::CoUninitialize();
}

//
// Initialize method that will be called via the SharedObject<AnnotatorApplication>::Create method
//
HRESULT AnnotatorApplication::Initialize()
{
    // Create image editor ribbon UI command handler and message handler. 
    // These need to be created before the main window so that they could 
    // be used during main window creation
    HRESULT hr = SharedObject<UICommandHandler>::Create(&m_commandHandler);
    if (SUCCEEDED(hr))
    {
        // Create message handler
        hr = SharedObject<ImageEditorHandler>::Create(&m_imageEditorHandler);
    }

    // Set application Id
    if (SUCCEEDED(hr))
    {
        hr = SetCurrentProcessExplicitAppUserModelID(AppUserModelID.c_str());
    }

    // Register file type association to be ready for displaying jump list
    if (SUCCEEDED(hr))
    {
        RegisterFileAssociation();
    }

    ComPtr<IImageEditor> imageEdtor;
    if (SUCCEEDED(hr))
    {
        hr = m_imageEditorHandler.QueryInterface(&imageEdtor);
    }

    ComPtr<IImageEditorCommandHandler> imageEditorRibbon;
    if (SUCCEEDED(hr))
    {
        hr = m_commandHandler.QueryInterface(&imageEditorRibbon);
    }

    if (SUCCEEDED(hr))
    {
        hr = imageEditorRibbon->SetImageEditor(imageEdtor);
    }

    // Create main window
    if (SUCCEEDED(hr))
    {
        WindowApplication::Initialize();
    }

    ComPtr<IWindow> mainWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&mainWindow);
    }

    if (SUCCEEDED(hr))
    {
        // Set the title bar caption
        static const int MaxTitleLength = 100;
        wchar_t title[MaxTitleLength] = {0};
        ::LoadString(HINST_THISCOMPONENT, IDS_APP_TITLE, title, MaxTitleLength);

        hr = mainWindow->SetTitle(title);
    }

    if (SUCCEEDED(hr))
    {
        // Set the large icon
        hr = mainWindow->SetLargeIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_ANNOTATOR)));
    }

    if (SUCCEEDED(hr))
    {
        // Set the small icon
        hr = mainWindow->SetSmallIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_SMALL)));
    }

    ComPtr<IWindowFactory> windowFactory;
    if (SUCCEEDED(hr))
    {
        // Get window factory
        hr = GetWindowFactory(&windowFactory);
    }

    if (SUCCEEDED(hr))
    {
        POINT location = {0, 0};
        SIZE imageEditorSize = {CW_USEDEFAULT, CW_USEDEFAULT};

        // Create image editor window
        hr = windowFactory->Create(
            location,
            imageEditorSize,
            m_imageEditorHandler,
            mainWindow,
            &m_imageEditorWindow);
    }

    if (SUCCEEDED(hr))
    {
        // Update main window postition based on height of the ribbon
        hr = UpdateRenderWindowPosition(mainWindow, m_imageEditorWindow);
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IImageEditor> imageEditor;

        hr = m_imageEditorHandler.QueryInterface(&imageEditor);
        if (SUCCEEDED(hr))
        {
            // Attempt to load from command line. This method returns S_FALSE if no images are loaded
            hr = imageEditor->SetCurrentLocationFromCommandLine();
        }

        if (SUCCEEDED(hr))
        {
            if (S_FALSE == hr)
            {
                ComPtr<IShellItem> currentBrowseLocationItem;

                // Default to pictures library
                hr = ::SHCreateItemInKnownFolder(FOLDERID_PicturesLibrary, 0, nullptr, IID_PPV_ARGS(&currentBrowseLocationItem));

                if (FAILED(hr))
                {
                    // Set to top-level computer folder
                    hr = ::SHGetKnownFolderItem(FOLDERID_ComputerFolder, static_cast<KNOWN_FOLDER_FLAG>(0), nullptr, IID_PPV_ARGS(&currentBrowseLocationItem));
                }

                if (SUCCEEDED(hr))
                {
                    hr = imageEditor->SetCurrentLocation(currentBrowseLocationItem, false);

                    if (HRESULT_FROM_WIN32(ERROR_CANCELLED) == hr)
                    {
                        // ERROR_CANCELLED indicates that the new location does not have any images to manipulate.
                        // Let initialization finish successfully.
                        hr = S_OK;

                        // Exit since there is nothing to annotate.
                        ::PostQuitMessage(0);
                    }
                }
            }
        }
    }

    // Add two buttons to thumbnail toolbar
    if (SUCCEEDED(hr))
    {
        HWND hwnd;
        mainWindow->GetWindowHandle(&hwnd);
        Taskbar taskbar(hwnd);
        ThumbnailToobarButton backButton = {APPCOMMAND_BROWSER_BACKWARD, true};
        ThumbnailToobarButton nextButton = {APPCOMMAND_BROWSER_FORWARD, true};
        hr = taskbar.CreateThumbnailToolbarButtons(backButton, nextButton);
    }

    return hr;
}

//
// Initializes the ribbon framework
//
HRESULT AnnotatorApplication::InitializeRibbonFramework(IWindow* window)
{
    // Instantiate the Ribbon framework object
    HRESULT hr = CoCreateInstance(CLSID_UIRibbonFramework, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&m_ribbonFramework));

    ComPtr<IImageEditor> imageEditor;
    if (SUCCEEDED(hr))
    {
        hr = m_imageEditorHandler.QueryInterface(&imageEditor);
    }

    if (SUCCEEDED(hr))
    {
        hr = imageEditor->SetUIFramework(m_ribbonFramework);
    }

    HWND hWnd = nullptr;
    if (SUCCEEDED(hr))
    {
        hr = window->GetWindowHandle(&hWnd);
    }

    if (SUCCEEDED(hr))
    {
        // Initilize the ribbon framework
        hr = m_ribbonFramework->Initialize(hWnd, this);
    }

    // Finally, we load the binary markup.  This will initiate callbacks to the IUIApplication object 
    // that was provided to the framework earlier, allowing command handlers to be bound to individual
    // commands.
    if (SUCCEEDED(hr))
    {
        static const int MaxResStringLength = 100;
        wchar_t ribbonMarkup[MaxResStringLength] = {0}; // The ribbon markup
        ::LoadString(HINST_THISCOMPONENT, IDS_RIBBON_MARKUP, ribbonMarkup, MaxResStringLength);

        hr = m_ribbonFramework->LoadUI(GetModuleHandle(nullptr), ribbonMarkup);
    }

    return hr;
}

//
// Retrieves the current height of the ribbon
//
unsigned int AnnotatorApplication::GetRibbonHeight()
{
    unsigned int ribbonHeight = 0;

    if (m_ribbonFramework)
    {
        ComPtr<IUIRibbon> ribbon;
        HRESULT hr = m_ribbonFramework->GetView(0, IID_PPV_ARGS(&ribbon));
        if (SUCCEEDED(hr))
        {
            hr = ribbon->GetHeight(&ribbonHeight);

            if (FAILED(hr))
            {
                ribbonHeight = 0;
            }
        }
    }

    return ribbonHeight;
}

//
// Sets the rectangle for the child window based on the main window and the height of the ribbon
//
HRESULT AnnotatorApplication::UpdateRenderWindowPosition(IWindow* mainWindow, IWindow* childWindow)
{
    RECT rect;
    HRESULT hr = mainWindow->GetClientRect(&rect);

    if (SUCCEEDED(hr))
    {
        // Adjust the top of the rectangle based on the current ribbon height
        rect.top += GetRibbonHeight();
        hr = childWindow->SetRect(rect);
    }

    return hr;
}

//
// Called by the Ribbon framework for each command specified in markup, to allow
// the host application to bind a command handler to that command.
//
HRESULT AnnotatorApplication::OnCreateUICommand(unsigned int /*commandId*/, UI_COMMANDTYPE /*typeId*/, IUICommandHandler** commandHandler)
{
    // The ribbon uses only one command handler
    return m_commandHandler->QueryInterface(IID_PPV_ARGS(commandHandler));
}

//
//  Called when the state of a View (Ribbon is a view) changes, for example, created, destroyed, or resized.
//
HRESULT AnnotatorApplication::OnViewChanged(unsigned int /*viewId*/, UI_VIEWTYPE typeId, IUnknown* /*view*/, UI_VIEWVERB verb, int /*reasonCode*/)
{
    HRESULT hr = E_NOTIMPL;

    // Checks to see if the view that was changed was a Ribbon view.
    if (UI_VIEWTYPE_RIBBON == typeId)
    {
        switch (verb)
        {
        case UI_VIEWVERB_CREATE:
            // The view was newly created.
            break;

        case UI_VIEWVERB_SIZE:
            {
                // The view has been resized.  For the Ribbon view, the application should
                // call GetHeight to determine the height of the ribbon.
                if (m_imageEditorWindow)
                {
                    ComPtr<Hilo::WindowApiHelpers::IWindow> mainWindow;

                    if (SUCCEEDED(GetMainWindow(&mainWindow)))
                    {
                        hr = UpdateRenderWindowPosition(mainWindow, m_imageEditorWindow);
                    }
                }
            }
            break;

        case UI_VIEWVERB_DESTROY:
            // The view was destroyed.
            break;
        }
    }

    return hr;
}

//
// Called by the Ribbon framework for each command at the time of ribbon destruction.
//
HRESULT AnnotatorApplication::OnDestroyUICommand(UINT32 /*commandId*/, UI_COMMANDTYPE /*typeId*/, IUICommandHandler* commandHandler)
{
    ComPtr<IImageEditorCommandHandler> imageEditorRibbon;
    HRESULT hr = commandHandler->QueryInterface(IID_PPV_ARGS(&imageEditorRibbon));

    if (SUCCEEDED(hr))
    {
        // Release resources of the ribbon
        hr = imageEditorRibbon->SetImageEditor(nullptr);
    }

    ComPtr<IWindow> window;
    if (SUCCEEDED(hr))
    {
        hr = GetWindow(&window);
    }

    ComPtr<IImageEditor> imageEditor;
    if (SUCCEEDED(hr))
    {
        hr = m_imageEditorHandler.QueryInterface(&imageEditor);
    }

    if (SUCCEEDED(hr))
    {
        // Release resources of the ribbon
        hr = imageEditor->SetUIFramework(nullptr);
    }

    return hr;
}

//
// Called when the AnnotatorApplication window is created
//
HRESULT AnnotatorApplication::OnCreate()
{
    // We use the GetWindow function here, because m_mainWindow might not have been initialized yet.
    ComPtr<IWindow> window;
    HRESULT hr = GetWindow(&window);

    if (SUCCEEDED(hr))
    {
        hr = InitializeRibbonFramework(window);
    }

    return hr;
}

//
// Called when a key is pressed
//
HRESULT AnnotatorApplication::OnKeyDown(unsigned int key)
{
    HWND childWindow;
    HRESULT hr = m_imageEditorWindow->GetWindowHandle(&childWindow);

    if (SUCCEEDED(hr))
    {
        // Forward this message to the image editor child window
        ::SendMessage(childWindow, WM_KEYDOWN, key, NULL);
    }

    return hr;
}

//
// Called when the main application window is resized
//
HRESULT AnnotatorApplication::OnSize(unsigned int /*width*/, unsigned int /*height*/)
{
    if (!m_imageEditorWindow)
    {
        return S_OK;
    }

    ComPtr<IWindow> mainWindow;
    HRESULT hr = GetMainWindow(&mainWindow);
    if (SUCCEEDED(hr))
    {
        hr = UpdateRenderWindowPosition(mainWindow, m_imageEditorWindow);
    }

    return hr;
}

//
// Event called when the mouse wheel is rotated
//
HRESULT AnnotatorApplication::OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short /*delta*/, int /*keys*/) 
{  
    // Returning S_FALSE, notifies the calling function to send the same msg to child windows as well
    return S_FALSE;
}

//
// Event called when WM_COMMAND message is received (e.g. users click the thumbnail tool bar button, etc)
//
HRESULT AnnotatorApplication::OnCommand(WPARAM wParam, LPARAM lParam) 
{
    HWND childWindow;

    HRESULT hr = m_imageEditorWindow->GetWindowHandle(&childWindow);
    if (SUCCEEDED(hr))
    {
        // Forward this message to the image editor child window
        ::SendMessage(childWindow, WM_COMMAND, wParam, lParam);
    }

    return hr;
}

//
// Event that is fired right before the application in closed
//
HRESULT AnnotatorApplication::OnClose()
{
    ComPtr<IImageEditor> imageEditor;

    HRESULT hr = m_imageEditorHandler->QueryInterface(&imageEditor);
    if (SUCCEEDED(hr))
    {
        hr = imageEditor->SaveFiles();
    }

    ::PostQuitMessage(0);

    return S_OK;
}

//
// In order to show the recent files in Jumplist, we need to register the file association. The registration need 
// elevated privilege, so this function is to launch a seperate process to register the file types.
//
bool AnnotatorApplication::RegisterFileAssociation()
{
    // Since file registeration needs elevation, it is better to check if the file types are registered already
    // or not so as to avoid unnecessary launching a seperate registration application with elevation priviledge.
    if (TaskbarHelper::AreFileTypesRegistered(ProgID.c_str()))
    {
        // Since the file types are already registered, we do not need to register it again.
        return true;
    }

    // Get the path of target exe first
    wchar_t currentFileName[FILENAME_MAX];

    if ( !GetModuleFileName(nullptr, currentFileName, FILENAME_MAX) )
    {
        return false;
    }

    // Annotator should be found in the same directory as this binary
    std::wstring currentDirectory = currentFileName;
    std::wstring externalFileName = currentDirectory.substr(0, currentDirectory.find_last_of(L"\\") + 1);
    externalFileName += L"RegistrationHelper.exe";

    // Construct file extensions
    unsigned int fileTypeCount = ARRAYSIZE(FileTypeExtensions);
    std::wstring extensions;
    for( unsigned int i = 0 ; i < fileTypeCount ; ++i )
    {
        extensions += FileTypeExtensions[i];
        extensions += L" ";
    }

    // Create command line parameter list
    // Quote the currentFileName and FriendlyName because they can have multiple words which are seperated by space
    std::wstring commandLine = L"TRUE " + ProgID + L" \"" + currentFileName + L"\" \"" + FriendlyName + L"\" " + AppUserModelID + L" " + extensions;
    SHELLEXECUTEINFO shellExcuteInfo;

    // Initialize startup and process info structures
    ZeroMemory(&shellExcuteInfo, sizeof( shellExcuteInfo) );

    shellExcuteInfo.cbSize         = sizeof( SHELLEXECUTEINFO );
    shellExcuteInfo.fMask          = 0;
    shellExcuteInfo.hwnd           = 0;
    shellExcuteInfo.lpVerb         = L"runas"; // Require elevated privilege
    shellExcuteInfo.lpFile         = externalFileName.c_str();
    shellExcuteInfo.lpParameters   = commandLine.c_str();
    shellExcuteInfo.lpDirectory    = L".";
    shellExcuteInfo.nShow          = SW_NORMAL;

    return ::ShellExecuteEx( &shellExcuteInfo ) == 0;
}
