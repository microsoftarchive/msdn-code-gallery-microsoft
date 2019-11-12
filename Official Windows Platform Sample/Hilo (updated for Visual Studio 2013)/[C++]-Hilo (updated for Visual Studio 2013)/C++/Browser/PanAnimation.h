//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "Animation.h"
#include "SharedObject.h"

class PanAnimation : public IInitializable, public IPanAnimation
{
public:
    HRESULT __stdcall GetValue(__out double *currentPosition);
    HRESULT __stdcall Setup(__in const double startPosition, __in const double endPosition, __in const double duration);

protected:
    // Constructor/Destructor
    PanAnimation();
    virtual ~PanAnimation();

    // QueryInterface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IInitializable>::CastTo(iid, this, object) ||
            CastHelper<IPanAnimation>::CastTo(iid, this, object);
    }

    // IInitializable implementation
    HRESULT __stdcall Initialize();

private:
    // Animation variables
    ComPtr<IUIAnimationVariable> m_panValue;
};