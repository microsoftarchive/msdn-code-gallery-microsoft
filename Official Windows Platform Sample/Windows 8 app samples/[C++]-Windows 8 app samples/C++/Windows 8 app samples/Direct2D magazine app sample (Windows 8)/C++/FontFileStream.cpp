//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

FontFileStream::FontFileStream(_In_ Platform::Array<byte>^ data) :
    m_refCount(),
    m_data(data)
{
}

HRESULT STDMETHODCALLTYPE FontFileStream::QueryInterface(
    REFIID uuid,
    _Outptr_ void** object
    )
{
    if (    uuid == IID_IUnknown
        ||  uuid == __uuidof(IDWriteFontFileStream)
        )
    {
        *object = this;
        AddRef();
        return S_OK;
    }
    else
    {
        *object = nullptr;
        return E_NOINTERFACE;
    }
}

ULONG STDMETHODCALLTYPE FontFileStream::AddRef()
{
    return static_cast<ULONG>(InterlockedIncrement(&m_refCount));
}

ULONG STDMETHODCALLTYPE FontFileStream::Release()
{
    ULONG newCount = static_cast<ULONG>(InterlockedDecrement(&m_refCount));

    if (newCount == 0)
    {
        delete this;
    }

    return newCount;
}

HRESULT STDMETHODCALLTYPE FontFileStream::ReadFileFragment(
    _Outptr_result_bytebuffer_(fragmentSize) void const** fragmentStart,
    UINT64 fileOffset,
    UINT64 fragmentSize,
    _Out_ void** fragmentContext
    )
{
    // The loader is responsible for doing a bounds check.
    if (    fileOffset <= m_data->Length
        &&  fragmentSize + fileOffset <= m_data->Length
        )
    {
        *fragmentStart = m_data->Data + static_cast<ULONG>(fileOffset);
        *fragmentContext = nullptr;
        return S_OK;
    }
    else
    {
        *fragmentStart = nullptr;
        *fragmentContext = nullptr;
        return E_FAIL;
    }
}

void STDMETHODCALLTYPE FontFileStream::ReleaseFileFragment(
    _In_ void* fragmentContext
    )
{
}

HRESULT STDMETHODCALLTYPE FontFileStream::GetFileSize(
    _Out_ UINT64* fileSize
    )
{
    *fileSize = m_data->Length;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE FontFileStream::GetLastWriteTime(
    _Out_ UINT64* lastWriteTime
    )
{
    // The concept of last write time does not apply to this loader.
    *lastWriteTime = 0;
    return E_NOTIMPL;
}
