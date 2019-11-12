#include "pch.h"
#include "PrintHelper.h"
#include <OleAuto.h>
#include <OCIdl.h>
#include <AsyncInfo.h>
#include <PrinterExtensionDispId.h>

using namespace DeviceAppForPrinters;
using namespace Microsoft::WRL;

PrintHelper::PrintHelper()
{
}

PrintHelper::~PrintHelper(void)
{
}

bool PrintHelper::Initialize(Platform::Object^ contextObject)
{
    // Convert the object to IPrinterExtensionContext
    ComPtr<IUnknown> spUnknown = reinterpret_cast<IUnknown*>(contextObject);
    ComPtr<IPrinterExtensionContext> spContext;
    HRESULT hr = spUnknown.As(&spContext);

    // Cache printer extension context
    if (SUCCEEDED(hr))
    {
        _spContext.Attach(spContext.Detach());
    }

    // Get printer queue
    ComPtr<IPrinterQueue> spQueue;
    if (SUCCEEDED(hr))
    {
        hr = _spContext->get_PrinterQueue(&spQueue);

        // Cache printer queue
        if (SUCCEEDED(hr))
        {
            _spQueue.Attach(spQueue.Detach());
        }
    }

    // Retrieve the print ticket
    ComPtr<IPrintSchemaTicket> spTicket;
    if (SUCCEEDED(hr))
    {
        hr = _spContext->get_PrintSchemaTicket(&spTicket);

        // Get the capabilities
        ComPtr<IPrintSchemaCapabilities> spCapabilities;
        if (SUCCEEDED(hr))
        {
            hr = spTicket->GetCapabilities(&spCapabilities);

            // Cache print ticket and capabilities
            if (SUCCEEDED(hr))
            {
                _spTicket.Attach(spTicket.Detach());
                _spCapabilities.Attach(spCapabilities.Detach());
            }
        }

        // It is okay to fail for tile scenario
        hr = S_OK;
    }

    return SUCCEEDED(hr);
}

bool PrintHelper::FeatureExists(Platform::String^ feature)
{
    if ((nullptr == feature) || (nullptr == _spCapabilities))
        return false;

    // Return true only if feature is within capabilities and ticket
    ComPtr<IPrintSchemaFeature> spFeature;
    ComPtr<IPrintSchemaFeature> spTicketFeature;
    BSTR bstrFeature = SysAllocString(feature->Data());
    HRESULT hr = _spCapabilities->GetFeatureByKeyName(bstrFeature, &spFeature);
    if (SUCCEEDED(hr))
    {
        hr = _spTicket->GetFeatureByKeyName(bstrFeature, &spTicketFeature);
        if (SUCCEEDED(hr))
        {
            hr = (nullptr != spFeature && nullptr != spTicketFeature) ? S_OK : E_FAIL;
        }
    }
    SysFreeString(bstrFeature);

    return SUCCEEDED(hr);
}

Platform::String^ PrintHelper::GetFeatureDisplayName(Platform::String^ feature)
{
    if ((nullptr == feature) || (nullptr == _spCapabilities))
        return nullptr;

    ComPtr<IPrintSchemaFeature> spFeature;
    BSTR bstrFeature = SysAllocString(feature->Data());
    HRESULT hr = _spCapabilities->GetFeatureByKeyName(bstrFeature, &spFeature);
    Platform::String^ displayName;
    if (SUCCEEDED(hr))
    {
        BSTR bstrDisplayName;
        hr = spFeature->get_DisplayName(&bstrDisplayName);
        if (SUCCEEDED(hr))
        {
            displayName = ref new Platform::String(bstrDisplayName);
            SysFreeString(bstrDisplayName);
        }
    }
    SysFreeString(bstrFeature);

    return displayName;
}

bool PrintHelper::GetOptionInfo(Platform::String^ feature, Windows::UI::Xaml::Controls::ComboBox^ collection)
{
    if ((nullptr == feature) || (nullptr == _spCapabilities))
        return false;

    ComPtr<IPrintSchemaFeature> spFeature;
    BSTR bstrFeature = SysAllocString(feature->Data());
    HRESULT hr = _spCapabilities->GetFeatureByKeyName(bstrFeature, &spFeature);
    if (SUCCEEDED(hr))
    {
        ComPtr<IPrintSchemaOptionCollection> spCollection;
        hr = _spCapabilities->GetOptions(spFeature.Get(), &spCollection);
        if (SUCCEEDED(hr))
        {
            ULONG count = 0;
            hr = spCollection->get_Count(&count);
            if (SUCCEEDED(hr) && (0 < count))
            {
                ComPtr<IPrintSchemaOption> spOption;		
                for (ULONG i=0; i<count; i++)
                {
                    hr = spCollection->GetAt(i, spOption.ReleaseAndGetAddressOf());
                    if (SUCCEEDED(hr))
                    {
                        BSTR bstrDisplayName;
                        hr = spOption->get_DisplayName(&bstrDisplayName);
                        if (SUCCEEDED(hr))
                        {
                            Windows::UI::Xaml::Controls::ComboBoxItem^ item = ref new Windows::UI::Xaml::Controls::ComboBoxItem;
                            item->Content = ref new Platform::String(bstrDisplayName);
                            collection->Items->Append(item);
                            BOOL bIsSelected = FALSE;
                            hr = spOption->get_Selected(&bIsSelected);
                            if (SUCCEEDED(hr) && bIsSelected)
                            {
                                collection->SelectedIndex = i;
                            }
                            SysFreeString(bstrDisplayName);
                        }
                    }
                }
                // It is possible for the PrintTicket object to not contain a current selection for this feature causing none 
                // of the options in the print capabilities to be marked as selected.  In this case, the developers should 
                // be familiar enough with the printer hardware to display and set the feature to the correct printer default option.
                // Because this is a generic sample app, the first option will be displayed and set when the user hits the back button.
                if (-1 == collection->SelectedIndex)
                {
                    collection->SelectedIndex = 0;
                }
            }
        }
    }
    SysFreeString(bstrFeature);

    return SUCCEEDED(hr);
}

bool PrintHelper::IsOptionConstrained(Platform::String^ feature, int optionIndex)
{
    HRESULT hr = S_OK;
    bool bConstrained = false;
    if ((nullptr == feature) || (0 > optionIndex))
    {
        return bConstrained;
    }

    ComPtr<IPrintSchemaFeature> spFeature;
    BSTR bstrFeature = SysAllocString(feature->Data());
    try
    {
        hr = _spTicket->GetFeatureByKeyName(bstrFeature, &spFeature);
        if (SUCCEEDED(hr))
        {
            ComPtr<IPrintSchemaOptionCollection> spCollection;
            hr = _spCapabilities->GetOptions(spFeature.Get(), &spCollection);
            if (SUCCEEDED(hr))
            {
                ULONG count = 0;
                hr = spCollection->get_Count(&count);
                if (SUCCEEDED(hr) && (0 < count))
                {
                    ComPtr<IPrintSchemaOption> spOption;			
                    hr = spCollection->GetAt(optionIndex, spOption.ReleaseAndGetAddressOf());
                    if (SUCCEEDED(hr))
                    {
                        PrintSchemaConstrainedSetting constraint;
                        hr = spOption->get_Constrained(&constraint);
                        if (SUCCEEDED(hr))
                        {
                            if (PrintSchemaConstrainedSetting_None != constraint)
                            {
                                bConstrained = true;
                            }
                        }
                    }
                }
            }
        }
    }
    catch (Platform::Exception^ exception)
    {
        return bConstrained;
    }

    SysFreeString(bstrFeature);
    return bConstrained;
}

bool PrintHelper::SetFeatureOption(Platform::String^ feature, int optionIndex)
{
    HRESULT hr = S_OK;
    if ((nullptr == feature) || (0 > optionIndex))
    {
        return false;
    }

    ComPtr<IPrintSchemaFeature> spFeature;
    BSTR bstrFeature = SysAllocString(feature->Data());
    try
    {
        hr = _spTicket->GetFeatureByKeyName(bstrFeature, &spFeature);
        if (SUCCEEDED(hr))
        {
            ComPtr<IPrintSchemaOptionCollection> spCollection;
            hr = _spCapabilities->GetOptions(spFeature.Get(), &spCollection);
            if (SUCCEEDED(hr))
            {
                ULONG count = 0;
                hr = spCollection->get_Count(&count);
                if (SUCCEEDED(hr) && (0 < count))
                {
                    ComPtr<IPrintSchemaOption> spOption;
                    hr = spCollection->GetAt(optionIndex, spOption.ReleaseAndGetAddressOf());
                    if (SUCCEEDED(hr))
                    {
                        // Set the new option only if the user has selected an option different from the original
                        // Note that for options with user defined parameters, such as CustomMediaSize, extra information is needed
                        // than what is present in the Print Capabilities.  For simplicity, we have chosen to use the default
                        // parameters in this sample app. But developers should use specialized UI to prompt the user for the required parameters.
                        BOOL bIsSelected = FALSE;
                        hr = spOption->get_Selected(&bIsSelected);
                        if (SUCCEEDED(hr) && !(bIsSelected))
                        {
                            hr = spFeature->put_SelectedOption(spOption.Get());
                        }
                    }
                }
            }
        }
    }
    catch (Platform::Exception^ exception)
    {
        return false;
    }

    SysFreeString(bstrFeature);
    return SUCCEEDED(hr);
}

Windows::Foundation::IAsyncOperation<Platform::String^>^ PrintHelper::GetInkLevelAsync(void)
{
    return Concurrency::create_async([this]() -> Platform::String^ {
        return GetInkLevel();
    });
}

Platform::String^ PrintHelper::GetInkLevel()
{
    Platform::String^ inkLevel = nullptr;
    if (nullptr == _spQueue)
    {
        return inkLevel;
    }

    HRESULT hr = S_OK;

    ComPtr<PrinterQueueEventHandler> spHandler = Microsoft::WRL::Make<PrinterQueueEventHandler>();
    hr = (nullptr != spHandler) ? S_OK : E_OUTOFMEMORY;

    // Get the connection point
    DWORD dwCookie  = 0;
    ComPtr<IConnectionPoint> spConnection;
    if (SUCCEEDED(hr))
    {
        ComPtr<IConnectionPointContainer> spContainer;
        hr = _spQueue.As(&spContainer);
        if (SUCCEEDED(hr))
        {
            spContainer->FindConnectionPoint(__uuidof(IPrinterQueueEvent), &spConnection);
            if (SUCCEEDED(hr))
            {
                ComPtr<IUnknown> spUnk;
                hr = spHandler.As(&spUnk);
                if (SUCCEEDED(hr))
                {
                    hr = spConnection->Advise(spUnk.Get(), &dwCookie);
                }
            }
        }
    }

    // Send the bidi query
    if (SUCCEEDED(hr))
    {
        BSTR queryString = SysAllocString(L"\\Printer.Consumables");
        hr = _spQueue->SendBidiQuery(queryString);
        SysFreeString(queryString);
    }

    // Wait for the response
    if (SUCCEEDED(hr))
    {
        inkLevel = spHandler->WaitForBidiResponse();
    }

    // Remove the connection point
    if (0 != dwCookie)
    {
        hr = spConnection->Unadvise(dwCookie);
    }
    return inkLevel;
}

PrinterQueueEventHandler::PrinterQueueEventHandler(void)
{
    _hEvent = CreateEventEx(nullptr, nullptr, 0, EVENT_ALL_ACCESS);
}

PrinterQueueEventHandler::~PrinterQueueEventHandler(void)
{
    CloseHandle(_hEvent);
}

HRESULT PrinterQueueEventHandler::OnBidiResponseReceived(BSTR bstrResponse, HRESULT hrStatus)
{
    _bidiResponse = ref new Platform::String(bstrResponse);
    SetEvent(_hEvent);
    return S_OK;
}

Platform::String^ PrinterQueueEventHandler::WaitForBidiResponse(void)
{
    DWORD dwResult = WaitForSingleObjectEx(_hEvent, INFINITE, TRUE);
    if (WAIT_OBJECT_0 == dwResult)
    {
        return _bidiResponse;
    }
    else
    {
        return nullptr;
    }
}

HRESULT PrinterQueueEventHandler::Invoke(_In_  DISPID id, _In_  REFIID, _In_  LCID, _In_  WORD, _In_  DISPPARAMS * params, _Out_opt_  VARIANT *pVarResult, _Out_opt_  EXCEPINFO *pExcepInfo, _Out_opt_  UINT *puArgErr)
{
    HRESULT hr = E_INVALIDARG;
    if (DISPID_PRINTERQUEUEEVENT_ONBIDIRESPONSERECEIVED == id)
    {
        if (nullptr != params)
        {
            if (2 <= params->cArgs)
            {
                // There should be two variants, the first one is hrStatus and the second one is the bidi response string
                VARIANT varStatus = params->rgvarg[0];
                VARIANT varResponse = params->rgvarg[1];
                hr = OnBidiResponseReceived(varResponse.bstrVal, varStatus.scode);
            }
        }
    }
    // No need to return anything so reset output pointers
    if (nullptr != pVarResult)
    {
        VariantInit(pVarResult);
    }
    if (nullptr != pExcepInfo)
    {
        ZeroMemory(pExcepInfo, sizeof(*pExcepInfo));
    }
    if (nullptr != puArgErr)
    {
        *puArgErr = 0;
    }
    return hr;
}
