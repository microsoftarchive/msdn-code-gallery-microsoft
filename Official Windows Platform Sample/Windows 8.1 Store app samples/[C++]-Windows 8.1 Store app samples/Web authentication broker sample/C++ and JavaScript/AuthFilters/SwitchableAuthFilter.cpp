#include "pch.h"
#include "OAuth.h"
#include "OAuth2.h"
#include "SwitchableAuthFilter.h"
#include <winerror.h>
#include <ppltasks.h>
#include <collection.h>

using namespace AuthFilters;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;




SwitchableAuthFilter::SwitchableAuthFilter(IHttpFilter^ innerFilter)
{
	if (innerFilter == nullptr)
	{
		throw ref new Exception(E_INVALIDARG, "innerFilter cannot be null.");
	}
	this->m_InnerFilter = innerFilter;
}

SwitchableAuthFilter::~SwitchableAuthFilter()
{
	/*
	for (auto v = m_FilterVector.begin(); v != m_FilterVector.end(); v++)
	{
		(*v)->SetInnerFilter(nullptr);
		delete (*v);
	}
	m_FilterVector.clear();
	*/

	for (auto m = this->m_FilterMap.begin(); m != this->m_FilterMap.end(); m++)
	{
		delete m->first;
		IHttpFilter^ filter = m->second;
		ISetInnerFilter^ setInner = dynamic_cast<ISetInnerFilter^>(filter);
		if (setInner != nullptr)
		{
			setInner->SetInnerFilter(nullptr);
		}
		delete filter;
	}
	m_FilterMap.clear();


	if (m_InnerFilter != nullptr)
	{
		delete m_InnerFilter;
		m_InnerFilter = nullptr;
	}
}

void SwitchableAuthFilter::ClearAll()
{
	/*
	for (auto i = m_FilterVector.begin(); i != m_FilterVector.end(); i++)
	{
		(*i)->Clear();
	}
	*/
	for (auto i = m_FilterMap.begin(); i != m_FilterMap.end(); i++)
	{
		//(*i)->Clear();
		auto f1 = dynamic_cast<OAuthFilter^>(i->second);
		if (f1 != nullptr)
		{
			f1->Clear();
		}
		auto f2 = dynamic_cast<OAuth2Filter^>(i->second);
		if (f2 != nullptr)
		{
			f2->Clear();
		}
	}
}


void SwitchableAuthFilter::AddOAuthFilter(AuthFilters::OAuthFilter^ newFilter)
{
	m_FilterMap[newFilter->AuthConfiguration.ApiUriPrefix] = newFilter;
}


void SwitchableAuthFilter::AddOAuth2Filter(AuthFilters::OAuth2Filter^ newFilter)
{
	//m_FilterVector.push_back(newFilter);
	m_FilterMap[newFilter->AuthConfiguration.ApiUriPrefix] = newFilter;
}


void SwitchableAuthFilter::AddFilter(Platform::String^ stringToMatch, Windows::Web::Http::Filters::IHttpFilter^ newFilter)
{
	m_FilterMap[stringToMatch] = newFilter;
}


IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ SwitchableAuthFilter::SendRequestAsync(
	HttpRequestMessage^ request)
{
	if (m_InnerFilter == nullptr)
	{
		throw ref new Exception(E_INVALIDARG, "innerFilter cannot be set to null.");
	}
	// Walk the list and pick the right filter
	auto requestUri = std::wstring(request->RequestUri->AbsoluteCanonicalUri->Data());
	for (auto i = m_FilterMap.begin(); i != m_FilterMap.end(); i++)
	{
		//auto matchString = std::wstring((*i)->AuthConfiguration.ApiUriPrefix->Data());
		auto matchString = std::wstring(i->first->Data());
		if (requestUri.compare(0, matchString.size(), matchString) == 0)
		{
			IHttpFilter^ filter = i->second;
			ISetInnerFilter^ setInner = dynamic_cast<ISetInnerFilter^>(filter);
			if (setInner != nullptr)
			{
				setInner->SetInnerFilter(m_InnerFilter);
			}
			//(*i)->SetInnerFilter (m_InnerFilter);
			return filter->SendRequestAsync(request);
		}
	}

	return m_InnerFilter->SendRequestAsync(request);
}
