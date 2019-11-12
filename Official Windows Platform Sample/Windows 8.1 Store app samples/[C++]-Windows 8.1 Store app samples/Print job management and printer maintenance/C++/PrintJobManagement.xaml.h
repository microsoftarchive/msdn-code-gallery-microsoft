//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// PrintJobManagement.xaml.h
// Declaration of the classes related to print job management scenario.
//

#pragma once
#include "pch.h"
#include "PrintJobManagement.g.h"
#include <OCIdl.h>
#include "Helpers.h"

namespace SDKSample
{
    namespace DeviceAppForPrinters2
    {
        ref class PrintJob; // Forward declaration
        class PrintJobEventHandler; // Forward declaration

        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class PrintJobManagement sealed
        {
            // Constructors and public methods
        public:
            PrintJobManagement();
            virtual ~PrintJobManagement();

            /// <summary>
            /// This method is invoked when the user navigates to this scenario page.
            /// </summary>
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

            /// <summary>
            /// This method is invoked when the user navigates away from this scenario page.
            /// </summary>
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

            /// <summary>
            /// Update the data binding source on the ComboBox to reflect the contents of the vector.
            /// </summary>
            void UpdatePrinterList(Windows::Foundation::Collections::IVector<PrinterInfo^>^ printerList);

            /// <summary>
            /// Update the data binding source on the ComboBox to reflect the contents of the vector.
            /// </summary>
            void UpdatePrintJobList(Windows::Foundation::Collections::IVector<PrintJob^>^ jobVector);

            // Private methods and event handlers
        private:

            /// <summary>
            /// Event handler for clicks on the "Enumerate printers" button.
            /// </summary>
            void EnumeratePrinters_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            /// <summary>
            /// Event handler for clicks on the "Cancel print job" button.
            /// </summary>
            void CancelPrintJob_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            /// <summary>
            /// This method is invoked when a printer is selected on the UI.
            /// Displays the print jobs in the printer's queue.
            /// </summary>
            void Printer_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

            /// <summary>
            /// This method is invoked when a print job is selected on the UI.
            /// Displays the details of the selected print job.
            /// </summary>
            void PrintJob_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

            /// <summary>
            /// This creates an IPrinterQueue2 object, given a device id.
            /// </summary>
            static void CreatePrinterQueueObject(Platform::String^ deviceId, Microsoft::WRL::ComPtr<IPrinterQueue2>& printerQueue);

            /// <summary>
            /// This method begins display of the print queue view for the suppled IPrinterQueue2 object.
            /// </summary>
            void SetPrintQueueView(Microsoft::WRL::ComPtr<IPrinterQueue2>& printerQueue);

            /// <summary>
            /// This method resets the print queue view being displayed.
            /// </summary>
            void ResetPrintQueueView();

            // Private variables
        private:
            /// <summary>
            /// Holds the dispatcher object for the main UI thread.
            /// </summary>
            Windows::UI::Core::CoreDispatcher^ disp;

            /// <summary>
            /// Holds a reference to the print job event handler object, which calls back into the 
            /// scenario page to update the print job list.
            /// </summary>
            Microsoft::WRL::ComPtr<PrintJobEventHandler> printJobEventHandler;
        };

        /// <summary>
        /// Represents the event handler that is invoked when there is a change to the print queue.
        /// </summary>
        class PrintJobEventHandler : 
            public Microsoft::WRL::RuntimeClass<
                        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::ClassicCom>,
                        IPrinterQueueViewEvent,
                        IDispatch>
        {
            // Public constructor and methods
        public:
            PrintJobEventHandler(PrintJobManagement^ scenarioPage, Microsoft::WRL::ComPtr<IPrinterQueueView>& printerQueueView);

            ~PrintJobEventHandler() { }

            /// <summary>
            /// Starts the print job enumeration and calls back into the scenario page.
            /// </summary>
            void StartPrintJobEnumeration();

            /// <summary>
            /// Stops the print job enumeration and releases its reference to the scenario page.
            /// </summary>
            void StopPrintJobEnumeration();

#pragma region IPrinterQueueViewEvent members
            /// <summary>
            /// This callback method is invoked when print jobs are enumerated.
            /// </summary>
            HRESULT STDMETHODCALLTYPE OnChanged(
                _In_ IPrintJobCollection* collection,
                ULONG viewOffset,
                ULONG viewSize,
                ULONG countJobsInPrintQueue);
#pragma endregion

#pragma region IDispatch members

            /// <summary>
            /// It is sufficient to implement IDispatch::Invoke(). Other members of interface
            /// IDispatch need not be implemented for the event handler to receive events.
            /// </summary>
            virtual HRESULT STDMETHODCALLTYPE
            Invoke(
                DISPID id,
                _In_  REFIID,
                _In_  LCID,
                _In_  WORD ,
                _In_  DISPPARAMS * params,
                _Out_opt_  VARIANT *,
                _Out_opt_  EXCEPINFO *,
                _Out_opt_  UINT *);

            virtual HRESULT STDMETHODCALLTYPE
            GetTypeInfoCount(
                __RPC__out UINT *)
            {
                return E_NOTIMPL;
            }

            virtual HRESULT STDMETHODCALLTYPE
            GetTypeInfo(
                UINT,
                LCID,
                __RPC__deref_out_opt ITypeInfo **)
            {
                return E_NOTIMPL;
            }

            virtual HRESULT STDMETHODCALLTYPE
            GetIDsOfNames(
                __RPC__in REFIID,
                __RPC__in_ecount_full(cNames) LPOLESTR *,
                __RPC__in_range(0,16384) UINT cNames,
                LCID,
                __RPC__out_ecount_full(cNames) DISPID *)
            {
                return E_NOTIMPL;
            }
#pragma endregion

        private:
            /// <summary>
            /// This event handler holds a reference to the scenario page.
            /// This is used update the UI when print jobs are enumerated.
            /// </summary>
            PrintJobManagement^ scenarioPage;

            /// <summary>
            /// Convert the IPrintJobCollection object to a Vector<PrintJob^> object
            /// to facilitate data binding.
            /// </summary>
            Windows::Foundation::Collections::IVector<PrintJob^>^ GetPrintJobVector(_In_ IPrintJobCollection* collection);

            /// <summary>
            /// Represents the connection between the event handler and the object.
            /// </summary>
            Microsoft::WRL::ComPtr<IConnectionPoint> connectionPoint;
            DWORD connectionPointCookie;
        };

        /// <summary>
        /// Represents one print job, and exposes properties in a fashion suitable to data binding.
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        public ref class PrintJob sealed : Platform::Object
        {
            // Constructors and public methods
        public:
            PrintJob(Platform::Object^ iPrintJobComObject);

            /// <summary>
            /// Requests cancelation of the print job.
            /// </summary>
            void RequestCancel()
            {
                ThrowIfFailed(
                    printJob->RequestCancel());
            }

            // Print job properties
        public:
            /// <summary>
            /// Retrieves the job name (i.e. document name).
            /// </summary>
            property Platform::String^ Name
            {
                Platform::String^ get()
                {
                    return name;
                }
            }

            /// <summary>
            /// Retrieves the job id.
            /// </summary>
            property uint32 Id
            {
                uint32 get()
                {
                    return id;
                }
            }

            /// <summary>
            /// Retrieves the count of pages printed.
            /// </summary>
            property uint32 PrintedPages
            {
                uint32 get()
                {
                    return printedPages;
                }
            }

            /// <summary>
            /// Retrieves the total count of pages in the jobs.
            /// </summary>
            property uint32 TotalPages
            {
                uint32 get()
                {
                    return totalPages;
                }
            }

            /// <summary>
            /// Retrieves a displayable version of the print job status.
            /// </summary>
            property Platform::String^ Status
            {
                Platform::String^ get()
                {
                    return status;
                }
            }

            /// <summary>
            /// Retrieves the job submission time.
            /// </summary>
            property Platform::String^ SubmissionTime
            {
                Platform::String^ get()
                {
                    return submissionTime;
                }
            }

            // Private methods
        private:
            /// <summary>
            /// Converts the job status to a display string.
            /// </summary>
            void ConvertStatusToDisplayString(PrintJobStatus status);

            /// <summary>
            /// Converts the job submission time to a display string.
            /// </summary>
            void ConvertVariantTimeToDisplayString(DATE submissionTime);

            // Private variables
        private:
            /// <summary>
            /// Reference to the underlying IPrintJob object.
            /// </summary>
            Microsoft::WRL::ComPtr<IPrintJob> printJob;

            // Print job properties.
            Platform::String^ name;
            Platform::String^ status;
            Platform::String^ submissionTime;

            unsigned long id;
            unsigned long printedPages;
            unsigned long totalPages;
        };
    }
}
