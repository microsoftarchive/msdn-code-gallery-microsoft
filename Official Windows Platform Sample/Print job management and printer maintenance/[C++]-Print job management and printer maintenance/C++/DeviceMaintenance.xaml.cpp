//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// DeviceMaintenance.xaml.cpp
// Implementation of the DeviceMaintenance and BidiSetRequestCallback classes
//

#include "pch.h"

#include "PrinterExtension.h"

#include "PrinterEnumeration.h"
#include "Helpers.h"

#include "DeviceMaintenance.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DeviceAppForPrinters2;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation::Collections;

using namespace Platform;
using namespace Platform::Collections;

using namespace Microsoft::WRL;

using namespace Concurrency;

DeviceMaintenance::DeviceMaintenance()
{
    InitializeComponent();

    disp = Windows::UI::Core::CoreWindow::GetForCurrentThread()->Dispatcher;
}

void
DeviceMaintenance::OnNavigatedTo(NavigationEventArgs^ e)
{
    String^ defaultBidiQuery = 
        "<bidi:Set xmlns:bidi=\"http://schemas.microsoft.com/windows/2005/03/printing/bidi\">\r\n"
        "    <Query schema='\\Printer.Maintenance:CleanHead'>\r\n"
        "        <BIDI_BOOL>false</BIDI_BOOL>\r\n"
        "    </Query>\r\n"
        "</bidi:Set>";
    BidiQueryInput->Text = defaultBidiQuery;

    MainPage::Current->NotifyUser(DisplayStrings::ENUMERATE_PRINTERS_TO_CONTINUE, NotifyType::StatusMessage);
}

void
DeviceMaintenance::UpdateBidiResponse(Platform::String^ response)
{
    BidiResponseOutput->Text = response;

    MainPage::Current->NotifyUser(DisplayStrings::BIDI_RESPONSE_RECEIVED, NotifyType::StatusMessage);
}

void
DeviceMaintenance::UpdatePrinterList(IVector<PrinterInfo^>^ printerList)
{
    if (printerList->Size > 0)
    {
        PrinterComboBox->ItemsSource = printerList;
        PrinterComboBox->SelectedIndex = 0;

        MainPage::Current->NotifyUser(DisplayStrings::PRINTERS_ENUMERATED, NotifyType::StatusMessage);
    }
    else
    {
        MainPage::Current->NotifyUser(DisplayStrings::NO_PRINTER_ENUMERATED, NotifyType::ErrorMessage);
    }
}

void
DeviceMaintenance::EnumeratePrinters_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser(DisplayStrings::PRINTERS_ENUMERATING, NotifyType::StatusMessage);

    String^ packageFamilyName = Windows::ApplicationModel::Package::Current->Id->FamilyName;

    // Asynchronously enumerate printers
    create_task(PrinterEnumeration::EnumeratePrintersAsync(packageFamilyName))
    .then([this] (IVector<PrinterInfo^>^ printerList)
    {
        UpdatePrinterList(printerList);
    })
    .then([] (task<void> t) // Task-based continuation that will handle exceptions thrown from the async task.
    {
        try
        {
            t.get();
        }
        catch(Exception^ e)
        {
            MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);;
        }
    });
}

void
DeviceMaintenance::SendBidiRequest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        MainPage::Current->NotifyUser("Sending the Bidi \"Set\" Query", NotifyType::StatusMessage);

        // Retrieve the device id of the selected printer.
        if (PrinterComboBox->Items->Size == 0)
        {
            MainPage::Current->NotifyUser("No printers enumerated", NotifyType::ErrorMessage);
            return;
        }

        PrinterInfo^ printerInfo = reinterpret_cast<PrinterInfo^>(PrinterComboBox->SelectedItem);
        String^ deviceId = (String^) printerInfo->DeviceId;

        // Create an IPrinterQueue2 object, and an IPrinterBidiSetRequestCallback callback object.
        ComPtr<IPrinterQueue2> printerQueue2;
        CreatePrinterQueueObject(deviceId, printerQueue2);

        ComPtr<BidiSetRequestCallback> callback = Make<BidiSetRequestCallback>(this);
        if (callback == nullptr)
        {
            throw ref new OutOfMemoryException("Failed to create the bidi response callback object");
        }

        ComPtr<IPrinterBidiSetRequestCallback> spCallback;
        ThrowIfFailed
            (callback.As(&spCallback));

        // Send the bidi async operation
        ComPtr<IPrinterExtensionAsyncOperation> spAsyncOperation;
        AutoBSTR data(BidiQueryInput->Text);

        ThrowIfFailed(
            printerQueue2->SendBidiSetRequestAsync(
                data,
                spCallback.Get(),
                &spAsyncOperation));

        // The spAsyncOperation variable can be used to cancel the async operation if required.
    }
    catch(Exception^ e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);
    }
    catch(std::exception e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);
    }
}

/* static */ void
DeviceMaintenance::CreatePrinterQueueObject(Platform::String^ deviceId, ComPtr<IPrinterQueue2>& printerQueue2)
{
    // Use the static WinRT factory to create an instance of IPrinterExtensionContext, and retrieve
    // an IPrinterQueue2 object from it
    Object^ obj = Windows::Devices::Printers::Extensions::PrintExtensionContext::FromDeviceId(deviceId);
    ComPtr<IUnknown> spUnknown = reinterpret_cast<IUnknown*>(obj);

    ComPtr<IPrinterExtensionContext> context;
    ThrowIfFailed(
        spUnknown.As(&context));

    ComPtr<IPrinterQueue> printerQueue;
    ThrowIfFailed(
        context->get_PrinterQueue(&printerQueue));

    ThrowIfFailed(
        printerQueue.As(&printerQueue2));
}

// Note: This COM callback method should return the correct success/failure HRESULT value
// from the result of its operations.
HRESULT STDMETHODCALLTYPE
BidiSetRequestCallback::Completed(
    _In_ BSTR bstrResponse,
    HRESULT hrStatus)
{
    HRESULT hr = S_OK;

    // This method must not throw any exceptions because it represents a COM object boundary.
    try
    {
        String^ result;
        std::wstringstream ws;

        if (SUCCEEDED(hrStatus))
        {
            ws << L"The Bidi response is:" << std::endl << bstrResponse;
        }
        else
        {
            // If the Bidi request reports a failed hrStatus, the bstrResponse variable is not populated.
            ws << L"The HRESULT received is: 0x";
            ws << std::hex << hrStatus << std::endl;
            ws << L"No Bidi response was received";
        }

        result = ref new String(ws.str().c_str());

        scenarioPage->UpdateBidiResponse(result);
    }
    catch (Platform::Exception^ e)
    { 
        hr = e->HResult;
    }
    catch(...)
    {
        hr = E_FAIL;
    }

    return hr;
}