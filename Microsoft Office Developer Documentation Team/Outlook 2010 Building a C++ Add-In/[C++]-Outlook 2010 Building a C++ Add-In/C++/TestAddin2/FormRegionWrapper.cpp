#include "stdafx.h"
#include "FormRegionWrapper.h"

#define ReturnOnFailureHr(h) { hr = (h); ATLASSERT(SUCCEEDED((hr))); if (FAILED(hr)) return hr; }

// Macro that calls a COM method returning HRESULT value.
#define CHK_HR(stmt)        do { hr=(stmt); if (FAILED(hr)) ; } while(0)

// Macro to verify memory allcation.
#define CHK_ALLOC(p)        do { if (!(p)) { hr = E_OUTOFMEMORY; ; } } while(0)

// Macro that releases a COM object if not NULL.
#define SAFE_RELEASE(p)     do { if ((p)) { (p)->Release(); (p) = NULL; } } while(0)
class AutoVariant : 
    public VARIANT
{

public:

    AutoVariant()
    {
        VariantInit(this);
    }

    ~AutoVariant()
    {
        VariantClear(this);
    }

    HRESULT
    SetBSTRValue(
        LPCWSTR sourceString
        )
    {
        VariantClear(this);
        V_VT(this) = VT_BSTR;
        V_BSTR(this) = SysAllocString(sourceString);
        if (!V_BSTR(this))
        {
            return E_OUTOFMEMORY;
        }

        return S_OK;
    }

    void
    SetObjectValue(
        IUnknown *sourceObject
        )
    {
        VariantClear(this);

        V_VT(this) = VT_UNKNOWN;
        V_UNKNOWN(this) = sourceObject;

        if (V_UNKNOWN(this))
        {
            V_UNKNOWN(this)->AddRef();
        }
    }

};

class BingHttpRequest
{

public:
	BingHttpRequest()
	{
	
	}
	
void Complete()
{
	  HRESULT hr = CoInitializeEx(NULL,COINIT_APARTMENTTHREADED);
	  m_buffer.Append(m_tempBuffer);
	  loadDOM(m_buffer);
	 CoUninitialize();
}
void DoRequestSync(LPWSTR request,IWebBrowser* pWebBrowser)
{
	m_WebBrowser = pWebBrowser;
	DWORD err =0;
	DWORD dwSize = 0;
    DWORD dwDownloaded = 0;
    LPSTR pszOutBuffer;

	HINTERNET hSession = ::WinHttpOpen(0, WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
                                  WINHTTP_NO_PROXY_NAME,
                                  WINHTTP_NO_PROXY_BYPASS,
                                  0);
	HINTERNET hConnection  = ::WinHttpConnect(hSession,L"api.bing.net",INTERNET_DEFAULT_PORT,0);
	CString lpstrRequest =  L"xml.aspx?Sources=web&AppID=70FA6D77B407BA830359D291DD4531800EA4DA38";
	lpstrRequest += L"&query=";
	lpstrRequest += request;
	HINTERNET hRequest = ::WinHttpOpenRequest(hConnection,L"GET",(lpstrRequest.GetString()),
										0, // use HTTP version 1.1
										WINHTTP_NO_REFERER,
                                         WINHTTP_DEFAULT_ACCEPT_TYPES,
                                         0); // flags

	if (!::WinHttpSendRequest(hRequest,
                          WINHTTP_NO_ADDITIONAL_HEADERS,
                          0, // headers length
                          WINHTTP_NO_REQUEST_DATA,
                          0, // request data length
                          0, // total length
                          (DWORD_PTR)this)) // context
	{
	   
		 err = GetLastError();
	}
	else
	{ 
		bool result = WinHttpReceiveResponse( hRequest, NULL);
		 do 
        {

            // Check for available data.
            dwSize = 0;
            if (!WinHttpQueryDataAvailable( hRequest, &dwSize))
                printf( "Error %u in WinHttpQueryDataAvailable.\n",
                        GetLastError());

            // Allocate space for the buffer.
            pszOutBuffer = new char[dwSize+1];
            if (!pszOutBuffer)
            {
                printf("Out of memory\n");
                dwSize=0;
            }
            else
            {
                // Read the Data.
                ZeroMemory(pszOutBuffer, dwSize+1);

                if (!WinHttpReadData( hRequest, (LPVOID)pszOutBuffer, 
                                      dwSize, &dwDownloaded))
                    printf( "Error %u in WinHttpReadData.\n", GetLastError());
                else
				{
					m_buffer.Append(pszOutBuffer);
                    printf("%s", pszOutBuffer);
				}
            
                // Free the memory allocated to the buffer.
                delete [] pszOutBuffer;
            }

        } while (dwSize > 0);
		OutputDebugStringW(m_buffer);
 		loadDOM(m_buffer);

	
	}
	WinHttpCloseHandle(hSession);
}
CComPtr<IWebBrowser> m_WebBrowser;
	
	
HRESULT VariantFromString(PCWSTR wszValue, VARIANT &Variant)
{
    HRESULT hr = S_OK;
    BSTR bstr = SysAllocString(wszValue);
    CHK_ALLOC(bstr);    
    V_VT(&Variant)   = VT_BSTR;
    V_BSTR(&Variant) = bstr;


    return hr;
}

// Helper function to create a DOM instance. 
HRESULT CreateAndInitDOM(IXMLDOMDocument2 **ppDoc)
{
	HRESULT hr = CoCreateInstance(CLSID_DOMDocument, NULL, CLSCTX_INPROC_SERVER,  IID_IXMLDOMDocument2,(void**)ppDoc);
   
    return hr;
}

void loadDOM(BSTR xml)
{
	OutputDebugStringW(xml);
    HRESULT hr = S_OK;
    IXMLDOMDocument2 *pXMLDom=NULL;
    IXMLDOMParseError *pXMLErr = NULL;
    BSTR bstrXML = NULL;
    BSTR bstrErr = NULL;
    VARIANT_BOOL varStatus;
	WCHAR ns[]= L"'xmlns:web='http://schemas.microsoft.com/LiveSearch/2008/04/XML/web' " 
				L"'xmlns:search='http://schemas.microsoft.com/LiveSearch/2008/04/XML/element' ";
	AutoVariant v;
	v.SetBSTRValue(L"xmlns:search=\"http://schemas.microsoft.com/LiveSearch/2008/04/XML/element\"");
    CHK_HR(CreateAndInitDOM(&pXMLDom));    
    CHK_HR(pXMLDom->loadXML(xml, &varStatus));
    if (varStatus == VARIANT_TRUE)
    {
        CHK_HR(pXMLDom->get_xml(&bstrXML));
    }
    else
    {
        CHK_HR(pXMLDom->get_parseError(&pXMLErr));			
        CHK_HR(pXMLErr->get_reason(&bstrErr));
        printf("Failed to load DOM from BING. %S\n", bstrErr);
    }
	CComPtr<IXMLDOMNodeList> resultNodes;
    pXMLDom->setProperty (L"SelectionNamespaces",  CComVariant(L"xmlns:search=\"http://schemas.microsoft.com/LiveSearch/2008/04/XML/element\" xmlns:web=\"http://schemas.microsoft.com/LiveSearch/2008/04/XML/web\""));
    pXMLDom->setProperty(L"SelectionLanguage", CComVariant("XPath"));
	CComPtr<IXMLDOMElement> doc;
  	CHK_HR(pXMLDom->get_documentElement(&doc));
   	hr =  doc->selectNodes(CComBSTR(L"//search:SearchResponse/web:Web/web:Results/web:WebResult"),&resultNodes);
   	long length;
	if(resultNodes!=NULL)
	{
		resultNodes->get_length(&length);
		printf("Results node count %d",length);
		 if(m_WebBrowser)
		 {
			  CComBSTR html("<html><body><ul>");
			  CComPtr<IXMLDOMNode> pNode;
			  CComPtr<IXMLDOMNode> pTitleNode;
			  CComPtr<IXMLDOMNode> pUrlNode;
			  CComBSTR title;
			  CComBSTR url;
				for(int i=0;i<length;i++)
				{
					resultNodes->get_item(i,&pNode);
					pNode->get_firstChild(&pTitleNode);
					pNode->selectSingleNode(CComBSTR(L"web:Url"),&pUrlNode);
					if(pTitleNode!=NULL&&pUrlNode!=NULL)
					{				
						html.Append("<li><a target=\"_blank\" href=\"");
						pUrlNode->get_text(&url);
						html.AppendBSTR(url);
						html.Append("\">");
						pTitleNode->get_text(&title);
						html.AppendBSTR(title);
						html.Append("</a></li>");
					
					}
					pNode.Release();
					pUrlNode.Release();
					pTitleNode.Release();
				}
		CComPtr<IDispatch> docDispatch;
		m_WebBrowser->get_Document(&docDispatch);
		if(docDispatch==NULL)
		{			
			VARIANT vDummy;
			vDummy.vt=VT_EMPTY;
			m_WebBrowser->Navigate(L"about:blank",&vDummy,&vDummy,&vDummy,&vDummy);
			m_WebBrowser->get_Document(&docDispatch);
			if(docDispatch!=NULL)
			{
			CComPtr<IHTMLDocument2> doc;
			hr = docDispatch.QueryInterface<IHTMLDocument2>(&doc);
			if(hr==S_OK)
			{
			// Creates a new one-dimensional array
    			SAFEARRAY *psaStrings = SafeArrayCreateVector(VT_VARIANT, 0, 1);
    			VARIANT *param;
    			HRESULT hr = SafeArrayAccessData(psaStrings, (LPVOID*)&param);
    			param->vt = VT_BSTR;
    			param->bstrVal = html.Detach();
    			hr = SafeArrayUnaccessData(psaStrings);
				doc->write(psaStrings);			
				SafeArrayDestroy(psaStrings);
			}
			else
			{
				OutputDebugString(L"QI for IHtmlDocument2 failed");
			}
			}
			else
				OutputDebugString(L"DOC IS STILL NULL!!!!");
		}
	}
	else
	{
		printf("No nodes found");
	}
	}

}

LPSTR m_tempBuffer;
CComBSTR m_buffer;
					   


};
/*!-----------------------------------------------------------------------
	FormRegionWrapper implementation
-----------------------------------------------------------------------!*/

_ATL_FUNC_INFO FormRegionWrapper::VoidFuncInfo = {CC_STDCALL, VT_EMPTY, 0, 0}; 


HRESULT FormRegionWrapper::HrInit(_FormRegion* pFormRegion)
{
	HRESULT hr = S_OK;
	m_spFormRegion = pFormRegion;	
	FormRegionEventSink::DispEventAdvise(m_spFormRegion);
	CComPtr<IDispatch> spDispatch;
	ReturnOnFailureHr(pFormRegion->get_Form(&spDispatch));
	CComPtr<Forms::_UserForm> spForm;
	ReturnOnFailureHr(spDispatch->QueryInterface(&spForm));
	CComPtr<Forms::Controls> spControls;
	ReturnOnFailureHr(spForm->get_Controls(&spControls));
	CComPtr<Forms::IControl> spControl;

	CComBSTR bstrWBName(L"_webBrowser");
	spControls->_GetItemByName(bstrWBName.Detach(),&spControl);
	ReturnOnFailureHr(spControl->QueryInterface<IWebBrowser>(&m_spWebBrowser));	
	spControl.Release();
	
	CComPtr<IDispatch> spDispItem;
	ReturnOnFailureHr(pFormRegion->get_Item(&spDispItem));
	ReturnOnFailureHr(spDispItem->QueryInterface(&m_spMailItem));
	return hr;
}

void FormRegionWrapper::Show()
{
	if(m_spFormRegion)
	{
		m_spFormRegion->Select();
		
	}
}
void FormRegionWrapper::SearchSelection()
{
if (m_spMailItem)
	{
		CComPtr<_Inspector> pInspector;
		m_spMailItem->get_GetInspector(&pInspector);
		CComPtr<IDispatch> pWordDispatch;
		pInspector->get_WordEditor(&pWordDispatch);
		CComQIPtr<Word::_Document> pWordDoc(pWordDispatch);
		CComPtr<Word::_Application> pWordApp;
		pWordDoc->get_Application(&pWordApp);
		CComPtr<Word::Selection> pSelection;
		pWordApp->get_Selection(&pSelection);
		if(pSelection)
		{
			CComBSTR text;
			pSelection->get_Text(&text);	
			Search(text);
		}
}
}
void FormRegionWrapper::Search(BSTR term)
{
			BingHttpRequest* r = new BingHttpRequest();	
			r->DoRequestSync(term,m_spWebBrowser);
}


void FormRegionWrapper::OnFormRegionClose()
{
	
	if (m_spMailItem)
	{
		m_spMailItem.Release();
	}
	if (m_spFormRegion)
	{
		FormRegionEventSink::DispEventUnadvise(m_spFormRegion);
		m_spFormRegion.Release();
	}
	
	
}
