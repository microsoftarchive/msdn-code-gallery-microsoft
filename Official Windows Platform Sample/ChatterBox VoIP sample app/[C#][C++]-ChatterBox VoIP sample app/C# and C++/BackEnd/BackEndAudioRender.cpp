/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "BackEndAudioRender.h"
#include "ApiLock.h"
#include "BackEndNativeBuffer.h"
#include "BackEndAudioHelpers.h"

using namespace Windows::System::Threading;

using namespace PhoneVoIPApp::BackEnd;

BackEndAudioRender::BackEndAudioRender(void) :
    m_pDefaultRenderDevice(NULL),
    m_pRenderClient(NULL),
    m_pClock(NULL),
    m_pVolume(NULL),
    m_nMaxFrameCount(0),
    m_pwfx(NULL),
    transportController(nullptr),
    started(false)
{
    this->onTransportMessageReceivedHandler = ref new MessageReceivedEventHandler(this, &BackEndAudioRender::OnTransportMessageReceived);
}


BackEndAudioRender::~BackEndAudioRender(void)
{
    if (transportController)
    {
        transportController->AudioMessageReceived -= onTransportMessageReceivedHandlerToken;
        transportController = nullptr;
    }
}
void BackEndAudioRender::Start()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (started)
        return;

    HRESULT hr =  InitRender();

    if (FAILED(hr))
    {
        Stop();
        throw ref new Platform::COMException(hr, L"Unable to start audio capture");
    }

    started = true;
}

void BackEndAudioRender::Stop()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (!started)
        return;

    started = false;
}

HRESULT BackEndAudioRender::InitRender()
{
    HRESULT hr = E_FAIL;

    LPCWSTR pwstrRendererId = GetDefaultAudioRenderId(AudioDeviceRole::Communications);

    if (NULL == pwstrRendererId)
    {
        hr = E_FAIL;
    }

    hr = ActivateAudioInterface(pwstrRendererId, __uuidof(IAudioClient2), (void**)&m_pDefaultRenderDevice);

        // Set the category through SetClientProperties
    AudioClientProperties properties = {};
    if (SUCCEEDED(hr))
    {
        properties.cbSize = sizeof AudioClientProperties;
        properties.eCategory = AudioCategory_Communications;
        hr = m_pDefaultRenderDevice->SetClientProperties(&properties);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetMixFormat(&m_pwfx);
    }

    WAVEFORMATEX format = {};
    if (SUCCEEDED(hr))
    {
        m_sourceFrameSizeInBytes = (m_pwfx->wBitsPerSample / 8) * m_pwfx->nChannels;
        //FillPcmFormat(format, m_pwfx->nChannels, m_pwfx->nSamplesPerSec, m_pwfx->wBitsPerSample); 
        hr = m_pDefaultRenderDevice->Initialize(AUDCLNT_SHAREMODE_SHARED,
            0,
            2000 * 10000,  // Seconds in hns
            0, // periodicity
            m_pwfx, 
            NULL);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetService(__uuidof(IAudioRenderClient), (void**)&m_pRenderClient);
    }
    
    // Check for other supported GetService interfaces as well
    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetService(__uuidof(IAudioClock), (void**)&m_pClock);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetService(__uuidof(ISimpleAudioVolume), (void**)&m_pVolume);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetBufferSize(&m_nMaxFrameCount);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->Start();
    }

    if (pwstrRendererId)
    {
        CoTaskMemFree((LPVOID)pwstrRendererId);
    }

    return hr;
}

void BackEndAudioRender::OnTransportMessageReceived(Windows::Storage::Streams::IBuffer^ stream, UINT64, UINT64)
{
    //
    // TODO: Add buffering to smooth out audio for both, WASAPI and XAUDIO2 cases
    //
    BYTE* pBuffer = NativeBuffer::GetBytesFromIBuffer(stream);
    int size = stream->Length;

    while (m_pRenderClient && size && pBuffer)
    {
        HRESULT hr = E_FAIL;
        unsigned int padding = 0;

        hr = m_pDefaultRenderDevice->GetCurrentPadding(&padding);
        if (SUCCEEDED(hr))
        {
            BYTE* pRenderBuffer = NULL;
            unsigned int incomingFrameCount = size / m_sourceFrameSizeInBytes;
            unsigned int framesToWrite = m_nMaxFrameCount - padding;
            
            if (framesToWrite > incomingFrameCount)
            {
                framesToWrite = incomingFrameCount;
            }

            if (framesToWrite)
            {
                hr = m_pRenderClient->GetBuffer(framesToWrite, &pRenderBuffer);

                if (SUCCEEDED(hr))
                {
                    unsigned int bytesToBeWritten = framesToWrite * m_sourceFrameSizeInBytes;

                    memcpy(pRenderBuffer, pBuffer, bytesToBeWritten);

                    // Release the buffer
                    m_pRenderClient->ReleaseBuffer(framesToWrite, 0);

                    pBuffer += bytesToBeWritten;
                    size -= bytesToBeWritten;
                }
            }
        }
    }
}

void BackEndAudioRender::SetTransport(BackEndTransport^ transport)
{
    transportController = transport;
    if (transportController != nullptr)
    {
        onTransportMessageReceivedHandlerToken = transportController->AudioMessageReceived += onTransportMessageReceivedHandler;
    }
}
