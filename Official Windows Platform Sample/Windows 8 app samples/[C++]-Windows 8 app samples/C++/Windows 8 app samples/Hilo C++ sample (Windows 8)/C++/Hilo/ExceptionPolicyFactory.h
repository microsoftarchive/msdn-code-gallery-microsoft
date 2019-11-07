// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    class ExceptionPolicy;

    class ExceptionPolicyFactory
    {
    public:
        static std::shared_ptr<ExceptionPolicy> GetCurrentPolicy();

        static std::shared_ptr<ExceptionPolicy> m_policy;
    };
}