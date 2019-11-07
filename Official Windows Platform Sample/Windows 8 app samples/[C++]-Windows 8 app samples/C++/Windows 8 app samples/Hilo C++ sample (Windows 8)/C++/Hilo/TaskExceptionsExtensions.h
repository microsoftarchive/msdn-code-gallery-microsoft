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
    template<typename T>
    struct ObserveException
    {
        ObserveException(std::shared_ptr<ExceptionPolicy> handler) : m_handler(handler)
        {
        }

        concurrency::task<T> operator()(concurrency::task<T> antecedent) const
        {
            T result;
            try 
            {
                result = antecedent.get();
            }
            catch(const concurrency::task_canceled&)
            {
                // don't need to run canceled tasks through the policy
            }
            catch(const std::exception&)
            {
                auto translatedException = ref new Platform::FailureException();
                m_handler->HandleException(translatedException);
                throw;
            }
            catch(Platform::Exception^ ex)
            {
                m_handler->HandleException(ex);
                throw;
            }
            return antecedent;
        }

    private:
        std::shared_ptr<ExceptionPolicy> m_handler;
    };

    template<>
    concurrency::task<void> ObserveException<void>::operator()(concurrency::task<void> antecedent) const
    {
        try 
        {
            antecedent.get();
        }
        catch(const concurrency::task_canceled&)
        {
            // don't need to run canceled tasks through the policy
        }
        catch(const std::exception&)
        {
            auto translatedException = ref new Platform::FailureException();
            m_handler->HandleException(translatedException);
            throw;
        }
        catch(Platform::Exception^ ex)
        {
            m_handler->HandleException(ex);
            throw;
        }

        return antecedent;
    }
}