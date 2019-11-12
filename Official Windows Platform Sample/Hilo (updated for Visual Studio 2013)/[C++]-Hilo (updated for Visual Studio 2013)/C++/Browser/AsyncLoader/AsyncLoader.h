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
#include "CriticalSectionLocker.h"

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class AsyncLoader
        //
        // It starts the background thread, and pumps events until it receives a 'shutdown' event.
        //
        class AsyncLoader : public IAsyncLoader, public IAsyncLoaderLayoutManagerClient, public IInitializable
        {
            //
            // PassEnumerator is a helper class that encapsulates the prioritization
            // of pages to load, and their enumeration.
            // 
            class PassEnumerator
            {
                static const unsigned int m_passCount;
            public:
                PassEnumerator();
                ~PassEnumerator();
                HRESULT GetCount(unsigned int* count);
                HRESULT GetAt(unsigned int passIndex, LoadPage* loadPage, LoadSize* loadSize);
            };
            PassEnumerator passEnumerator;

            // Thread handling...
            enum ThreadControl
            {
                RunThread,
                PauseThread,
                EndThread
            };

            // Worker thread state/information...
            ThreadControl m_workerThreadControl;
            HANDLE        m_workerThreadHandle;
            unsigned long         m_workerThreadId;
            unsigned long         m_workerThreadExitCode;

            // Worker thread entry point...
            static unsigned long WINAPI ThreadProc(void* threadData);

            // Pumping loop...
            HRESULT Pump();

            // Synchronization objects

            // Critical section must be locked before accessing thread-shared resources.
            CRITICAL_SECTION m_criticalSection;

            // Event object is signaled by client thread when a change that requires the worker thread attention takes place.
            // The worker thread reset the event once it is done processing the events.
            HANDLE m_commandsQueuedEvent;

            // Helpers to set worker thread various states...
            HRESULT ResetEvents();
            HRESULT WaitForEvents();
            HRESULT MoreEvents();

            // Hooks into helper objects:

            // The list of items which the worker thread operates on...
            ComPtr<IAsyncLoaderItemList>      m_loadableItems;

            // The layout manager that decides which page each item will be placed in...
            ComPtr<IAsyncLoaderLayoutManager> m_layoutManager;

            // The memory manager that decides how much memory can be allocated for items of a given page...
            ComPtr<IAsyncLoaderMemoryManager> m_memoryManager;

            // Private members...
            HRESULT ProcessNextItem(ComPtr<IAsyncLoaderItem> item, unsigned int pass);

        protected:

            // Constructor
            AsyncLoader();

            virtual ~AsyncLoader();

            bool QueryInterfaceHelper(const IID &iid, void **object)
            {
                return CastHelper<IAsyncLoader>::CastTo(iid, this, object) ||
                    CastHelper<IAsyncLoaderLayoutManagerClient>::CastTo(iid, this, object) ||
                    CastHelper<IInitializable>::CastTo(iid, this, object);
            }

            HRESULT __stdcall Initialize();

        public:
            // IAsyncLoader

            // Synchronication...
            HRESULT __stdcall GetCriticalSection(CRITICAL_SECTION** cs);

            // Item list...
            HRESULT __stdcall GetItemsList(IAsyncLoaderItemList** itemList);

            // Page layout manager ...
            HRESULT __stdcall GetItemLayoutManager(IAsyncLoaderLayoutManager** layoutManager);

            // Memory manager...
            HRESULT __stdcall GetMemoryManager(IAsyncLoaderMemoryManager** memoryManager);

            // Thread control...
            HRESULT __stdcall StartWorkerThread();
            HRESULT __stdcall IsWorkingThreadActive(bool* active);
            HRESULT __stdcall PauseWorkerThread();
            HRESULT __stdcall ResumeWorkerThread();
            HRESULT __stdcall EndWorkerThread();

            // Life-time management...
            HRESULT __stdcall Shutdown();

            // IAsyncLoaderLayoutManagerClient
            HRESULT __stdcall LayoutChanged();
        };
    }
}