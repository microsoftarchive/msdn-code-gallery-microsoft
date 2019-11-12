//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SmartCardSample;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;

namespace SDKTemplate
{

public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
{
    // Change the string below to reflect the name of your sample.
    // This is used on the main page as the title of the sample.
    public const string FEATURE_NAME = "Smart Card C# Sample";

    public const int ADMIN_KEY_LENGTH_IN_BYTES = 24;

    // Change the array below to reflect the name of your scenarios.
    // This will be used to populate the list of scenarios on the main page with
    // which the user will choose the specific scenario that they are interested in.
    // These should be in the form: "Navigating to a web page".
    // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
    List<Scenario> scenarios = new List<Scenario>
    {
        new Scenario()
        {
            Title = "Create and provision a TPM virtual smart card",
            ClassType = typeof(Scenario1)
        },
        new Scenario()
        {
            Title = "Change smart card PIN",
            ClassType = typeof(Scenario2)
        },
        new Scenario()
        {
            Title = "Reset smart card PIN",
            ClassType = typeof(Scenario3)
        },
        new Scenario()
        {
            Title = "Change smart card admin key",
            ClassType = typeof(Scenario4)
        },
        new Scenario()
        {
            Title = "Verify response",
            ClassType = typeof(Scenario5)
        },
        new Scenario()
        {
            Title = "Delete TPM virtual smart card",
            ClassType = typeof(Scenario6)
        },
        new Scenario()
        {
            Title = "List all smart cards",
            ClassType = typeof(Scenario7) 
        },
    };

    public Windows.Storage.Streams.IBuffer AdminKey { get; set; }

    public String SmartCardReaderDeviceId { get; set; }

    public async Task<SmartCard> GetSmartCard()
    {
        SmartCardReader reader = await SmartCardReader.FromIdAsync(
            SmartCardReaderDeviceId);
        IReadOnlyList<SmartCard> cards = await reader.FindAllCardsAsync();

        if (1 != cards.Count)
        {
            throw new InvalidOperationException(
                "Reader has an unexpected number of cards (" +
                cards.Count + ")");
        }

        return cards[0];
    }

    public bool ValidateTPMSmartCard()
    {
        if (string.IsNullOrEmpty(SmartCardReaderDeviceId))
        {
            return false;
        }
        return true;
    }
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
