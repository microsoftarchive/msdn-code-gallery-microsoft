//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "CryptographicBuffer", "SDKSample.CryptoWinRT.Scenario1" }, 
    { "Hash Algorithms", "SDKSample.CryptoWinRT.Scenario2" },
    { "Hmac Algorithms", "SDKSample.CryptoWinRT.Scenario3" },
    { "Key Derivation", "SDKSample.CryptoWinRT.Scenario4" },
    { "Cipher Algorithms", "SDKSample.CryptoWinRT.Scenario5" },
    { "Authenticated Encryption Algorithms", "SDKSample.CryptoWinRT.Scenario6" },
    { "Encrypt and Decrypt", "SDKSample.CryptoWinRT.Scenario7" },
    { "Sign and Verify Signature", "SDKSample.CryptoWinRT.Scenario8" },
    { "Asynchronous Data Protection", "SDKSample.CryptoWinRT.Scenario9" },
    { "Asynchronous Data Protection with Streams", "SDKSample.CryptoWinRT.Scenario10" }
}; 
