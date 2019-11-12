#pragma once
#include "OAuth2.h"
#include <ppltasks.h>

namespace AuthFilters
{

	public ref class OAuthFilter sealed : public Windows::Web::Http::Filters::IHttpFilter, ISetInnerFilter
	{
	public:
		OAuthFilter(Windows::Web::Http::Filters::IHttpFilter^ innerFilter);
		virtual ~OAuthFilter();
		virtual Windows::Foundation::IAsyncOperationWithProgress <
			Windows::Web::Http::HttpResponseMessage^,
			Windows::Web::Http::HttpProgress > ^ SendRequestAsync(Windows::Web::Http::HttpRequestMessage^ request);

	private:
		AuthConfigurationData m_AuthConfigurationData;
	public:
		property AuthConfigurationData AuthConfiguration
		{
			AuthConfigurationData get() { return m_AuthConfigurationData; }
			void set(AuthConfigurationData data) { m_AuthConfigurationData = data; }
		}
	private:
		Windows::Web::Http::Filters::IHttpFilter^ m_InnerFilter;

	public:
		virtual Windows::Web::Http::Filters::IHttpFilter^ SetInnerFilter(Windows::Web::Http::Filters::IHttpFilter^ newValue)
		{
			auto ReturnValue = m_InnerFilter;
			m_InnerFilter = newValue;
			return ReturnValue;
		}


	public:
		void Clear(); // Clears the login information i.e. the AccessToken.


	private:
		Platform::String^ m_RequestToken;
		Platform::String^ m_RequestSecret;
		Platform::String^ m_AccessToken;
		Platform::String^ m_OAuthToken;
		Platform::String^ m_OAuthTokenSecret;
		Windows::Storage::ApplicationDataContainer^ m_RoamingSettings;

		Platform::String^ GetNonce();
		Platform::String^ GetTimeStamp();

		/*
		virtual Windows::Foundation::IAsyncOperationWithProgress <
			Windows::Web::Http::HttpResponseMessage^,
			Windows::Web::Http::HttpProgress > ^ SendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);


		virtual Windows::Foundation::IAsyncOperationWithProgress <
			Windows::Web::Http::HttpResponseMessage^,
			Windows::Web::Http::HttpProgress > ^ CallWabAndSendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);
		*/
	};
}