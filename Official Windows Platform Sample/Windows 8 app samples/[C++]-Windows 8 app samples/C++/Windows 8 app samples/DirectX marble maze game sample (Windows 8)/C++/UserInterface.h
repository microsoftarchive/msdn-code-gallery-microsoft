//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

class TextStyle
{
public:
    TextStyle();

    void SetFontName(Platform::String^ fontName);
    void SetFontSize(float fontSize);
    void SetFontWeight(DWRITE_FONT_WEIGHT fontWeight);
    void SetFontStyle(DWRITE_FONT_STYLE fontStyle);
    void SetTextAlignment(DWRITE_TEXT_ALIGNMENT textAlignment);

    IDWriteTextFormat* GetTextFormat();

    bool HasTextFormatChanged() { return (m_textFormat == nullptr); }

private:
    Platform::String^       m_fontName;
    float                   m_fontSize;
    DWRITE_FONT_WEIGHT      m_fontWeight;
    DWRITE_FONT_STYLE       m_fontStyle;
    DWRITE_TEXT_ALIGNMENT   m_textAlignment;

    Microsoft::WRL::ComPtr<IDWriteTextFormat>   m_textFormat;
};

enum AlignType
{
    AlignNear,
    AlignCenter,
    AlignFar,
};

struct Alignment
{
    AlignType horizontal;
    AlignType vertical;
};

class ElementBase
{
public:
    virtual void Initialize() { }
    virtual void Update(float timeTotal, float timeDelta) { }
    virtual void Render() { }

    void SetAlignment(AlignType horizontal, AlignType vertical);
    virtual void SetContainer(const D2D1_RECT_F& container);
    void SetVisible(bool visible);

    D2D1_RECT_F GetBounds();

    bool IsVisible() const { return m_visible; }

protected:
    ElementBase();

    virtual void CalculateSize() { }

    Alignment       m_alignment;
    D2D1_RECT_F     m_container;
    D2D1_SIZE_F     m_size;
    bool            m_visible;
};

typedef std::set<ElementBase*> ElementSet;

class TextElement : public ElementBase
{
public:
    TextElement();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void SetTextColor(const D2D1_COLOR_F& textColor);
    void SetTextOpacity(float textOpacity);

    void SetText(__nullterminated WCHAR* text);
    void SetText(Platform::String^ text);

    TextStyle& GetTextStyle() { return m_textStyle; }

    void FadeOut(float fadeOutTime);

protected:
    virtual void CalculateSize();

    Platform::String^   m_text;
    D2D1_RECT_F         m_textExtents;

    TextStyle           m_textStyle;

    bool                m_isFadingOut;
    float               m_fadeStartingOpacity;
    float               m_fadeOutTime;
    float               m_fadeOutTimeElapsed;

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textColorBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_shadowColorBrush;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>       m_textLayout;

    void CreateTextLayout();
};

class CountdownTimer : public TextElement
{
public:
    CountdownTimer();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void StartCountdown(int seconds);

    bool IsCountdownComplete() const;

protected:
    float   m_elapsedTime;
    int     m_secondsRemaining;
};

class StopwatchTimer : public TextElement
{
public:
    StopwatchTimer();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void Start();
    void Stop();
    void Reset();

    float GetElapsedTime() const { return m_elapsedTime; };
    void SetElapsedTime(float time) { m_elapsedTime = time; };

    void GetFormattedTime(WCHAR* buffer, int length) const;
    template <size_t _Len>
    void GetFormattedTime(WCHAR (&buffer)[_Len]) const { GetFormattedTime(buffer, _Len); }

    static void GetFormattedTime(WCHAR* buffer, int length, float time);
    template <size_t _Len>
    static void GetFormattedTime(WCHAR (&buffer)[_Len], float time) { GetFormattedTime(buffer, _Len, time); }

protected:
    bool    m_active;
    float   m_elapsedTime;
};

class TextButton : public TextElement
{
public:
    TextButton();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void SetPadding(D2D1_SIZE_F padding);
    void SetSelected(bool selected);

    bool GetSelected() const { return m_selected; }

    void SetPressed(bool pressed);
    bool IsPressed() const { return m_pressed; }

protected:
    virtual void CalculateSize();

    D2D1_SIZE_F     m_padding;
    bool            m_selected;
    bool            m_pressed;
};

struct HighScoreEntry
{
    Platform::String^ tag;
    float elapsedTime;
    bool wasJustAdded;
};

#define MAX_HIGH_SCORES 5
typedef std::vector<HighScoreEntry> HighScoreEntries;

class HighScoreTable : public TextElement
{
public:
    HighScoreTable();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void AddScoreToTable(HighScoreEntry& entry);
    HighScoreEntries GetEntries() { return m_entries; };
    void Reset();

protected:
    HighScoreEntries    m_entries;

    void UpdateText();
};

class UserInterface
{
public:
    static UserInterface& GetInstance() { return m_instance; }

    static IDWriteFactory* GetDWriteFactory() { return m_instance.m_dwriteFactory.Get(); }
    static ID2D1DeviceContext* GetD2DContext() { return m_instance.m_d2dContext.Get(); }

    void Initialize(
        _In_ ID2D1Device*         d2dDevice,
        _In_ ID2D1DeviceContext*  d2dContext,
        _In_ IWICImagingFactory*  wicFactory,
        _In_ IDWriteFactory*      dwriteFactory
        );

    void Update(float timeTotal, float timeDelta);
    void Render();

    void RegisterElement(ElementBase* element);
    void UnregisterElement(ElementBase* element);

    void HitTest(D2D1_POINT_2F point);

private:
    UserInterface() { }
    ~UserInterface() { }

    static UserInterface m_instance;

    Microsoft::WRL::ComPtr<ID2D1Factory1>           m_d2dFactory;
    Microsoft::WRL::ComPtr<ID2D1Device>             m_d2dDevice;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    Microsoft::WRL::ComPtr<IDWriteFactory>          m_dwriteFactory;
    Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock>  m_stateBlock;
    Microsoft::WRL::ComPtr<IWICImagingFactory>      m_wicFactory;

    ElementSet m_elements;
};
