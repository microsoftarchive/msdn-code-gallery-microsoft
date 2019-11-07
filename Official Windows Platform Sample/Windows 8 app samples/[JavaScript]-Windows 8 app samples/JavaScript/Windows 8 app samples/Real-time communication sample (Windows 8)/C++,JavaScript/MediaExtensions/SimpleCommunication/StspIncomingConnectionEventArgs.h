//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "microsoft.samples.simplecommunication.h"

namespace Stsp {
class CMediaSink;

class CIncomingConnectionEventArgs : 
    public Microsoft::WRL::RuntimeClass<ABI::Microsoft::Samples::SimpleCommunication::IIncomingConnectionEventArgs, FtmBase>
{
    InspectableClass(RuntimeClass_Microsoft_Samples_SimpleCommunication_IncomingConnectionEventArgs, BaseTrust);

public:
    CIncomingConnectionEventArgs();
    virtual ~CIncomingConnectionEventArgs();

    HRESULT RuntimeClassInitialize(DWORD dwConnectionId, HSTRING strRemoteUrl, CMediaSink *pMediaSink);

    IFACEMETHOD (get_RemoteUrl) ( 
        HSTRING *value);
                    
    IFACEMETHOD (Accept) ();
                    
    IFACEMETHOD (Refuse) ();

private:
    DWORD _dwConnectionId;
    HString _strRemoteUrl;
    ComPtr<CMediaSink> _spMediaSink;
};
} // namespace Stsp