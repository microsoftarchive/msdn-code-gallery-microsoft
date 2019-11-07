//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.g.h"

ref class Document;
ref class Element;

namespace Magazine
{
    public ref class MainPage sealed
    {
    public:
        MainPage();

        void LoadState(_In_ Windows::Foundation::Collections::IPropertySet^ localSettings);

        void SaveState(_In_ Windows::Foundation::Collections::IPropertySet^ localSettings);

    private:

        void DocumentLoaded(_In_ Document^ document);

        void OnDeviceLost();

        void OnWindowSizeChanged(
            _In_ Platform::Object^ sender,
            _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
            );

        // Current document loaded
        Document^ m_document;
    };
}
