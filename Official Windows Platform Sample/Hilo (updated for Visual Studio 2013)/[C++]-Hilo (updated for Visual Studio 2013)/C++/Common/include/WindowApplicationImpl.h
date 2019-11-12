//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once
#include "ComPtr.h"
#include "WindowApplication.h"
#include "WindowFactory.h"
#include "WindowMessageHandlerImpl.h"

using namespace Hilo::WindowApiHelpers;

// 
// The base implementation of a default window application.
// A window application will create a single main window,
// and will act as the message handler for this main window.
// In addition, this class provides a method to run
// the message loop which will block until there are no more
// messages to be processed.
// Finally, each window application will create a IWindowFactory
// that could be used to create other windows as well.
// 
class WindowApplication : public IWindowApplication, public IInitializable, public WindowMessageHandler
{
private:
    ComPtr<IWindowFactory> m_windowFactory;
    ComPtr<IWindow> m_applicationWindow;

protected:
    // Messages
    virtual HRESULT OnEraseBackground();
    virtual HRESULT OnDestroy();

    WindowApplication();
    virtual ~WindowApplication();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IWindowApplication>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object) ||
            WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

public:
    // IInitializable implementation
    HRESULT __stdcall Initialize();

    // IWindowApplication implementation
    HRESULT __stdcall GetMainWindow(__out IWindow** mainWindow);
    HRESULT __stdcall SetMainWindow(__in IWindow* mainWindow);
    HRESULT __stdcall GetWindowFactory(__out IWindowFactory** windowFactory);
    HRESULT __stdcall SetWindowFactory(__in IWindowFactory* windowFactory);
    HRESULT __stdcall RunMessageLoop();
};
