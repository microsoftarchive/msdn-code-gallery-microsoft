//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once
#include "ImageOperation.h"

[uuid("34DA09D9-42D3-464B-9CCD-12BE17267910")]
__interface IImageEditor : public IUnknown
{
    HRESULT __stdcall SetCurrentLocation(__in IShellItem* shellFolder, __in bool isSingleImage);
    HRESULT __stdcall SetCurrentLocationFromCommandLine();
    HRESULT __stdcall SetDrawingOperation(__in ImageOperationType imageDrawingOperation);
    HRESULT __stdcall GetDrawingOperation(__out ImageOperationType* imageDrawingOperation);
    HRESULT __stdcall SetUIFramework(__in IUIFramework* framework);
    HRESULT __stdcall UpdateUIFramework();
    
    // Ink settings
    HRESULT __stdcall SetPenColor(__in D2D1_COLOR_F penColor);
    HRESULT __stdcall SetPenSize(__in float penSize);

    // Save/Open files
    HRESULT __stdcall OpenFile();
    HRESULT __stdcall SaveFiles();
    HRESULT __stdcall SaveFileAs();

    // Undo/Redo operations
    HRESULT __stdcall CanUndo(__out bool* canUndo);
    HRESULT __stdcall CanRedo(__out bool* canRedo);
    HRESULT __stdcall Undo();
    HRESULT __stdcall Redo();

    // View zoom
    HRESULT __stdcall ZoomIn();
    HRESULT __stdcall ZoomOut();
    HRESULT __stdcall ZoomFull();
};

