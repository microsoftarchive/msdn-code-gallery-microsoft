// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "DiagnosticsHelper.h"
#include<Windows.h>

using namespace HttpClientTransportHelper::DiagnosticsHelper;
using namespace Platform;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;

void Diag::SetCoreDispatcher(_In_ CoreDispatcher^ dispatcher)
{
    coreDispatcher = dispatcher;
}

void Diag::SetDebugTextBlock(_In_ TextBlock^ debug)
{
    debugOutputTextBlock = debug;
}

void Diag::DebugPrint(_In_ String^ msg)
{
    if (coreDispatcher != nullptr)
    {
        coreDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([msg](){
            SYSTEMTIME systime;
            GetLocalTime(&systime);
            if (debugOutputTextBlock->Text != nullptr)
            {
                debugOutputTextBlock->Text = debugOutputTextBlock->Text + systime.wMonth + "/" +
                    systime.wDay + "/" + systime.wYear + " " + systime.wHour + ":" + systime.wMinute + ":" + " " + msg + "\r\n";
            }
            else
            {
                debugOutputTextBlock->Text = systime.wMonth + "/" +
                    systime.wDay + "/" + systime.wYear + " " + systime.wHour + ":" + systime.wMinute + ":" + " " + msg + "\r\n";
            }
        }));
    }
}
