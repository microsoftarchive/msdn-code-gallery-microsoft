//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "Direct2DUtility.h"
#include "WindowMessageHandler.h"

using namespace Hilo::WindowApiHelpers;

namespace Hilo 
{
    namespace WindowApiHelpers
    {
        // 
        // Serves as the default implementation for IWindowMessageHandler 
        // and is the base class for all window message handlers.
        // To receive a specific message, a subclass should override the 
        // corresponding OnXxx() message handler.
        //
        // In general handlers should return success codes (S_OK), if
        // it succeeds in processing the message. For a few handlers, such
        // as mouse and keyboard, an alternate success code (S_FALSE)
        // will signal to the calling method that additional processing
        // is needed, for examples send the mouse message to the parent 
        // window. Returning the error code E_NOTIMPL, will indicate
        // that this message handler has been processed at, which is
        // the default for all message handlers.
        //
        // If additional message handlers are needed, they should 
        // simply be added to this class. And the corresponding
        // logic added in OnMessageReceived() implementation. 
        //
        class WindowMessageHandler : public IWindowMessageHandler
        {
        public:
            HRESULT __stdcall GetWindow(__out IWindow** window);

        protected:
            WindowMessageHandler();
            ~WindowMessageHandler();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IWindowMessageHandler>::CastTo(iid, this, object);
            }

            HRESULT __stdcall OnMessageReceived(IWindow* window, unsigned int message, WPARAM wParam, LPARAM lParam, LRESULT* result);

            // Window message handlers, to be overriden by subclasses as needed.

            //
            // Mouse message handlers
            //
            // Mouse messages are sent to the child window first. A handler 
            // returning S_FALSE signals that the message should be sent 
            // to the parent window as well
            //
            // Important: Mouse positions are adjusted to the current DPI 
            // for quick and easy processing by a Direct2D app.
            // For example absolute pixel coordinate { 75, 90 } on 144 dpi monitor
            // are translated to { 50.0, 60.0 }
            // If this is not needed, then alternate or additional handler methods 
            // could be added and OnMessageReceived() method updated. 
            // 
            virtual HRESULT OnLeftMouseButtonDown(D2D1_POINT_2F /*mousePosition*/) { return E_NOTIMPL; }
            virtual HRESULT OnLeftMouseButtonUp(D2D1_POINT_2F /*mousePosition*/) { return E_NOTIMPL; }
            virtual HRESULT OnLeftMouseButtonDoubleClick(D2D1_POINT_2F /*mousePosition*/) { return E_NOTIMPL; }
            virtual HRESULT OnMouseMove(D2D1_POINT_2F /*mousePosition*/) { return E_NOTIMPL; }
            virtual HRESULT OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short /*delta*/, int /*keys*/) {  return E_NOTIMPL; }
            virtual HRESULT OnMouseEnter(D2D1_POINT_2F /*mousePosition*/) {  return E_NOTIMPL; }
            virtual HRESULT OnMouseLeave() {  return E_NOTIMPL; }

            // Rendering handlers

            // Called when when the window background must be erased (for example, when a window is resized).
            virtual HRESULT OnEraseBackground() { return E_NOTIMPL; }

            // Called when the system or another application makes a request to paint the window,
            // or when the display resolution changes, so rendering will be required.
            virtual HRESULT OnRender() { return E_NOTIMPL; }

            // Called after a window size has changed
            virtual HRESULT OnSize(unsigned int /*width*/, unsigned int /*height*/) { return E_NOTIMPL; }

            // Window lifetime handlers
            //
            // The OnCreate will be called when the window is first created.
            // This message is sent "before" the IWindowFactory::Create()
            // method returns, so it will not be called if the message
            // handler is not provided to the Create() method.
            // Note, if this method fails with an error code other 
            // than E_NOTIMPL, the window will be destroyed.
            virtual HRESULT OnCreate() { return E_NOTIMPL; }

            // Called to signal that a window is being destroyed
            virtual HRESULT OnDestroy() { return E_NOTIMPL; }

            // Called to signal that a window should terminate
            virtual HRESULT OnClose() { return E_NOTIMPL; }
            
            // Called when the mouse causes the cursor to move 
            // within a window and mouse input is not captured. 
            virtual HRESULT OnSetCursor() { return E_NOTIMPL; }

            // Keyboard handlers
            //
            // The key is sent to the window with keyboard focus first. 
            // A handler returning S_FALSE will signal 
            // that the message should be sent to a child window
            // currently at the mouse cursor.
            //
            virtual HRESULT OnKeyDown(unsigned int /*vKey*/) { return E_NOTIMPL; }
            virtual HRESULT OnKeyUp() { return E_NOTIMPL; }

            // AppCommand message handlers
            virtual HRESULT OnAppCommandBrowserBackward() { return E_NOTIMPL; }
            virtual HRESULT OnAppCommandBrowserForward() { return E_NOTIMPL; }

            // Touch specific message handlers
            virtual HRESULT OnPan(D2D1_POINT_2F /*panLocation*/, unsigned long /*flags*/) { return E_NOTIMPL; }
            virtual HRESULT OnZoom(float /*zoomFactor*/) { return E_NOTIMPL; }

            // Handle WM_COMMAND message
            virtual HRESULT OnCommand(WPARAM /*wParam*/, LPARAM /*lParam*/)  { return E_NOTIMPL; }
        private:
            // Called when the parent of the current 
            // window need to be notified as well
            bool NotifyParent(IWindow* window, unsigned int message, WPARAM wParam, LPARAM lParam);

            // Called when the child at the current mouse pos 
            // need to be notified as well
            LRESULT NotifyChild(IWindow* window, unsigned int message, WPARAM wParam, LPARAM lParam);

        private:
            // The window receiving the messages
            // Assigned a value when the window is first created,
            // before OnCreate() is called.
            ComPtr<IWindow> m_window;

            // Used to track if mouse cusror in window
            bool m_isMouseCursorInWindow;
        };
    }
}
