//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include <Windows.h>
#include "WindowLayoutInterface.h"

//
// Represents a child window interacting with 
// a window layout object
//
[uuid("34DA09D9-42D3-464B-9CCD-12BE17267910")]
__interface IWindowLayoutChild : public IUnknown
{
    HRESULT __stdcall SetWindowLayout(__in IWindowLayout* windowLayout);
    HRESULT __stdcall Finalize();
};