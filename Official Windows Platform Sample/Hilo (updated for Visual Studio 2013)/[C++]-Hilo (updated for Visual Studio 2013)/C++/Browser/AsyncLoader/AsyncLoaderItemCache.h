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
#include "AsyncLoaderInterfaces.h"

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class AsyncLoaderItemCache
        //
        // This is a helper class for a client item that is to be loaded/unloaded
        // by the AsyncLoader.
        //
        // Its goal is to keep track of a primary and secondary objects, and encapsulate
        // the logic behind which one is used by the client item, which is unloaded, etc.
        //
        class AsyncLoaderItemCache : public IAsyncLoaderItemCache
        {
            CRITICAL_SECTION* m_criticalSection;

            // Primary object
            ComPtr<IUnknown>  m_primaryObject;
            unsigned int      m_primaryObjectSize;   // Size is in client units.

            // Secondary object
            ComPtr<IUnknown>  m_secondaryObject;
            unsigned int      m_secondaryObjectSize; // Size is in client units.

            ComPtr<IAsyncLoaderItem>          m_loadableItem;   // The owner client item.
            ComPtr<IAsyncLoaderMemoryManager> m_memoryManager;  // The memory manager is needed
                                                                // to know at which size the item
                                                                // should be loaded.

            // Private helper methods:
            HRESULT Load(IAsyncLoaderItem* loadableItem, LoadPage loadPage, LoadSize loadSize, bool* set, IUnknown** newObject);

        protected:
            AsyncLoaderItemCache();
            virtual ~AsyncLoaderItemCache();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoaderItemCache>::CastTo(iid, this, object);
            }

        public:
            // IAsyncLoaderItemCache
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);
            HRESULT __stdcall SetLoadableItem(IAsyncLoaderItem* loadableItem);
            HRESULT __stdcall SetMemoryManager(IAsyncLoaderMemoryManager* memoryManager);
            HRESULT __stdcall Load(LoadPage loadPage, LoadSize loadSize);
            HRESULT __stdcall Unload(LoadSize loadSize);
            HRESULT __stdcall Shutdown();
        };
    }
}
