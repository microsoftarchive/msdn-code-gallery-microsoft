//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "SimpleController.h"
#include "DirectXSample.h"

namespace
{
    const char16    APPLICATION_TITLE[] = L"XInput game controller sample";

    const float     CLEAR_COLOR[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    const char16    FONT_LOCAL[] = L"en-US";
    const char16    FONT_NAME[]  = L"Segoe UI";
    const float     FONT_SIZE_HEADER = 18.0f;
    const float     FONT_SIZE_TEXT   = 18.0f;

    const float     INFORMATION_START_X = 32.0f;
    const float     INFORMATION_START_Y = 150.0f;
    const float     LINE_HEIGHT  = FONT_SIZE_HEADER * 1.5f;

    const float     INPUT_LABEL_START = INFORMATION_START_X;
    const float     INPUT_DATA_START  = INFORMATION_START_X + 132.0f;
    const float     CAPS_LABEL_START  = INFORMATION_START_X + 300.0f;
    const float     CAPS_DATA_START   = INFORMATION_START_X + 400.0f;

    const char16    MSG_NEED_CONTROLLER[]       = L"Please attach an Xbox 360 common controller device.";
    const uint64    XINPUT_ENUM_TIMEOUT_MS = 2000;  // 2 seconds

    const char16    STATE_HEADER[]              = L"XInput State";
    const char16    LABEL_STATE_PACKET_NUMBER[] = L"Packet Number";
    const char16    LABEL_STATE_LEFT_TRIGGER[]  = L"Left Trigger";
    const char16    LABEL_STATE_RIGHT_TRIGGER[] = L"Right Trigger";
    const char16    LABEL_STATE_LEFT_THUMB_X[]  = L"Left Thumb X";
    const char16    LABEL_STATE_LEFT_THUMB_Y[]  = L"Left Thumb Y";
    const char16    LABEL_STATE_RIGHT_THUMB_X[] = L"Right Thumb X";
    const char16    LABEL_STATE_RIGHT_THUMB_Y[] = L"Right Thumb Y";
    const char16    LABEL_STATE_BUTTONS[]       = L"Buttons";

    const char16    CAPS_HEADER[]               = L"XInput Capabilties";
    const char16    LABEL_CAPS_TYPE[]           = L"Type";
    const char16    LABEL_CAPS_SUBTYPE[]        = L"Subtype";
    const char16    LABEL_CAPS_FLAGS[]          = L"Flags";
    const char16    VALUE_CAPS_WIRED[]          = L"Wired";
    const char16    VALUE_CAPS_WIRELESS[]       = L"Wireless";
    const char16    VALUE_CAPS_VOICE_SUPPORT[]  = L"Voice Support";
};

SimpleController::SimpleController() :
    m_isControllerConnected(false)
{
    m_lastEnumTime = ::GetTickCount64();
}

void SimpleController::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    //
    // Create and initialize the default Sample graphics object
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
    // Setup the local graphics objects
    //
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            FONT_NAME,
            nullptr,
            DWRITE_FONT_WEIGHT_SEMI_BOLD,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            FONT_SIZE_HEADER,
            FONT_LOCAL,
            &m_headerTextFormat
            )
        );
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            FONT_NAME,
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            FONT_SIZE_TEXT,
            FONT_LOCAL,
            &m_dataTextFormat
            )
        );

    DX::ThrowIfFailed(
        m_headerTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );
    DX::ThrowIfFailed(
        m_headerTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
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

void SimpleController::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void SimpleController::Render()
{
    // Bind the render target
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get());

    // Clear the render target
    m_d3dContext->ClearRenderTargetView(m_d3dRenderTargetView.Get(), CLEAR_COLOR);

    // Render the default sample header
    m_sampleOverlay->Render();

    // Render the controller data
    RenderControllerInput();
}

void SimpleController::FetchControllerInput()
{
    if (!m_isControllerConnected)
    {
        //
        // Ennumerating for XInput devices takes 'time' on the order of milliseconds.
        // Any time a device is not currently known as connected (not yet called XInput, or calling
        // an XInput function after a failure) ennumeration happens.
        // An app should avoid repeatedly calling XInput functions if there are no known devices connected
        // as this can slow down application performance.
        // This sample takes the simple approach of not calling XInput functions after failure
        // until a specified timeout has passed.
        //
        uint64 currentTime = ::GetTickCount64();
        if (currentTime - m_lastEnumTime < XINPUT_ENUM_TIMEOUT_MS)
        {
            return;
        }
        m_lastEnumTime = currentTime;

        // Check for controller connection by trying to get the capabilties
        uint32 capsResult = XInputGetCapabilities(0, XINPUT_FLAG_GAMEPAD, &m_xinputCaps);
        if (capsResult != ERROR_SUCCESS)
        {
            return;
        }

        // Device is connected
        m_isControllerConnected = true;
    }

    uint32 stateResult = XInputGetState(0, &m_xinputState);
    if (stateResult != ERROR_SUCCESS)
    {
        // Device is no longer connected
        m_isControllerConnected = false;
        m_lastEnumTime = ::GetTickCount64();
    }
}

void SimpleController::RenderControllerInput()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();
    D2D1_RECT_F pos = D2D1::RectF(INFORMATION_START_X, INFORMATION_START_Y, size.width, size.height);

    m_d2dContext->BeginDraw();

    if (!m_isControllerConnected)
    {
        //
        // Display message instructing user to connect a controller.
        //
        DrawText(MSG_NEED_CONTROLLER, pos);
    }
    else
    {
        //
        // Display State information
        //

        // Labels
        DrawHeader(STATE_HEADER, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_PACKET_NUMBER, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_LEFT_TRIGGER, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_RIGHT_TRIGGER, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_LEFT_THUMB_X, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_LEFT_THUMB_Y, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_RIGHT_THUMB_X, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_RIGHT_THUMB_Y, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_STATE_BUTTONS, pos);

        // Values
        pos.top = INFORMATION_START_Y + LINE_HEIGHT;
        pos.left = INPUT_DATA_START;
        DrawText(static_cast<uint32>(m_xinputState.dwPacketNumber), pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.bLeftTrigger, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.bRightTrigger, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.sThumbLX, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.sThumbLY, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.sThumbRX, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputState.Gamepad.sThumbRY, pos);
        pos.top += LINE_HEIGHT;
        DrawButtonText(m_xinputState.Gamepad.wButtons, pos);

        //
        // Display Capabilties
        //

        // Labels
        pos.top = INFORMATION_START_Y;
        pos.left = CAPS_LABEL_START;
        DrawHeader(CAPS_HEADER, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_CAPS_TYPE, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_CAPS_SUBTYPE, pos);
        pos.top += LINE_HEIGHT;
        DrawText(LABEL_CAPS_FLAGS, pos);

        // Values
        pos.top = INFORMATION_START_Y + LINE_HEIGHT;
        pos.left = CAPS_DATA_START;
        DrawText(m_xinputCaps.Type, pos);
        pos.top += LINE_HEIGHT;
        DrawText(m_xinputCaps.SubType, pos);
        pos.top += LINE_HEIGHT;
        if (m_xinputCaps.Flags & XINPUT_CAPS_WIRELESS)
        {
            DrawText(VALUE_CAPS_WIRELESS, pos);
        }
        else
        {
            DrawText(VALUE_CAPS_WIRED, pos);
        }
        if (m_xinputCaps.Flags & XINPUT_CAPS_VOICE_SUPPORTED)
        {
            pos.top += LINE_HEIGHT;
            DrawText(VALUE_CAPS_VOICE_SUPPORT, pos);
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

void SimpleController::Update(float /*timeTotal*/, float /*timeDelta*/)
{
    FetchControllerInput();
}

void SimpleController::DrawHeader(const char16* text, const D2D1_RECT_F& loc)
{
    m_d2dContext->DrawText(
        text,
        static_cast<UINT32>(::wcslen(text)),
        m_headerTextFormat.Get(),
        loc,
        m_textBrush.Get()
        );
}


void SimpleController::DrawText(const char16* text, const D2D1_RECT_F& loc)
{
    m_d2dContext->DrawText(
        text,
        static_cast<UINT32>(::wcslen(text)),
        m_dataTextFormat.Get(),
        loc,
        m_textBrush.Get()
        );
}

void SimpleController::DrawText(uint32 value, const D2D1_RECT_F& loc)
{
    char16 text[16];
    ::_snwprintf_s(text, sizeof(text)/sizeof(char16), L"0x%08X", value);
    DrawText(text, loc);
}

void SimpleController::DrawText(int16 value, const D2D1_RECT_F& loc)
{
    char16 text[16];
    ::_snwprintf_s(text, sizeof(text)/sizeof(char16), L"%05d", value);
    DrawText(text, loc);
}

void SimpleController::DrawText(uint8 value, const D2D1_RECT_F& loc)
{
    char16 text[8];
    ::_snwprintf_s(text, sizeof(text)/sizeof(char16), L"0x%02X", value);
    DrawText(text, loc);
}

void SimpleController::DrawButtonText(uint16 buttons, const D2D1_RECT_F& loc)
{
    char16 text[64];
    size_t where = 0;
    if (buttons & XINPUT_GAMEPAD_A)
    {
        text[where++] = L'A';
    }
    if (buttons & XINPUT_GAMEPAD_B)
    {
        text[where++] = L'B';
    }
    if (buttons & XINPUT_GAMEPAD_X)
    {
        text[where++] = L'X';
    }
    if (buttons & XINPUT_GAMEPAD_Y)
    {
        text[where++] = L'Y';
    }
    if (where != 0)
    {
        text[where++] = L' ';
    }

    size_t groupStart = where;
    if (buttons & XINPUT_GAMEPAD_DPAD_UP)
    {
        text[where++] = L'U';
    }
    if (buttons & XINPUT_GAMEPAD_DPAD_DOWN)
    {
        text[where++] = L'D';
    }
    if (buttons & XINPUT_GAMEPAD_DPAD_LEFT)
    {
        text[where++] = L'L';
    }
    if (buttons & XINPUT_GAMEPAD_DPAD_RIGHT)
    {
        text[where++] = L'R';
    }
    if (where != groupStart)
    {
        text[where++] = L' ';
    }

    if (buttons & XINPUT_GAMEPAD_LEFT_THUMB)
    {
        text[where++] = L'L';
        text[where++] = L'T';
        text[where++] = L' ';
    }
    if (buttons & XINPUT_GAMEPAD_RIGHT_THUMB)
    {
        text[where++] = L'R';
        text[where++] = L'T';
        text[where++] = L' ';
    }
    if (buttons & XINPUT_GAMEPAD_LEFT_SHOULDER)
    {
        text[where++] = L'L';
        text[where++] = L'S';
        text[where++] = L' ';
    }
    if (buttons & XINPUT_GAMEPAD_RIGHT_SHOULDER)
    {
        text[where++] = L'R';
        text[where++] = L'S';
        text[where++] = L' ';
    }
    if (buttons & XINPUT_GAMEPAD_START)
    {
        text[where++] = L'S';
        text[where++] = L't';
        text[where++] = L'a';
        text[where++] = L'r';
        text[where++] = L't';
        text[where++] = L' ';
    }
    if (buttons & XINPUT_GAMEPAD_BACK)
    {
        text[where++] = L'B';
        text[where++] = L'a';
        text[where++] = L'c';
        text[where++] = L'k';
    }
    text[where] = L'\0';

    DrawText(text, loc);
}
