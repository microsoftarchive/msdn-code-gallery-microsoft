//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

enum ImageOperationType
{
    ImageOperationTypeNone = 0,
    ImageOperationTypePen,
    ImageOperationTypeCrop,
    ImageOperationTypeRotateClockwise,
    ImageOperationTypeRotateCounterClockwise,
    ImageOperationTypeFlipHorizontal,
    ImageOperationTypeFlipVertical,
};

[uuid("8D4D6824-63E7-4BC9-A8E8-15ED1FD63F0E")]
__interface IImageOperation : public IUnknown
{
};
