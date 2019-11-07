//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AsyncLoaderHelper.h"
#include "AsyncLoader\CriticalSectionLocker.h"

using namespace Hilo::AsyncLoader;

//
// Constructor
//
AsyncLoaderHelper::AsyncLoaderHelper() :
    m_criticalSection(nullptr),
    m_pageItemCount(10),
    m_pageStart(0),
    m_nextPagePercentage(200),
    m_previousPagePercentage(100),
    m_smallerSizePercentage(120) // smaller size = more items in the window by %20...
{
}

//
// Destructor
//
AsyncLoaderHelper::~AsyncLoaderHelper()
{
    bool active = true;
    do {
        HRESULT hr = m_asyncLoader->IsWorkingThreadActive(&active);
        if (FAILED(hr))
        {
            active = false;
        }
        else
        {
            if (active)
            {
                m_asyncLoader->EndWorkerThread();
                ::Sleep(100);
            }
        }
    } while (active);
}

//
// Initialize
//
HRESULT AsyncLoaderHelper::Initialize()
{
    HRESULT hr = SharedObject<AsyncLoader>::Create(&m_asyncLoader);
    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoader->GetCriticalSection(&m_criticalSection);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoader->GetItemsList(&m_asyncLoaderItemList);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoader->GetItemLayoutManager(&m_asyncLoaderLayoutManager);
    }

    if (SUCCEEDED(hr))
    {
        // Configure layout manager...
        m_asyncLoaderLayoutManager->SetCurrentPageItemCount(static_cast<int>(m_pageItemCount));
        m_asyncLoaderLayoutManager->SetCurrentPage(static_cast<int>(m_pageStart));

        // Configure the memory manager...
        hr = m_asyncLoader->GetMemoryManager(&m_asyncLoaderMemoryManager);
    }

    if (SUCCEEDED(hr))
    {
        // Connect the converter...
        hr = SharedObject<MemorySizeConverter>::Create(&m_memorySizeConverter);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoaderMemoryManager->SetMemorySizeConverter(m_memorySizeConverter);
    }

    if (SUCCEEDED(hr))
    {
        // Configure the memory manager...
        m_asyncLoaderMemoryManager->SetMemoryCap(500000000);
    }

    return hr;
}

//
// Start background loading
//
HRESULT AsyncLoaderHelper::StartBackgroundLoading()
{
    return m_asyncLoader->StartWorkerThread();
}

//
// Pause background loading
//
HRESULT AsyncLoaderHelper::PauseBackgroundLoading()
{
    CriticalSectionLocker l(m_criticalSection);

    return m_asyncLoader->PauseWorkerThread();
}

//
// Resume background loading
//
HRESULT AsyncLoaderHelper::ResumeBackgroundLoading()
{
    CriticalSectionLocker l(m_criticalSection);

    return m_asyncLoader->ResumeWorkerThread();
}

//
// Shutdown
//
HRESULT AsyncLoaderHelper::Shutdown()
{
    CriticalSectionLocker l(m_criticalSection);

    HRESULT hr = m_asyncLoader->EndWorkerThread();
    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoader->Shutdown();
    }

    return hr;
}

//
// Get the associated AsyncLoader
//
HRESULT AsyncLoaderHelper::GetAsyncLoader(IAsyncLoader** asyncLoader)
{
    CriticalSectionLocker l(m_criticalSection);

    *asyncLoader = nullptr;

    return AssignToOutputPointer(asyncLoader, m_asyncLoader);
}

//
// SetPageItemCount
//
HRESULT AsyncLoaderHelper::SetPageItemCount(unsigned int count)
{
    CriticalSectionLocker l(m_criticalSection);

    return m_asyncLoaderLayoutManager->SetCurrentPageItemCount(count);
}

//
// SetCurrentPagePivot
//
HRESULT AsyncLoaderHelper::SetCurrentPagePivot(unsigned int pivot)
{
    CriticalSectionLocker l(m_criticalSection);

    m_pageStart = static_cast<float>(pivot);
    return m_asyncLoaderLayoutManager->SetCurrentPage(static_cast<int>(m_pageStart));
}

//
// Scroll by the specified amount
//
HRESULT AsyncLoaderHelper::Scroll(int scrollBy)
{
    CriticalSectionLocker l(m_criticalSection);

    m_pageStart += scrollBy;
    return m_asyncLoaderLayoutManager->SetCurrentPage((int)m_pageStart);
}

//
// Zoom
//
HRESULT AsyncLoaderHelper::Zoom(float factor)
{
    CriticalSectionLocker l(m_criticalSection);

    m_pageItemCount = m_pageItemCount * factor;
    return m_asyncLoaderLayoutManager->SetCurrentPageItemCount(static_cast<unsigned int>(m_pageItemCount));
}

//
// Connect to the specified client
//
HRESULT AsyncLoaderHelper::ConnectClient(IAsyncLoaderMemoryManagerClient* client)
{
    return m_asyncLoaderMemoryManager->RegisterClient(client);
}

//
// ConnectItem
//
HRESULT AsyncLoaderHelper::ConnectItem(IUnknown* newItem, int location)
{
    ComPtr<IAsyncLoaderItem> asyncLoaderItem;

    HRESULT hr = newItem->QueryInterface(IID_PPV_ARGS(&asyncLoaderItem));
    if (SUCCEEDED(hr))
    {
        hr = asyncLoaderItem->SetMemoryManager(m_asyncLoaderMemoryManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = asyncLoaderItem->SetLocation(location);
    }

    if (SUCCEEDED(hr))
    {
        hr = asyncLoaderItem->SetCriticalSection(m_criticalSection);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_asyncLoaderItemList->Add(asyncLoaderItem);
    }

    return hr;
}

//
// Clear items
//
HRESULT AsyncLoaderHelper::ClearItems()
{
    return m_asyncLoaderItemList->Clear();
}
