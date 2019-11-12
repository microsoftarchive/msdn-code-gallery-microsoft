#pragma once
#include "OpQueue.h"
#include "AsyncCB.h"
#include "critsec.h"

class CGeometricMediaStream;

extern wchar_t const __declspec(selectany) c_szGeometricScheme[] = L"myscheme";
extern wchar_t const __declspec(selectany) c_szGeometricSchemeWithColon[] = L"myscheme:";

enum GeometricShape
{
    GeometricShape_Square,
    GeometricShape_Circle,
    GeometricShape_Triangle,
    GeometricShape_Count,
};

extern LPCWSTR __declspec(selectany) c_arrShapeNames[] =
{
    L"square",
    L"circle",
    L"triangle",
};

// Possible states of the source object
enum SourceState
{
    // Invalid state, source cannot be used 
    SourceState_Invalid,
    // Opening 
    SourceState_Opening,
    // Starting
    SourceState_Starting,
    // Started
    SourceState_Started,
    // Stopped
    SourceState_Stopped,
    // Source is shut down
    SourceState_Shutdown,
};

// Base class representing asyncronous source operation
class CSourceOperation : public IUnknown
{
public:
    enum Type
    {
        // Start the source
        Operation_Start,
        // Stop the source
        Operation_Stop,
        // Set rate
        Operation_SetRate,
    };

public:
    CSourceOperation(Type opType);
    virtual ~CSourceOperation();
    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    Type GetOperationType() const {return _opType;}
    const PROPVARIANT& GetData() const { return _data;}
    HRESULT SetData(const PROPVARIANT& varData);

private:
    long                        _cRef;                      // reference count
    Type                        _opType;
    PROPVARIANT                 _data;
};

// Start operation
class CStartOperation : public CSourceOperation
{
public:
    CStartOperation(IMFPresentationDescriptor *pPD);
    ~CStartOperation();

    IMFPresentationDescriptor *GetPresentationDescriptor() {return _spPD.Get();}

private:
    ComPtr<IMFPresentationDescriptor> _spPD;   // Presentation descriptor
};

// SetRate operation
class CSetRateOperation : public CSourceOperation
{
public:
    CSetRateOperation(BOOL fThin, float flRate);
    ~CSetRateOperation();

    BOOL IsThin() const {return _fThin;}
    float GetRate() const {return _flRate;}

private:
    BOOL _fThin;
    float _flRate;
};

class CGeometricMediaSource
    : public OpQueue<CGeometricMediaSource, CSourceOperation>
    , public IMFMediaSourceEx
    , public IMFGetService
    , public IMFRateControl
{
public:
    static HRESULT CreateInstance(CGeometricMediaSource**ppSource);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback* pCallback,IUnknown* punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult* pResult, IMFMediaEvent** ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent** ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue);

    // IMFMediaSource
    IFACEMETHOD (CreatePresentationDescriptor) (IMFPresentationDescriptor** ppPresentationDescriptor);
    IFACEMETHOD (GetCharacteristics) (DWORD* pdwCharacteristics);
    IFACEMETHOD (Pause) ();
    IFACEMETHOD (Shutdown) ();
    IFACEMETHOD (Start) (
        IMFPresentationDescriptor* pPresentationDescriptor,
        const GUID* pguidTimeFormat,
        const PROPVARIANT* pvarStartPosition
        );
    IFACEMETHOD (Stop)();

    // IMFMediaSourceEx
    IFACEMETHOD (GetSourceAttributes) (_Outptr_ IMFAttributes **ppAttributes);
    IFACEMETHOD (GetStreamAttributes) (_In_ DWORD dwStreamIdentifier, _Outptr_ IMFAttributes **ppAttributes);
    IFACEMETHOD (SetD3DManager) (_In_opt_ IUnknown *pManager);

    // IMFGetService
    IFACEMETHOD (GetService) ( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject);

    // IMFRateControl
    IFACEMETHOD (SetRate) (BOOL fThin, float flRate);        
    IFACEMETHOD (GetRate) (_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate);

    // OpQueue
    __override HRESULT DispatchOperation(CSourceOperation *pOp);
    __override HRESULT ValidateOperation(CSourceOperation *pOp);

    // Called by the byte stream handler.
    HRESULT BeginOpen(LPCWSTR pszUrl, IMFAsyncCallback *pCB, IUnknown *pUnkState);
    HRESULT EndOpen(IMFAsyncResult *pResult);

    _Acquires_lock_(_critSec)
        void Lock() {_critSec.Lock();}

    _Releases_lock_(_critSec)
        void Unlock() {_critSec.Unlock();}

protected:
    CGeometricMediaSource(void);
    ~CGeometricMediaSource(void);

private:
    HRESULT Initialize();

    void HandleError(HRESULT hResult);    
    HRESULT CompleteOpen(HRESULT hResult);

    HRESULT DoStart(CStartOperation *pOp);
    HRESULT DoStop(CSourceOperation *pOp);
    HRESULT DoSetRate(CSetRateOperation *pOp);

    HRESULT ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD);
    HRESULT ParseServerUrl(LPCWSTR pszUrl, GeometricShape *pShape);

    HRESULT OnFinishOpen(IMFAsyncResult *pResult);

    BOOL IsRateSupported(float flRate, float *pflAdjustedRate);

    HRESULT CheckShutdown() const
    {
        if (_eSourceState == SourceState_Shutdown)
        {
            return MF_E_SHUTDOWN;
        }
        else
        {
            return S_OK;
        }
    }

private:
    long                        _cRef;                      // reference count
    CritSec                     _critSec;                   // critical section for thread safety
    SourceState                 _eSourceState;              // Flag to indicate if Shutdown() method was called.
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    ComPtr<IMFAsyncResult>      _spOpenResult;              // Asynchronous result of open operation
    ComPtr<CGeometricMediaStream> _spStream;                // We only have one stream
    ComPtr<IMFPresentationDescriptor> _spPresentationDescriptor;
    ComPtr<IMFDXGIDeviceManager> _spDeviceManager;
    ComPtr<IMFAttributes>       _spAttributes;
    float                       _flRate;

    // Asynchronous callbacks
    AsyncCallback<CGeometricMediaSource> _OnFinishOpenCB;         
};

