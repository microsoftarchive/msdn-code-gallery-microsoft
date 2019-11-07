//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "WindowLayout.h"
#include "WindowApplicationImpl.h"

class BrowserApplication : public WindowApplication
{
    unsigned int m_carouselPaneIndex;
    unsigned int m_mediaPaneIndex;
private:
    ComPtr<IShellItem> m_currentBrowseLocationItem;

    HRESULT InitializeCarouselPane(Hilo::WindowApiHelpers::IWindow** window);
    HRESULT InitializeMediaPane(Hilo::WindowApiHelpers::IWindow** window);

protected:
    ComPtr<IWindowLayout> m_WindowLayout;

    // Messages
    HRESULT OnSize(unsigned int /*width*/, unsigned int /*height*/);
    HRESULT OnKeyDown(unsigned int vKey);
    HRESULT OnMouseWheel(D2D1_POINT_2F mousePosition, short delta, int keys);
    HRESULT OnDestroy();

    BrowserApplication();
    ~BrowserApplication();

public:

    HRESULT __stdcall Initialize();
};
