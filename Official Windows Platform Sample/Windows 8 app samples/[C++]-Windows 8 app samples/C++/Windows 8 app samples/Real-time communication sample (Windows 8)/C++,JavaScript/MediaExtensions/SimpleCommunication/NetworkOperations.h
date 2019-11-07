//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <StspNetwork.h>
#include "critsec.h"

namespace Stsp { namespace Net {

    class CMediaBufferWrapper;

    interface DECLSPEC_UUID("FF7921F5-4884-46AF-949E-0C0808DFE9B0") DECLSPEC_NOVTABLE INetworkOperationContext : public IUnknown
    {
    public:
        virtual IMFAsyncCallback *GetCallback() const = 0;
        virtual IUnknown *GetState() const = 0;
    };

    interface DECLSPEC_UUID("7EC9F6FB-2CA3-46AA-87ED-283475D96015") DECLSPEC_NOVTABLE IAcceptOperationContext : public INetworkOperationContext
    {
    public:
        virtual IStreamSocket *GetSocket() const = 0;
        virtual HRESULT StartListeningAsync(unsigned short wPort) = 0;
        virtual void Close() = 0;
    };

    interface DECLSPEC_UUID("17F3E165-4D68-40CA-8BA6-6FE1236D2A78") DECLSPEC_NOVTABLE ISendOperationContext : public INetworkOperationContext
    {
    public:
        virtual UINT32 GetBytesWritten() const = 0;
        virtual IMFAsyncCallback *GetCallback() const = 0;
        virtual IUnknown *GetState() const = 0;
    };

    interface DECLSPEC_UUID("0B0B80D0-C904-42A4-A9DC-529F28D3F284") DECLSPEC_NOVTABLE IReceiveOperationContext : public INetworkOperationContext
    {
    public:
        virtual UINT32 GetBytesRead() const = 0;
    };

    class CConnectOperation : 
        public IAsyncActionCompletedHandler, 
        public INetworkOperationContext
    {
    public:
        static HRESULT CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncActionCompletedHandler **ppOp);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // IAsyncActionCompletedHandler
        IFACEMETHOD (Invoke) ( 
            _In_opt_ IAsyncAction *asyncInfo, AsyncStatus status);

        // INetworkOperationContext
        IMFAsyncCallback *GetCallback() const {return _spCallback.Get();}
        IUnknown *GetState() const { return _spState.Get(); }

    protected:
        CConnectOperation(IMFAsyncCallback *pCallback, IUnknown *pState);
        ~CConnectOperation();

    private:
        long _cRef;
        ComPtr<IMFAsyncCallback> _spCallback;
        ComPtr<IUnknown> _spState;
    };

    class CAcceptOperation : 
        public ITypedEventHandler<StreamSocketListener*,StreamSocketListenerConnectionReceivedEventArgs*>,
        public IAsyncActionCompletedHandler, 
        public IAcceptOperationContext
    {
    public:
        static HRESULT CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAcceptOperationContext **ppOp);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // ITypedEventHandler
        IFACEMETHOD(Invoke) (IStreamSocketListener *sender, IStreamSocketListenerConnectionReceivedEventArgs *args);

        // IAsyncActionCompletedHandler
        IFACEMETHOD (Invoke) ( 
            _In_opt_ IAsyncAction *asyncInfo, AsyncStatus status);

        // IAcceptOperationContext
        IStreamSocket *GetSocket() const {return _spSocket.Get();}
        HRESULT StartListeningAsync(unsigned short wPort);
        void Close();

        // INetworkOperationContext
        IMFAsyncCallback *GetCallback() const {return _spCallback.Get();}
        IUnknown *GetState() const { return _spState.Get(); }


    protected:
        CAcceptOperation(IMFAsyncCallback *pCallback, IUnknown *pState);
        ~CAcceptOperation();

        HRESULT Initialize();

    private:
        long _cRef;
        ComPtr<IMFAsyncCallback> _spCallback;
        ComPtr<IUnknown> _spState;
        ComPtr<IStreamSocket> _spSocket;
        EventRegistrationToken _token; 
        ComPtr<IStreamSocketListener> _spListener;
        CritSec _critSec;                // critical section for thread safety
    };

    class CSendOperation : 
        public IAsyncOperationWithProgressCompletedHandler<UINT32, UINT32>,
        public ISendOperationContext
    {
    public:
        static HRESULT CreateInstance(long cSendOperations, IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncOperationWithProgressCompletedHandler<UINT32, UINT32> **ppOp);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // IAsyncOperationWithProgressCompletedHandler
        IFACEMETHOD (Invoke) ( 
            _In_opt_ IAsyncOperationWithProgress<UINT32, UINT32> *asyncInfo, AsyncStatus status );

        // INetworkOperationContext
        IMFAsyncCallback *GetCallback() const {return _spCallback.Get();}
        IUnknown *GetState() const { return _spState.Get(); }

        // ISendOperationContext
        UINT32 GetBytesWritten() const {return _cbWritten;}

    protected:
        CSendOperation(long cSendOperations, IMFAsyncCallback *pCallback, IUnknown *pState);
        ~CSendOperation();

        HRESULT ReportResult();

    private:
        long _cRef;
        long _cSendOperations;
        ComPtr<IMFAsyncCallback> _spCallback;
        ComPtr<IUnknown> _spState;
        UINT32 _cbWritten;
        HRESULT _hResult;
    };

    class CReceiveOperation : 
        public IAsyncOperationWithProgressCompletedHandler<IBuffer*, UINT32>,
        public IReceiveOperationContext
    {
    public:
        static HRESULT CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncOperationWithProgressCompletedHandler<IBuffer*,UINT32> **ppOp);

        // IUnknown
        IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
        IFACEMETHOD_(ULONG, AddRef) ();
        IFACEMETHOD_(ULONG, Release) ();

        // IAsyncOperationWithProgressCompletedHandler
        IFACEMETHOD (Invoke) ( 
            _In_opt_ IAsyncOperationWithProgress<IBuffer*, UINT32> *asyncInfo, AsyncStatus status );

        // INetworkOperationContext
        IMFAsyncCallback *GetCallback() const {return _spCallback.Get();}
        IUnknown *GetState() const { return _spState.Get(); }

        // IReceiveOperationContext
        UINT32 GetBytesRead() const {return _cbRead;}

    protected:
        CReceiveOperation(IMFAsyncCallback *pCallback, IUnknown *pState);
        ~CReceiveOperation();

    private:
        long _cRef;
        ComPtr<IMFAsyncCallback> _spCallback;
        ComPtr<IUnknown> _spState;
        UINT32 _cbRead;
        HRESULT _hResult;
    };

} } // namespace Stsp::Net
