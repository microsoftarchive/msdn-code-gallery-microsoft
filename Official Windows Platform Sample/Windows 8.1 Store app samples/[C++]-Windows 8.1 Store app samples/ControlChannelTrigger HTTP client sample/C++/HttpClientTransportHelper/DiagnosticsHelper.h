// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include<Windows.h>

namespace HttpClientTransportHelper
{
    namespace DiagnosticsHelper
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Diag sealed
        {
        public:
            static void SetCoreDispatcher(_In_ Windows::UI::Core::CoreDispatcher^ dispatcher);
            static void SetDebugTextBlock(_In_ Windows::UI::Xaml::Controls::TextBlock^ debug);
            static void DebugPrint(_In_ Platform::String^ msg);

        private:
            static Windows::UI::Core::CoreDispatcher^ coreDispatcher;
            static Windows::UI::Xaml::Controls::TextBlock^ debugOutputTextBlock;
        };
    }
}
