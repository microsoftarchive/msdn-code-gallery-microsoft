//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "stdafx.h"
#include "StspIncomingConnectionEventArgs.h"
#include "StspMediaSink.h"

using namespace Stsp;

CIncomingConnectionEventArgs::CIncomingConnectionEventArgs()
    : _dwConnectionId(0)
{
}

CIncomingConnectionEventArgs::~CIncomingConnectionEventArgs()
{
}

HRESULT CIncomingConnectionEventArgs::RuntimeClassInitialize(DWORD dwConnectionId, HSTRING strRemoteUrl, CMediaSink *pMediaSink)
{
    _dwConnectionId = dwConnectionId;
    _spMediaSink = pMediaSink;
    return WindowsDuplicateString(strRemoteUrl, _strRemoteUrl.GetAddressOf());
}

IFACEMETHODIMP CIncomingConnectionEventArgs::get_RemoteUrl (HSTRING *value)
{
    if (value == nullptr)
    {
        return E_INVALIDARG;
    }

    *value = _strRemoteUrl.Get();

    return S_OK;
}
                    
IFACEMETHODIMP CIncomingConnectionEventArgs::Accept()
{
    return _spMediaSink->_TriggerAcceptConnection(_dwConnectionId);
}
                    
IFACEMETHODIMP CIncomingConnectionEventArgs::Refuse()
{
    return _spMediaSink->_TriggerRefuseConnection(_dwConnectionId);
}
