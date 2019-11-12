// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "DebugLoggingExceptionPolicy.h"

using namespace Hilo;
using namespace Platform;
using namespace std;

void DebugLoggingExceptionPolicy::HandleException(Exception^ exception)
{
    assert(exception != nullptr);
    wstringstream ss;
    ss << "[HR: " << exception->HResult << "] " << exception->Message->Data();
    OutputDebugString(ss.str().c_str());
}