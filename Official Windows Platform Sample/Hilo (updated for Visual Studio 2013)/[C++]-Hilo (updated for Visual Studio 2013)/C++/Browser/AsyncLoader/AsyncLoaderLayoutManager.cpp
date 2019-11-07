//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include <string>
#include "AsyncLoaderLayoutManager.h"
#include "CriticalSectionLocker.h"

using namespace Hilo::AsyncLoader;

//
// Constructor
//
AsyncLoaderLayoutManager::AsyncLoaderLayoutManager() :
m_criticalSection(nullptr),
    m_pageItemCount(0),
    m_lowerBoundary(0),
    m_upperBoundary(0),
    m_nextPagePercentage(200),        // Two full pages.
    m_previousPagePercentage(100),    // One full page.
    m_secondarySizePercentage(121),   // by default : secondary size = smaller size
                                      // smaller size = more items in the window by %10 ^ 2...
                                      // Current size items...
    m_currentPageStart(0),
    m_currentPageEnd(0),
    m_nextPageStart(0),
    m_nextPageEnd(0),
    m_previousPageStart(0),
    m_previousPageEnd(0),
    // Secondary size items...
    m_currentPageSecondarySizePageStart(0),
    m_currentPageSecondarySizePageEnd(0),
    m_nextPageSecondarySizePageStart(0),
    m_nextPageSecondarySizePageEnd(0),
    m_previousPageSecondarySizePageStart(0),
    m_previousPageSecondarySizePageEnd(0)
{
}

//
// Destructor
//
AsyncLoaderLayoutManager::~AsyncLoaderLayoutManager()
{
}

//
// Recalculate page settings
//
HRESULT AsyncLoaderLayoutManager::RecalculatePages()
{
    CriticalSectionLocker l(m_criticalSection);

    int pageItemCount = m_pageItemCount;
    int nextPageItemCount = static_cast<int>((static_cast<float>(m_pageItemCount) / 100 * m_nextPagePercentage) + 1);
    int previousPageItemCount = static_cast<int>((static_cast<float>(m_pageItemCount) / 100 * m_previousPagePercentage) + 1);

    // Current size...
    m_currentPageStart = m_currentPageStart;
    m_currentPageEnd = m_currentPageStart + pageItemCount;

    m_nextPageStart = m_currentPageEnd;
    m_nextPageEnd = m_nextPageStart + nextPageItemCount;

    m_previousPageStart = m_currentPageStart - previousPageItemCount;
    m_previousPageEnd = m_currentPageStart;

    // Current size : fix boundaries...
    m_currentPageStart = std::min( std::max(m_currentPageStart, m_lowerBoundary), m_upperBoundary);
    m_currentPageEnd = std::max( std::min(m_currentPageEnd, m_upperBoundary), m_lowerBoundary);

    m_nextPageStart = std::min( std::max(m_nextPageStart, m_lowerBoundary), m_upperBoundary);
    m_nextPageEnd = std::max( std::min(m_nextPageEnd, m_upperBoundary), m_lowerBoundary);

    m_previousPageStart = std::min( std::max(m_previousPageStart, m_lowerBoundary), m_upperBoundary);
    m_previousPageEnd = std::max(std::min(m_previousPageEnd, m_upperBoundary), m_lowerBoundary);

    // Secondary size...
    int currentPageSecondarySizePageSize = static_cast<int>((static_cast<float>(pageItemCount) / 100 * m_secondarySizePercentage) + 1);
    int nextPageSecondarySizePageSize = static_cast<int>((static_cast<float>(currentPageSecondarySizePageSize) / 100 * m_nextPagePercentage) + 1);
    int previousPageSecondarySizePageSize  = static_cast<int>((static_cast<float>(currentPageSecondarySizePageSize) / 100 * m_previousPagePercentage) + 1);

    m_currentPageSecondarySizePageStart = m_currentPageStart; // Pivot.
    m_currentPageSecondarySizePageEnd = m_currentPageSecondarySizePageStart + currentPageSecondarySizePageSize;

    m_nextPageSecondarySizePageStart = m_currentPageSecondarySizePageEnd;
    m_nextPageSecondarySizePageEnd = m_nextPageSecondarySizePageStart + nextPageSecondarySizePageSize;

    m_previousPageSecondarySizePageStart = m_currentPageSecondarySizePageStart - previousPageSecondarySizePageSize;
    m_previousPageSecondarySizePageEnd = m_currentPageSecondarySizePageStart;

    // Secondary size : fix boundaries...
    m_currentPageSecondarySizePageStart = std::min(std::max(m_currentPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_currentPageSecondarySizePageEnd = std::max( std::min(m_currentPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    m_nextPageSecondarySizePageStart = std::min( std::max(m_nextPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_nextPageSecondarySizePageEnd = std::max( std::min(m_nextPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    m_previousPageSecondarySizePageStart = std::min( std::max(m_previousPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_previousPageSecondarySizePageEnd = std::max( std::min(m_previousPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    return FireLayoutChanged();
}

//
// Sets the critical section
//
HRESULT AsyncLoaderLayoutManager::SetCriticalSection(CRITICAL_SECTION* criticalSection)
{
    m_criticalSection = criticalSection;
    return S_OK;
}

//
// Sets the starting and ending boundaries
//
HRESULT AsyncLoaderLayoutManager::SetBoundaries(int start, int end)
{
    if (start > end)
    {
        return E_INVALIDARG;
    }

    CriticalSectionLocker l(m_criticalSection);

    m_lowerBoundary = start;
    m_upperBoundary = end;

    return RecalculatePages();
}

//
// Sets the current number of items on the current page (view)
//
HRESULT AsyncLoaderLayoutManager::SetCurrentPageItemCount(unsigned int count)
{
    CriticalSectionLocker l(m_criticalSection);

    m_pageItemCount = count;

    return RecalculatePages();
}

//
// Sets the current page
//
HRESULT AsyncLoaderLayoutManager::SetCurrentPage(int start)
{
    CriticalSectionLocker l(m_criticalSection);

    m_currentPageStart = start;
    m_currentPageEnd = m_currentPageStart  + m_pageItemCount;

    return RecalculatePages();
}

//
// Sets the next page percentage
//
HRESULT AsyncLoaderLayoutManager::SetNextPagePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_criticalSection);

    m_nextPagePercentage = percentage;

    return RecalculatePages();
}

//
// Sets the previous page percentage
//
HRESULT AsyncLoaderLayoutManager::SetPreviousPagePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_criticalSection);

    m_previousPagePercentage = percentage;

    return RecalculatePages();
}

//
// Sets the secondary size percentage
//
HRESULT AsyncLoaderLayoutManager::SetSecondarySizePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_criticalSection);

    m_secondarySizePercentage = percentage;

    return RecalculatePages();
}

//
// Registers the manager with the specified client
//
HRESULT AsyncLoaderLayoutManager::RegisterClient(IAsyncLoaderLayoutManagerClient* client)
{
    CriticalSectionLocker l(m_criticalSection);

    m_clients.push_back(ComPtr<IAsyncLoaderLayoutManagerClient>(client));
    return S_OK;
}

//
// Triggers the layout changed event
//
HRESULT AsyncLoaderLayoutManager::FireLayoutChanged()
{
    CriticalSectionLocker l(m_criticalSection);

    //std::vector<ComPtr<IAsyncLoaderLayoutManagerClient>>::iterator it;
    for (auto client = m_clients.begin() ; client != m_clients.end() ; client++)
    {
        (*client)->LayoutChanged();
    }

    return S_OK;
}

//
// Checks if the current location is in the specified page
//
HRESULT AsyncLoaderLayoutManager::IsInPage(int location, LoadPage loadPage, LoadSize loadSize, bool* inPage)
{
    CriticalSectionLocker l(m_criticalSection);

    *inPage = false;

    int currentPageStart = m_currentPageStart;
    int currentPageEnd = m_currentPageEnd;
    int nextPageStart = m_nextPageStart;
    int nextPageEnd = m_nextPageEnd;
    int previousPageStart = m_previousPageStart;
    int previousPageEnd = m_previousPageEnd;
    int newLocation = location;

    // Based on loadSize, scale location.
    switch(loadSize)
    {
    case AnySize:
    case PrimarySize:
        // No scaling is necessary...
        // No changes for intervals...
        break;
    case SecondarySize:
        newLocation = static_cast<int>((static_cast<float>(location - m_currentPageStart) / 100 * m_secondarySizePercentage) + m_currentPageStart);
        currentPageStart = m_currentPageSecondarySizePageStart;
        currentPageEnd = m_currentPageSecondarySizePageEnd;
        nextPageStart = m_nextPageSecondarySizePageStart;
        nextPageEnd = m_nextPageSecondarySizePageEnd;
        previousPageStart = m_previousPageSecondarySizePageStart;
        previousPageEnd = m_previousPageSecondarySizePageEnd;
        break;
    }

    switch(loadPage)
    {
    case OtherPage:
        if ( (newLocation < previousPageStart) || (nextPageEnd < newLocation) )
        {
            *inPage = true;
        }
        break;
    case CurrentPage:
        if ( (currentPageStart <= newLocation) && (newLocation < currentPageEnd) )
        {
            *inPage = true;
        }
        break;
    case NextPage:
        if ( (nextPageStart <= newLocation) && (newLocation < nextPageEnd) )
        {
            *inPage = true;
        }
        break;
    case PreviousPage:
        if ( (previousPageStart <= newLocation) && (newLocation < previousPageEnd) )
        {
            *inPage = true;
        }
        break;
    }

    return S_OK;
}

//
// Gets the number of items per page
//
HRESULT AsyncLoaderLayoutManager::GetPageItemCount(LoadPage loadPage, LoadSize loadSize, unsigned int* count)
{
    CriticalSectionLocker l(m_criticalSection);

    switch( loadPage )
    {
    case CurrentPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = m_currentPageEnd - m_currentPageStart;
                break;
            case SecondarySize:
                *count = m_currentPageSecondarySizePageEnd - m_currentPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case NextPage:
        {
            switch(loadSize)
            {
            case AnySize:
            case PrimarySize:
                *count = m_nextPageEnd - m_nextPageStart;
                break;
            case SecondarySize:
                *count = m_nextPageSecondarySizePageEnd - m_nextPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case PreviousPage:
        {
            switch(loadSize)
            {
            case AnySize:
            case PrimarySize:
                *count = m_previousPageEnd - m_previousPageStart;
                break;
            case SecondarySize:
                *count = m_previousPageSecondarySizePageEnd - m_previousPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case OtherPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = (m_previousPageStart - m_lowerBoundary) + (m_upperBoundary - m_nextPageEnd);
                break;
            case SecondarySize:
                *count = (m_previousPageSecondarySizePageStart - m_lowerBoundary) + (m_upperBoundary - m_nextPageSecondarySizePageEnd);
                break;
            }
        }
        break;
    }

    return S_OK;
}

//
// Shutsdown the layout manager and releases all associated objects
//
HRESULT AsyncLoaderLayoutManager::Shutdown()
{
    CriticalSectionLocker l(m_criticalSection);

    for (auto client = m_clients.begin(); client != m_clients.end(); client++)
    {
        *client = nullptr;
    }

    m_clients.clear();

    return S_OK;
}
