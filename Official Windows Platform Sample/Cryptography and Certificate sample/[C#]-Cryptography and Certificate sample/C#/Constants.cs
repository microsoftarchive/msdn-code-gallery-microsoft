//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;
using CertificateEnrollment;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Cryptography and Certificate";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Encrypt and Decrypt", ClassType = typeof(CryptoWinRT.EncryptDecrypt) },
            new Scenario() { Title = "Sign and Verify Signature", ClassType = typeof(CryptoWinRT.SignVerify) },
            new Scenario() { Title = "Hash Algorithms", ClassType = typeof(CryptoWinRT.Hashing) },
            new Scenario() { Title = "Key Derivation", ClassType = typeof(CryptoWinRT.KeyDerivation) },
            new Scenario() { Title = "Enroll Certificate", ClassType = typeof(Enroll) },
            new Scenario() { Title = "Import Pfx Certificate", ClassType = typeof(ImportPfx) },
            new Scenario() { Title = "View and Use Certificate", ClassType = typeof(ViewCert) },
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
