//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// PrintJobManagement.xaml.cpp
// Implementation of the classes related to print job management scenario.
//

#include "pch.h"

#include <OleAuto.h>
#include <OCIdl.h>

#include "PrinterExtension.h"
#include "PrinterExtensionDispId.h"

#include "PrinterEnumeration.h"
#include "Helpers.h"

#include "PrintJobManagement.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DeviceAppForPrinters2;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation::Collections;

using namespace Microsoft::WRL;

using namespace Platform;
using namespace Platform::Collections;

using namespace Concurrency;

PrintJobManagement::PrintJobManagement()
{
    InitializeComponent();

    disp = Windows::UI::Core::CoreWindow::GetForCurrentThread()->Dispatcher;
}

void
PrintJobManagement::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser(DisplayStrings::ENUMERATE_PRINTERS_TO_CONTINUE, NotifyType::StatusMessage);
}

void
PrintJobManagement::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // When the user navigates away from this page, it is important to remove the current print queue view
    // so that the print job enumeration is stopped.
    try
    {
        ResetPrintQueueView();
    }
    catch(...) {/* Do not throw exceptions in scenario navigation method. */ }
}

PrintJobManagement::~PrintJobManagement()
{
    try
    {
        ResetPrintQueueView();
    }
    catch(...) { /* Do nothing. Destructors cannot throw exceptions. */}
}

void
PrintJobManagement::UpdatePrinterList(IVector<PrinterInfo^>^ printerList)
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
PrintJobManagement::UpdatePrintJobList(IVector<PrintJob^>^ jobVector)
{
    disp->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::Normal,
        ref new Windows::UI::Core::DispatchedHandler([this, jobVector] () {
            PrintJobListBox->ItemsSource = jobVector;
    }, Platform::CallbackContext::Any));
}

void
PrintJobManagement::EnumeratePrinters_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser(DisplayStrings::PRINTERS_ENUMERATING, NotifyType::StatusMessage);

    String^ packageFamilyName = Windows::ApplicationModel::Package::Current->Id->FamilyName;

    // Asynchronously enumerate printers
    create_task(PrinterEnumeration::EnumeratePrintersAsync(packageFamilyName))
    .then([this](IVector<PrinterInfo^>^ printerList)
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
PrintJobManagement::CancelPrintJob_Click(Object^ sender, RoutedEventArgs^ e)
{
    try
    {
        PrintJob^ job = reinterpret_cast<PrintJob^>(PrintJobListBox->SelectedItem);
        if (job != nullptr)
        {
            job->RequestCancel();
        }
    }
    catch(Exception^ e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);
    }
    catch(std::exception e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);;
    }
}


void
PrintJobManagement::Printer_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    try
    {
        PrinterInfo^ selectedPrinter = reinterpret_cast<PrinterInfo^>(PrinterComboBox->SelectedItem);
        if (selectedPrinter != nullptr)
        {
            // The current print queue view must be reset before it can be set with another printer's view.
            ResetPrintQueueView();

            ComPtr<IPrinterQueue2> printerQueue;
            CreatePrinterQueueObject(selectedPrinter->DeviceId, printerQueue);

            // Display the print queue view for the newly selected printer
            SetPrintQueueView(printerQueue);
        }
    }
    catch(Exception^ e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);;
    }
    catch(std::exception e)
    {
        MainPage::Current->NotifyUser(ExceptionHelper::ToString(e), NotifyType::ErrorMessage);;
    }
}

void
PrintJobManagement::PrintJob_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    try
    {
        PrintJob^ job = reinterpret_cast<PrintJob^>(PrintJobListBox->SelectedItem);
        if (job != nullptr)
        {
            String^ content =
                "Details of print job: " + job->Name + "\r\n" +
                "Pages printed: " + job->PrintedPages + "/" + job->TotalPages + "\r\n" +
                "Submission time: " + job->SubmissionTime + "\r\n" +
                "Job status: " + job->Status;

            PrintJobDetails->Text = content;
        }
        else
        {
            PrintJobDetails->Text = "Please select a print job";
        }
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

void
PrintJobManagement::SetPrintQueueView(ComPtr<IPrinterQueue2>& printerQueue)
{
    if (printJobEventHandler != nullptr)
    {
        throw ref new InvalidArgumentException("Please ensure ResetPrintQueueView() is invoked before calling SetPrintQueueView()");
    }

    // Retrieve the printer queue view that will enumerate jobs in 0-10 positions.
    const int FIRST_PRINT_JOB_ENUMERATED = 0;
    const int LAST_PRINT_JOB_ENUMERATED = 10;
    ComPtr<IPrinterQueueView> printerQueueView;
    ThrowIfFailed(
        printerQueue->GetPrinterQueueView(FIRST_PRINT_JOB_ENUMERATED, LAST_PRINT_JOB_ENUMERATED, &printerQueueView));

    // Create an instance of the print job event handler and start print job enumeration.
    printJobEventHandler = Make<PrintJobEventHandler>(this, printerQueueView);
    if (printJobEventHandler == nullptr)
    {
        throw ref new OutOfMemoryException();
    }

    printJobEventHandler->StartPrintJobEnumeration();
}

void
PrintJobManagement::ResetPrintQueueView()
{
    // Stop the print job enumeration and release the reference to the printJobEventHandler object
    // because it is mandatory to break the circular reference between the PrintJobManagement and
    // printJobEventHandler classes.
    if (printJobEventHandler != nullptr)
    {
        printJobEventHandler->StopPrintJobEnumeration();
        printJobEventHandler = nullptr;
    }
}

/* static */ void
PrintJobManagement::CreatePrinterQueueObject(Platform::String^ deviceId, ComPtr<IPrinterQueue2>& printerQueue2)
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

PrintJobEventHandler::PrintJobEventHandler(PrintJobManagement^ scenarioPage, ComPtr<IPrinterQueueView>& printerQueueView)
{
    this->scenarioPage = scenarioPage;

    ComPtr<IConnectionPointContainer> connectionPointContainer;
    ThrowIfFailed(
        printerQueueView.As(&connectionPointContainer));

    ThrowIfFailed(
        connectionPointContainer->FindConnectionPoint(__uuidof(IPrinterQueueViewEvent), &connectionPoint));
}

HRESULT STDMETHODCALLTYPE
PrintJobEventHandler::OnChanged(
    _In_ IPrintJobCollection* collection,
    ULONG viewOffset,
    ULONG viewSize,
    ULONG countJobsInPrintQueue)
{
    HRESULT hr = S_OK;

    // This method must not throw any exceptions because it represents a COM object boundary.
    try
    {
        Windows::Foundation::Collections::IVector<PrintJob^>^ jobList = GetPrintJobVector(collection);
        scenarioPage->UpdatePrintJobList(jobList);
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

HRESULT STDMETHODCALLTYPE
PrintJobEventHandler::Invoke(
    DISPID id,
    _In_  REFIID,
    _In_  LCID,
    _In_  WORD ,
    _In_  DISPPARAMS* params,
    _Out_opt_  VARIANT* var,
    _Out_opt_  EXCEPINFO* excepInfo,
    _Out_opt_  UINT* wrongArgument)
{
    HRESULT hr = S_OK;

    try
    {
        // Perform basic sanity checks
        if ((params == nullptr) ||
            (params->cArgs != 4))
        {
            throw Platform::Exception::CreateException(DISP_E_BADPARAMCOUNT);
        }

        if (id != DISPID_PRINTERQUEUEVIEW_EVENT_ONCHANGED)
        {
            throw Platform::Exception::CreateException(DISP_E_MEMBERNOTFOUND);
        }

        // Verify the contents of the VARIANT structure are in order.
        VARIANT countJobs   = params->rgvarg[0];
        VARIANT viewSize    = params->rgvarg[1];
        VARIANT viewOffset  = params->rgvarg[2];
        VARIANT collection  = params->rgvarg[3];

        if (countJobs.vt != VT_UI4)
        {
            *wrongArgument = 0;
            throw Platform::Exception::CreateException(DISP_E_TYPEMISMATCH);
        }
        else if (viewSize.vt != VT_UI4)
        {
            *wrongArgument = 1;
            throw Platform::Exception::CreateException(DISP_E_TYPEMISMATCH);
        }
        else if (viewOffset.vt != VT_UI4)
        {
            *wrongArgument = 2;
            throw Platform::Exception::CreateException(DISP_E_TYPEMISMATCH);
        }
        else if (collection.vt != VT_DISPATCH)
        {
            *wrongArgument = 3;
            throw Platform::Exception::CreateException(DISP_E_TYPEMISMATCH);
        }

        IPrintJobCollection* pc = reinterpret_cast<IPrintJobCollection*>(collection.pdispVal);
        if (pc == nullptr)
        {
            *wrongArgument = 3;
            throw Platform::Exception::CreateException(DISP_E_TYPEMISMATCH);
        }

        // Invoke the event handler method to handle the queue enumeration.
        ThrowIfFailed(
            OnChanged(pc, viewOffset.ulVal, viewSize.ulVal, countJobs.ulVal));
    }
    catch(Exception^ exception)
    {
        hr = exception->HResult;
    }
    
    // No value to be returned from this function invocation. Initialize the output VARIANT to empty.
    if (var != nullptr)
    {
        ::VariantInit(var);
    }

    // No exception information is returned from this function invocation. Hence, zero initialize it.
    if (excepInfo != nullptr)
    {
        ZeroMemory(excepInfo, sizeof(EXCEPINFO));
    }

    return hr;
}

IVector<PrintJob^>^
PrintJobEventHandler::GetPrintJobVector(_In_ IPrintJobCollection* collection)
{
    IVector<PrintJob^>^ jobVector = ref new Vector<PrintJob^>();

    ULONG countJobs;
    ThrowIfFailed(
        collection->get_Count(&countJobs));

    for (ULONG i = 0; i < countJobs; i++)
    {
        ComPtr<IPrintJob> spJob;
        ThrowIfFailed(
            collection->GetAt(i, &spJob));

        PrintJob^ job = ref new PrintJob(reinterpret_cast<Object^>(spJob.Get()));
        if (job == nullptr)
        {
            throw ref new OutOfMemoryException();
        }

        jobVector->Append(job);
    }

    return jobVector;
}

void
PrintJobEventHandler::StartPrintJobEnumeration()
{
    // Subscribe this instance of the class as an event handler via a call to IConnectionPoint::Advise().
    // This operation starts the print job enumeration.
    ThrowIfFailed(
        connectionPoint->Advise(reinterpret_cast<IUnknown*>(this), &connectionPointCookie));
}

void
PrintJobEventHandler::StopPrintJobEnumeration()
{
    // Unsubscribe this instance of the class via a call to IConnectionPoint::Unadvise().
    // This operation stop the print job enumeration.
    if (connectionPoint != nullptr)
    {
        ThrowIfFailed(
            connectionPoint->Unadvise(connectionPointCookie));

        connectionPointCookie = 0;
        connectionPoint = nullptr;
    }

    // Release the reference to the scenario page. This is mandatory because the
    // circular reference between the scenario page and this class needs to be broken.
    scenarioPage = nullptr;
}

PrintJob::PrintJob(Object^ iPrintJobComObject)
{
    printJob = reinterpret_cast<IPrintJob*>(iPrintJobComObject);
    if (printJob == nullptr)
    {
        throw ref new Platform::InvalidArgumentException();
    }

    // Populate the properties of the job
    AutoBSTR jobName;
    ThrowIfFailed(
        printJob->get_Name(jobName.Get()));
    name = ref new Platform::String(jobName);

    ThrowIfFailed(
        printJob->get_Id(&id));

    ThrowIfFailed(
        printJob->get_PrintedPages(&printedPages));

    ThrowIfFailed(
        printJob->get_TotalPages(&totalPages));

    PrintJobStatus status;
    ThrowIfFailed(
        printJob->get_Status(&status));
    ConvertStatusToDisplayString(status);

    DATE submissionTime;
    ThrowIfFailed(
        printJob->get_SubmissionTime(&submissionTime));
    ConvertVariantTimeToDisplayString(submissionTime);
}

void
PrintJob::ConvertStatusToDisplayString(PrintJobStatus printJobStatus)
{
    // Since PrintJobStatus is a set of flags, the final status is the
    // concatenated result.
    std::wstring statusString;
    if (printJobStatus & PrintJobStatus_Paused)
    {
        statusString.append(L"Paused,");
    }
    if (printJobStatus & PrintJobStatus_Error)
    {
        statusString.append(L"Error,");
    }
    if (printJobStatus & PrintJobStatus_Deleting)
    {
        statusString.append(L"Deleting,");
    }
    if (printJobStatus & PrintJobStatus_Spooling)
    {
        statusString.append(L"Spooling,");
    }
    if (printJobStatus & PrintJobStatus_Printing)
    {
        statusString.append(L"Printing,");
    }
    if (printJobStatus & PrintJobStatus_Offline)
    {
        statusString.append(L"Offline,");
    }
    if (printJobStatus & PrintJobStatus_PaperOut)
    {
        statusString.append(L"Out of paper,");
    }
    if (printJobStatus & PrintJobStatus_Printed)
    {
        statusString.append(L"Printed,");
    }
    if (printJobStatus & PrintJobStatus_Deleted)
    {
        statusString.append(L"Deleted,");
    }
    if (printJobStatus & PrintJobStatus_BlockedDeviceQueue)
    {
        statusString.append(L"Device queue blocked,");
    }
    if (printJobStatus & PrintJobStatus_UserIntervention)
    {
        statusString.append(L"User intervention required,");
    }
    if (printJobStatus & PrintJobStatus_Restarted)
    {
        statusString.append(L"Restarted,");
    }
    if (printJobStatus & PrintJobStatus_Complete)
    {
        statusString.append(L"Complete,");
    }
    if (printJobStatus & PrintJobStatus_Retained)
    {
        statusString.append(L"Retained,");
    }

    size_t length = statusString.length();
    if (length > 0)
    {
        // Trim the trailing comma
        statusString.resize(length - 1);
    }
    else
    {
        statusString.append(L"Not available");
    }

    status = ref new Platform::String(statusString.c_str());
}

void
PrintJob::ConvertVariantTimeToDisplayString(DATE submissionTime)
{
    SYSTEMTIME st;
    if (FALSE == ::VariantTimeToSystemTime(submissionTime, &st))
    {
        throw ref new InvalidArgumentException();
    }

    std::wstringstream ws;
    ws << st.wMonth << L"/" << st.wDay << L"/" << st.wYear << " ";
    ws << st.wHour << L":" << st.wMinute << L":" << st.wSecond;

    this->submissionTime = ref new String(ws.str().c_str());
}