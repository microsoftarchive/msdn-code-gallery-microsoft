#pragma once
#include <ppltasks.h>

namespace AuthFilters
{

	public interface class ISetInnerFilter
	{
		Windows::Web::Http::Filters::IHttpFilter^ SetInnerFilter(Windows::Web::Http::Filters::IHttpFilter^ newValue);
	};

	//
	// Configuration data needed for the OAuth2 filter.
	// Some of this (e.g., ClientId and ClientSecret)
	// are different for each app.
	// Others are site-specific but don't change per-client
	//
	public value struct AuthConfigurationData
	{
	public:
		Platform::String^ ClientId;
		Platform::String^ ClientSecret;

		//
		// Often the same for all clients (but are
		// different for different sites)
		//
		Platform::String^ TechnicalName; // e.g. facebook.com; used for saving access_token
        Platform::String^ RedirectUri; // = new Uri("http://localhost");
        Platform::String^ Scope; // = null;
        Platform::String^ Display; // = null;
        Platform::String^ State; // = null;
		Platform::String^ AdditionalParameterName;
		Platform::String^ AdditionalParameterValue;
		Platform::String^ ResponseType; // = null; // "token"; // Nice default for the best case (where we just need a client id).
        Platform::String^ AccessTokenLocation; // = "query";
        Platform::String^ AccessTokenQueryParameterName; // = "access_token";

        Platform::String^  AuthorizationUri;
        Platform::String^  AuthorizationCodeToTokenUri;

		//
		// Meta-data about the site.
		//
		Platform::String^ ApiUriPrefix;
		Platform::String^ SampleUri;
	};


	// A reasonable generic OAuth2 filter that has been tested against a number of popular
	// OAuth2-using sites.
	//
	// To use, you have to:
	// 1. Create an AuthConfigurationData struct that matches the web service requirements.
	// 2. 
    public ref class OAuth2Filter sealed : public Windows::Web::Http::Filters::IHttpFilter, ISetInnerFilter
    {
    public:
		OAuth2Filter(Windows::Web::Http::Filters::IHttpFilter^ innerFilter);
		virtual ~OAuth2Filter();
        virtual Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Web::Http::HttpResponseMessage^,
            Windows::Web::Http::HttpProgress>^ SendRequestAsync(Windows::Web::Http::HttpRequestMessage^ request);

	private:
		AuthConfigurationData m_AuthConfigurationData;
	public:
		property AuthConfigurationData AuthConfiguration
		{
			AuthConfigurationData get() { return m_AuthConfigurationData; }
			void set (AuthConfigurationData data) { m_AuthConfigurationData = data; }
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
		Platform::String^ m_AccessToken;
		Windows::Storage::ApplicationDataContainer^ m_RoamingSettings;

        virtual Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Web::Http::HttpResponseMessage^,
            Windows::Web::Http::HttpProgress>^ SendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);


        virtual Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Web::Http::HttpResponseMessage^,
            Windows::Web::Http::HttpProgress>^ CallWabAndSendRequestWithAccessTokenAsync(Windows::Web::Http::HttpRequestMessage^ request);


        //Windows::Web::Http::Filters::IHttpFilter^ innerFilter;
    };
}