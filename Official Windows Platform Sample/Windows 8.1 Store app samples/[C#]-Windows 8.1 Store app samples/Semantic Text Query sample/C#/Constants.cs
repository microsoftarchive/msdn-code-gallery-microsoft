//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using SemanticTextQuery;
using Windows.Data.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace SDKTemplate
{
    public partial class MainPage
    {
        public const string FEATURE_NAME = "SemanticTextQuery C# Sample";
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Find Scenario",                            ClassType = typeof(Scenario1) },
            new Scenario() { Title = "FindInProperty Scenario",                  ClassType = typeof(Scenario2) },
            new Scenario() { Title = "GetMatchingPropertiesWithRanges Scenario", ClassType = typeof(Scenario3) }
        };

        public void HighlightRanges(TextBlock tb, String TextContent, IReadOnlyList<TextSegment> ranges)
        {
            int currentPosition = 0;
            foreach (var range in ranges)
            {
                // Add the next chunk of non-range text
                if (range.StartPosition > currentPosition)
                {
                    int length = (int)range.StartPosition - currentPosition;
                    var subString = TextContent.Substring(currentPosition, length);
                    tb.Inlines.Add(new Run() { Text = subString });
                    currentPosition += length;
                }
                // Add the next range
                var boldString = TextContent.Substring((int)range.StartPosition, (int)range.Length);
                tb.Inlines.Add(new Run() { Text = boldString, FontWeight = FontWeights.Bold });
                currentPosition += (int)range.Length;
            }
            // Add the text after the last matching segment
            if (currentPosition < TextContent.Length)
            {
                var subString = TextContent.Substring(currentPosition);
                tb.Inlines.Add(new Run() { Text = subString });
            }
            tb.Inlines.Add(new Run() { Text = "\r\n" });
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
