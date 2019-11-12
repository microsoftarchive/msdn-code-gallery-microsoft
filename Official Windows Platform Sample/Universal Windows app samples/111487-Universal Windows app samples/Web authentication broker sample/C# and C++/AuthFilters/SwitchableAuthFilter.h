#pragma once
#include <ppltasks.h>
#include <map>
#include "OAuth2.h"

namespace AuthFilters
{
	// SwitchableAuthFilter picks one (or none) from a set of OAuth2Filters based on matching
	// the incoming request Uri against a template in the Oauth2Filter->AuthConfiguration.ApiUriPrefix
	// 
	// This way, the SwitchableAuthFilter can be pre-populated with a set of filters which can
	// authorize against different web services.  App requests to different web services will
	// automatically be routed to the correct sub-filter.
	//
	// Note that every time the SendRequest is called, the sub-filter's InnerFilter will be re-set
	// to the current SwitchableAuthFilter's innerFilter.
	public ref class SwitchableAuthFilter sealed : public Windows::Web::Http::Filters::IHttpFilter, ISetInnerFilter
	{
	public:
		SwitchableAuthFilter(Windows::Web::Http::Filters::IHttpFilter^ innerFilter);
		virtual ~SwitchableAuthFilter();
		virtual Windows::Foundation::IAsyncOperationWithProgress <
			Windows::Web::Http::HttpResponseMessage^,
			Windows::Web::Http::HttpProgress > ^ SendRequestAsync(Windows::Web::Http::HttpRequestMessage^ request);
		void AddOAuthFilter(AuthFilters::OAuthFilter^ newFilter);
		void AddOAuth2Filter(AuthFilters::OAuth2Filter^ newFilter);
		void AddFilter(Platform::String^ uriToMatch, Windows::Web::Http::Filters::IHttpFilter^ newFilter);
		void ClearAll();

	private:
		Windows::Web::Http::Filters::IHttpFilter^ m_InnerFilter;
	public:
		virtual Windows::Web::Http::Filters::IHttpFilter^ SetInnerFilter(Windows::Web::Http::Filters::IHttpFilter^ newValue)
		{
			auto ReturnValue = m_InnerFilter;
			m_InnerFilter = newValue;
			return ReturnValue;
		}

	private:
		//std::vector<AuthFilters::OAuth2Filter^> m_FilterVector;
		std::map<Platform::String^, Windows::Web::Http::Filters::IHttpFilter^> m_FilterMap;

	};
}