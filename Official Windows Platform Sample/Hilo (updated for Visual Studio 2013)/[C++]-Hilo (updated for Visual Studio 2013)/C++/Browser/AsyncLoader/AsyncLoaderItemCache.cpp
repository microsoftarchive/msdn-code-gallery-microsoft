//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AsyncLoaderItemCache.h"
#include "CriticalSectionLocker.h"

using namespace Hilo::AsyncLoader;

//
// Constructor
//
AsyncLoaderItemCache::AsyncLoaderItemCache() : m_criticalSection(nullptr)
{
}

// Destructor
AsyncLoaderItemCache::~AsyncLoaderItemCache()
{
}

//
// Sets the critical section
//
HRESULT AsyncLoaderItemCache::SetCriticalSection(CRITICAL_SECTION* cs)
{
    m_criticalSection = cs;
    return S_OK;
}

//
// Sets the loadable item
//
HRESULT AsyncLoaderItemCache::SetLoadableItem(IAsyncLoaderItem* loadableItem)
{
    m_loadableItem = loadableItem;
    return S_OK;
}

//
// Sets the memory manager
//
HRESULT AsyncLoaderItemCache::SetMemoryManager(IAsyncLoaderMemoryManager* memoryManager)
{
    m_memoryManager = memoryManager;
    return S_OK;
}

//
// Shutdown the item cache
//
HRESULT AsyncLoaderItemCache::Shutdown()
{
    CriticalSectionLocker l(m_criticalSection);
    m_loadableItem = nullptr;
    m_memoryManager = nullptr;
    return S_OK;
}

//
// Load
//
HRESULT AsyncLoaderItemCache::Load(IAsyncLoaderItem* loadableItem, LoadPage loadPage, LoadSize loadSize, bool* set, IUnknown** newObject)
{
    *newObject = nullptr;
    *set = false;

    HRESULT hr = S_OK;
    unsigned int clientItemSizeToLoad = 0;

    if (nullptr != m_memoryManager)
    {
        hr = m_memoryManager->GetClientItemSize(loadPage, loadSize, &clientItemSizeToLoad);
        if (FAILED(hr))
        {
            return E_FAIL;
        }
    }

    if ( 0 == clientItemSizeToLoad )
    {
        hr = Unload(loadSize);
        return hr;
    }

    // Can we use any of the cached Objects?
    if (AnySize == loadSize)
    {
        if ( nullptr != m_primaryObject )
        {
            (*set) = true;
            hr = AssignToOutputPointer(newObject, m_primaryObject);
            return hr;
        }
        if ( nullptr != m_secondaryObject )
        {
            (*set) = true;
            hr = AssignToOutputPointer(newObject, m_secondaryObject);
            return hr;
        }
    }

    switch(loadSize)
    {
    case AnySize:
    case PrimarySize:
        // Need to (1) have something the primay and (2) be the right size
        if ((m_primaryObjectSize != clientItemSizeToLoad) || (nullptr == m_primaryObject))
        {
            // Save for secondary...
            ComPtr<IUnknown>  oldPrimaryObject = m_primaryObject;
            unsigned int oldPrimaryObjectSize = m_primaryObjectSize;

            // Free existing one...
            m_primaryObject = nullptr;

            // Can we use the secondary image?
            if ((m_secondaryObjectSize == clientItemSizeToLoad) && (nullptr != m_secondaryObject))
            {
                // Move the secondary to the primary since it has the right size...
                // This is typical when stepping down...
                m_primaryObject = m_secondaryObject;
                m_primaryObjectSize = m_secondaryObjectSize;

                m_secondaryObject = oldPrimaryObject;
                m_secondaryObjectSize = oldPrimaryObjectSize;
            }
            else
            {
                // Nothing suitable is cached...
                // Move primary to secondary, just in case it'll be needed. If not, the Secondary iteration will remove it.
                m_secondaryObject = oldPrimaryObject;
                m_secondaryObjectSize = oldPrimaryObjectSize;

                // Use the local loadableItem - not the member m_loadableItem.
                // By this time, m_loadableItem might be null due to item shutdown being in progress.
                hr = loadableItem->LoadResource(clientItemSizeToLoad, &m_primaryObject);
                if (SUCCEEDED(hr))
                {
                    m_primaryObjectSize = clientItemSizeToLoad;
                }
                else
                {
                    m_primaryObject = nullptr;
                    m_primaryObjectSize = 0;
                }
            }

            AssignToOutputPointer(newObject, m_primaryObject);
            if ( nullptr != m_primaryObject )
            {
                (*set) = true;
            }
        }
        else
        {
            // Else, do nothing since the size matches. Note that we assume the loaded file does not change 
            //       for a given ImageThumbnailControl object.
            // We don't need to set (*newObject) = m_primaryObject since (*set) will be false.
        }
        break;
    case SecondarySize:
        // Load into Secondary...
        if ( m_secondaryObjectSize != clientItemSizeToLoad )
        {
            m_secondaryObject = nullptr;

            // Use the local loadableItem - not the member m_loadableItem.
            // By this time, m_loadableItem might be null due to item shutdown being in progress.
            hr = loadableItem->LoadResource(clientItemSizeToLoad, &m_secondaryObject);
            if (SUCCEEDED(hr))
            {
                m_secondaryObjectSize = clientItemSizeToLoad;
            }
            else
            {
                m_secondaryObject = nullptr;
                m_secondaryObjectSize = 0;
            }

            AssignToOutputPointer(newObject, m_secondaryObject);
            if ( nullptr != m_secondaryObject )
            {
                (*set) = true;
            }
        }
        break;
    }

    return hr;
}

//
// Load
//
HRESULT AsyncLoaderItemCache::Load(LoadPage loadPage, LoadSize loadSize)
{
    ComPtr<IAsyncLoaderItem> loadableItem;
    {
        // We need to create a copy to avoid holding the lock for too long.
        // Especially, if there is a shutdown in progress.
        CriticalSectionLocker l(m_criticalSection);
        loadableItem = m_loadableItem;
    }

    bool set = false;
    ComPtr<IUnknown> newObject;

    HRESULT hr = Load(loadableItem, loadPage, loadSize, &set, &newObject);
    if (SUCCEEDED(hr))
    {
        if (set)
        {
            loadableItem->SetResource(newObject);
            if ((loadPage == CurrentPage) && (loadSize != SecondarySize))
            {
                loadableItem->RenderResource();
            }
        }
    }

    return hr;
}

//
// Unload
//
HRESULT AsyncLoaderItemCache::Unload(LoadSize loadSize)
{
    switch(loadSize)
    {
    case PrimarySize:
        m_primaryObject = nullptr;
        break;
    case SecondarySize:
        m_secondaryObject = nullptr;
        break;
    }

    return S_OK;
}
