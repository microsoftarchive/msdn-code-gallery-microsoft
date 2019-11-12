//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <StspDefs.h>

interface DECLSPEC_UUID("D30E9A4E-31BD-484C-940D-0E5FF536036D") DECLSPEC_NOVTABLE IMediaBufferWrapper : public IUnknown
{
    IFACEMETHOD_(IMFMediaBuffer *, GetMediaBuffer) () const = 0;
    IFACEMETHOD_(BYTE *, GetBuffer) () const = 0;

    IFACEMETHOD (SetOffset) (DWORD nOffset) = 0;
    IFACEMETHOD_(DWORD, GetOffset) () const = 0;

    IFACEMETHOD (GetCurrentLength) (DWORD *pcbCurrentLength) = 0;
    IFACEMETHOD (SetCurrentLength) (DWORD cbCurrentLength) = 0;

    IFACEMETHOD (TrimLeft) (DWORD cbSize) = 0;
    IFACEMETHOD (TrimRight) (DWORD cbSize, _Out_ IMediaBufferWrapper **pWrapper) = 0;

    IFACEMETHOD (Reset) () = 0;
};

interface DECLSPEC_UUID("3FEBFE65-E515-4448-8DB4-10D7DBC030A8") DECLSPEC_NOVTABLE IBufferEnumerator : public IUnknown
{
    IFACEMETHOD_ (bool, IsValid) ();
    IFACEMETHOD (GetCurrent) (_Out_ IMediaBufferWrapper **ppBuffer);
    IFACEMETHOD (MoveNext) ();
    IFACEMETHOD (Reset) ();
};

interface DECLSPEC_UUID("E6D1193E-8B68-4526-885A-9C858613807D") DECLSPEC_NOVTABLE IBufferPacket : public IUnknown
{
    IFACEMETHOD (AddBuffer) (IMediaBufferWrapper *pBuffer) = 0;
    IFACEMETHOD (InsertBuffer) (unsigned int nIndex, _In_ IMediaBufferWrapper *pBuffer) = 0;
    IFACEMETHOD (RemoveBuffer) (unsigned int nIndex, _Outptr_ IMediaBufferWrapper **ppBuffer) = 0;
    IFACEMETHOD (Clear) () = 0;
    IFACEMETHOD_(size_t, GetBufferCount) () const = 0;

    IFACEMETHOD (GetTotalLength) (_Out_ DWORD *pcbTotalLength) = 0;
    IFACEMETHOD (CopyTo) (DWORD nOffset, DWORD cbSize, _In_reads_bytes_(cbSize) void *pDest, _Out_ DWORD *pcbCopied) = 0;
    IFACEMETHOD (MoveLeft) (DWORD cbSize, _Out_writes_bytes_(cbSize) void *pDest) = 0;
    IFACEMETHOD (TrimLeft) (DWORD cbSize) = 0;
    IFACEMETHOD (ToMFSample) (IMFSample **ppSample) = 0;

    IFACEMETHOD (GetEnumerator) (_Out_ IBufferEnumerator **pEnumerator) = 0;

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
    IFACEMETHOD (FillWSABUF) (WSABUF *pBuffers, size_t cBuffers) = 0;
    IFACEMETHOD (SaveToFile) (LPCWSTR pszFileName) = 0;
#endif
};

interface DECLSPEC_UUID("3371A40D-FFAE-47B4-9606-46A128974770") DECLSPEC_NOVTABLE IHostDescription : public IUnknown
{
    IFACEMETHOD_(LPCWSTR, GetHostName) () = 0;
    IFACEMETHOD_(LPCWSTR, GetHostService) () = 0;
    IFACEMETHOD_(StspNetworkType, GetNetworkType) () = 0;
};

interface DECLSPEC_UUID("F0BFC35D-E016-44ED-8B35-99870B5DD902") DECLSPEC_NOVTABLE INetworkChannel : public IUnknown
{
    IFACEMETHOD (BeginSend) (_In_ IBufferPacket *pPacket, _In_ IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState) = 0;
    IFACEMETHOD (EndSend) (_In_ IMFAsyncResult * pResult ) = 0;
    IFACEMETHOD (BeginReceive) (_In_ IMediaBufferWrapper *pBuffer, _In_ IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState) = 0;
    IFACEMETHOD (EndReceive) (_In_ IMFAsyncResult * pResult) = 0;
    IFACEMETHOD (Close) () = 0;
    IFACEMETHOD (Disconnect) () = 0;
};

interface DECLSPEC_UUID("BACADC0B-C9DD-477B-B575-F7E340304144") DECLSPEC_NOVTABLE INetworkServer : public IUnknown
{
    IFACEMETHOD (BeginAccept) (
        _In_ IMFAsyncCallback * pCallback,
        _In_opt_ IUnknown * pState ) = 0;
    IFACEMETHOD (EndAccept) (
        _In_ IMFAsyncResult * pResult, _Outptr_ IHostDescription **ppRemoteHostDescription  ) = 0;
};

interface DECLSPEC_UUID("F7143371-8F6C-464F-9973-2E93A538D6AD") DECLSPEC_NOVTABLE INetworkClient : public IUnknown
{
    IFACEMETHOD (BeginConnect) (
        _In_ LPCWSTR szUrl,
        WORD wPort,
        _In_ IMFAsyncCallback * pCallback,
        _In_opt_ IUnknown * pState ) = 0;
    IFACEMETHOD (EndConnect) (
        _In_ IMFAsyncResult * pResult ) = 0;
};

HRESULT CreateNetworkServer(unsigned short wPort, _Outptr_ INetworkServer **ppNetworkServer);
HRESULT CreateNetworkClient(_Outptr_ INetworkClient **ppNetworkClient);
HRESULT CreateMediaBufferWrapper(DWORD dwMaxLength, _Outptr_ IMediaBufferWrapper **ppMediaBufferWrapper);
HRESULT CreateMediaBufferWrapper(_In_ IMFMediaBuffer *pMediaBuffer, _Outptr_ IMediaBufferWrapper **ppMediaBufferWrapper);
HRESULT CreateBufferPacketFromMFSample(_In_ IMFSample *pSample, _Outptr_ IBufferPacket **ppBufferPacket);
HRESULT CreateBufferPacket(_Outptr_ IBufferPacket **ppBufferPacket);
HRESULT CreateHostDescription(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType, _Outptr_ IHostDescription **ppHostDesc);
