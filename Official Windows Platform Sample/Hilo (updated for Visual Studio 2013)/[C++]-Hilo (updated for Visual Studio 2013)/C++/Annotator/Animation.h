//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "stdafx.h"

[uuid("F45A0AEF-698A-4F42-A6A0-653FED496EE6")]
__interface IPointAnimation : public IUnknown
{
public:
    HRESULT __stdcall GetCurrentPoint(__out D2D1_POINT_2F* point);
    HRESULT __stdcall Setup(D2D1_POINT_2F targetPoint, double duration);
};