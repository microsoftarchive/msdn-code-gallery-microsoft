//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    namespace Json
    {
        // Mark as WebHostHidden since Windows.Data.Json API is not available in JavaScript.
        [Windows::Foundation::Metadata::WebHostHidden]
        [Windows::UI::Xaml::Data::Bindable]
        public ref class School sealed
        {
        public:
            School(void);
            School(Windows::Data::Json::JsonObject^ jsonObject);
            Windows::Data::Json::JsonObject^ ToJsonObject();

            property Platform::String^ Id
            {
                Platform::String^ get();
                void set(Platform::String^ value);
            }

            property Platform::String^ Name
            {
                Platform::String^ get();
                void set(Platform::String^ value);
            }

            property Platform::String^ Type
            {
                Platform::String^ get();
                void set(Platform::String^ value);
            }

        private:
            static Platform::String^ idKey;
            static Platform::String^ nameKey;
            static Platform::String^ schoolKey;
            static Platform::String^ typeKey;

            Platform::String^ id;
            Platform::String^ name;
            Platform::String^ type;
        };
    }
}
