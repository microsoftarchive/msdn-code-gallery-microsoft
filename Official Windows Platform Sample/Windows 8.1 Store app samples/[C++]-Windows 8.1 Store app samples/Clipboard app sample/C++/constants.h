//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    internal:
        static property Platform::String^ FEATURE_NAME
        {
            Platform::String^ get()
            {
                return "Clipboard C++";
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

        void NotifyUserBackgroundThread(Platform::String^ strMessage, NotifyType type);
        void EnableClipboardContentChangedNotifications(bool enable);
        Platform::String^ BuildClipboardFormatsOutputString();

    private:
        void OnClipboardChanged(Platform::Object^ sender, Platform::Object^ e);
        void OnWindowActivated(Platform::Object^ sender, Windows::UI::Core::WindowActivatedEventArgs^ e);
        void DisplayChangedFormats();
        void HandleClipboardChanged();

        static Platform::Array<Scenario>^ scenariosInner;
        Windows::Foundation::EventRegistrationToken contentChangedToken;
        Windows::Foundation::EventRegistrationToken windowActivatedToken;
        static bool windowActive;
        bool needToPrintClipboardFormat;
    };

    namespace Clipboard
    {
        // Sample specific types should be in this namespace
    }
}
