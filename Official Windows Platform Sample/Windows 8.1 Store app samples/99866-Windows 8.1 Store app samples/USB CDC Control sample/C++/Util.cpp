//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"

HRESULT SDKSample::IsSameData(Windows::Storage::Streams::IBuffer^ buffer, Windows::Storage::Streams::IBuffer^ otherBuffer, __out bool* isSame)
{
    if (!isSame)
    {
        return E_POINTER;
    }
    if (buffer->Length != otherBuffer->Length)
    {
        *isSame = false;
        return S_OK;
    }
    if (buffer->Length == 0)
    {
        *isSame = true;
        return S_OK;
    }

    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess;
    Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*) buffer);
    pBuffer.As(&pBufferByteAccess);
    BYTE* szBuffer = nullptr;
    pBufferByteAccess->Buffer(&szBuffer);
    if (!szBuffer)
    {
        return E_INVALIDARG;
    }

    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pOtherBufferByteAccess;
    Microsoft::WRL::ComPtr<IUnknown> pOtherBuffer((IUnknown*) otherBuffer);
    pOtherBuffer.As(&pOtherBufferByteAccess);
    BYTE* szOtherBuffer = nullptr;
    pOtherBufferByteAccess->Buffer(&szOtherBuffer);
    if (!szOtherBuffer)
    {
        return E_INVALIDARG;
    }

    *isSame = std::memcmp(szBuffer, szOtherBuffer, buffer->Length) == 0;
    return S_OK;
}

Platform::String^ SDKSample::AsciiBufferToAsciiString(Windows::Storage::Streams::IBuffer^ buffer)
{
    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess;
    Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*) buffer);
    pBuffer.As(&pBufferByteAccess);
    BYTE* szBuffer;
    pBufferByteAccess->Buffer(&szBuffer);
    wchar_t * wstr = new wchar_t[buffer->Length + 1];
    size_t wstrlen = 0;
    mbstowcs_s(&wstrlen, wstr, buffer->Length + 1, (char*) szBuffer, _TRUNCATE);
    auto str = ref new Platform::String(wstr);
    delete [] wstr;
    return str;
}

Platform::String^ SDKSample::BinaryBufferToBinaryString(Windows::Storage::Streams::IBuffer^ buffer)
{
    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess;
    Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*) buffer);
    pBuffer.As(&pBufferByteAccess);
    BYTE* szBuffer;
    pBufferByteAccess->Buffer(&szBuffer);
    auto str = ref new Platform::String();
    for (unsigned long i = 0; i < buffer->Length; i++)
    {
        wchar_t wstr[4];
        swprintf_s(wstr, 4, L"%02X ", szBuffer[i]);
        str += ref new Platform::String(wstr);
    }
    return str;
}

Platform::String^ SDKSample::BinaryArrayToBinaryString(Platform::Array<uint8>^ _array)
{
    auto str = ref new Platform::String();
    for (unsigned long i = 0; i < _array->Length; i++)
    {
        wchar_t wstr[4];
        swprintf_s(wstr, 4, L"%02X ", _array->get(i));
        str += ref new Platform::String(wstr);
    }
    return str;
}

void SDKSample::GotoEndPosTextBox(Windows::UI::Xaml::Controls::TextBox^ textBox)
{
    auto count = Windows::UI::Xaml::Media::VisualTreeHelper::GetChildrenCount(textBox);
    for (int i = 0; i < count; i++)
    {
        auto child = Windows::UI::Xaml::Media::VisualTreeHelper::GetChild(textBox, i);
        auto morecount = Windows::UI::Xaml::Media::VisualTreeHelper::GetChildrenCount(child);
        for (int j = 0; j < morecount; j++)
        {
            auto grandchild = Windows::UI::Xaml::Media::VisualTreeHelper::GetChild(child, j);
            auto scroll = dynamic_cast<Windows::UI::Xaml::Controls::ScrollViewer^>(grandchild) ;
            if (scroll != nullptr)
            {
                scroll->ChangeView(nullptr, scroll->ExtentHeight, nullptr);
                return;
            }
        }
    }
}

Windows::UI::Xaml::Controls::Control^ SDKSample::FindControl(Windows::UI::Xaml::DependencyObject^ root, Platform::String^ name)
{
    auto count = Windows::UI::Xaml::Media::VisualTreeHelper::GetChildrenCount(root);
    for (int i = 0; i < count; i++)
    {
        auto child = Windows::UI::Xaml::Media::VisualTreeHelper::GetChild(root, i);
        auto childControl = dynamic_cast<Windows::UI::Xaml::Controls::Control^>(child) ;
        if (childControl != nullptr)
        {
            if (childControl->Name == name)
            {
                return childControl;
            }
        }
        Windows::UI::Xaml::Controls::Control^ found = FindControl(child, name);
        if (found != nullptr && found->Name == name)
        {
            return found;
        }
    }
    return nullptr;
}