/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    SpbRequest.h

Abstract:

    This module contains the type definitions for the SPB accelerometer's
    SPB request class.

--*/

#ifndef _SPBREQUEST_H_
#define _SPBREQUEST_H_

#pragma once

#define SPB_REQUEST_TIMEOUT -1000000 //100ms

class CSpbRequest : 
    public CComObjectRoot,
    public CComCoClass<CSpbRequest, &CLSID_SpbRequest>,
    public IRequest
{
public:
    CSpbRequest();
    virtual ~CSpbRequest();
    
    DECLARE_NO_REGISTRY()
    DECLARE_NOT_AGGREGATABLE(CSpbRequest)

    BEGIN_COM_MAP(CSpbRequest)
        COM_INTERFACE_ENTRY(IRequest)
    END_COM_MAP()

// Public methods
public:

// COM Interface methods
public:
    // IRequest methods
    HRESULT STDMETHODCALLTYPE Initialize(
        _In_  IWDFDevice*        pWdfDevice,
        _In_  PCWSTR             pszTargetPath
        );

    HRESULT STDMETHODCALLTYPE CreateAndSendWrite(
        _In_reads_(inBufferSize)  BYTE*   pInBuffer,
        _In_                      SIZE_T  inBufferSize
        );

    HRESULT STDMETHODCALLTYPE CreateAndSendWriteReadSequence(
        _In_reads_(inBufferSize)     BYTE*   pInBuffer,
        _In_                         SIZE_T  inBufferSize,
        _Out_writes_(outBufferSize)  BYTE*   pOutBuffer,
        _In_                         SIZE_T  outBufferSize,
        _In_                         ULONG   delayInUs
        );

    HRESULT STDMETHODCALLTYPE Cancel(
        );

// Private methods
private:
    HRESULT CreateAndSendIoctl(
        _In_                      ULONG       ioctlCode,
        _In_reads_(inBufferSize)  BYTE*       pInBuffer,
        _In_                      SIZE_T      inBufferSize,
        _Out_                     ULONG_PTR*  pbytesTransferred
        );

// Private members
private:
    // Interface pointers
    CComPtr<IWDFDriver>        m_spWdfDriver;
    CComPtr<IWDFDevice>        m_spWdfDevice;
    CComPtr<IWDFRemoteTarget>  m_spRemoteTarget;
    CComPtr<IWDFIoRequest>     m_spWdfIoRequest;
    CComPtr<IWDFMemory>        m_spWdfMemory;

    // Protect request instance
    CComAutoCriticalSection    m_CriticalSection;

    // Track initialization state of request
    BOOL                       m_fInitialized;
};

OBJECT_ENTRY_AUTO(__uuidof(SpbRequest), CSpbRequest)

#endif // _SPBREQUEST_H_
