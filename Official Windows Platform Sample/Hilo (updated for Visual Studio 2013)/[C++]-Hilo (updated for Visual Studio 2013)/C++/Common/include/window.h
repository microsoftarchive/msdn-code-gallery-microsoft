//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

namespace Hilo 
{
    namespace WindowApiHelpers
    {
        //
        // Specifies a placement in the Z order relative
        //
        enum ZOrderPlacement
        {
            Top = 0,         // HWND_TOP = ((HWND)0)
            Bottom = 1,      // HWND_BOTTOM = (HWND)1)
            TopMost = -1,    // HWND_TOPMOST = (HWND)-1)
            NonTopMost = -2, // HWND_NOTOPMOST = ((HWND)-2)
        };

        __interface IWindowMessageHandler;

        //
        // The interface representing a window object.
        // Providing functions to manipulate a window such as 
        // showing/hiding the window, settings its message handler, 
        // setting the title or icon or setting its size and/or 
        // position.
        //
        [uuid("FEC0E939-41BF-46E3-AA13-A6E66A8CA2F2")]
        __interface IWindow : public IUnknown
        {
            HRESULT __stdcall Show(__in bool isVisible);
            HRESULT __stdcall RedrawWindow();
            HRESULT __stdcall RedrawWindow(__in bool eraseBackground);
            HRESULT __stdcall UpdateWindow();
            HRESULT __stdcall Close();
            HRESULT __stdcall GetTitle(__out std::wstring* title);
            HRESULT __stdcall SetTitle(__in const std::wstring& title);
            HRESULT __stdcall GetLargeIcon(__out HICON* icon);
            HRESULT __stdcall SetLargeIcon(__in HICON icon);
            HRESULT __stdcall GetSmallIcon(__out HICON* icon);
            HRESULT __stdcall SetSmallIcon(__in HICON icon);
            HRESULT __stdcall GetSize(__out unsigned int* pixelWidth, __out unsigned int* pixelHeight);
            HRESULT __stdcall SetSize(__in unsigned int pixelWidth, __in unsigned int pixelHeight);
            HRESULT __stdcall SetPosition(__in unsigned int posX, __in unsigned int posY);
            HRESULT __stdcall GetRect(__out RECT* rect);
            HRESULT __stdcall GetParentWindowRect(__out RECT* rect);
            HRESULT __stdcall SetRect(__in RECT rect);
            HRESULT __stdcall SetZOrder(__in Hilo::WindowApiHelpers::IWindow *windowInsertAfter);
            HRESULT __stdcall SetZOrder(__in ZOrderPlacement placement);
            HRESULT __stdcall GetWindowHandle(__out HWND* hWnd);
            HRESULT __stdcall GetParentWindowHandle(__out HWND* hWnd);
            HRESULT __stdcall SetWindowHandle(__in HWND hWnd);
            HRESULT __stdcall GetMessageHandler(__out IWindowMessageHandler** messageHandler);
            HRESULT __stdcall SetMessageHandler(__in IWindowMessageHandler* messageHandler);
            HRESULT __stdcall GetClientRect(__out RECT* clientRect);
            HRESULT __stdcall SetCapture();
            HRESULT __stdcall SetFocus();
            HRESULT __stdcall IsMouseCaptured(__out bool* isMouseCaptured);
        };
    }
}
