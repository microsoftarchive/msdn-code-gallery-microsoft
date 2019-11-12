#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace HttpClientSample
    {
        public ref class SlowInputStream sealed : public Windows::Storage::Streams::IInputStream
        {
        public:
            SlowInputStream(unsigned long long length);
            virtual ~SlowInputStream(void);
            virtual Windows::Foundation::IAsyncOperationWithProgress<
                Windows::Storage::Streams::IBuffer^,
                unsigned int>^ ReadAsync(
                    Windows::Storage::Streams::IBuffer^ buffer,
                    unsigned int count,
                    Windows::Storage::Streams::InputStreamOptions options);

        private:
            unsigned long long length;
            unsigned long long position;
        };
    }
}
