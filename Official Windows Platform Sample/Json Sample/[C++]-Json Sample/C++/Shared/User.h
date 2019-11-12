//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#include "School.h"

namespace SDKSample
{
    namespace Json
    {
        // Mark as WebHostHidden since School is WebHostHidden.
        [Windows::Foundation::Metadata::WebHostHidden]
        [Windows::UI::Xaml::Data::Bindable]
        public ref class User sealed
        {
        public:
            User(void);
            User(Platform::String^ jsonString);
            Platform::String^ Stringify();

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

            property Windows::Foundation::Collections::IVector<SDKSample::Json::School^>^ Education
            {
                Windows::Foundation::Collections::IVector<SDKSample::Json::School^>^ get();
            }

            property double Timezone
            {
                double get();
                void set(double value);
            }

            property bool Verified
            {
                bool get();
                void set(bool value);
            }

        private:
            static Platform::String^ idKey;
            static Platform::String^ nameKey;
            static Platform::String^ educationKey;
            static Platform::String^ timezoneKey;
            static Platform::String^ verifiedKey;

            Platform::String^ id;
            Platform::String^ name;
            Platform::Collections::Vector<School^>^ education;
            double timezone;
            bool verified;
        };
    }
}
