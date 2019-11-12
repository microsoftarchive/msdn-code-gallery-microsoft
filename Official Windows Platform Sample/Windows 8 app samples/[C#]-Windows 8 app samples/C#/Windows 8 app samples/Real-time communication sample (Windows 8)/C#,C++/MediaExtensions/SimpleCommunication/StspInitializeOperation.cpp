//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include "StspInitializeOperation.h"
#include "StspMediaSink.h"

using namespace Stsp;

CInitializeOperation::CInitializeOperation()
{
}

CInitializeOperation::~CInitializeOperation()
{
    OnClose();
}

HRESULT CInitializeOperation::RuntimeClassInitialize(
    CMediaSink *pMediaSink, 
    ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *audioEncodingProperties, 
    ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *videoEncodingProperties)
{
    _spSink = pMediaSink;
    _spAudioEncodingProperties = audioEncodingProperties;
    _spVideoEncodingProperties = videoEncodingProperties;

    return Start();
}

// AsyncBase methods
HRESULT CInitializeOperation::OnStart(void)
{
    return _spSink->_TriggerInitialization();
}

void CInitializeOperation::OnClose(void)
{
    _spSink.ReleaseAndGetAddressOf();
    _spAudioEncodingProperties.ReleaseAndGetAddressOf();
    _spVideoEncodingProperties.ReleaseAndGetAddressOf();
}

void CInitializeOperation::OnCancel(void)
{
    _spSink.ReleaseAndGetAddressOf();
    _spAudioEncodingProperties.ReleaseAndGetAddressOf();
    _spVideoEncodingProperties.ReleaseAndGetAddressOf();
}
