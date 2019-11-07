//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AsyncLoader.h"
#include "LoadableItemList.h"
#include "AsyncLoaderLayoutManager.h"
#include "AsyncLoaderMemoryManager.h"

using namespace Hilo::AsyncLoader;

const unsigned int AsyncLoader::PassEnumerator::m_passCount = 9;

//
// Constructor for PassEnumerator
//
AsyncLoader::PassEnumerator::PassEnumerator()
{
}

//
// Destructor for PassEnumerator
//
AsyncLoader::PassEnumerator::~PassEnumerator()
{
}

//
// Retrieves the total number of passes
//
HRESULT AsyncLoader::PassEnumerator::GetCount(unsigned int *count)
{
    *count = m_passCount;
    return S_OK;
}

//
// PassEnumerator::GetAt() purpose is to define which page/which size should be loaded
// for a given pass.
//
// In the Hilo application, we want the combinations in the following order:
// 
//                     Other Page  Previous Page  Current Page  Next Page
//  Any Size                                          1
//  Primary Size            9           4             2            3
//  Secondary Size          8           7             5            6
//
// Note that the count and order of transitions changes do not allow us to 
// use nested loops:
//   [1-> 2] : Current Page [ Any Size -> Primary Size]
//   [2-> 3] : [Current Page -> Next Page] Primary size
//
HRESULT AsyncLoader::PassEnumerator::GetAt(unsigned int passIndex, LoadPage* loadPage, LoadSize* loadSize)
{
    if (passIndex >= m_passCount)
    {
        return E_POINTER;
    }

    switch(passIndex)
    {
    case 0 : *loadPage = CurrentPage;   *loadSize = AnySize;
        break;
    case 1 : *loadPage = CurrentPage;   *loadSize = PrimarySize;
        break;
    case 2 : *loadPage = NextPage;      *loadSize = PrimarySize;
        break;
    case 3 : *loadPage = PreviousPage;  *loadSize = PrimarySize;
        break;
    case 4 : *loadPage = CurrentPage;   *loadSize = SecondarySize;
        break;
    case 5 : *loadPage = NextPage;      *loadSize = SecondarySize;
        break;
    case 6 : *loadPage = PreviousPage;  *loadSize = SecondarySize;
        break;
    case 7: *loadPage = OtherPage;      *loadSize = SecondarySize;
        break;
    case 8: *loadPage = OtherPage;      *loadSize = PrimarySize;
        break;
    }

    return S_OK;
}

//
// Constructor for AsyncLoader
//
AsyncLoader::AsyncLoader() :
m_commandsQueuedEvent(nullptr),
    m_workerThreadControl(RunThread),
    m_workerThreadHandle(nullptr),
    m_workerThreadId(0),
    m_workerThreadExitCode(0)
{
    memset(&m_criticalSection, 0, sizeof(m_criticalSection));
    InitializeCriticalSection(&m_criticalSection);
}

//
// Destructor for AsyncLoader
//
AsyncLoader::~AsyncLoader()
{
    if (nullptr != m_commandsQueuedEvent)
    {
        CloseHandle(m_commandsQueuedEvent);
    }

    DeleteCriticalSection(&m_criticalSection);
}

//
// Initializes a new instance of the asynchronous loader
//
HRESULT AsyncLoader::Initialize()
{
    WCHAR eventSeedString[3];
    static unsigned int eventSeed = 0;
    if (eventSeed > 99)
    {
        // To avoid going beyond the allocated buffer.
        return E_FAIL;
    }

    wsprintf(eventSeedString, L"%02d", eventSeed);
    std::wstring eventName = L"CommandsQueuedEvent" + std::wstring(eventSeedString);
    ++eventSeed;

    m_commandsQueuedEvent = CreateEvent(nullptr, true, true, eventName.c_str());
    if (nullptr == m_commandsQueuedEvent)
    {
        return E_FAIL;
    }

    unsigned int passCount = 0;
    HRESULT hr = passEnumerator.GetCount(&passCount);
    if (SUCCEEDED(hr))
    {
        hr = SharedObject<LoadableItemList>::Create(&m_loadableItems);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_loadableItems->SetCriticalSection(&m_criticalSection);
    }

    if (SUCCEEDED(hr))
    {
        // Create layout manager...
        m_loadableItems->SetPassCount(passCount);
        hr = SharedObject<AsyncLoaderLayoutManager>::Create(&m_layoutManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_layoutManager->SetCriticalSection(&m_criticalSection);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_loadableItems->SetLayoutManager(m_layoutManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_layoutManager->RegisterClient(this);
    }

    if (SUCCEEDED(hr))
    {
        // Create memory manager...
        hr = SharedObject<AsyncLoaderMemoryManager>::Create(&m_memoryManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_layoutManager->SetCriticalSection(&m_criticalSection);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_memoryManager->SetLayoutManager(m_layoutManager);
    }

    ComPtr<IAsyncLoaderLayoutManagerClient> layoutManagerClient;
    if (SUCCEEDED(hr))
    {
        hr = m_memoryManager.QueryInterface(&layoutManagerClient);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_layoutManager->RegisterClient(layoutManagerClient);
    }

    return hr;
}

//
// Reset asynchronous loader events
//
HRESULT AsyncLoader::ResetEvents()
{
    if (nullptr == m_commandsQueuedEvent)
    {
        return S_OK;
    }

    // Reset...
    if (FALSE == ::ResetEvent(m_commandsQueuedEvent))
    {
        return E_FAIL;
    }

    return S_OK;
}

//
// Wait for events
//
HRESULT AsyncLoader::WaitForEvents()
{
    if ( nullptr == m_commandsQueuedEvent )
    {
        return S_OK;
    }

    // Wait...
    HRESULT hr = S_OK;
    unsigned long waitResult;
    waitResult = WaitForSingleObject(m_commandsQueuedEvent, INFINITE);

    switch (waitResult)
    {
        // Event object was signaled
    case WAIT_OBJECT_0:
        break;
    default:
        hr = E_FAIL;
    }

    return hr;
}

//
// Check for more events
//
HRESULT AsyncLoader::MoreEvents()
{
    return (FALSE == ::SetEvent(m_commandsQueuedEvent)) ? E_FAIL : S_OK;
}

//
// Retrieves the critical section
//
HRESULT AsyncLoader::GetCriticalSection(CRITICAL_SECTION** criticalSection)
{
    *criticalSection = &m_criticalSection;
    return S_OK;
}

//
// Retrieves AsyncLoader item list
//
HRESULT AsyncLoader::GetItemsList(IAsyncLoaderItemList** itemList)
{
    *itemList = nullptr;

    return AssignToOutputPointer(itemList, m_loadableItems);
}

//
// Retrieves page layout manager
//
HRESULT AsyncLoader::GetItemLayoutManager(IAsyncLoaderLayoutManager** layoutManager)
{
    *layoutManager = nullptr;

    return AssignToOutputPointer(layoutManager, m_layoutManager);
}

//
// Retrieves memory manager
//
HRESULT AsyncLoader::GetMemoryManager(IAsyncLoaderMemoryManager** memoryManager)
{
    *memoryManager = nullptr;

    return AssignToOutputPointer(memoryManager, m_memoryManager);
}

//
// Thread control process
//
unsigned long WINAPI AsyncLoader::ThreadProc(void* threadData)
{
    AsyncLoader* asyncLoader = reinterpret_cast<AsyncLoader*>(threadData);

    HRESULT hr = CoInitialize(nullptr);
    if (SUCCEEDED(hr))
    {
        hr = asyncLoader->Pump();
        CoUninitialize();
    }

    return hr;
}

//
// Create worker thread
//
HRESULT AsyncLoader::StartWorkerThread()
{
    // Only one thread can be started...
    if (nullptr != m_workerThreadHandle)
    {
        return S_OK;
    }

    m_workerThreadHandle = CreateThread(
        nullptr,                       // Thread attributes...
        0,                             // Stack size. If 0, use default stack size.
        ThreadProc,
        reinterpret_cast<void*>(this), // Pointer to data
        0,                             // run immediately after creation
        &m_workerThreadId);
    if (nullptr == m_workerThreadHandle)
    {
        return E_FAIL;
    }

    if (!SetThreadPriority(m_workerThreadHandle, THREAD_PRIORITY_IDLE))
    {
        return E_FAIL;
    }

    return S_OK;
}

//
// Checks if the working thread is active
//
HRESULT AsyncLoader::IsWorkingThreadActive(bool* active)
{
    if (nullptr == m_workerThreadHandle)
    {
        *active = false;
        return S_OK;
    }

    unsigned long exitCode = 0;
    if (!GetExitCodeThread(m_workerThreadHandle, &exitCode))
    {
        return E_FAIL;
    }

    if (STILL_ACTIVE == exitCode)
    {
        *active = true;
        return S_OK;
    }

    *active = false;
    m_workerThreadExitCode = exitCode;

    return S_OK;
}

//
// Pauses the worker thread
//
HRESULT AsyncLoader::PauseWorkerThread()
{
    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = PauseThread;

    return MoreEvents();
}

//
// Resumes the worker thread
//
HRESULT AsyncLoader::ResumeWorkerThread()
{
    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = RunThread;

    return MoreEvents();
}

//
// Ends the worker thread
//
HRESULT AsyncLoader::EndWorkerThread()
{
    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = EndThread;

    return MoreEvents();
}

//
// Processes the next item
//
HRESULT AsyncLoader::ProcessNextItem(ComPtr<IAsyncLoaderItem> loadableItem, unsigned int pass)
{
    if ( nullptr == loadableItem )
    {
        return E_FAIL;
    }

    LoadPage loadPage = OtherPage;
    LoadSize loadSize = AnySize;

    // Get pass information...
    int location = 0;
    HRESULT hr = passEnumerator.GetAt(pass, &loadPage, &loadSize);
    if (SUCCEEDED(hr))
    {
        // Get item location...
        hr = loadableItem->GetLocation(&location);
    }

    bool inPage = false;
    if (SUCCEEDED(hr))
    {
        // Is 'this location' in 'this category'? 
        //   where 'this category' is (loadPage, loadSize)
        hr = m_layoutManager->IsInPage(location, loadPage, loadSize, &inPage);
    }

    if (SUCCEEDED(hr))
    {
        if (!inPage )
        {
            // No action...
            return S_OK;
        }

        if ( OtherPage != loadPage )
        {
            hr = loadableItem->Load(loadPage, loadSize);
        }
        else
        {
            hr = loadableItem->Unload(loadSize);
        }
    }

    return hr;
}

//
// Thread state pump
//
HRESULT AsyncLoader::Pump()
{
    HRESULT hr = S_OK;

    unsigned int debugPass = 0;
    while ((EndThread != m_workerThreadControl ) && (SUCCEEDED(hr)))
    {
        switch(m_workerThreadControl)
        {
        case RunThread:
            {
                // Get next item...
                ComPtr<IAsyncLoaderItem> loadableItem;
                unsigned int pass;
                {
                    CriticalSectionLocker l(&m_criticalSection);
                    hr = m_loadableItems->EnumerateNextItem(&loadableItem, &pass);
                    if (FAILED(hr))
                    {
                        break;
                    }

                    HRESULT tempHr = S_OK;
                    tempHr = ResetEvents();
                    if (FAILED(tempHr))
                    {
                        break;
                    }
                }

                if (debugPass != pass)
                {
                    debugPass = pass;
                }

                // There was no next item..
                if (S_FALSE == hr)
                {
                    // Wait...
                    hr = WaitForEvents();
                }
                else
                {
                    // There is a next item...
                    hr = ProcessNextItem(loadableItem, pass);
                }
            }
            break;
        case PauseThread:
            // Reset...
            hr = ResetEvents();
            if (SUCCEEDED(hr))
            {
                // Wait...
                hr = WaitForEvents();
            }
            break;
        }
    }

    return hr;
}

//
// Indicates the the layout has changed for the layout manager
//
HRESULT AsyncLoader::LayoutChanged()
{
    HRESULT hr = m_loadableItems->ResetEnumeration();
    if (SUCCEEDED(hr))
    {
        hr = MoreEvents();
    }

    return hr;
}

//
// Shutdown the asynchronous loader
//
HRESULT AsyncLoader::Shutdown()
{
    HRESULT hr = m_memoryManager->Shutdown();
    if (SUCCEEDED(hr))
    {
        hr = m_layoutManager->Shutdown();
    }

    if (SUCCEEDED(hr))
    {
        hr = m_loadableItems->Shutdown();
    }

    return hr;
}
