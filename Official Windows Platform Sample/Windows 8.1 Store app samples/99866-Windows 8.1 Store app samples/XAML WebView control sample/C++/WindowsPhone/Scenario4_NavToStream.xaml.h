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
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "pch.h"
#include "Scenario4_NavToStream.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace WebViewControl
    {
        
		/// Sample URI resolver object for use with NavigateToLocalStreamUri
		/// This sample uses the local storage of the package as an example of how to write a resolver.
		/// The object needs to implement the IUriToStreamResolver interface
		/// 
		/// Note: If you really want to browse the package content, the ms-appx-web:// protocol demonstrated
		/// in scenario 3, is the simpler way to do that.
		public ref class StreamUriWinRTResolver sealed : public Windows::Web::IUriToStreamResolver
		{
		public:
			StreamUriWinRTResolver();
			virtual Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::IInputStream^>^ UriToStreamAsync(Windows::Foundation::Uri^ uri);

		private:
			Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::IInputStream^>^ GetFileStreamFromApplicationUriAsync(Windows::Foundation::Uri^ uri);
		};
		
		
		
		
		/// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario4 sealed
        {
        public:
            Scenario4();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
			StreamUriWinRTResolver^ myResolver;
    	};
    }
}
