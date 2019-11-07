//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include <string>

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // interface IMemorySizeConverter
        //
        // This interface defines the conversion interface between a memory manager 
        // allocation unit and a client item unit.
        //
        // The AsyncLoader client typically creates implementers of this interface.
        //
         [uuid("E7CFEC51-D747-46c7-AEC0-12CF7B46C8E0")]
        __interface IMemorySizeConverter : public IUnknown
        {
            HRESULT __stdcall MemorySizeToClientItemSize(unsigned int memorySize, unsigned int* clientItemSize);
            HRESULT __stdcall ClientItemSizeToMemorySize(unsigned int clientItemSize, unsigned int* memorySize);

        };

        //
        // enum LoadPage
        //
        // It defines the states a page can have. A page represents the grouping
        // of a set of items under one priority.
        //
        enum LoadPage
        {
            PreviousPage,
            CurrentPage,
            NextPage,
            OtherPage,
        };

        //
        // enum LoadSize
        //
        // It defines the sizes that can be associated with a page.
        //
        enum LoadSize
        {
            SecondarySize,
            PrimarySize,
            AnySize,
        };

        //
        // interface IAsyncLoaderLayoutManagerClient
        //
        // Objects which need to be notified when the layout changes must implement
        // this interface and register with the layout manager object.
        //
        [uuid("D32866C7-1148-447b-9BDB-6DEB9FBFB74D")]
        __interface IAsyncLoaderLayoutManagerClient : public IUnknown
        {
            HRESULT __stdcall LayoutChanged();
        };

        //
        // interface IAsyncLoaderLayoutManager
        //
        // Objects that can handle the layout of items into separate pages should
        // implement this interface if they were to be used by the AsynchLoader.
        //
        // The AsyncLoader typically creates implementers of this interface.
        //
        [uuid("BAC4E446-68BC-4c05-86C0-55F93EBE9242")]
        __interface IAsyncLoaderLayoutManager : public IUnknown
        {
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

        //
        // interface IAsyncLoaderMemoryManagerClient
        //
        // The object owned client item can communicate the item size (in client units)
        // by implementing this interface and registering with the memory managers.
        //
        [uuid("D9C62C71-6161-4110-A102-9FEDBA7724CF")]
        __interface IAsyncLoaderMemoryManagerClient : public IUnknown
        {
            HRESULT __stdcall GetClientItemSize(unsigned int* clientItemSize);
        };

        //
        // interface IAsyncLoaderMemoryManager
        //
        // Objects that can assign memory chunks (without actually allocating) to
        // client items based on the current layout (as defined by the layout manager),
        // should implement this interface if they are to be used with the AsyncLoader.
        //
        // The AsyncLoader typically creates implementers of this interface.
        //
        [uuid("296D8466-7E16-4bac-A13F-D61660AECC64")]
        __interface IAsyncLoaderMemoryManager : public IUnknown
        {
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);

            // Configuration
            HRESULT __stdcall SetStepRatio(float stepRatio);

            HRESULT __stdcall SetPrimaryStepUpCount(unsigned int count);
            HRESULT __stdcall SetPrimaryStepDownCount(unsigned int count);

            HRESULT __stdcall SetSecondaryStepUpCountStart(unsigned int count);
            HRESULT __stdcall SetSecondaryStepUpCountEnd(unsigned int count);
            HRESULT __stdcall SetSecondaryStepDownCountStart(unsigned int count);
            HRESULT __stdcall SetSecondaryStepDownCountEnd(unsigned int count);

            HRESULT __stdcall SetMemorySizeConverter(IMemorySizeConverter* sizeConverter);

            HRESULT __stdcall SetMemoryCap(unsigned int memorySize);
            HRESULT __stdcall SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager);

            HRESULT __stdcall RegisterClient(IAsyncLoaderMemoryManagerClient* client);

            // Actions...
            HRESULT __stdcall GetClientItemSize(LoadPage loadPage, LoadSize loadSize, unsigned int* clientItemSizeToLoad);
            HRESULT __stdcall Shutdown();
        };

        //
        // interface IAsyncLoaderItem
        //
        // Client items whose loading and unloading are to be managed by the AsyncLoader
        // must impelement this interface.
        //
        // The AsyncLoader client typically creates implementers of this interface.
        //
        [uuid("0BA3D8D5-3205-4d81-8166-12ACCF64DB41")]
        __interface IAsyncLoaderItem : public IUnknown
        {
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);
            HRESULT __stdcall SetMemoryManager(IAsyncLoaderMemoryManager* memoryManager);

            HRESULT __stdcall SetLocation(int location);
            HRESULT __stdcall GetLocation(int* location);

            HRESULT __stdcall EnableAsyncLoading(bool enable);

            HRESULT __stdcall LoadResource(unsigned int size, IUnknown** resource);
            HRESULT __stdcall SetResource(IUnknown* resource);
            HRESULT __stdcall RenderResource();

            HRESULT __stdcall Load(LoadPage loadPage, LoadSize loadSize);
            HRESULT __stdcall Unload(LoadSize loadSize);

            HRESULT __stdcall Shutdown();
        };

        //
        // interface IAsyncLoaderItemCache
        //
        // This is a helper interface that maybe implemented by the client items
        // to manage the caching of payloads when the AsyncLoader calls into them.
        //
        // An implementation of this interface is typically shared among client items
        // that rely on the AsyncLoader to manager their loading and unloading.
        //
        // This interface is not required by the AsyncLoader.
        //
        // The AsyncLoader client typically creates implementers of this interface.
        //
        [uuid("0A521F7B-8503-4977-A7A4-24458CB68D25")]
        __interface IAsyncLoaderItemCache : public IUnknown
        {
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);
            HRESULT __stdcall SetLoadableItem(IAsyncLoaderItem* loadableItem);
            HRESULT __stdcall SetMemoryManager(IAsyncLoaderMemoryManager* memoryManager);
            HRESULT __stdcall Load(LoadPage loadPage, LoadSize loadSize);
            HRESULT __stdcall Unload(LoadSize loadSize);
            HRESULT __stdcall Shutdown();
        };

        //
        // interface IAsyncLoaderItemList
        //
        // Objects that hold and manage the list of client items that will be accessed
        // by the AsyncLoader must implement this interface.
        //
        // The AsyncLoader typically creates implementers of this interface.
        //
        [uuid("F609CB0B-302B-460c-B177-8AF96BA63F97")]
        __interface IAsyncLoaderItemList : public IUnknown
        {
            HRESULT __stdcall SetCriticalSection(CRITICAL_SECTION* cs);
            HRESULT __stdcall SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager);
            HRESULT __stdcall Add(IAsyncLoaderItem* item);
            HRESULT __stdcall GetCount(unsigned int* count);
            HRESULT __stdcall ResetOnAdd(bool reset);
            HRESULT __stdcall ResetEnumeration();
            HRESULT __stdcall EnumerateNextItem(IAsyncLoaderItem** item, unsigned int* pass);
            HRESULT __stdcall SetPassCount(unsigned int passCount);
            HRESULT __stdcall Clear();
            HRESULT __stdcall Shutdown();
        };

        //
        // interface IAsyncLoader
        //
        // Objects that implement this interface should be able to start a worker thread
        // and pump events.
        //
        // Such an object is typically the AsyncLoader.
        //
        [uuid("46365483-8197-4f76-9944-780BA5FBFB79")]
        __interface IAsyncLoader : public IUnknown
        {
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
            HRESULT __stdcall Shutdown();
        };
    }
}
