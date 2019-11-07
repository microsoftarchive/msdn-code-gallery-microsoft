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
#include "ComPtr.h"
#include <vector>

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class LoadableItemList
        //
        // List of items for the AsyncLoader to loop through and call unload/load.
        // It encapsulates:
        // - Thread-safe access to the list of items.
        // - Ability to notify the layout manager when items are added or removed.
        // - Ability to enumerate items.
        // - Ability to integrate passes into its enumeration.
        //
        class LoadableItemList : public IAsyncLoaderItemList
        {
            // Synchronization...
            CRITICAL_SECTION* m_criticalSection;

            // Layout manager...
            ComPtr<IAsyncLoaderLayoutManager> m_layoutManager;

            // Items...
            std::vector<ComPtr<IAsyncLoaderItem>> m_loadableItems;
            size_t m_currentItem;
            unsigned int   m_passCount;
            unsigned int   m_currentPass;
            bool   m_resetOnAdd;

        protected:
            // constructor
            LoadableItemList();

            virtual ~LoadableItemList();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoaderItemList>::CastTo(iid, this, object);
            }

        public:
            // IAsyncLoaderItemList
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* _cs);
            HRESULT __stdcall SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager);
            HRESULT __stdcall Add(IAsyncLoaderItem* _p);
            HRESULT __stdcall GetCount(unsigned int* count);
            HRESULT __stdcall ResetOnAdd(bool reset);
            HRESULT __stdcall ResetEnumeration();
            HRESULT __stdcall EnumerateNextItem(IAsyncLoaderItem** item, unsigned int* pass);
            HRESULT __stdcall SetPassCount(unsigned int i);
            HRESULT __stdcall Clear();
            HRESULT __stdcall Shutdown();
        };
    }
}
