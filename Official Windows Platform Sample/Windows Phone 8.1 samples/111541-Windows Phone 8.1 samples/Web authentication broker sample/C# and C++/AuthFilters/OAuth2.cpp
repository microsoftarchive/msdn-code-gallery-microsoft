#include "pch.h"
#include "OAuth2.h"
#include <agents.h>
#include <winerror.h>
#include <ppltasks.h>

#include <string>
#include <sstream>
#include <vector>

using namespace Concurrency;
using namespace AuthFilters;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;


#include <collection.h>
#include <algorithm>
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Security::Authentication::Web;



OAuth2Filter::OAuth2Filter(IHttpFilter^ innerFilter)
{
	if (innerFilter == nullptr)
	{
		throw ref new Exception(E_INVALIDARG, "innerFilter cannot be null.");
	}
	m_InnerFilter = innerFilter;
	m_RoamingSettings = Windows::Storage::ApplicationData::Current->RoamingSettings;
}

OAuth2Filter::~OAuth2Filter()
{
}

IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ OAuth2Filter::SendRequestAsync(
	HttpRequestMessage^ request)
{
	// Get the old access token.  This lets people stay logged in
	// between sessions.
	if (m_AccessToken == nullptr)
	{
		if(m_RoamingSettings != nullptr)
		{
			String^ accessToken = safe_cast<String^>(m_RoamingSettings->Values->Lookup(AuthConfiguration.TechnicalName+"access_token"));
			if (accessToken)
			{
				m_AccessToken = accessToken;
			}
		}
	}

	if (m_AccessToken != nullptr)
	{
		return SendRequestWithAccessTokenAsync (request);
	}
	return CallWabAndSendRequestWithAccessTokenAsync (request);
}

void OAuth2Filter::Clear()
{
	m_AccessToken = nullptr;

	try
	{
		m_RoamingSettings->Values->Remove(AuthConfiguration.TechnicalName+"access_token");
	}
	catch (Exception^)
	{
		// API throws where there is successfully nothing found.
	}
}


IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ OAuth2Filter::CallWabAndSendRequestWithAccessTokenAsync(
	HttpRequestMessage^ request
	)
{
	return create_async([=](progress_reporter<HttpProgress> reporter, cancellation_token token)
	{
		Platform::String^ redirectUriString = AuthConfiguration.RedirectUri;
		if (redirectUriString == "")
		{
			redirectUriString = "http://localhost";
		}
		Platform::String^ uri  = AuthConfiguration.AuthorizationUri;
		uri += "?client_id=" + AuthConfiguration.ClientId;
		uri += "&redirect_uri=" + Windows::Foundation::Uri::EscapeComponent(redirectUriString);
		auto responseType = AuthConfiguration.ResponseType;
        if (responseType == "") 
        {
            // Token if we can, code if we must.  Assume that if the AuthorizationCodeToTokenUri was specified,
            // it must have been specified for a reason.
            responseType = "token";
            if (AuthConfiguration.AuthorizationCodeToTokenUri != "")
            {
                responseType = "code";
            }
        }
		uri += "&response_type=" + responseType;

		if (AuthConfiguration.Scope != "")
		{
			uri += "&scope=" + AuthConfiguration.Scope;
		}
		if (AuthConfiguration.Display != "")
		{
			uri += "&display=" + AuthConfiguration.Display;
		}
		if (AuthConfiguration.State != "")
		{
			uri += "&state=" + AuthConfiguration.State;
		}
		if (AuthConfiguration.AdditionalParameterName != "")
		{
			uri += "&" + AuthConfiguration.AdditionalParameterName;
			if (AuthConfiguration.AdditionalParameterValue != "")
			{
				uri += "=" + AuthConfiguration.AdditionalParameterValue;
			}
		}
		auto authUri = ref new Uri (uri);

		// WAB is in the Windows::Security::Authentication::Web:: namespace.
		auto wabOperation = WebAuthenticationBroker::AuthenticateAsync(WebAuthenticationOptions::None, authUri, ref new Uri(redirectUriString));
		return create_task (wabOperation)
			.then ([=](Windows::Security::Authentication::Web::WebAuthenticationResult^ wabResult)
		{
			//
			// Handle the results of the WAB call.  There are three possibilities:
			// 1. WAB succeeded and we have a new access_token 
			// 2. WAB succeeded and we have a new code.
			// 3. something else
			//
			// The next task will either do something for #2, or will be an empty async.
			//
			m_AccessToken = nullptr;
			HttpRequestMessage^ requestMessage = nullptr;
			if (wabResult->ResponseData != "")
			{
				auto resp = ref new Uri (wabResult->ResponseData);
				const wchar_t* ptr = resp->Fragment->Begin();
				if (*ptr == L'#') ptr++; // It actually always starts with a '#'
				std::wstring buffer (ptr);
				std::vector<std::wstring> fragments;
				std::wstringstream ss (ptr);
				std::wstring fragment;
				std::wstring access_token (L"access_token=");
				std::wstring refresh_token (L"refresh_token=");
				while (std::getline(ss, fragment, L'&'))
				{
					if (fragment.compare(0, access_token.size(), access_token)==0)
					{
						std::wstring value (fragment.substr(access_token.size()));
						m_AccessToken = ref new Platform::String(value.c_str());
						if (m_AccessToken != "")
						{
							auto values = m_RoamingSettings->Values;
							values->Insert(AuthConfiguration.TechnicalName+"access_token", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(m_AccessToken)));
						}
					}
				}
				std::wstring code_query (L"code=");
				Platform::String^ code;
				std::wstring query (resp->Query->Begin());
                if (m_AccessToken==nullptr && query.compare(1, code_query.size(), code_query)==0) 
                {
                    auto q = resp->Query->Begin();
                    if (q[0] == L'?') q += 1;
					std::wstringstream ss (q);
					std::wstring fragment;
					while (std::getline(ss, fragment, L'&'))
					{
						if (fragment.compare(0, code_query.size(), code_query)==0)
						{
							std::wstring value (fragment.substr(code_query.size()));
							code = ref new Platform::String(value.c_str());
						}
					}

                    requestMessage = ref new HttpRequestMessage(HttpMethod::Post, ref new Uri(AuthConfiguration.AuthorizationCodeToTokenUri));
					auto content = ref new Map<Platform::String^, Platform::String^>();
					content->Insert(ref new Platform::String(L"code"), code);
					content->Insert(ref new Platform::String(L"client_id"), AuthConfiguration.ClientId);
					content->Insert(ref new Platform::String(L"client_secret"), AuthConfiguration.ClientSecret);
					content->Insert(ref new Platform::String(L"redirect_uri"), redirectUriString);
					content->Insert(ref new Platform::String(L"grant_type"), ref new Platform::String(L"authorization_code"));
                    requestMessage->Content = ref new HttpFormUrlEncodedContent(content);
                }

			}

			auto nextOperation = requestMessage != nullptr ? m_InnerFilter->SendRequestAsync(requestMessage)
				: create_async([=](progress_reporter<HttpProgress> reporter, cancellation_token token){
					auto retval = ref new HttpResponseMessage(Windows::Web::Http::HttpStatusCode::None);
					retval->Content = ref new HttpStringContent(""); // just add in a blank string
					return retval;
				});

			return create_task (nextOperation);
		}).then([=](HttpResponseMessage^ response) {
			//
			// Might have a "blank" response; might have a real response.
			//
			return create_task (response->Content->ReadAsStringAsync());
		}).then([=](Platform::String^ jsonString) {
			//
			// Parse the string and pull out the access token
			//
			if (m_AccessToken == nullptr || m_AccessToken == "")
			{
				Windows::Data::Json::JsonValue^ json;
				bool jsonResult = Windows::Data::Json::JsonValue::TryParse(jsonString, &json);
				if (jsonResult)
				{
					auto TokenType = json->GetObject()->GetNamedString("token_type", "");
					m_AccessToken = json->GetObject()->GetNamedString("access_token", "");
					if (m_AccessToken != "")
					{
						auto values = m_RoamingSettings->Values;
						values->Insert(AuthConfiguration.TechnicalName+"access_token", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(m_AccessToken)));
					}
				}
			}

			auto sendOperation = SendRequestWithAccessTokenAsync (request);
			return create_task (sendOperation, token);
		});
	});
}


static Uri^ AppendQuery(Uri^ startingUri, Platform::String^ name, Platform::String^ value)
{
	Platform::String^ queryToAppend = (value == "") ? name : name + "=" + value;
	bool alreadyHasQuery = startingUri->Query != "";
	Platform::String^ queryChar = alreadyHasQuery ? "&" : "?";
	Platform::String^ newUriString = startingUri->AbsoluteUri + queryChar + queryToAppend;
	return ref new Uri(newUriString);
}

IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ OAuth2Filter::SendRequestWithAccessTokenAsync(
	HttpRequestMessage^ request)
{
	if (m_AccessToken == "")
	{
		// If there's no access token, just pass the request through.  Any failures
		// will be picked up by the service and returned.
	}
	else if (AuthConfiguration.AccessTokenLocation == "authorizationHeader")
	{
		request->Headers->Authorization = ref new Windows::Web::Http::Headers::HttpCredentialsHeaderValue("Bearer", m_AccessToken);
	}
	else // "query" or the default
	{
		auto accessTokenQueryParameterName = (AuthConfiguration.AccessTokenQueryParameterName != "") 
			? AuthConfiguration.AccessTokenQueryParameterName : "access_token";
		request->RequestUri = AppendQuery(request->RequestUri, accessTokenQueryParameterName, m_AccessToken);
	}
	if (AuthConfiguration.AdditionalParameterName != "")
	{
		request->RequestUri = AppendQuery(request->RequestUri, AuthConfiguration.AdditionalParameterName, AuthConfiguration.AdditionalParameterValue);
	}

	IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ operation = m_InnerFilter->SendRequestAsync(request);
	return operation;
}


