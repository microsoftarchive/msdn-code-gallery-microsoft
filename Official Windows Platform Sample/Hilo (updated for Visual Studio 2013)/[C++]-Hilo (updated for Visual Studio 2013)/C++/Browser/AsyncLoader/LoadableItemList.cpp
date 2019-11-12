//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "LoadableItemList.h"
#include "CriticalSectionLocker.h"

using namespace Hilo::AsyncLoader;

//
// Constructor
//
LoadableItemList::LoadableItemList() :
    m_criticalSection(nullptr),
    m_currentItem(0),
    m_passCount(1),
    m_currentPass(0),
    m_resetOnAdd(true)
{
}

//
// Destructor
//
LoadableItemList::~LoadableItemList()
{
}

//
// Sets the critical section
//
HRESULT LoadableItemList::SetCriticalSection(CRITICAL_SECTION* criticalSection)
{
    m_criticalSection = criticalSection;
    return S_OK;
}

//
// Sets the layout manager
//
HRESULT LoadableItemList::SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager)
{
    m_layoutManager = layoutManager;
    return S_OK;
}

//
// Adds the specified item
//
HRESULT LoadableItemList::Add(IAsyncLoaderItem* item)
{
    assert(item);

    CriticalSectionLocker l(m_criticalSection);

    m_loadableItems.push_back(ComPtr<IAsyncLoaderItem>(item));
    if (m_resetOnAdd)
    {
        ResetEnumeration();
    }

    HRESULT hr = S_OK;
    if ( nullptr != m_layoutManager )
    {
        hr = m_layoutManager->SetBoundaries(0, static_cast<unsigned int>(m_loadableItems.size()));
    }

    return hr;
}

//
// Get number of items in this list
//
HRESULT LoadableItemList::GetCount(unsigned int* count)
{
    CriticalSectionLocker l(m_criticalSection);

    *count = static_cast<unsigned int>(m_loadableItems.size());
    return S_OK;
}

//
// ResetOnAdd
//
HRESULT LoadableItemList::ResetOnAdd(bool reset)
{
    CriticalSectionLocker l(m_criticalSection);

    m_resetOnAdd = reset;
    return S_OK;
}

//
// Reset enumeration
//
HRESULT LoadableItemList::ResetEnumeration()
{
    CriticalSectionLocker l(m_criticalSection);

    m_currentItem = 0;
    m_currentPass = 0;
    return S_OK;
}

//
// Enumerate next item
//
HRESULT LoadableItemList::EnumerateNextItem(IAsyncLoaderItem** item, unsigned int* pass)
{
    *item = nullptr;

    CriticalSectionLocker l(m_criticalSection);

    if (0 == m_loadableItems.size())
    {
        // No more passes.
        *pass = 0;
        m_currentItem = 0;
        m_currentPass = 0;
        return S_FALSE;
    }

    // We've finished enumerating all items in the list...
    if (m_currentItem >= m_loadableItems.size())
    {
        // Move to the next pass...
        ++m_currentPass;

        // Should we do more passes?
        if (m_currentPass < m_passCount)
        {
            // Reset...
            m_currentItem = 0;
        }
        else
        {
            // No more passes...
            *pass = 0;
            return S_FALSE;
        }
    }

    AssignToOutputPointer(item, m_loadableItems[m_currentItem]);
    *pass = m_currentPass;

    ++m_currentItem;

    return S_OK;
}

//
// SetPassCount
//
HRESULT LoadableItemList::SetPassCount(unsigned int passCount)
{
    CriticalSectionLocker l(m_criticalSection);

    // Cannot have less than one pass...
    if ( passCount < 1 )
    {
        return E_FAIL;
    }

    m_passCount = passCount;
    m_currentPass = 0;
    m_currentItem = 0;

    return S_OK;
}

//
// Clear the list
//
HRESULT LoadableItemList::Clear()
{
    CriticalSectionLocker l(m_criticalSection);

    // Reset internal vector...
    for (auto it = m_loadableItems.begin() ; it != m_loadableItems.end() ; ++it )
    {
        (*it)->Shutdown();
        (*it) = nullptr;
    }

    m_loadableItems.clear();

    // Reset enumeration state...
    m_currentPass = 0;
    m_currentItem = 0;

    ResetEnumeration();

    // Notify the layout manager of the change...
    HRESULT hr = S_OK;
    if (nullptr != m_layoutManager)
    {
        hr = m_layoutManager->SetBoundaries(0, static_cast<unsigned int>(m_loadableItems.size()));
    }

    return hr;
}

//
// Shutdown
//
HRESULT LoadableItemList::Shutdown()
{
    HRESULT hr = Clear();

    m_layoutManager = nullptr;
    return hr;
}
