//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "GameUIConstants.h"
#include "DeviceResources.h"
#include "DirectXSample.h"
#include "GameInfoOverlay.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::UI::ViewManagement;
using namespace Windows::Graphics::Display;
using namespace D2D1;
using namespace GameControl;

static const D2D1_RECT_F titleRectangle = D2D1::RectF(
    GameInfoOverlayConstant::SideMargin,
    GameInfoOverlayConstant::TopMargin,
    GameInfoOverlayConstant::Width - GameInfoOverlayConstant::SideMargin,
    GameInfoOverlayConstant::TopMargin + GameInfoOverlayConstant::TitleHeight
    );
static const D2D1_RECT_F actionRectangle = D2D1::RectF(
    GameInfoOverlayConstant::SideMargin,
    GameInfoOverlayConstant::Height - (GameInfoOverlayConstant::ActionHeight + GameInfoOverlayConstant::BottomMargin),
    GameInfoOverlayConstant::Width - GameInfoOverlayConstant::SideMargin,
    GameInfoOverlayConstant::Height - GameInfoOverlayConstant::BottomMargin
    ); 
static const D2D1_RECT_F bodyRectangle = D2D1::RectF(
    GameInfoOverlayConstant::SideMargin,
    titleRectangle.bottom + GameInfoOverlayConstant::Separator,
    GameInfoOverlayConstant::Width - GameInfoOverlayConstant::SideMargin,
    actionRectangle.top - GameInfoOverlayConstant::Separator
    );

static const int bufferLength = 1000;
static char16 wsbuffer[bufferLength];

GameInfoOverlay::GameInfoOverlay(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_visible(false),
    m_titleString("Title"),
    m_bodyString("Body"),
    m_actionString("Action"),
    m_dpi(-1.0f)
{
    auto dwriteFactory = m_deviceResources->GetDWriteFactory();

    // Create D2D Resources
    DX::ThrowIfFailed(
        dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_MEDIUM,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            GameInfoOverlayConstant::TitlePointSize,
            L"en-us",   // locale
            &m_textFormatTitle
            )
        );

    DX::ThrowIfFailed(
        dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            GameInfoOverlayConstant::BodyPointSize,
            L"en-us",   // locale
            &m_textFormatBody
            )
        );

    DX::ThrowIfFailed(
        m_textFormatTitle->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );
    DX::ThrowIfFailed(
        m_textFormatTitle->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
        );
    DX::ThrowIfFailed(
        m_textFormatBody->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );
    DX::ThrowIfFailed(
        m_textFormatBody->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
        );
}

//----------------------------------------------------------------------

void GameInfoOverlay::CreateDeviceDependentResources()
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();
    m_dpi = -1.0f;

    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_textBrush
            )
        );
    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_backgroundBrush
            )
        );
    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(0xdb7100, 1.0f),
            &m_actionBrush
            )
        );
}

//----------------------------------------------------------------------

void GameInfoOverlay::CreateWindowSizeDependentResources()
{
    auto dpi = m_deviceResources->GetDpi();
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    // The GameInfoOverlay is not Window Size dependent, however it is DPI dependent,
    // so the only time resources need to be recreated is when the DPI changes.
    if (m_dpi == dpi)
    {
        return;
    }
    m_dpi = dpi;

    m_levelBitmap = nullptr;
    m_tooSmallBitmap = nullptr;

    // Create a D2D Bitmap to be used for Game Info Overlay when waiting to
    // start a level or when display game statistics.
    D2D1_BITMAP_PROPERTIES1 properties;
    properties.pixelFormat.format = DXGI_FORMAT_B8G8R8A8_UNORM;
    properties.pixelFormat.alphaMode = D2D1_ALPHA_MODE_PREMULTIPLIED;
    properties.dpiX = m_dpi;
    properties.dpiY = m_dpi;
    properties.bitmapOptions = D2D1_BITMAP_OPTIONS_TARGET;
    properties.colorContext = nullptr;
    DX::ThrowIfFailed(
        d2dContext->CreateBitmap(
            D2D1::SizeU(
                static_cast<UINT32>(GameInfoOverlayConstant::Width * m_dpi / 96.0f),
                static_cast<UINT32>(GameInfoOverlayConstant::Height * m_dpi / 96.0f)
                ),
            nullptr,
            0,
            &properties,
            &m_levelBitmap
            )
        );
    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));
    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        titleRectangle,
        m_textBrush.Get()
        );
    d2dContext->DrawText(
        m_bodyString->Data(),
        m_bodyString->Length(),
        m_textFormatBody.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );
    d2dContext->DrawText(
        m_actionString->Data(),
        m_actionString->Length(),
        m_textFormatBody.Get(),
        actionRectangle,
        m_actionBrush.Get()
        );
    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // Create the TooSmall Bitmap.
    DX::ThrowIfFailed(
        d2dContext->CreateBitmap(
        D2D1::SizeU(
        static_cast<UINT32>(GameInfoOverlayConstant::Width * m_dpi / 96.0f),
        static_cast<UINT32>(GameInfoOverlayConstant::Height * m_dpi / 96.0f)
        ),
        nullptr,
        0,
        &properties,
        &m_tooSmallBitmap
        )
        );
    d2dContext->SetTarget(m_tooSmallBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    Platform::String^ string = "Paused";

    d2dContext->DrawText(
        string->Data(),
        string->Length(),
        m_textFormatTitle.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );
    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

ID2D1Bitmap1* GameInfoOverlay::Bitmap()
{
    if (m_tooSmallActive)
    {
        return m_tooSmallBitmap.Get();
    }
    else
    {
        return m_levelBitmap.Get();
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::ReleaseDeviceDependentResources()
{
    m_levelBitmap.Reset();
    m_tooSmallBitmap.Reset();
    m_textBrush.Reset();
    m_backgroundBrush.Reset();
    m_actionBrush.Reset();
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetGameLoading(uint32 dots)
{
    int length;
    m_titleString = "Loading Resources";
    m_bodyString = "";

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&actionRectangle, m_backgroundBrush.Get());

    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        titleRectangle,
        m_textBrush.Get()
        );

    dots = dots % 10;
    for (length = 0; length < 25; length++)
    {
        wsbuffer[length] = L' ';
    }
    for (uint32 i = 0; i < dots; i++)
    {
        wsbuffer[length++] = 0x25CF;   // This is a Dot character in the font.
        wsbuffer[length++] = L' ';
        wsbuffer[length++] = L' ';
        wsbuffer[length++] = L' ';
    }

    m_bodyString = ref new Platform::String(wsbuffer, length);
    d2dContext->DrawText(
        m_bodyString->Data(),
        m_bodyString->Length(),
        m_textFormatBody.Get(),
        bodyRectangle,
        m_actionBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetGameStats(int maxLevel, int hitCount, int shotCount)
{
    int length;

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    m_titleString = "High Score";

    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        titleRectangle,
        m_textBrush.Get()
        );
    length = swprintf_s(
        wsbuffer,
        bufferLength,
        L"Levels Completed %d\nTotal Points %d\nTotal Shots %d",
        maxLevel,
        hitCount,
        shotCount
        );
    m_bodyString = ref new Platform::String(wsbuffer, length);
    d2dContext->DrawText(
        m_bodyString->Data(),
        m_bodyString->Length(),
        m_textFormatBody.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetGameOver(bool win, int maxLevel, int hitCount, int shotCount, int highScore)
{
    int length;

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    if (win)
    {
        m_titleString = "You WON!";
    }
    else
    {
        m_titleString = "Game Over";
    }
    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        titleRectangle,
        m_textBrush.Get()
        );
    length = swprintf_s(
        wsbuffer,
        bufferLength,
        L"Levels Completed %d\nTotal Points %d\nTotal Shots %d\n\nHigh Score %d\n",
        maxLevel,
        hitCount,
        shotCount,
        highScore
        );
    m_bodyString = ref new Platform::String(wsbuffer, length);
    d2dContext->DrawText(
        m_bodyString->Data(),
        m_bodyString->Length(),
        m_textFormatBody.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetLevelStart(int level, Platform::String^ objective, float timeLimit, float bonusTime)
{
    int length;

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    length = swprintf_s(wsbuffer, bufferLength, L"Level %d", level);
    m_titleString = ref new Platform::String(wsbuffer, length);
    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        titleRectangle,
        m_textBrush.Get()
        );

    if (bonusTime > 0.0f)
    {
        length = swprintf_s(
            wsbuffer,
            bufferLength,
            L"Objective: %s\nTime  Limit: %6.1f sec\nBonus Time: %6.1f sec\n",
            objective->Data(),
            timeLimit,
            bonusTime
            );
    }
    else
    {
        length = swprintf_s(
            wsbuffer,
            bufferLength,
            L"Objective: %s\nTime  Limit: %6.1f sec\n",
            objective->Data(),
            timeLimit
            );
    }
    m_bodyString = ref new Platform::String(wsbuffer, length);
    d2dContext->DrawText(
        m_bodyString->Data(),
        m_bodyString->Length(),
        m_textFormatBody.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetPause(int /* level */, int /* hitCount */, int /* shotCount */, float /*timeRemaining */)
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&titleRectangle, m_backgroundBrush.Get());
    d2dContext->FillRectangle(&bodyRectangle, m_backgroundBrush.Get());
    m_titleString = "Game Paused";
    m_bodyString = "";

    d2dContext->DrawText(
        m_titleString->Data(),
        m_titleString->Length(),
        m_textFormatTitle.Get(),
        bodyRectangle,
        m_textBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------

void GameInfoOverlay::ShowTooSmall()
{
    m_visible = true;
    m_tooSmallActive = true;
}

//----------------------------------------------------------------------

void GameInfoOverlay::HideTooSmall()
{
    m_visible = false;
    m_tooSmallActive = false;
}

//----------------------------------------------------------------------

void GameInfoOverlay::SetAction(GameInfoOverlayCommand action)
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->SetTarget(m_levelBitmap.Get());
    d2dContext->BeginDraw();
    d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    d2dContext->FillRectangle(&actionRectangle, m_backgroundBrush.Get());

    switch (action)
    {
    case GameInfoOverlayCommand::PlayAgain:
        m_actionString = "Tap to play again ...";
        break;
    case GameInfoOverlayCommand::PleaseWait:
        m_actionString = "Level loading, please wait ...";
        break;
    case GameInfoOverlayCommand::TapToContinue:
        m_actionString = "Tap to continue ...";
        break;
    default:
        m_actionString = "";
        break;
    }
    if (action != GameInfoOverlayCommand::None)
    {
        d2dContext->DrawText(
            m_actionString->Data(),
            m_actionString->Length(),
            m_textFormatBody.Get(),
            actionRectangle,
            m_actionBrush.Get()
            );
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

//----------------------------------------------------------------------
