#pragma once
#include <mfapi.h>
#include <mftransform.h>
#include <mfidl.h>
#include <assert.h>
#include <wrl\client.h>

using namespace Microsoft::WRL;

class VideoBufferLock 
{
public:
    VideoBufferLock(_In_ IMFMediaBuffer *pBuffer, _In_ MF2DBuffer_LockFlags flags, _In_ LONG lHeightInPixels, _In_ LONG lDefaultStride) 
        : _fLockedBuffer(false)
        , _fLocked2D(false)
        , _fLocked2D2(false)
        , _pData(nullptr)
        , _lPitch(0) 
        , _spInputBuffer(pBuffer)
        , _lHeightInPixels(lHeightInPixels)
    {
        try
        {
            Lock(flags, lDefaultStride);
        }
        catch(Exception^)
        {
            Unlock();
            throw;
        }
    }

    ~VideoBufferLock() 
    { 
        Unlock(); 
    }

    HRESULT Unlock()
    {
        HRESULT hr = S_OK;

        if(_fLockedBuffer && _spInputBuffer)
        {
            hr = _spInputBuffer->Unlock();
            _fLockedBuffer = false;
            _spInputBuffer = nullptr;
        }
        if(_fLocked2D && _spInputBuffer2D)
        {
            hr = _spInputBuffer2D->Unlock2D();
            _fLocked2D = false;
            _spInputBuffer2D = nullptr;
        }
        if(_fLocked2D2 && _spInputBuffer2D2)
        {
            hr = _spInputBuffer2D2->Unlock2D();
            _fLocked2D2 = false;
            _spInputBuffer2D2 = nullptr;
        }
        return hr;
    }

    BYTE *GetData() const { return _pData; }
    BYTE *GetTopRow() const 
    {
        if (_fLockedBuffer && _lPitch < 0)
        {
            // We only do that when we locked IMFMediaBuffer.
            return _pData + abs(_lPitch) * (_lHeightInPixels - 1);
        }
        else
        {
            return _pData;
        }
    }
    LONG GetStride(){ return _lPitch; }

private:
    void Lock(_In_ MF2DBuffer_LockFlags flags, _In_ LONG lDefaultStride)
    {
        DWORD dwBufferCount = 0;
        DWORD maxSize = 0;
        BYTE *pbBufferStart = nullptr;
        DWORD dwBufferLength = 0;

        assert(_spInputBuffer != nullptr);

        // if the lock is already locked, assert
        assert(!(_fLocked2D || _fLocked2D2 || _fLockedBuffer));

        if (SUCCEEDED(_spInputBuffer.As(&_spInputBuffer2D2)))
        {
            if (SUCCEEDED(_spInputBuffer2D2->Lock2DSize( flags, &_pData, &_lPitch, &pbBufferStart, &dwBufferLength )))
            {
                if (dwBufferLength < _lHeightInPixels * abs(lDefaultStride) || abs(_lPitch) < abs(lDefaultStride))
                {
                    _spInputBuffer2D2->Unlock2D();
                    ThrowException(MF_E_BUFFERTOOSMALL);
                }
                _fLocked2D2 = true;
            }
        }
      
        if(!_fLocked2D2)
        {
            if (SUCCEEDED(_spInputBuffer.As(&_spInputBuffer2D)))
            {
                if (SUCCEEDED(_spInputBuffer2D->Lock2D(&_pData, &_lPitch)))
                {
                    if (abs(_lPitch) < abs(lDefaultStride))
                    {
                        _spInputBuffer2D->Unlock2D();
                        ThrowException(MF_E_BUFFERTOOSMALL);
                    }
                    _fLocked2D = true;
                }
            }
        }
      
        if (!_fLocked2D2 && !_fLocked2D)
        {
            ThrowIfError(_spInputBuffer->Lock(&_pData, &maxSize, nullptr));
      
            if (maxSize < _lHeightInPixels * abs(lDefaultStride))
            {
                _spInputBuffer->Unlock();
                ThrowException(MF_E_BUFFERTOOSMALL);
            }

            _fLockedBuffer = true;
            _lPitch = lDefaultStride;
        }
    }

private:
    ComPtr<IMFMediaBuffer> _spInputBuffer;
    bool _fLockedBuffer;

    ComPtr<IMF2DBuffer> _spInputBuffer2D;
    bool _fLocked2D;

    ComPtr<IMF2DBuffer2> _spInputBuffer2D2;
    bool _fLocked2D2;

    BYTE *_pData;
    LONG _lPitch;

    DWORD _lHeightInPixels;
};
