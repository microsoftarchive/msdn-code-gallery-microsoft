//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
#pragma once

#include <wrl.h>
#include <mfmediaengine.h>
#include <d3d11_1.h>
//#include <d3d10.h>
#include <ppltasks.h>
#include <agile.h>

#ifndef MEPLAYER_H
#define MEPLAYER_H

#define ME_CAN_SEEK 0x00000002

namespace MEDIA
{
    inline void ThrowIfFailed(HRESULT hr)
    {
        if (FAILED(hr))
        {
            // Set a breakpoint on this line to catch DirectX API errors
            throw Platform::Exception::CreateException(hr);
        }
    }
}

//-----------------------------------------------------------------------------
// MediaEngineNotifyCallback
//
// Defines the callback method to process media engine events.
//-----------------------------------------------------------------------------

ref struct MediaEngineNotifyCallback abstract
{
internal:
    virtual void OnMediaEngineEvent(DWORD meEvent) = 0;
};

// MEPlayer: Manages the Media Engine.

ref class MEPlayer: public MediaEngineNotifyCallback
{
    // DX11 related
    Microsoft::WRL::ComPtr<ID3D11Device>                m_spDX11Device;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext>         m_spDX11DeviceContext;
    Microsoft::WRL::ComPtr<IDXGIOutput>                 m_spDXGIOutput;
    Microsoft::WRL::ComPtr<IDXGISwapChain1>             m_spDX11SwapChain;
    Microsoft::WRL::ComPtr<IMFDXGIDeviceManager>        m_spDXGIManager;

    // Media Engine related
    Microsoft::WRL::ComPtr<IMFMediaEngine>              m_spMediaEngine;
    Microsoft::WRL::ComPtr<IMFMediaEngineEx>            m_spEngineEx;

    BSTR                                    m_bstrURL;
    BOOL                                    m_fPlaying;
    BOOL                                    m_fLoop;
    BOOL                                    m_fEOS;
    BOOL                                    m_fStopTimer;
    RECT                                    m_rcTarget;
    DXGI_FORMAT                             m_d3dFormat;
    MFARGB                                  m_bkgColor;

    HANDLE                                  m_TimerThreadHandle;
    CRITICAL_SECTION                        m_critSec;

    concurrency::task<Windows::Storage::StorageFile^>   m_pickFileTask;
    concurrency::cancellation_token_source              m_tcs;
    BOOL                                                m_fInitSuccess;    
    BOOL                                                m_fExitApp;
    BOOL                                                m_fUseDX;

internal:

    MEPlayer();

    // DX11 related
    void CreateDX11Device();
    void CreateBackBuffers();

    // Initialize/Shutdown
    void Initialize(Windows::UI::Core::CoreWindow^ window);
    void Shutdown();
    BOOL ExitApp();	

    // Media Engine related
    virtual void OnMediaEngineEvent(DWORD meEvent) override;

    // Media Engine related Options
    void AutoPlay(BOOL autoPlay)
    {
        if (m_spMediaEngine)
        {
            m_spMediaEngine->SetAutoPlay(autoPlay);
        }
    }

    void Loop()
    {
        if (m_spMediaEngine)
        {   
            (m_fLoop) ? m_fLoop = FALSE : m_fLoop = TRUE;
            m_spMediaEngine->SetLoop(m_fLoop);
        }
    }

    BOOL IsPlaying()
    {
        return m_fPlaying;
    }

    void CloseFilePicker()
    {
        m_tcs.cancel();
    }

    // Loading
    void OpenFile();
    void SetURL(Platform::String^ szURL);  
    void SetBytestream(Windows::Storage::Streams::IRandomAccessStream^ streamHandle);

    // Transport state
    void Play();
    void Pause();
    void FrameStep();

    // Audio
    void SetVolume(float fVol);
    void SetBalance(float fBal);
    void Mute(BOOL mute);

    // Video
    void ResizeVideo(HWND hwnd);
    void EnableVideoEffect(BOOL enable);

    // Seeking and duration.
    void GetDuration(double *pDuration, BOOL *pbCanSeek);
    void SetPlaybackPosition(float pos);    
    double  GetPlaybackPosition();    
    BOOL    IsSeeking();

    // Window Event Handlers
    void UpdateForWindowSizeChange();

    // Timer thread related
    void StartTimer();
    void StopTimer();	
    void OnTimer();
    DWORD RealVSyncTimer();

private:
    ~MEPlayer();
    Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
};

#endif /* MEPLAYER_H */
