//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>
namespace SDKSample
{
    public enum class NotifyType
    {
        StatusMessage,
        ErrorMessage,
    };

    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    class Status
    {
    public:
        static Platform::String^ FileAdded;
        static Platform::String^ FileRemoved;
        static Platform::String^ FileAddFailed;
    };

    public ref class PageSizeChangedEventArgs sealed
    {
    public:
        property Windows::UI::ViewManagement::ApplicationViewState ViewState
        {
            Windows::UI::ViewManagement::ApplicationViewState get()
            {
                return viewState;
            }

            void set(Windows::UI::ViewManagement::ApplicationViewState value)
            {
                viewState = value;
            }
        }

    private:
        Windows::UI::ViewManagement::ApplicationViewState viewState;
    };
}