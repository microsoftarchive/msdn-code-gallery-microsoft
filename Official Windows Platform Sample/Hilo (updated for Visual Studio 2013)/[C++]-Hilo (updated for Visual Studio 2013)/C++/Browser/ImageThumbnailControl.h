//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "ThumbnailImpl.h"

class ImageThumbnail : public IInitializable, public Thumbnail
{
public:
    static const D2D1_COLOR_F SelectionBorderColor;

    // Rendering method
    HRESULT _stdcall Draw();

protected:
    // Constructor/destructor
    ImageThumbnail(ThumbnailInfo);
    virtual ~ImageThumbnail();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Thumbnail::QueryInterfaceHelper(iid, object);
    }
    
    HRESULT __stdcall Initialize();

private:
    // Draw methods
    HRESULT CreateBitmap(ID2D1Bitmap** bitmap);
    HRESULT CreateFullBitmap(ID2D1Bitmap** bitmap);
    HRESULT CreateThumbnailBitmap(ID2D1Bitmap** bitmap);
    HRESULT LoadBitmapFromShellItem(unsigned int thumbnailSize, ID2D1Bitmap** bitmap);
    
    // Layout methods
    HRESULT CreateTextLayout();
    D2D1_RECT_F CalculateBitmapRect(ID2D1Bitmap* bitmap);

    // Rendering methods
    void DrawSelectionBorder();
    void DrawShadow(D2D1_RECT_F bitmapRect);
    void DrawText();
};
