//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once
using namespace Hilo::WindowApiHelpers;

//
// The interface representing a standard window application.
// A window application will have at least one top level window (the main window),
// will provide access t oa window factory, to allow creating other windows and
// will run a message loop to receive Windows messages.
//
[uuid("A624D983-3728-46A9-91B4-3F08FE244D7E")]
__interface IWindowApplication : IUnknown
{
    HRESULT __stdcall GetMainWindow(__out IWindow** mainWindow);
    HRESULT __stdcall SetMainWindow(__in IWindow* mainWindow);
    HRESULT __stdcall GetWindowFactory(__out IWindowFactory** windowFactory);
    HRESULT __stdcall SetWindowFactory(__in IWindowFactory* windowFactory);
    HRESULT __stdcall RunMessageLoop();
};
