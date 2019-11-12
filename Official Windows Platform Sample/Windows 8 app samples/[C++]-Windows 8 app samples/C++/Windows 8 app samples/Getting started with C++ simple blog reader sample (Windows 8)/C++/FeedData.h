
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//feeddata.h

#pragma once
#include "pch.h"
#include <collection.h>


namespace SimpleBlogReader
{
    // To be bindable, a class must be defined within a namespace
    // and a bindable attribute needs to be applied.
    // A FeedItem represents a single blog post.
    [Windows::UI::Xaml::Data::Bindable]
    public ref class FeedItem sealed
    {
    public:
        FeedItem(void){}

        property Platform::String^ Title;
        property Platform::String^ Author;
        property Platform::String^ Content;      
        property Windows::Foundation::DateTime PubDate;      
        property Windows::Foundation::Uri^ Link;

    private:
        ~FeedItem(void){}
    };

    // A FeedData object represents a feed that contains 
    // one or more FeedItems. 
    [Windows::UI::Xaml::Data::Bindable]
    public ref class FeedData sealed
    {
    public:
        FeedData(void)
        {
            m_items = ref new Platform::Collections::Vector<FeedItem^>();
        }

        // The public members must be Windows Runtime types so that
        // the XAML controls can bind to them from a separate .winmd.
        property Platform::String^ Title;            
        property Windows::Foundation::Collections::IVector<FeedItem^>^ Items
        {
            Windows::Foundation::Collections::IVector<FeedItem^>^ get() {return m_items; }
        }

        property Platform::String^ Description;
        property Windows::Foundation::DateTime PubDate;
        property Platform::String^ Uri;

    private:
        ~FeedData(void){}

        Platform::Collections::Vector<FeedItem^>^ m_items;
    };   

    // A FeedDataSource represents a collection of FeedData objects
    // and provides the methods to download the source data from which
    // FeedData and FeedItem objects are constructed. This class is 
    // instantiated at startup by this declaration in the 
    // ResourceDictionary in app.xaml: <local:FeedDataSource x:Key="feedDataSource" />
    [Windows::UI::Xaml::Data::Bindable]
    public ref class FeedDataSource sealed
    {
    private:
        Platform::Collections::Vector<FeedData^>^ m_feeds;
        std::map<Platform::String^, concurrency::task_completion_event<FeedData^>> m_feedCompletionEvents;
        FeedData^ GetFeedData(Platform::String^ feedUri, Windows::Web::Syndication::SyndicationFeed^ feed);       

    public:
        FeedDataSource();
        property Windows::Foundation::Collections::IObservableVector<FeedData^>^ Feeds
        {
            Windows::Foundation::Collections::IObservableVector<FeedData^>^ get()
            {
                return this->m_feeds;
            }
        }
        void InitDataSource();
        static Windows::Foundation::IAsyncOperation<FeedData^>^ GetFeedAsync(Platform::String^ title);
        static FeedItem^ GetFeedItem(FeedData^ fd, Platform::String^ uniqueiD);
    };
}