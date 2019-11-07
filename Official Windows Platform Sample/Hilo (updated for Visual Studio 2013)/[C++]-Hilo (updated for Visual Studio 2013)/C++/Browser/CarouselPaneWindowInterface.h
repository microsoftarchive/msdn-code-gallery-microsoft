//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "PaneInterface.h"

[uuid("4263B0F3-AD44-4A85-BC40-38F5CA850DAA")]
__interface ICarouselPaneWindow : public IUnknown
{
    HRESULT __stdcall SetMediaPane(__in IPane* mediaPane);
};