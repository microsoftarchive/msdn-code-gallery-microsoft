//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include <vector>
#include "AsyncLoaderHelperInterface.h"
#include "AsyncLoader\AsyncLoader.h"
#include "AsyncLoader\MemorySizeConverter.h"

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class AsyncLoaderHelper
        //
        // This is a helper class to implement most of the functionality needed
        // by a typical AsyncLoader client. Its goal is minimize the footprint
        // of the AsyncLoader on the Hilo application.
        //
        class AsyncLoaderHelper : public IAsyncLoaderHelper, public IInitializable
        {
            // Synchronization...
            CRITICAL_SECTION* m_criticalSection;

            // Window...
            float m_pageItemCount;  // need to be float to record small changes.
            float m_pageStart;      // need to be float to record small changes.
            unsigned int  m_nextPagePercentage;
            unsigned int  m_previousPagePercentage;
            unsigned int  m_smallerSizePercentage;

            // Helpers
            ComPtr<IAsyncLoader>              m_asyncLoader;
            ComPtr<IAsyncLoaderLayoutManager> m_asyncLoaderLayoutManager;
            ComPtr<IAsyncLoaderMemoryManager> m_asyncLoaderMemoryManager;
            ComPtr<IAsyncLoaderItemList>      m_asyncLoaderItemList;
            ComPtr<IMemorySizeConverter>      m_memorySizeConverter;

        protected:

            // constructor
            AsyncLoaderHelper();
            virtual ~AsyncLoaderHelper();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoaderHelper>::CastTo(iid, this, object) || CastHelper<IInitializable>::CastTo(iid, this, object);
            }

            // IInitializable
            HRESULT __stdcall Initialize();

        public:
            // IAsyncLoaderHelper
            HRESULT __stdcall StartBackgroundLoading();
            HRESULT __stdcall PauseBackgroundLoading();
            HRESULT __stdcall ResumeBackgroundLoading();
            HRESULT __stdcall Shutdown();

            HRESULT __stdcall GetAsyncLoader(IAsyncLoader** _p);

            HRESULT __stdcall SetPageItemCount(unsigned int count);
            HRESULT __stdcall SetCurrentPagePivot(unsigned int pivot);
            HRESULT __stdcall Scroll(int scrollBy);
            HRESULT __stdcall Zoom(float factor);

            HRESULT __stdcall ConnectClient(IAsyncLoaderMemoryManagerClient* client);
            HRESULT __stdcall ConnectItem(IUnknown* newItem, int location);
            HRESULT __stdcall ClearItems();
        };
    }
}
