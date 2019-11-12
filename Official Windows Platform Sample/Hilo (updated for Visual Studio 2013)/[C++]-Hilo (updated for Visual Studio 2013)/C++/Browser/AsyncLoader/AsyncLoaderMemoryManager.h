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
        // class AsyncLoaderMemoryManager
        //
        // This class is responsible for assigning memory portions to client items based
        // on user-specified memory cap and the priority of the page the item lies int.
        //
        // A page is a set of loadable client items that share the same priority.
        //
        class AsyncLoaderMemoryManager : public IAsyncLoaderMemoryManager, public IAsyncLoaderLayoutManagerClient
        {
            // Synchronization...
            CRITICAL_SECTION* m_criticalSection;

            // This is the ratio between two possible client item sizes.
            float m_stepRatio;

            // A list of generated sizes, where the difference between two consecutive 
            // sizes is m_stepRatio.
            std::vector<unsigned int> m_clientItemSizesToLoad;

            // A list parallel to m_clientItemSizesToLoad where:
            // An item value of 1 indicates the corresponding size in m_clientItemSizesToLoad 
            // is a pivot.
            // A pivot size is what gets communicated to the client item to use for loading.
            std::vector<unsigned int> m_pivots;

            // The seed pivot is the very first client item size requested.
            unsigned int m_seedPivot;
            unsigned int m_seedPivotIndex;

            // Boundaries..
            unsigned int m_lowerBoundary;    // The smallest value of the client item size...
            unsigned int m_upperBoundary;    // The largest value of the client item size...
            unsigned int m_overlapStepCount; // The overlap between primary and secondary ranges 
            // expressed in steps. Step size is defined by SetStepRatio().

            // Primary size range..
            unsigned int m_primaryStepUpCount;      // Upper boundary relative to current pivot in steps.
            unsigned int m_primaryStepDownCount;    // Lower boundary relative to current pivot in steps.

            // Primary size pre-calculated percentages...
            float m_primaryUpRatio;         // m_primaryStepUpCount translated into percentage.
            float m_primaryDownRatio;       // m_primaryStepDownCount translated into percentage.

            // Secondary size - Upper range...
            unsigned int m_secondaryStepUpCountStart;// Upper boundary relative to current pivot in steps.
            unsigned int m_secondaryStepUpCountEnd;  // Lower boundary relative to current pivot in steps.

            // Secondary size - Upper range pre-calculated percentages...
            float m_secondaryUpRatioStart;
            float m_secondaryUpRatioEnd;

            // Secondary size - Lower range...
            unsigned int m_secondaryStepDownCountStart;// Upper boundary relative to current pivot in steps.
            unsigned int m_secondaryStepDownCountEnd;  // Lower boundary relative to current pivot in steps.

            // Secondary size - Lower range pre-calculated percentages...
            float m_secondaryDownRatioStart;
            float m_secondaryDownRatioEnd;

            // Helpers...

            // Converts between memory size and client item size (both directions).
            ComPtr<IMemorySizeConverter> m_sizeConverter;

            // Memory manager needs to know the current layout...
            ComPtr<IAsyncLoaderLayoutManager> m_layoutManager;

            // Private data...
            unsigned int m_memoryCap;                   // Memory cap

            static const unsigned int PageMatrixColumnCount;
            static const unsigned int PageMatrixRowCount;
            static const unsigned int InitialLoadSize;
            static const unsigned int InitialUpperBound;
            static const unsigned int InitialLowerBound;

            std::vector< std::vector<unsigned int> > m_pageItemCount;           // Cached copy from the layout manager.
            std::vector< std::vector<unsigned int> > m_clientItemSizeToLoad;    // Cached results for client item call backs during loading.

            // Private helper methods...
            HRESULT UpdateRatios();
            HRESULT ClearTables();
            HRESULT PopulatePivots(unsigned int newSeedPivot);
            HRESULT GetUncappedLoadSizes(unsigned int clientItemSize, unsigned int* uncappedPrimaryLoadSize, unsigned int* uncappedSecondaryLoadSize);

            // Helper to recalculate memory assignments for each page given the size
            // of an item in the primary current page.
            HRESULT AssignItemSizeToPage(unsigned int* availableMemory, LoadPage loadPage, LoadSize loadSize, unsigned int* clientItemSize);
            HRESULT AssignItemSizesToPageGroup(unsigned int* availableMemory, LoadSize size, unsigned int clientItemSize);
            HRESULT AssignItemSizesToAll(unsigned int clientItemSize);

            // AsyncLoader Client (owner of items to load).
            ComPtr<IAsyncLoaderMemoryManagerClient> m_client;

        protected:

            // constructor
            AsyncLoaderMemoryManager();

            virtual ~AsyncLoaderMemoryManager();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoaderMemoryManager>::CastTo(iid, this, object) || 
                    CastHelper<IAsyncLoaderLayoutManagerClient>::CastTo(iid, this, object) ;
            }

        public:

            // IAsyncLoaderMemoryManager
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);

            // Configuration
            HRESULT __stdcall SetStepRatio(float stepRatio);

            HRESULT __stdcall SetPrimaryStepUpCount(unsigned int count);
            HRESULT __stdcall SetPrimaryStepDownCount(unsigned int count);

            HRESULT __stdcall SetSecondaryStepUpCountStart(unsigned int count);
            HRESULT __stdcall SetSecondaryStepUpCountEnd(unsigned int count);
            HRESULT __stdcall SetSecondaryStepDownCountStart(unsigned int count);
            HRESULT __stdcall SetSecondaryStepDownCountEnd(unsigned int count);

            HRESULT __stdcall SetMemorySizeConverter(IMemorySizeConverter* _sizeConverter);

            HRESULT __stdcall SetMemoryCap(unsigned int memorySize);

            HRESULT __stdcall SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager);

            HRESULT __stdcall RegisterClient(IAsyncLoaderMemoryManagerClient* _client);
            HRESULT __stdcall UnregisterClient(IAsyncLoaderLayoutManagerClient* _client);

            // Actions...
            HRESULT __stdcall GetClientItemSize(LoadPage loadPage, LoadSize loadSize, unsigned int* clientItemSizeToLoad);

            HRESULT __stdcall Shutdown();

            // IAsyncLoaderLayoutManagerClient
            HRESULT __stdcall LayoutChanged();
        };
    }
}
