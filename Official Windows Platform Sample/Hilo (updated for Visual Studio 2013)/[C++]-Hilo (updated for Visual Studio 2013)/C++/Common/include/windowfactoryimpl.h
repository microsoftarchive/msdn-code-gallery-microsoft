//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "SharedObject.h"
#include "WindowFactory.h"

namespace Hilo 
{
    namespace WindowApiHelpers
    {
        // The default implementation of a window factory
        // providing methods to create windows using a number of
        // options and configurations.
        class WindowFactory : public IWindowFactory
        {
        protected:
            WindowFactory();
            virtual ~WindowFactory();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IWindowFactory>::CastTo(iid, this, object);
            }

        public:
            // IWindowFactory implementation
            HRESULT __stdcall Create(__in POINT location, __in SIZE size, __in_opt IWindow* parent, __out IWindow** window);
            HRESULT __stdcall Create(__in POINT location, __in SIZE size, __in_opt IWindowMessageHandler* messageHandler, __in_opt IWindow* parent, __out IWindow** window);

        protected:
            static LRESULT CALLBACK WndProc(HWND hWnd, unsigned int message, WPARAM wParam, LPARAM lParam);

        private:
            bool m_classRegistered;
            ATOM RegisterClass(HINSTANCE hInstance, const wchar_t* className);
        };
    }
}