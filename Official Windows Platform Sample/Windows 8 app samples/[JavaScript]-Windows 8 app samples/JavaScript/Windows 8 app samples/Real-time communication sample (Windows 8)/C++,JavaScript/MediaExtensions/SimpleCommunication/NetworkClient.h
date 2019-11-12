//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "NetworkChannel.h"

namespace Stsp { namespace Net {

class CNetworkClient : public CNetworkChannel, public INetworkClient
{
public:
    // Static method to create the object.
    static HRESULT CreateInstance(_Outptr_ INetworkClient **ppNetworkClient);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // INetworkClient
    IFACEMETHOD (BeginConnect) (
        _In_ LPCWSTR szUrl,
        WORD wPort,
        _In_ IMFAsyncCallback * pCallback,
        _In_opt_ IUnknown * pState );
    IFACEMETHOD (EndConnect) (
        _In_ IMFAsyncResult * pResult );

protected:
    CNetworkClient();
    ~CNetworkClient();

private:
    HRESULT Initialize();

private:
    long                        _cRef;  // reference count
};

} } // namespace Stsp::Net
