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
    VideoBufferLock() : _bLockedBuffer(FALSE), _bLocked2D(FALSE), _bLocked2D2(FALSE), _pData(NULL), _lPitch(0) {}

    HRESULT Lock(__in IMFMediaBuffer *pBuffer, __in MF2DBuffer_LockFlags flags, __in UINT uiMinSize, __in LONG lDefaultStride)
    {
        HRESULT hr = S_OK;
        DWORD dwBufferCount = 0;
        DWORD maxSize = 0;
        BYTE *pbBufferStart = NULL;
        DWORD dwBufferLength = 0;

        assert(pBuffer);

        // if the lock is already locked, assert
        assert(!(_bLocked2D || _bLocked2D2 || _bLockedBuffer));
        _pInputBuffer = pBuffer;

        hr = _pInputBuffer.As(&_pInputBuffer2D2);
        if (SUCCEEDED(hr))
        {
            hr = _pInputBuffer2D2->Lock2DSize( flags, &_pData, &_lPitch, &pbBufferStart, &dwBufferLength );
            if (SUCCEEDED(hr))
            {
                if (dwBufferLength < uiMinSize || abs(_lPitch) < abs(lDefaultStride))
                {
                    _pInputBuffer2D2->Unlock2D();
                    hr = MF_E_BUFFERTOOSMALL;
                    goto done;
                }
                _bLocked2D2 = TRUE;
            }
        }
      
        if(!_bLocked2D2)
        {
            hr = _pInputBuffer.As(&_pInputBuffer2D);
            if (SUCCEEDED(hr))
            {
                hr = _pInputBuffer2D->Lock2D(&_pData, &_lPitch);
                if (SUCCEEDED(hr))
                {
                    if (abs(_lPitch) < abs(lDefaultStride))
                    {
                        _pInputBuffer2D->Unlock2D();
                        hr = MF_E_BUFFERTOOSMALL;
                        goto done;
                    }
                    _bLocked2D = TRUE;
                }
            }
        }
      
        if (!(_bLocked2D2 || _bLocked2D))
        {
            hr = _pInputBuffer->Lock(&_pData, &maxSize, NULL);
      
            if (SUCCEEDED(hr))
            {
                if (maxSize < uiMinSize)
                {
                    _pInputBuffer->Unlock();
                    hr = MF_E_BUFFERTOOSMALL;
                    goto done;
                }

                _bLockedBuffer = TRUE;
                _lPitch = lDefaultStride;
            }
        }

    done:
        return hr;
    }

    ~VideoBufferLock() { Unlock(); }

    HRESULT Unlock()
    {
        HRESULT hr = S_OK;

        if(_bLockedBuffer && _pInputBuffer)
        {
            hr = _pInputBuffer->Unlock();
            _bLockedBuffer = FALSE;
            _pInputBuffer = nullptr;
        }
        if(_bLocked2D && _pInputBuffer2D)
        {
            hr = _pInputBuffer2D->Unlock2D();
            _bLocked2D = FALSE;
            _pInputBuffer2D = nullptr;
        }
        if(_bLocked2D2 && _pInputBuffer2D2)
        {
            hr = _pInputBuffer2D2->Unlock2D();
            _bLocked2D2 = FALSE;
            _pInputBuffer2D2 = nullptr;
        }
        return hr;
    }

    BYTE *GetData(){ return _pData; }
    LONG GetStride(){ return _lPitch; }

private:
    ComPtr<IMFMediaBuffer> _pInputBuffer;
    BOOL _bLockedBuffer;

    ComPtr<IMF2DBuffer> _pInputBuffer2D;
    BOOL _bLocked2D;

    ComPtr<IMF2DBuffer2> _pInputBuffer2D2;
    BOOL _bLocked2D2;

    BYTE *_pData;
    LONG _lPitch;
};
