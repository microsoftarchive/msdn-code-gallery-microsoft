//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BasicSound.h"

#include "SoundFileReader.h"
#include "DirectXSample.h"

namespace
{
};

namespace
{
    const char16* APPLICATION_TITLE = L"XAudio2 sound playback sample";
    const float TITLE_MARGIN = 100.0f;

    // Audio File Names
    const char16* SOUND_FILE_LIST[] = {
            L"media\\wavs\\counting_adpcm.wav",
            L"media\\wavs\\electro_adpcm.wav",
            L"media\\wavs\\heli_adpcm.wav",
            L"media\\wavs\\hiphoppy_adpcm.wav",
            L"media\\wavs\\musicmono_adpcm.wav",
            L"media\\wavs\\pow_adpcm.wav",
            L"media\\wavs\\techno_adpcm.wav",
            nullptr
    };

    const uint32 SOUND_SAMPLE_RATE = 48000;


    // Constants used for displaying error message
    const char16 MSG_NEED_AUDIO[]       = L"No usable audio devices were found; install an audio device and restart the sample.";
    const float INFORMATION_START_X     = 32.0f;
    const float INFORMATION_START_Y     = 150.0f;

    const char16    FONT_LOCALE[]       = L"en-US";
    const char16    FONT_NAME[]         = L"Segoe UI";
    const float     FONT_SIZE_TEXT      = 18.0f;



}

//--------------------------------------------------------------------------------------
// Name: BasicSound constructor
// Desc: Default construct items
//--------------------------------------------------------------------------------------
BasicSound::BasicSound() :
    m_rectangleWidth(0),
    m_rectangleHeight(0),
    m_whiteBrush(nullptr),
    m_greenBrush(nullptr)
{
    try
    {
        m_soundPlayer = new XAudio2SoundPlayer(SOUND_SAMPLE_RATE);
    }
    catch (Platform::Exception^)
    {
        //
        // This can fail when there are no audio devices
        //
        m_soundPlayer = nullptr;
    }
}

//--------------------------------------------------------------------------------------
// Name: BasicSound destructor
// Desc: Cleanup allocated items
//--------------------------------------------------------------------------------------
BasicSound::~BasicSound()
{
    delete m_soundPlayer;
    m_soundPlayer = nullptr;
}

//--------------------------------------------------------------------------------------
// Name: BasicSound::CreateDeviceIndependentResources
// Desc: Common sample set up
//--------------------------------------------------------------------------------------
void BasicSound::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    //
    // Read the sound files and add them to the XAudio2 sound player, if we have sound.
    //
    if (m_soundPlayer == nullptr)
    {
        // We have no sound devices
        return;
    }
    for (size_t index = 0; SOUND_FILE_LIST[index]; ++index)
    {
        try
        {
            SoundFileReader nextSound(ref new Platform::String(SOUND_FILE_LIST[index]));
            (void)m_soundPlayer->AddSound(nextSound.GetSoundFormat(), nextSound.GetSoundData());
        }
        catch (Platform::FailureException^)
        {
            // If we have a failure, don't play that file, not worth stopping the whole sample
        }
    }

    // We should have at least one sound to play
    if (m_soundPlayer->GetSoundCount() == 0)
    {
        throw ref new Platform::FailureException();
    }
}

//--------------------------------------------------------------------------------------
// Name: BasicSound::CreateDeviceResources
// Desc: Common sample set up
//--------------------------------------------------------------------------------------
void BasicSound::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    //
    // Create the generic sample overlay
    //
    m_sampleOverlay = ref new SampleOverlay();
    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        ref new Platform::String(APPLICATION_TITLE)
        );


    //
    // Create the drawing brushes for the rectangles
    //
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Green),
            &m_greenBrush
            )
        );

    //
    // Setup the graphics objects related to drawing text
    //
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            FONT_NAME,
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            FONT_SIZE_TEXT,
            FONT_LOCALE,
            &m_dataTextFormat
            )
        );

    DX::ThrowIfFailed(
        m_dataTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );
    DX::ThrowIfFailed(
        m_dataTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_textBrush)
        );
}

//--------------------------------------------------------------------------------------
// Name: BasicSound::CreateWindowSizeDependentResources
// Desc:
//          XAudio2Engine
//          Mastering Voice
//          Create the SoundFilePlayer objects
//--------------------------------------------------------------------------------------
void BasicSound::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    if (m_soundPlayer != nullptr)
    {
        // Devide up the screen into sections based on the number of Wave Files
        D2D1_SIZE_F size = m_d2dContext->GetSize();
        m_rectangleWidth = (size.width / m_soundPlayer->GetSoundCount()) - 5;
        m_rectangleHeight = (size.height - TITLE_MARGIN)/2 - 5;
    }

    m_sampleOverlay->UpdateForWindowSizeChange();
}


void BasicSound::Render()
{
    // bind the render targets
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float ClearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        ClearColor
        );

    m_sampleOverlay->Render();

    m_d2dContext->BeginDraw();

    if (m_soundPlayer == nullptr)
    {
        //
        // If there is no audio, render a message.
        //
        D2D1_SIZE_F size = m_d2dContext->GetSize();
        D2D1_RECT_F pos = D2D1::RectF(INFORMATION_START_X, INFORMATION_START_Y, size.width, size.height);

        // Display a message when controller is not connected
        m_d2dContext->DrawText(
            MSG_NEED_AUDIO,
            static_cast<uint32>(::wcslen(MSG_NEED_AUDIO)),
            m_dataTextFormat.Get(),
            pos,
            m_textBrush.Get()
            );

    }
    else
    {
        //
        // There is Audio, display rectangles for clicking and playing indication
        //
        D2D1_RECT_F r = D2D1::RectF(0, TITLE_MARGIN, m_rectangleWidth, m_rectangleHeight + TITLE_MARGIN);
        for (size_t next = 0; next < m_soundPlayer->GetSoundCount(); ++next)
        {
            if (m_soundPlayer->IsSoundPlaying(next))
            {
                m_d2dContext->FillRectangle(&r, m_greenBrush.Get());
            }
            else
            {
                m_d2dContext->DrawRectangle(&r, m_whiteBrush.Get());
            }
            r.left += m_rectangleWidth;
            r.right += m_rectangleWidth;
        }
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void BasicSound::OnPointerPressed(_In_ Windows::UI::Input::PointerPoint^ p)
{
    if (m_soundPlayer == nullptr)
    {
        //
        // No audio, ignore pointer presses
        //
        return;
    }

    if ((p->RawPosition.Y > TITLE_MARGIN) && (p->RawPosition.Y < (m_rectangleHeight + TITLE_MARGIN)))
    {
        size_t which = static_cast<size_t>(p->RawPosition.X / m_rectangleWidth);
        if (which < m_soundPlayer->GetSoundCount())
        {
            if (m_soundPlayer->IsSoundPlaying(which))
            {
                // Stop playing sound
                m_soundPlayer->StopSound(which);
            }
            else
            {
                // Start non-playing sound
                m_soundPlayer->PlaySound(which);
            }
        }
    }
}

void BasicSound::OnSuspending()
{
    if (m_soundPlayer != nullptr)
    {
        m_soundPlayer->Suspend();
    }
}

void BasicSound::OnResuming()
{
    if (m_soundPlayer != nullptr)
    {
        m_soundPlayer->Resume();
    }
}

