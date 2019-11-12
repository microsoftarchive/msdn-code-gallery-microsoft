//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "BrowserApplication.h"
#include "CarouselPane.h"
#include "MediaPane.h"
#include "resource.h"
#include "JumpList.h"

using namespace Hilo::WindowApiHelpers;
using namespace Hilo::Direct2DHelpers;

// Application Id for taskbar
const wchar_t* const AppUserModeId = L"Microsoft.Hilo.Browser";

//
// Constructor
//
BrowserApplication::BrowserApplication()
{
}

//
// Destructor
//
BrowserApplication::~BrowserApplication()
{
}

//
// Initialize the application layout and message handler
//
HRESULT BrowserApplication::Initialize()
{
    // Set application Id
    HRESULT hr = SetCurrentProcessExplicitAppUserModelID(AppUserModeId);
    if (SUCCEEDED(hr))
    {
        // Create main window...
        hr = WindowApplication::Initialize();
    }
    
    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);
    }

    if (SUCCEEDED(hr))
    {
        static const int MaxTitleLength = 100;
        wchar_t title[MaxTitleLength] = {0}; // The title bar text
        ::LoadString(HINST_THISCOMPONENT, IDS_APP_TITLE, title, MaxTitleLength);

        hr = applicationWindow->SetTitle(title);
    }

    if (SUCCEEDED(hr))
    {
        // Set the large icon
        hr = applicationWindow->SetLargeIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_BROWSER)));
    }

    if (SUCCEEDED(hr))
    {
        // Set the small icon
        hr = applicationWindow->SetSmallIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_SMALL)));
    }

    // Create child windows...
    ComPtr<IWindow> carouselPaneWindow;
    if (SUCCEEDED(hr))
    {
        hr = InitializeCarouselPane(&carouselPaneWindow);
    }

    ComPtr<IWindow> mediaPaneWindow;
    if (SUCCEEDED(hr))
    {
        hr = InitializeMediaPane(&mediaPaneWindow);
    }

    // Connect the carousel pane with the media pane...
    ComPtr<IWindowMessageHandler> mediaPaneHandler;
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneWindow->GetMessageHandler(&mediaPaneHandler);
    }

    ComPtr<IWindowMessageHandler> carouselPaneHandler;
    if (SUCCEEDED(hr))
    {
        hr = carouselPaneWindow->GetMessageHandler(&carouselPaneHandler);
    }

    ComPtr<IPane> mediaPane;
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneHandler.QueryInterface(&mediaPane);
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<ICarouselPaneWindow> carouselPane;
        hr = carouselPaneHandler.QueryInterface(&carouselPane);

        if (SUCCEEDED(hr))
        {
            hr = carouselPane->SetMediaPane(mediaPane);
        }
    }

    // Create and initialize window layout...
    if (SUCCEEDED(hr))
    {
        hr = SharedObject<WindowLayout>::Create(&m_WindowLayout);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->SetMainWindow(applicationWindow);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->InsertChildWindow(carouselPaneWindow, &m_carouselPaneIndex);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->InsertChildWindow(mediaPaneWindow, &m_mediaPaneIndex);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->UpdateLayout();
    }

    // Set the current location...
    if (SUCCEEDED(hr))
    {
        // No location has been defined yet, so we just load the pictures library
        if (nullptr == m_currentBrowseLocationItem) 
        {
            // Defualt to Libraris library
            hr = ::SHCreateItemInKnownFolder(FOLDERID_PicturesLibrary, 0, nullptr, IID_PPV_ARGS(&m_currentBrowseLocationItem));
        }

        // If the Pictures Library was not not found
        if (FAILED(hr))
        {
            // Try obtaining the "Computer" known folder
            hr = ::SHGetKnownFolderItem(FOLDERID_ComputerFolder, static_cast<KNOWN_FOLDER_FLAG>(0), nullptr, IID_PPV_ARGS(&m_currentBrowseLocationItem));
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->UpdateLayout();
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IPane> carouselPane;
        hr = carouselPaneHandler.QueryInterface(&carouselPane);

        if (SUCCEEDED(hr))
        {
            hr = carouselPane->SetCurrentLocation(m_currentBrowseLocationItem, false);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Create a task in the Task Category of the jump list to launch annotator.exe application
        // Get the path of target exe
        wchar_t currentFileName[FILENAME_MAX];

        if (!GetModuleFileName(nullptr, currentFileName, FILENAME_MAX))
        {
            return S_OK;
        }

        // Annotator should be found in the same directory as this binary
        std::wstring currentDirectory = currentFileName;
        std::wstring externalFileName = currentDirectory.substr(0, currentDirectory.find_last_of(L"\\") + 1);
        externalFileName += L"annotator.exe";

        JumpList jumpList(AppUserModeId);
        hr = jumpList.AddUserTask(externalFileName.c_str(), nullptr, L"Launch Annotator");
    }

    return hr;
}

//
// Initialize the carousel pane
//
HRESULT BrowserApplication::InitializeCarouselPane(IWindow** window)
{
    static POINT location = {0, 0};
    static SIZE size =
    {
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(800)), 
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(300))
    };

    ComPtr<IWindowMessageHandler> carouselPaneMessageHandler;
    HRESULT hr = SharedObject<CarouselPaneMessageHandler>::Create(&carouselPaneMessageHandler);

    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);
    }

    ComPtr<IWindowFactory> windowFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWindowFactory(&windowFactory);
    }

    ComPtr<IWindow> carouselPane;
    if (SUCCEEDED(hr))
    {
        hr = windowFactory->Create(
            location,
            size,
            carouselPaneMessageHandler,
            applicationWindow,
            &carouselPane);
    }

    if (SUCCEEDED(hr))
    {
        hr = carouselPane->SetMessageHandler(carouselPaneMessageHandler);
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(window, carouselPane);
    }

    return hr;
}

//
// Initialize the media pane
//
HRESULT BrowserApplication::InitializeMediaPane(IWindow** window)
{
    static POINT location = {0, 0};
    static SIZE size = 
    {
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(800)), 
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(300))
    };

    ComPtr<IWindowMessageHandler> mediaPaneMessageHandler;
    HRESULT hr = SharedObject<MediaPaneMessageHandler>::Create(&mediaPaneMessageHandler);

    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);
    }

    ComPtr<IWindowFactory> windowFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWindowFactory(&windowFactory);
    }

    ComPtr<IWindow> mediaPane;
    if (SUCCEEDED(hr))
    {
        hr = windowFactory->Create(
            location,
            size,
            mediaPaneMessageHandler,
            applicationWindow,
            &mediaPane);
    }

    if (SUCCEEDED(hr))
    {
        hr = mediaPane->SetMessageHandler(mediaPaneMessageHandler);
    }

    // Enable touch gestures for media pane
    static const unsigned long enable = GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
    static const unsigned long disable = GC_PAN_WITH_SINGLE_FINGER_VERTICALLY | GC_PAN_WITH_GUTTER;

    GESTURECONFIG config [] = 
    {
        { GID_PAN, enable, disable },
        { GID_ZOOM, GC_ZOOM, 0}
    };

    if (SUCCEEDED(hr))
    {
        HWND hWnd;
        hr = mediaPane->GetWindowHandle(&hWnd);

        if (SUCCEEDED(hr))
        {
            // Specify which touch gestures to handle based on the gesture configuration. This can also be 
            // called during the WM_NOTIFY message if an application wanted to dynamically update
            // which gestures to support based on application state
            ::SetGestureConfig(hWnd, 0, ARRAYSIZE(config), config, sizeof(GESTURECONFIG));
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(window, mediaPane);
    }

    return hr;
}

//
// Called when the main window is resized
//
HRESULT BrowserApplication::OnSize(unsigned int /*width*/, unsigned int /*height*/)
{
    if (nullptr != m_WindowLayout)
    {
        m_WindowLayout->UpdateLayout();
    }

    return S_OK;
}

//
// Called when a key on the keyboard is pressed
//
HRESULT BrowserApplication::OnKeyDown(unsigned int /*vKey*/)
{
    // Make sure to notify children
    return S_FALSE;
}

//
// Called when the mousewheel is moved
//
HRESULT BrowserApplication::OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short /*delta*/, int /*keys*/) 
{
    // Make sure to notify children
    return S_FALSE;
}

//
// Called when the window is about to be destroyed
//
HRESULT BrowserApplication::OnDestroy()
{
    m_WindowLayout->Finalize();

    return WindowApplication::OnDestroy();
}