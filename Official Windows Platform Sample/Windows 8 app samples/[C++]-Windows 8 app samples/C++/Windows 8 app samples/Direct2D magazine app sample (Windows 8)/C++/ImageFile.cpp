//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace concurrency;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

ImageFile::ImageFile() :
    m_refCount(0),
    m_offset(0),
    m_data(nullptr)
{
}

//  Asynchronously load image data from an image file
//
void ImageFile::LoadAsync(
    _In_ StorageFolder^ location,
    _In_ Platform::String^ fileName
    )
{
    task<StorageFile^> getFile(location->GetFileAsync(fileName));
    getFile.then([=](StorageFile^ file)
    {
        return FileIO::ReadBufferAsync(file);
    }).then([=](IBuffer^ buffer)
    {
        m_data = ref new Platform::Array<byte>(buffer->Length);
        DataReader::FromBuffer(buffer)->ReadBytes(m_data);
    });
}

HRESULT STDMETHODCALLTYPE ImageFile::QueryInterface(
    REFIID uuid,
    _Outptr_ void** object
    )
{
    *object = nullptr;

    if (    uuid == __uuidof(IUnknown)
        ||  uuid == __uuidof(IStream)
        ||  uuid == __uuidof(ISequentialStream)
        )
    {
        *object = static_cast<IStream*>(this);
        AddRef();
        return S_OK;
    }
    else
    {
        *object = nullptr;
        return E_NOINTERFACE;
    }
}

ULONG STDMETHODCALLTYPE ImageFile::AddRef()
{
    return static_cast<ULONG>(InterlockedIncrement(&m_refCount));
}

ULONG STDMETHODCALLTYPE ImageFile::Release()
{
    ULONG newCount = static_cast<ULONG>(InterlockedDecrement(&m_refCount));

    if (newCount == 0)
        delete this;

    return newCount;
}

HRESULT STDMETHODCALLTYPE ImageFile::Read(
    _Out_writes_bytes_(bytesToRead) void* outputBuffer,
    ULONG bytesToRead,
    _Out_ ULONG* bytesRead
    )
{
    HRESULT hr = S_OK;

    if (m_offset + bytesToRead > m_data->Length)
    {
        bytesToRead = m_data->Length - m_offset;
        hr = S_FALSE;
    }

    if (bytesToRead > 0)
    {
        memcpy(outputBuffer, m_data->Data + m_offset, bytesToRead);
    }

    *bytesRead = bytesToRead;
    m_offset += bytesToRead;

    return hr;
}

HRESULT STDMETHODCALLTYPE ImageFile::Seek(
    LARGE_INTEGER distance,
    DWORD origin,
    _Out_ ULARGE_INTEGER* newLocation
    )
{
    ULONG offset = 0;

    switch (origin)
    {
        case STREAM_SEEK_CUR:
            offset = m_offset;

        case STREAM_SEEK_SET:
            if (    distance.HighPart != 0
                ||  distance.LowPart + offset > m_data->Length
                )
            {
                return STG_E_INVALIDFUNCTION;
            }

            offset += distance.LowPart;
            break;

        case STREAM_SEEK_END:
            if (    distance.HighPart != 0
                ||  distance.LowPart > m_data->Length
                )
            {
                return STG_E_INVALIDFUNCTION;
            }

            offset = m_data->Length - distance.LowPart;
            break;

        default:
            return STG_E_INVALIDFUNCTION;
    }

    m_offset = offset;

    if (newLocation)
    {
        newLocation->HighPart = 0;
        newLocation->LowPart = m_offset;
    }

    return S_OK;
}

HRESULT STDMETHODCALLTYPE ImageFile::Write(
    _In_reads_bytes_(bytesToWrite) void const* inputBuffer,
    ULONG bytesToWrite,
    _Out_ ULONG* bytesWritten
    )
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::SetSize(ULARGE_INTEGER)
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::CopyTo(
    _In_ IStream*,
    ULARGE_INTEGER,
    _Out_ ULARGE_INTEGER*,
    _Out_ ULARGE_INTEGER*
    )
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::Commit(DWORD)
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::Revert()
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::LockRegion(
    ULARGE_INTEGER,
    ULARGE_INTEGER,
    DWORD
    )
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::UnlockRegion(
    ULARGE_INTEGER,
    ULARGE_INTEGER,
    DWORD
    )
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::Clone(_Outptr_ IStream **)
{
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE ImageFile::Stat(
    _Out_ STATSTG* statistics,
    DWORD
    )
{
    memset(statistics, 0, sizeof(*statistics));

    statistics->cbSize.LowPart = m_data->Length;
    statistics->cbSize.HighPart = 0;

    return S_OK;
}
