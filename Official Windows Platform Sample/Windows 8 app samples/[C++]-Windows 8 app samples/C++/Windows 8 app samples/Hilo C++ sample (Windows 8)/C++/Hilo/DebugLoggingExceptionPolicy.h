// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once
#include "ExceptionPolicy.h"

namespace Hilo 
{
    class DebugLoggingExceptionPolicy: public ExceptionPolicy
    {
    public:
        virtual void HandleException(Platform::Exception^ exception);
    };
}