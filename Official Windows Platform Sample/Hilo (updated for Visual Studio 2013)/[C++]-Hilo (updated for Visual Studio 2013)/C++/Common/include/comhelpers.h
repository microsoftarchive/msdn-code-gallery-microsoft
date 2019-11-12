//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "ComPtr.h"

template<class T>
HRESULT AssignToOutputPointer(T** pp, const ComPtr<T> &p)
{
    assert(pp);
    *pp = p;
    if ( nullptr != (*pp) )
    {
        (*pp)->AddRef();
    }

    return S_OK;
}
