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

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Cryptography WinRT APIs";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "CryptographicBuffer", ClassType = typeof(CryptoWinRT.Scenario1) },
            new Scenario() { Title = "Hash Algorithms", ClassType = typeof(CryptoWinRT.Scenario2) },
            new Scenario() { Title = "Hmac Algorithms", ClassType = typeof(CryptoWinRT.Scenario3) },
            new Scenario() { Title = "Key Derivation", ClassType = typeof(CryptoWinRT.Scenario4) },
            new Scenario() { Title = "Cipher Algorithms", ClassType = typeof(CryptoWinRT.Scenario5) },
            new Scenario() { Title = "Authenticated Encryption Algorithms", ClassType = typeof(CryptoWinRT.Scenario6) },
            new Scenario() { Title = "Encrypt and Decrypt", ClassType = typeof(CryptoWinRT.Scenario7) },
            new Scenario() { Title = "Sign and Verify Signature", ClassType = typeof(CryptoWinRT.Scenario8) },
            new Scenario() { Title = "Asynchronous Data Protection", ClassType = typeof(CryptoWinRT.Scenario9) },
            new Scenario() { Title = "Asynchronous Data Protection with Streams", ClassType = typeof(CryptoWinRT.Scenario10) }
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
