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

using namespace Windows::Security::Credentials;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Web::Syndication;
using namespace Windows::Web::AtomPub;
using namespace Windows::Foundation;
using namespace Windows::Data::Html;

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"AtomPub"); 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

        bool TryGetUri(Platform::String^ uriString, Windows::Foundation::Uri^* uri)
        {
            // Create a Uri instance and catch exceptions related to invalid input. This method returns 'true'
            // if the Uri instance was successfully created and 'false' otherwise.
            try
            {
                *uri = ref new Windows::Foundation::Uri(StringHelper::Trim(uriString));
                return true;
            }
            catch (Platform::NullReferenceException^ exception)
            {
                NotifyUser("Error: URI must not be null or empty.", NotifyType::ErrorMessage);
            }
            catch (Platform::InvalidArgumentException^ exception)
            {
                NotifyUser("Error: Invalid URI", NotifyType::ErrorMessage);
            }

            return false;
        }

        bool HandleException(Platform::Exception^ exception, Windows::UI::Xaml::Controls::TextBox^ outputField)
        {
            SyndicationErrorStatus syndicationError = SyndicationError::GetStatus(exception->HResult);
            if (syndicationError != SyndicationErrorStatus::Unknown)
            {
                outputField->Text += "The response content is not valid. " +
                    "Please make sure to use a URI that points to an Atom feed.\r\n";
            }
            else
            {
                Windows::Web::WebErrorStatus webError = Windows::Web::WebError::GetStatus(exception->HResult);
                if (webError == Windows::Web::WebErrorStatus::Unauthorized)
                {
                    outputField->Text += "Incorrect username or password.\r\n";
                }
                else if (webError == Windows::Web::WebErrorStatus::Unknown)
                {
                    // Neither a syndication nor a web error.
                    return false;
                }
            }

            NotifyUser(exception->Message, NotifyType::ErrorMessage);

            return true;
        }

    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    class StringHelper
    {
    public:
        static Platform::String^ Trim(Platform::String^ s)
        {
            const WCHAR* first = s->Begin();
            const WCHAR* last = s->End();

            while (first != last && iswspace(*first))
            {
                ++first;
            }

            while (first != last && iswspace(last[-1]))
            {
                --last;
            }

            return ref new Platform::String(first, last - first);
        }

        static Platform::String^ Join(Platform::String^ separator, Windows::Foundation::Collections::IVectorView<Platform::String^>^ iv) 
        {
            const std::wstring sep(separator->Begin(), separator->End());
            std::wstring ret;

            for (unsigned int i = 0, n = iv->Size; i < n; ++i) 
            {
                if (i > 0) 
                {
                    ret += sep;
                }

                Platform::String^ elem = iv->GetAt(i);

                ret.append(elem->Begin(), elem->End());
            }

            return ref new Platform::String(ret.data(), ret.size());
        }
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CommonData sealed
    {
    public:
        static void Restore(Page^ inputFrame)
        {
            // Set authentication fields.
            ((TextBox^)inputFrame->FindName("ServiceAddressField"))->Text = baseUri;
            ((TextBox^)inputFrame->FindName("UserNameField"))->Text = user;
            ((PasswordBox^)inputFrame->FindName("PasswordField"))->Password = password;
        }

        static void Save(Platform::String^ baseUri, Platform::String^ user, Platform::String^ password)
        {
            // Keep values of authentication fields.
            CommonData::baseUri = baseUri;
            CommonData::user = user;
            CommonData::password = password;
        }

        static AtomPubClient^ GetClient()
        {
            AtomPubClient^ client;
        
            client = ref new AtomPubClient();
            client->BypassCacheOnRetrieve = true;
        
            if ((user != nullptr) && (password != nullptr))
            {
                client->ServerCredential = ref new PasswordCredential();
                client->ServerCredential->UserName = user;
                client->ServerCredential->Password = password;
            }
            else
            {
                client->ServerCredential = nullptr;
            }
        
            return client;
        }

        static property Platform::String^ EditUri
        {
            Platform::String^ get() 
            {  
                return editUri;
            }
        }

        static property Platform::String^ ServiceDocUri
        {
            Platform::String^ get() 
            {  
                return serviceDocUri;
            }
        }

        static property Platform::String^ FeedUri
        {
            Platform::String^ get()
            {
                return feedUri;
            }
        }

    private:
        static Platform::String^ baseUri;
        static Platform::String^ user;
        static Platform::String^ password;

        // The default Service Document and Edit 'URIs' for WordPress.
        static Platform::String^ editUri;
        static Platform::String^ serviceDocUri;
        static Platform::String^ feedUri;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class SyndicationItemIterator sealed
    {
    private:
        SyndicationFeed^ feed;
        unsigned int index;

    public:
        SyndicationItemIterator()
        {
            this->feed = nullptr;
            this->index = 0;
        }

        void AttachFeed(SyndicationFeed^ feed)
        {
            this->feed = feed;
            this->index = 0;
        }

        void MoveNext()
        {
            if (feed != nullptr && index < feed->Items->Size - 1)
            {
                index++;
            }
        }

        void MovePrevious()
        {
            if (feed != nullptr && index > 0)
            {
                index--;
            }
        }

        bool HasElements()
        {
            return feed != nullptr && feed->Items->Size > 0;
        }

        Platform::String^ GetTitle()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "(no title)";
            }

            if (feed->Items->GetAt(index)->Title != nullptr)
            {
                return HtmlUtilities::ConvertToText(feed->Items->GetAt(index)->Title->Text);
            }

            return "(no title)";
        }

        Platform::String^ GetContent()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "(no value)";
            }

            if ((feed->Items->GetAt(index)->Content != nullptr) && (feed->Items->GetAt(index)->Content->Text != nullptr))
            {
                return feed->Items->GetAt(index)->Content->Text;
            }

            return "(no value)";
        }

        Platform::String^ GetIndexDescription()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "0 of 0";
            }

            return (index + 1).ToString() + " of " + feed->Items->Size.ToString();
        }

        Uri^ GetEditUri()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return nullptr;
            }

            return feed->Items->GetAt(index)->EditUri;
        }

        SyndicationItem^ GetSyndicationItem()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return nullptr;
            }

            return feed->Items->GetAt(index);
        }
    };
}
