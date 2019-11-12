//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

//  Resource supporting sequential stream interface used by the DOM API.
//
//  This class only implements stream reading and not writing.
//
class ImageFile : public IStream
{
public:
    ImageFile();

    void LoadAsync(
        _In_ Windows::Storage::StorageFolder^ location,
        _In_ Platform::String^ fileName
        );

    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID uuid,
        _Outptr_ void** object
        ) override;

    virtual ULONG STDMETHODCALLTYPE AddRef() override;

    virtual ULONG STDMETHODCALLTYPE Release() override;

    virtual HRESULT STDMETHODCALLTYPE Read(
        _Out_writes_bytes_(bytesToRead) void* outputBuffer,
        ULONG bytesToRead,
        _Out_ ULONG* bytesRead
        ) override;

    virtual HRESULT STDMETHODCALLTYPE Write(
        _In_reads_bytes_(bytesToWrite) void const* inputBuffer,
        ULONG bytesToWrite,
        _Out_ ULONG* bytesWritten
        ) override;

    virtual HRESULT STDMETHODCALLTYPE SetSize(ULARGE_INTEGER) override;

    virtual HRESULT STDMETHODCALLTYPE CopyTo(
        _In_ IStream* destinationStream,
        ULARGE_INTEGER bytesToCopy,
        _Out_ ULARGE_INTEGER* bytesRead,
        _Out_ ULARGE_INTEGER* bytesWritten
        ) override;

    virtual HRESULT STDMETHODCALLTYPE Commit(DWORD flags) override;

    virtual HRESULT STDMETHODCALLTYPE Revert() override;

    virtual HRESULT STDMETHODCALLTYPE LockRegion(
        ULARGE_INTEGER bytesOffset,
        ULARGE_INTEGER bytesToLock,
        DWORD lockType
        ) override;

    virtual HRESULT STDMETHODCALLTYPE UnlockRegion(
        ULARGE_INTEGER bytesOffset,
        ULARGE_INTEGER bytesToLock,
        DWORD lockType
        ) override;

    virtual HRESULT STDMETHODCALLTYPE Clone(_Outptr_ IStream **cloneStream) override;

    virtual HRESULT STDMETHODCALLTYPE Seek(
        LARGE_INTEGER distance,
        DWORD origin,
        _Out_ ULARGE_INTEGER* newLocation
        ) override;

    virtual HRESULT STDMETHODCALLTYPE Stat(
        _Out_ STATSTG* statistics,
        DWORD flags
        ) override;

    inline bool Ready()
    {
        return m_data != nullptr;
    }

private:
    // Reference counter
    ULONG m_refCount;

    // Resource data
    Platform::Array<byte>^ m_data;

    // Current read offset in bytes
    ULONG m_offset;
};
