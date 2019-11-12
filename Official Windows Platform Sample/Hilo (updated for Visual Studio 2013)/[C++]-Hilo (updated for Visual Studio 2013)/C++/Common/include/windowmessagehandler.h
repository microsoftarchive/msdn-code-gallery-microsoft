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
        __interface IWindow;

        //
        // The interface representing the message handler for a window.
        // The main method in this interface, OnMessageReceived, is
        // called everytime a window messge is received by the
        // window owning this message handler.
        //
        [uuid("4459495C-350B-4959-915B-190915CFB15C")]
        __interface IWindowMessageHandler : IUnknown
        {
            HRESULT __stdcall OnMessageReceived(__in IWindow* window, __in unsigned int message, __in WPARAM wParam, __in LPARAM lParam, __out LRESULT* result);
            HRESULT __stdcall GetWindow(__out IWindow** window);
        };

        /// <summary>
        /// The interface used by a child window to notify a parent window of some change.
        /// </summary>
        [uuid("B2AA7B70-4560-4BCD-BCA4-D8DE858FE6AA")]
        __interface IChildNotificationHandler : public IUnknown
        {
            HRESULT __stdcall OnChildChanged();
        };
    }
}