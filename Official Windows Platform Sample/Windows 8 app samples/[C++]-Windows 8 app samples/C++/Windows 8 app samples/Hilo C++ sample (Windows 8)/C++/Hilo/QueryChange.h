// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    // The QueryChange class subscribes to the ContentsChanged method of Windows::Storage::Search. It notifies Hilo when a 
    // change in the file system has occurred that would alter the results of the given file query.
    ref class QueryChange sealed
    {
    internal:
        QueryChange(Windows::Storage::Search::IStorageQueryResultBase^ query, std::function<void()> callback) : m_query(query), m_callback(callback)
        {
            m_watchToken = m_query->ContentsChanged::add(ref new Windows::Foundation::TypedEventHandler<Windows::Storage::Search::IStorageQueryResultBase^, Object^>(this, &QueryChange::OnFileQueryContentsChanged));
        }

    public:
        virtual ~QueryChange()
        {
            m_query->ContentsChanged::remove(m_watchToken);
        }

    private:
        Windows::Storage::Search::IStorageQueryResultBase^ m_query;
        std::function<void()> m_callback;
        Windows::Foundation::EventRegistrationToken m_watchToken;

        void OnFileQueryContentsChanged(Windows::Storage::Search::IStorageQueryResultBase^ sender, Platform::Object^ e)
        {
            // Since OnFileQueryContentsChanged may be called on any thread, we must dispatch the callback to the app's main thread,
            // on the assumption that the callback will modify XAML structures.
            Windows::UI::Core::CoreWindow^ wnd = Windows::ApplicationModel::Core::CoreApplication::MainView->CoreWindow;
            Windows::UI::Core::CoreDispatcher^ dispatcher = wnd->Dispatcher;
            dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this] () 
            {
                m_callback();
            }));
        }
    };
}