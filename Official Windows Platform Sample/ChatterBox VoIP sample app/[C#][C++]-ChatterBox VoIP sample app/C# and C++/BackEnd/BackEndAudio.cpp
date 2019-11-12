/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "BackEndAudio.h"
#include "ApiLock.h"
#include "BackEndNativeBuffer.h"

using namespace PhoneVoIPApp::BackEnd;
using namespace Windows::System::Threading;

// Begin of audio helpers
size_t inline GetWaveFormatSize(const WAVEFORMATEX& format)
{
    return (sizeof WAVEFORMATEX) + (format.wFormatTag == WAVE_FORMAT_PCM ? 0 : format.cbSize);
}

void FillPcmFormat(WAVEFORMATEX& format, WORD wChannels, int nSampleRate, WORD wBits)
{
    format.wFormatTag        = WAVE_FORMAT_PCM;
    format.nChannels         = wChannels;
    format.nSamplesPerSec    = nSampleRate;
    format.wBitsPerSample    = wBits;
    format.nBlockAlign       = format.nChannels * (format.wBitsPerSample / 8);
    format.nAvgBytesPerSec   = format.nSamplesPerSec * format.nBlockAlign;
    format.cbSize            = 0;
}

size_t BytesFromDuration(int nDurationInMs, const WAVEFORMATEX& format)
{
    return size_t(nDurationInMs * FLOAT(format.nAvgBytesPerSec) / 1000);
}

size_t SamplesFromDuration(int nDurationInMs, const WAVEFORMATEX& format)
{
    return size_t(nDurationInMs * FLOAT(format.nSamplesPerSec) / 1000);
}
// End of audio helpers

BOOL WaveFormatCompare(const WAVEFORMATEX& format1, const WAVEFORMATEX& format2)
{
    size_t cbSizeFormat1 = GetWaveFormatSize(format1);
    size_t cbSizeFormat2 = GetWaveFormatSize(format2);

    return (cbSizeFormat1 == cbSizeFormat2) && (memcmp(&format1, &format2, cbSizeFormat1) == 0);
}

BackEndAudio::BackEndAudio() :
    m_pDefaultRenderDevice(NULL),
    m_pRenderClient(NULL),
    m_pClock(NULL),
    m_pVolume(NULL),
    m_nMaxFrameCount(0),
    m_pwfx(NULL),
    m_pDefaultCaptureDevice(NULL),
    m_pCaptureClient(NULL),
    m_sourceFrameSizeInBytes(0),
    hCaptureEvent(NULL),
    hShutdownEvent(NULL),
    m_CaptureThread(nullptr),
    transportController(nullptr),
    started(false)
{
    this->onTransportMessageReceivedHandler = ref new MessageReceivedEventHandler(this, &BackEndAudio::OnTransportMessageReceived);
}

void BackEndAudio::OnTransportMessageReceived(Windows::Storage::Streams::IBuffer^ stream, UINT64, UINT64)
{
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

void BackEndAudio::Start()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (started)
        return;

    HRESULT hr = InitCapture();
    if (SUCCEEDED(hr))
    {
        hr =  InitRender();
    }

    if (SUCCEEDED(hr))
    {
        hr = StartAudioThreads();
    }

    if (FAILED(hr))
    {
        Stop();
        throw ref new Platform::COMException(hr, L"Unable to start audio");
    }

    started = true;
}

void BackEndAudio::Stop()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (!started)
        return;

    // Shutdown the threads
    if (hShutdownEvent)
    {
        SetEvent(hShutdownEvent);
    }

    if (m_CaptureThread != nullptr)
    {
        m_CaptureThread->Cancel();
        m_CaptureThread->Close();
        m_CaptureThread = nullptr;
    }

    if (m_pDefaultRenderDevice)
    {
        m_pDefaultRenderDevice->Stop();
    }

    if (m_pDefaultCaptureDevice)
    {
        m_pDefaultCaptureDevice->Stop();
    }

    if (m_pVolume)
    {
        m_pVolume->Release();
        m_pVolume = NULL;
    }
    if (m_pClock)
    {
        m_pClock->Release();
        m_pClock = NULL;
    }

    if (m_pRenderClient)
    {
        m_pRenderClient->Release();
        m_pRenderClient = NULL;
    }

    if (m_pCaptureClient)
    {
        m_pCaptureClient->Release();
        m_pCaptureClient = NULL;
    }

    if (m_pDefaultRenderDevice)
    {
        m_pDefaultRenderDevice->Release();
        m_pDefaultRenderDevice = NULL;
    }

    if (m_pDefaultCaptureDevice)
    {
        m_pDefaultCaptureDevice->Release();
        m_pDefaultCaptureDevice = NULL;
    }

    if (m_pwfx)
    {
        CoTaskMemFree((LPVOID)m_pwfx);
        m_pwfx = NULL;
    }

    if (hCaptureEvent)
    {
        CloseHandle(hCaptureEvent);
        hCaptureEvent = NULL;
    }

    if (hShutdownEvent)
    {
        CloseHandle(hShutdownEvent);
        hShutdownEvent = NULL;
    }

    started = false;
}

void BackEndAudio::CaptureThread(Windows::Foundation::IAsyncAction^ operation)
{
    HRESULT hr = m_pDefaultCaptureDevice->Start();
    BYTE *pLocalBuffer = new BYTE[MAX_RAW_BUFFER_SIZE];
    HANDLE eventHandles[] = {
                             hCaptureEvent,        // WAIT_OBJECT0 
                             hShutdownEvent        // WAIT_OBJECT0 + 1
                            };

    if (SUCCEEDED(hr) && pLocalBuffer)
    {
        unsigned int uAccumulatedBytes = 0;
        while (SUCCEEDED(hr))
        {
            DWORD waitResult = WaitForMultipleObjectsEx(SIZEOF_ARRAY(eventHandles), eventHandles, FALSE, INFINITE, FALSE);
            if (WAIT_OBJECT_0 == waitResult)
            {
                BYTE* pbData = nullptr;
                UINT32 nFrames = 0;
                DWORD dwFlags = 0;
                if (SUCCEEDED(hr))
                {
                    hr = m_pCaptureClient->GetBuffer(&pbData, &nFrames, &dwFlags, nullptr, nullptr);
                    unsigned int incomingBufferSize = nFrames * m_sourceFrameSizeInBytes;

                    if (MAX_RAW_BUFFER_SIZE - uAccumulatedBytes < incomingBufferSize)
                    {
                        // Send what has been accumulated
                        if (transportController)
                        {
                            transportController->WriteAudio(pLocalBuffer, uAccumulatedBytes);
                        }

                        // Reset our counter
                        uAccumulatedBytes = 0;
                    }

                    memcpy(pLocalBuffer + uAccumulatedBytes, pbData, incomingBufferSize);
                    uAccumulatedBytes += incomingBufferSize;
                }

                if (SUCCEEDED(hr))
                {
                    hr = m_pCaptureClient->ReleaseBuffer(nFrames);
                }
            }
            else if (WAIT_OBJECT_0 + 1 == waitResult)
            {
                // We're being asked to shutdown
                break;
            }
            else
            {
                // Unknown return value
                DbgRaiseAssertionFailure();
            }
        }
    }
    delete[] pLocalBuffer;
}

HRESULT BackEndAudio::StartAudioThreads()
{
    hShutdownEvent = CreateEventEx(NULL, NULL, CREATE_EVENT_MANUAL_RESET, EVENT_ALL_ACCESS);
    if (!hShutdownEvent)
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    m_CaptureThread = ThreadPool::RunAsync(ref new WorkItemHandler(this, &BackEndAudio::CaptureThread), WorkItemPriority::High, WorkItemOptions::TimeSliced);
    return S_OK;
}

BackEndAudio::~BackEndAudio()
{
    if (transportController)
    {
        transportController->AudioMessageReceived -= onTransportMessageReceivedHandlerToken;
        transportController = nullptr;
    }
}

HRESULT BackEndAudio::InitRender()
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

    WAVEFORMATEX* pwfx = nullptr;
    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultRenderDevice->GetMixFormat(&pwfx);
    }

    WAVEFORMATEX format = {};
    if (SUCCEEDED(hr))
    {
        FillPcmFormat(format, pwfx->nChannels, pwfx->nSamplesPerSec, pwfx->wBitsPerSample); 
        hr = m_pDefaultRenderDevice->Initialize(AUDCLNT_SHAREMODE_SHARED,
            0,
            2000 * 10000,  // Seconds in hns
            0, // periodicity
            &format, 
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

HRESULT BackEndAudio::InitCapture()
{
    HRESULT hr = E_FAIL;

    LPCWSTR pwstrCaptureId = GetDefaultAudioCaptureId(AudioDeviceRole::Communications);

    if (NULL == pwstrCaptureId)
    {
        hr = E_FAIL;
    }

    hr = ActivateAudioInterface(pwstrCaptureId, __uuidof(IAudioClient2), (void**)&m_pDefaultCaptureDevice);

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->GetMixFormat(&m_pwfx);
    }

    // Set the category through SetClientProperties
    AudioClientProperties properties = {};
    if (SUCCEEDED(hr))
    {
        properties.cbSize = sizeof AudioClientProperties;
        properties.eCategory = AudioCategory_Communications;
        hr = m_pDefaultCaptureDevice->SetClientProperties(&properties);
    }

    if (SUCCEEDED(hr))
    {
        //0x88140000 has AUDCLNT_STREAMFLAGS_EVENTCALLBACK in it already
        WAVEFORMATEX temp;
        FillPcmFormat(temp, 2, 48000, 32); 
        *m_pwfx = temp;
        m_sourceFrameSizeInBytes = (m_pwfx->wBitsPerSample / 8) * m_pwfx->nChannels;

        hr = m_pDefaultCaptureDevice->Initialize(AUDCLNT_SHAREMODE_SHARED, 0x88140000, 1000 * 10000, 0, m_pwfx, NULL);
    }

    if (SUCCEEDED(hr))
    {
        hCaptureEvent = CreateEventEx(NULL, NULL, 0, EVENT_ALL_ACCESS);
        if (NULL == hCaptureEvent)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->SetEventHandle(hCaptureEvent);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->GetService(__uuidof(IAudioCaptureClient), (void**)&m_pCaptureClient);
    }
    
    if (pwstrCaptureId)
    {
        CoTaskMemFree((LPVOID)pwstrCaptureId);
    }
    return hr;
}

void BackEndAudio::SetTransport(BackEndTransport^ transport)
{
    transportController = transport;
    if (transportController != nullptr)
    {
        onTransportMessageReceivedHandlerToken = transportController->AudioMessageReceived += onTransportMessageReceivedHandler;
    }
}
