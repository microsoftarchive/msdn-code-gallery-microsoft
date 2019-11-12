// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ExceptionPolicyFactory.h"
#include "DebugLoggingExceptionPolicy.h"

using namespace Hilo;
using namespace std;

shared_ptr<ExceptionPolicy> ExceptionPolicyFactory::GetCurrentPolicy()
{
    return std::make_shared<DebugLoggingExceptionPolicy>();
}
