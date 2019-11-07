//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "NetworkChannel.h"

namespace Stsp { namespace Net {
interface IAcceptOperationContext;

class CNetworkServer : 
    public CNetworkChannel, 
    public INetworkServer
{
public:
    // Static method to create the object.
    static HRESULT CreateInstance(unsigned short wPort, INetworkServer **ppNetworkSender);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // INetworkServer
    IFACEMETHOD (BeginAccept) (
        _In_ IMFAsyncCallback * pCallback,
        _In_ IUnknown * pState );
    IFACEMETHOD (EndAccept) (
        _In_ IMFAsyncResult * pResult, _Outptr_ IHostDescription **ppHostDescription );

protected:
    CNetworkServer(unsigned short wPort);
    ~CNetworkServer();

    __override void OnClose();

private:
    void SetAcceptOperation(IAcceptOperationContext *pOperation) {_spAcceptOperation = pOperation;}
    IAcceptOperationContext *GetAcceptOperation() const {return _spAcceptOperation.Get();}

    HRESULT CreateHostDescription(IStreamSocketInformation *pInfo, IHostDescription **ppHostDescription);

private:
    long            _cRef; // reference count
    ComPtr<IAcceptOperationContext> _spAcceptOperation; // Accept operation
    unsigned short  _wListeningPort;
};

} } // namespace Stsp::Net
