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
        // Forward declarations
        __interface IWindow;
        __interface IWindowMessageHandler;

        //
        // The interface representing a window factory.
        // A window factory provides methods to create windows.
        //
        [uuid("B2F92698-7D01-4FEB-B6D0-EEB3C9042FEA")]
        __interface IWindowFactory : IUnknown
        {
            HRESULT __stdcall Create(
                __in POINT location,
                __in SIZE size,
                __in_opt IWindow* parent,
                __out IWindow** window);

            HRESULT __stdcall Create(
                __in POINT location,
                __in SIZE size,
                __in_opt IWindowMessageHandler* messageHandler,
                __in_opt IWindow* parent,
                __out IWindow** window);
        };

        extern HRESULT CreateWindowFactory(__out IWindowFactory** factory);
    }
}