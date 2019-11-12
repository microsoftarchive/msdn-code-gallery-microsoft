//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteVerticalText.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

DWriteVerticalText::DWriteVerticalText()
{
}

void DWriteVerticalText::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Set Layout Sample Text
    m_readingDirection = ReadingDirectionLeftToRightTopToBottom;
    m_text =
        L"『世界人権宣言』\r\n"
        L"（1948.12.10 第３回国連総会採択）〈前文〉\r\n"
        L"\r\n"
        L"人類社会のすべての構成員の固有の尊厳と平等で譲ることのできない権利とを承認することは、世界における自由、正義及び平和の基礎であるので、\r\n"
        L"\r\n"
        L"人権の無視及び軽侮が、人類の良心を踏みにじった野蛮行為をもたらし、言論及び信仰の自由が受けられ、恐怖及び欠乏のない世界の到来が、一般の人々の最高の願望として宣言されたので、\r\n"
        L"\r\n"
        L"人間が専制と圧迫とに対する最後の手段として反逆に訴えることがないようにするためには、法の支配によって人権を保護することが肝要であるので、\r\n"
        L"\r\n"
        L"諸国間の友好関係の発展を促進することが肝要であるので、\r\n"
        L"\r\n"
        L"国際連合の諸国民は、国連憲章において、基本的人権、人間の尊厳及び価値並びに男女の同権についての信念を再確認し、かつ、一層大きな自由のうちで社会的進歩と生活水準の向上とを促進することを決意したので、\r\n"
        L"\r\n"
        L"加盟国は、国際連合と協力して、人権及び基本的自由の普遍的な尊重及び遵守の促進を達成することを誓約したので、\r\n"
        L"\r\n"
        L"これらの権利及び自由に対する共通の理解は、この誓約を完全にするためにもっとも重要であるので、\r\n"
        L"\r\n"
        L"よって、ここに、国連総会は、\r\n"
        L"\r\n"
        L"\r\n"
        L"社会の各個人及び各機関が、この世界人権宣言を常に念頭に置きながら、加盟国自身の人民の間にも、また、加盟国の管轄下にある地域の人民の間にも、これらの権利と自由との尊重を指導及び教育によって促進すること並びにそれらの普遍的措置によって確保することに努力するように、すべての人民とすべての国とが達成すべき共通の基準として、この人権宣言を公布する。\r\n"
        L"\r\n"
        L"第１条\r\n"
        L"すべての人間は、生まれながらにして自由であり、かつ、尊厳と権利と について平等である。人間は、理性と良心とを授けられており、互いに同 胞の精神をもって行動しなければならない。\r\n"
        L"\r\n"
        L"第２条"
        L"すべて人は、人種、皮膚の色、性、言語、宗教、政治上その他の意見、\r\n"
        L"\r\n"
        L"国民的もしくは社会的出身、財産、門地その他の地位又はこれに類するい\r\n"
        L"\r\n"
        L"かなる自由による差別をも受けることなく、この宣言に掲げるすべての権\r\n"
        L"\r\n"
        L"利と自由とを享有することができる。\r\n"
        L"\r\n"
        L"さらに、個人の属する国又は地域が独立国であると、信託統治地域で\r\n"
        L"\r\n"
        L"あると、非自治地域であると、又は他のなんらかの主権制限の下にあると\r\n"
        L"\r\n"
        L"を問わず、その国又は地域の政治上、管轄上又は国際上の地位に基ずくい\r\n"
        L"\r\n"
        L"かなる差別もしてはならない。\r\n"
        L"\r\n"
        L"第３条\r\n"
        L"すべての人は、生命、自由及び身体の安全に対する権利を有する。\r\n"
        L"\r\n"
        L"第４条\r\n"
        L"何人も、奴隷にされ、又は苦役に服することはない。奴隷制度及び奴隷\r\n"
        L"\r\n"
        L"売買は、いかなる形においても禁止する。\r\n"
        L"\r\n"
        L"第５条\r\n"
        L"何人も、拷問又は残虐な、非人道的なもしくは屈辱的な取扱もしくは刑\r\n"
        L"\r\n"
        L"罰を受けることはない。\r\n";

    m_fontName                        = L"";
    m_localeName                      = L"";
    m_textLength                      = 0;
    m_fontSize                        = 14;

    m_fontName = L"Meiryo UI";
    m_localeName = L"ja-jp";
    m_readingDirection = ReadingDirectionTopToBottomRightToLeft;

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            m_fontName,
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            m_fontSize,
            m_localeName,
            &m_textFormat
            )
        );
}

void DWriteVerticalText::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectWrite vertical text sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
}

void DWriteVerticalText::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    m_flowLayoutSource = new FlowLayoutSource;
    m_flowLayoutSink = new FlowLayoutSink(m_dwriteFactory.Get());
    m_flowLayout = new FlowLayout(m_dwriteFactory.Get());
    m_flowLayout->SetTextFormat(m_textFormat.Get());
    m_flowLayout->SetReadingDirection(m_readingDirection);
    m_flowLayout->SetReadingDirection(m_readingDirection);

    if (m_textLength == 0)
    {
        m_textLength = static_cast<UINT32>(wcsnlen(m_text, UINT32_MAX));
    }

    m_flowLayout->SetText(m_text, m_textLength);
    m_flowLayout->AnalyzeText();
    m_flowLayoutSource->SetSize(size.width, size.height);
    m_flowLayoutSource->Reset();
    m_flowLayoutSink->Reset();
    m_flowLayout->FlowText(m_flowLayoutSource.Get(), m_flowLayoutSink.Get());
}

void DWriteVerticalText::Render()
{
    m_d2dContext->BeginDraw();
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    m_flowLayoutSink->DrawGlyphRuns(m_d2dContext.Get(), m_renderingParams.Get(), m_blackBrush.Get());

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void DWriteVerticalText::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DWriteVerticalText::OnActivated);
}

void DWriteVerticalText::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DWriteVerticalText::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DWriteVerticalText::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &DWriteVerticalText::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void DWriteVerticalText::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void DWriteVerticalText::Run()
{
    m_window->Activate();

    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void DWriteVerticalText::Uninitialize()
{
}

void DWriteVerticalText::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void DWriteVerticalText::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void DWriteVerticalText::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void DWriteVerticalText::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DWriteVerticalText();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}