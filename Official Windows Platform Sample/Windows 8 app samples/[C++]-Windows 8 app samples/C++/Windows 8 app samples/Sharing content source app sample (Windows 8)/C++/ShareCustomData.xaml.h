//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ShareCustomData.xaml.h
// Declaration of the ShareCustomData class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "ShareCustomData.g.h"
#include "MainPage.xaml.h"

#define CUSTOM_DATA_CONTENT "{ \
                                \"type\" : \"http://schema.org/Book\", \
                                \"properties\" : \
                                { \
                                    \"image\" : \"http://sourceurl.com/catcher-in-the-rye-book-cover.jpg\", \
                                    \"name\" : \"The Catcher in the Rye\", \
                                    \"bookFormat\" : \"http://schema.org/Paperback\", \
                                    \"author\" : \"http://sourceurl.com/author/jd_salinger.html\", \
                                    \"numberOfPages\" : 224, \
                                    \"publisher\" : \"Little, Brown, and Company\", \
                                    \"datePublished\" : \"1991-05-01\", \
                                    \"inLanguage\" : \"English\", \
                                    \"isbn\" : \"0316769487\" \
                                } \
                            }"

namespace SDKSample
{
    namespace ShareSource
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ShareCustomData sealed
        {
        public:
            ShareCustomData();
    
        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;
        };
    }
}
