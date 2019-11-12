//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <StspNetwork.h>
#include <AsyncCB.h>
#include <CritSec.h>

namespace Stsp { namespace Net {

class CNetworkChannel : public INetworkChannel
{
public:
    // INetworkChannel
    IFACEMETHOD (BeginSend) (_In_ IBufferPacket *pPacket, IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState);
    IFACEMETHOD (EndSend) (_In_ IMFAsyncResult * pResult );
    IFACEMETHOD (BeginReceive) (_In_ IMediaBufferWrapper *pBuffer, IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState);
    IFACEMETHOD (EndReceive) (_In_ IMFAsyncResult * pResult);
    IFACEMETHOD (Close) ();
    IFACEMETHOD (Disconnect) ();

protected:
    CNetworkChannel();
    ~CNetworkChannel(void);

    virtual void OnClose() {}

    void SetSocket(IStreamSocket *pSocket) {_spSocket = pSocket;}
    IStreamSocket *GetSocket() const {return _spSocket.Get();}

    // CheckClosed: Returns MF_E_SHUTDOWN if the object was shut down.
    HRESULT CheckClosed() const 
    {
        return (_IsClosed? MF_E_SHUTDOWN : S_OK);
    }

protected:
    CritSec                     _critSec;                // critical section for thread safety

private:
    bool                        _IsClosed;               // Flag to indicate if Close() method was called.
    ComPtr<IStreamSocket>       _spSocket;
};

} } // namespace Stsp::Net
