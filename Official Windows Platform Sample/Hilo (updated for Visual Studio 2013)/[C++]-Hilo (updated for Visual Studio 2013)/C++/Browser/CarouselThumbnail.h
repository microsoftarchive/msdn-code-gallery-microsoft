//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "StdAfx.h"
#include "ThumbnailImpl.h"
#include "ThumbnailControl.h"
#include "AsyncLoader\AsyncLoaderInterfaces.h"

class CarouselThumbnail : public IInitializable, public Thumbnail
{
public:
    // Methods
    HRESULT __stdcall Draw();

protected:
    // Constructor/Destructor
    CarouselThumbnail(ThumbnailInfo thumbnailInfo);
    virtual ~CarouselThumbnail();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Thumbnail::QueryInterfaceHelper(iid, object);
    }
    
    HRESULT __stdcall Initialize();

private:
    // Methods
    HRESULT LoadBitmapFromShellItem(unsigned int thumbnailSize, ID2D1Bitmap** bitmap);
    HRESULT CreateBitmap(ID2D1Bitmap** bitmap);
    HRESULT CreateTextLayout();
};
