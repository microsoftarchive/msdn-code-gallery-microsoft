//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "WindowApplicationImpl.h"

using namespace Hilo::Direct2DHelpers;

WindowApplication::WindowApplication()
{
}

WindowApplication::~WindowApplication()
{
    ::CoUninitialize();
}

// 
// Create the application main window and set its message handler
//
HRESULT WindowApplication::Initialize()
{
    HRESULT hr = ::CoInitialize(nullptr);

    if (SUCCEEDED(hr))
    {
        // Create a window factory to create main and child windows
        hr = CreateWindowFactory(&m_windowFactory);
    }

    if (SUCCEEDED(hr))
    {
        POINT location = {CW_USEDEFAULT, CW_USEDEFAULT};
        SIZE size = {CW_USEDEFAULT, CW_USEDEFAULT};

        hr = m_windowFactory->Create(location, size, this, nullptr, &m_applicationWindow);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_applicationWindow->Show(true);
    }

    return hr;
}


HRESULT WindowApplication::GetMainWindow(__out IWindow** mainWindow)
{
    assert(mainWindow);
    *mainWindow = nullptr;
    return AssignToOutputPointer(mainWindow, m_applicationWindow);
}

HRESULT WindowApplication::SetMainWindow(__in IWindow* mainWindow)
{
    m_applicationWindow = mainWindow;
    return S_OK;
}

HRESULT WindowApplication::GetWindowFactory(__out IWindowFactory** windowFactory)
{
    assert(windowFactory);
    *windowFactory = nullptr;
    return AssignToOutputPointer(windowFactory, m_windowFactory);
}

HRESULT WindowApplication::SetWindowFactory(__in IWindowFactory* windowFactory)
{
    m_windowFactory = windowFactory;
    return S_OK;
}

//
// Run the windows message pump to receive all
// messages arriving to this window.
//
HRESULT WindowApplication::RunMessageLoop()
{
    MSG message;

    while (::GetMessage(&message, nullptr, 0, 0))
    {
        ::TranslateMessage(&message);
        ::DispatchMessage(&message);
    }

    return S_OK;
}

HRESULT WindowApplication::OnEraseBackground()
{
    // Returning success will indicate this message has
    // been processed and ensure that the message will
    // not be re-routed to the default window procedure
    // (DefWndProc) by the base message handler class.
    return S_OK;
}

//
// Called when the WM_DESTROY message is received
//
HRESULT WindowApplication::OnDestroy()
{
    ::PostQuitMessage(0);
    return S_OK;
}