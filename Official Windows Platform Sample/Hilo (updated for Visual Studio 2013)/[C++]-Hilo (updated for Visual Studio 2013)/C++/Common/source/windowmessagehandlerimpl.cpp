//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"

#include "WindowMessageHandlerImpl.h"

using namespace Hilo::WindowApiHelpers;
using namespace Hilo::Direct2DHelpers;

WindowMessageHandler::WindowMessageHandler() : m_isMouseCursorInWindow(false)
{
}

WindowMessageHandler::~WindowMessageHandler()
{
}

HRESULT WindowMessageHandler::GetWindow(IWindow** window)
{
    return AssignToOutputPointer(window, m_window);
}

// 
// Called when a message is received by the window
// message loop
//
HRESULT WindowMessageHandler::OnMessageReceived(
    IWindow* window,
    unsigned int message,
    WPARAM wParam,
    LPARAM lParam,
    LRESULT* result)
{
    HRESULT hr = E_NOTIMPL;

    switch (message)
    {
    case WM_SIZE:
        {
            unsigned int width = LOWORD(lParam);
            unsigned int height = HIWORD(lParam);

            hr = OnSize(width, height);
            if (SUCCEEDED(hr))
            {
                *result = 0;
            }
            break;
        }

    case WM_ERASEBKGND:
        {
            hr = OnEraseBackground();
            if (SUCCEEDED(hr))
            {
                // The application should return nonzero if it 
                // erases the background;  otherwise, 
                // it should return zero. 
                *result = 1;
            }
            break;
        }
    case WM_DESTROY:
        {
            hr = OnDestroy();
            if (SUCCEEDED(hr))
            {
                *result = 0;
            }
            // If a window is being destroyed, make sure to release its message handler too.
            // Otherwise, the window itself might not be destructed if the handler has
            // a reference to it.
            window->SetMessageHandler(nullptr);

            break;
        }
    case WM_CLOSE:
        {
            hr = OnClose();
            if (SUCCEEDED(hr))
            {
                *result = 0;
            }

            break;
        }

    case WM_CREATE:
        {
            // Save the window
            m_window = window;

            hr = OnCreate();
            if (SUCCEEDED(hr) || hr == E_NOTIMPL)
            {
                *result = 0;
            }
            else // if the function fails, the window will be destroyed
            {
                *result = -1;
            }
            break;
        }

    case WM_PAINT:
    case WM_DISPLAYCHANGE:
        {
            HWND hWnd;

            hr = window->GetWindowHandle(&hWnd);
            if (SUCCEEDED(hr))
            {
                PAINTSTRUCT ps;
                BeginPaint(hWnd, &ps);
                hr = OnRender();
                EndPaint(hWnd, &ps);
            }

            if (SUCCEEDED(hr))
            {
                *result = 0;
            }
            break;
        }

    case WM_APPCOMMAND:
        {
            unsigned int cmd = GET_APPCOMMAND_LPARAM(lParam);
            if (cmd == APPCOMMAND_BROWSER_FORWARD)
            {
                hr = OnAppCommandBrowserForward();
            }
            else if (cmd == APPCOMMAND_BROWSER_BACKWARD)
            {
                hr = OnAppCommandBrowserBackward();
            }

            if (SUCCEEDED(hr))
            {
                *result = 1;
            }
            break;
        }
    case WM_COMMAND:
        {
            OnCommand(wParam, lParam);
            break;
        }
    case WM_LBUTTONDOWN:
        {
            hr = OnLeftMouseButtonDown(Direct2DUtility::GetMousePositionForCurrentDpi(lParam));
            if (SUCCEEDED(hr))
            {
                // We use S_FALSE to signal that the parent of 
                // this window should be notified as well
                if (hr == S_FALSE)
                {
                    NotifyParent(window, message, wParam, lParam);
                }

                *result = 0;
            }

            break;
        }

    case WM_LBUTTONDBLCLK :
        {
            hr = OnLeftMouseButtonDoubleClick(Direct2DUtility::GetMousePositionForCurrentDpi(lParam));
            if (SUCCEEDED(hr))
            {
                // We use S_FALSE to signal that the parent of 
                // this window should be notified as well
                if (hr == S_FALSE)
                {
                    NotifyParent(window, message, wParam, lParam);
                }

                *result = 0;
            }

            break;
        }

    case WM_LBUTTONUP:
        {
            hr = OnLeftMouseButtonUp(Direct2DUtility::GetMousePositionForCurrentDpi(lParam));
            if (SUCCEEDED(hr))
            {
                // We use S_FALSE to signal that the parent of 
                // this window should be notified as well
                if (hr == S_FALSE)
                {
                    NotifyParent(window, message, wParam, lParam);
                }

                *result = 0;
            }

            break;
        }

    case WM_MOUSEMOVE:
        {
            D2D1_POINT_2F mousePosition = Direct2DUtility::GetMousePositionForCurrentDpi(lParam);
            if (!m_isMouseCursorInWindow)
            {
                m_isMouseCursorInWindow = true;

                TRACKMOUSEEVENT trackMouseEvent = { sizeof(trackMouseEvent) };
                trackMouseEvent.dwFlags = TME_LEAVE;
                HWND hWnd;
                window->GetWindowHandle(&hWnd);
                trackMouseEvent.hwndTrack = hWnd;
                TrackMouseEvent(&trackMouseEvent);

                // Ignore the mouse enter return value because it's not used by Windows
                OnMouseEnter(mousePosition);
            }

            hr = OnMouseMove(mousePosition);
            if (SUCCEEDED(hr))
            {
                // A return value of S_FALSE signlas the need to notify parent window
                if (hr == S_FALSE)
                {
                    NotifyParent(window, message, wParam, lParam);
                }

                *result = 0;
            }

            break;
        }
    case WM_MOUSELEAVE:
        {
            m_isMouseCursorInWindow = false;

            hr = OnMouseLeave();
            if (SUCCEEDED(hr))
            {
                *result = 0;
            }
            break;
        }

    case WM_MOUSEWHEEL:
        {
            hr = OnMouseWheel(
                Direct2DUtility::GetMousePositionForCurrentDpi(lParam), 
                GET_WHEEL_DELTA_WPARAM(wParam), 
                GET_KEYSTATE_WPARAM(wParam));
            if (SUCCEEDED(hr))
            {
                // A return value of S_FALSE signals the need to notify parent window
                if (hr == S_FALSE)
                {
                    *result = NotifyChild(window, message, wParam, lParam);
                }
                else
                {
                    *result = 0;
                }
            }

            break;
        }

    case WM_GESTURE:
        {
            bool handled = false;

            GESTUREINFO info;
            info.cbSize = sizeof(info);
            if (::GetGestureInfo((HGESTUREINFO)lParam, &info))
            {
                switch(info.dwID)
                {
                case GID_PAN:
                    {
                        D2D1_POINT_2F panLocation = Direct2DUtility::GetPositionForCurrentDPI(info.ptsLocation);

                        hr = OnPan(panLocation, info.dwFlags);
                        if (SUCCEEDED(hr))
                        {
                            if (S_OK == hr)
                            {
                                handled = true;
                            }
                        }

                        break;
                    }

                case GID_ZOOM:
                    {
                        static double previousValue = 1;

                        switch(info.dwFlags)
                        {
                        case GF_BEGIN:
                            // Call the OnZoom handler to see if zoom has been implemented. If OnZoom returns any
                            // value other than S_OK, then this message will be passed on to the DefWindowsProc
                            // where it can be converted to a CTRL + MouseWheel message
                            hr = OnZoom(1.0f);
                            break;
                        case 0:
                            hr = OnZoom(static_cast<float>(LODWORD(info.ullArguments) / previousValue));
                            break;
                        }

                        if (SUCCEEDED(hr))
                        {
                            previousValue = LODWORD(info.ullArguments);

                            if (S_OK == hr)
                            {
                                handled = true;
                            }
                        }

                        break;
                    }
                }
            }

            if (handled)
            {
                ::CloseGestureInfoHandle((HGESTUREINFO)lParam);
                *result = 0;
            }
            else
            {
                *result = 1;
            }

            break;
        }

    case WM_KEYDOWN:
        {
            hr = OnKeyDown(static_cast<unsigned int>(wParam));
            if (SUCCEEDED(hr))
            {
                // A return value of S_FALSE signals the need to notify child windows
                if (hr == S_FALSE)
                {
                    *result = NotifyChild(window, message, wParam, lParam);
                }
                else
                {
                    *result = 0;
                }
            }

            break;
        }

    case WM_SETCURSOR:
        {
            hr = OnSetCursor();
            if (SUCCEEDED(hr))
            {
                *result = 1;
            }
            else
            {
                *result = 0;
            }

            break;
        }

    }

    return hr;
}

bool WindowMessageHandler::NotifyParent(
    IWindow* window,
    unsigned int message,
    WPARAM wParam,
    LPARAM lParam)
{
    HWND hWnd;
    BOOL result = FALSE;

    if (SUCCEEDED(window->GetWindowHandle(&hWnd)))
    {
        result = ::SendNotifyMessage(GetParent(hWnd), message, wParam, lParam);
    }

    return result != FALSE;
}


LRESULT WindowMessageHandler::NotifyChild(
    IWindow* window,
    unsigned int message,
    WPARAM wParam,
    LPARAM lParam)
{
    LRESULT result = 0;
    HRESULT hr = S_OK;
    POINT mousePos;

    if (::GetCursorPos(&mousePos))
    {
        HWND hWnd;
        HWND childWindow;

        hr = window->GetWindowHandle(&hWnd);
        if (SUCCEEDED(hr) && nullptr != hWnd)
        {
            childWindow = ::WindowFromPoint(mousePos);

            if (nullptr != childWindow && ::IsChild(hWnd, childWindow))
            {
                result = ::SendMessage(childWindow, message, wParam, lParam);
            }
        }
    }

    return result;
}
