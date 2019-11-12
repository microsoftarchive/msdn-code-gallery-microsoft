//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "AsyncLoaderInterfaces.h"

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class MemorySizeConverter
        //
        // Helper classes to convert between image dimensions in pixels and amount
        // of memory need for its storage.
        //
        class MemorySizeConverter : public IMemorySizeConverter
        {
        protected:
            // constructor...
            MemorySizeConverter();

            virtual ~MemorySizeConverter();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IMemorySizeConverter>::CastTo(iid, this, object);
            }

        public:
            // IMemorySizeConverter
            HRESULT __stdcall MemorySizeToClientItemSize(unsigned int memorySize, unsigned int* clientItemSize);
            HRESULT __stdcall ClientItemSizeToMemorySize(unsigned int clientItemSize, unsigned int* memorySize);
        };
    }
}
