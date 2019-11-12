// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "DelegateCommand.h"

using namespace Hilo;
using namespace Platform;

DelegateCommand::DelegateCommand(ExecuteDelegate^ execute, CanExecuteDelegate^ canExecute) : m_executeDelegate(execute), m_canExecuteDelegate(canExecute)
{
}

void DelegateCommand::Execute(Object^ parameter)
{
    assert(m_executeDelegate != nullptr);
    if (nullptr != m_executeDelegate)
    {
        m_executeDelegate(parameter);
    }
}

bool DelegateCommand::CanExecute(Object^ parameter)
{
    if (m_canExecuteDelegate == nullptr)
    {
        return true;
    }

    bool temp = m_canExecuteDelegate(parameter);
    if (m_canExecute != temp)
    {
        m_canExecute = temp;
        CanExecuteChanged(this, nullptr);
    }
    return m_canExecute;
}