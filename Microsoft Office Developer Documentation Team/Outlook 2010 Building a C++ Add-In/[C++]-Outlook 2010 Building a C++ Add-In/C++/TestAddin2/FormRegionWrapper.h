#pragma once
#include "stdafx.h"
#include <vector>
#include <algorithm>

/*!-----------------------------------------------------------------------
	FormRegionWrapper- Used to help track all the form regions and to 
	listen to some basic events such as the send button click and close.
-----------------------------------------------------------------------!*/
class FormRegionWrapper;


typedef 
	IDispEventSimpleImpl<1, FormRegionWrapper, &__uuidof(FormRegionEvents)>
	FormRegionEventSink;

const DWORD dispidEventOnClose = 0xF004;

class FormRegionWrapper
	: public FormRegionEventSink
{
public:
	HRESULT HrInit(_FormRegion* pFormRegion);
	void Show();
	void SearchSelection();
	void Search(BSTR term);
	
private:
	
	static _ATL_FUNC_INFO VoidFuncInfo; 
public:
	BEGIN_SINK_MAP(FormRegionWrapper)
		SINK_ENTRY_INFO(1, __uuidof(FormRegionEvents), dispidEventOnClose, OnFormRegionClose, &VoidFuncInfo)
	END_SINK_MAP()

	void __stdcall OnFormRegionClose();

private:
	CComPtr<_FormRegion> m_spFormRegion;
	CComPtr<_MailItem> m_spMailItem;
	CComPtr<IWebBrowser> m_spWebBrowser;
};

