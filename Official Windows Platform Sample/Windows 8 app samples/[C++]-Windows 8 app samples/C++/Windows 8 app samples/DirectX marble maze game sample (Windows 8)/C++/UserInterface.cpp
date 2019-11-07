//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "UserInterface.h"
#include "DirectXSample.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::UI::ViewManagement;
using namespace D2D1;

#pragma region TextStyle

TextStyle::TextStyle()
{
    m_fontName = L"Segoe UI";
    m_fontSize = 24.0f;
    m_fontWeight = DWRITE_FONT_WEIGHT_NORMAL;
    m_fontStyle = DWRITE_FONT_STYLE_NORMAL;
    m_textAlignment = DWRITE_TEXT_ALIGNMENT_LEADING;
}

void TextStyle::SetFontName(Platform::String^ fontName)
{
    if (!m_fontName->Equals(fontName))
    {
        m_fontName = fontName;
        m_textFormat = nullptr;
    }
}

void TextStyle::SetFontSize(float fontSize)
{
    if (m_fontSize != fontSize)
    {
        m_fontSize = fontSize;
        m_textFormat = nullptr;
    }
}

void TextStyle::SetFontWeight(DWRITE_FONT_WEIGHT fontWeight)
{
    if (m_fontWeight != fontWeight)
    {
        m_fontWeight = fontWeight;
        m_textFormat = nullptr;
    }
}

void TextStyle::SetFontStyle(DWRITE_FONT_STYLE fontStyle)
{
    if (m_fontStyle != fontStyle)
    {
        m_fontStyle = fontStyle;
        m_textFormat = nullptr;
    }
}

void TextStyle::SetTextAlignment(DWRITE_TEXT_ALIGNMENT textAlignment)
{
    if (m_textAlignment != textAlignment)
    {
        m_textAlignment = textAlignment;
        m_textFormat = nullptr;
    }
}

IDWriteTextFormat* TextStyle::GetTextFormat()
{
    if (m_textFormat == nullptr)
    {
        IDWriteFactory* dwriteFactory = UserInterface::GetDWriteFactory();

        DX::ThrowIfFailed(
            dwriteFactory->CreateTextFormat(
                m_fontName->Data(),
                nullptr,
                m_fontWeight,
                m_fontStyle,
                DWRITE_FONT_STRETCH_NORMAL,
                m_fontSize,
                L"en-US",
                &m_textFormat
                )
            );

        DX::ThrowIfFailed(
            m_textFormat->SetTextAlignment(m_textAlignment)
            );

        DX::ThrowIfFailed(
            m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
            );
    }

    return m_textFormat.Get();
}

#pragma endregion

#pragma region ElementBase

ElementBase::ElementBase():
    m_visible(false)
{
}

void ElementBase::SetAlignment(AlignType horizontal, AlignType vertical)
{
    m_alignment.horizontal = horizontal;
    m_alignment.vertical = vertical;
}

void ElementBase::SetContainer(const D2D1_RECT_F& container)
{
    m_container = container;
}

void ElementBase::SetVisible(bool visible)
{
    m_visible = visible;
}

D2D1_RECT_F ElementBase::GetBounds()
{
    CalculateSize();

    D2D1_RECT_F bounds = D2D1::RectF();

    switch (m_alignment.horizontal)
    {
    case AlignNear:
        bounds.left = m_container.left;
        bounds.right = bounds.left + m_size.width;
        break;

    case AlignCenter:
        bounds.left = m_container.left + (m_container.right - m_container.left - m_size.width) / 2.0f;
        bounds.right = bounds.left + m_size.width;
        break;

    case AlignFar:
        bounds.right = m_container.right;
        bounds.left = bounds.right - m_size.width;
        break;
    }

    switch (m_alignment.vertical)
    {
    case AlignNear:
        bounds.top = m_container.top;
        bounds.bottom = bounds.top + m_size.height;
        break;

    case AlignCenter:
        bounds.top = m_container.top + (m_container.bottom - m_container.top - m_size.height) / 2.0f;
        bounds.bottom = bounds.top + m_size.height;
        break;

    case AlignFar:
        bounds.bottom = m_container.bottom;
        bounds.top = bounds.bottom - m_size.height;
        break;
    }

    return bounds;
}

#pragma endregion

#pragma region TextElement

TextElement::TextElement():
    m_isFadingOut(false)
{
}

void TextElement::Initialize()
{
    ID2D1DeviceContext* d2dContext = UserInterface::GetD2DContext();

    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            ColorF(ColorF::White),
            &m_textColorBrush
            )
        );

    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            ColorF(ColorF::Black),
            &m_shadowColorBrush
            )
        );
}

void TextElement::Update(float timeTotal, float timeDelta)
{
    if (m_isFadingOut)
    {
        m_fadeOutTimeElapsed += timeDelta;

        float delta = min(1.0f, m_fadeOutTimeElapsed / m_fadeOutTime);
        SetTextOpacity((1.0f - delta) * m_fadeStartingOpacity);

        if (m_fadeOutTimeElapsed >= m_fadeOutTime)
        {
            m_isFadingOut = false;
            SetVisible(false);
        }
    }
}

void TextElement::Render()
{
    ID2D1DeviceContext* d2dContext = UserInterface::GetD2DContext();

    D2D1_RECT_F bounds = GetBounds();
    D2D1_POINT_2F origin = Point2F(
        bounds.left - m_textExtents.left,
        bounds.top - m_textExtents.top
        );

    m_shadowColorBrush->SetOpacity(m_textColorBrush->GetOpacity() * 0.5f);

    d2dContext->DrawTextLayout(
        Point2F(origin.x + 4.0f, origin.y + 4.0f),
        m_textLayout.Get(),
        m_shadowColorBrush.Get(),
        D2D1_DRAW_TEXT_OPTIONS_NO_SNAP
        );

    d2dContext->DrawTextLayout(
        origin,
        m_textLayout.Get(),
        m_textColorBrush.Get(),
        D2D1_DRAW_TEXT_OPTIONS_NO_SNAP
        );
}

void TextElement::SetTextColor(const D2D1_COLOR_F& textColor)
{
    m_textColorBrush->SetColor(textColor);
}

void TextElement::SetTextOpacity(float textOpacity)
{
    m_textColorBrush->SetOpacity(textOpacity);
}

void TextElement::SetText(__nullterminated WCHAR* text)
{
    SetText(ref new Platform::String(text));
}

void TextElement::SetText(Platform::String^ text)
{
    if (!m_text->Equals(text))
    {
        m_text = text;
        m_textLayout = nullptr;
    }
}

void TextElement::FadeOut(float fadeOutTime)
{
    m_fadeStartingOpacity = m_textColorBrush->GetOpacity();
    m_fadeOutTime = fadeOutTime;
    m_fadeOutTimeElapsed = 0.0f;
    m_isFadingOut = true;
}

void TextElement::CalculateSize()
{
    CreateTextLayout();

    DWRITE_TEXT_METRICS metrics;
    DWRITE_OVERHANG_METRICS overhangMetrics;
    DX::ThrowIfFailed(
        m_textLayout->GetMetrics(&metrics)
        );
    DX::ThrowIfFailed(
        m_textLayout->GetOverhangMetrics(&overhangMetrics)
        );

    m_textExtents = RectF(
        -overhangMetrics.left,
        -overhangMetrics.top,
        overhangMetrics.right + metrics.layoutWidth,
        overhangMetrics.bottom + metrics.layoutHeight
        );

    m_size = SizeF(
        m_textExtents.right - m_textExtents.left,
        m_textExtents.bottom - m_textExtents.top
        );
}

void TextElement::CreateTextLayout()
{
    if ((m_textLayout == nullptr) || m_textStyle.HasTextFormatChanged())
    {
        m_textLayout = nullptr;

        IDWriteFactory* dwriteFactory = UserInterface::GetDWriteFactory();

        DX::ThrowIfFailed(
            dwriteFactory->CreateTextLayout(
                m_text->Data(),
                m_text->Length(),
                m_textStyle.GetTextFormat(),
                m_container.right - m_container.left,
                m_container.bottom - m_container.top,
                &m_textLayout
                )
            );
    }
}

#pragma endregion

#pragma region CountdownTimer

CountdownTimer::CountdownTimer():
    m_elapsedTime(0.0f),
    m_secondsRemaining(0)
{
}

void CountdownTimer::Initialize()
{
    TextElement::Initialize();
}

void CountdownTimer::Update(float timeTotal, float timeDelta)
{
    if (m_secondsRemaining > 0)
    {
        m_elapsedTime += timeDelta;
        if (m_elapsedTime >= 1.0f)
        {
            m_elapsedTime -= 1.0f;

            if (--m_secondsRemaining > 0)
            {
                WCHAR buffer[4];
                swprintf_s(buffer, L"%2d", m_secondsRemaining);
                SetText(buffer);
                SetTextOpacity(1.0f);
                FadeOut(1.0f);
            }
            else
            {
                SetText(L"Go!");
                SetTextOpacity(1.0f);
                FadeOut(1.0f);
            }
        }
    }

    TextElement::Update(timeTotal, timeDelta);
}

void CountdownTimer::Render()
{
    TextElement::Render();
}

void CountdownTimer::StartCountdown(int seconds)
{
    m_secondsRemaining = seconds;
    m_elapsedTime = 0.0f;

    WCHAR buffer[4];
    swprintf_s(buffer, L"%2d", m_secondsRemaining);
    SetText(buffer);
    SetTextOpacity(1.0f);
    FadeOut(1.0f);
}

bool CountdownTimer::IsCountdownComplete() const
{
    return (m_secondsRemaining == 0);
}

#pragma endregion

#pragma region StopwatchTimer

StopwatchTimer::StopwatchTimer():
    m_active(false),
    m_elapsedTime(0.0f)
{
}

void StopwatchTimer::Initialize()
{
    TextElement::Initialize();
}

void StopwatchTimer::Update(float timeTotal, float timeDelta)
{
    if (m_active)
    {
        m_elapsedTime += timeDelta;

        WCHAR buffer[16];
        GetFormattedTime(buffer);
        SetText(buffer);
    }

    TextElement::Update(timeTotal, timeDelta);
}

void StopwatchTimer::Render()
{
    TextElement::Render();
}

void StopwatchTimer::Start()
{
    m_active = true;
}

void StopwatchTimer::Stop()
{
    m_active = false;
}

void StopwatchTimer::Reset()
{
    m_elapsedTime = 0.0f;
}

void StopwatchTimer::GetFormattedTime(WCHAR* buffer, int length) const
{
    GetFormattedTime(buffer, length, m_elapsedTime);
}

void StopwatchTimer::GetFormattedTime(WCHAR* buffer, int length, float time)
{
    int partial = (int)floor(fmodf(time * 10.0f, 10.0f));
    int seconds = (int)floor(fmodf(time, 60.0f));
    int minutes = (int)floor(time / 60.0f);
    swprintf_s(buffer, length, L"%02d:%02d.%01d", minutes, seconds, partial);
}

#pragma endregion

#pragma region TextButton

TextButton::TextButton():
    m_selected(false)
{
}

void TextButton::Initialize()
{
    TextElement::Initialize();
}

void TextButton::Update(float timeTotal, float timeDelta)
{
    TextElement::Update(timeTotal, timeDelta);
}

void TextButton::Render()
{
    ID2D1DeviceContext* d2dContext = UserInterface::GetD2DContext();

    if (m_selected)
    {
        D2D1_RECT_F bounds = GetBounds();

        d2dContext->DrawRectangle(
            bounds,
            m_textColorBrush.Get(),
            4.0f
            );
    }

    TextElement::Render();
}

void TextButton::SetPadding(D2D1_SIZE_F padding)
{
    m_padding = padding;
}

void TextButton::SetSelected(bool selected)
{
    m_selected = selected;
}

void TextButton::SetPressed(bool pressed)
{
    m_pressed = pressed;
}

void TextButton::CalculateSize()
{
    TextElement::CalculateSize();
    m_textExtents.left -= m_padding.width;
    m_textExtents.top -= m_padding.height;
    m_size.width += m_padding.width * 2;
    m_size.height += m_padding.height * 2;
}

#pragma endregion

#pragma region HighScoreTable

HighScoreTable::HighScoreTable()
{
}

void HighScoreTable::Initialize()
{
    TextElement::Initialize();
    UpdateText();
}

void HighScoreTable::Reset()
{
    m_entries.clear();
    UpdateText();
}

void HighScoreTable::Update(float timeTotal, float timeDelta)
{
    TextElement::Update(timeTotal, timeDelta);
}

void HighScoreTable::Render()
{
    TextElement::Render();
}

void HighScoreTable::AddScoreToTable(HighScoreEntry& entry)
{
    for (auto iter = m_entries.begin(); iter != m_entries.end(); ++iter)
    {
        iter->wasJustAdded = false;
    }

    entry.wasJustAdded = false;

    for (auto iter = m_entries.begin(); iter != m_entries.end(); ++iter)
    {
        if (entry.elapsedTime < iter->elapsedTime)
        {
            m_entries.insert(iter, entry);
            while (m_entries.size() > MAX_HIGH_SCORES)
                m_entries.pop_back();

            entry.wasJustAdded = true;
            UpdateText();
            return;
        }
    }

    if (m_entries.size() < MAX_HIGH_SCORES)
    {
        m_entries.push_back(entry);
        UpdateText();
        entry.wasJustAdded = true;
    }
}

void HighScoreTable::UpdateText()
{
    WCHAR formattedTime[32];
    WCHAR lines[1024] = { 0, };
    WCHAR buffer[128];

    swprintf_s(lines, L"High Scores:");
    for (unsigned int i = 0; i < MAX_HIGH_SCORES; ++i)
    {
        if (i < m_entries.size())
        {
            StopwatchTimer::GetFormattedTime(formattedTime, m_entries[i].elapsedTime);
            swprintf_s(
                buffer,
                (m_entries[i].wasJustAdded ? L"\n>> %s\t%s <<" : L"\n%s\t%s"),
                m_entries[i].tag->Data(),
                formattedTime
                );
            wcscat_s(lines, buffer);
        }
        else
        {
            wcscat_s(lines, L"\n-");
        }
    }

    SetText(lines);
}

#pragma endregion

#pragma region UserInterface

UserInterface UserInterface::m_instance;

void UserInterface::Initialize(
    _In_ ID2D1Device*         d2dDevice,
    _In_ ID2D1DeviceContext*  d2dContext,
    _In_ IWICImagingFactory*  wicFactory,
    _In_ IDWriteFactory*      dwriteFactory
    )
{
    m_wicFactory = wicFactory;
    m_dwriteFactory = dwriteFactory;
    m_d2dDevice = d2dDevice;
    m_d2dContext = d2dContext;

    ComPtr<ID2D1Factory> factory;
    d2dDevice->GetFactory(&factory);

    DX::ThrowIfFailed(
        factory.As(&m_d2dFactory)
        );

    DX::ThrowIfFailed(
        m_d2dFactory->CreateDrawingStateBlock(&m_stateBlock)
        );
}

void UserInterface::Update(float timeTotal, float timeDelta)
{
    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        (*iter)->Update(timeTotal, timeDelta);
    }
}

void UserInterface::Render()
{
    m_d2dContext->SaveDrawingState(m_stateBlock.Get());
    m_d2dContext->BeginDraw();
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    m_d2dContext->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE);

    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        if ((*iter)->IsVisible())
            (*iter)->Render();
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_d2dContext->RestoreDrawingState(m_stateBlock.Get());
}

void UserInterface::RegisterElement(ElementBase* element)
{
    m_elements.insert(element);
}

void UserInterface::UnregisterElement(ElementBase* element)
{
    auto iter = m_elements.find(element);
    if (iter != m_elements.end())
    {
        m_elements.erase(iter);
    }
}

inline bool PointInRect(D2D1_POINT_2F point, D2D1_RECT_F rect)
{
    if ((point.x < rect.left) ||
        (point.x > rect.right) ||
        (point.y < rect.top) ||
        (point.y > rect.bottom))
    {
        return false;
    }

    return true;
}

void UserInterface::HitTest(D2D1_POINT_2F point)
{
    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        if (!(*iter)->IsVisible())
            continue;

        TextButton* textButton = dynamic_cast<TextButton*>(*iter);
        if (textButton != nullptr)
        {
            D2D1_RECT_F bounds = (*iter)->GetBounds();
            textButton->SetPressed(PointInRect(point, bounds));
        }
    }
}

#pragma endregion
