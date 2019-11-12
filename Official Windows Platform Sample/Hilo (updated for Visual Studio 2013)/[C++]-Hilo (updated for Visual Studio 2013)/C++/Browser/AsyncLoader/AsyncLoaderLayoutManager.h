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
        // class AsyncLoaderLayoutManager
        //
        // It decides which items are on which page based on (1) the page pivot
        // and (2) the number of items per page.
        // A page is a set of items that share the same priority in loading/unloading.
        // The page pivot is the fist item in the list that appears in the curent 
        // page.
        //
        class AsyncLoaderLayoutManager : public IAsyncLoaderLayoutManager
        {
            // Synchronization...
            CRITICAL_SECTION* m_criticalSection;

            // Boundaries...
            unsigned int m_pageItemCount;   // Count of items in a page.
            int  m_lowerBoundary;           // Lower boundary for item location.
            int  m_upperBoundary;           // Upper boundary for item location.

            // Next/Previous percentages...
            unsigned int m_nextPagePercentage;      // percentage of current page size.
            unsigned int m_previousPagePercentage;  // percentage of current page size.

            // - Primary size items...
            int m_currentPageStart;
            int m_currentPageEnd;           // items at m_*End are not included.

            int m_nextPageStart;
            int m_nextPageEnd;

            int m_previousPageStart;
            int m_previousPageEnd;

            // - Secondary size items...
            unsigned int m_secondarySizePercentage;

            int m_currentPageSecondarySizePageStart;
            int m_currentPageSecondarySizePageEnd;

            int m_nextPageSecondarySizePageStart;
            int m_nextPageSecondarySizePageEnd;

            int m_previousPageSecondarySizePageStart;
            int m_previousPageSecondarySizePageEnd;

            // m_clients...
            std::vector<ComPtr<IAsyncLoaderLayoutManagerClient>> m_clients;

            // Private methods
            HRESULT RecalculatePages();

            // Events...
            HRESULT FireLayoutChanged();
    
        protected:
            // Constructor.
            AsyncLoaderLayoutManager();

            virtual ~AsyncLoaderLayoutManager();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoaderLayoutManager>::CastTo(iid, this, object);
            }

         public:
           // IAsyncLoaderLayoutManager
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);
            HRESULT __stdcall SetBoundaries(int start, int end);
            HRESULT __stdcall SetCurrentPageItemCount(unsigned int count);
            HRESULT __stdcall SetCurrentPage(int start);
            HRESULT __stdcall SetNextPagePercentage(unsigned int percentage);
            HRESULT __stdcall SetPreviousPagePercentage(unsigned int percentage);
            HRESULT __stdcall SetSecondarySizePercentage(unsigned int percentage);
            HRESULT __stdcall RegisterClient(IAsyncLoaderLayoutManagerClient* client);
            HRESULT __stdcall IsInPage(int location, LoadPage loadPage, LoadSize loadSize, bool* inPage);
            HRESULT __stdcall GetPageItemCount(LoadPage loadPage, LoadSize loadSize, unsigned int* count);
            HRESULT __stdcall Shutdown();
        };
    }
}
