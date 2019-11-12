//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "WindowFactoryImpl.h"
#include "WindowImpl.h"

using namespace Hilo::WindowApiHelpers;

WindowFactory::WindowFactory() : m_classRegistered(false)
{
}

WindowFactory::~WindowFactory()
{
}

ATOM WindowFactory::RegisterClass(HINSTANCE hInstance, const wchar_t* className)
{
    WNDCLASSEX wcex = {0};

    wcex.cbSize         = sizeof(WNDCLASSEX);
    wcex.style          = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS;
    wcex.lpfnWndProc    = WindowFactory::WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = sizeof(LONG_PTR);
    wcex.hInstance      = hInstance;
    wcex.hIcon          = nullptr;
    wcex.hIconSm        = nullptr;
    wcex.hCursor        = nullptr; // If the class cursor is not null, the system restores the class cursor each time the mouse is moved.
    wcex.hbrBackground  = (HBRUSH) (COLOR_WINDOW+1);
    wcex.lpszMenuName   = nullptr;
    wcex.lpszClassName  = className;

    return ::RegisterClassEx(&wcex);
}

HRESULT WindowFactory::Create(
    POINT location,
    SIZE size,
    IWindow* parent,
    IWindow** window)
{
    return Create(location, size, nullptr, parent, window);
}

HRESULT WindowFactory::Create(
    POINT location,
    SIZE size,
    IWindowMessageHandler* messageHandler,
    IWindow* parent,
    IWindow** window)
{
    static const wchar_t className [] = L"HiloWindowClass";

    assert(window);
    *window = nullptr;

    if (!m_classRegistered)
    {
        if (0 != RegisterClass(HINST_THISCOMPONENT, className))
        {
            m_classRegistered = true;
        }
        else
        {
            // Could not register the Window class
            return HRESULT_FROM_WIN32(::GetLastError());
        }
    
    }

    HWND parentHWnd = nullptr;
    if (nullptr != parent)
    {
        parent->GetWindowHandle(&parentHWnd);
    }

    ComPtr<IWindow> windowPtr;
    HRESULT hr = SharedObject<Window>::Create(&windowPtr);
    if (FAILED(hr))
    {
        return hr;
    }

    if (messageHandler)
    {
        windowPtr->SetMessageHandler(messageHandler);
    }

    HWND windowHandle = ::CreateWindow(
        className,
        nullptr, // Title is initially empty
        nullptr == parent? WS_OVERLAPPEDWINDOW : WS_CHILD | WS_VISIBLE ,
        location.x,
        location.y,
        size.cx,
        size.cy,
        parentHWnd,
        nullptr,
        HINST_THISCOMPONENT,
        windowPtr);

    if (nullptr != windowHandle)
    {
        hr = AssignToOutputPointer(window, windowPtr);
    }
    else
    {
        hr = HRESULT_FROM_WIN32(::GetLastError());
    }

    return hr;
}

//
// Static WndProc function
// This function retrieves the windows message from the message queue, 
// and sends it to a IWindowMessageHandler of the owner IWindow, if both
// could be obtained. Otherwise, the DefWindowProc will be called.
// 
LRESULT CALLBACK WindowFactory::WndProc(HWND hWnd, unsigned int message, WPARAM wParam, LPARAM lParam)
{
    LRESULT result = 0;
    ComPtr<IWindow> window;
    ComPtr<IWindowMessageHandler> handler;

    // The WM_NCCREATE message is sent prior to the 
    // WM_CREATE message when a window is first created.
    if (message == WM_NCCREATE)
    {
        LPCREATESTRUCT pcs = (LPCREATESTRUCT)lParam;
        window = reinterpret_cast<IWindow*>(pcs->lpCreateParams);
        window->SetWindowHandle(hWnd);

        ::SetWindowLongPtrW(hWnd, GWLP_USERDATA, PtrToUlong(window.GetInterface()));
    }
    else
    {
        IWindow* windowPtr = reinterpret_cast<IWindow*>(::GetWindowLongPtrW(hWnd, GWLP_USERDATA));
        if (windowPtr)
        {
            window = dynamic_cast<IWindow*>(windowPtr);
        }
    }

    if (window)
    {
        // Try to get handler
        window->GetMessageHandler(&handler);
    }

    // Check if we were able to obtain the window and the message handler. 
    // If both are available call the handler's OnMessageReceived.
    // Otherwise, call the default WndProc instead.
    if (!window || !handler || FAILED(handler->OnMessageReceived(window, message, wParam, lParam, &result)))
    {
        result = ::DefWindowProc(hWnd, message, wParam, lParam);
    }

    return result;
}

HRESULT Hilo::WindowApiHelpers::CreateWindowFactory(IWindowFactory** factory)
{
    assert(factory);
    *factory = nullptr;

    static ComPtr<IWindowFactory> windowFactory;
    HRESULT hr = S_OK;

    if (nullptr == windowFactory)
    {
        hr = SharedObject<WindowFactory>::Create(&windowFactory);
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, windowFactory);
    }

    return hr;
}
