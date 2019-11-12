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
    { "Encrypt and Decrypt", "SDKSample.CryptographyAndCertificate.EncryptDecrypt" },
    { "Sign and Verify Signature", "SDKSample.CryptographyAndCertificate.SignVerify" },
    { "Hash Algorithms", "SDKSample.CryptographyAndCertificate.Hashing" },
    { "Key Derivation", "SDKSample.CryptographyAndCertificate.KeyDerivation" },
    { "Enroll Certificate", "SDKSample.CryptographyAndCertificate.Enroll" }, 
    { "Import Pfx Certificate", "SDKSample.CryptographyAndCertificate.ImportPfx" },
    { "View and Use Certificate", "SDKSample.CryptographyAndCertificate.ViewCert" },
}; 
