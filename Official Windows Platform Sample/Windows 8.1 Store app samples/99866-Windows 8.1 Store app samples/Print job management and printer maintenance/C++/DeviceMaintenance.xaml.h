//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// DeviceMaintenance.xaml.h
// Declaration of the DeviceMaintenance and BidiSetRequestCallback classes
//

#pragma once
#include "pch.h"
#include "DeviceMaintenance.g.h"
#include "PrinterEnumeration.h"
#include "PrinterExtension.h"

namespace SDKSample
{
    namespace DeviceAppForPrinters2
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class DeviceMaintenance sealed
        {
            // Public constructor and methods
        public:
            DeviceMaintenance();

            /// <summary>
            /// Update the Bidi response on the UI.
            /// </summary>
            void UpdateBidiResponse(Platform::String^ response);

            /// <summary>
            /// Update the data binding source on the ComboBox to reflect the contents of the vector.
            /// </summary>
            void UpdatePrinterList(Windows::Foundation::Collections::IVector<PrinterInfo^>^ printerList);

            /// <summary>
            /// This method is invoked when the user navigates to this scenario page.
            /// </summary>
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

            // Private methods
        private:
            /// <summary>
            /// Event handler for clicks on the "Enumerate printers" button.
            /// </summary>
            void EnumeratePrinters_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            /// <summary>
            /// Event handler for clicks on the "Send Bidi request" button.
            /// </summary>
            void SendBidiRequest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            /// <summary>
            /// This creates an IPrinterQueue2 object, given a device id.
            /// </summary>
            static void CreatePrinterQueueObject(Platform::String^ deviceId, Microsoft::WRL::ComPtr<IPrinterQueue2>& printerQueue);

            // Private members
        private:
            /// <summary>
            /// Holds the dispatcher object for the main UI thread.
            /// </summary>
            Windows::UI::Core::CoreDispatcher^ disp;
        };

        /// <summary>
        /// Represents the callback object that is invoked when a Bidi "Set" operation is completed.
        /// </summary>
        class BidiSetRequestCallback
            : public Microsoft::WRL::RuntimeClass<
                        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::ClassicCom>,
                        IPrinterBidiSetRequestCallback>
        {
            // Public constructor and methods
        public:
            BidiSetRequestCallback(DeviceAppForPrinters2::DeviceMaintenance^ scenarioPage)
            {
                this->scenarioPage = scenarioPage;
            }

            ~BidiSetRequestCallback() { }

            // Private methods
        private:
            /// <summary>
            /// This method is invoked when the asynchronous Bidi "Set" operation is completed.
            /// </summary>
            virtual HRESULT STDMETHODCALLTYPE Completed(
                _In_ BSTR bstrResponse,
                HRESULT hrStatus);

            // Private variables
        private:
            DeviceMaintenance^ scenarioPage;
        };
    }
}
