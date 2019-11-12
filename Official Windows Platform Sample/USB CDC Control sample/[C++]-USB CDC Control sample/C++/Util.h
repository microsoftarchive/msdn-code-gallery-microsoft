//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    HRESULT IsSameData(Windows::Storage::Streams::IBuffer^ buffer, Windows::Storage::Streams::IBuffer^ otherBuffer, __out bool* isSame);

    Platform::String^ AsciiBufferToAsciiString(Windows::Storage::Streams::IBuffer^ buffer);

    Platform::String^ BinaryBufferToBinaryString(Windows::Storage::Streams::IBuffer^ buffer);

    Platform::String^ BinaryArrayToBinaryString(Platform::Array<uint8>^ _array);

    void GotoEndPosTextBox(Windows::UI::Xaml::Controls::TextBox^ textBox);

    Windows::UI::Xaml::Controls::Control^ FindControl(Windows::UI::Xaml::DependencyObject^ root, Platform::String^ name);
}