//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2.g.h"

using namespace SDKSample;
using namespace SDKSample::BarcodeScannerCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::PointOfService;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;


namespace SDKSample
{
	namespace BarcodeScannerCPP
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Scenario2 sealed
		{
		public:
			Scenario2();

		private:
			MainPage^ rootPage;
			ClaimedBarcodeScanner^ claimedBarcodeScannerInstance1 ;
			ClaimedBarcodeScanner^ claimedBarcodeScannerInstance2 ;
			BarcodeScanner^ scannerInstance1;
			BarcodeScanner^ scannerInstance2;

			// tokens for instance1
			Windows::Foundation::EventRegistrationToken dataReceivedTokenInstance1;
			Windows::Foundation::EventRegistrationToken releaseDeviceRequestedTokenInstance1;

			// tokens for instance2
			Windows::Foundation::EventRegistrationToken dataReceivedTokenInstance2;
			Windows::Foundation::EventRegistrationToken releaseDeviceRequestedTokenInstance2;

			/// <summary>
			/// Enumerator for current active Instance.
			/// </summary>
			enum BarcodeScannerInstance
			{
				Instance1,
				Instance2
			};

			//utility functions for creating, enabling and disabling barcode scanner instances
			Concurrency::task<void> CreateDefaultScannerObject(BarcodeScannerInstance instance);
			Concurrency::task<void> ClaimScanner(BarcodeScannerInstance instance);
			Concurrency::task<void> EnableScanner(BarcodeScannerInstance instance);
			
			void DisableScanner(BarcodeScannerInstance instance);
			void DestroyScanner(BarcodeScannerInstance instance);

			//UI updates
			void UpdateOutput(Platform::String^ strMessage);
			void ResetUI();
			void SetUI(BarcodeScannerInstance instance);

			//Event handlers for the Scanner Instance1
			void ButtonStartScanningInstance1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ButtonEndScanningInstance1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OnDataReceivedInstance1(Windows::Devices::PointOfService::ClaimedBarcodeScanner ^sender, Windows::Devices::PointOfService::BarcodeScannerDataReceivedEventArgs ^args);
			void OnReleaseDeviceRequestedInstance1(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedBarcodeScanner ^args);

			//Event handlers for Scanner Instance2
			void ButtonStartScanningInstance2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ButtonEndScanningInstance2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OnDataReceivedInstance2(Windows::Devices::PointOfService::ClaimedBarcodeScanner ^sender, Windows::Devices::PointOfService::BarcodeScannerDataReceivedEventArgs ^args);
			void OnReleaseDeviceRequestedInstance2(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedBarcodeScanner ^args);

		};
	}
}
