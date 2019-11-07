/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


Module Name:

    ReadWriteRequest.h

Abstract:

    This module contains the type definitions for the HID Read/write request
    completion callback class.
--*/

#pragma once

class CSensorDDI;

// This class implements IRequestCallbackRequestCompletion callback
// to get the request completion notification when request is sent down the
// stack. 

class CReadWriteRequest :
    public CComObjectRoot,
    public IRequestCallbackRequestCompletion
{
public:
    CReadWriteRequest();
    virtual ~CReadWriteRequest();

    DECLARE_NOT_AGGREGATABLE(CReadWriteRequest)

    BEGIN_COM_MAP(CReadWriteRequest)
        COM_INTERFACE_ENTRY(IRequestCallbackRequestCompletion)
    END_COM_MAP() 

    inline HRESULT EnterProcessing(DWORD64 dwControlFlag);
    inline void    ExitProcessing(DWORD64 dwControlFlag);

//
// Public methods
//
public:

    HRESULT InitializeRequest(IWDFDevice *pWdfDevice, CSensorDDI* pCallback);
    HRESULT UninitializeRequest();

    //Create and send read request down the stack
    HRESULT CreateAndSendReadRequest();

    //Create and send write request down the stack
    HRESULT CreateAndSendWriteRequest(BYTE* buffer, ULONG bufferSize);
    
    //Create and send ioctl request down the stack
    HRESULT CreateAndSendIoctlRequest(BYTE* buffer, ULONG bufferSize, ULONG ioctlCode, BYTE* pInBuffer);

    //Cancel and stop pending request
    HRESULT CancelAndStopPendingRequest();

    //
    // IRequestCallbackRequestCompletion method
    //

    virtual void STDMETHODCALLTYPE OnCompletion( _In_ IWDFIoRequest*                 FxRequest,
                                                 _In_ IWDFIoTarget*                  FxIoTarget,
                                                 _In_ IWDFRequestCompletionParams*   CompletionParams,
                                                 _In_ PVOID                          Context
                                                );
// Private data members.
private:
    
    // All the one-time setup HID Request parameters
    CComPtr<IWDFDevice>             m_spHidWdfDevice;
    CComPtr<IWDFDriver>             m_spHidWdfDriver;
    CComPtr<IWDFDriverCreatedFile>  m_spHidWdfFile;
    CComPtr<IWDFIoTarget>           m_spHidWdfIoTarget;

    CComPtr<IWDFIoRequest>          m_spHidWdfReadRequest;  // UMDF Framework Request object
    CSensorDDI*                     m_pParentCallback;  // Parent callback

    CComAutoCriticalSection         m_CriticalSection; // This is used to make all read/write calls thread safe

    BYTE    m_buffer[READ_BUFFER_SIZE]; // Buffer for read result
    ULONG   m_bytesRead;

    BOOL m_fResend; // Flag that indicates if a resend is needed
};
