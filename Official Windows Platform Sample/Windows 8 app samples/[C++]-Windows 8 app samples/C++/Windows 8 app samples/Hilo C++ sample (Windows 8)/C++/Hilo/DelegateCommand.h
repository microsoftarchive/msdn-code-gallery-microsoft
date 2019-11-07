// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    public delegate void ExecuteDelegate(Platform::Object^ parameter);
    public delegate bool CanExecuteDelegate(Platform::Object^ parameter);

    // The DelegateCommand class implements Hilo commands. It is used by XAML button controls.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DelegateCommand sealed : public Windows::UI::Xaml::Input::ICommand
    {
    public:
        DelegateCommand(ExecuteDelegate^ execute, CanExecuteDelegate^ canExecute);

        virtual event Windows::Foundation::EventHandler<Platform::Object^>^ CanExecuteChanged;

        virtual void Execute(Platform::Object^ parameter);

        virtual bool CanExecute(Platform::Object^ parameter);

    private:
        ExecuteDelegate^ m_executeDelegate;
        CanExecuteDelegate^ m_canExecuteDelegate;
        bool m_canExecute;
    };
}
